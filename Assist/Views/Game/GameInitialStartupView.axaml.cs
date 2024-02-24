using Assist.ViewModels.Game;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Game;

public partial class GameInitialStartupView : UserControl
{
    private readonly GameInitalStartupViewModel _viewModel;

    public GameInitialStartupView()
    {
        DataContext = _viewModel = new GameInitalStartupViewModel();
        InitializeComponent();
    }

    private async void GameInitalStartup_Loaded(object? sender, RoutedEventArgs e)
    {
        await _viewModel.Inital();
    }
}