using System;
using Assist.Services;
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

        public MainView(UserControl PageToOpenTo)
        {
            DataContext = _viewModel = new MainViewViewModel();
            InitializeComponent();
            MainViewNavigationController.ContentControl = this.FindControl<TransitioningContentControl>("ContentView");
            MainViewNavigationController.Change(PageToOpenTo);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void MainView_Initializaed(object? sender, EventArgs e)
        {
            _viewModel.SetupUserCount();
        }
    }
}
