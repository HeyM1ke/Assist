using Assist.ViewModels.Store;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Serilog;

namespace Assist.Views.Store;

public partial class StoreView : UserControl
{
    private readonly StoreViewModel _viewModel;

    public StoreView()
    {
        DataContext = _viewModel = new StoreViewModel();
        InitializeComponent();
    }

    private async void Store_OnLoaded(object? sender, RoutedEventArgs e)
    {
        Log.Information("StoreOnloaded Event");
        if (Design.IsDesignMode) return;

        await _viewModel.SetupStoreView();
    }
}