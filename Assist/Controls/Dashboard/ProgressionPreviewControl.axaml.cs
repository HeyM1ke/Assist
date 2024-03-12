using System;
using Assist.ViewModels.Dashboard;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Dashboard;

public partial class ProgressionPreviewControl : UserControl
{
    private readonly ProgressionPreviewViewModel _viewModel;

    public ProgressionPreviewControl()
    {
        DataContext = _viewModel = new ProgressionPreviewViewModel();
        InitializeComponent();
    }

    private async void ProgressionPreview_Init(object? sender, EventArgs e)
    {
        if (!Design.IsDesignMode) await _viewModel.Setup();
    }

    private async void ProgressionPreview_Loaded(object? sender, RoutedEventArgs e)
    {
        if (!Design.IsDesignMode) await _viewModel.LoadedCheck();
    }
}