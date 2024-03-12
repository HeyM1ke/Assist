using Assist.ViewModels.Modules;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Modules.Pages;

public partial class DodgeView : UserControl
{
    private readonly DodgeViewModel _viewModel;

    public DodgeView()
    {
        DataContext = _viewModel = new DodgeViewModel();
        InitializeComponent();
    }

    private async void DodgeView_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode) return;

        await _viewModel.Load();
    }

    private void DodgeView_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode) return;

        _viewModel.Unload();
    }
}