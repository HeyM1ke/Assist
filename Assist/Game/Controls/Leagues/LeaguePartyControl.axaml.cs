using System;
using Assist.Game.Controls.Leagues.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Controls.Leagues;

public partial class LeaguePartyControl : UserControl
{
    private readonly LeaguePartyControlViewModel _viewModel;

    public LeaguePartyControl()
    {
        DataContext = _viewModel = new LeaguePartyControlViewModel();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void PartyControl_Init(object? sender, EventArgs e)
    {
        if (Design.IsDesignMode)
            return;
        await _viewModel.Initialize();
    }

    private async void Control_OnUnloaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode)
            return;
        await _viewModel.UnbindToEvents();
    }
}