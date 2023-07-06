using Assist.Services;
using Assist.Views.Dashboard.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Dashboard;

public partial class DashboardViewV2 : UserControl
{
    private readonly DashboardViewModel _viewModel;

    public DashboardViewV2()
    {
        DataContext = _viewModel = new DashboardViewModel();
        InitializeComponent();
        MainViewNavigationController.CurrentPage = Page.DASHBOARD;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}