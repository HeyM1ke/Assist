using System;
using Assist.Game.Views.Profile.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Serilog;

namespace Assist.Game.Views.Profile.Pages;

public partial class BadgesPage : UserControl
{
    private readonly BadgePageViewModel _viewModel;

    public BadgesPage()
    {
        DataContext = _viewModel = new BadgePageViewModel();
        InitializeComponent();
    }




    private async void BadgePage_Init(object? sender, EventArgs e)
    {
        if (Design.IsDesignMode)
        {
            return;
        }
        await _viewModel.Setup();
    }

    private async void FeaturedBtn_Click(object? sender, RoutedEventArgs e)
    {
        Log.Information("Featured BTN Clicked");
        await _viewModel.UpdateFeaturedBadge(sender);
    }
}