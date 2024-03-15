using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Controls.Game.Live;
using Assist.Controls.Store;
using Assist.Core.Helpers;
using Assist.Models.Socket;
using Assist.Services.Assist;
using Assist.Shared.Models.Assist;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using ValNet.Objects.Local;
using ValNet.Objects.Pregame;

namespace Assist.ViewModels.Game;

public partial class PregamePageViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<LivePlayerPreviewControl> _userControls = new ObservableCollection<LivePlayerPreviewControl>();
    [ObservableProperty] private string _matchId;
    [ObservableProperty] private string _errorMessage;
    
    
    [ObservableProperty] private string _serverName;
    [ObservableProperty] private string _mapName;
    [ObservableProperty] private string _mapImage;
    [ObservableProperty] private string _queueName;
    [ObservableProperty] private string _localTeamSide;
    [ObservableProperty] private bool _localTeamVisable = false;

    private bool _setupFlag = false;
    public async Task PageSetup()
    {
        try
        {
            var getPlayerResp = await AssistApplication.ActiveUser.Pregame.GetPlayer();
            if (getPlayerResp != null)
            {
                MatchId = getPlayerResp.MatchID;
            }

            if (string.IsNullOrEmpty(MatchId))
            {
                Log.Error("Failed to get MatchID");
                ErrorMessage = "Failed to Get MatchID";
                return;
            }
        }
        catch (Exception e)
        {
            if (e is ValNet.Objects.Exceptions.RequestException)
            {
                var ex = e as ValNet.Objects.Exceptions.RequestException;
                Log.Fatal("Error on getting player match or Pres");
                Log.Fatal("PREGAME ERROR: " + ex.StatusCode);
                Log.Fatal("PREGAME ERROR: " + ex.Content);
                Log.Fatal("PREGAME ERROR: " + ex.Message);
                
                if(ex.StatusCode == HttpStatusCode.BadRequest){
                    Log.Fatal("PREGAME TOKEN ERROR: ");
                    await AssistApplication.RefreshService.CurrentUserOnTokensExpired();
                }
            }
                
            return;
        }


        if (!_setupFlag)
        {
            // First Subscribe to Updates from the PREGAME Api on Websocket. To Update the Data for whenever there is an UPDATE.
            await UpdateData(); // Do inital Pregame Check
            AssistApplication.RiotWebsocketService.PregameMessageEvent -= RiotWebsocketServiceOnPregameMessageEvent;    
            AssistApplication.RiotWebsocketService.PregameMessageEvent += RiotWebsocketServiceOnPregameMessageEvent;
                
            _setupFlag = true;
        }
    }
    
    private async void RiotWebsocketServiceOnPregameMessageEvent(object obj)
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
            Log.Error("Updating Data on Pregame");
            PregameMatch MatchResp = new PregameMatch();
            ChatV4PresenceObj PresenceResp = new ChatV4PresenceObj();
            try
            {
                MatchResp = await AssistApplication.ActiveUser.Pregame.GetMatch(MatchId!);
                PresenceResp = await AssistApplication.ActiveUser.Presence.GetPresences();
                if (MatchResp == null || PresenceResp == null)
                {
                    Log.Error("Failed on getting update data. Match or Presence");
                    ErrorMessage = "Failed on getting update data. Match or Presence";
                    return;
                }
            }
            catch (Exception e)
            {
                if (e is ValNet.Objects.Exceptions.RequestException)
                {
                    var ex = e as ValNet.Objects.Exceptions.RequestException;
                    Log.Fatal("Error on getting player match or Pres");
                    Log.Fatal("PREGAME ERROR: " + ex.StatusCode);
                    Log.Fatal("PREGAME ERROR: " + ex.Content);
                    Log.Fatal("PREGAME ERROR: " + ex.Message);
                
                    if(ex.StatusCode == HttpStatusCode.BadRequest){
                        Log.Fatal("PREGAME TOKEN ERROR: ");
                        await AssistApplication.RefreshService.CurrentUserOnTokensExpired();
                    }
                }
                
                return;
            }
            

            if(MatchResp.AllyTeam == null)
                return;

            // Log MatchID to Log
            Log.Information($"PREGAME MATCH ID: {MatchResp.ID}");
            
            // With the presences grab all team members presenses

            // get all ids from team
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var teamIds = MatchResp.AllyTeam.Players.Select(p => p.Subject).ToList();

                var allTeamPresences =
                    PresenceResp.presences.FindAll(pres =>
                        teamIds.Contains(pres.puuid)); // Contains every presence from the team.

                //TODO Request Data for teammatesID and store them in the LiveViewViewModel Class to the ReputationUserV2s List. 
                //await LiveViewViewModel.GetUserReputations(teamIds);
                
            

            // Now that we have the match data lets go through it. 
            // First check if this is our first time around.
            if (UserControls.Count == 0) // Checking if the List is empty to signify if this is our first time around
            {
                foreach (var allyPlayer in MatchResp.AllyTeam.Players)
                {
                    Log.Information("Adding Player to pregame. : " + allyPlayer.Subject);
                    await AddUserToList(allyPlayer);
                }
            }
            else
            {
                // For every other time around, Update the Players that already excist.
                foreach (var allyPlayer in MatchResp.AllyTeam.Players)
                {
                    Log.Information("Updating Player to pregame. : " + allyPlayer.Subject);
                    await UpdateUserInList(allyPlayer);
                }
            }
            });

            // Update Map Data
            HandleMapData(MatchResp);
            
            HandlePregameMatchTracking(MatchResp);
        }
    
     private async Task AddUserToList(PregameMatch.Player Player, Dictionary<string, IBrush> dic = null)
        {
               Log.Information("Adding User to list.");
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                UserControls.Add(new LivePlayerPreviewControl(Player));
            });
        }

        private async Task UpdateUserInList(PregameMatch.Player Player, Dictionary<string, IBrush> dic = null)
        {
            var control = UserControls.FirstOrDefault(control => control.PlayerId == Player.Subject);
            //dic.TryGetValue(Player.Subject, out IBrush? color);
            if (control != null)
            {
                Log.Information("Updating Data for Previously found player for pregame. : " + Player.Subject);
                Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await control.UpdatePlayer(Player);
                });

                return;
            }

            Log.Fatal("Tried Updating Control for User that does not exist?");
            Log.Fatal("Adding User to list.");
            await AddUserToList(Player, dic);

        }

        private async Task HandleMapData(PregameMatch Match)
        {
            if (Match.MapID != null)
            {
                Log.Information("Getting map data for ID of: " +  Match.MapID.ToLower());
                MapName = ValorantHelper.MapsByPath?[Match.MapID.ToLower()].ToUpper();
                MapImage =
                    $"https://cdn.assistval.com/maps/{ValorantHelper.MapsByPath?[Match.MapID.ToLower()]}_Featured.png";
                try
                {
                    ServerName = ValorantHelper.Servers[Match.GamePodID];
                    QueueName = ValorantHelper.DetermineQueueKey(Match.QueueID.ToLower()).ToUpper();
                    if (Match.ProvisioningFlowID == "CustomGame")
                        QueueName = ValorantHelper.DetermineQueueKey(Match.ProvisioningFlowID.ToLower()).ToUpper();
                }
                catch (Exception e)
                {
                    Log.Error("Failed to get server from ServerNames List.");
                }
            }

            
        }
        
        public void UnsubscribeFromEvents()
        {
            Log.Information("Page is Unloaded, Unsubbing from Events from PregamePageView");
            UserControls.Clear();
            AssistApplication.RiotWebsocketService.PregameMessageEvent -= RiotWebsocketServiceOnPregameMessageEvent;
        }
        
        private async Task HandlePregameMatchTracking(PregameMatch pregameMatch)
        {
            var recentM = new RecentMatch
            {
                MatchId = pregameMatch.ID,
                AllyTeamId = pregameMatch.AllyTeam.TeamID,
                IsCompleted = false,
                Result = RecentMatch.MatchResult.IN_PROGRESS,
                MapId = pregameMatch.MapID,
                QueueId = pregameMatch.QueueID,
                DateOfMatch = DateTime.UtcNow,
                LengthOfMatchInSeconds = 0,
                AllyTeamScore = 0,
                EnemyTeamScore = 0,
                MatchTrack_LastState = "PREGAME",
                OwningPlayer = AssistApplication.ActiveUser.UserData.sub
            };
            
            HandlePlayers(pregameMatch, recentM);
            HandleRecentMatchPlayers(pregameMatch, recentM);
            
            if (RecentService.Current.RecentMatches.Exists(x => x.MatchId.Equals(recentM.MatchId)))
            {
                RecentService.Current.UpdateMatch(recentM);
                return;
            }
            
            RecentService.Current.AddMatch(recentM);
        }
        
        private async void HandlePlayers(PregameMatch matchDetails , RecentMatch recentMatch)
        {
            var players = matchDetails.AllyTeam.Players;

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
                recentPlayerData.LastSeenMatchId = matchDetails.ID;
                recentPlayerData.Matches.Add(matchDetails.ID);

                if (!recentPlayerData.TimesSeen.TryAdd(matchDetails.ID, recentPlayerData.LastSeen))
                {
                    recentPlayerData.TimesSeen[matchDetails.ID] = recentPlayerData.LastSeen;
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
        private async void HandleRecentMatchPlayers(PregameMatch matchDetails, RecentMatch recentMatch)
        {
            var players = matchDetails.AllyTeam.Players;

            foreach (var playerObj in players)
            {
                var data = UserControls.FirstOrDefault(x =>
                    x.PlayerId.Equals(playerObj.Subject, StringComparison.OrdinalIgnoreCase));

                if (data is null)
                {
                    continue;
                }

                if (recentMatch.Players.Exists(x => x.PlayerId.Equals(playerObj.Subject)))
                {
                    continue;
                }
                
                var nP = new RecentMatch.Player()
                {
                    PlayerId = playerObj.Subject,
                    CompetitiveTier = (int)data._viewModel.PlayerCompetitiveTier,
                    PlayerAgentId = playerObj.CharacterID,
                    PlayerRealName = data._viewModel.PlayerRealName,
                    PlayerName = !data._viewModel.UsingAssistProfile ? data._viewModel.PlayerName : data._viewModel.SecondaryText.Split('#')[0],
                    PlayerTag = !data._viewModel.UsingAssistProfile ? $"{data._viewModel.TagLineText.Replace("#","")}" : data._viewModel.SecondaryText.Split('#')[^1],
                    TeamId = matchDetails.AllyTeam.TeamID,
                    Statistics = null
                };
            
                recentMatch.Players.Add(nP);
            }
        }
}