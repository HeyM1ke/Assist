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
            if(Design.IsDesignMode)return;
            
            AssistSettings.Current.GameModeEnabled = (bool)gameModeEnabled.IsChecked;

            PopupSystem.KillPopups();
        }

        private async void GameLaunch_Init(object? sender, EventArgs e)
        {
            gameModeEnabled = this.FindControl<CheckBox>("GameModeEnable");

            gameModeEnabled.IsChecked = AssistSettings.Current.GameModeEnabled;
        }
    }
}
