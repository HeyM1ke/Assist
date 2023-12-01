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
        private TextBox riotClientAdditionalArgs;
        public GameLaunchSettingsPopup()
        {
            InitializeComponent();
        }

        private async void Apply_Click(object? sender, RoutedEventArgs e)
        {
            if(Design.IsDesignMode)return;
            
            AssistSettings.Current.GameModeEnabled = (bool)gameModeEnabled.IsChecked;

            AssistSettings.Current.AdditionalArgs = riotClientAdditionalArgs.Text ?? string.Empty;
            
            PopupSystem.KillPopups();
        }

        private async void GameLaunch_Init(object? sender, EventArgs e)
        {
            gameModeEnabled = this.FindControl<CheckBox>("GameModeEnable");
            riotClientAdditionalArgs = this.FindControl<TextBox>("RiotClientAdditionalArgs");
            gameModeEnabled.IsChecked = AssistSettings.Current.GameModeEnabled;
            riotClientAdditionalArgs.Text = AssistSettings.Current.AdditionalArgs;
        }
    }
}
