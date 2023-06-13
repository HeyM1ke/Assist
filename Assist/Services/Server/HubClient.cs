using System;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;
using Microsoft.AspNetCore.SignalR.Client;
using Serilog;


namespace Assist.Services.Server
{
    public class HubClient
    {
        protected HubConnection _hubConnection;
        public string HubConnectionUrl { get; set; }
        public bool IsConnected => _hubConnection.ConnectionId != null;
        public string? ConnectionId => _hubConnection.ConnectionId;
        protected void Init()
        {
            _hubConnection = new HubConnectionBuilder().WithUrl(HubConnectionUrl).WithAutomaticReconnect().Build();

            _hubConnection.Reconnected += _hubConnection_Reconnected;
            _hubConnection.Reconnecting += _hubConnection_Reconnecting;
            _hubConnection.Closed += _hubConnection_Closed;
        }
        
        protected void InitWithAuth(string accessToken)
        {
            _hubConnection = new HubConnectionBuilder().WithUrl(HubConnectionUrl, settings =>
            {
                settings.AccessTokenProvider = () => Task.FromResult(accessToken);
            }).WithAutomaticReconnect().Build();

            _hubConnection.Reconnected += _hubConnection_Reconnected;
            _hubConnection.Reconnecting += _hubConnection_Reconnecting;
            _hubConnection.Closed += _hubConnection_Closed;
        }
        public async void CloseHub()
        {
            await _hubConnection.StopAsync();
        }

        protected async Task StartHubInternal()
        {
            try
            {
                await _hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message + " " + ex.StackTrace);
            }

        }

        private Task _hubConnection_Closed(Exception? arg)
        {
            Log.Information("_hubConnection_Closed New State:" + _hubConnection.State);
            return Task.CompletedTask;
            if (arg != null)
            {
                Log.Error($"Connection closed due to an error: {arg}");
                Log.Error($"Connection closed due to an error: {arg.Message}");
            }
            return null;
        }

        Task _hubConnection_Reconnecting(Exception? exception)
        {
            Log.Information("_hubConnection_Reconnecting New State:" + _hubConnection.State + " " + _hubConnection.ConnectionId);
            if (exception != null)
            {
                Log.Error($"Connection started reconnecting due to an error: {exception}");
                Log.Error($"Connection started reconnecting due to an error: {exception.Message}");
            }
            return null;
        }

        Task _hubConnection_Reconnected(string? s)
        {
            Log.Information("_hubConnection_Reconnected New State:" + _hubConnection.State + " " + _hubConnection.ConnectionId);
            if (s != null)
            {
                Log.Information($"Connection successfully reconnected: {s}");
            }

            return null;
        }

       

    }
}
