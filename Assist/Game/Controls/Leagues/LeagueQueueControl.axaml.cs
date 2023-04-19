using System;
using Assist.Game.Controls.Leagues.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Controls.Leagues;

public partial class LeagueQueueControl : UserControl
{
    private readonly LeagueQueueViewModel _viewModel;

    public LeagueQueueControl()
    {
        DataContext = _viewModel = new LeagueQueueViewModel();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void QueueBtn_Click(object? sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        btn.IsEnabled = false;
        await _viewModel.ButtonClick();
        btn.IsEnabled = true;
    }

    private async void QueueControl_Init(object? sender, EventArgs e)
    {
        if (Design.IsDesignMode)
            return;
        await _viewModel.Setup();
    }
}