using System;
using Assist.ViewModels.ProfileSwap;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views.ProfileSwap;

public partial class ProfileSwapView : UserControl
{
    private readonly ProfileSwapViewViewModel _viewModel;
    public ProfileSwapView()
    {
        DataContext = _viewModel = new ProfileSwapViewViewModel();
        InitializeComponent();
    }

    private async void ProfileSwapView_Init(object? sender, EventArgs e)
    {
        if (Design.IsDesignMode) return;
        await _viewModel.Setup();
    }

    private  async void ProfileSwapView_Unload(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode) return;
        await _viewModel.Unload();
    }
    
    
}