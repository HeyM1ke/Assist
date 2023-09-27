using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Assist.Game.Services;
using Assist.ViewModels;
using Avalonia.Threading;
using DynamicData.Binding;
using ReactiveUI;
using Serilog;

namespace Assist.Game.Controls.Live.ViewModels;

public class MatchReportSlideoutViewModel : ViewModelBase
{
    private bool _slideOpen = false;

    public bool SlideOpen
    {
        get => _slideOpen;
        set => this.RaiseAndSetIfChanged(ref _slideOpen, value);
    }
    
    private ObservableCollectionExtended<MatchReportDisplayControl> _matchControls = new ObservableCollectionExtended<MatchReportDisplayControl>(){};

    public ObservableCollectionExtended<MatchReportDisplayControl> MatchControls
    {
        get => _matchControls;
        set => this.RaiseAndSetIfChanged(ref _matchControls, value);
    }

    private bool _binded = false;
    public async Task LoadMatches()
    {
        if (RecentService.Current.RecentMatches?.Count > 0)
        {
            for (int i = RecentService.Current.RecentMatches.Count-1; i >= 0; i--)
            {
                var mat = new MatchReportDisplayControl(RecentService.Current.RecentMatches[i]);
                MatchControls.Add(mat);    
            }
                
        }

        if (!_binded)
        {
            RecentService.Current.RecentServiceUpdated += RecentServiceUpdated; 
        }
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
                        var mat = new MatchReportDisplayControl(RecentService.Current.RecentMatches[i]);
                        MatchControls.Insert(RecentService.Current.RecentMatches.Count - 1 - i, mat);
                    }
                    else
                    {
                        possibleControl.UpdateData(RecentService.Current.RecentMatches[i]);
                    }
                }
            }
        }
    }
}



// Bathroom & Stretch break.