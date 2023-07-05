using System;
using Assist.ViewModels.Windows;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Views.Windows
{
    public partial class StartupSplash : Window
    {
        private readonly StartupSplashViewModel _viewModel;
        public StartupSplash()
        {

            DataContext = _viewModel = new StartupSplashViewModel();

            InitializeComponent();
        }

        private async void StartupSplash_Initialized(object? sender, System.EventArgs e)
        {
            
        }
        
        
        private async void StartupSplash_OnOpened(object? sender, EventArgs e)
        {
            if(Design.IsDesignMode)
                return;

            await _viewModel.Startup();
        }
    }
}
