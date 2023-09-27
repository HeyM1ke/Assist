using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Models.Recent;
using Assist.Settings;
using Assist.ViewModels;
using DynamicData;
using Serilog;
using ValNet.Objects.Exceptions;
using ValNet.Objects.Player;


namespace Assist.Game.Services;

public class RecentService
{
    public List<RecentMatch>? RecentMatches = new List<RecentMatch>();
    public List<RecentPlayer>? RecentPlayers = new List<RecentPlayer>();
    
    public static RecentService Current;
    public static string RecentFolderPath = Path.Combine(AssistSettings.SettingsFolderPath, "Game", "Modules", "MatchTrack");
    public static string RecentMatchesPath = Path.Combine(RecentFolderPath, "RecentMatches.json");
    public static string RecentPlayersPath = Path.Combine(RecentFolderPath, "RecentPlayers.json");
    
    public Action RecentServiceUpdated;
    public RecentService()
    {
        if (Current is not null) return;

        Current = this;
        Directory.CreateDirectory(RecentFolderPath);
        LoadRecentLists();
    }
    
    public static void SaveSettings()
    {
        try
        {
            
            File.WriteAllText(RecentMatchesPath,
                JsonSerializer.Serialize(Current.RecentMatches, new JsonSerializerOptions() { WriteIndented = true }),
                Encoding.UTF8);
            File.WriteAllText(RecentPlayersPath,
                JsonSerializer.Serialize(Current.RecentPlayers, new JsonSerializerOptions() { WriteIndented = true }),
                Encoding.UTF8);
        }
        catch (Exception e)
        {
            Log.Error("Failed to Save Recent Matches & Player Settings");
        }
    }

    public async Task AddMatch(string matchId)
    {
        if (string.IsNullOrEmpty(matchId))return;
        if (RecentMatches.Exists(mth => mth.MatchId.Equals(matchId, StringComparison.OrdinalIgnoreCase)))
            return;

        var data = await GetMatchData(matchId);
        if (data is null)
            return;
        
        RecentMatches.Add(data);
        SaveSettings();
        RecentServiceUpdated?.Invoke();
    }
    
    public void AddMatch(RecentMatch? matchData)
    {
        if (matchData is null)
            return;
        
        if (RecentMatches.Exists(mth => mth.MatchId.Equals(matchData.MatchId, StringComparison.OrdinalIgnoreCase)))
            return;

        RecentMatches.Add(matchData);
        SaveSettings();
        RecentServiceUpdated?.Invoke();
    }
    
    public async Task UpdateMatch(string matchId)
    {
        if (string.IsNullOrEmpty(matchId))return;
        
        if (!RecentMatches.Exists(mth => mth.MatchId.Equals(matchId, StringComparison.OrdinalIgnoreCase)))
            return;
        
        var t = await GetMatchData(matchId);

        if (t is null)
            return;
        
        
        var original = RecentMatches.Find(x => x.MatchId.Equals(matchId, StringComparison.OrdinalIgnoreCase));
        
        RecentMatches.Replace(original, t);
        SaveSettings();
        RecentServiceUpdated?.Invoke();
    }
    
    public async void UpdateMatch(RecentMatch? matchData)
    {
        if (matchData is null)
            return;
        
        if (!RecentMatches.Exists(mth => mth.MatchId.Equals(matchData.MatchId, StringComparison.OrdinalIgnoreCase)))
            return;
        
        var original = RecentMatches.Find(x => x.MatchId.Equals(matchData.MatchId, StringComparison.OrdinalIgnoreCase));
        
        RecentMatches.Replace(original, matchData);
        SaveSettings();
        RecentServiceUpdated?.Invoke();
    }
    
    public void RemoveMatch(string matchId)
    {
        if (!RecentMatches.Exists(mth => mth.MatchId.Equals(matchId, StringComparison.OrdinalIgnoreCase)))
            return;
        
        var mth = RecentMatches.Find(x => x.MatchId.Equals(matchId, StringComparison.OrdinalIgnoreCase));
        RecentMatches.Remove(mth);
        SaveSettings();
        RecentServiceUpdated?.Invoke();
    }


