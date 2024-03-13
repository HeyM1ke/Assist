using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Assist.Controls.Game.MatchTrack;
using Assist.Core.Helpers;
using Assist.Models.Enums;
using Assist.Shared.Models.Assist;
using Assist.Shared.Services.Utils;
using Assist.Shared.Settings;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Assist.ViewModels.Game;

public partial class MatchTrackMatchViewModel : ViewModelBase
{
    [ObservableProperty] private RecentMatch? _recentMatchData;

    private static IBrush VictoryGreen = new SolidColorBrush(new Color(204, 92, 255, 177));
    private static IBrush LossRed = new SolidColorBrush(new Color(204, 255, 75, 75));
    private static IBrush RemakeYellow = new SolidColorBrush(new Color(255, 249, 224, 0));
    private static IBrush AllyBlue = new SolidColorBrush(new Color(255, 75, 169, 255));
    private static IBrush EnemyRed = new SolidColorBrush(new Color(255, 246, 30, 81));
    private static IBrush DefaultWhite = new SolidColorBrush(new Color(255, 222, 227, 232));
    
    [ObservableProperty] private string _localPlayerStats;    
    [ObservableProperty] private string _localPlayerAgentIcon;
    [ObservableProperty] private string _mapImage;
    [ObservableProperty]private string _statusText;
    [ObservableProperty]private IBrush? _specialColor = DefaultWhite;
    [ObservableProperty]private bool _scoreVisible = false;
    [ObservableProperty]private bool _inProgress = false;
    [ObservableProperty]private string _allyScore = "0";
    [ObservableProperty]private string _enemyScore = "0";
    [ObservableProperty]private string _dateOfMatch = "00/00/00";
    [ObservableProperty]private string _gameMode = "";
    [ObservableProperty]private string _lengthOfMatch = "00:00:00";
    [ObservableProperty]private string _gameModeIcon = "0";
    [ObservableProperty]private ObservableCollection<MatchTrackTeamShowcaseControl>? _teamControls = new ObservableCollection<MatchTrackTeamShowcaseControl>();
     public async Task SetupDisplay()
    {
        if (RecentMatchData is null)
            return;
        
        MapImage = $"https://cdn.assistval.com/maps/{ValorantHelper.MapsByPath[RecentMatchData.MapId.ToLower()]}_BWlistview.png";
        
        
        // Determine Match State
        switch (RecentMatchData.Result)
        {
            case RecentMatch.MatchResult.VICTORY:
                SpecialColor = VictoryGreen;
                ScoreVisible = true;
                InProgress = false;
                StatusText = "VICTORY";
                break;
            case RecentMatch.MatchResult.LOSS:
                SpecialColor = LossRed;
                ScoreVisible = true;
                InProgress = false;
                StatusText = "LOSS";
                break;
            case RecentMatch.MatchResult.REMAKE:
                SpecialColor = RemakeYellow;
                StatusText = "REMAKE";
                InProgress = false;
                break;
            default:
                SpecialColor = DefaultWhite;
                ScoreVisible = true;
                InProgress = true;
                StatusText = "In Progress";
                break;
        }

        AllyScore = RecentMatchData.AllyTeamScore.ToString();
        EnemyScore = RecentMatchData.EnemyTeamScore.ToString();

        if (RecentMatchData.QueueId.Equals("hurm") || RecentMatchData.QueueId.Equals("deathmatch"))
            ScoreVisible = false;
        
        GameMode = ValorantHelper.DetermineQueueKey(RecentMatchData.QueueId.ToLower());
        
        var language = AssistSettings.Default.Language;
        var attribute = language.GetAttribute<LanguageAttribute>();
        DateOfMatch = RecentMatchData.DateOfMatch.ToLocalTime().ToString("M/d/yy");
        LengthOfMatch = TimeSpan.FromSeconds(RecentMatchData.LengthOfMatchInSeconds).ToString("hh\\:mm\\:ss",new CultureInfo(attribute.Code));

        SetupLocalUserData();

        RecentMatchData.Players = RecentMatchData.Players.OrderByDescending(x => x.Statistics?.Kills).ToList();
        
        GenerateTeamObjects();
    }

