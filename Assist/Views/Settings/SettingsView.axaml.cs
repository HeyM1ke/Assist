using System;
using System.Collections.Generic;
using Assist.Controls.Global.Navigation;
using Assist.Controls.Settings;
using Assist.Services;
using Assist.Views.Dashboard;
using Assist.Views.Settings.Pages;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Settings
{
    public partial class SettingsView : UserControl
    {

        List<SettingsNavigationButton> navigationButtons = new List<SettingsNavigationButton>();

        public SettingsView()
        {
            MainViewNavigationController.CurrentPage = Page.SETTINGS;
            InitializeComponent();
            SettingsViewNavigationController.ContentControl = this.FindControl<TransitioningContentControl>("ContentControl");
            NavigationBar.Instance.SetSelected(2);
            SetupButtons();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void SetupButtons()
        {
            navigationButtons.Add(this.FindControl<SettingsNavigationButton>("GeneralSettingsBtn"));
            navigationButtons.Add(this.FindControl<SettingsNavigationButton>("AccountSettingsBtn"));
            navigationButtons.Add(this.FindControl<SettingsNavigationButton>("AssistSettingsBtn"));
            navigationButtons.Add(this.FindControl<SettingsNavigationButton>("InfoSettingsBtn"));

            navigationButtons[0].IsSelected = true;
        }

        private void DeselectAllButtons(SettingsNavigationButton toSkip = null)
        {
            foreach (var button in navigationButtons)
            {
                if(button != toSkip)
                    button.IsSelected = false;
            }
        }

        private void GeneralSettingsBtn_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            DeselectAllButtons(sender as SettingsNavigationButton);

            if (SettingsViewNavigationController.CurrentPage != SettingsPages.GENERAL)
                SettingsViewNavigationController.Change(new GeneralSettingsPage());
        }

        private void AccountSettingsBtn_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            DeselectAllButtons(sender as SettingsNavigationButton);

            if (SettingsViewNavigationController.CurrentPage != SettingsPages.ACCOUNTS)
                SettingsViewNavigationController.Change(new AccountsSettingsPage());
        }

        private void AssistSettingsBtn_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            DeselectAllButtons(sender as SettingsNavigationButton);
            if (SettingsViewNavigationController.CurrentPage != SettingsPages.ASSISTSET)
                SettingsViewNavigationController.Change(new AssistSettingsPage());
        }

        private void InfoSettingsBtn_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            DeselectAllButtons(sender as SettingsNavigationButton);
            if (SettingsViewNavigationController.CurrentPage != SettingsPages.INFORMATION)
                SettingsViewNavigationController.Change(new InformationSettingsPage());
        }

        private void SettingsView_OnInitialized(object? sender, EventArgs e)
        {
            SettingsViewNavigationController.Change(new GeneralSettingsPage());
        }
    }
}