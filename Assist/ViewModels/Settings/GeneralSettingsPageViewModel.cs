using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Assist.Core.Settings.Options;
using Assist.Models.Enums;
using Assist.Shared.Settings;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.ViewModels.Settings;

public partial class GeneralSettingsPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<ComboBoxItem> _resolutionItems = new ObservableCollection<ComboBoxItem>();
    
    [ObservableProperty] private int _resolutionIndex = 0;
    [ObservableProperty] private int _modeIndex = 0;

    [ObservableProperty]private bool _setupOngoing = true;
    public async Task Setup()
    {
        LoadResolutions();
    }
    
    public void LoadResolutions()
    {

        ResolutionIndex = (int)AssistSettings.Default.SelectedResolution + 2;
        ModeIndex = (int)AssistSettings.Default.AppType;
        SetupOngoing = !SetupOngoing;
    }

    public void SetResolution(EResolution resolution)
    {
        if (SetupOngoing)return;
        
        if (AssistSettings.Default.SelectedResolution != resolution)
        {
            AssistApplication.ChangeMainWindowResolution(resolution);
            AssistSettings.Default.SelectedResolution = resolution;
            AssistSettings.Save();
            //AssistApplication.OpenMainWindowToSettings();
        }
    }
    
    public void SetMode(AssistApplicationType appType)
    {
        if (SetupOngoing)return;
        
        if (AssistSettings.Default.AppType != appType)
        {
            AssistSettings.Default.AppType = appType;
            AssistSettings.Save();
        }
    }
    
    [RelayCommand]
    public async void CheckForUpdates()
    {
        var updateAvailable = await AssistApplication.CheckForUpdates();
        if (updateAvailable)
        {
            Log.Information("New Version Available. Showing to Update Page");
            AssistApplication.OpenUpdateWindow();
            return;
        }
        
    }
}
