using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Assist.Core.Helpers;
using Assist.Models.Socket;
using Assist.ViewModels;
using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using Microsoft.Win32;
using Serilog;

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
        }
        catch (Exception e)
        {
            Log.Error("Unhandled Ex Source: " + e.Source);
            Log.Error("Unhandled Ex StackTrace: " + e.StackTrace);
            Log.Error("Unhandled Ex Message: " + e.Message);
        }
            
        AssistApplication.RiotWebsocketService.UserPresenceMessageEvent += UpdateDiscordRpcWithDataFromPresence;
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

    private void DeterminePregamePresence(PlayerPresence pres)
    {
        
    }

    private void DetermineIngamePresence(PlayerPresence pres)
    {
        if (pres.partyState.Equals("MATCHMADE_GAME_STARTING") || string.IsNullOrEmpty(pres.matchMap))
            return;
        
        // Update Basic Party Details
        
    }

    private void DetermineMenusPresence(PlayerPresence pres)
    {
        switch (pres.partyState)
        {
            case "MATCHMAKING":
                _client.UpdateDetails($"Queueing: {ValorantHelper.DetermineQueueKey(pres.queueId)}"); 
                break;
            default:
                _client.UpdateDetails($"In Lobby");
                break;
        }
        
        // Handle Party
        _client.UpdateParty(new Party()
        {
            ID = pres.partyId,
            Max = pres.maxPartySize,
            Privacy = pres.partyAccessibility.Equals("CLOSED") ? Party.PrivacySetting.Private : Party.PrivacySetting.Public,
            Size = pres.partySize
        });

        HandleSecrets(pres.partyId); // Handle to see if players can enable launching.

        ResetTimer();
        
        _client.UpdateSmallAsset($"rank_{pres.competitiveTier}",
            ValorantHelper.RankNames[pres.competitiveTier]);
        
        _client.UpdateLargeAsset("default", "Updating...");
        
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

    private void HandleDiscordClient_OnJoin(object sender, JoinMessage args)
    {
        Log.Information("On Join Event sent from Discord");
    }
    
    private void HandleDiscordClient_OnJoinRequested(object sender, JoinRequestMessage args)
    {
        Log.Information("On Join Requested Event sent from Discord");
        Log.Information("Someone is requesting to join your game.");
        
        
    }
    
    public async Task Shutdown()
    {
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