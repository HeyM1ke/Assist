using Assist.Controls.Navigation;
using Assist.Models.Enums;
using Assist.Services.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.ViewModels.Modules;

public partial class ModulesViewModel : ViewModelBase
{
    [ObservableProperty] private bool _isGameMode =  AssistApplication.CurrentMode == EAssistMode.GAME;
    
    
    [RelayCommand]
    public void OpenDodgeView()
    {
        Log.Information("Player has asked to open the dodge module");
        
        NavigationContainer.ViewModel.ChangePage(AssistPage.DODGE);
    }
    
    [RelayCommand]
    public void OpenDiscordView()
    {
        Log.Information("Player has asked to open the dodge module");
        
        NavigationContainer.ViewModel.ChangePage(AssistPage.DISCORD);
    }
}