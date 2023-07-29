using System;
using Assist.Objects.Helpers;
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
            AssistApplication.Current.Mode = AssistMode.LAUNCHER;
            DataContext = _viewModel = new MainViewViewModel();
            InitializeComponent();
            MainViewNavigationController.ContentControl = this.FindControl<TransitioningContentControl>("ContentView");
            MainViewNavigationController.Change(new DashboardViewV2());
        }
        
        public MainView(UserControl popup)
        {
            AssistApplication.Current.Mode = AssistMode.LAUNCHER;
            DataContext = _viewModel = new MainViewViewModel();
            InitializeComponent();
            MainViewNavigationController.ContentControl = this.FindControl<TransitioningContentControl>("ContentView");
            if (MainViewNavigationController.CurrentPage != null)
            {
                switch (MainViewNavigationController.CurrentPage)
                {
                    case Page.STORE:
                        MainViewNavigationController.Change(new StoreViewV2());
                        break;
                    case Page.MODULES:
                        MainViewNavigationController.Change(new StoreViewV2());
                        break;
                    case Page.DASHBOARD:
                        MainViewNavigationController.Change(new DashboardViewV2());
                        break;
                    default:
                        MainViewNavigationController.Change(new DashboardViewV2());
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
