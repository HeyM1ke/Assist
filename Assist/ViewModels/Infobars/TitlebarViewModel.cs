using System.Threading.Tasks;
using Assist.Controls.Navigation;
using Assist.Core.Helpers;
using Assist.Models.Enums;
using Assist.Services.Navigation;
using Assist.Shared.Settings.Accounts;
using Assist.Views.ProfileSwap;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.ViewModels.Infobars;

public partial class TitlebarViewModel : ViewModelBase
{
    [ObservableProperty] private bool _accountSwapVisible = false;
    [ObservableProperty] private bool _settingsEnabled = false;
    [ObservableProperty] private bool _accountSwapEnabled = true;
    [ObservableProperty] private string _accountName;
    [ObservableProperty] private string _accountProfilePic;


    public async Task ShowcaseProfile(AccountProfile _profile)
    {
        AccountName = _profile.Personalization.RiotId;
        AccountProfilePic =
            $"https://cdn.assistval.com/playercards/{_profile.Personalization.PlayerCardId}_DisplayIcon.png";
    }
    
    [RelayCommand]
    public void OpenProfileSwapPage()
    {
        if (AssistApplication.CurrentMode == EAssistMode.LAUNCHER)
        {
            AssistApplication.ChangeMainWindowPopupView(new ProfileSwapView());
        }
    }

    [RelayCommand]
    public void OpenSettingsPage()
    {
        NavigationContainer.ViewModel.ChangePage(AssistPage.SETTINGS);
    }
}