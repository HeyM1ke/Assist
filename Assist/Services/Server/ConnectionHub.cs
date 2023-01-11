using Microsoft.AspNetCore.SignalR.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assist.Services.Server
{
    internal class ConnectionHub : HubClient
    {
        public event Action<object>? RecieveMessageEvent;
        public event Action<string>? RedirectCodeEvent; 
        public async Task Connect()
        {
            HubConnectionUrl = "https://api.assistapp.dev/dev/authentication";
            base.Init();

            _hubConnection.On<string?>("onConnected", OnConnection_Message);
            _hubConnection.On<string>("redirectCode", RedirectCode_Message);
            await StartHubInternal();
        }

        private void RedirectCode_Message(string obj)
        {
            if (obj != null) RedirectCodeEvent.Invoke(obj);
            Log.Information("Got Redirect Code Message");
        }

        public bool IsConnected => _hubConnection.State == HubConnectionState.Connected || _hubConnection.State == HubConnectionState.Connecting;

        public void OnConnection_Message(string? users)
        {
            if (users != null) RecieveMessageEvent.Invoke(users);
            Log.Information("Got Updated User Count");
        }


        public void Disconnect()
        {
            CloseHub();
        }
    }
}
