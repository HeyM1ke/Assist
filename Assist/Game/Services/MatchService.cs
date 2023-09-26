using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Assist.Game.Models;
using Assist.Objects.AssistApi.Game;
using Assist.Objects.RiotSocket;
using Assist.ViewModels;
using AssistUser.Lib.Leagues.Models;
using AssistUser.Lib.Leagues.Models.Server;
using AssistUser.Lib.Parties.Models;
using Serilog;
using ValNet.Objects.CustomGame;
using ValNet.Objects.Exceptions;
using ValNet.Objects.Local;

namespace Assist.Game.Services;

public class MatchService
{
    public static MatchService? Instance;
    public AssistMatch CurrentMatchData;
    private bool currentlyBinded = false;

    private bool _awaitingInvite = false;
    private bool _waitingForMatchToStart = false;
    private bool _matchIsInSession = false;
    
    public MatchService()
    {
        if (Instance is null) Instance = this; else return;

        BindToEvents();
    }

    private void BindToEvents()
    {
        if (currentlyBinded)
            return;
        
        AssistApplication.Current.GameServerConnection.MATCH_MatchUpdateMessageReceived += GameServerConnectionOnMATCH_MatchUpdateMessageReceived;
        AssistApplication.Current.GameServerConnection.MATCH_CustomGameSettingsReceived += GameServerConnectionOnMATCH_CustomGameSettingsReceived;
        AssistApplication.Current.GameServerConnection.MATCH_PartyInformationRequested += GameServerConnectionOnMATCH_PartyInformationRequested;
        AssistApplication.Current.GameServerConnection.MATCH_StartValorantMatchReceived += GameServerConnectionOnMATCH_StartValorantMatchReceived;
        AssistApplication.Current.GameServerConnection.MATCH_ValorantPartyJoinMatchReceived += GameServerConnectionOnMATCH_ValorantPartyJoinMatchReceived;
        AssistApplication.Current.GameServerConnection.MATCH_MatchValorantPartyCreateReceived += GameServerConnectionOnMATCH_MatchValorantPartyCreateReceived;
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
        var resp = await AssistApplication.Current.AssistUser.League.PREMATCH_GetUserMatch();

        if (resp.Code != 200)
        {
            Log.Error("CANNOT GET MATCH DATA ON MATCHSERVICE on PREMATCH");
            Log.Error(resp.Message);
        }
        else
        {
            CurrentMatchData = JsonSerializer.Deserialize<AssistMatch>(resp.Data.ToString());
            return CurrentMatchData; 
        }
        
        resp = await AssistApplication.Current.AssistUser.League.MATCH_GetUserMatch();

        if (resp.Code != 200)
        {
            Log.Error("CANNOT GET MATCH DATA ON MATCHSERVICE on MATCH");
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
    
    public static async Task UnreadyClient()
    {
        try
        {
            await AssistApplication.Current.CurrentUser.Party.SetPartyReadiness(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    
    #region Event Handlers

    private void GameServerConnectionOnMATCH_MatchUpdateMessageReceived(string? matchDataReceived)
    {
        try
        {
            CurrentMatchData = JsonSerializer.Deserialize<AssistMatch>(matchDataReceived);
        }
        catch (Exception e)
        {
            Log.Error("Failed to Parse Match Data from Server");
            Log.Error($"Exception: {e.Message}");
            Log.Error($"Stack: {e.StackTrace}");
        }
    }
    
    private void GameServerConnectionOnMATCH_PartyInformationRequested(object? obj)
    {
        
    }

    private async void GameServerConnectionOnMATCH_MatchValorantPartyCreateReceived(string? customGameSettings)
    {
        Log.Information("Party Create has been received from the server.");
        
        // Check if the player is ingame.
        // If the player is INGAME, Leave the game. 

        Log.Information("Attempting to get Presence from Local Socket");
        var currPres = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
        var clientPres = currPres.presences.Find(x => x.puuid == AssistApplication.Current.CurrentUser.UserData.sub);
        var decodedPres = await GetPresenceData(clientPres);
        Log.Information("Checking if player is inGAmE");
        if (decodedPres.sessionLoopState.Equals("INGAME", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                await QuitIngameValorantMatch();
            }
            catch (Exception e)
            {
            }
        }
        
        
        try
        {
            if (!decodedPres.isPartyOwner || decodedPres.partySize != 1)
            {
                Log.Information("Leaving Current Party");
                await AssistApplication.Current.CurrentUser.Party.LeaveParty(decodedPres.partyId);
            }

            Log.Information("Making Party Open");
            await AssistApplication.Current.CurrentUser.Party.SetPartyAccessibility(true);
            Log.Information("Creating the custom game");
            await AssistApplication.Current.CurrentUser.CustomGame.MakeCustomGame();
            
            Log.Information("Decoding the Settings from the server");
            var customData = JsonSerializer.Deserialize<AssistMatchValorantSettings>(customGameSettings);
            var data = new
            {
                Map = customData.Map.ToString(),
                Mode = customData.GameMode,
                GamePod = customData.Server,
                GameRules = new Dictionary<string, string>()
                {
                    {"TournamentMode",customData.UseTournamentMode ? "true" : "false"},
                    {"UseCheats",customData.UseCheats ? "true" : "false"},
                    {"IsOvertimeWinByTwo",customData.WinByTwo ? "true" : "false"}
                }
            };

            Log.Information("Settings the Custom Settings");
            var t = await AssistApplication.Current.CurrentUser.CustomGame.SetCustomGameSettings(data);
            Log.Information("Sending Server Details.");
            await AssistApplication.Current.AssistUser.League.PREMATCH_CaptainSetup(CurrentMatchData.Id, t);
        }
        catch(Exception e)
        {
            Log.Error("Failed to setup Custom Game");
            Log.Error($"Exception: {e.Message}");
            Log.Error($"Stack: {e.StackTrace}");
            if (e is RequestException)
            {
                var ex = e as RequestException;
                Log.Error($"Exception: {ex.Content}");
                Log.Error($"Stack: {ex.Message}");
            }
        }
    }

    private async void GameServerConnectionOnMATCH_CustomGameSettingsReceived(string? customGamesSettings)
    {
        Log.Information("Custom Game Settings Received from the Server.");
        
        try
        {
            var customData = JsonSerializer.Deserialize<AssistMatchValorantSettings>(customGamesSettings);
            var data = new
            {
                Map = customData.Map,
                Mode = customData.GameMode,
                GamePod = customData.Server,
                GameRules = new Dictionary<string, string>()
                {
                    {"TournamentMode",customData.UseTournamentMode ? "true" : "false"},
                    {"UseCheats",customData.UseCheats ? "true" : "false"},
                    {"IsOvertimeWinByTwo",customData.WinByTwo ? "true" : "false"}
                }
            };

            var t = await AssistApplication.Current.CurrentUser.CustomGame.SetCustomGameSettings(data);
        }
        catch (Exception e)
        {
            Log.Error("Failed to set Custom Games Data");
            Log.Error($"Exception: {e.Message}");
            Log.Error($"Stack: {e.StackTrace}");
        }
        
    }
    
    private async void GameServerConnectionOnMATCH_StartValorantMatchReceived(string? startMatchReceiveData)
    {
        var customData = JsonSerializer.Deserialize<AssistMatchValorantSettings>(startMatchReceiveData);
        /*AssistApplication.Current.CurrentUser.CustomGame.
        // Start the custom game, Check players to make sure they are on the right team.
        for (int i = 0; i < CurrentMatchData.TeamOne.Players.Count; i++)
        {
            var player = CurrentMatchData.TeamOne.Players[i];
            
        }*/

        var vData = new
        {
            Map = customData.Map,
            Mode = customData.GameMode,
            GamePod = customData.Server,
            GameRules = new Dictionary<string, string>()
            {
                {"TournamentMode",customData.UseTournamentMode ? "true" : "false"},
                {"UseCheats",customData.UseCheats ? "true" : "false"},
                {"IsOvertimeWinByTwo",customData.WinByTwo ? "true" : "false"}
            }
        };
        
        try
        {
           var t = await AssistApplication.Current.CurrentUser.CustomGame.SetCustomGameSettings(vData);
           if (!VerifyPlayerTeams(t))
           {
               Log.Error("Teams are not correct.");
           }

           await AssistApplication.Current.CurrentUser.CustomGame.StartCustomGame();
           //TODO: Tell Server Match Has Started With Match Data.
           _matchIsInSession = true;
        }
        catch (Exception e)
        {
            Log.Error("Attempted to Start the game but failed.");
            Log.Error("Attempted to Start the game but failed. MESSAGE: " + e.Message);
            Log.Error("Attempted to Start the game but failed. STACK: " + e.StackTrace);
            if (e is RequestException)
            {
                var d = e as RequestException;
                Log.Error("Attempted to Start the game but failed. CONTENT: " + d.Content);
            }
        }
    }
    
    private async void GameServerConnectionOnMATCH_ValorantPartyJoinMatchReceived(string? valPartyData)
    {
        Log.Information("Party Join Request recieved from the server.");
        
        var partyData = JsonSerializer.Deserialize<JoinValorantParty>(valPartyData);
        
        // Check if the player is ingame.
            // If the player is INGAME, Leave the game. 

        var currPres = await AssistApplication.Current.CurrentUser.Presence.GetPresences();
        var clientPres = currPres.presences.Find(x => x.puuid == AssistApplication.Current.CurrentUser.UserData.sub);
        var decodedPres = await GetPresenceData(clientPres);

        if (decodedPres.sessionLoopState.Equals("INGAME", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                await QuitIngameValorantMatch();
            }
            catch (Exception e)
            {
            }
        }
        
        // Join the Party.

        try
        {
            Log.Information("Attempting to Join Party of id " + partyData.ValorantPartyId);
            await AssistApplication.Current.CurrentUser.Party.JoinParty(partyData.ValorantPartyId);
        }
        catch (Exception e)
        {
            Log.Error("Failed to join Valorant Party for Match.");
            Log.Error("Failed to join Valorant Party for Match. MESSAGE " + e.Message);
            Log.Error("Failed to join Valorant Party for Match. STACK " + e.StackTrace);
            
            // If party is denied. 
            // Request separate invite from server. Return.
            
            // Report to server it was a failed join. With Username Data of current Client to receive invite.
            _awaitingInvite = true;
            // This is sent to the server which will confirm if the player id that request is apart of the match.
            // if so then the player will get sent an invite.
            var reqObj = new RequestPartyInviteMatch()
            {
                CurrentGameName = clientPres.game_name,
                CurrentTag = clientPres.game_tag,
                MatchId = CurrentMatchData.Id
            };
            await AssistApplication.Current.GameServerConnection.RequestPartyInviteForMatch(reqObj);
            return;
        }

        // If Party Join is successful, Unready user.
        await AssistApplication.Current.CurrentUser.Party.SetPartyReadiness(false);
        // Swap user to spectate, then to Team id sent.
        bool onCorrectTeam = false;
        while (!onCorrectTeam)
        {
            try
            {
                await AssistApplication.Current.CurrentUser.CustomGame.ChangeTeam("TeamSpectate");
                var cData=  await AssistApplication.Current.CurrentUser.CustomGame.ChangeTeam(partyData.TeamId);

                switch (partyData.TeamId)
                {
                    case "TeamOne":
                        var possibleUser = cData.CustomGameLobby.Membership.teamOne.Find(x =>
                            x.Subject == AssistApplication.Current.CurrentUser.UserData.sub);
                        if (possibleUser is not null)
                            onCorrectTeam = true;
                        break;
                    case "TeamTwo":
                        var possibleUserT = cData.CustomGameLobby.Membership.teamTwo.Find(x =>
                            x.Subject == AssistApplication.Current.CurrentUser.UserData.sub);
                        if (possibleUserT is not null)
                            onCorrectTeam = true;
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error("Error on Swapping Teams.");
                if (e is RequestException)
                {
                    var ex = e as RequestException;
                    Log.Error(ex.Content);
                    Log.Error(ex.Content);
                }
                
                await Task.Delay(3000);
            }
        }
        
        await AssistApplication.Current.CurrentUser.Party.SetPartyReadiness(false);
        // Watch Custom Game Updates, Make sure we stick to the right team. 
        // Make sure player does not go into game, Watch Pres. Make sure they stick to the party.
        
        

        
        
        // Subscribe to Presence Updates to Watch Player Movements. 
        AssistApplication.Current.RiotWebsocketService.UserPresenceMessageEvent += WatchUserPresenceWhileWaiting;
    }

    private void WatchUserPresenceWhileWaiting(PresenceV4Message obj)
    {
        
    }
    
    #endregion

    #region Private Methods
    

    public async Task<PlayerPresence> GetPresenceData(ChatV4PresenceObj.Presence data)
    {
        if (string.IsNullOrEmpty(data.Private))
            return new PlayerPresence();
        byte[] stringData = Convert.FromBase64String(data.Private);
        string decodedString = Encoding.UTF8.GetString(stringData);
        return JsonSerializer.Deserialize<PlayerPresence>(decodedString);
    }

    private bool VerifyPlayerTeams(CustomGameData customGameData)
    {
        return true;
    }
    #endregion
}