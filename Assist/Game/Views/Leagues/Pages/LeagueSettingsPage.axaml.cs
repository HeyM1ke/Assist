using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.Leagues.Pages;

public partial class LeagueSettingsPage : UserControl
{
    public LeagueSettingsPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void GeneralSettings_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }

    private void PlayersSettings_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }
}