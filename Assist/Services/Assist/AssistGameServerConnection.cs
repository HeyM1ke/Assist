﻿using System;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;
using Serilog;

namespace Assist.Services.Assist;

public class AssistGameServerConnection: HubClient
{
    public event Action<object>? RecieveMessageEvent;
    private string GAMESERVERURL = $"https://live.assistval.com/server";
    
    public event Action ASSIST_NewNotificationAlert;
    public async Task Connect()
    {
        HubConnectionUrl = GAMESERVERURL;
        InitWithAuth(AssistApplication.AssistUser.userTokens.AccessToken);
        
        _hubConnection.On<object?>("clientNewNotificationAlert", (object data) => ASSIST_NewNotificationAlert?.Invoke());
        
        await StartHubInternal();
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

    #endregion
}