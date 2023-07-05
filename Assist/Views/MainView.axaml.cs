using System;
using Assist.Services;
using Assist.Services.Popup;
using Assist.ViewModels;
using Assist.Views.Dashboard;
using Assist.Views.Progression;
using Assist.Views.Store;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views
{
    public partial class MainView : UserControl
    {
        private MainViewViewModel _viewModel;
     
        public MainView()
        {
            DataContext = _viewModel = new MainViewViewModel();
            InitializeComponent();
            MainViewNavigationController.ContentControl = this.FindControl<TransitioningContentControl>("ContentView");
            MainViewNavigationController.Change(new DashboardView());
        }
        
        public MainView(UserControl popup)
        {
            DataContext = _viewModel = new MainViewViewModel();
            InitializeComponent();
            MainViewNavigationController.ContentControl = this.FindControl<TransitioningContentControl>("ContentView");
            if (MainViewNavigationController.CurrentPage != null)
            {
                switch (MainViewNavigationController.CurrentPage)
                {
                    case Page.STORE:
                        MainViewNavigationController.Change(new StoreView());
                        break;
                    case Page.MODULES:
                        MainViewNavigationController.Change(new StoreView());
                        break;
                    case Page.DASHBOARD:
                        MainViewNavigationController.Change(new DashboardView());
                        break;
                    default:
                        MainViewNavigationController.Change(new DashboardView());
                        break;
                }
                
                PopupSystem.SpawnCustomPopup(popup);
            }
        }


        private async void MainView_Initializaed(object? sender, EventArgs e)
        {
            if(Design.IsDesignMode)
                return;

            _viewModel.SetupUserCount();
        }
    }
}
