using System;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Controls.Global;
using Assist.Objects.AssistApi.Game;
using Assist.Services.Popup;
using Assist.Services.Server;
using Assist.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;
using Serilog;

namespace Assist.Game.Services;

public class AssistGameServerConnection : HubClient
{
    public event Action<object>? RecieveMessageEvent;
    private const string GAMESERVERURL = "https://api.assistapp.dev/game/main";

    public event Action<string?> LOBBY_InviteRequested;
    public event Action<string?> LOBBY_InviteSentFromCreator;
    
    public async Task Connect()
    {
        HubConnectionUrl = GAMESERVERURL;

        base.InitWithAuth(AssistApplication.Current.AssistUser.Tokens.AccessToken);

        _hubConnection.On<string>("inviteLobbyPlayerToParty", PartyInviteRequestedFromLobbyUser);
        _hubConnection.On<string>("inviteLobbyRecieved", PartyInviteSentFromCreator);
        await StartHubInternal();
    }

    private void PartyInviteSentFromCreator(string data)
    {
        LOBBY_InviteSentFromCreator?.Invoke(data);
    }

    private async void PartyInviteRequestedFromLobbyUser(string data)
    {
        LOBBY_InviteRequested?.Invoke(data);
    }
    
    public async Task RequestPartyInvite(RequestPartyJoin data)
    {
        Log.Information("Requesting new Party Invite Data From Server: " + data);
        // Request Invite from server Data Confirming that an invite was sent.
        await _hubConnection.SendAsync("requestPrivateLobbyInvite", JsonSerializer.Serialize(data));
    }
    
    public async Task PartyInviteSentFromAssist(InvitePlayerData data)
    {
        Log.Information("Recieved new InviteData Data From Server: " + data);
        // Send Server Data Confirming that an invite was sent.
        _hubConnection.SendAsync("confirmPrivateLobbyInvite", JsonSerializer.Serialize(data));
    }
}