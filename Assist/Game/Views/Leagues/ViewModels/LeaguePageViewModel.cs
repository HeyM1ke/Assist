using System.Threading.Tasks;
using Assist.Game.Services;
using Assist.Game.Services.Leagues;
using Assist.Settings;
using Assist.ViewModels;
using AssistUser.Lib.Leagues.Models;
using ReactiveUI;
using Serilog;

namespace Assist.Game.Views.Leagues.ViewModels;

public class LeaguePageViewModel : ViewModelBase
{
    private string _leagueId;

    public string LeagueId
    {
        get => _leagueId;
        set => this.RaiseAndSetIfChanged(ref _leagueId, value);
    }
    
    public string VersionNumber
    {
        get => $"{AssistSettings.Current.ApplicationVersion} : In Development";
    }

    private bool _leaderboardEnabled = true;

    public bool LeaderboardEnabled
    {
        get => _leaderboardEnabled;
        set => this.RaiseAndSetIfChanged(ref _leaderboardEnabled, value);
    }
    
    private bool _moderationEnabled = false;

    public bool ModerationEnabled
    {
        get => _moderationEnabled;
        set => this.RaiseAndSetIfChanged(ref _moderationEnabled, value);
    }

    public async Task Setup()
    {
        Log.Information("Setting up LeaguePage");
        var l = await LeagueService.Instance.GetCurrentLeagueInformation();

        if (l.OwnerId == AssistApplication.Current.AssistUser.Account.AccountInfo.id)
        {
            ModerationEnabled = true;
        }
    }


}