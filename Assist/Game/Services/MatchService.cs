using System;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.ViewModels;
using AssistUser.Lib.Leagues.Models;
using AssistUser.Lib.Parties.Models;
using Serilog;

namespace Assist.Game.Services;

public class MatchService
{
    public static MatchService Instance;
    public AssistMatch CurrentMatchData;
    private bool currentlyBinded = false;
    public MatchService()
    {
        if (Instance is null) Instance = this; else return;

        BindToEvents();
    }

    private void BindToEvents()
    {
        if (currentlyBinded)return;
        AssistApplication.Current.GameServerConnection.MATCH_MatchUpdateMessageReceived += GameServerConnectionOnMATCH_MatchUpdateMessageReceived;
        AssistApplication.Current.GameServerConnection.MATCH_CustomGameSettingsReceived += GameServerConnectionOnMATCH_CustomGameSettingsReceived;
        AssistApplication.Current.GameServerConnection.MATCH_PartyInformationRequested += GameServerConnectionOnMATCH_PartyInformationRequested;
        currentlyBinded = !currentlyBinded;
    }
    public void UnbindToEvents()
    {
        AssistApplication.Current.GameServerConnection.MATCH_MatchUpdateMessageReceived -= GameServerConnectionOnMATCH_MatchUpdateMessageReceived;
        AssistApplication.Current.GameServerConnection.MATCH_CustomGameSettingsReceived -= GameServerConnectionOnMATCH_CustomGameSettingsReceived;
        AssistApplication.Current.GameServerConnection.MATCH_PartyInformationRequested -= GameServerConnectionOnMATCH_PartyInformationRequested;
    }
    
    
    public async Task<AssistMatch> GetCurrentMatch()
    {
        var resp = await AssistApplication.Current.AssistUser.League.GetUserMatch();

        if (resp.Code != 200)
        {
            Log.Error("CANNOT GET MATCH DATA ON MATCHSERVICE");
            Log.Error(resp.Message);
            return new AssistMatch();
        }
        
        CurrentMatchData = JsonSerializer.Deserialize<AssistMatch>(resp.Data.ToString());
        return CurrentMatchData;
    }
    
    public static async Task QuitIngameValorantMatch()
    {
        try
        {
            var resp = await AssistApplication.Current.CurrentUser.CoreGame.FetchPlayer();
            await AssistApplication.Current.CurrentUser.CoreGame.QuitMatch(resp.MatchID);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    #region Event Handlers

    private void GameServerConnectionOnMATCH_MatchUpdateMessageReceived(object? obj)
    {
        
    }
    
    private void GameServerConnectionOnMATCH_PartyInformationRequested(object? obj)
    {
        
    }

    private void GameServerConnectionOnMATCH_CustomGameSettingsReceived(object? obj)
    {
        
    }
    

    #endregion
}