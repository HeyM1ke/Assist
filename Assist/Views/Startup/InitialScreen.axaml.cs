using System;
using Assist.Views.Startup.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Serilog;

namespace Assist.Views.Startup
{
    public partial class InitialScreen : UserControl
    {
        private readonly StartupScreenViewModel _viewModel;

        public InitialScreen()
        {
            DataContext = _viewModel = new StartupScreenViewModel();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void StyledElement_OnInitialized(object? sender, EventArgs e)
        {

            Log.Information("Initial Screen Initialized");

            await _viewModel.StartupSetup();
        }
    }
}
