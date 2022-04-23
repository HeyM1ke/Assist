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
        private InitPageViewModel _viewModel;

        public InitPage()
        {
            AssistLog.Normal("InitPage Constructor Called");
            DataContext = _viewModel = new InitPageViewModel();
            InitializeComponent();
        }

        private async void InitWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
            var resp = await AssistApplication.AppInstance.AssistApiController.GetMaintenanceStatus();

            if (resp.bDownForMaintenance)
                new MaintenanceWindow().ShowDialog();

            
            AssistLog.Normal("InitWindow_Loaded Called");

            if (AssistSettings.Current.bNewUser)
            {
                AssistLog.Normal("New User Flag is True, Running First Time Setup.");
                await _viewModel.FirstTimeSetup();
            }
            else
            {
                AssistLog.Normal("Returning User, Running Default Startup");
                await _viewModel.DefaultStartup();
            }
        }
    }
}
