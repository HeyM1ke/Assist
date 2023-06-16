using Assist.Game.Services;
using Assist.Game.Views.Leagues.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.Leagues;

public partial class MatchPage : UserControl
{
    private readonly MatchPageViewModel _viewModel;

    public MatchPage()
    {
        GameViewNavigationController.CurrentPage = Page.MATCH;
        DataContext = _viewModel = new MatchPageViewModel();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void MatchPage_Loaded(object? sender, RoutedEventArgs e)
    {
        // Get Match Data
        if (Design.IsDesignMode) return;

        await _viewModel.SetupBasePage();
    }

    private async void BanBtn_Click(object? sender, RoutedEventArgs e)
    {
        _viewModel.MapPickBanEnabled = false;
    }

    private async void LobbyJoinOrReadyBtn_Click(object? sender, RoutedEventArgs e)
    {
        
        await _viewModel.JoinOrReadyInMatch();
        
    }
}