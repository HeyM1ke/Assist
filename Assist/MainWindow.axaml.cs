using System;
using Assist.Controls.Global.Popup;
using Assist.Objects.Enums;
using Assist.Services;
using Assist.Services.Popup;
using Assist.ViewModels;
using Assist.Views;
using Assist.Views.Authentication;
using Assist.Views.Startup;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist
{
    public partial class MainWindow : Window
    {
        
        private readonly MainWindowViewModel _viewModel;
        private int number = 0;
        public MainWindow()
        {
            DataContext = _viewModel = new MainWindowViewModel();
            _viewModel.DetermineScaleRate();
            InitializeComponent();
        }

        private void MainWindow_Initialized(object? sender, EventArgs e)
        {
            MainWindowContentController.ContentControl = this.FindControl<TransitioningContentControl>("ContentView");
            PopupSystem.PopupController = this.FindControl<PopupMaster>("PopupMaster");
        }


        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            if(number % 2 == 0)
                MainWindowContentController.Change(new MainView());
            else
            {
                MainWindowContentController.Change(new AuthenticationView());
            }

            number++;
        }

        public void ChangeResolution(EResolution res)
        {
            _viewModel.ChangeResolution(res);
        }
    }
}
