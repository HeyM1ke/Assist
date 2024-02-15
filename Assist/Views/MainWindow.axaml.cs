using System;
using Assist.Models.Enums;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Styling;

namespace Assist.Views;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;
    public WindowNotificationManager notificationManager;
    public MainWindow()
    {
        DataContext = _viewModel = new MainWindowViewModel();
        InitializeComponent();
    }
    
    
    public void ChangeResolution(EResolution res)
    {
        _viewModel.ChangeResolution(res);
    }
    
    public void ChangeView(UserControl newView)
    {
        _viewModel.ChangeMainView(newView);
    }
    
    public void ChangePopupView(UserControl newView)
    {
        _viewModel.ChangePopupView(newView);
    }
    
    private async void MainWindow_OnOpened(object? sender, EventArgs e)
    {
        if(Design.IsDesignMode)
            return;

        await _viewModel.Startup();
    }
}