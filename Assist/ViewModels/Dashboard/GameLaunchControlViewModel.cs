using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Assist.Controls.General;
using Assist.Controls.Navigation;
using Assist.Core.Settings.Options;
using Assist.Services.Riot;
using Assist.Shared.Settings;
using Assist.Shared.Settings.Accounts;
using Assist.Views.Extras;
using Assist.Views.Game;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.ViewModels.Dashboard;

public partial class GameLaunchControlViewModel : ViewModelBase
{
    [ObservableProperty] private bool _launchBtnEnabled = false;

    [ObservableProperty] private string _imageLink =
        "https://www.dexerto.com/cdn-image/wp-content/uploads/2024/01/10/valorant-episode-8-act-1-release-date.jpg?width=600&quality=75";

    private AccountProfile? _currentlySelected;

    public GameLaunchControlViewModel()
    {
        if (Design.IsDesignMode) return;
    }
    
    public async Task LoadProfiles()
    {
        _currentlySelected = AssistApplication.ActiveAccountProfile;
        
        //TODO: Loop through all profiles and add them to the display
        
        CheckButtonEnableState();
    }

    private void CheckButtonEnableState()
    {
        if (_currentlySelected is null) return;
        LaunchBtnEnabled = _currentlySelected.CanLauncherBoot;
    }

    [RelayCommand]
    public async Task LaunchButtonClick()
    {
        Log.Information("Launch Button Clicked");
        NavigationContainer.ViewModel.DisableAllButtons();
        AssistApplication.ChangeMainWindowPopupView(new VLRLaunchedPage());
        Log.Information("Popup View changed to showcase that Valorant is Booting.");
        
        Log.Information("Setting up Riot Client Files");
        await ApplyLauncherFiles();
        
        
    }

    
    private static readonly string defaultConfigLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Riot Client", "Data");
    private static readonly string defaultBetaConfigLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games", "Beta", "Data");
    
    private async Task ApplyLauncherFiles()
    {
        if (!File.Exists(_currentlySelected.BackupZipPath))
        {
            Log.Error("Launcher files dont exist");

            AssistApplication.ChangeMainWindowPopupView(new ErrorMessagePopup());
            NavigationContainer.ViewModel.EnableAllButtons();
            return;
        }
        
        Log.Information("Backup File Exists.");
        try
        {
            await RiotClientService.CloseRiotRelatedPrograms();
            Log.Information("Deleting Files");
            RemoveAnyExistingClientDataFiles();
            var _currentDataFolderPath = Path.Exists(defaultBetaConfigLocation) ? defaultBetaConfigLocation : defaultConfigLocation;
            ZipFile.ExtractToDirectory(_currentlySelected.BackupZipPath, _currentDataFolderPath, true);
        }
        catch (Exception e)
        {
           Log.Error(e.Message);
           Log.Error(e.StackTrace);
           return;
        }
        string clientLocation = await RiotClientService.FindRiotClient();
            
        if (clientLocation == null)
            Log.Error("DID NOT FIND CLIENT");

        Log.Information("Launching VALORANT.");
            
        ProcessStartInfo riotClientStart = new ProcessStartInfo(clientLocation, $"--launch-product=valorant --launch-patchline=live")
        {
            UseShellExecute = true
        };

        Process.Start(riotClientStart);
        await Task.Delay(1000);
        var serv = new RiotClientService();
        serv.StartWorker();
    }

    private void RemoveAnyExistingClientDataFiles()
    {
        if (Directory.Exists(defaultConfigLocation))
        {
            DirectoryInfo di = new DirectoryInfo(defaultConfigLocation);
            foreach (var filePath in di.GetFiles())
            {
                filePath.Delete();
            }
            // removed any currently logged in client
        }

        if (Directory.Exists(defaultBetaConfigLocation))
        {
            DirectoryInfo di = new DirectoryInfo(defaultBetaConfigLocation);
            foreach (var filePath in di.GetFiles())
            {
                filePath.Delete();
            }
            // removed any currently logged in client
        }
    }
}