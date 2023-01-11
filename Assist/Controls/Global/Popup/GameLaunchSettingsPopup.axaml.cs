using System;
using Assist.Services.Popup;
using Assist.Settings;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Assist.Controls.Global.Popup
{
    public partial class GameLaunchSettingsPopup : UserControl
    {
        private ComboBox patchLineBox;
        private CheckBox gameModeEnabled;
        public GameLaunchSettingsPopup()
        {
            InitializeComponent();
        }

        private async void Apply_Click(object? sender, RoutedEventArgs e)
        {
            AssistApplication.Current.ClientLaunchSettings.Patchline =
                (patchLineBox.SelectedItem as ComboBoxItem).Content.ToString();

            AssistSettings.Current.GameModeEnabled = (bool)gameModeEnabled.IsChecked;

            PopupSystem.KillPopups();
        }

        private async void GameLaunch_Init(object? sender, EventArgs e)
        {
            patchLineBox = this.FindControl<ComboBox>("PatchLineCheckbox");
            gameModeEnabled = this.FindControl<CheckBox>("GameModeEnable");

            gameModeEnabled.IsChecked = AssistSettings.Current.GameModeEnabled;

            //TEMP
            if (AssistApplication.Current.ClientLaunchSettings.Patchline == "PBE")
                patchLineBox.SelectedIndex = 1;
        }
    }
}
