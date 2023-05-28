using System;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Controls.Global;
using Assist.Objects.AssistApi.Game;
using Assist.Objects.AssistApi.Server;
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
    public event Action<string?> GLOBALCHAT_MessageReceived; 
    public async Task Connect()
    {
        HubConnectionUrl = GAMESERVERURL;

        base.InitWithAuth(AssistApplication.Current.AssistUser.userTokens.AccessToken);

        _hubConnection.On<string>("inviteLobbyPlayerToParty", PartyInviteRequestedFromLobbyUser);
        _hubConnection.On<string>("inviteLobbyRecieved", PartyInviteSentFromCreator);
        _hubConnection.On<string>("recieveGlobalChatMessage", GlobalChatMessageReceived);
        _hubConnection.On<string>("receiveLeaguePartyInformation", PartyUpdateReceived);
        _hubConnection.On<string>("receiveLeaguePartyKickedUpdate", PartyKickReceived);
        _hubConnection.On<string?>("receiveInQueueMessage", InQueueMessageReceived);
        _hubConnection.On<string?>("receiveLeaveQueueMessage", LeaveQueueMessageReceived);
        _hubConnection.On<string?>("receiveMatchUpdateMessage", MatchUpdateMessageReceived);
        _hubConnection.On<string?>("receiveJoinedMatchMessage", JoinedMatchMessageReceived);

        await StartHubInternal();
    }

    private async void GlobalChatMessageReceived(string data)
    {
        GLOBALCHAT_MessageReceived?.Invoke(data);
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

    public async Task SendGlobalChatMessage(string messageText)
    {
        Log.Information("Sending a Message to the global chat.");

        // Create data model
        var data = new SendServerChatMessage()
        {
            Message = messageText,
            TimeSent = DateTime.Now,
            UserId = AssistApplication.Current.AssistUser.Account.AccountInfo.id
        };

        await _hubConnection.SendAsync("sendGlobalChatMessage", JsonSerializer.Serialize(data));
    }


    #region League Parties

    public event Action<string?> PARTY_PartyUpdateReceived;
    public event Action<string?> PARTY_PartyKickReceived;
    
    private async void PartyUpdateReceived(string data)
    {
        Log.Information("RECEIVED PARTY UPDATE MESSAGE FROM SERVER");
        Log.Information("DATA:");
        Log.Information(data);
        Log.Information("------------------");
        PARTY_PartyUpdateReceived?.Invoke(data);
    }
    
    private async void PartyKickReceived(string data)
    {
        Log.Information("RECEIVED PARTY KICK MESSAGE FROM SERVER");
        PARTY_PartyKickReceived?.Invoke(data);
    }

    #endregion

    #region Queue

    public event Action<object?> QUEUE_InQueueMessageReceived;
    public event Action<object?> QUEUE_LeaveQueueMessageReceived;

    
    private async void InQueueMessageReceived(string data)
    {
        Log.Information("RECEIVED InQueue MESSAGE FROM SERVER");
        QUEUE_InQueueMessageReceived?.Invoke(data);
    }
    
    private async void LeaveQueueMessageReceived(string data)
    {
        Log.Information("RECEIVED LeaveQueue MESSAGE FROM SERVER");
        QUEUE_LeaveQueueMessageReceived?.Invoke(null);
    }
    #endregion
    
    #region Match

    public event Action<object?> MATCH_JoinedMatchMessageReceived;
    public event Action<object?> MATCH_MatchUpdateMessageReceived; // Recieves Update for Ready Change, Map Change, State Change,
    public event Action<object?> MATCH_PartyInformationRequested; // When Recieves, Server is Requesting Information on VALORANT Party.
    public event Action<object?> MATCH_CustomGameSettingsReceived; // When Recieves, Server is has sent custom game settings.
    
    private async void JoinedMatchMessageReceived(string data)
    {
        Log.Information("RECEIVED Match Joined MESSAGE FROM SERVER");
        MATCH_JoinedMatchMessageReceived?.Invoke(data);
    }
    
    private async void MatchUpdateMessageReceived(string data)
    {
        Log.Information("RECEIVED Match Update MESSAGE FROM SERVER");
        MATCH_MatchUpdateMessageReceived?.Invoke(null);
    }
    
    private async void PartyInformationRequested(string data)
    {
        Log.Information("RECEIVED PartyInformationRequested MESSAGE FROM SERVER");
        MATCH_PartyInformationRequested?.Invoke(null);
    }
    
    private async void CustomGameSettingsReceived(string data)
    {
        Log.Information("RECEIVED CustomGameSettings MESSAGE FROM SERVER");
        MATCH_CustomGameSettingsReceived?.Invoke(null);
    }
    #endregion
}