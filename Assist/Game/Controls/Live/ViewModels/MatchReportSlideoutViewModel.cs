using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Assist.Game.Services;
using Assist.ViewModels;
using ReactiveUI;

namespace Assist.Game.Controls.Live.ViewModels;

public class MatchReportSlideoutViewModel : ViewModelBase
{
    private bool _slideOpen = false;

    public bool SlideOpen
    {
        get => _slideOpen;
        set => this.RaiseAndSetIfChanged(ref _slideOpen, value);
    }
    
    private ObservableCollection<MatchReportDisplayControl> _matchControls = new ObservableCollection<MatchReportDisplayControl>(){};

    public ObservableCollection<MatchReportDisplayControl> MatchControls
    {
        get => _matchControls;
        set => this.RaiseAndSetIfChanged(ref _matchControls, value);
    }
    
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
    }
    
}



// Bathroom & Stretch break.