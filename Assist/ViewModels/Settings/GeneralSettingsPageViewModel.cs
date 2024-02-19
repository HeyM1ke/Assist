using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

    private bool _setup = true;
    public async Task Setup()
    {
        LoadResolutions();
    }
    
    public void LoadResolutions()
    {

        var names = Enum.GetNames(typeof(EResolution));

        for (int i = 0; i < names.Length; i++)
        {
            
            
            ResolutionItems.Add(new ComboBoxItem()
            {
                Content = $"{names[i].Replace("R","")}P",
                Tag = i-2,
            });

            if (i-2 == (int)AssistSettings.Default.SelectedResolution)
            {
                ResolutionIndex = i;
            }
        }

        _setup = !_setup;
    }

    public void SetResolution(EResolution resolution)
    {
        if (_setup)return;
        
        if (AssistSettings.Default.SelectedResolution != resolution)
        {
            AssistApplication.ChangeMainWindowResolution(resolution);
            AssistSettings.Default.SelectedResolution = resolution;
            AssistSettings.Save();
            //AssistApplication.OpenMainWindowToSettings();
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