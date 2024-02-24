using Assist.ViewModels.ProfileSwap;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.ProfileSwap;

public partial class SwapPage : UserControl
{
    private readonly SwapPageViewModel _viewModel;

    public SwapPage()
    {
        DataContext = _viewModel = new SwapPageViewModel();
        InitializeComponent();
    }
    
    
    public SwapPage(string profileId)
    {
        DataContext = _viewModel = new SwapPageViewModel();
        InitializeComponent();
        _viewModel.ProfileId = profileId;
    }


    private async void SwapPage_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode)return;

        await _viewModel.SwapProfile();
    }
}