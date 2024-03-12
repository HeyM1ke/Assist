using System;
using Assist.ViewModels.Modules;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Modules.Pages;

public partial class DiscordView : UserControl
{
    private readonly DiscordRPViewModel _viewModel;

    public DiscordView()
    {
        DataContext = _viewModel = new DiscordRPViewModel();
        InitializeComponent();
    }

    private void DiscordView_Init(object? sender, EventArgs e)
    {
        _viewModel.Setup();
    }

    private void DiscordEnabled_CheckChanged(object? sender, RoutedEventArgs e)
    {
        _viewModel.HandleDiscordEnableChange();
    }
}