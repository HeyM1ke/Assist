using System;
using Assist.Game.Views.Leagues.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.Leagues.Pages;

public partial class LeagueLeaderboardPage : UserControl
{
    private readonly LeagueLeaderboardPageViewModel _viewModel;

    public LeagueLeaderboardPage()
    {
        DataContext = _viewModel = new LeagueLeaderboardPageViewModel();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void LeaderboardMemberControl_Loaded(object? sender, RoutedEventArgs e)
    {
        _viewModel.SetupLeaderboard();
    }

    private void PlayerStats_Loaded(object? sender, RoutedEventArgs e)
    {
        _viewModel.SetupPlayerStats();
    }

    private void LeaderboardMemberControl_Initialize(object? sender, EventArgs e)
    {
        _viewModel.SetupLeaderboard();
    }
}