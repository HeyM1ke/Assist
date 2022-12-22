using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
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
            if (e != null) RecieveMessageEvent.Invoke(e.Data);
        }

        public async Task Disconnect()
        {
            _socket.CloseAsync();
        }


    }
}
