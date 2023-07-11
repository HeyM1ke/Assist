using System;
using System.Linq;
using System.Reflection;
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
using Serilog;

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



    private void CloseBtn_Click(object? sender, RoutedEventArgs e)
    {
        PopupSystem.KillPopups();
    }
    
    private void WindowSizeBox_Init(object? sender, EventArgs e)
    {
        Log.Information("Window loaded");
        
            Log.Information("Is main window ok?");

            if (AssistApplication.CurrentApplication.MainWindow is null)
            {
                Log.Information("Main Window Is nulL");
                return;
            }
            
            Window mainRef = AssistApplication.CurrentApplication.MainWindow;
            Log.Information("Seems so");
            Log.Information("ComboBox Null?");
            var comboBox = sender as ComboBox;
            Log.Information("ComboBox Nope doesnt seem so");
            Log.Information("Checking MainRef");
            if (mainRef.Screens.Primary.WorkingArea.Height <= 1080)
            {
                Log.Information("Screen is 1080p or less");
                var li = comboBox.Items.Select(x => x as ComboBoxItem).ToList();
                li.RemoveAt(li.Count-1);
                li.RemoveAt(li.Count-1);
                comboBox.Items.Clear();
                comboBox.ItemsSource = li;
            }

            if ((int)AssistSettings.Current.SelectedResolution < -2 || (int)AssistSettings.Current.SelectedResolution > 3)
            {
                AssistSettings.Current.SelectedResolution = EResolution.R720;
            }
            
            comboBox.SelectedIndex = (int)AssistSettings.Current.SelectedResolution + 2;
            load = true;
        
        
    }

    private void EnableGPUBox_Loaded(object? sender, RoutedEventArgs e)
    {
        var comboBox = sender as CheckBox;
        comboBox.IsChecked = AssistSettings.Current.EglEnabled;
    }

    private void LanguageBox_Loaded(object? sender, RoutedEventArgs e)
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
        { return;
        }


        var _resComboBox = sender as ComboBox;
        if (AssistSettings.Current.SelectedResolution != (EResolution)(_resComboBox.SelectedIndex - 2))
        {
            AssistApplication.Current.ChangeMainWindowResolution((EResolution)(_resComboBox.SelectedIndex - 2));
            AssistSettings.Current.SelectedResolution = (EResolution)(_resComboBox.SelectedIndex - 2);
            AssistSettings.Save();
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

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        
        var tb = sender as TextBlock;

        tb.Text = $"Version: {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version}";
    }
}