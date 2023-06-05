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
        
        _hubConnection.On<string?>("receiveMatchUpdate", MatchUpdateMessageReceived);
        _hubConnection.On<string?>("receiveJoinedMatchMessage", JoinedMatchMessageReceived);
        
        _hubConnection.On<string?>("receiveValorantPartyJoinMatch", ValorantPartyJoinMatchReceived);
        _hubConnection.On<string?>("receiveMatchValorantPartyCreate", MatchValorantPartyCreateReceived);
        _hubConnection.On<string?>("requestValorantPartyInformation", PartyInformationRequested);
        
        _hubConnection.On<string?>("receiveStartValorantMatch", StartValorantMatchReceived);
        
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

    /// <summary>
    /// Received when a Match is Joined.
    /// </summary>
    public event Action<string?> MATCH_JoinedMatchMessageReceived;
    /// <summary>
    /// Receives Update for Ready Change, Map Change, State Change,
    /// </summary>
    public event Action<string?> MATCH_MatchUpdateMessageReceived;
    /// <summary>
    /// When Receives, Server is Requesting Information on VALORANT Party.
    /// </summary>
    public event Action<string?> MATCH_PartyInformationRequested; 
    
    /// <summary>
    /// When Receives, Server is has sent custom game settings.
    /// </summary>
    public event Action<string?> MATCH_CustomGameSettingsReceived;
    
    /// <summary>
    /// When Receives, Server has ordered the start of the VALORANT game.
    /// </summary>
    public event Action<string?> MATCH_StartValorantMatchReceived; 
    
    /// <summary>
    /// When Receieves, Server has ordered client to create VALORANT Party. 
    /// </summary>
    public event Action<string?> MATCH_MatchValorantPartyCreateReceived;
    
    /// <summary>
    /// When Receives, Server has ordered client to join VALORANT Party.
    /// </summary>
    public event Action<string?> MATCH_ValorantPartyJoinMatchReceived; 
    
    /// <summary>
    /// When Receives, Server has ordered the transfer of party ownership to another user.
    /// </summary>
    public event Action<string?> MATCH_TransferValorantPartyOwnershipReceived; 
    
    private void JoinedMatchMessageReceived(string data)
    {
        Log.Information("RECEIVED Match Joined MESSAGE FROM SERVER");
        MATCH_JoinedMatchMessageReceived?.Invoke(data);
    }
    
    private void MatchUpdateMessageReceived(string data)
    {
        Log.Information("RECEIVED Match Update MESSAGE FROM SERVER");
        MATCH_MatchUpdateMessageReceived?.Invoke(data);
    }
    
    private void PartyInformationRequested(string data)
    {
        Log.Information("RECEIVED PartyInformationRequested MESSAGE FROM SERVER");
        MATCH_PartyInformationRequested?.Invoke(null);
    }
    
    private void CustomGameSettingsReceived(string data)
    {
        Log.Information("RECEIVED CustomGameSettings MESSAGE FROM SERVER");
        MATCH_CustomGameSettingsReceived?.Invoke(data);
    }

    private void StartValorantMatchReceived(string data)
    {
        Log.Information("RECEIVED StartValorantMatch MESSAGE FROM SERVER");
        MATCH_StartValorantMatchReceived?.Invoke(data);
    }
    
    private void ValorantPartyJoinMatchReceived(string data)
    {
        Log.Information("RECEIVED ValorantPartyJoinMatch MESSAGE FROM SERVER");
        MATCH_ValorantPartyJoinMatchReceived?.Invoke(data);
    }
    
    private void TransferValorantPartyOwnershipReceived(string data)
    {
        Log.Information("RECEIVED TransferValorantPartyOwnership MESSAGE FROM SERVER");
        MATCH_TransferValorantPartyOwnershipReceived?.Invoke(data);
    }

    private void MatchValorantPartyCreateReceived(string data)
    {
        Log.Information("RECEIVED MatchValorantPartyCreate MESSAGE FROM SERVER");
        MATCH_MatchValorantPartyCreateReceived?.Invoke(data);
    }
    
    public async Task RequestPartyInviteForMatch(RequestPartyInviteMatch data)
    {
        Log.Information("Requesting new Party Invite Data From Server for Match: " + data);
        // Request Invite from server Data Confirming that an invite was sent.
        await _hubConnection.SendAsync("requestPrivateLobbyInviteForMatch", JsonSerializer.Serialize(data));
    }
    
    public async Task PartyInviteSentFromAssistForMatch(PartyInviteMatch data)
    {
        Log.Information("Recieved new InviteData Data From Server: " + data);
        // Send Server Data Confirming that an invite was sent.
        _hubConnection.SendAsync("confirmPrivateLobbyInvite", JsonSerializer.Serialize(data));
    }
    #endregion
}