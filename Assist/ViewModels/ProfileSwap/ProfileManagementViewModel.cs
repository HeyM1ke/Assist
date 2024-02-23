using System;
using System.Linq;
using System.Windows.Input;
using Assist.Controls.Infobars;
using Assist.Controls.Navigation;
using Assist.Properties;
using Assist.Services.Riot;
using Assist.Shared.Settings.Accounts;
using Assist.Views.ProfileSwap;
using Assist.Views.RAccount;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.ViewModels.ProfileSwap;

public partial class ProfileManagementViewModel : ViewModelBase
{
    [ObservableProperty] private string _profileId;
    [ObservableProperty] private ICommand? _closePopup;

    [ObservableProperty] private string _profilePlayercard;
    [ObservableProperty] private string _profileRiotName;
    
    [ObservableProperty] private bool _assistEnabled;
    [ObservableProperty] private bool _gameLaunchEnabled;
    [ObservableProperty] private bool _gameLaunchButtonEnabled = true;
    [ObservableProperty] private string _gameLaunchButtonText = Resources.Common_Enable;
    [ObservableProperty] private bool _accountExpired;
    [ObservableProperty] private bool _defaultAccount;
    public async void Setup()
    {
        var profile = AccountSettings.Default.Accounts.FirstOrDefault(x => x.Id == ProfileId);

        ProfileRiotName = profile.Personalization.RiotId;
        ProfilePlayercard =  $"https://content.assistapp.dev/playercards/{profile.Personalization.PlayerCardId}_DisplayIcon.png";
        
        GameLaunchEnabled = profile.CanLauncherBoot;
        AssistEnabled = profile.CanAssistBoot;
        AccountExpired = profile.IsExpired;
        DefaultAccount = AccountSettings.Default.DefaultAccount.Equals(ProfileId, StringComparison.OrdinalIgnoreCase);

        if (!profile.CanLauncherBoot)
        {
            var riotPath = await RiotClientService.FindRiotClient();
            if (string.IsNullOrEmpty(riotPath))
            {
                GameLaunchButtonEnabled = false;
                GameLaunchButtonText = Resources.ProfileManager_NoRiotClient;
            }
            
        }
    }

    [RelayCommand]
    public void OpenGameLaunchWindow()
    {
        AssistApplication.ChangeMainWindowPopupView(null);
        
        Dispatcher.UIThread.Invoke(() =>
        {
            Titlebar.ViewModel.AccountSwapVisible = false;
            NavigationContainer.ViewModel.HideAllButtons();
        });
        AssistApplication.ChangeMainWindowView(new RAccountAddPage(true));
    }
    
    [RelayCommand]
    public void RemoveProfile()
    {
        
        var profile = AccountSettings.Default.Accounts.FirstOrDefault(x => x.Id == ProfileId);

        var t = AccountSettings.Default.Accounts.Remove(profile);

        if (t)
        {
            Log.Information("Successfully removed profile");
            AccountSettings.Save();
        }

        if (AssistApplication.ActiveAccountProfile.Id.Equals(ProfileId))
        {
            Log.Information("The profile removed was the currently logged in profile.");

            if (AccountSettings.Default.Accounts.Count == 0)
            {
                Log.Information("No more profiles available. Going to Add Page");
                AssistApplication.ChangeMainWindowPopupView(null);
        
                Dispatcher.UIThread.Invoke(() =>
                {
                    Titlebar.ViewModel.AccountSwapVisible = false;
                    NavigationContainer.ViewModel.HideAllButtons();
                });
                AssistApplication.ChangeMainWindowView(new RAccountAddPage());
                return;
            }

            var acc = AccountSettings.Default.Accounts.FirstOrDefault(x => x.CanAssistBoot);
            AssistApplication.ChangeMainWindowPopupView(null);
            Dispatcher.UIThread.Invoke(() =>
            {
                Titlebar.ViewModel.AccountSwapVisible = false;
                Titlebar.ViewModel.SettingsEnabled = false;
                NavigationContainer.ViewModel.HideAllButtons();
            
            });
        
            AssistApplication.ChangeMainWindowView(new SwapPage(acc.Id));
            
            return;
        }
        
        AssistApplication.ChangeMainWindowPopupView(new ProfileSwapView());
    }
    
    [RelayCommand]
    public void OpenAssistNormalLoginWindow()
    {
        AssistApplication.ChangeMainWindowPopupView(null);
        
        Dispatcher.UIThread.Invoke(() =>
        {
            Titlebar.ViewModel.AccountSwapVisible = false;
            NavigationContainer.ViewModel.HideAllButtons();
        });
        AssistApplication.ChangeMainWindowView(new RAccountAddPage());
    }

    public void MakeAccountDefault()
    {
        AccountSettings.Default.DefaultAccount = ProfileId;
        AccountSettings.Save();
    }
}
