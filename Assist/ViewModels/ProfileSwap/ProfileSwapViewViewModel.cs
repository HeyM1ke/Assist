using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Assist.Controls.Infobars;
using Assist.Controls.Navigation;
using Assist.Controls.ProfileSwap;
using Assist.Controls.Store;
using Assist.Models.Enums;
using Assist.Shared.Settings.Accounts;
using Assist.Views.Dashboard;
using Assist.Views.ProfileSwap;
using Assist.Views.RAccount;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.ViewModels.ProfileSwap;

public partial class ProfileSwapViewViewModel : ViewModelBase
{
    
    [ObservableProperty] private ObservableCollection<Control> _profileControls = new ObservableCollection<Control>();
    
    public async Task Setup()
    {
        NavigationContainer.ViewModel.DisableAllButtons();
        Titlebar.ViewModel.AccountSwapEnabled = false;
        Titlebar.ViewModel.AccountSwapEnabled = false;
        Log.Information("Setting up ProfileSwapView Controls");
        for (int i = 0; i < AccountSettings.Default.Accounts.Count; i++)
        {
            var ctr = new AccProfilePreviewControl()
            {
                PlayerName = string.IsNullOrEmpty(AccountSettings.Default.Accounts[i].Personalization.AccountNickName) ? AccountSettings.Default.Accounts[i].Personalization.RiotId : AccountSettings.Default.Accounts[i].Personalization.AccountNickName,
                AccountId = AccountSettings.Default.Accounts[i].Id,
                PlayerIconImage = $"https://content.assistapp.dev/playercards/{AccountSettings.Default.Accounts[i].Personalization.PlayerCardId}_DisplayIcon.png",
                AssistEnabled = AccountSettings.Default.Accounts[i].CanAssistBoot && !AccountSettings.Default.Accounts[i].IsExpired,
                GameLaunchEnabled = AccountSettings.Default.Accounts[i].CanLauncherBoot && !AccountSettings.Default.Accounts[i].IsExpired,
                IsExpired = AccountSettings.Default.Accounts[i].IsExpired,
                IsCurrent = AccountSettings.Default.Accounts[i].Id == AssistApplication.ActiveAccountProfile.Id && AccountSettings.Default.Accounts[i].CanAssistBoot,
                SwitchCommand = SwapAccountCommand
            };

            if (ctr.AssistEnabled == false)
            {
                ctr.IsCurrent = true; // this prevents swaps to assist accs
            }

            ProfileControls.Add(ctr);
        }
        
        ProfileControls.Add(new AccProfileAddControl()
        {
            Command = AddAccountCommand
        });
    }


    public async Task Unload()
    {
        ProfileControls.Clear();
        NavigationContainer.ViewModel.EnableAllButtons();
        Titlebar.ViewModel.AccountSwapEnabled = true;
        GC.Collect();
    }

    [RelayCommand]
    public async Task CloseView()
    {
        AssistApplication.ChangeMainWindowPopupView(null);
    }
    
    [RelayCommand]
    public async Task AddAccount()
    {
        Log.Information("Player wants to add an account, opening menu.");
        AssistApplication.ChangeMainWindowPopupView(null);
        
        Dispatcher.UIThread.Invoke(() =>
        {
            Titlebar.ViewModel.AccountSwapVisible = false;
            NavigationContainer.ViewModel.HideAllButtons();
        });
        AssistApplication.ChangeMainWindowView(new RAccountAddPage());
        
    }

    [RelayCommand]
    public async Task SwapAccount(string code)
    {
        Log.Information("Player is wanting to swap accounts.");
        
        if (string.IsNullOrEmpty(code)) return;
        
        AssistApplication.ChangeMainWindowPopupView(null);
        Dispatcher.UIThread.Invoke(() =>
        {
            Titlebar.ViewModel.AccountSwapVisible = false;
            Titlebar.ViewModel.SettingsEnabled = false;
            NavigationContainer.ViewModel.HideAllButtons();
            
        });
        
        AssistApplication.ChangeMainWindowView(new SwapPage(code));
    }
}