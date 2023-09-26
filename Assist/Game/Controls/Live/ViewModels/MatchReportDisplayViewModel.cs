using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Assist.Game.Models.Recent;
using Assist.Objects.Enums;
using Assist.Objects.Helpers;
using Assist.Services.Utils;
using Assist.Settings;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;

namespace Assist.Game.Controls.Live.ViewModels;

public class MatchReportDisplayViewModel : ViewModelBase
{
    public RecentMatch? RecentMatchData = null;
    
    private static IBrush VictoryGreen = new SolidColorBrush(new Color(204, 92, 255, 177));
    private static IBrush LossRed = new SolidColorBrush(new Color(204, 255, 75, 75));
    private static IBrush RemakeYellow = new SolidColorBrush(new Color(255, 249, 224, 0));
    private static IBrush AllyBlue = new SolidColorBrush(new Color(255, 75, 169, 255));
    private static IBrush EnemyRed = new SolidColorBrush(new Color(255, 246, 30, 81));
    private static IBrush DefaultWhite = new SolidColorBrush(new Color(255, 222, 227, 232));

    #region Variables
    private string _mapImage;

    public string MapImage
    {
        get => _mapImage;
        set => this.RaiseAndSetIfChanged(ref _mapImage, value);
    }
    
    private string _statusText;

    public string StatusText
    {
        get => _statusText;
        set => this.RaiseAndSetIfChanged(ref _statusText, value);
    }

    private IBrush? _specialColor = DefaultWhite;

    public IBrush? SpecialColor
    {
        get => _specialColor;
        set => this.RaiseAndSetIfChanged(ref _specialColor, value);
    }

    private bool _scoreVisible = false;

    public bool ScoreVisible
    {
        get => _scoreVisible;
        set => this.RaiseAndSetIfChanged(ref _scoreVisible, value);
    }
    
    private string _allyScore = "0";

    public string AllyScore
    {
        get => _allyScore;
        set => this.RaiseAndSetIfChanged(ref _allyScore, value);
    }
    private string _enemyScore = "0";

    public string EnemyScore
    {
        get => _enemyScore;
        set => this.RaiseAndSetIfChanged(ref _enemyScore, value);
    }
    
    private string _gameMode = "";

    public string GameMode
    {
        get => _gameMode;
        set => this.RaiseAndSetIfChanged(ref _gameMode, value);
    }
    
    private string _dateOfMatch = "00/00/00";

    public string DateOfMatch
    {
        get => _dateOfMatch;
        set => this.RaiseAndSetIfChanged(ref _dateOfMatch, value);
    }
    
    private string _lengthOfMatch = "00:00:00";

    public string LengthOfMatch
    {
        get => _lengthOfMatch;
        set => this.RaiseAndSetIfChanged(ref _lengthOfMatch, value);
    }
    
    private string _gameModeIcon = "0";

    public string GameModeIcon
    {
        get => _gameModeIcon;
        set => this.RaiseAndSetIfChanged(ref _gameModeIcon, value);
    }

    private List<MatchReportTeamDisplayControl>? _teamControls = new List<MatchReportTeamDisplayControl>();
    
    public List<MatchReportTeamDisplayControl>? TeamControls
    {
        get => _teamControls;
        set => this.RaiseAndSetIfChanged(ref _teamControls, value);
    }
    
    #endregion

    public async Task SetupDisplay()
    {
        if (RecentMatchData is null)
            return;
        
        MapImage = $"https://content.assistapp.dev/maps/{MapNames.MapsByPath[RecentMatchData.MapId.ToLower()]}_BWlistview.png";
        
        
        // Determine Match State
        switch (RecentMatchData.Result)
        {
            case RecentMatch.MatchResult.VICTORY:
                SpecialColor = VictoryGreen;
                StatusText = "VICTORY";
                break;
            case RecentMatch.MatchResult.LOSS:
                SpecialColor = LossRed;
                StatusText = "LOSS";
                break;
            case RecentMatch.MatchResult.REMAKE:
                SpecialColor = RemakeYellow;
                StatusText = "REMAKE";
                break;
            default:
                SpecialColor = DefaultWhite;
                StatusText = "In Progress";
                break;
        }

        AllyScore = RecentMatchData.AllyTeamScore.ToString();
        EnemyScore = RecentMatchData.AllyTeamScore.ToString();
        GameMode = QueueNames.DetermineQueueKey(RecentMatchData.QueueId.ToLower());
        
        var language = AssistSettings.Current.Language;
        var attribute = language.GetAttribute<LanguageAttribute>();
        DateOfMatch = RecentMatchData.DateOfMatch.ToLocalTime().ToString("M/d/yy", new CultureInfo(attribute.Code));
        LengthOfMatch = TimeSpan.FromSeconds(RecentMatchData.LengthOfMatchInSeconds).ToString("g",new CultureInfo(attribute.Code));

        GenerateTeamObjects();

    }

