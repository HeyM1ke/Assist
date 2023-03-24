using Assist.Services.Popup;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Settings;

public partial class SettingsPopup : UserControl
{
    public SettingsPopup()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void CloseBtn_Click(object? sender, RoutedEventArgs e)
    {
        PopupSystem.KillPopups();
    }
}