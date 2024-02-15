using System.Reflection;
using System.Threading.Tasks;
using Assist.Controls.Navigation;
using Assist.Views.Assist;
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

    public SettingsViewModel()
    {
        CurrentContent = new GeneralSettingsPageView();
    }

    [RelayCommand]
    public async void SwitchToSound()
    {
        
    }
    
    [RelayCommand]
    public async void SwitchToAssistAccount()
    {
        CurrentContent = new AssistAccountSettingsPageView();
    }
    
    [RelayCommand]
    public async void OpenRiotAccount()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            AssistApplication.ChangeMainWindowPopupView(new AssistAuthenticationView());
        });
    }
    
    [RelayCommand]
    public async Task ReturnToPreviousPage()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            NavigationContainer.ViewModel.ChangeToPreviousPage();
        });
    }
}