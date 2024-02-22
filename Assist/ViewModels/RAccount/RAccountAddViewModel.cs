using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Assist.Controls.Infobars;
using Assist.Controls.Navigation;
using Assist.Controls.RAccount;
using Assist.Services.Navigation;
using Assist.Services.Riot;
using Assist.Shared.Services.Utils;
using Assist.Shared.Settings.Accounts;
using Assist.Views.Dashboard;
using AssistUser.Lib.Account;
using AssistUser.Lib.V2;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using ValNet;
using ValNet.Objects.Authentication;

namespace Assist.ViewModels.RAccount;

public partial class RAccountAddViewModel : ViewModelBase
{
    [ObservableProperty] 
    private Control _currentContent = new Control();

    private List<string> _sequenceHistory = new List<string>();

    private Dictionary<string, Control> _sequenceControls = new Dictionary<string, Control>();


    [ObservableProperty]private bool _backButtonEnabled = false;

    public RAccountAddViewModel()
    {
        Titlebar.ViewModel.SettingsEnabled = false;
        NavigationContainer.ViewModel.ChangePage(AssistPage.UNKNOWN);
        CreateControls();
        
        _sequenceHistory.Add(nameof(RAccountMethodSelectionControl));
        CurrentContent = _sequenceControls[nameof(RAccountMethodSelectionControl)];
    }
    
    public RAccountAddViewModel(string username)
    {
        CreateControls();
        NavigationContainer.ViewModel.ChangePage(AssistPage.UNKNOWN);
        var ctr = _sequenceControls[nameof(RAccountUsernameLoginFormControl)] as RAccountUsernameLoginFormControl;
        ctr.UpdateUsernameFieldExternal(username);
        CurrentContent = ctr;
    }
    
    public RAccountAddViewModel(bool skipInital)
    {
        CreateControls();
        if (skipInital)
        {
            NavigationContainer.ViewModel.ChangePage(AssistPage.UNKNOWN);
            var ctr = _sequenceControls[nameof(RAccountSecondaryClientLoginControl)] as RAccountSecondaryClientLoginControl;
            CurrentContent = ctr;
        }
        
    }
    
    
    private void CreateControls()
    {
        _sequenceControls.Add(nameof(RAccountMethodSelectionControl), new RAccountMethodSelectionControl()
        {
            UsernameButtonCommand = UserButtonCommandCommand,
            ClientButtonCommand = ClientButtonCommandCommand,
            CloudExperimentCommand = CloudButtonCommandCommand
        });
        
        _sequenceControls.Add(nameof(RAccountUsernameLoginFormControl), new RAccountUsernameLoginFormControl(InitialLoginCompletedCommandCommand));
        _sequenceControls.Add(nameof(RAccountCloudControl), new RAccountCloudControl(InitialLoginCompletedCommandCommand)
        {
            
        });
        
        _sequenceControls.Add(nameof(RAccountClientLoginControl), new RAccountClientLoginControl());
        _sequenceControls.Add(nameof(RAccountSecondarySelectionControl), new RAccountSecondarySelectionControl()
        {
            YesSelectionCommand = ContinueSecondaryLoginCommandCommand ,
            NoSelectionCommand = NoSecondaryLoginCommandCommand
        });
        
        _sequenceControls.Add(nameof(RAccountSecondaryClientLoginControl), new RAccountSecondaryClientLoginControl(SecondaryLoginCompletedCommand));
    }

    [RelayCommand]
    private async Task UserButtonCommand()
    {
        Log.Information("User selected Username/Password Login, Switching to Page.");
        
        _sequenceHistory.Add(nameof(RAccountUsernameLoginFormControl));
        CurrentContent = _sequenceControls[nameof(RAccountUsernameLoginFormControl)];
    }
    
    [RelayCommand] 
    private async Task ClientButtonCommand()
    {
        Log.Information("User selected Username/Password Login, Switching to Page.");
        
        _sequenceHistory.Add(nameof(RAccountClientLoginControl));
        CurrentContent = _sequenceControls[nameof(RAccountClientLoginControl)];
    }
    
    [RelayCommand] 
    private async Task CloudButtonCommand()
    {
        Log.Information("User selected cloud Login, Switching to Page.");
        
        _sequenceHistory.Add(nameof(RAccountCloudControl));
        CurrentContent = _sequenceControls[nameof(RAccountCloudControl)];
    }
    
    
    [RelayCommand]
    private async Task InitialLoginCompletedCommand()
    {
       Log.Information("Initial Login Command Hit");
       Log.Information("Checking for Riot Client Installs");
       var riotPath = await RiotClientService.FindRiotClient();

       if (riotPath is null)
       {
           Log.Information("There are no Riot Client Installs Detected.");
           Log.Information("There are no Riot Client Installs Detected.");
           NoSecondaryLoginCommandCommand.Execute(null);
           return;
       }

       if (File.Exists(AssistApplication.ActiveAccountProfile.BackupZipPath))
       {
           
           AssistApplication.ActiveAccountProfile.CanLauncherBoot = true;
           await AccountSettings.Default.UpdateAccount(AssistApplication.ActiveAccountProfile);
           _sequenceControls.Clear();
           _sequenceHistory.Clear();
           GC.Collect();
        
           await AssistApplication.SetupComplete_Launcher();
           return;
       }
       
       Log.Information("There exists a Riot Client on the computer");
       Log.Information("Showing Options for Launch Options");
       
       _sequenceHistory.Add(nameof(RAccountSecondarySelectionControl));
       CurrentContent = _sequenceControls[nameof(RAccountSecondarySelectionControl)];
    }
    
    [RelayCommand] 
    private async Task NoSecondaryLoginCommand()
    {
        Log.Information("User Selected to not continue with secondary login");
        Log.Information("User is already setup as the ActiveUser in AssistApplication");
        
        Log.Information("Loading Dashboard...");
        
        _sequenceControls.Clear();
        _sequenceHistory.Clear();
        GC.Collect();
        
        await AssistApplication.SetupComplete_Launcher();
    }
    
    [RelayCommand] 
    private async Task ContinueSecondaryLoginCommand()
    {
        Log.Information("User Selected to continue with secondary login");
        
        Log.Information("Loading Secondary Client Login");
        
        _sequenceHistory.Add(nameof(RAccountSecondaryClientLoginControl));
        CurrentContent = _sequenceControls[nameof(RAccountSecondaryClientLoginControl)];
    }

    [RelayCommand]
    private async Task SecondaryLoginCompleted()
    {
        Log.Information("User completed secondary login");
        Log.Information("Loading Dashboard...");
        
        _sequenceControls.Clear();
        _sequenceHistory.Clear();
        GC.Collect();

        if (AccountSettings.Default.Accounts.Count == 1)
        {
            AccountSettings.Default.DefaultAccount = AccountSettings.Default.Accounts[0].Id;
            AccountSettings.Save();
        }

        await AssistApplication.SetupComplete_Launcher();
    }
    
}