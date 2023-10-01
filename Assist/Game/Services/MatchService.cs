using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Assist.Game.Models;
using Assist.Game.Views.Leagues;
using Assist.Objects.AssistApi.Game;
using Assist.Objects.RiotSocket;
using Assist.ViewModels;
using AssistUser.Lib.Leagues.Models;
using AssistUser.Lib.Leagues.Models.Server;
using AssistUser.Lib.Parties.Models;
using Avalonia.Threading;
using Serilog;
using ValNet.Objects.Coregame;
using ValNet.Objects.CustomGame;
using ValNet.Objects.Exceptions;
using ValNet.Objects.Local;
using ValNet.Objects.Pregame;

namespace Assist.Game.Services;

public class MatchService
{
    public static MatchService? Instance;
    public AssistMatch CurrentMatchData;
    private bool _currentlyBinded = false;

    private bool _awaitingInvite = false;
    private bool _waitingForMatchToStart = false;
    private bool _matchIsInSession = false;
    private bool _currentlyIngame = false;
    public bool CurrentlyIngame => _currentlyIngame;
    private bool _serverleftParty = false;
    private string _localMatchId ="";
    private string _lastRecordedSessionState = "";
    public MatchService()
    {
        if (Instance is not null)
        {
            Instance.UnbindToEvents();
            Instance = null;
        }
        
        if (Instance is null) 
            Instance = this;

        BindToEvents();
    }

    private void BindToEvents()
    {
        if (_currentlyBinded)
            return;
        
        AssistApplication.Current.GameServerConnection.MATCH_MatchUpdateMessageReceived += GameServerConnectionOnMATCH_MatchUpdateMessageReceived;
        AssistApplication.Current.GameServerConnection.MATCH_CustomGameSettingsReceived += GameServerConnectionOnMATCH_CustomGameSettingsReceived;
        AssistApplication.Current.GameServerConnection.MATCH_PartyInformationRequested += GameServerConnectionOnMATCH_PartyInformationRequested;
        AssistApplication.Current.GameServerConnection.MATCH_StartValorantMatchReceived += GameServerConnectionOnMATCH_StartValorantMatchReceived;
        AssistApplication.Current.GameServerConnection.MATCH_ValorantPartyJoinMatchReceived += GameServerConnectionOnMATCH_ValorantPartyJoinMatchReceived;
        AssistApplication.Current.GameServerConnection.MATCH_MatchValorantPartyCreateReceived += GameServerConnectionOnMATCH_MatchValorantPartyCreateReceived;
        
        
        AssistApplication.Current.RiotWebsocketService.UserPresenceMessageEvent += PlayerPresenceMessageReceived;
        AssistApplication.Current.RiotWebsocketService.PregameMessageEvent += PregameMessageReceived;
        AssistApplication.Current.RiotWebsocketService.CoregameMessageEvent += CoregameMessageReceived;
        _currentlyBinded = true;
    }
    
    public void UnbindToEvents()
    {
        AssistApplication.Current.GameServerConnection.MATCH_MatchUpdateMessageReceived -= GameServerConnectionOnMATCH_MatchUpdateMessageReceived;
        AssistApplication.Current.GameServerConnection.MATCH_CustomGameSettingsReceived -= GameServerConnectionOnMATCH_CustomGameSettingsReceived;
        AssistApplication.Current.GameServerConnection.MATCH_PartyInformationRequested -= GameServerConnectionOnMATCH_PartyInformationRequested;
        AssistApplication.Current.GameServerConnection.MATCH_StartValorantMatchReceived -= GameServerConnectionOnMATCH_StartValorantMatchReceived;
        AssistApplication.Current.GameServerConnection.MATCH_ValorantPartyJoinMatchReceived -= GameServerConnectionOnMATCH_ValorantPartyJoinMatchReceived;
        AssistApplication.Current.GameServerConnection.MATCH_MatchValorantPartyCreateReceived -= GameServerConnectionOnMATCH_MatchValorantPartyCreateReceived;
        
        AssistApplication.Current.RiotWebsocketService.UserPresenceMessageEvent -= PlayerPresenceMessageReceived;
        AssistApplication.Current.RiotWebsocketService.PregameMessageEvent -= PregameMessageReceived;
        AssistApplication.Current.RiotWebsocketService.CoregameMessageEvent -= CoregameMessageReceived;
        _currentlyBinded = false;
    }
    
    
    public async Task<AssistMatch> GetCurrentMatch()
    {
        var resp = await AssistApplication.Current.AssistUser.League.PREMATCH_GetUserMatch();

        if (resp.Code != 200)
        {
            Log.Error("CANNOT GET MATCH DATA ON MATCHSERVICE on PREMATCH");
            Log.Error(resp.Message);
        }
        else
        {
            CurrentMatchData = JsonSerializer.Deserialize<AssistMatch>(resp.Data.ToString());
            return CurrentMatchData; 
        }
        
        resp = await AssistApplication.Current.AssistUser.League.MATCH_GetUserMatch();

        if (resp.Code != 200)
        {
            Log.Error("CANNOT GET MATCH DATA ON MATCHSERVICE on MATCH");
            Log.Error(resp.Message);
            return new AssistMatch();
        }
        
        CurrentMatchData = JsonSerializer.Deserialize<AssistMatch>(resp.Data.ToString());
        return CurrentMatchData;
    }
    
