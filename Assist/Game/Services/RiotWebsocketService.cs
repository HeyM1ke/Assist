using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Objects.RiotSocket;
using Assist.ViewModels;
using Serilog;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace Assist.Game.Services
{
    internal class RiotWebsocketService
    {
        private WebSocket _socket;

        public event Action<object>? RecieveMessageEvent;
        public event Action<PresenceV4Message>? PresenceMessageEvent;
        public event Action<PresenceV4Message>? UserPresenceMessageEvent;
        public event Action<object>? PregameMessageEvent;
        public event Action<object>? OnErrorEvent;

        public async Task Connect()
        {
            if (AssistApplication.Current.CurrentUser == null)
            {
                return;
            }

            var userLockfile = AssistApplication.Current.CurrentUser.LockfileData;
            _socket = new WebSocket($"wss://127.0.0.1:{userLockfile.Port}/", "wamp");
            _socket.SetCredentials("riot", userLockfile.Password, true);
            _socket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
            _socket.SslConfiguration.ServerCertificateValidationCallback = delegate { return true; };
            _socket.Connect();
            _socket.Send("[5, \"OnJsonApiEvent\"]");

            _socket.OnMessage += _socket_OnMessage;
            _socket.OnError += _socket_OnError;
        }

        private void _socket_OnError(object? sender, ErrorEventArgs e)
        {
            if (e != null) OnErrorEvent.Invoke(e.Message);

            Log.Fatal("Websocket Error:");
            Log.Fatal(e.Exception.Message);
            Log.Fatal(e.Exception.StackTrace);
        }

        private void _socket_OnMessage(object? sender, MessageEventArgs e)
        {
            var t = JsonSerializer.Deserialize<object[]>(e.Data);
            if (e != null)
                RecieveMessageEvent.Invoke(t[2]);

            DetermineCustomEvent(t[2]);
        }

        private void OnPresenceMessageEvent(object data)
        {
            var presData = JsonSerializer.Deserialize<PresenceV4Message>(data.ToString());

            PresenceMessageEvent?.Invoke(presData);

            if(presData.data.presences[0].puuid == AssistApplication.Current.CurrentUser.UserData.sub)
                UserPresenceMessageEvent?.Invoke(presData);

        }

        private void OnPregameMessageEvent(object data)
        {
            PregameMessageEvent?.Invoke(data);
        }

        private void DetermineCustomEvent(object data)
        {
            var d = JsonSerializer.Deserialize<DefaultSocketDataMessage>(data.ToString());

            switch (d.uri)
            {
                case "/chat/v4/presences":
                    OnPresenceMessageEvent(data);
                    break;
                case { } uri when uri.Contains("ares-pregame/pregame/v1/"):
                    OnPregameMessageEvent(data);
                    break;
            }
        }

        public async Task Disconnect()
        {
            _socket.CloseAsync();
        }


    }
}
