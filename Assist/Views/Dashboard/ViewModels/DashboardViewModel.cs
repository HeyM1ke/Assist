using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Controls.Dashboard;
using Assist.Controls.Profile;
using Assist.Controls.Progression;
using Assist.Objects.AssistApi.Valorant;
using Assist.Objects.Helpers;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Serilog;
using ValNet.Objects.Contracts;
using ValNet.Objects.Exceptions;
using ValNet.Objects.Player;

namespace Assist.Views.Dashboard.ViewModels
{
    internal class DashboardViewModel
    {
        private static List<Mission> allMissions = null;
        private static Dictionary<string, MatchHistoryObj> _matchHistory = new Dictionary<string, MatchHistoryObj>();
        private static Dictionary<string, List<MatchDetailsObj>> MatchDetailsdata = new Dictionary<string, List<MatchDetailsObj>>();
        private ContactsFetchObj _userContacts = null;
        private MatchHistoryObj _matchHistoryObj = null;
        public async Task<IEnumerable<MissionControl>> GetDailyMissions()
        {
            if(_userContacts is null)
                _userContacts = await AssistApplication.Current.CurrentUser.Contracts.GetAllContracts();

            if (allMissions is null)
                allMissions = await AssistApplication.ApiService.GetAllMissions();

            var date = DateTime.Now.AddDays(1);


            var dailyMissions = _userContacts.Missions.FindAll(_mission => (_mission.ExpirationTime.Day == date.Day) || (_mission.ExpirationTime.Day == DateTime.Now.Day));

            List<MissionControl> controls = new List<MissionControl>();

            for (int i = 0; i < dailyMissions.Count; i++)
            {
                var missionData = allMissions.Find(_m => dailyMissions[i].ID == _m.Uuid);

                controls.Add(new MissionControl()
                {
                    Title = missionData.Title,
                    CurrentProgress = dailyMissions[i].Objectives.First().Value,
                    MaxProgress = missionData.ProgressToComplete,
                    XpGrantAmount = $"{missionData.XpGrant}XP",
                    PreviewText = $"{dailyMissions[i].Objectives.First().Value}/{missionData.ProgressToComplete}"
                });
            }

            return controls;
        }

        public async Task<IEnumerable<MissionControl>> GetWeeklyMissions()
        {
            if (_userContacts is null)
                _userContacts = await AssistApplication.Current.CurrentUser.Contracts.GetAllContracts();

            if (allMissions is null)
                allMissions = await AssistApplication.ApiService.GetAllMissions();

            var date = DateTime.Now.AddDays(1);


            var dailyMissions = _userContacts.Missions.FindAll(_mission => (_mission.ExpirationTime.Day != date.Day) || (_mission.ExpirationTime.Day != DateTime.Now.Day));

            List<MissionControl> controls = new List<MissionControl>();

            for (int i = 0; i < dailyMissions.Count; i++)
            {
                var missionData = allMissions.Find(_m => dailyMissions[i].ID == _m.Uuid);

                controls.Add(new MissionControl()
                {
                    Title = missionData.Title,
                    CurrentProgress = dailyMissions[i].Objectives.First().Value,
                    MaxProgress = missionData.ProgressToComplete,
                    XpGrantAmount = $"{missionData.XpGrant}XP",
                    PreviewText = $"{dailyMissions[i].Objectives.First().Value}/{missionData.ProgressToComplete}"
                });
            }

            return controls;
        }

        public async Task<MatchHistoryObj> GetMatchHistory()
        {
            if(_matchHistory.ContainsKey(AssistApplication.Current.CurrentUser.UserData.sub))
            {
                return _matchHistory[AssistApplication.Current.CurrentUser.UserData.sub];
            }

            try
            {
                var matchHistory = await AssistApplication.Current.CurrentUser.Player.GetPlayerMatchHistory();
                _matchHistory.Add(AssistApplication.Current.CurrentUser.UserData.sub, matchHistory);
                return matchHistory;
            }
            catch (Exception e)
            {
                if (e is RequestException)
                {
                    var t = e as RequestException;
                    Log.Error(t.Content);
                    Log.Error($"{t.StatusCode}");
                }
                Log.Error(e.Message);
                return null;
            }
        }

