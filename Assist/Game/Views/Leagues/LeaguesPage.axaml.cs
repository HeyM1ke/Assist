using System;
using Assist.Game.Views.Leagues.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.Leagues;

public partial class LeaguesPage : UserControl
{
    private readonly LeaguePageViewModel _viewModel;

    public LeaguesPage()
    {
        DataContext = _viewModel = new LeaguePageViewModel();
        InitializeComponent();
    }
    
    public LeaguesPage(string leagueId)
    {
        InitializeComponent();
    }



    private async void LeaguesPage_Init(object? sender, EventArgs e)
    {
        if (Design.IsDesignMode)
        {
            return;
        }
        await _viewModel.Setup();
    }
}