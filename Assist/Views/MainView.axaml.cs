using System;
using Assist.Services;
using Assist.Views.Dashboard;
using Assist.Views.Progression;
using Assist.Views.Store;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            MainViewNavigationController.ContentControl = this.FindControl<TransitioningContentControl>("ContentView");
            MainViewNavigationController.Change(new DashboardView());
        }

        public MainView(UserControl PageToOpenTo)
        {
            InitializeComponent();
            MainViewNavigationController.ContentControl = this.FindControl<TransitioningContentControl>("ContentView");
            MainViewNavigationController.Change(PageToOpenTo);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void MainView_Initializaed(object? sender, EventArgs e)
        {
            
        }
    }
}
