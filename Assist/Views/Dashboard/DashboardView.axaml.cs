using Assist.Controls.Infobars;
using Assist.Controls.Navigation;
using Assist.Core.Helpers;
using Assist.Services.Navigation;
using Assist.ViewModels.Navigation;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace Assist.Views.Dashboard;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        if (!Design.IsDesignMode)
        {
            NavigationViewModel.SetActivePage(AssistPage.DASHBOARD);
            Titlebar.ViewModel.AccountSwapVisible = true;
        }
        InitializeComponent();

        
    }
}