    /// <summary>
    /// Fills Recent Matches with Match History (Recent 10)
    /// </summary>
    public async Task FillMatches()
    {
        MatchHistoryObj? matchHistoryObj = null;
        try
        {
            matchHistoryObj = await AssistApplication.Current.CurrentUser.Player.GetPlayerMatchHistory();
        }
        catch (RequestException e)
        {
            Log.Fatal("FAILED MATCH HISTORY ERROR: " + e.StatusCode);
            Log.Fatal("FAILED MATCH HISTORY ERROR: " + e.Content);

            if (e.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Fatal("Token Error while getting match histoy: ");
                await AssistApplication.Current.RefreshService.CurrentUserOnTokensExpired();
                await FillMatches();
                return;
            }
        }

        
        for (int i = matchHistoryObj.History.Count-1; i >= 0; i--)
        {
            Log.Information("Checking Match In History");
            
            var mthExists = RecentMatches.Exists(mth => mth.MatchId.Equals(matchHistoryObj.History[i].MatchID));
            if (mthExists)
            {
                Log.Information("Match Existed");
                var preExistMatch = RecentMatches.Find(x => x.MatchId.Equals(matchHistoryObj.History[i].MatchID));
                if (preExistMatch.IsCompleted = true) continue;

                Log.Information("Match Existed but was not completed, updating data.");
                await UpdateMatch(matchHistoryObj.History[i].MatchID);
            }

            var data = await GetMatchData(matchHistoryObj.History[i].MatchID);
            AddMatch(data);
        }
    }

    private async Task<RecentMatch?> GetMatchData(string matchId, bool isUpdating = false)
    {
        if (string.IsNullOrEmpty(matchId))
            return null;

        MatchDetailsObj? matchDetailsObj = null;
        try
        {
            matchDetailsObj = await AssistApplication.Current.CurrentUser.Player.GetMatchDetails(matchId);
        }
        catch (Exception ex)
        {
            if (ex is RequestException)
            {
                RequestException e = ex as RequestException;
                Log.Fatal("FAILED MATCH DETAILS ERROR: " + e.StatusCode);
                Log.Fatal("FAILED MATCH DETAILS ERROR: " + e.Content);
            
                if(e.StatusCode == HttpStatusCode.BadRequest){
                    Log.Fatal("Token Error while getting match details: ");
                    await AssistApplication.Current.RefreshService.CurrentUserOnTokensExpired();
                    return await GetMatchData(matchId);
                }
            }
            Log.Fatal("FAILED to Get/Parse Match Details: " + ex.Message);
            return null;
        }

        if (matchDetailsObj is null) return null;

        var data = CreateRecentMatch(matchDetailsObj);

        return data;
    }

    private RecentMatch CreateRecentMatch(MatchDetailsObj matchDetails)
    {
        Log.Information($"Creating Recent Match for ID of {matchDetails.MatchInformation.MatchId}");
        var recentMatch = new RecentMatch()
        {
            MatchId = matchDetails.MatchInformation.MatchId,
            DateOfMatch = DateTime.UnixEpoch.AddMilliseconds(matchDetails.MatchInformation.GameStartMillis),
            LengthOfMatchInSeconds = matchDetails.MatchInformation.GameLengthMillis / 1000,
            Gamemode = matchDetails.MatchInformation.GameMode,
            QueueId = matchDetails.MatchInformation.QueueID,
            IsCompleted = matchDetails.MatchInformation.IsCompleted,
            MapId = matchDetails.MatchInformation.MapId
        };

        // Get Local Player, Should always be valid due to the players history
        var localPlayerData = matchDetails.Players.Find(x => x.Subject.Equals(AssistApplication.Current.CurrentUser.UserData.sub, StringComparison.OrdinalIgnoreCase));
        
        // Determine Win & Score Conditions
        int indexOfLocalTeam = matchDetails.Teams.FindIndex(x => x.TeamId.Equals(localPlayerData.TeamId));
        recentMatch.AllyTeamId = localPlayerData.TeamId;
        recentMatch.AllyTeamScore = matchDetails.Teams[indexOfLocalTeam].RoundsWon;
        recentMatch.Result = matchDetails.Teams[indexOfLocalTeam].Won
            ? RecentMatch.MatchResult.VICTORY
            : RecentMatch.MatchResult.LOSS;
        recentMatch.EnemyTeamScore = matchDetails.Teams[indexOfLocalTeam].RoundsPlayed - matchDetails.Teams[indexOfLocalTeam].RoundsWon;

        // Check if match is a Premier match and if So, Store team Names
        if (matchDetails.Teams[indexOfLocalTeam].RosterInfo is not null)
        {
            Log.Information($"Premier Data is Valid for Recent Match for ID of {matchDetails.MatchInformation.MatchId}");
            var info = matchDetails.Teams[indexOfLocalTeam].RosterInfo;
            if (info != null)
            {
                recentMatch.AllyTeamName = info.Name;
                var rosterInfo = matchDetails.Teams[(matchDetails.Teams.Count - 1) - indexOfLocalTeam].RosterInfo;
                if (rosterInfo != null)
                    recentMatch.EnemyTeamName =
                        rosterInfo.Name;
            }
        }
        
        // Handle Recent Players
        HandlePlayers(matchDetails, recentMatch);
        
        // Handle Adding Players to Match
        HandleRecentMatchPlayers(matchDetails, recentMatch);

        return recentMatch;
    }
    private async void HandlePlayers(MatchDetailsObj matchDetails , RecentMatch recentMatch)
    {
        var players = matchDetails.Players;

        foreach (var playerObj in players)
        {
            var recentPlayerData = RecentPlayers.Find(ply => ply.PlayerId.Equals(playerObj.Subject, StringComparison.OrdinalIgnoreCase));
            if (recentPlayerData is null)
            {
                recentPlayerData = new RecentPlayer()
                {
                    FirstSeen = DateTime.UtcNow,
                    PlayerId = playerObj.Subject
                };
            }


            recentPlayerData.LastSeen = DateTime.UtcNow;
            recentPlayerData.LastSeenMatchId = matchDetails.MatchInformation.MatchId;
            recentPlayerData.Matches.Add(matchDetails.MatchInformation.MatchId);

            int index = RecentPlayers.FindIndex(ply => ply.PlayerId.Equals(recentPlayerData.PlayerId));
            if (index < 0)
                RecentPlayers.Add(recentPlayerData);
            else
                RecentPlayers[index] = recentPlayerData;
        }
        
        
    }
    
    
    
