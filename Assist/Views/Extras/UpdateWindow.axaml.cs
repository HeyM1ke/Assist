using Assist.ViewModels.Extras;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Extras;

public partial class UpdateWindow : Window
{
    private readonly UpdateWindowViewModel _viewModel;

    public UpdateWindow()
    {
        DataContext = _viewModel = new UpdateWindowViewModel();
        InitializeComponent();
    }

    private async void UpdateWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode)
            return;
        
        await _viewModel.Setup();
    }
}