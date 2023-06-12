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

            _leaderboard = JsonSerializer.Deserialize<AssistLeaderboard>(r.Data.ToString());
            _currentLeagueId = LeagueService.Instance.CurrentLeagueId;
        }

        for (int i = 0; i < _leaderboard.Members.Count; i++)
        {
            MemberControls.Add(new LeagueLeaderboardMemberControl()
            {
                Width = 555,
                Height = 40,
                PositionText = $"#{_leaderboard.Members[i].Position:n0}",
                PlayerText = _leaderboard.Members[i].Username,
                LeaguePointText = $"{_leaderboard.Members[i].Points:n0} LP"
            });
        }
    }

    public async Task SetupPlayerStats()
    {
        if (ProfilePageViewModel.ProfileData is null)
        {
            var resp = await AssistApplication.Current.AssistUser.Profile.GetProfile();
            if (resp.Code != 200)
            {
                return;
            }

            ProfilePageViewModel.ProfileData = JsonSerializer.Deserialize<AssistProfile>(resp.Data.ToString());
        }
        
        var playerData =
            ProfilePageViewModel.ProfileData.Leagues.Find(x => x.Id == LeagueService.Instance.CurrentLeagueId);

        if (playerData is null)
        {
            CurrentGamesPlayed = "Failed";
            CurrentPlayerPoints = "Failed";
            return;
        }
        
        CurrentGamesPlayed = $"{playerData.Matches.Count:n0} Games Played";
        CurrentPlayerPoints = $"{playerData.CurrentLeaguePoints:n0} LP";
        PlayerProfileImage = ProfilePageViewModel.ProfileData.ProfileImage;
    }
}