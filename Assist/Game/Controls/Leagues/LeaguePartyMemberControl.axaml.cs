using Assist.Game.Controls.Leagues.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Controls.Leagues;

public partial class LeaguePartyMemberControl : UserControl
{
    private readonly LeaguePartyMemberViewModel _viewModel;

    public LeaguePartyMemberControl()
    {
        DataContext = _viewModel = new LeaguePartyMemberViewModel();
        InitializeComponent();
    }

    public LeaguePartyMemberControl(object data)
    {
        DataContext = _viewModel = new LeaguePartyMemberViewModel();
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void UpdatePlayerData()
    {
        
    }
}