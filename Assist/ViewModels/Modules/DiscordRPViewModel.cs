using Assist.Controls.Navigation;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.ViewModels.Modules;

public partial class DiscordRPViewModel : ViewModelBase
{
    
    
    [RelayCommand]
    public void ReturnToModules()
    {
        Log.Information("Player is attempting to return to modules page.");
        Dispatcher.UIThread.Invoke(() =>
        {
            NavigationContainer.ViewModel.ChangeToPreviousPage();
        });
    }
}