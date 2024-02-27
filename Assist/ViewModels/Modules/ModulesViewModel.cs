using Assist.Controls.Navigation;
using Assist.Services.Navigation;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assist.ViewModels.Modules;

public partial class ModulesViewModel : ViewModelBase
{
    [RelayCommand]
    public void OpenDodgeView()
    {
        Log.Information("Player has asked to open the dodge module");
        
        NavigationContainer.ViewModel.ChangePage(AssistPage.DODGE);
    }
}