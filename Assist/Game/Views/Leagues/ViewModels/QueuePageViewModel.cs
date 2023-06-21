using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Assist.Game.Controls.Leagues;
using Assist.Game.Services;
using Assist.Game.Services.Leagues;
using Assist.Services;
using Assist.ViewModels;
using Assist.Views.Store;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using Serilog;

namespace Assist.Game.Views.Leagues.ViewModels;

public class QueuePageViewModel : ViewModelBase
{
    
    private ObservableCollection<Control> _playerControls = new ObservableCollection<Control>();
    
    public ObservableCollection<Control> PlayerControls
    {
        get => _playerControls;
        set => this.RaiseAndSetIfChanged(ref _playerControls, value);
    }
    
    private string _timer = "00:00";
    public string Timer
    {
        get => _timer;
        set => this.RaiseAndSetIfChanged(ref _timer, value);
    }
    
    public async Task LeaveQueue()
    {
        Log.Information("Button pressed to DEQUEUE ");
        var r = await AssistApplication.Current.AssistUser.League.LeaveQueue(LeagueService.Instance.CurrentLeagueId);

        if (r.Code != 200)
        {
            Log.Information("Failed to Leave Queue");
            Log.Information($"Response: {r.Code}");
            Log.Information(r.Message);
            return;
        }
        
        Log.Information("Left Queue");
        Log.Information(r.Message);
    }

    public async Task Setup()
    {
        for (int i = 0; i < LeagueService.Instance.CurrentPartyInfo.Members.Count; i++)
        {
            PlayerControls.Add(new QueuePlayerPreview()
            {
                PlayerName = LeagueService.Instance.CurrentPartyInfo.Members[i].DisplayName,
                ImageUrl = LeagueService.Instance.CurrentPartyInfo.Members[i].ProfileImage
            });
        }

        await StartTimer();

        try
        {
            await MatchService.UnreadyClient();
        }
        catch (Exception e)
        {
            
        }
    }
    
    
    private DispatcherTimer bundleTimer;
    private int bunSec = 0;
    private async Task StartTimer()
    {
        bundleTimer = new DispatcherTimer();
        bundleTimer.Tick += TimerTick;
        bundleTimer.Interval = TimeSpan.FromSeconds(1);
        bundleTimer.Start();
    }

    private void TimerTick(object? sender, EventArgs e)
    {
        bunSec += 1;
        var t = TimeSpan.FromSeconds(bunSec);
        Timer = t.ToString(@"mm\:ss");

    }
}