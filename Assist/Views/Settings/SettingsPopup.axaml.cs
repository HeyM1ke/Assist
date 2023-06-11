using System;
using Assist.Game.Views.Authentication;
using Assist.Game.Views.LinkRiot;
using Assist.Objects.Enums;
using Assist.Services;
using Assist.Services.Popup;
using Assist.Settings;
using Assist.ViewModels;
using Assist.Views.Settings.ViewModels;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Settings;

public partial class SettingsPopup : UserControl
{
    private bool load = false;
    private bool load2 = false;
    private bool load3 = false;
    private readonly SettingsPopupViewModel _viewModel;

    public SettingsPopup()
    {
        DataContext = _viewModel = new SettingsPopupViewModel();
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
    
    private void WindowSizeBox_OnInitialized(object? sender, EventArgs e)
    {
        if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Window mainRef = desktop.MainWindow;
            var comboBox = sender as ComboBox;
            if (mainRef.Screens.Primary.WorkingArea.Height <= 1080)
            {
                var list = comboBox.Items as AvaloniaList<Object>;

                list.RemoveAt(list.Count-2);

                comboBox.Items = list;
            }
            
            comboBox.SelectedIndex = (int)AssistSettings.Current.SelectedResolution + 2;
        }
        
        
    }

    private void EnableGPUBox_OnInitialized(object? sender, EventArgs e)
    {
        var comboBox = sender as CheckBox;
        comboBox.IsChecked = AssistSettings.Current.EglEnabled;
    }

    private void LanguageBox_OnInitialized(object? sender, EventArgs e)
    {
        var langBox = sender as ComboBox;
        langBox.SelectedIndex = (int)AssistSettings.Current.Language;
    }

    private void LanguageBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!load2)
        {
            load2 = true; return;
        }
        
        
        var _settComboBox = sender as ComboBox;
        
        if (AssistSettings.Current.Language != (ELanguage)_settComboBox.SelectedIndex)
        {
            AssistSettings.Current.Language = (ELanguage)_settComboBox.SelectedIndex;
            App.ChangeLanguage();
        }
    }

    private void WindowSizeBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!load)
        {
            load = true; return;
        }


        var _resComboBox = sender as ComboBox;
        if (AssistSettings.Current.SelectedResolution != (EResolution)(_resComboBox.SelectedIndex - 2))
        {
            AssistApplication.Current.ChangeMainWindowResolution((EResolution)(_resComboBox.SelectedIndex - 2));
            AssistSettings.Current.SelectedResolution = (EResolution)(_resComboBox.SelectedIndex - 2);
            AssistApplication.Current.OpenMainWindowToSettings();
        }
        
        
    }

    private void EnableGPUBox_OnChecked(object? sender, RoutedEventArgs e)
    {
        AssistSettings.Current.EglEnabled = true;
    }

    private void EnableGPUBox_OnUnchecked(object? sender, RoutedEventArgs e)
    {
        AssistSettings.Current.EglEnabled = false;
    }

    private void SignInBtn_Click(object? sender, RoutedEventArgs e)
    {
        MainWindowContentController.Change(new AssistAuthenticationView());
        PopupSystem.KillPopups();
    }

    private async void SignedInGrid_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode)
            return;
        
        await _viewModel.SetupSignedInGrid();
    }

    private void SignOutBtn_Click(object? sender, RoutedEventArgs e)
    {
        
    }

    private void LinkRiotAccBtn_Click(object? sender, RoutedEventArgs e)
    {
        PopupSystem.SpawnCustomPopup(new LinkRiotPopupView());
    }
}