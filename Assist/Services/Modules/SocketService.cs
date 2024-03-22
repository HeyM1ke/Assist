using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Core.Helpers;
using Assist.Models.Game;
using Assist.Models.Socket;
using Assist.Services.Riot;
using Assist.Shared.Models.Modules.Socket;
using Assist.ViewModels;
using Assist.ViewModels.Game;
using Serilog;
using ValNet.Objects.Coregame;
using ValNet.Objects.Exceptions;
using ValNet.Objects.Pregame;
using WebSocketSharp;

namespace Assist.Services.Modules;

public class SocketService
{
    public static SocketService Instance = new SocketService();
    
    public WebSocket Websocket;

    public Action OnConnect;
    public Action OnDisconnect;
    
    public bool IsConnected = false;
    private string wbsUrl = string.Empty;
    private string _sessionId = string.Empty;
    private CoregameMatch? _latestCoregameMatchData = null;
    private PregameMatch? _latestPregameMatchData = null;
    private Dictionary<string, PlayerInformation> _playerInformations = new Dictionary<string, PlayerInformation>();
    private Dictionary<string, int> _matchScores = new Dictionary<string, int>();
    public void Connect(string url)
    {
        wbsUrl = url;
        try
        {
            ConnectToSocket(wbsUrl);
        }
        catch (Exception e)
        {
            throw e;
        }
        
        SubscribeToEvent();
        
        if (string.IsNullOrEmpty(_sessionId)) // This makes it so when a reconnect happens it doesnt override the current session
            CreateNewSession();
    }
    
    public void Disconnect()
    {
        UnSubscribeToEvent();
        Websocket.Close();
    }

    public void CreateNewSession()
    {
        Log.Information("Player Requested to start new Session");

        _sessionId = Guid.NewGuid().ToString();
        _playerInformations = new Dictionary<string, PlayerInformation>();
        _latestPregameMatchData = null;
        _latestCoregameMatchData = null;


        SendMessage(ESocketMessageType.SESSIONINFO, _sessionId);
        
    }

    
    JsonSerializerOptions serializeOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    private void SendMessage(ESocketMessageType sessioninfo, object data)
    {
        var enumName = Enum.GetName(typeof(ESocketMessageType), sessioninfo);
        var content = new DefaultMessage()
        {
            Type = enumName,
            Data = data
        };

        if (Websocket.IsAlive)
            Websocket.Send(JsonSerializer.Serialize(content, serializeOptions));
    }

    private void ConnectToSocket(string address)
    {
        Websocket = new WebSocket(address);
        Websocket.OnOpen += (sender, args) =>
        {
            Log.Information("Socket is connected");
            IsConnected = true;
            OnConnect?.Invoke();
        };
        
        Websocket.Connect();
        
        Websocket.OnClose += (sender, args) =>
        {
            Log.Information("Socket Service: Disconnected.");
            IsConnected = false;
            OnDisconnect?.Invoke();
        };
    }

    private void SubscribeToEvent()
    {
        AssistApplication.RiotWebsocketService.RecieveMessageEvent -= ValorantSocket_OnMessageEvent;
        AssistApplication.RiotWebsocketService.RecieveMessageEvent += ValorantSocket_OnMessageEvent;
    }
    
    private void UnSubscribeToEvent()
    {
        AssistApplication.RiotWebsocketService.RecieveMessageEvent -= ValorantSocket_OnMessageEvent;
        
    }

    private void ValorantSocket_OnMessageEvent(object obj)
    {
        Log.Information("Socket Message Received");
        Log.Information(obj.ToString());
        
        DetermineCustomEvent(obj);
    }

    private void DetermineCustomEvent(object data)
    {
        var d = JsonSerializer.Deserialize<ValorantWebsocketClient.DefaultSocketDataMessage>(data.ToString());
        //Websocket.Send(JsonSerializer.Serialize(data));
        switch (d.uri)
        {
            case "/chat/v4/presences":
                HandlePresenceMessage(data);
                break;
            case { } uri when uri.Contains("ares-pregame/pregame/v1/"):
                HandlePregameMessage(data);
                break;
            case { } uri when uri.Contains("ares-core-game/core-game/v1/"):
                HandleCoregameMessage(data);
                break;
        }
    }

