using System;
using Assist.Controls.Global.Popup;
using Assist.Controls.Global.ViewModels;
using Assist.Services.Popup;
using Assist.Services.Riot;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Dashboard;

public partial class GameLaunchControlV2 : UserControl
{
    private readonly GameLaunchViewModel _viewModel;

    public GameLaunchControlV2()
    {
        DataContext = _viewModel = new GameLaunchViewModel();
        InitializeComponent();
    }

    private async void LaunchBtn_Click(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode)
            return;

        var btn = sender as Button;
        btn.IsEnabled = false;
        btn.Content = Properties.Resources.Global_Loading;
        await new RiotClientService().LaunchClient();
    }

    private void SettingsBtn_Click(object? sender, RoutedEventArgs e)
    {
        PopupSystem.SpawnCustomPopup(new GameLaunchSettingsPopup());
    }

    private async void GameLaunchControlV2_Init(object? sender, EventArgs e)
    {
        _viewModel.CheckEnable();
        var inv = await _viewModel.SetPlayercard();

        if (inv != null)
        {
            _viewModel.ProfilePlayercard = $"https://content.assistapp.dev/playercards/{inv.PlayerData.PlayerCardID}_DisplayIcon.png";
        }

        await _viewModel.CheckPatchlines();

        var t = this.FindControl<ComboBox>("PatchlineSelectionBox");
        t.SelectedIndex = 0;
    }

    private void PatchlineSelectionBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    { 
        AssistApplication.Current.ClientLaunchSettings.Patchline =
            ((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString();
    }
}