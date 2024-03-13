using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Models.Enums;
using Assist.Models.Socket;
using Assist.Services.Modules;
using Assist.ViewModels;
using Assist.Views.Game;
using Assist.Views.Startup;
using Avalonia.Threading;
using Serilog;
using WebSocketSharp;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

namespace Assist.Services.Riot;


public class ValorantWebsocketClient
{
    private const string RWS_SUBTOEVENTS = "[5, \"OnJsonApiEvent\"]";
    private LockfileData _currentLockfileData;
    public WebSocket ClientSocket;

    private PresenceV4Message? _latestUserPresResponse;
    #region Events

    public event Action<object>? RecieveMessageEvent;
    public event Action<PresenceV4Message>? PresenceMessageEvent;
    public event Action<PresenceV4Message>? UserPresenceMessageEvent;
    public event Action<object>? PregameMessageEvent;
    public event Action<object>? CoregameMessageEvent;
    public event Action<object>? OnErrorEvent;

    #endregion


    public async Task Connect()
    {
        try
        {
            Log.Information("Finding/Parsing Lockfile");
            FindLockfile();
        }
        catch (Exception e)
        {
            Log.Error("Error on Lockfile");
            Log.Error("Error Message:" + e.Message);
            return;
        }
        
        Log.Information("Success Finding/Parsing Lockfile");
        Log.Information("Creating Websocket");
        
        ClientSocket = new WebSocket($"wss://127.0.0.1:{_currentLockfileData.Port}/", "wamp");
        
        Log.Information("Applying Auth to Websocket");
        
        ClientSocket.SetCredentials("riot", _currentLockfileData.Password, true);
        
        if (Environment.OSVersion.Version.Major < 10)
        {
            Log.Information("OS older than Win 10, Socket needs to use TLS12 and Callbacks to true");
            ClientSocket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
            ClientSocket.SslConfiguration.ServerCertificateValidationCallback = delegate { return true; };
        }
        
        Log.Information("Connecting to Socket");
        ClientSocket.Connect();

        if (ClientSocket.IsAlive)
        {
            Log.Information("Socket is Alive");
        }
        
        ClientSocket.Send(RWS_SUBTOEVENTS);

        Log.Information("Subscribing to Socket Events");
        SubscribeToEvents();
    }

    public PresenceV4Message? GetLatestMessage() => _latestUserPresResponse;
    private void SubscribeToEvents()
    {
        ClientSocket.OnMessage += ClientSocketOnOnMessage;
        ClientSocket.OnError += ClientSocketOnOnError;
        ClientSocket.OnClose += ClientSocketOnOnClose;
    }

    private async void ClientSocketOnOnClose(object? sender, CloseEventArgs e)
    {
        Log.Information("Client Socket has Closed, Moving back to Launcher");
        await RiotClientService.CloseRiotRelatedPrograms();

        await RichPresenceService.Default.Shutdown();

        if (AssistApplication.ActiveUser is null)
        {
            Dispatcher.UIThread.InvokeAsync(() => AssistApplication.ChangeMainWindowView(new StartupView()));
            return;
        }
        
        Dispatcher.UIThread.InvokeAsync(() => AssistApplication.ChangeMainWindowView(new StartupView(AssistApplication.ActiveUser.UserData.sub)));
    }

    private void ClientSocketOnOnError(object? sender, ErrorEventArgs e)
    {
        if (e != null) OnErrorEvent?.Invoke(e.Message);

        Log.Fatal("Websocket Error:");
        Log.Fatal(e.Exception.Message);
        Log.Fatal(e.Exception.StackTrace);
    }

    private void ClientSocketOnOnMessage(object? sender, MessageEventArgs e)
    {
        var t = JsonSerializer.Deserialize<object[]>(e.Data);
        if (e != null)
            RecieveMessageEvent?.Invoke(t[2]);

        DetermineCustomEvent(t[2]);
    }
    
    private void OnPresenceMessageEvent(object data)
    {
        var presData = JsonSerializer.Deserialize<PresenceV4Message>(data.ToString());

        PresenceMessageEvent?.Invoke(presData);

        if (presData.MessageData.Presences[0].puuid == AssistApplication.ActiveUser.UserData.sub)
        {
            UserPresenceMessageEvent?.Invoke(presData);
            _latestUserPresResponse = presData;
        }
            

    }

    private void OnPregameMessageEvent(object data)
    {
        PregameMessageEvent?.Invoke(data);
    }
    private void OnCoregameMessageEvent(object data)
    {
        CoregameMessageEvent?.Invoke(data);
    }
    
    private void DetermineCustomEvent(object data)
    {
        var d = JsonSerializer.Deserialize<DefaultSocketDataMessage>(data.ToString());

#if DEBUG
        
#endif
        
        switch (d.uri)
        {
            case "/chat/v4/presences":
                OnPresenceMessageEvent(data);
                break;
            case { } uri when uri.Contains("ares-pregame/pregame/v1/"):
                OnPregameMessageEvent(data);
                break;
            case { } uri when uri.Contains("ares-core-game/core-game/v1/"):
                OnCoregameMessageEvent(data);
                break;
        }
    }

    private void FindLockfile()
    {
        var lockfileLocation = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Riot Games\Riot Client\Config\lockfile";
        var lockfileBetaLocation = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Riot Games\Beta\Config\lockfile";
        if (!File.Exists(lockfileLocation) && !File.Exists(lockfileBetaLocation))
            throw new Exception("Lockfile not Found");

        _currentLockfileData = ParseLockfile();
    }
    
    LockfileData ParseLockfile()
    {
        var _lockfileData = new LockfileData();
        string lockfileLocation = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Riot Games\Riot Client\Config\lockfile";

        if (!File.Exists(lockfileLocation))
        {
            var lockfileBetaLocation = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Riot Games\Beta\Config\lockfile";
                
            if (File.Exists(lockfileBetaLocation))
            {
                lockfileLocation = lockfileBetaLocation;
            }
            else
            {
                throw new Exception("How the fuck did you get here lmao? No Local File Detected");
            }
        }
            
        using (FileStream fileStream = new FileStream(lockfileLocation, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
        using (StreamReader sr = new StreamReader(fileStream))
        {
            string[] parts = sr.ReadToEnd().Split(':');
            _lockfileData.ProcessName = parts[0];
            _lockfileData.ProcessId = parts[1];
            _lockfileData.Port = parts[2];
            _lockfileData.Password = parts[3];
            _lockfileData.Protocol = parts[4];
        }
        return _lockfileData;
    }
    
    public struct LockfileData
    {
        public string ProcessName;
        public string ProcessId;
        public string Port;
        public string Password;
        public string Protocol;
    }
    
    internal class DefaultSocketDataMessage
    {
        public string eventType { get; set; }
        public string uri { get; set; }
        
    }
}