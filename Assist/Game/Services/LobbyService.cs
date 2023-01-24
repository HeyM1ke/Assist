using System;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Objects.AssistApi.Game;
using Assist.ViewModels;
using Serilog;
using Serilog.Core;

namespace Assist.Game.Services;

public class LobbyService
{
    public static LobbyService Instance;

    public LobbyService()
    {
        if (Instance == null)
            Instance = this;
        AssistApplication.Current.GameServerConnection.LOBBY_InviteRequested += GameServerConnectionOnLOBBY_InviteRequested;
        AssistApplication.Current.GameServerConnection.LOBBY_InviteSentFromCreator += GameServerConnectionOnLOBBY_InviteSentFromCreator;
    }
    
    // Handle Server Events
    
    // Join Lobbies
    
    // Create Lobbies
    
    //Send Join Request
    public async Task RequestPartyJoin(RequestPartyJoin data)
    {
        Log.Information("Requesting to Join Party");
        // Send Request to Server for Invite
        await AssistApplication.Current.GameServerConnection.RequestPartyInvite(data);
    }
    
    private async void GameServerConnectionOnLOBBY_InviteRequested(string? obj)
    {
        var data = JsonSerializer.Deserialize<InvitePlayerData>(obj);
        
        Log.Information("Recieved Request to Invite Player");
        
        Log.Information($"Sending invite to Player {data.CurrentGameName}#{data.CurrentTag}");

        try
        {
            await AssistApplication.Current.CurrentUser.Party.InvitePlayerToParty(data.CurrentGameName, data.CurrentTag);
        }
        catch (Exception e)
        {
            Log.Error("Failed to send Player an Invite to the current party.");
            Log.Error("Failed to send Player an Invite to the current party. MESSAGE " + e.Message);
            Log.Error("Failed to send Player an Invite to the current party. STACK " + e.StackTrace);
            return;
        }

        await AssistApplication.Current.GameServerConnection.PartyInviteSentFromAssist(data);
    }
    
    private void GameServerConnectionOnLOBBY_InviteSentFromCreator(string? obj)
    {
        Log.Information("Received an Invite from a Creator");
        var data = JsonSerializer.Deserialize<InvitePlayerData>(obj);
        
        Log.Information("recieved an invite for party of id of: " + data.PartyId);
        try
        {
            AssistApplication.Current.CurrentUser.Party.JoinParty(data.PartyId);
            Log.Information("Joined Party of id " + data.PartyId);
        }
        catch (Exception e)
        {
            Log.Error("Failed to Join party from an Invite to the expected lobby.");
            Log.Error("Failed to Join party from an Invite to the expected lobby. MESSAGE " + e.Message);
            Log.Error("Failed to Join party from an Invite to the expected lobby. STACK " + e.StackTrace);
            return;
        }
        
    }
}