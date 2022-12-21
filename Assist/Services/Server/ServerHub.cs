using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Serilog;

namespace Assist.Services.Server
{
    internal class ServerHub : HubClient
    {
        public event Action<object>? RecieveMessageEvent;

        public async Task Connect()
        {
            HubConnectionUrl = "https://api.assistapp.dev/hub";

            base.Init();
            
            _hubConnection.On<int?>("updateTotalUsers", Recieve_UpdateTotalUsers);
            
            await StartHubInternal();
        }

        public bool IsConnected => _hubConnection.State == HubConnectionState.Connected || _hubConnection.State == HubConnectionState.Connecting;

        public void Recieve_UpdateTotalUsers(int? users)
        {
            if (users != null) RecieveMessageEvent.Invoke(users);
            Log.Information("Got Updated User Count");
        }
        

        public void Disconnect()
        {
            
        }
    }
}
