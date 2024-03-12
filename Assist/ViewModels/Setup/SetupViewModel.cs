using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assist.Controls.Assist.Authentication;
using Assist.Controls.Setup;
using Assist.Core.Settings.Options;
using Assist.Models.Enums;
using Assist.Shared.Settings;
using Assist.Views.Startup;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.ViewModels.Setup;

public partial class SetupViewModel : ViewModelBase
{
    [ObservableProperty] 
    private Control _currentContent = new Control();

    private List<string> _sequenceHistory = new List<string>();

    private Dictionary<string, Control> _sequenceControls = new Dictionary<string, Control>();


    [ObservableProperty]private bool _backButtonEnabled = false;
    public SetupViewModel()
    {
        
        CurrentContent = new WelcomeSetupControl()
        {
            NextButtonCommand = WelcomeButtonCommandCommand
        };
        
        _sequenceControls.Add(nameof(LangSetupControl), new LangSetupControl()
        {
            LangButtonCommand = LangButtonCommandCommand
        });
    }
    
    
    [RelayCommand]
    public async Task BackButtonCommand()
    {
        Log.Information("Back Button Pressed, Moving to Back");
        
        var previousPage  = _sequenceHistory[_sequenceHistory.Count - 2];
        _sequenceHistory.Remove(_sequenceHistory.Last());
        BackButtonEnabled = _sequenceHistory.Count >= 2;
        CurrentContent = _sequenceControls[previousPage];
    }
    
    [RelayCommand]
    public async Task WelcomeButtonCommand()
    {
        Log.Information("Welcome Button Pressed, Moving to First Step!");
        BackButtonEnabled = _sequenceHistory.Count >= 2;

        _sequenceHistory.Add(nameof(LangSetupControl));
        CurrentContent = _sequenceControls[nameof(LangSetupControl)];
    }
    
    [RelayCommand]
    public async Task LangButtonCommand(string regionCode)
    {

        var num = Int32.Parse(regionCode);
        Log.Information("Language Selected: " + (ELanguage)num);

        AssistSettings.Default.Language = (ELanguage)num;
        
        App.ChangeLanguage();
        AssistSettings.Save();
        
        BackButtonEnabled = true;  
        
        CreateControls();
        
        
        _sequenceHistory.Add(nameof(ModeSelectionControl));
        CurrentContent = _sequenceControls[nameof(ModeSelectionControl)];
    }
    
    [RelayCommand]
    public async Task ModeSelectCommand(string modeCode)
    {

        var num = Int32.Parse(modeCode);
        Log.Information("Mode Selected: " + (AssistApplicationType)num);

        AssistSettings.Default.AppType = (AssistApplicationType)num;
        
        AssistSettings.Save();
        BackButtonEnabled = _sequenceHistory.Count >= 2;
        
        
        _sequenceHistory.Add(nameof(AssistAccountSelectionControl));
        CurrentContent = _sequenceControls[nameof(AssistAccountSelectionControl)];
    }
    
    [RelayCommand]
    public async Task AccountSelectCommand(string optionSelected)
    {
        Log.Information("User has selected an option on the account selection, setup is complete as user is overall done with program setup");
        AssistSettings.Default.CompletedSetup = true;
        AssistSettings.Save();
        Log.Information("Clearing all Controls from Memory");
        _sequenceHistory.Clear();
        _sequenceControls.Clear();
        GC.Collect();
        
        
        var num = Int32.Parse(optionSelected);

        if (num == 0)
        {
            Log.Information("User has selected to create an Assist account.");
            
            Log.Information("Creating Assist Account Controls");
            _sequenceControls.Add(nameof(AssistAccountDuelForm), new AssistAccountDuelForm(OpenLoginWindowCommand,AssistAccountCompletedCommand ));
            
            Log.Information("Loading Account Creation Control...");
            _sequenceHistory.Add(nameof(AssistAccountDuelForm));
            CurrentContent = _sequenceControls[nameof(AssistAccountDuelForm)];
            
            _sequenceControls.Add(nameof(AssistAccountLoginForm), new AssistAccountLoginForm(OpenRegisterWindowCommand, AssistAccountCompletedCommand));
            BackButtonEnabled = false;
            
            return;
        }
        
        Log.Information("User has Selected not to create an account.");
        Log.Information("Setup is now complete.");
        Log.Information("Assist Settings Setup Value: " + AssistSettings.Default.CompletedSetup);
        Log.Information("Switching Back to inital Setup");
        
        
        AssistApplication.ChangeMainWindowView(new StartupView());
    }
    
    [RelayCommand]
    public async Task OpenLoginWindow()
    {
        
        _sequenceHistory.Add(nameof(AssistAccountLoginForm));
        CurrentContent = _sequenceControls[nameof(AssistAccountLoginForm)];
    }
    
    [RelayCommand]
    public async Task OpenRegisterWindow()
    {
        _sequenceHistory.Add(nameof(AssistAccountDuelForm));
        CurrentContent = _sequenceControls[nameof(AssistAccountDuelForm)];
    }
    
    [RelayCommand]
    public async Task AssistAccountCompleted()
    {
        _sequenceHistory.Clear();
        _sequenceControls.Clear();
        GC.Collect();
        
        AssistApplication.ChangeMainWindowView(new StartupView());
    }

    private bool _controlsAdded = false;
    private void CreateControls()
    {
        if (_controlsAdded) return;
        
        _sequenceControls.Add(nameof(ModeSelectionControl), new ModeSelectionControl()
        {
            ModeSelectionCommand = ModeSelectCommandCommand
        });
        
        _sequenceControls.Add(nameof(AssistAccountSelectionControl), new AssistAccountSelectionControl()
        {
            AccountSelectionCommand = AccountSelectCommandCommand
        });
        
        _controlsAdded = true;
    }
}