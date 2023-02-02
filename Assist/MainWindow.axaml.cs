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
        WindowNotificationManager notificationManager;
        public MainWindow()
        {
            DataContext = _viewModel = new MainWindowViewModel();
            notificationManager = new WindowNotificationManager(this);
            _viewModel.DetermineScaleRate();
            InitializeComponent();
        }

        private void MainWindow_Initialized(object? sender, EventArgs e)
        {
            MainWindowContentController.ContentControl = this.FindControl<TransitioningContentControl>("ContentView");
            PopupSystem.PopupController = this.FindControl<PopupMaster>("PopupMaster");
        }

        public void ChangeResolution(EResolution res)
        {
            _viewModel.ChangeResolution(res);
        }

        public void ChangeGameResolution(EResolution res)
        {
            _viewModel.ChangeGameModeResolution(res);
        }
    }
}
