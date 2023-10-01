using Assist.Game.Services;
using Assist.Game.Views.Leagues.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.Leagues;

public partial class PostMatchResultView : UserControl
{
    private readonly string _matchId;
    private readonly PostMatchPageViewModel _viewModel;
    public PostMatchResultView()
    {
        GameViewNavigationController.CurrentPage = Page.POSTMATCH;
        DataContext = _viewModel = new PostMatchPageViewModel();
        InitializeComponent();
    }
    
    public PostMatchResultView(string matchId)
    {
        GameViewNavigationController.CurrentPage = Page.POSTMATCH;
        DataContext = _viewModel = new PostMatchPageViewModel();
        _matchId = matchId;
        InitializeComponent();
    }

    private async void PostMatchView_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode) return;
        
        await _viewModel.Setup(_matchId);
    }

    private void ReturnToLeaguesBtn_Click(object? sender, RoutedEventArgs e)
    {
        GameViewNavigationController.Change(new LeagueMainPage());
    }
}