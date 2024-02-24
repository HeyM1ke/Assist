using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assist.Controls.Assist.Authentication;
using Assist.Controls.Navigation;
using Assist.Controls.Setup;
using Assist.Models.Enums;
using Assist.Services.Navigation;
using Assist.Views.Startup;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Assist.ViewModels.Assist;

public partial class AssistAuthenticationViewModel : ViewModelBase
{
    [ObservableProperty] 
    private Control _currentContent = new Control();

    private List<string> _sequenceHistory = new List<string>();

    private Dictionary<string, Control> _sequenceControls = new Dictionary<string, Control>();

    public AssistAuthenticationViewModel()
    {
        _sequenceControls.Add(nameof(AssistAccountLoginForm), new AssistAccountLoginForm(OpenRegisterWindowCommand, AssistAccountCompletedCommand));
        
        _sequenceControls.Add(nameof(AssistAccountDuelForm), new AssistAccountDuelForm(OpenLoginWindowCommand, AssistAccountCompletedCommand));
        CurrentContent = _sequenceControls[nameof(AssistAccountLoginForm)];
    }

    private bool _controlsAdded = false;
   
    
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

        AssistApplication.ChangeMainWindowPopupView(null);
        NavigationContainer.ViewModel.ChangePage(AssistPage.UNKNOWN);
        if (AssistApplication.CurrentMode == EAssistMode.LAUNCHER)
            await AssistApplication.SetupComplete_Launcher();
        else 
            await AssistApplication.SetupComplete_Game();
    }

    [RelayCommand]
    public async Task CloseLoginWindow()
    {
        _sequenceHistory.Clear();
        _sequenceControls.Clear();
        GC.Collect();
        
        AssistApplication.ChangeMainWindowPopupView(null);
    }
}