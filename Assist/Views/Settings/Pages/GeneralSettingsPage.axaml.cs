using Assist.Objects.Enums;
using Assist.Services;
using Assist.Settings;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Assist.Views.Settings.Pages
{
    public partial class GeneralSettingsPage : UserControl
    {
        private ComboBox _resComboBox;
        public GeneralSettingsPage()
        {
            SettingsViewNavigationController.CurrentPage = SettingsPages.GENERAL;
            InitializeComponent();
            ApplySettings();
        }

        private void ApplySettings()
        {
            _resComboBox = this.FindControl<ComboBox>("WindowSizeBox");
            _resComboBox.SelectedIndex = (int)AssistSettings.Current.SelectedResolution;
        }

        private void ApplyBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            bool changed = false;


            if (AssistSettings.Current.SelectedResolution != (EResolution)_resComboBox.SelectedIndex)
            {
                AssistApplication.Current.ChangeMainWindowResolution((EResolution)_resComboBox.SelectedIndex);
                AssistSettings.Current.SelectedResolution = (EResolution)_resComboBox.SelectedIndex; changed = true;
            }

            if(changed)
                AssistApplication.Current.OpenMainWindowToSettings();
        }
    }
}
