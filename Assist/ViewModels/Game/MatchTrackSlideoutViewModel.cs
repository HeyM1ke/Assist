using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Assist.Controls.Game.MatchTrack;
using Assist.Services.Assist;
using Assist.Shared.Models.Assist;
using Avalonia;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.ViewModels.Game;

public partial class MatchTrackSlideoutViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<MatchTrackMatchControl> _matchControls = new ObservableCollection<MatchTrackMatchControl>();
    [ObservableProperty] private bool _slideOpen = false;
    [ObservableProperty] private string _slideButtonText = "<";
    [ObservableProperty] private Thickness _thisAnnoyingPieceOfCrap;
    private bool _binded = false;

    [RelayCommand]
    public void OpenCloseSlidePanel()
    {
        SlideOpen = !SlideOpen;
        SlideButtonText = SlideOpen ? ">" : "<";
        ThisAnnoyingPieceOfCrap = SlideOpen ? new Thickness(0, 0, 5, 0) : new Thickness(0, 0, 0, 0);
        if (!SlideOpen)
            UnloadPage();
        else
            LoadPage();
    }
    
    public async Task LoadMatches()
    {
        if (RecentService.Current.RecentMatches?.Count > 0)
        {
            MatchControls.Clear();
            int limiter = 0;
            for (int i = RecentService.Current.RecentMatches.Count-1; i >= 0; i--)
            {
                MatchControls.Add(new MatchTrackMatchControl(RecentService.Current.RecentMatches[i]));
                limiter++;
                if (limiter == 5)
                {
                    i = 0;
                }
            }
                
        }

        RecentService.Current.RecentServiceUpdated -= RecentServiceUpdated; 
        RecentService.Current.RecentServiceUpdated += RecentServiceUpdated; 
    }
    
    private void RecentServiceUpdated()
    {
        Log.Information("Received Update Event to Update Values in Recent Matches List.");
        Dispatcher.UIThread.InvokeAsync(() => { RefreshList(); });
    }

    public async Task RefreshList()
    {
        if (RecentService.Current.RecentMatches?.Count > 0)
        {
            int limiter = 0;
            if (RecentService.Current.RecentMatches.Count != MatchControls.Count || RecentService.Current.RecentMatches.Any(x => !x.IsCompleted))
            {
                Log.Information("Refreshing Match Tracking Control.");
                
                // Check first for any MISSING matches.
                for (int i = RecentService.Current.RecentMatches.Count - 1; i >= 0; i--)
                {
                    var possibleControl = MatchControls.FirstOrDefault(x => x.MatchId.Equals(RecentService.Current.RecentMatches[i].MatchId));
                    if (possibleControl is null)
                    {
                        Log.Information("Control is Missing from List.");
                        var mat = new MatchTrackMatchControl(RecentService.Current.RecentMatches[i]);
                        MatchControls.Insert(RecentService.Current.RecentMatches.Count - 1 - i, mat);
                    }
                    else
                    {
                        possibleControl.UpdateData(RecentService.Current.RecentMatches[i]);
                    }
                    limiter++;
                    if (limiter == 5)
                    {
                        i = 0;
                    }
                }
            }
        }
    }

    public void LoadPage()
    {
        /*Log.Information("Page has been loaded, subbing from events.");
        RecentService.Current.RecentServiceUpdated += RecentServiceUpdated; */
    }
    public void UnloadPage()
    {
        /*Log.Information("Page has been unloaded, unsubbing from events.");
        RecentService.Current.RecentServiceUpdated -= RecentServiceUpdated; */
    }

    
}