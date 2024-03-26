using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Assist.Core.Helpers;
using Assist.Models.Enums;
using Assist.Models.Socket;
using Assist.Shared.Settings.Modules;
using Assist.ViewModels;
using Avalonia.Controls.Documents;
using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using Microsoft.Win32;
using Serilog;
using ValNet.Objects.Coregame;
using ValNet.Objects.Exceptions;
using ValNet.Objects.Local;

namespace Assist.Services.Modules;

public class RichPresenceService
{
    public static RichPresenceService Default = new RichPresenceService();
    #region Defaults

    private const string CLIENTID = "925134832453943336";
    private static Button[] _clientButtons =
    {
        new Button() { Label = "Download Assist", Url = "https://assistval.com/" }
    };
    private RichPresence _currentPresence;
    public bool BDiscordPresenceActive;
    public DateTime timeStart;
    #endregion

    private DiscordRpcClient? _client = new(CLIENTID, autoEvents: true);

    public void Initialize() 
    {
        Default._client.RegisterUriScheme(null,"assistdebug://");
        
        Default._client.Subscribe(EventType.Join);
        Default._client.Subscribe(EventType.JoinRequest);
        Default._client.OnJoin += HandleDiscordClient_OnJoin;
        Default._client.OnJoinRequested += HandleDiscordClient_OnJoinRequested;
        
        Default._client.OnReady += delegate(object sender, ReadyMessage args)
        {
            Log.Information("Discord Presence Client Ready, User: " + args.User.Username);
            timeStart = DateTime.Now;
        };
        Default._client.OnConnectionFailed += delegate(object sender, ConnectionFailedMessage args)
        {
            Log.Information("Discord Presence Client Failed to Connect to Discord, failed pipe number: " +
                            args.FailedPipe);
        };
        
        _currentPresence = new RichPresence
        {
            Buttons = _clientButtons,
            Assets = new Assets()
            {
                LargeImageKey = "default",
                LargeImageText = "Powered By Assist"
            },
            Party = new Party()
            {
                Max = 5,
                Size = 1
            },
            Secrets = null,
            State = "VALORANT",
        };

        Default._client.SetPresence(_currentPresence);
        try
        {
            Default._client.Initialize();
            BDiscordPresenceActive = true;

            ResetTimer();
            ForceUpdate();
        }
        catch (Exception e)
        {
            Log.Error("Unhandled Ex Source: " + e.Source);
            Log.Error("Unhandled Ex StackTrace: " + e.StackTrace);
            Log.Error("Unhandled Ex Message: " + e.Message);
        }
            
        AssistApplication.RiotWebsocketService.UserPresenceMessageEvent += UpdateDiscordRpcWithDataFromPresence;
    }

