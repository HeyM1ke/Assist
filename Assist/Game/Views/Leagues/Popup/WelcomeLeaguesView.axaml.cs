using Assist.Services.Popup;
using Assist.Settings;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Views.Leagues.Popup;

public partial class WelcomeLeaguesView : UserControl
{
    public WelcomeLeaguesView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ReadyBtn_Click(object? sender, RoutedEventArgs e)
    {
        AssistSettings.Current.SeenLeaguesNP = true;
        AssistSettings.Save();
        PopupSystem.KillPopups();
    }
}