    private void SetupLocalUserData()
    {
        var localUserData = RecentMatchData.Players.Find(x => x.PlayerId == AssistApplication.ActiveUser.UserData.sub); // Find Local Player to display stats on top.

        if (localUserData != null)
        {
            LocalPlayerAgentIcon = $"https://cdn.assistval.com/agents/{localUserData.PlayerAgentId.ToLower()}_displayicon.png";
            LocalPlayerStats = localUserData.Statistics is not null
                ? $"{localUserData.Statistics.Kills} / {localUserData.Statistics.Deaths} / {localUserData.Statistics.Assists}"
                : "";
        }
    }

    private void GenerateTeamObjects()
    {
        if (RecentMatchData.Result == RecentMatch.MatchResult.REMAKE)
        {
            var teamObj = new MatchTrackTeamShowcaseControl()
            {
                TeamName = string.IsNullOrEmpty(RecentMatchData.AllyTeamName)
                    ? "Ally Team"
                    : RecentMatchData.AllyTeamName,
                TeamColor = SpecialColor,
                TeamId = RecentMatchData.AllyTeamId,
                TeammateControls = new ObservableCollection<MatchTrackTeammateDisplayControl>()
            };

            foreach (var player in RecentMatchData.Players)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    
                    string name = $"{player.PlayerName}#{player.PlayerTag}";
                    if (RecentMatchData.Result == RecentMatch.MatchResult.IN_PROGRESS)
                    {
                        name = $"{player.PlayerName}#{player.PlayerTag}";
                    }
                    else if(player.PlayerName.Equals("player", StringComparison.OrdinalIgnoreCase))
                    {
                        name = $"{player.PlayerName}";
                    }
                    else if (!string.IsNullOrEmpty(player.PlayerRealName) && RecentMatchData.Result != RecentMatch.MatchResult.IN_PROGRESS)
                    {
                        name = player.PlayerRealName;
                    }
                    var teamMateObj = new MatchTrackTeammateDisplayControl()
                    {
                        AgentIcon =
                            $"https://cdn.assistval.com/agents/{player.PlayerAgentId}_displayicon.png",
                        TeammateName = name,
                        RankIcon =
                            $"https://cdn.assistval.com/ranks/TX_CompetitiveTier_Large_{player.CompetitiveTier}.png",
                        Statline = player.Statistics is not null ? $"{player.Statistics.Kills} // {player.Statistics.Deaths} // {player.Statistics.Assists}" : ""
                    };

                    if (string.IsNullOrEmpty(player.PlayerAgentId))
                        teamMateObj.AgentIcon = $"https://cdn.assistval.com/agents/unknown_displayicon.png";
                   
                    teamObj.TeammateControls.Add(teamMateObj);
                });
            }
            
            
           
            
            TeamControls.Add(teamObj);
            return;
        }
        
        if (RecentMatchData.QueueId.Equals("deathmatch", StringComparison.OrdinalIgnoreCase))
        {
            var teamObj = new MatchTrackTeamShowcaseControl()
            {
                TeamName = string.IsNullOrEmpty(RecentMatchData.AllyTeamName)
                    ? "Players"
                    : RecentMatchData.AllyTeamName,
                TeamColor = SpecialColor,
                TeammateControls = new ObservableCollection<MatchTrackTeammateDisplayControl>()
            };

            foreach (var player in RecentMatchData.Players)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    string name = $"{player.PlayerName}#{player.PlayerTag}";
                    if (RecentMatchData.Result == RecentMatch.MatchResult.IN_PROGRESS)
                    {
                        name = $"{player.PlayerName}#{player.PlayerTag}";
                    }
                    else if (!string.IsNullOrEmpty(player.PlayerRealName) && RecentMatchData.Result != RecentMatch.MatchResult.IN_PROGRESS)
                    {
                        name = player.PlayerRealName;
                    }
                    
                    var teamMateObj = new MatchTrackTeammateDisplayControl()
                    {
                        AgentIcon =
                            $"https://cdn.assistval.com/agents/{player.PlayerAgentId}_displayicon.png",
                        TeammateName = name,
                        RankIcon =
                            $"https://cdn.assistval.com/ranks/TX_CompetitiveTier_Large_{player.CompetitiveTier}.png",
                        Statline =
                            player.Statistics is not null ? $"{player.Statistics.Kills} // {player.Statistics.Deaths} // {player.Statistics.Assists}" : ""
                    };

                    
                    
                    
                    teamObj.TeammateControls.Add(teamMateObj);
                });
            }
            
            TeamControls.Add(teamObj);
            return;
        }

        if (RecentMatchData.Result == RecentMatch.MatchResult.LOSS || RecentMatchData.Result == RecentMatch.MatchResult.VICTORY || RecentMatchData.Result == RecentMatch.MatchResult.IN_PROGRESS)
        {

            Dictionary<string, MatchTrackTeamShowcaseControl> displayControls =
                new Dictionary<string, MatchTrackTeamShowcaseControl>();

            foreach (var player in RecentMatchData.Players)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    string name = $"{player.PlayerName}#{player.PlayerTag}";
                    if (RecentMatchData.Result == RecentMatch.MatchResult.IN_PROGRESS)
                    {
                        name = $"{player.PlayerName}#{player.PlayerTag}";
                    }
                    else if (!string.IsNullOrEmpty(player.PlayerRealName) && RecentMatchData.Result != RecentMatch.MatchResult.IN_PROGRESS)
                    {
                        name = player.PlayerRealName;
                    }
                    
                    var teamMateObj = new MatchTrackTeammateDisplayControl()
                    {
                        AgentIcon =
                            $"https://cdn.assistval.com/agents/{player.PlayerAgentId}_displayicon.png",
                        TeammateName = name,
                        RankIcon =
                            $"https://cdn.assistval.com/ranks/TX_CompetitiveTier_Large_{player.CompetitiveTier}.png",
                        Statline =
                            player.Statistics is not null ? $"{player.Statistics.Kills} // {player.Statistics.Deaths} // {player.Statistics.Assists}" : ""
                    };

                   
                    
                
                    if (displayControls.TryGetValue(player.TeamId, out MatchTrackTeamShowcaseControl teamDisplayControl))
                    {
                        teamDisplayControl.TeammateControls.Add(teamMateObj);
                    }
                    else
                    {
                        MatchTrackTeamShowcaseControl displayControl = new MatchTrackTeamShowcaseControl()
                        {
                            TeamName = RecentMatchData.AllyTeamId.Equals(player.TeamId, StringComparison.OrdinalIgnoreCase)
                                ? "Ally Team"
                                : "Enemy Team",
                            TeamColor = RecentMatchData.AllyTeamId.Equals(player.TeamId, StringComparison.OrdinalIgnoreCase)
                                ? AllyBlue
                                : EnemyRed,
                            TeamId = player.TeamId,
                            TeammateControls = new ObservableCollection<MatchTrackTeammateDisplayControl>()
                        };
                    
                        displayControl.TeammateControls.Add(teamMateObj);
                        displayControls.TryAdd(player.TeamId, displayControl);
                    }
                });
            }

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var controls = displayControls.Values.ToList();
                if (controls.Count > 1)
                {
                    var index = controls.FindIndex(x =>
                        x.TeamId.Equals(RecentMatchData.AllyTeamId, StringComparison.OrdinalIgnoreCase));

                    var tp = controls[0];
                    controls[0] = controls[index];
                    controls[index] = tp;
                }
                TeamControls = new ObservableCollection<MatchTrackTeamShowcaseControl>(controls);
            });
        }
        
    }
}