using Assist.Services.Navigation;
using Assist.ViewModels.Startup;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Startup;

public partial class StartupView : UserControl
{
    private readonly StartupViewModel _viewModel;

    public StartupView()
    {
        DataContext = _viewModel = new StartupViewModel();
        InitializeComponent();
    }
    
    public StartupView(string passedId)
    {
        DataContext = _viewModel = new StartupViewModel();
        InitializeComponent();
        _viewModel.AttemptProfileId = passedId;
    }

    private async void Startup_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode)return;
        
        await _viewModel.Startup();
    }
}