using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Controls.Global;
using Assist.Game.Controls.Lobbies.Popup;
using Assist.Game.Models;
using Assist.Objects.AssistApi.Game;
using Assist.Objects.RiotSocket;
using Assist.Services.Popup;
using Assist.ViewModels;
using AssistUser.Lib.Lobbies.Models;
using Avalonia.Threading;
using Serilog;
using Serilog.Core;
using ValNet.Enums;
using ValNet.Objects.Local;
using ValNet.Objects.Parties;

namespace Assist.Game.Services;

public class LobbyService
{
    public static LobbyService Instance;
    public DateTime CreatedLobbyAt;
    public bool CurrentLobbyOwner;
    private bool previousPartyMade = false;
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
    public async Task CreateLobby(CreateLobbyData data)
    {
        AssistApplication.Current.RiotWebsocketService.UserPresenceMessageEvent -= UpdatePartyOnPresenceMessage;

        var pres = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
        var p = pres.presences.Find(pres => pres.puuid == AssistApplication.Current.CurrentUser.UserData.sub);

        ValorantParty? pData;
        try
        {
            pData = await AssistApplication.Current.CurrentUser.Party.FetchParty();
        }
        catch (Exception e)
        {
            Log.Error("FAILED TO GET PARTY");
            Log.Error("FAILED TO GET PARTY Message: " + e.Message);
            Log.Error("FAILED TO GET PARTY Stack: " + e.StackTrace);
            return;
        }
        data.valorantPartyId = pData.ID.ToLower();
        data.region = Enum.GetName(typeof(RiotRegion), AssistApplication.Current.CurrentUser.GetRegion());
        data.requiresPassword = !string.IsNullOrEmpty(data.password);
        
        var r = await AssistApplication.Current.AssistUser.Lobbies.CreateLobby(data);

        if (r.IsSuccessful)
        {
            // Subscribe to Events to update the party
            CreatedLobbyAt = DateTime.Now;
            previousPartyMade = true;
            CurrentLobbyOwner = true;
            UpdatePartyOnPresenceMessage(null);
            AssistApplication.Current.RiotWebsocketService.UserPresenceMessageEvent += UpdatePartyOnPresenceMessage;
        }

        
        PopupSystem.SpawnCustomPopup(new CreateLobbyPopup(r));
    }

    private async void UpdatePartyOnPresenceMessage(PresenceV4Message? obj)
    {
        
        if (CreatedLobbyAt.AddMinutes(5) <= DateTime.Now || CurrentLobbyOwner == false)  // Party is Expired
        {
            Log.Information("Lobby Expired No Longer Sending Updates.");
            previousPartyMade = false;
            AssistApplication.Current.RiotWebsocketService.PresenceMessageEvent -= UpdatePartyOnPresenceMessage;
            return;
        }
        Log.Information("Lobby Sending Update.");
        ValorantParty? pData;
        try
        {
            pData = await AssistApplication.Current.CurrentUser.Party.FetchParty();
        }
        catch (Exception e)
        {
            Log.Error("FAILED TO GET PARTY");
            Log.Error("FAILED TO GET PARTY Message: " + e.Message);
            Log.Error("FAILED TO GET PARTY Stack: " + e.StackTrace);
            return;
        }
        var pres = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
        var p = pres.presences.Find(pres => pres.puuid == AssistApplication.Current.CurrentUser.UserData.sub);
        var presData = await GetPresenceData(p);
        var updateData = new UpdateLobbyData();
        updateData.MaxPartySize = presData.maxPartySize;
        updateData.CurrentPartySize = presData.partySize;
        updateData.PartyClosed = string.Equals(pData.Accessibility, "CLOSED");
        updateData.CurrentValorantQueue = presData.maxPartySize > 5 ? "CUSTOM_GAME" : presData.queueId;
        updateData.ValorantIdsParty = new List<string>();
        foreach (var member in pData.Members)
        {
            updateData.ValorantIdsParty.Add(member.Subject);
        }
        var r = await AssistApplication.Current.AssistUser.Lobbies.UpdateLobby(updateData);
        if (r.IsSuccessStatusCode)
            Log.Information("Lobby Sent Update.");
    }

    //Send Join Request
    public async Task RequestPartyJoin(RequestPartyJoin data)
    {
        Log.Information("Requesting to Join Party");
        // Send Request to Server for Invite
        await AssistApplication.Current.GameServerConnection.RequestPartyInvite(data);
    }
    
    private async void GameServerConnectionOnLOBBY_InviteRequested(string? obj)
    {
        Log.Information("Received Request to Invite Player");
        var data = JsonSerializer.Deserialize<InvitePlayerData>(obj);
        
        Log.Information("Deserialized Data from Server.");
        
        Log.Information($"Sending invite to Player {data.CurrentGameName}#{data.CurrentTag}");

        try
        {
            await AssistApplication.Current.CurrentUser.Party.InvitePlayerToParty(data.CurrentGameName, data.CurrentTag);
            await Task.Delay(1000);// Delay 1 Second after sending request, prevent spam to riot servers.
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
    
    private async void GameServerConnectionOnLOBBY_InviteSentFromCreator(string? obj)
    {
        Log.Information("Received an Invite from a Creator");
        var data = JsonSerializer.Deserialize<InvitePlayerData>(obj);
        
        Log.Information("recieved an invite for party of id of: " + data.PartyId);
        try
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                PopupSystem.SpawnCustomPopup(new LoadingPopup());
            });
            await AssistApplication.Current.CurrentUser.Party.JoinParty(data.PartyId);
            Log.Information("Joined Party of id " + data.PartyId);
        }
        catch (Exception e)
        {
            Log.Error("Failed to Join party from an Invite to the expected lobby.");
            Log.Error("Failed to Join party from an Invite to the expected lobby. MESSAGE " + e.Message);
            Log.Error("Failed to Join party from an Invite to the expected lobby. STACK " + e.StackTrace);
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                PopupSystem.KillPopups();
            });
            return;
        }
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            PopupSystem.KillPopups();
        });
    }
    
    private async Task<PlayerPresence> GetPresenceData(ChatV4PresenceObj.Presence data)
    {
        if (string.IsNullOrEmpty(data.Private))
            return new PlayerPresence();
        byte[] stringData = Convert.FromBase64String(data.Private);
        string decodedString = Encoding.UTF8.GetString(stringData);
        return JsonSerializer.Deserialize<PlayerPresence>(decodedString);
    }
}