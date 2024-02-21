using System;
using Assist.Controls.Global.Popup;
using Assist.Objects.Enums;
using Assist.Services;
using Assist.Services.Popup;
using Assist.ViewModels;
using Assist.Views;
using Assist.Views.Authentication;
using Assist.Views.Dashboard;
using Assist.Views.Startup;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist
{
    public partial class MainWindow : Window
    {
        
        private readonly MainWindowViewModel _viewModel;
        private int number = 0;
        public WindowNotificationManager notificationManager;
        public MainWindow()
        {
            DataContext = _viewModel = new MainWindowViewModel();
            
            _viewModel.DetermineScaleRate();
            InitializeComponent();
        }

        private void MainWindow_Initialized(object? sender, EventArgs e)
        {
            MainWindowContentController.ContentControl = this.FindControl<TransitioningContentControl>("ContentView");
            
            PopupSystem.PopupController = this.FindControl<PopupMaster>("PopupMasterController");
            
            notificationManager = new WindowNotificationManager(this)
            {
                Position = NotificationPosition.TopRight,
                MaxItems = 5,
                Margin = OperatingSystem.IsWindows() ? new Thickness(0, 30, 0, 0) : new Thickness(0),
            };
        }

        public void ChangeResolution(EResolution res)
        {
            _viewModel.ChangeResolution(res);
        }

        public void ChangeGameResolution(EResolution res)
        {
            _viewModel.ChangeResolution(res);
        }

        private void PopupMaster_OnLoaded(object? sender, RoutedEventArgs e)
        {
            PopupSystem.ContentControl = sender as TransitioningContentControl;
            
        }
    }
}
