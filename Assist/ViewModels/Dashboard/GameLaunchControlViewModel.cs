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

    [ObservableProperty] private string _imageLink = "https://images.contentstack.io/v3/assets/bltb6530b271fddd0b1/blt771c027adeb49ffb/65f4c76f55464d0a8f0dfe10/8-2-Clove-Gameplay-Thumb-16x9_textless.jpg?width=600&quality=75";

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
        await new RiotClientService().ApplyLauncherFiles(); // Instance currently being used is stored within the class Instance Variable

    }
    
}