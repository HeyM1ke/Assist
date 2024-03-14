using System.Threading.Tasks;
using Assist.ViewModels.Settings;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Settings.Pages;

public partial class AssistAccountSettingsPageView : UserControl
{
    private readonly AssistAccountSettingsViewModel _viewModel;

    public AssistAccountSettingsPageView()
    {
        DataContext = _viewModel = new AssistAccountSettingsViewModel();
        InitializeComponent();
    }


    private async void AssistAccountPage_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode)return;

        await _viewModel.SetupPage();
    }
}