        public async Task SetupCompetitiveDetails(PlayerStatisticsView playerStatisticsView)
        {
            try
            {
                var playerMmr = await AssistApplication.Current.CurrentUser.Player.GetPlayerMmr();

                int currentRankTier = 0;
                int currentRR = 0;
                var currentSeasonId = playerMmr.LatestCompetitiveUpdate.SeasonID;
                if (playerMmr.QueueSkills.competitive.SeasonalInfoBySeasonID != null)
                {
                    currentRankTier = playerMmr.QueueSkills.competitive.SeasonalInfoBySeasonID[currentSeasonId].CompetitiveTier;
                    currentRR = playerMmr.QueueSkills.competitive.SeasonalInfoBySeasonID[currentSeasonId].RankedRating;
                    playerStatisticsView.SeasonWins =
                        $"{playerMmr.QueueSkills.competitive.SeasonalInfoBySeasonID[currentSeasonId].NumberOfWins} Wins";
                }

                if (currentRankTier >= 24) playerStatisticsView.PlayerRR = $"{currentRR}RR";
                else playerStatisticsView.PlayerRR = $"{currentRR}/100 RR";
                playerStatisticsView.PlayerRankIcon = $"https://content.assistapp.dev/ranks/TX_CompetitiveTier_Large_{currentRankTier}.png";
                if (currentRankTier != null)
                    playerStatisticsView.RankName = CompetitiveNames.RankNames[currentRankTier].ToUpper();
            }
            catch (Exception e)
            {
                if (e is RequestException)
                {
                    var t =e as RequestException;
                    Log.Error(t.Content);
                    Log.Error($"{t.StatusCode}");
                }
                Log.Error(e.Message);
            }
        }

        public async Task<string> GetMostCommonAgent(List<MatchDetailsObj> details)
        {
            Dictionary<string, int> agentsPlayed = new Dictionary<string, int>();

            

            if (details.Count == 0)
                return "";

            foreach (var match in details)
            {
                var currentP = match.Players.Find(player => player.Subject == AssistApplication.Current.CurrentUser.UserData.sub);

                if (currentP != null)
                {
                    var hasBeenPlayed = agentsPlayed.ContainsKey(currentP.CharacterId);

                    if (hasBeenPlayed)
                    {
                        var curr = agentsPlayed[currentP.CharacterId];
                        curr += 1;
                        agentsPlayed[currentP.CharacterId] = curr;
                    }
                    else
                    {
                        agentsPlayed.Add(currentP.CharacterId, 1);
                    }
                }
            }

            var mostPlayedKey = agentsPlayed.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            return mostPlayedKey;
        }

        public async Task<List<MatchDetailsObj>> GetMatchDetails(MatchHistoryObj matchHistory)
        {
            if (MatchDetailsdata.ContainsKey(AssistApplication.Current.CurrentUser.UserData.sub))
            {
                return MatchDetailsdata[AssistApplication.Current.CurrentUser.UserData.sub];
            }

            var matches = new List<History>();
            for (int i = 0; i < matchHistory.History.Count && i < 4; i++) // Get up to the first 4 matches
            {
                matches.Add(matchHistory.History[i]);
            }

            var matchDetails = new List<MatchDetailsObj>();
            try
            {
                var r = matches.Select(
                        match => AssistApplication.Current.CurrentUser.Player.GetMatchDetails(match.MatchID))
                    .ToList();

                await Task.WhenAll(r); // Wait for All Match Requests

                return r.Select(_quest => _quest.Result).ToList();
            }
            catch (Exception e)
            {
                if (e is RequestException)
                {
                    var t = e as RequestException;
                    Log.Error(t.Content);
                    Log.Error($"{t.StatusCode}");
                }
                Log.Error(e.Message);
            }

            return matchDetails;
        }

        public async Task<List<MatchPreviewControl>> CreateMatchControls(List<MatchDetailsObj> details)
        {
            List<MatchPreviewControl> controls = new List<MatchPreviewControl>();

            if (details.Count == 0)
                return controls;

            foreach (var match in details)
            {
                var control = new MatchPreviewControl();
                var currentP = match.Players.Find(player => player.Subject == AssistApplication.Current.CurrentUser.UserData.sub);
                var theSetWeAreRepping = currentP.TeamId;


                if (currentP != null)
                {
                    control.PlayerAgent =
                        $"https://content.assistapp.dev/agents/{currentP.CharacterId}_displayicon.png";

                    control.ExtraData = $"{currentP.Stats.Kills} / {currentP.Stats.Deaths} / {currentP.Stats.Assists}";
                }

                var ourTeam = match.Teams.Find(team => team.TeamId == theSetWeAreRepping);
                var notOurTeam = match.Teams.Find(team => team.TeamId != ourTeam.TeamId);
                if (ourTeam != null)
                {
                    // Determine if our team won
                        // if not 
                        if (ourTeam.Won)
                        {
                            control.MatchScore = $"{ourTeam.RoundsWon} - {notOurTeam.RoundsWon}";
                            control.ResultText = Properties.Resources.Profile_WonText;
                            control.MatchWin = true;
                        }
                        else{
                            control.MatchScore = $"{notOurTeam.RoundsWon} - {ourTeam.RoundsWon}";
                            control.ResultText = Properties.Resources.Profile_LossText;
                            control.MatchWin = false;
                        }
                }

                if (match.MatchInformation != null)
                {
                    control.MatchMap = MapNames.MapsByPath?[match.MatchInformation.MapId].ToUpper();
                }

                controls.Add(control);
            }

            return controls;
        }


        
    }
}