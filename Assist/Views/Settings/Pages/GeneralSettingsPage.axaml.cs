using System;
using System.Collections.Generic;
using Assist.Objects.Enums;
using Assist.Services;
using Assist.Settings;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
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

        private void WindowSizeBox_OnInitialized(object? sender, EventArgs e)
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Window mainRef = desktop.MainWindow;
                if (mainRef.Screens.Primary.WorkingArea.Height <= 1080)
                {
                    var comboBox = sender as ComboBox;

                    var list = comboBox.Items as AvaloniaList<Object>;

                    list.RemoveAt(list.Count-1);

                    comboBox.Items = list;
                }
            }
        }
    }
}
