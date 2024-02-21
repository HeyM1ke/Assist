using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Game.Controls.Leagues;
using Assist.Game.Services.Leagues;
using Assist.Game.Views.Profile.ViewModels;
using Assist.ViewModels;
using AssistUser.Lib.Leagues.Models;
using AssistUser.Lib.Profiles.Models;
using Avalonia.Controls;
using Avalonia.Layout;
using ReactiveUI;
using Serilog;

namespace Assist.Game.Views.Leagues.ViewModels;

public class LeagueLeaderboardPageViewModel : ViewModelBase
{
    private static AssistLeaderboard? _leaderboard = null;
    private static string _currentLeagueId;
    private ObservableCollection<LeagueLeaderboardMemberControl> _memberControls = new ObservableCollection<LeagueLeaderboardMemberControl>();

    public ObservableCollection<LeagueLeaderboardMemberControl> MemberControls
    {
        get => _memberControls;
        set => this.RaiseAndSetIfChanged(ref _memberControls, value);
    }

    private ObservableCollection<LeagueLeaderboardMemberControl> _currentPlayerStats = new ObservableCollection<LeagueLeaderboardMemberControl>();

    public ObservableCollection<LeagueLeaderboardMemberControl> CurrentPlayerStats
    {
        get => _currentPlayerStats;
        set => this.RaiseAndSetIfChanged(ref _currentPlayerStats, value);
    }
    
    
    private string _currentPlayerPoints;

    public string CurrentPlayerPoints
    {
        get => _currentLeagueId;
        set => this.RaiseAndSetIfChanged(ref _currentLeagueId, value);
    }

    private string _currentGamesPlayed;

    public string CurrentGamesPlayed
    {
        get => _currentLeagueId;
        set => this.RaiseAndSetIfChanged(ref _currentLeagueId, value);
    }
    
    private string _playerProfileImage;

    public string PlayerProfileImage
    {
        get => _playerProfileImage;
        set => this.RaiseAndSetIfChanged(ref _playerProfileImage, value);
    }
    public async Task SetupLeaderboard()
    {
        MemberControls.Clear();
        if (_leaderboard is null || !string.Equals(_currentLeagueId, LeagueService.Instance.CurrentLeagueId))
        {
            var r = await AssistApplication.Current.AssistUser.League.GetLeaderboard(LeagueService.Instance.CurrentLeagueId);
            if (r.Code != 200)
            {
                Log.Error("Failed to get leaderboard.");
                _leaderboard = new AssistLeaderboard()
                {
                    Members = new List<AssistLeaderboardMember>(),
                    LastUpdated = DateTime.UtcNow
                };
                return;
            }

            var data = r.Data.ToString();
            _leaderboard = JsonSerializer.Deserialize<AssistLeaderboard>(data);
            _currentLeagueId = LeagueService.Instance.CurrentLeagueId;
        }

        for (int i = 0; i < _leaderboard.Members.Count; i++)
        {
            MemberControls.Add(new LeagueLeaderboardMemberControl()
            {
                Height = 45,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                PositionText = $"#{_leaderboard.Members[i].Position:n0}",
                PlayerText = _leaderboard.Members[i].Username,
                LeaguePointText = $"{_leaderboard.Members[i].Points:n0} LP",
                GamesPlayed = $"{_leaderboard.Members[i].GamesPlayed} {Properties.Resources.Leagues_LeaderboardMatchesPlayed}"
            });
        }

        if (_leaderboard.CallerPosition is not null)
        {
            CurrentPlayerStats.Add(new LeagueLeaderboardMemberControl()
            {
                Height = 70,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                PositionText = $"#{_leaderboard.CallerPosition.Position:n0}",
                PlayerText = _leaderboard.CallerPosition.Username,
                LeaguePointText = $"{_leaderboard.CallerPosition.Points:n0} LP",
                GamesPlayed = $"{_leaderboard.CallerPosition.GamesPlayed} {Properties.Resources.Leagues_LeaderboardMatchesPlayed}"
            });
        }
    }
}