    /// <summary>
    /// Creates the Player for the RecentMatch Object.
    /// </summary>
    /// <param name="matchDetails"></param>
    /// <param name="recentMatch"></param>
    private async void HandleRecentMatchPlayers(MatchDetailsObj matchDetails, RecentMatch recentMatch)
    {
        var players = matchDetails.Players;

        var oldMatch = RecentMatches?.Find(x => x.MatchId.Equals(recentMatch.MatchId));
        
        foreach (var playerObj in players)
        {
            var oldP = oldMatch?.Players.Find(x => x.PlayerId.Equals(playerObj.Subject));
            
            
            var nP = new RecentMatch.Player()
            {
                PlayerId = playerObj.Subject,
                CompetitiveTier = oldP is null ? (int)playerObj.CompetitiveTier : oldP.CompetitiveTier,
                PlayerAgentId = playerObj.CharacterId,
                PlayerName = playerObj.GameName,
                PlayerTag = playerObj.TagLine,
                TeamId = playerObj.TeamId,
                Statistics = new RecentMatch.Player.Stats()
                {
                    Kills = (int)playerObj.Stats.Kills,
                    Assists = (int)playerObj.Stats.Assists,
                    Deaths = (int)playerObj.Stats.Deaths,
                    Score = (int)playerObj.Stats.Score
                }
            };
            
            recentMatch.Players.Add(nP);
        }
    }
    
    private async void LoadRecentLists()
    {
        Log.Information("Loading Recent Lists");
        
        if (File.Exists(RecentMatchesPath))
        {
            var settingsContent = File.ReadAllText(RecentMatchesPath);
            try
            {
                Current.RecentMatches = JsonSerializer.Deserialize<List<RecentMatch>>(settingsContent);
                Log.Information("Successfully read the Recent Matches File");
            }
            catch (Exception e)
            {
                Log.Error("FAILED to read: Recent Matches File does not exist. Creating File.");
            }
        }
        else
        {
            Log.Information("Recent Matches File does not exist. Creating File.");
            SaveSettings();
        }
        
        if (File.Exists(RecentPlayersPath))
        {
            var settingsContent = File.ReadAllText(RecentPlayersPath);
            try
            {
                Current.RecentPlayers = JsonSerializer.Deserialize<List<RecentPlayer>>(settingsContent);
                Log.Information("Successfully read the Recent Players File");
            }
            catch (Exception e)
            {
                Log.Error("FAILED to read: Recent Players File does not exist. Creating File.");
            }
        }
        else
        {
            Log.Information("Recent Players File does not exist. Creating File.");
            SaveSettings();
        }

        if (RecentMatches?.Count == 0)
        {
            FillMatches();
        }
    }
    
}