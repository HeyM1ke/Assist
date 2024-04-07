using Assist.ViewModels.Dashboard;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Dashboard;

public partial class GameLaunchControl : UserControl
{
    private readonly GameLaunchControlViewModel _viewModel;

    public GameLaunchControl()
    {
        DataContext = _viewModel = new GameLaunchControlViewModel();
        InitializeComponent();
    }

    private async void GameLaunchControl_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode)return;
        await _viewModel.LoadProfiles();
    }
}