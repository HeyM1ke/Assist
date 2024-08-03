using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Controls.Game.Live;
using Assist.Core.Helpers;
using Assist.Models.Enums;
using Assist.Services.Assist;
using Assist.Shared.Models.Assist;
using Assist.Views.Game.Live;
using Assist.Views.Game.Live.Pages;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using ValNet.Objects.Coregame;
using ValNet.Objects.Exceptions;
using ValNet.Objects.Local;
using WebSocketSharp;

namespace Assist.ViewModels.Game;

public partial class IngamePageViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<LivePlayerPreviewControl> _allyTeamControls =
        new ObservableCollection<LivePlayerPreviewControl>();

    [ObservableProperty]
    private ObservableCollection<LiveEnemyPlayerPreviewControl> _enemyTeamControls =
        new ObservableCollection<LiveEnemyPlayerPreviewControl>();
    
    [ObservableProperty]
    private ObservableCollection<LiveSlimPlayerPreviewControl> _deathMatchControls =
        new ObservableCollection<LiveSlimPlayerPreviewControl>();

    [ObservableProperty] private bool _isDeathmatch;
    [ObservableProperty] private bool _isRange = false;

    [ObservableProperty] private string _allyScore;
    [ObservableProperty] private string _enemyScore;

    [ObservableProperty] private string _matchId;
    [ObservableProperty] private string _queueName;
    [ObservableProperty] private string _serverName;
    [ObservableProperty] private string _mapName;
    [ObservableProperty] private string _mapImage;
    [ObservableProperty] private string _gamemodeName;

    private bool _setupFlag = false;

    public async Task PageSetup()
    {
        try
        {
            var getPlayerResp = await AssistApplication.ActiveUser.CoreGame.FetchPlayer();
            if (getPlayerResp != null)
            {
                MatchId = getPlayerResp.MatchID;
            }

            if (string.IsNullOrEmpty(MatchId))
            {
                Log.Error("Failed to get MatchID");

                return;
            }
        }
        catch (RequestException e)
        {
            Log.Fatal("Error on getting player coregame");
            Log.Fatal("COREGAME ERROR: " + e.StatusCode);
            Log.Fatal("COREGAME ERROR: " + e.Content);
            Log.Fatal("COREGAME ERROR: " + e.Message);

            if (e.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Fatal("TOKEN ERROR: " + e.Message);
                await AssistApplication.RefreshService.CurrentUserOnTokensExpired();
                await this.PageSetup();
                return;
            }
        }


        if (!_setupFlag)
        {
            // First Subscribe to Updates from the PREGAME Api on Websocket. To Update the Data for whenever there is an UPDATE.
            await UpdateData(); // Do inital Pregame Check
            
            AssistApplication.RiotWebsocketService.UserPresenceMessageEvent += RiotWebsocketServiceOnUserPresenceMessageEvent;

            _setupFlag = true;
        }
    }

    private async void RiotWebsocketServiceOnUserPresenceMessageEvent(object obj)
    {
        // On message recieved Check if it is a PREGAME Message.
        await UpdateData();
    }

    public async Task UpdateData()
    {
        if (string.IsNullOrEmpty(MatchId))
        {
            await PageSetup();
        }

        Log.Error("Updating Data on Ingame");
        CoregameMatch MatchResp = new CoregameMatch();
        ChatV4PresenceObj PresenceResp = new ChatV4PresenceObj();
        try
        {
            MatchResp = await AssistApplication.ActiveUser.CoreGame.FetchMatch(MatchId!);
            PresenceResp = await AssistApplication.ActiveUser.Presence.GetPresences();
            
            
            if (MatchResp == null || PresenceResp == null)
            {
                Log.Error("Failed on getting update data. Match or Presence");
                return;
            }
        }
        catch (RequestException e)
        {
            Log.Fatal("Error on getting player match or Pres");
            Log.Fatal("COREGAME ERROR: " + e.StatusCode);
            Log.Fatal("COREGAME ERROR: " + e.Content);
            Log.Fatal("COREGAME ERROR: " + e.Message);

            if (e.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Fatal("COREGAME TOKEN ERROR: ");
                await AssistApplication.RefreshService.CurrentUserOnTokensExpired();
                return;
            }
            
            if (e.Content.Contains("match was not found", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var getPlayerResp = await AssistApplication.ActiveUser.CoreGame.FetchPlayer();
                    LiveView._viewModel.ChangePage(new MenusPageView());
                    LiveView._viewModel.CurrentPage = ELivePage.MENUS;
                }
                catch (Exception exception)
                {
                    return;
                }
            }

            return;
        }
        

        if (MatchResp.Players == null || MatchResp.Players.Count == 0)
            return;

        // Log MatchID to Log
        Log.Information($"COREGAME MATCH ID: {MatchResp.MatchID}");

        // With the presences grab all team members presenses

        // get all ids from team
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var allyTeam = await GetAllyTeam(MatchResp.Players);
            var enemyTeam = await GetEnemyTeam(MatchResp.Players);
            var allMatchPlayerIds = MatchResp.Players.Select(p => p.Subject).ToList();

            //TODO Request Data for teammatesID and store them in the LiveViewViewModel Class to the ReputationUserV2s List. 
            //await LiveViewViewModel.GetUserReputations(teamIds);

            var allTeamPresences =
                PresenceResp.presences.FindAll(pres =>
                    allMatchPlayerIds.Contains(pres.puuid));

            // Update Map Data
            HandleMatchData(MatchResp,
                PresenceResp.presences.Find(p => p.puuid == AssistApplication.ActiveUser.UserData.sub));

            if (IsDeathmatch)
            {
                if (DeathMatchControls.Count == 0)
                {
                    foreach (var allyPlayer in allyTeam)
                    {
                        Log.Information("Adding Deathmatch Player to Coregame. : " + allyPlayer.Subject);
                        await AddUserToAllyList(allyPlayer);
                    }
                }
                else
                {
                    // For every other time around, Update the Players that already excist.
                    foreach (var allyPlayer in allyTeam)
                    {
                        Log.Information("Updating Deathmatch Player to Coregame. : " + allyPlayer.Subject);
                        await UpdateUserInAllyList(allyPlayer);
                    }
                }
                
                return;
            }
            
            if (AllyTeamControls.Count == 0)
            {
                foreach (var allyPlayer in allyTeam)
                {
                    Log.Information("Adding Ally Player to Coregame. : " + allyPlayer.Subject);
                    await AddUserToAllyList(allyPlayer);
                }
            }
            else
            {
                // For every other time around, Update the Players that already excist.
                foreach (var allyPlayer in allyTeam)
                {
                    Log.Information("Updating Ally Player to Coregame. : " + allyPlayer.Subject);
                    await UpdateUserInAllyList(allyPlayer);
                }
            }

            if (EnemyTeamControls.Count == 0)
            {
                foreach (var enemyPlayer in enemyTeam)
                {
                    Log.Information("Adding Enemy Player to Coregame. : " + enemyPlayer.Subject);
                    await AddUserToEnemyList(enemyPlayer);
                }
            }
            else
            {
                // For every other time around, Update the Players that already excist.
                foreach (var enemyPlayer in enemyTeam)
                {
                    Log.Information("Updating Enemy Player to Coregame. : " + enemyPlayer.Subject);
                    await UpdateUserInEnemyList(enemyPlayer);
                }
            }
        });
        
        HandleIngameMatchTracking(MatchResp, PresenceResp.presences.Find(p => p.puuid == AssistApplication.ActiveUser.UserData.sub));
    }

    private async Task<List<CoregameMatch.Player>> GetEnemyTeam(List<CoregameMatch.Player> matchRespPlayers)
    {
        var currentPlayer =
            matchRespPlayers.Find(player => player.Subject == AssistApplication.ActiveUser.UserData.sub);

        var enemyTeam = matchRespPlayers.FindAll(player => player.TeamID != currentPlayer.TeamID);

        return enemyTeam;
    }

    private async Task<List<CoregameMatch.Player>> GetAllyTeam(List<CoregameMatch.Player> matchRespPlayers)
    {
        var currentPlayer =
            matchRespPlayers.Find(player => player.Subject == AssistApplication.ActiveUser.UserData.sub);

        var allyTeamMates = matchRespPlayers.FindAll(player => player.TeamID == currentPlayer.TeamID);

        return allyTeamMates;
    }

    private async Task HandleMatchData(CoregameMatch Match, ChatV4PresenceObj.Presence currentUser)
    {
        if (Match.MapID != null)
        {
            
            
            Log.Information("Getting map data for ID of: " + Match.MapID.ToLower());
            MapName = ValorantHelper.MapsByPath?[Match.MapID.ToLower()].ToUpper();
            if (Match.MapID.Contains("poveglia", StringComparison.OrdinalIgnoreCase))
                IsRange = true;
            else
                MapImage = $"https://cdn.assistval.com/maps/{ValorantHelper.MapsByPath?[Match.MapID.ToLower()]}_Featured.png";
            
            
            
        }

        if (Match.MatchData != null)
        {
            Log.Information("Getting queue data for ID of: " + Match.MatchData.QueueID.ToLower());

            var queueName = ValorantHelper.DetermineQueueKey(Match.MatchData.QueueID.ToLower());

            // Check if the Queue is Deathmatch.
            IsDeathmatch = Match.MatchData.QueueID.ToLower() == "deathmatch";

            IsRange = Match.MapID.Contains("poveglia"); // this is dumb but works.
            QueueName = queueName.ToUpper();

            try
            {
                ServerName = ValorantHelper.Servers[Match.GamePodID];
            }
            catch (Exception e)
            {
                Log.Error("Failed to get Gamepod Server Name from Dictionary");
            }
        }

        if (Match.ProvisioningFlow == "CustomGame")
        {
            Log.Information("Getting ProvisioningFlow data for ID of: " + Match.ProvisioningFlow);

            QueueName = "CUSTOM GAME";

            try
            {
                ServerName = ValorantHelper.Servers[Match.GamePodID];
            }
            catch (Exception e)
            {
                Log.Error("Failed to get Gamepod Server Name from Dictionary");
            }
        }

        

        if (currentUser != null)
        {
            var pres = await ValorantHelper.GetPresenceData(currentUser);

            if (pres != null)
            {
                AllyScore = pres.partyOwnerMatchScoreAllyTeam.ToString();
                EnemyScore = pres.partyOwnerMatchScoreEnemyTeam.ToString();
            }
        }


    }

    private async Task AddUserToAllyList(CoregameMatch.Player Player, Dictionary<string, IBrush> dic = null)
    {
        var checkForExcisting = AllyTeamControls.FirstOrDefault(control => control.PlayerId == Player.Subject);
        if (checkForExcisting != null)
            return;
        
        var dmexistingPlayer = DeathMatchControls.FirstOrDefault(control => control.PlayerId == Player.Subject);
        if (dmexistingPlayer != null)
            return;

        if (dic != null)
            dic.TryGetValue(Player.Subject, out IBrush? color);

        if (IsRange)
            return;

        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (IsDeathmatch)
                DeathMatchControls.Add(new LiveSlimPlayerPreviewControl(Player));
            else
                AllyTeamControls.Add(new LivePlayerPreviewControl(Player));
        });
    }

    private async Task UpdateUserInAllyList(CoregameMatch.Player Player, Dictionary<string, IBrush> dic = null)
    {
        LivePlayerPreviewControl? control = null;
        LiveSlimPlayerPreviewControl? dmControl = null;
        if (IsRange)
            return;
        
        if (IsDeathmatch)
            dmControl = DeathMatchControls.FirstOrDefault(control => control.PlayerId == Player.Subject);
        else
            control = AllyTeamControls.FirstOrDefault(control => control.PlayerId == Player.Subject);

        if (dic != null)
            dic.TryGetValue(Player.Subject, out IBrush? color);
        
        if (dmControl != null)
        {
            Log.Information("Updating Data for Previously found player for Coregame deathmatch. : " + Player.Subject);
            Dispatcher.UIThread.InvokeAsync(async () => { await dmControl.UpdatePlayer(Player); });

            return;
        }
        
        if (control != null)
        {
            Log.Information("Updating Data for Previously found player for Coregame. : " + Player.Subject);
            Dispatcher.UIThread.InvokeAsync(async () => { await control.UpdatePlayer(Player); });

            return;
        }

        Log.Fatal("Tried Updating Control for Player that doesnt exist.");
        await AddUserToAllyList(Player, dic);

    }

    private async Task AddUserToEnemyList(CoregameMatch.Player Player, Dictionary<string, IBrush> dic = null)
    {

        var checkForExcisting = EnemyTeamControls.FirstOrDefault(control => control.PlayerId == Player.Subject);
        if (checkForExcisting != null)
            return;

        if (dic != null)
            dic.TryGetValue(Player.Subject, out IBrush? color);


        Dispatcher.UIThread.InvokeAsync(async () => { EnemyTeamControls.Add(new LiveEnemyPlayerPreviewControl(Player)); });
    }

    private async Task UpdateUserInEnemyList(CoregameMatch.Player Player, Dictionary<string, IBrush> dic = null)
    {
        var control = EnemyTeamControls.FirstOrDefault(control => control.PlayerId == Player.Subject);

        if (dic != null)
            dic.TryGetValue(Player.Subject, out IBrush? color);
        
        
        if (control != null)
        {
            Log.Information("Updating Data for Previously found player for pregame. : " + Player.Subject);
            Dispatcher.UIThread.InvokeAsync(async () => { await control.UpdatePlayer(Player); });

            return;
        }

        Log.Fatal("Tried Updating Control for User that does not exist?");
        Log.Fatal("Adding User to list.");
        await AddUserToAllyList(Player, dic);

    }
    
    private async Task HandleIngameMatchTracking(CoregameMatch matchResp, ChatV4PresenceObj.Presence? currentUser)
        {

            var existingMatch = RecentService.Current.RecentMatches?.Find(x => x.MatchId.Equals(matchResp.MatchID));
            var pres = await ValorantHelper.GetPresenceData(currentUser);
            
            var recentM = new RecentMatch
            {
                MatchId = matchResp.MatchID,
                AllyTeamId = matchResp.Players?.Find(x => x.Subject.Equals(AssistApplication.ActiveUser.UserData.sub,
                    StringComparison.OrdinalIgnoreCase))?.TeamID ?? "",
                IsCompleted = false,
                Result = RecentMatch.MatchResult.IN_PROGRESS,
                MapId = matchResp.MapID,
                QueueId = matchResp.MatchData == null ? "customgame" : matchResp.MatchData.QueueID,
                DateOfMatch = DateTime.UtcNow,
                LengthOfMatchInSeconds = 0,
                AllyTeamScore = pres.partyOwnerMatchScoreAllyTeam,
                EnemyTeamScore = pres.partyOwnerMatchScoreEnemyTeam,
                MatchTrack_LastState = "INGAME",
                OwningPlayer = AssistApplication.ActiveUser.UserData.sub
            };

            if (existingMatch is not null)
            {
                recentM = existingMatch;
            }

            if (matchResp.Players?.Count == 0)
                return;
            
            HandlePlayers(matchResp, recentM);
            HandleRecentMatchPlayers(matchResp, recentM);
            
            if (RecentService.Current.RecentMatches.Exists(x => x.MatchId.Equals(recentM.MatchId)))
            {
                RecentService.Current.UpdateMatch(recentM);
                return;
            }
            
            RecentService.Current.AddMatch(recentM);
        }
        
        private async void HandlePlayers(CoregameMatch matchDetails , RecentMatch recentMatch)
        {
            var players = matchDetails.Players;

            foreach (var playerObj in players)
            {
                var recentPlayerData = RecentService.Current.RecentPlayers?.Find(ply => ply.PlayerId.Equals(playerObj.Subject, StringComparison.OrdinalIgnoreCase));
                if (recentPlayerData is null)
                {
                    recentPlayerData = new RecentPlayer()
                    {
                        FirstSeen = DateTime.UtcNow,
                        PlayerId = playerObj.Subject
                    };
                }


                recentPlayerData.LastSeen = DateTime.UtcNow;
                recentPlayerData.LastSeenMatchId = matchDetails.MatchID;
                recentPlayerData.Matches.Add(matchDetails.MatchID);

                if (!recentPlayerData.TimesSeen.TryAdd(matchDetails.MatchID, recentPlayerData.LastSeen))
                {
                    recentPlayerData.TimesSeen[matchDetails.MatchID] = recentPlayerData.LastSeen;
                }
                
                int index = RecentService.Current.RecentPlayers.FindIndex(ply => ply.PlayerId.Equals(recentPlayerData.PlayerId));
                if (index < 0)
                    RecentService.Current.RecentPlayers?.Add(recentPlayerData);
                else
                    RecentService.Current.RecentPlayers[index] = recentPlayerData;
            }
        
        
        }
        
        /// <summary>
        /// Creates the Player for the RecentMatch Object.
        /// </summary>
        /// <param name="matchDetails"></param>
        /// <param name="recentMatch"></param>
        private async void HandleRecentMatchPlayers(CoregameMatch matchDetails, RecentMatch recentMatch)
        {
            var players = matchDetails.Players;

            if (players.Count == 0)
            {
                return;
            }
            
            foreach (var playerObj in players)
            {
                LivePlayerPreviewControl? data = null;
                LiveEnemyPlayerPreviewControl? dataEnemy = null;
                LiveSlimPlayerPreviewControl? dmEnemy = null;
                if (IsDeathmatch)
                {
                    dmEnemy = DeathMatchControls.ToList().Find(x =>
                        x.PlayerId.Equals(playerObj.Subject, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    data = AllyTeamControls.ToList().Find(x =>
                        x.PlayerId.Equals(playerObj.Subject, StringComparison.OrdinalIgnoreCase));

                    if (data is null)
                    {
                        dataEnemy = EnemyTeamControls.ToList().Find(x =>
                            x.PlayerId.Equals(playerObj.Subject, StringComparison.OrdinalIgnoreCase));
                    }
                }

                if (data is null && dataEnemy is null && dmEnemy is null)
                {
                    continue;
                }

                if (recentMatch.Players.Exists(x => x.PlayerId.Equals(playerObj.Subject)))
                {
                    continue;
                }

                RecentMatch.Player p = null;

                
                if (data == null && dataEnemy != null && dmEnemy == null)
                {
                    p = new RecentMatch.Player()
                    {
                        PlayerId = playerObj.Subject,
                        CompetitiveTier = (int)dataEnemy._viewModel.PlayerCompetitiveTier,
                        PlayerAgentId = playerObj.CharacterID,
                        PlayerName = !dataEnemy._viewModel.UsingAssistProfile ? dataEnemy._viewModel.PlayerName : dataEnemy._viewModel.SecondaryText.Split('#')[0],
                        PlayerTag = !dataEnemy._viewModel.UsingAssistProfile ? dataEnemy._viewModel.TagLineText : $"{dataEnemy._viewModel.SecondaryText.Split('#')[^1]}",
                        TeamId = playerObj.TeamID,
                        PlayerRealName = dataEnemy._viewModel.PlayerRealName,
                        Statistics = null
                    };
                }

                if (data != null && dataEnemy ==  null && dmEnemy == null)
                {
                    p = new RecentMatch.Player()
                    {
                        PlayerId = playerObj.Subject,
                        CompetitiveTier = (int)data._viewModel.PlayerCompetitiveTier,
                        PlayerAgentId = playerObj.CharacterID,
                        PlayerName = !data._viewModel.UsingAssistProfile ? data._viewModel.PlayerName : data._viewModel.SecondaryText.Split('#')[0],
                        PlayerTag = !data._viewModel.UsingAssistProfile ? data._viewModel.TagLineText : $"{data._viewModel.SecondaryText.Split('#')[^1]}",
                        TeamId = playerObj.TeamID,
                        PlayerRealName = data._viewModel.PlayerRealName,
                        Statistics = null
                    };
                }
                
                if (data == null && dataEnemy ==  null && dmEnemy != null)
                {
                    p = new RecentMatch.Player()
                    {
                        PlayerId = playerObj.Subject,
                        CompetitiveTier = (int)dmEnemy._viewModel.PlayerCompetitiveTier,
                        PlayerAgentId = playerObj.CharacterID,
                        PlayerName = !dmEnemy._viewModel.UsingAssistProfile ? dmEnemy._viewModel.PlayerName : dmEnemy._viewModel.SecondaryText.Split('#')[0],
                        PlayerTag = !dmEnemy._viewModel.UsingAssistProfile ? dmEnemy._viewModel.TagLineText : $"{dmEnemy._viewModel.SecondaryText.Split('#')[^1]}",
                        TeamId = playerObj.TeamID,
                        PlayerRealName = dmEnemy._viewModel.PlayerRealName,
                        Statistics = null
                    };
                }

                if (p is null)
                    continue;
            
                recentMatch.Players.Add(p);
            }
        }

        public void UnsubscribeFromEvents()
        {
            Log.Information("Page is Unloaded, Unsubbing from Events from IngameView");
            AllyTeamControls.Clear();
            EnemyTeamControls.Clear();
            DeathMatchControls.Clear();
            
            AssistApplication.RiotWebsocketService.UserPresenceMessageEvent -= RiotWebsocketServiceOnUserPresenceMessageEvent;
        }
}