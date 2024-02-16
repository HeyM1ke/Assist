using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.ViewModels.Settings;

public partial class GeneralSettingsPageViewModel : ViewModelBase
{
    [RelayCommand]
    public async void CheckForUpdates()
    {
        var updateAvailable = await AssistApplication.CheckForUpdates();
        if (updateAvailable)
        {
            Log.Information("New Version Available. Showing to Update Page");
            AssistApplication.OpenUpdateWindow();
            return;
        }
        
    }
}