    private async void HandleCoregameMessage(object data)
    {
        if (string.IsNullOrEmpty(_currentMatchId))
        {
            var t = await GetPlayerCoregameMatch();
            Log.Information($"CoregameMessage: Received Player Party {t}");
        }

        var gameInfo = await GetCoregameMatchData(_currentMatchId);

        if (gameInfo is null)
        {
            Log.Information("CoregameMatch failed to get info. Skipping Update");
            return;
        }
        
        
        var message = await ConvertToSocketMessage(gameInfo);
        
        SendMessage(ESocketMessageType.MATCHUPDATE, message);

    }

    private async void HandlePregameMessage(object data)
    {
        if (string.IsNullOrEmpty(_currentMatchId))
        {
            var t = await GetPlayerPregameMatch();
            Log.Information($"Pregame Message: Received Player Game? {t}");
        }

        var gameInfo = await GetPregameMatchData(_currentMatchId);

        if (gameInfo is null)
        {
            Log.Information("Pregame match failed to get info. Skipping Update");
            return;
        }

        var message = await ConvertToSocketMessage(gameInfo);
        
        SendMessage(ESocketMessageType.MATCHUPDATE, message);
    }

    
    private async void HandlePresenceMessage(object data)
    {
        var presData = JsonSerializer.Deserialize<PresenceV4Message>(data.ToString());
        
        //TODO: See if the message is from a player that is within the match.

        if (presData.MessageData.Presences[0] is null)
            return;

        var id = presData.MessageData.Presences[0].puuid;
        var pres = await ValorantHelper.GetPresenceData(presData.MessageData.Presences[0]);
        var inMatch = _playerInformations.TryGetValue(id, out var pInfo);

        if (inMatch)
        {
            var exists = _matchScores.TryGetValue(pInfo.TeamId, out int curr);
            if (exists)
            {
                _matchScores[pInfo.TeamId] = pres.partyOwnerMatchScoreAllyTeam > curr
                    ? pres.partyOwnerMatchScoreAllyTeam
                    : curr;
            }
            else
            {
                _matchScores.TryAdd(pInfo.TeamId, pres.partyOwnerMatchScoreAllyTeam);
            }

            //Handle Enemy Score
            
            switch (pInfo.TeamId)
            {
                case "Red":
                    exists = _matchScores.TryGetValue("Blue", out var redScore);
                    if (exists)
                        _matchScores["Blue"] = pres.partyOwnerMatchScoreEnemyTeam > redScore
                            ? pres.partyOwnerMatchScoreEnemyTeam
                            : redScore;
                    else
                        _matchScores.TryAdd("Blue", pres.partyOwnerMatchScoreEnemyTeam);
                    break;
                case "Blue":
                    exists = _matchScores.TryGetValue("Red", out var blueScore);
                    if (exists)
                        _matchScores["Red"] = pres.partyOwnerMatchScoreEnemyTeam > blueScore
                            ? pres.partyOwnerMatchScoreEnemyTeam
                            : blueScore;
                    else
                        _matchScores.TryAdd("Red", pres.partyOwnerMatchScoreEnemyTeam);
                    break;
                default:
                    break;
            }
            
            SendMessage(ESocketMessageType.SCOREUPDATE, _matchScores);
        }
    }


    private string _currentMatchId = string.Empty;

    #region Socket Helper Messages

    internal async Task<MatchUpdateMessage> ConvertToSocketMessage(CoregameMatch match)
    {
        MatchUpdateMessage msg = new MatchUpdateMessage();
        msg.MatchId = match.MatchID;
        
        msg.ServerInformation.GamePodId = match.GamePodID;
        msg.ServerInformation.LocalizedServerName = ValorantHelper.Servers.TryGetValue(match.GamePodID.ToLower(), out string lclSrvName) ? lclSrvName : "Unknown";

        msg.MapInformation.MapPath = match.MapID;
        msg.MapInformation.LocalizedMapName = ValorantHelper.MapsByPath.TryGetValue(match.GamePodID.ToLower(), out string lclMapName) ? lclMapName : "Unknown";

        msg.MatchState = match.State.ToLower();
        msg.ModeInformation.GamemodePath = match.ModeID;
        msg.ModeInformation.LocalizedGamemodeName = "Unknown";

        msg.LatestTeamScores = _matchScores;

        for (int i = 0; i < match.Players.Count; i++)
        {
            var ply = match.Players[i];
            var existing = _playerInformations.TryGetValue(ply.Subject, out var playerInformation);

            if (!existing)
                playerInformation = await CreatePlayerInfo(ply);
            else
                playerInformation = await UpdatePlayerInfo(ply);
            
           msg.Players.Add(playerInformation);
            
        }

        return msg;
    }
    
