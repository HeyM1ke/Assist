using Assist.Views.Assist;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;

namespace Assist.ViewModels.Settings;

public partial class AssistAccountSettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _currentlyLoggedIn = !AssistApplication.AssistUser.Authentication.Roles.IsNullOrEmpty();

    [RelayCommand]
    public async void OpenAssistLoginPopup()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            AssistApplication.ChangeMainWindowPopupView(new AssistAuthenticationView());
        });
    }
}