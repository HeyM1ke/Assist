using System;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.ViewModels;
using AssistUser.Lib.Leagues.Models;
using AssistUser.Lib.Parties;
using AssistUser.Lib.Parties.Models;
using AssistUser.Lib.Profiles.Models;
using Serilog;

namespace Assist.Game.Services.Leagues;

public class LeagueService
{

    public static LeagueService Instance;
    
    public AssistProfile ProfileData;
    public string CurrentLeagueId { get; set; }
    public AssistParty CurrentPartyInfo { get; set; }
    public AssistLeague CurrentLeagueInfo { get; set; }

    public LeagueNavigationController NavigationController = new LeagueNavigationController();
    public LeagueService()
    {
        if (Instance is null) Instance = this; else return;
        
        BindToEvents();   
    }

    public async Task<AssistProfile> GetProfileData()
    {
        var resp = await AssistApplication.Current.AssistUser.Profile.GetProfile();
        if (resp.Code != 200)
        {
            Log.Error("CANNOT GET PROFILE DATA ON LEAGUESERVICE");
            Log.Error(resp.Message);
        }

        ProfileData = JsonSerializer.Deserialize<AssistProfile>(resp.Data.ToString());
        return ProfileData;
    }
    
    public async Task<AssistParty> GetCurrentPartyData()
    {
        var resp = await AssistApplication.Current.AssistUser.Party.GetParty();
        if (resp.Code != 200)
        {
            Log.Error("CANNOT GET PARTY DATA ON LEAGUESERVICE");
            Log.Error(resp.Message);
            return new AssistParty();
        }

        CurrentPartyInfo = JsonSerializer.Deserialize<AssistParty>(resp.Data.ToString());
        return CurrentPartyInfo;
        
    }
    
    public async Task<AssistParty> CreateNewParty()
    {
        var resp = await AssistApplication.Current.AssistUser.Party.CreateParty(new CreateParty()
        {
            LeagueId = CurrentLeagueId,
        });
        if (resp.Code != 200)
        {
            Log.Error("CANNOT CREATE PARTY ON LEAGUESERVICE");
            Log.Error(resp.Message);
        }

        CurrentPartyInfo = JsonSerializer.Deserialize<AssistParty>(resp.Data.ToString());
        return CurrentPartyInfo;
    }
    
    
    public void BindToEvents()
    {
        AssistApplication.Current.GameServerConnection.PARTY_PartyUpdateReceived += GameServerConnectionOnPARTY_PartyUpdateReceived;
        AssistApplication.Current.GameServerConnection.PARTY_PartyKickReceived += GameServerConnectionOnPARTY_PartyKickReceived;
    }
    
    public void UnbindToEvents()
    {
        AssistApplication.Current.GameServerConnection.PARTY_PartyUpdateReceived -= GameServerConnectionOnPARTY_PartyUpdateReceived;
        AssistApplication.Current.GameServerConnection.PARTY_PartyKickReceived -= GameServerConnectionOnPARTY_PartyKickReceived;
    }
    
    private void GameServerConnectionOnPARTY_PartyKickReceived(string? obj)
    {
        Log.Information("Kick Recieved from Party");
        
    }

    private void GameServerConnectionOnPARTY_PartyUpdateReceived(string? obj)
    {
        try
        {
            CurrentPartyInfo = JsonSerializer.Deserialize<AssistParty>(obj);
        }
        catch (Exception e)
        {
            Log.Error("Failed to Serialize Party data");
            Log.Error("Failed on LeagueService");
            Log.Error(e.Message);
            Log.Error(e.StackTrace);
        }
    }

    public async Task<AssistLeague> GetCurrentLeagueInformation()
    {
        var resp = await AssistApplication.Current.AssistUser.League.GetLeagueInfo(Instance.CurrentLeagueId);
        if (resp.Code != 200)
        {
            Log.Error("CANNOT GET LEAGUE DATA ON LEAGUESERVICE");
            Log.Error(resp.Message);
        }

        CurrentLeagueInfo = JsonSerializer.Deserialize<AssistLeague>(resp.Data.ToString());
        return CurrentLeagueInfo;
    }
}