    internal async Task<MatchUpdateMessage> ConvertToSocketMessage(PregameMatch match)
    {
        MatchUpdateMessage msg = new MatchUpdateMessage();
        msg.MatchId = match.ID;
        
        msg.ServerInformation.GamePodId = match.GamePodID;
        msg.ServerInformation.LocalizedServerName = ValorantHelper.Servers.TryGetValue(match.GamePodID.ToLower(), out string lclSrvName) ? lclSrvName : "Unknown";

        msg.MapInformation.MapPath = match.MapID;
        msg.MapInformation.LocalizedMapName = ValorantHelper.MapsByPath.TryGetValue(match.MapID.ToLower(), out string lclMapName) ? lclMapName : "Unknown";

        msg.MatchState = match.PregameState.ToLower();
        msg.ModeInformation.GamemodePath = match.Mode;
        msg.ModeInformation.LocalizedGamemodeName = "Unknown";
        
        
        msg.LatestTeamScores = _matchScores;

        for (int i = 0; i < match.Teams.Length; i++)
        {
            var team = match.Teams[i];

            for (int j = 0; j < team.Players.Length; j++)
            {
                var ply = team.Players[j];
                var existing = _playerInformations.TryGetValue(ply.Subject, out var playerInformation);

                if (!existing)
                    playerInformation = await CreatePlayerInfo(ply, team.TeamID);
                else
                {
                    playerInformation = await UpdatePlayerInfo(ply, team.TeamID);
                }
                    
                
                
                if (playerInformation == null)
                {
                    Log.Error("Failed to get this players information.");
                    continue;
                }
                msg.Players.Add(playerInformation);
            }

        }

        return msg;
    }
    
    private async Task<PlayerInformation?> CreatePlayerInfo(PregameMatch.Player player, string teamId)
    {
        var pInfo = await GetPlayerInfo(player.Subject);

        var info = new PlayerInformation()
        {
            PlayerId = player.Subject,
            Personalization = new PlayerPersonalization()
            {
                Gamename = pInfo.GameName,
                Tagline = pInfo.Tagline
            },
            TeamId = teamId,
            SelectedAgentId = player.CharacterID,
            IsLocked = player.CharacterSelectionState.Equals("LOCKED", StringComparison.OrdinalIgnoreCase) // Player is Pregame, Check for locked state.
        };
        
        try
        {
            _playerInformations.TryAdd(player.Subject, info);
        }
        catch (Exception e)
        {
            
        }

        return info;
    }
    
    private async Task<PlayerInformation?> UpdatePlayerInfo(PregameMatch.Player player, string teamId)
    {
        _playerInformations.TryGetValue(player.Subject, out var playerInformation);

        bool agentChanged = !player.CharacterID.Equals(playerInformation.SelectedAgentId);
        var lockChanged = playerInformation.IsLocked != player.CharacterSelectionState.Equals("LOCKED", StringComparison.OrdinalIgnoreCase);
        
        
        // Update Fields
        playerInformation.SelectedAgentId = player.CharacterID;
        playerInformation.IsLocked = player.CharacterSelectionState.Equals("LOCKED", StringComparison.OrdinalIgnoreCase);

        // Replace
        _playerInformations[player.Subject] = playerInformation;

        if (agentChanged || lockChanged) 
            SendMessage(ESocketMessageType.AGENTUPDATE, playerInformation);
        
        return playerInformation;
    }
    
    internal async Task<PlayerInformation> CreatePlayerInfo(CoregameMatch.Player player)
    {
        var pInfo = await GetPlayerInfo(player.Subject);

        var info =  new PlayerInformation()
        {
            PlayerId = player.Subject,
            Personalization = new PlayerPersonalization()
            {
                Gamename = pInfo.GameName,
                Tagline = pInfo.Tagline
            },
            TeamId = player.TeamID,
            SelectedAgentId = player.CharacterID,
            IsLocked = true // Player is INGAME, agent is locked. Can change due to cheats.
        };

        try
        {
            _playerInformations.TryAdd(player.Subject, info);
        }
        catch (Exception e)
        {
            
        }

        return info;
    }
    
    internal async Task<PlayerInformation> UpdatePlayerInfo(CoregameMatch.Player player)
    {
        _playerInformations.TryGetValue(player.Subject, out var playerInformation);
        playerInformation.SelectedAgentId = player.CharacterID;
        playerInformation.IsLocked = true;

        _playerInformations[player.Subject] = playerInformation;

        return playerInformation;
    }

    #endregion
    