    public async void ForceUpdate()
    {
        if (!BDiscordPresenceActive)
            return;
        
        Log.Information("Forcing Discord RPC Update");
        var data = AssistApplication.RiotWebsocketService.GetLatestMessage();
        if (data is null)
        {
            try
            {
                var t = await AssistApplication.ActiveUser.Presence.GetPresences();
                var pres = t.presences.Find(x => x.puuid == AssistApplication.ActiveUser.UserData.sub);

                if (pres != null)
                {
                    var pp = await ValorantHelper.GetPresenceData(pres);
                    UpdateDiscordRpcWithDataFromPresence(pp);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            
            return;
        };
        UpdateDiscordRpcWithDataFromPresence(data);
    }
    
    private async void UpdateDiscordRpcWithDataFromPresence(PresenceV4Message obj)
    {
        // Decode Pres
        var pres = await ValorantHelper.GetPresenceData(obj.MessageData.Presences[0]);
        
        switch (pres.sessionLoopState)
        {
            case "MENUS":
                DetermineMenusPresence(pres);
                break;
            case "INGAME":
                DetermineIngamePresence(pres);
                break;
            case "PREGAME":
                DeterminePregamePresence(pres);
                break;
            default:
                break;
        }


    }
    
    private async void UpdateDiscordRpcWithDataFromPresence(PlayerPresence pres)
    {
        // Decode Pres
        
        
        switch (pres.sessionLoopState)
        {
            case "MENUS":
                DetermineMenusPresence(pres);
                break;
            case "INGAME":
                DetermineIngamePresence(pres);
                break;
            case "PREGAME":
                DeterminePregamePresence(pres);
                break;
            default:
                break;
        }


    }

    private void DeterminePregamePresence(PlayerPresence pres)
    {
        if (pres.partyState.Equals("MATCHMADE_GAME_STARTING") || string.IsNullOrEmpty(pres.matchMap))
            return;

        if (!_prevInGame)
        {
            ResetTimer();
            _prevInGame = true;
        }
        
        // Handle Party
        HandlePlayerParty(pres);

        HandleSmallImageData(pres);
        HandleLargeImageData(pres);
        HandleDetails(pres);

        _client.UpdateState("Agent Select");
    }

    private bool _prevInGame = false;
    private void DetermineIngamePresence(PlayerPresence pres)
    {
        if (!_prevInGame)
        {
            ResetTimer();
            _prevInGame = true;
        }
        
        // Handle Party
        HandlePlayerParty(pres);

        HandleSmallImageData(pres);
        HandleLargeImageData(pres);
        HandleDetails(pres);
    }

    private bool _prevInQueue = false;
    private void DetermineMenusPresence(PlayerPresence pres)
    {
        // Player isnt in a game anymore.
        _prevInGame = false;
        // Handle Party
        HandlePlayerParty(pres);
        
        switch (pres.partyState)
        {
            case "MATCHMAKING":
                if (ModuleSettings.Default.RichPresenceSettings.ShowMode)
                {
                    _client.UpdateDetails($"In Queue: {ValorantHelper.DetermineQueueKey(pres.queueId)}");
                    if (!_prevInQueue)
                        ResetTimer();

                    _prevInQueue = true;
                }
                break;
            default:
                _client.UpdateDetails($"In Lobby");
                _prevInQueue = false;
                
                
                
                break;
        }
        
        HandleSmallImageData(pres);
        HandleLargeImageData(pres);
        //HandleDetails(pres);
        _client.UpdateState("In Menus");
    }

    private void HandleDetails(PlayerPresence pres)
    {
        string detailsFinal = string.Empty;
        
        // First Determine Queue

        if (ModuleSettings.Default.RichPresenceSettings.ShowMode)
        {
            if (pres.partyState.Contains("CUSTOM_GAME"))
                detailsFinal += Properties.Resources.VALORANT_CustomGame;
            else if (pres.matchMap.Equals("/game/maps/poveglia/range", StringComparison.OrdinalIgnoreCase))
            {
                detailsFinal += Properties.Resources.VALORANT_TheRange;
            }
            else
            {
                var queueName = ValorantHelper.DetermineQueueKey(pres.queueId);
                detailsFinal += queueName;
            }
        }

        if (!string.IsNullOrEmpty(detailsFinal) && ModuleSettings.Default.RichPresenceSettings.ShowScore && !pres.matchMap.Equals("/game/maps/poveglia/range", StringComparison.OrdinalIgnoreCase)) // Adds Spacer if there is already data.
            detailsFinal += " | ";
        
        if (ModuleSettings.Default.RichPresenceSettings.ShowScore && !pres.matchMap.Equals("/game/maps/poveglia/range", StringComparison.OrdinalIgnoreCase))
        {
           detailsFinal += $"{pres.partyOwnerMatchScoreAllyTeam} - {pres.partyOwnerMatchScoreEnemyTeam}";
        }

        if (string.IsNullOrEmpty(detailsFinal))
        {
            detailsFinal = Properties.Resources.Common_Assist;
        }

        _client.UpdateDetails(detailsFinal);
    }

    private string _latestMatchId; 
    
    private async void HandleSmallImageData(PlayerPresence pres)
    {
        if (BDiscordPresenceActive)
        {
            switch (ModuleSettings.Default.RichPresenceSettings.SmallImageData)
            {
                case ERPDataType.RANK:
                    if (ModuleSettings.Default.RichPresenceSettings.ShowRank)
                    {
                        ValorantHelper.RankNames.TryGetValue(pres.competitiveTier, out string compName);
                        _client.UpdateSmallAsset($"rank_{pres.competitiveTier}", compName);   
                    }
                    else
                        _client.UpdateSmallAsset(string.Empty,null); 
                    break;
                case ERPDataType.LOGO:
                    _client.UpdateSmallAsset($"default", "Assist");
                    break;
                case ERPDataType.AGENT: // Agent is only showcased within INGAME, Not During Agent Select. 
                    if (ModuleSettings.Default.RichPresenceSettings.ShowAgent)
                    {
                        if (pres.sessionLoopState.Equals("INGAME", StringComparison.OrdinalIgnoreCase))
                        {
                            var data = await DisplayAgent(pres);
                            _client.UpdateSmallAsset(data.Key, data.Value);   
                        }
                        else
                            _client.UpdateSmallAsset(string.Empty,null); 
                    }
                    else
                        _client.UpdateSmallAsset(string.Empty,null); 
                    break;
                case ERPDataType.MAP:
                    if (!pres.sessionLoopState.Equals("MENUS", StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(pres.matchMap)) return;
                        ValorantHelper.MapsByPath.TryGetValue(pres.matchMap.ToLower(), out string MapName);
                        string code = MapName.ToLower();
                        if (pres.matchMap.Equals("/game/maps/poveglia/range", StringComparison.OrdinalIgnoreCase))
                            code = "range";
                        
                        _client.UpdateSmallAsset(code, MapName); 
                    }
                    else
                        _client.UpdateSmallAsset(string.Empty,null); 
                    break;
                case ERPDataType.DEFAULT:
                    if (pres.sessionLoopState.Equals("MENUS", StringComparison.OrdinalIgnoreCase))
                        _client.UpdateSmallAsset(string.Empty, null);
                    break;
                default:
                    _client.UpdateSmallAsset(string.Empty,null); 
                    break;
            }
        }
    }
    
    private async void HandleLargeImageData(PlayerPresence pres)
    {
        if (BDiscordPresenceActive)
        {
            switch (ModuleSettings.Default.RichPresenceSettings.LargeImageData)
            {
                case ERPDataType.RANK:
                    if (ModuleSettings.Default.RichPresenceSettings.ShowRank)
                    {
                        ValorantHelper.RankNames.TryGetValue(pres.competitiveTier, out string compName);
                        _client.UpdateLargeAsset($"rank_{pres.competitiveTier}", compName);   
                    }
                    else
                        _client.UpdateLargeAsset($"default", "Assist");
                    break;
                case ERPDataType.LOGO:
                    _client.UpdateLargeAsset($"default", "Assist");
                    break;
                case ERPDataType.AGENT: // Agent is only showcased within INGAME, Not During Agent Select. 
                    if (ModuleSettings.Default.RichPresenceSettings.ShowAgent)
                    {
                        if (pres.sessionLoopState.Equals("INGAME", StringComparison.OrdinalIgnoreCase))
                        {
                            var data = await DisplayAgent(pres);
                            _client.UpdateLargeAsset(data.Key, data.Value);   
                        }
                        else
                            _client.UpdateLargeAsset($"default", "Assist");
                    }
                    else
                        _client.UpdateLargeAsset($"default", "Assist");
                    break;
                case ERPDataType.MAP:
                    if (!pres.sessionLoopState.Equals("MENUS", StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(pres.matchMap)) return;
                        ValorantHelper.MapsByPath.TryGetValue(pres.matchMap.ToLower(), out string MapName);
                        string code = MapName.ToLower();
                        if (pres.matchMap.Equals("/game/maps/poveglia/range", StringComparison.OrdinalIgnoreCase))
                            code = "range";
                        
                        _client.UpdateLargeAsset(code, MapName); 
                    }
                    else
                        _client.UpdateLargeAsset($"default", "Assist");
                    break;
                case ERPDataType.DEFAULT:
                    if (pres.sessionLoopState.Equals("MENUS", StringComparison.OrdinalIgnoreCase))
                        _client.UpdateLargeAsset(null, null);
                    break;
                default:
                    _client.UpdateLargeAsset(null,null); 
                    break;
            }
        }
    }

    /// <summary>
    /// Returns the key, and Tooltip for the agent currently being played the client.
    /// </summary>
    /// <returns></returns>
    private async Task<KeyValuePair<string, string>> DisplayAgent(PlayerPresence pres)
    {
        var characterId = await GetAgent_Ingame();

        if (characterId is null)
            return new KeyValuePair<string, string>("unknown", Properties.Resources.Common_Unknown);

        ValorantHelper.AgentIdToNames.TryGetValue(characterId, out var charName);
        
        if (charName is null)
            return new KeyValuePair<string, string>("unknown", Properties.Resources.Common_Unknown);
        
        return new KeyValuePair<string, string>(characterId, charName);
    }
    
    /// <summary>
    /// Gets and Returns the Character ID of the Current Local Player INGAME
    /// </summary>
    /// <param name="retry"></param>
    /// <returns></returns>
    private async Task<string> GetAgent_Ingame(bool retry = false)
    {
        if (!await GetIngamePlayerMatch()) return null;
        
        if (string.IsNullOrEmpty(_latestMatchId))
            return null;
        
        CoregameMatch MatchResp = new CoregameMatch();
        try
        {
            MatchResp = await AssistApplication.ActiveUser.CoreGame.FetchMatch(_latestMatchId!);
            
            if (MatchResp == null)
            {
                Log.Error("Failed on getting COREGAME Match Data within the Discord RPC Module Service.");
                return null;
            }
        }
        catch (RequestException e)
        {
            Log.Fatal("Error on getting player match");
            Log.Fatal("DISCORDRPC_COREGAME ERROR: " + e.StatusCode);
            Log.Fatal("DISCORDRPC_COREGAME ERROR: " + e.Content);
            Log.Fatal("DISCORDRPC_COREGAME ERROR: " + e.Message);

            if (e.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Fatal("DISCORDRPC_COREGAME TOKEN ERROR: ");
                await AssistApplication.RefreshService.CurrentUserOnTokensExpired();
            }

            return null;
        }

        if (MatchResp.Players == null || MatchResp.Players.Count == 0)
            return null;
        
        var localPlayer =
            MatchResp.Players.Find(player => player.Subject == AssistApplication.ActiveUser.UserData.sub);

        if (localPlayer is null)
         return null;

        return localPlayer.CharacterID.ToLower();

    }
    private async Task<bool> GetIngamePlayerMatch()
    {
        try
        {
            var getPlayerResp = await AssistApplication.ActiveUser.CoreGame.FetchPlayer();
            if (getPlayerResp != null)
            {
                _latestMatchId = getPlayerResp.MatchID;
            }

            if (string.IsNullOrEmpty(_latestMatchId))
            {
                Log.Error("Failed to get MatchID");

                return false;
            }
        }
        catch (RequestException e)
        {
            Log.Fatal("Error on getting player coregame");
            Log.Fatal("COREGAME ERROR: " + e.StatusCode);
            Log.Fatal("COREGAME ERROR: " + e.Content);
            Log.Fatal("COREGAME ERROR: " + e.Message);

            if (e.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Fatal("TOKEN ERROR: " + e.Message);
                await AssistApplication.RefreshService.CurrentUserOnTokensExpired();
                return false;
            }
        }

        return true;
    }
    private void HandlePlayerParty(PlayerPresence pres)
    {
        if (ModuleSettings.Default.RichPresenceSettings.ShowParty)
        {
            if (!pres.sessionLoopState.Equals("MENUS"))
                _client.UpdateState($"In Party: ");
            
            _client.UpdateParty(new Party()
            {
                ID = pres.partyId,
                Max = pres.maxPartySize,
                Privacy = pres.partyAccessibility.Equals("CLOSED") ? Party.PrivacySetting.Private : Party.PrivacySetting.Public,
                Size = pres.partySize
            });

            if (ModuleSettings.Default.RichPresenceSettings.EnableDiscordInvite)
                HandleSecrets(pres.partyId); // Handle to see if players can enable launching.            
        }
    }
    private void ResetTimer()
    {
        _client.UpdateClearTime();
        _client.UpdateStartTime(DateTime.UtcNow);
    }

    public async void UpdatePresence(RichPresence data)
    {
        _currentPresence = data;
        // Send Data to Discord
        _client.SetPresence(data);
        _client.Invoke();
    }
    
    #region Discord Game Invites

    private void HandleDiscordClient_OnJoin(object sender, JoinMessage args)
    {
        Log.Information("On Join Event sent from Discord");
    }
    
    private void HandleDiscordClient_OnJoinRequested(object sender, JoinRequestMessage args)
    {
        Log.Information("On Join Requested Event sent from Discord");
        Log.Information("Someone is requesting to join your game.");
    }

    #endregion
    
    public async Task Shutdown()
    {
        if (!BDiscordPresenceActive)
        {
            Log.Information("Attempted to Shutdown Discord RPC while inactive.");
            return;
        }
        
        if (!_client.IsDisposed)
            _client.Dispose();

        if (_client.IsInitialized)
            _client.Deinitialize();

        _client = new DiscordRpcClient(CLIENTID);
        BDiscordPresenceActive = false;
    }
    
    public void Deinitialize() 
    {
        if (_client == null)return;
        _client.Dispose();
    }

    private string _lastPartyIdHandled = string.Empty;
    private void HandleSecrets(string partyId)
    {
        RegistryKey key = Registry.ClassesRoot.OpenSubKey("assistDebug");

        if (key != null && !string.Equals(_lastPartyIdHandled, partyId))
        {
            _client.UpdateButtons(null);
            _client.UpdateSecrets(new Secrets()
            {
                JoinSecret = partyId.Replace("-", "")
            });
            _lastPartyIdHandled = partyId;
        }
        else
            _client.UpdateSecrets(null);
    }
}