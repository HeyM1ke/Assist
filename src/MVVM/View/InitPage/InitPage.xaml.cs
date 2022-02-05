using Assist.MVVM.ViewModel;
using Assist.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Assist.MVVM.View.Extra;

namespace Assist.MVVM.View.InitPage
{
    /// <summary>
    /// Interaction logic for InitPage.xaml
    /// </summary>
    public partial class InitPage : Window
    {
        AssistApplication _viewModel => AssistApplication.AppInstance;

        public InitPage()
        {
            AssistApplication.AppInstance.Log.Normal("InitPage Constructor Called");
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void InitWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var resp = await _viewModel.AssistApiController.GetMaintenanceStatus();
            if (resp.bDownForMaintenance)
                new MaintenanceWindow().ShowDialog();

            AssistApplication.AppInstance.Log.Normal("InitWindow_Loaded Called");
            if (_viewModel.InitPageViewModel.isFirstTime())
            {
                AssistApplication.AppInstance.Log.Normal("First Time found");
                await _viewModel.InitPageViewModel.FirstTimeSetup();
            }
            else
            {
                AssistApplication.AppInstance.Log.Normal("Default Startup");
                await _viewModel.InitPageViewModel.DefaultStartup();
            }
        }
    }
}