    #region Helper Methods

    private async Task<ValorantPlayerStorage> GetPlayerInfo(string _playerId)
    {
        if (LiveViewViewModel.ValorantPlayers.TryGetValue(_playerId, out var possiblePlayerData))
        {
            if (possiblePlayerData.IsOld())
                await possiblePlayerData.Setup();

            return possiblePlayerData;
        }

        // If it doesnt exist create and set, and add to dictionary.
        var ply = new ValorantPlayerStorage(_playerId);
        await ply.Setup();
        try
        {
            LiveViewViewModel.ValorantPlayers.TryAdd(_playerId, ply);
        }
        catch (Exception e)
        {
            
        }

        return ply;
    }
    
    private async Task<bool> GetPlayerPregameMatch()
    {
        try
        {
            var getPlayerResp = await AssistApplication.ActiveUser.Pregame.GetPlayer();
            if (getPlayerResp != null)
            {
                _currentMatchId = getPlayerResp.MatchID;
            }

            if (string.IsNullOrEmpty(_currentMatchId))
            {
                Log.Error("Failed to get MatchID");

                return false;
            }
        }
        catch (RequestException e)
        {
            Log.Fatal("SocketService: Error on getting Player Pregame Match");
            Log.Fatal("SocketService:  ERROR: " + e.StatusCode);
            Log.Fatal("SocketService:  ERROR: " + e.Content);
            Log.Fatal("SocketService:  ERROR: " + e.Message);

            if (e.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Fatal("SocketService: TOKEN ERROR: " + e.Message);
                await AssistApplication.RefreshService.CurrentUserOnTokensExpired();
                await this.GetPlayerPregameMatch();
                return false;
            }
        }

        return true;
    }

    public async Task<PregameMatch?> GetPregameMatchData(string matchId)
    {
        try
        {
            if (string.IsNullOrEmpty(matchId))
                await GetPlayerPregameMatch();
            
            _latestPregameMatchData = await AssistApplication.ActiveUser.Pregame.GetMatch(matchId);
        }
        catch (RequestException e)
        {
            Log.Fatal("SocketService: Error on getting Pregame Matchdata");
            Log.Fatal("SocketService:  ERROR: " + e.StatusCode);
            Log.Fatal("SocketService:  ERROR: " + e.Content);
            Log.Fatal("SocketService:  ERROR: " + e.Message);

            if (e.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Fatal("SocketService: TOKEN ERROR: " + e.Message);
                await AssistApplication.RefreshService.CurrentUserOnTokensExpired();
            }
        }

        return _latestPregameMatchData;
    }
    public async Task<bool> GetPlayerCoregameMatch()
    {
        try
        {
            var getPlayerResp = await AssistApplication.ActiveUser.CoreGame.FetchPlayer();
            if (getPlayerResp != null)
            {
                _currentMatchId = getPlayerResp.MatchID;
            }

            if (string.IsNullOrEmpty(_currentMatchId))
            {
                Log.Error("Failed to get MatchID");

                return false;
            }
        }
        catch (RequestException e)
        {
            Log.Fatal("SocketService: Error on getting Player Match");
            Log.Fatal("SocketService:  ERROR: " + e.StatusCode);
            Log.Fatal("SocketService:  ERROR: " + e.Content);
            Log.Fatal("SocketService:  ERROR: " + e.Message);

            if (e.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Fatal("SocketService: TOKEN ERROR: " + e.Message);
                await AssistApplication.RefreshService.CurrentUserOnTokensExpired();
                await this.GetPlayerCoregameMatch();
                return false;
            }
        }

        return true;
    }
    
    public async Task<CoregameMatch?> GetCoregameMatchData(string matchId)
    {
        try
        {
            if (string.IsNullOrEmpty(matchId))
                await GetPlayerCoregameMatch();
            
            _latestCoregameMatchData = await AssistApplication.ActiveUser.CoreGame.FetchMatch(matchId);
        }
        catch (RequestException e)
        {
            Log.Fatal("SocketService: Error on getting Coregame Matchdata");
            Log.Fatal("SocketService:  ERROR: " + e.StatusCode);
            Log.Fatal("SocketService:  ERROR: " + e.Content);
            Log.Fatal("SocketService:  ERROR: " + e.Message);

            if (e.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Fatal("SocketService: TOKEN ERROR: " + e.Message);
                await AssistApplication.RefreshService.CurrentUserOnTokensExpired();
            }
        }

        return _latestCoregameMatchData;
    }

    #endregion
}