using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Assist.ViewModels;
using Avalonia.Controls;
using ReactiveUI;

namespace Assist.Game.Controls.Leagues.ViewModels;

public class LeagueSelectionViewModel : ViewModelBase
{
    private bool popupOpen = false;
    public bool PopupOpen
    {
        get => popupOpen;
        set => this.RaiseAndSetIfChanged(ref popupOpen, value);
    }
        
    private ObservableCollection<UserControl> _leagueSelectionControls = new ObservableCollection<UserControl>();
    public ObservableCollection<UserControl> LeagueSelectionControls
    {
        get => _leagueSelectionControls;
        set => this.RaiseAndSetIfChanged(ref _leagueSelectionControls, value);
    }

    public async Task Setup()
    {
        // Set name of Current League using League Service
    }
}