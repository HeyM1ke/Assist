using System.Reflection;
using System.Threading.Tasks;
using Assist.Controls.Navigation;
using Assist.Models.Enums;
using Assist.Views.Assist;
using Assist.Views.ProfileSwap;
using Assist.Views.Settings.Pages;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Assist.ViewModels.Settings;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty] private string _assistVersion = $"Version: {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version}";
    
    [ObservableProperty] 
    private Control _currentContent = new Control();

    [ObservableProperty] private bool _generalChecked = true;
    [ObservableProperty]private SettingsPage _currentPage = SettingsPage.UNKNOWN;
    public SettingsViewModel()
    {
        
    }
    
    [RelayCommand]
    public void SwitchToGeneral()
    {
        if (CurrentPage != SettingsPage.GENERAL)
        {
            CurrentContent = new GeneralSettingsPageView();
            CurrentPage = SettingsPage.GENERAL;
            GeneralChecked = true;
        }
    }

    [RelayCommand]
    public void SwitchToSound()
    {
        if (CurrentPage != SettingsPage.SOUND)
        {
            CurrentContent = new TextBlock()
            {
                Text = "Sound"
            };   
            
            CurrentPage = SettingsPage.SOUND;
        }
    }
    
    [RelayCommand]
    public void SwitchToAssistAccount()
    {
        if (CurrentPage != SettingsPage.ASSACC)
        {
            CurrentContent = new AssistAccountSettingsPageView();   
            CurrentPage = SettingsPage.ASSACC;
        }
    }
    
    [RelayCommand]
    public void OpenRiotAccount()
    {
        if (AssistApplication.CurrentMode == EAssistMode.LAUNCHER)
        {
            AssistApplication.ChangeMainWindowPopupView(new ProfileSwapView());
            CurrentPage = SettingsPage.UNKNOWN;
            SwitchToGeneral();
        }
    }

    public void SetUnknown() => CurrentPage = SettingsPage.UNKNOWN;
    
    [RelayCommand]
    public async Task ReturnToPreviousPage()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            NavigationContainer.ViewModel.ChangeToPreviousPage();
        });
    }

    public enum SettingsPage
    {
        UNKNOWN,
        GENERAL,
        SOUND,
        ASSACC,
        RACC
    }
}