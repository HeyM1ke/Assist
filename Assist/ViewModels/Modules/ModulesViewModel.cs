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
    [ObservableProperty] private bool _isAssistLoggedIn =  !string.IsNullOrEmpty(AssistApplication.AssistUser.userTokens.AccessToken);
    [ObservableProperty] private bool _accessToSocket =  AssistApplication.AssistUser.Authentication.Roles.Contains("ASS-socket-access");
    [ObservableProperty] private bool _accessToExtension =  AssistApplication.AssistUser.Authentication.Roles.Contains("ASS-extension-access");
    
    
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
    
    [RelayCommand]
    public void OpenSocketView()
    {
        Log.Information("Player has asked to open the socket module");
        
        NavigationContainer.ViewModel.ChangePage(AssistPage.ASSSOCKET);
    }
    
    [RelayCommand]
    public void OpenExtensionView()
    {
        Log.Information("Player has asked to open the Extension module");
        
        NavigationContainer.ViewModel.ChangePage(AssistPage.EXTENSION);
    }
}