    public static async Task QuitIngameValorantMatch(bool retry = false)
    {
        try
        {
            var resp = await AssistApplication.Current.CurrentUser.CoreGame.FetchPlayer();
            await AssistApplication.Current.CurrentUser.CoreGame.QuitMatch(resp.MatchID);
        }
        catch (RequestException e)
        {
            Console.WriteLine(e);
            if(e.StatusCode == HttpStatusCode.BadRequest){
                Log.Fatal("TOKEN ERROR: ");
                await AssistApplication.Current.RefreshService.CurrentUserOnTokensExpired();
                if (!retry)
                    await QuitIngameValorantMatch();
                return;
            }
        }
    }
    
    public static async Task UnreadyClient(bool retry = false)
    {
        try
        {
            await AssistApplication.Current.CurrentUser.Party.SetPartyReadiness(false);
        }
        catch (RequestException e)
        {
            Console.WriteLine(e);
            if(e.StatusCode == HttpStatusCode.BadRequest){
                Log.Fatal("TOKEN ERROR: ");
                await AssistApplication.Current.RefreshService.CurrentUserOnTokensExpired();
                if (!retry)
                    await UnreadyClient(true);
                return;
            }
        }
    }

    
    private async void PlayerPresenceMessageReceived(PresenceV4Message obj)
    {
        var playerPre = obj.data.presences.Find(x => x.puuid == AssistApplication.Current.CurrentUser.UserData.sub);
        var privatePlayerPres = await GetPresenceData(playerPre);

        if (string.IsNullOrEmpty(Instance?.CurrentMatchData.PregameDetails.ValorantPartyId))
        {
            _lastRecordedSessionState = privatePlayerPres.sessionLoopState;
            return;
        }
        
        if (privatePlayerPres.partyId != Instance.CurrentMatchData.PregameDetails.ValorantPartyId && !_serverleftParty)
        {
            Log.Error("Player is currently in the wrong party than the specified match party");
            if (privatePlayerPres.sessionLoopState.Equals("PREGAME", StringComparison.OrdinalIgnoreCase) || privatePlayerPres.sessionLoopState.Equals("INGAME", StringComparison.OrdinalIgnoreCase))
            {
                Log.Fatal("Player is in a match while being in the wrong party. This is not supposed to happen.");
            }
            _lastRecordedSessionState = privatePlayerPres.sessionLoopState;
            return;
        }
        
        Log.Information("Player is in the right party.");

        
        
        if (!privatePlayerPres.sessionLoopState.Equals("MENUS", StringComparison.OrdinalIgnoreCase))
        {
            Log.Information("Player is in a match while being in the right party.");
            Log.Information("Checking Match data to see if the correct players are in the match.");

            if (privatePlayerPres.sessionLoopState.Equals("INGAME") || MatchService.Instance.CurrentMatchData.SpecialState.Equals("REQUESTED_GAMESTART", StringComparison.OrdinalIgnoreCase))
            {
                Log.Information("Already Determined if the player was in the match.");

                switch (privatePlayerPres.sessionLoopState)
                {
                    case "INGAME":
                        await HandleCoregameMatchData();
                        break;
                }
                
                _lastRecordedSessionState = privatePlayerPres.sessionLoopState;
                return;
            }
            

           
        }

        if (privatePlayerPres.sessionLoopState.Equals("MENUS", StringComparison.OrdinalIgnoreCase) && CurrentMatchData.State.Equals("INGAME", StringComparison.OrdinalIgnoreCase))
        {
            if (_lastRecordedSessionState.Equals("INGAME"))
            {
                Instance.UnbindToEvents();
                AssistApplication.CurrentlyInAssistLeagueMatch = false;
                Log.Information("Showing Results Page.");
                // Move to results page and pass the matchID
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    GameViewNavigationController.Change(new PostMatchResultView(Instance.CurrentMatchData.Id));
                });
            }
        }
        
        
        
        
        _lastRecordedSessionState = privatePlayerPres.sessionLoopState;
    }

    private async void GetLocalPlayerMatchDetails_Pregame()
    {
        try
        {
            var getPlayerResp = await AssistApplication.Current.CurrentUser.Pregame.GetPlayer();
            if (getPlayerResp != null)
            {
                _localMatchId = getPlayerResp.MatchID;
            }

            if (string.IsNullOrEmpty(_localMatchId))
            {
                Log.Error("Failed to get MatchID");
                return;
            }
        }
        catch (RequestException e)
        {
            Log.Fatal("Error on getting player pregame");
            Log.Fatal("PREGAME ERROR: " + e.StatusCode);
            Log.Fatal("PREGAME ERROR: " + e.Content);
            Log.Fatal("PREGAME ERROR: " + e.Message);
                
            if(e.StatusCode == HttpStatusCode.BadRequest){
                Log.Fatal("TOKEN ERROR: ");
                AssistApplication.Current.RefreshService.CurrentUserOnTokensExpired();
                return;
            }
        }
    }
    
    private async void GetLocalPlayerMatchDetails_Coregame()
    {
        try
        {
            var getPlayerResp = await AssistApplication.Current.CurrentUser.CoreGame.FetchPlayer();
            if (getPlayerResp != null)
            {
                _localMatchId = getPlayerResp.MatchID;
            }

            if (string.IsNullOrEmpty(_localMatchId))
            {
                Log.Error("(MatchService) Failed to get MatchID");
                return;
            }
        }
        catch (RequestException e)
        {
            Log.Fatal("(MatchService) Error on getting player coregame fetching matchid");
            Log.Fatal("(MatchService) COREGAME FETCH MATCHID ERROR: " + e.StatusCode);
            Log.Fatal("(MatchService) COREGAME FETCH MATCHID ERROR: " + e.Content);
            Log.Fatal("(MatchService) COREGAME FETCH MATCHID ERROR: " + e.Message);
                
            if(e.StatusCode == HttpStatusCode.BadRequest){
                Log.Fatal("(MatchService) TOKEN ERROR: ");
                return;
            }
        }
    }
    
    private async Task HandlePregameMatchData(bool retry = false)
    {
        if (string.IsNullOrEmpty(_localMatchId))
        {
            GetLocalPlayerMatchDetails_Pregame();
        }
        
        PregameMatch MatchResp = new PregameMatch();
        ChatV4PresenceObj PresenceResp = new ChatV4PresenceObj();
        try
        {
            MatchResp = await AssistApplication.Current.CurrentUser.Pregame.GetMatch(_localMatchId);
            PresenceResp = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
            if (MatchResp == null || PresenceResp == null)
            {
                Log.Error("(MatchService) Failed on getting update data. Match or Presence");
                return;
            }
        }
        catch (RequestException e)
        {
            Log.Fatal("(MatchService) Error on getting player match or Pres");
            Log.Fatal("(MatchService) PREGAME ERROR: " + e.StatusCode);
            Log.Fatal("(MatchService) PREGAME ERROR: " + e.Content);
            Log.Fatal("(MatchService) PREGAME ERROR: " + e.Message);
                
            if(e.StatusCode == HttpStatusCode.BadRequest){
                Log.Fatal("(MatchService) PREGAME TOKEN ERROR: ");
                if (!retry)
                    await AssistApplication.Current.RefreshService.CurrentUserOnTokensExpired();
            }
            return;
        }
        
        
        if(MatchResp.AllyTeam == null)
            return;
        
        var localPlayer =
                PresenceResp.presences.Find(pres =>
                    pres.puuid.Equals(AssistApplication.Current.CurrentUser.UserData.sub, StringComparison.OrdinalIgnoreCase));
        
            var teamIds = MatchResp.AllyTeam.Players.Select(p => p.Subject).ToList();

            var allTeamPresences =
                PresenceResp.presences.FindAll(pres =>
                    teamIds.Contains(pres.puuid)); // Contains every presence from the team.
            
            Log.Information("(MatchService) Checking if all players on the team are on the Match Team");
            var possPlayer = MatchService.Instance.CurrentMatchData.TeamOne.Players.Find(x =>
                x.Id == AssistApplication.Current.AssistUser.Account.AccountInfo.id);
            List<AssistMatchPlayer> teamMates = new List<AssistMatchPlayer>();
            if (possPlayer is null)
                teamMates = MatchService.Instance.CurrentMatchData.TeamTwo.Players;
            else
                teamMates = MatchService.Instance.CurrentMatchData.TeamOne.Players;


            int teamMatesCorrect = 0;
            foreach (var teamMate in teamMates)
            {
                if (teamIds.Contains(teamMate.ValorantId))
                {
                    Log.Information($"(MatchService) Team Contains Teammate: {teamMate.Username}");
                    teamMatesCorrect += 1;
                }
                else
                {
                    Log.Information($"(MatchService) Teammate MISSING: {teamMate.Username}");
                    Log.Information($"(MatchService) Teammate MISSING: {teamMate.Username}");
                    Log.Information($"(MatchService) Teammate MISSING: {teamMate.Username}");
                }
            }


            if (teamMatesCorrect >= (MatchService.Instance.CurrentMatchData.TeamOne.Players.Count * .8))
            {
                Log.Information("(MatchService) TeamMates are correct on the team.");
                _currentlyIngame = true;
                await SendUpdate(new AssistMatchUpdate()
                {
                    UserData = localPlayer.Private,
                    PregameData = ConvertTo64(MatchResp)
                });
            }
    }


    private async void PregameMessageReceived(object obj)
    {
        Log.Information("(MatchService) Client Received a Pregame Event Update on MatchService");
        await HandlePregameMatchData();
    }

    private async void CoregameMessageReceived(object obj)
    {
        Log.Information("(MatchService) Client Received a CoreGame Event Update on MatchService");
        await HandleCoregameMatchData();
    }

    private async Task HandleCoregameMatchData(bool retry = false)
    {
        if (string.IsNullOrEmpty(_localMatchId))
        {
            GetLocalPlayerMatchDetails_Coregame();
        }
        
        CoregameMatch MatchResp = new CoregameMatch();
        ChatV4PresenceObj PresenceResp = new ChatV4PresenceObj();
        try
        {
            MatchResp = await AssistApplication.Current.CurrentUser.CoreGame.FetchMatch(_localMatchId);
            PresenceResp = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
            if (MatchResp == null || PresenceResp == null)
            {
                Log.Error("(MatchService) Failed on getting update data. Match or Presence");
                return;
            }
        }
        catch (RequestException e)
        {
            Log.Fatal("(MatchService) Error on getting player match or Pres");
            Log.Fatal("(MatchService) COREGAME ERROR: " + e.StatusCode);
            Log.Fatal("(MatchService) COREGAME ERROR: " + e.Content);
            Log.Fatal("(MatchService) COREGAME ERROR: " + e.Message);
                
            if(e.StatusCode == HttpStatusCode.BadRequest){
                Log.Fatal("(MatchService) COREGAME TOKEN ERROR: ");
                await AssistApplication.Current.RefreshService.CurrentUserOnTokensExpired();
                if (!retry) await HandleCoregameMatchData();
            }
            return;
        }

        var localPlayer =
            PresenceResp.presences.Find(pres =>
                pres.puuid.Equals(AssistApplication.Current.CurrentUser.UserData.sub, StringComparison.OrdinalIgnoreCase));

        var privatePlayerPres = await GetPresenceData(localPlayer);
        
        if (Instance.CurrentMatchData.State == "INGAME" || Instance.CurrentMatchData.SpecialState.Equals("REQUESTED_GAMESTART", StringComparison.OrdinalIgnoreCase) && privatePlayerPres.sessionLoopState.Equals("INGAME", StringComparison.OrdinalIgnoreCase))
        {
            Log.Information("(MatchService) Match Current State is INGAME");
            await SendUpdate(new AssistMatchUpdate()
            {
                UserData = localPlayer.Private,
                MatchData = ConvertTo64(MatchResp)
            });
        }
        
    }


    #region Assist Event Handlers

    private void GameServerConnectionOnMATCH_MatchUpdateMessageReceived(string? matchDataReceived)
    {
        try
        {
            CurrentMatchData = JsonSerializer.Deserialize<AssistMatch>(matchDataReceived);
        }
        catch (Exception e)
        {
            Log.Error("(MatchService) Failed to Parse Match Data from Server");
            Log.Error($"(MatchService) Exception: {e.Message}");
            Log.Error($"(MatchService) Stack: {e.StackTrace}");
        }
    }
    
    private void GameServerConnectionOnMATCH_PartyInformationRequested(object? obj)
    {
        
    }

    private async void GameServerConnectionOnMATCH_MatchValorantPartyCreateReceived(string? customGameSettings)
    {
        Log.Information("(MatchService) Party Create has been received from the server on Event Binding.");
        Log.Information("(MatchService) Party Create settings : ." + customGameSettings);
        await HandlePartyCreateReceived(customGameSettings);
    }

    private async Task HandlePartyCreateReceived(string? customGameSettings, bool retry = false)
    {
         Log.Information("(MatchService) Party Create has been received from the server.");
        
        // Check if the player is ingame.
        // If the player is INGAME, Leave the game. 

        Log.Information("(MatchService) Attempting to get Presence from Local Socket");
        var currPres = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
        var clientPres = currPres.presences.Find(x => x.puuid == AssistApplication.Current.CurrentUser.UserData.sub);
        var decodedPres = await GetPresenceData(clientPres);
        Log.Information("(MatchService) Checking if player is inGAmE");
        if (decodedPres.sessionLoopState.Equals("INGAME", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                await QuitIngameValorantMatch();
            }
            catch (Exception e)
            {
            }
        }
        
        
        try
        {
            if (!decodedPres.isPartyOwner || decodedPres.partySize != 1)
            {
                Log.Information("Leaving Current Party");
                await AssistApplication.Current.CurrentUser.Party.LeaveParty(decodedPres.partyId);
            }

            Log.Information("Making Party Open");
            await AssistApplication.Current.CurrentUser.Party.SetPartyAccessibility(true);
            Log.Information("Creating the custom game");
            await AssistApplication.Current.CurrentUser.CustomGame.MakeCustomGame();
            
            Log.Information("Decoding the Settings from the server");
            var customData = JsonSerializer.Deserialize<AssistMatchValorantSettings>(customGameSettings);
            var data = new
            {
                Map = customData.Map,
                Mode = customData.GameMode,
                GamePod = customData.Server,
                GameRules = new Dictionary<string, string>()
                {
                    {"TournamentMode",customData.UseTournamentMode ? "true" : "false"},
                    {"UseCheats",customData.UseCheats ? "true" : "false"},
                    {"IsOvertimeWinByTwo",customData.WinByTwo ? "true" : "false"}
                }
            };

            Log.Information("Settings the Custom Settings");
            var t = await AssistApplication.Current.CurrentUser.CustomGame.SetCustomGameSettings(data);
            Log.Information("Sending Server Details.");
            await AssistApplication.Current.AssistUser.League.PREMATCH_CaptainSetup(CurrentMatchData.Id, t);
        }
        catch(Exception e)
        {
            Log.Error("Failed to setup Custom Game");
            Log.Error($"Exception: {e.Message}");
            Log.Error($"Stack: {e.StackTrace}");
            if (e is RequestException)
            {
                var ex = e as RequestException;
                Log.Error($"Exception: {ex.Content}");
                Log.Error($"Stack: {ex.Message}");
                if(ex.StatusCode == HttpStatusCode.BadRequest){
                    Log.Fatal("TOKEN ERROR: ");
                    await AssistApplication.Current.RefreshService.CurrentUserOnTokensExpired();
                    if (!retry)
                        await HandlePartyCreateReceived(customGameSettings, true);   
                    return;
                }
            }
        }
    }

    private async void GameServerConnectionOnMATCH_CustomGameSettingsReceived(string? customGamesSettings)
    {
        await HandleCustomgameSettingsReceived(customGamesSettings);
    }

    private async Task HandleCustomgameSettingsReceived(string? customGamesSettings, bool retry = false)
    {
        Log.Information("Custom Game Settings Received from the Server.");
        
        try
        {
            var customData = JsonSerializer.Deserialize<AssistMatchValorantSettings>(customGamesSettings);
            var data = new
            {
                Map = customData.Map,
                Mode = customData.GameMode,
                GamePod = customData.Server,
                GameRules = new Dictionary<string, string>()
                {
                    {"TournamentMode",customData.UseTournamentMode ? "true" : "false"},
                    {"UseCheats",customData.UseCheats ? "true" : "false"},
                    {"IsOvertimeWinByTwo",customData.WinByTwo ? "true" : "false"}
                }
            };

            var t = await AssistApplication.Current.CurrentUser.CustomGame.SetCustomGameSettings(data);
        }
        catch (Exception e)
        {
            Log.Error("Failed to set Custom Games Data");
            Log.Error($"Exception: {e.Message}");
            Log.Error($"Stack: {e.StackTrace}");
            
            var ex = e as RequestException;
            if(ex.StatusCode == HttpStatusCode.BadRequest){
                Log.Fatal("TOKEN ERROR: ");
                await AssistApplication.Current.RefreshService.CurrentUserOnTokensExpired();
                
                if (!retry)
                    await HandleCustomgameSettingsReceived(customGamesSettings, true);   
                
                return;
            }
        }
    }
    
    
    private async void GameServerConnectionOnMATCH_StartValorantMatchReceived(string? startMatchReceiveData)
    {
        await HandleStartValorantMatchReceived(startMatchReceiveData);
    }

    private async Task HandleStartValorantMatchReceived(string? startMatchReceiveData, bool retry =false)
    {
         var customData = JsonSerializer.Deserialize<AssistMatchValorantSettings>(startMatchReceiveData);
        /*AssistApplication.Current.CurrentUser.CustomGame.
        // Start the custom game, Check players to make sure they are on the right team.
        for (int i = 0; i < CurrentMatchData.TeamOne.Players.Count; i++)
        {
            var player = CurrentMatchData.TeamOne.Players[i];
            
        }*/

        var vData = new
        {
            Map = customData.Map,
            Mode = customData.GameMode,
            GamePod = customData.Server,
            GameRules = new Dictionary<string, string>()
            {
                {"TournamentMode",customData.UseTournamentMode ? "true" : "false"},
                {"UseCheats",customData.UseCheats ? "true" : "false"},
                {"IsOvertimeWinByTwo",customData.WinByTwo ? "true" : "false"}
            }
        };
        
        try
        {
           var t = await AssistApplication.Current.CurrentUser.CustomGame.SetCustomGameSettings(vData);
           if (!VerifyPlayerTeams(t))
           {
               Log.Error("Teams are not correct.");
           }

           await AssistApplication.Current.CurrentUser.CustomGame.StartCustomGame();
           //TODO: Tell Server Match Has Started With Match Data.
           _matchIsInSession = true;
        }
        catch (Exception e)
        {
            Log.Error("Attempted to Start the game but failed.");
            Log.Error("Attempted to Start the game but failed. MESSAGE: " + e.Message);
            Log.Error("Attempted to Start the game but failed. STACK: " + e.StackTrace);
            if (e is RequestException)
            {
                var d = e as RequestException;
                Log.Error("Attempted to Start the game but failed. CONTENT: " + d.Content);
                
                if(d.StatusCode == HttpStatusCode.BadRequest){
                    Log.Fatal("TOKEN ERROR: ");
                    await AssistApplication.Current.RefreshService.CurrentUserOnTokensExpired();
                    
                    if (!retry)
                        await HandleStartValorantMatchReceived(startMatchReceiveData, true);   
                    
                    return;
                }
            }
        }
    }

    private async void GameServerConnectionOnMATCH_ValorantPartyJoinMatchReceived(string? valPartyData)
    {
        Log.Information("Party Join Request recieved from the server.");
        
        var partyData = JsonSerializer.Deserialize<JoinValorantParty>(valPartyData);
        
        // Check if the player is ingame.
            // If the player is INGAME, Leave the game. 

        var currPres = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
        var clientPres = currPres.presences.Find(x => x.puuid == AssistApplication.Current.CurrentUser.UserData.sub);
        var decodedPres = await GetPresenceData(clientPres);

        if (decodedPres.sessionLoopState.Equals("INGAME", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                await QuitIngameValorantMatch();
            }
            catch (Exception e)
            {
            }
        }
        
        // Join the Party.

        try
        {
            Log.Information("Attempting to Join Party of id " + partyData.ValorantPartyId);
            await AssistApplication.Current.CurrentUser.Party.JoinParty(partyData.ValorantPartyId);
        }
        catch (Exception e)
        {
            Log.Error("Failed to join Valorant Party for Match.");
            Log.Error("Failed to join Valorant Party for Match. MESSAGE " + e.Message);
            Log.Error("Failed to join Valorant Party for Match. STACK " + e.StackTrace);
            
            // If party is denied. 
            // Request separate invite from server. Return.
            
            // Report to server it was a failed join. With Username Data of current Client to receive invite.
            _awaitingInvite = true;
            // This is sent to the server which will confirm if the player id that request is apart of the match.
            // if so then the player will get sent an invite.
            var reqObj = new RequestPartyInviteMatch()
            {
                CurrentGameName = clientPres.game_name,
                CurrentTag = clientPres.game_tag,
                MatchId = CurrentMatchData.Id
            };
            await AssistApplication.Current.GameServerConnection.RequestPartyInviteForMatch(reqObj);
            return;
        }

        // If Party Join is successful, Unready user.
        await AssistApplication.Current.CurrentUser.Party.SetPartyReadiness(false);
        // Swap user to spectate, then to Team id sent.
        bool onCorrectTeam = false;
        while (!onCorrectTeam)
        {
            try
            {
                await AssistApplication.Current.CurrentUser.CustomGame.ChangeTeam("TeamSpectate");
                var cData=  await AssistApplication.Current.CurrentUser.CustomGame.ChangeTeam(partyData.TeamId);

                switch (partyData.TeamId)
                {
                    case "TeamOne":
                        var possibleUser = cData.CustomGameLobby.Membership.teamOne.Find(x =>
                            x.Subject == AssistApplication.Current.CurrentUser.UserData.sub);
                        if (possibleUser is not null)
                            onCorrectTeam = true;
                        break;
                    case "TeamTwo":
                        var possibleUserT = cData.CustomGameLobby.Membership.teamTwo.Find(x =>
                            x.Subject == AssistApplication.Current.CurrentUser.UserData.sub);
                        if (possibleUserT is not null)
                            onCorrectTeam = true;
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error("Error on Swapping Teams.");
                if (e is RequestException)
                {
                    var ex = e as RequestException;
                    Log.Error(ex.Content);
                    Log.Error(ex.Content);
                }
                
                await Task.Delay(3000);
            }
        }
        
        await AssistApplication.Current.CurrentUser.Party.SetPartyReadiness(false);
        // Watch Custom Game Updates, Make sure we stick to the right team. 
        // Make sure player does not go into game, Watch Pres. Make sure they stick to the party.
        
        

        
        
        // Subscribe to Presence Updates to Watch Player Movements. 
        AssistApplication.Current.RiotWebsocketService.UserPresenceMessageEvent += WatchUserPresenceWhileWaiting;
    }

    private void WatchUserPresenceWhileWaiting(PresenceV4Message obj)
    {
        
    }
    
    #endregion

    #region Private Methods
    

    public async Task<PlayerPresence> GetPresenceData(ChatV4PresenceObj.Presence data)
    {
        if (string.IsNullOrEmpty(data.Private))
            return new PlayerPresence();
        byte[] stringData = Convert.FromBase64String(data.Private);
        string decodedString = Encoding.UTF8.GetString(stringData);
        return JsonSerializer.Deserialize<PlayerPresence>(decodedString);
    }

    private bool VerifyPlayerTeams(CustomGameData customGameData)
    {
        return true;
    }
    
    private async Task SendUpdate(AssistMatchUpdate updateObj)
    {
        while (isLocked);
        isLocked = true;
        var resp = await AssistApplication.Current.AssistUser.League.MATCH_Update(Instance.CurrentMatchData.Id,
            updateObj);
        
        Log.Information("Send Update Information to Match");
        Log.Information($"Send Update Information to Match : {resp.Code}");
        Log.Information($"Send Update Information to Match : {resp?.Message}");
        isLocked = false;
    }

    public bool isLocked { get; set; } = false;

    private string ConvertTo64(object ob)
    {
        var json = JsonSerializer.Serialize(ob);
        var byteA = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(byteA);
    }
    #endregion
}