    private void GenerateTeamObjects()
    {
        if (RecentMatchData.Result == RecentMatch.MatchResult.REMAKE)
        {
            var teamObj = new MatchReportTeamDisplayControl()
            {
                TeamName = string.IsNullOrEmpty(RecentMatchData.AllyTeamName)
                    ? "Ally Team"
                    : RecentMatchData.AllyTeamName,
                TeamColor = SpecialColor,
                TeamId = RecentMatchData.AllyTeamId,
                TeammateControls = new ObservableCollection<MatchReportTeammateDisplayControl>()
            };

            for (int i = 0; i < RecentMatchData.Players.Count; i++)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var teamMateObj = new MatchReportTeammateDisplayControl()
                    {
                        AgentIcon =
                            $"https://content.assistapp.dev/agents/{RecentMatchData.Players[i].PlayerAgentId}_displayicon.png",
                        TeammateName =
                            $"{RecentMatchData.Players[i].PlayerName}#{RecentMatchData.Players[i].PlayerTag}",
                        RankIcon =
                            $"https://content.assistapp.dev/ranks/TX_CompetitiveTier_Large_{RecentMatchData.Players[i].CompetitiveTier}.png",
                        Statline =
                            $"{RecentMatchData.Players[i].Statistics.Kills} // {RecentMatchData.Players[i].Statistics.Deaths} // {RecentMatchData.Players[i].Statistics.Assists}"
                    };

                    teamObj.TeammateControls.Add(teamMateObj);
                });
            }
            
            TeamControls.Add(teamObj);
            return;
        }
        
        if (RecentMatchData.QueueId.Equals("deathmatch", StringComparison.OrdinalIgnoreCase))
        {
            var teamObj = new MatchReportTeamDisplayControl()
            {
                TeamName = string.IsNullOrEmpty(RecentMatchData.AllyTeamName)
                    ? "Players"
                    : RecentMatchData.AllyTeamName,
                TeamColor = SpecialColor,
                TeammateControls = new ObservableCollection<MatchReportTeammateDisplayControl>()
            };

            foreach (var player in RecentMatchData.Players)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var teamMateObj = new MatchReportTeammateDisplayControl()
                    {
                        AgentIcon =
                            $"https://content.assistapp.dev/agents/{player.PlayerAgentId}_displayicon.png",
                        TeammateName = $"{player.PlayerName}#{player.PlayerTag}",
                        RankIcon =
                            $"https://content.assistapp.dev/ranks/TX_CompetitiveTier_Large_{player.CompetitiveTier}.png",
                        Statline =
                            $"{player.Statistics.Kills} // {player.Statistics.Deaths} // {player.Statistics.Assists}"
                    };

                    
                    teamObj.TeammateControls.Add(teamMateObj);
                });
            }
            
            TeamControls.Add(teamObj);
            return;
        }

        if (RecentMatchData.Result == RecentMatch.MatchResult.LOSS || RecentMatchData.Result == RecentMatch.MatchResult.VICTORY || RecentMatchData.Result == RecentMatch.MatchResult.IN_PROGRESS)
        {

            Dictionary<string, MatchReportTeamDisplayControl> displayControls =
                new Dictionary<string, MatchReportTeamDisplayControl>();

            foreach (var player in RecentMatchData.Players)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var teamMateObj = new MatchReportTeammateDisplayControl()
                    {
                        AgentIcon =
                            $"https://content.assistapp.dev/agents/{player.PlayerAgentId}_displayicon.png",
                        TeammateName = $"{player.PlayerName}#{player.PlayerTag}",
                        RankIcon =
                            $"https://content.assistapp.dev/ranks/TX_CompetitiveTier_Large_{player.CompetitiveTier}.png",
                        Statline =
                            $"{player.Statistics.Kills} // {player.Statistics.Deaths} // {player.Statistics.Assists}"
                    };
                
                    if (displayControls.TryGetValue(player.TeamId, out MatchReportTeamDisplayControl teamDisplayControl))
                    {
                        teamDisplayControl.TeammateControls.Add(teamMateObj);
                    }
                    else
                    {
                        MatchReportTeamDisplayControl displayControl = new MatchReportTeamDisplayControl()
                        {
                            TeamName = RecentMatchData.AllyTeamId.Equals(player.TeamId, StringComparison.OrdinalIgnoreCase)
                                ? "Ally Team"
                                : "Enemy Team",
                            TeamColor = RecentMatchData.AllyTeamId.Equals(player.TeamId, StringComparison.OrdinalIgnoreCase)
                                ? AllyBlue
                                : EnemyRed,
                            TeamId = player.TeamId,
                            TeammateControls = new ObservableCollection<MatchReportTeammateDisplayControl>()
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
                TeamControls = controls;
            });
        }
        
    }
}