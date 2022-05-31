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
using Serilog;

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
            Log.Information("InitPage Constructor Called");
            DataContext = _viewModel = new InitPageViewModel();
            InitializeComponent();
        }

        private async void InitWindow_Loaded(object sender, RoutedEventArgs e)
        {

#if DEBUG

#else
        var resp = await AssistApplication.ApiService.GetMaintenanceStatus();

            if (resp.DownForMaintenance)
                new MaintenanceWindow(resp).ShowDialog(); 
#endif



            Log.Information("InitWindow_Loaded Called");

            if (AssistSettings.Current.bNewUser)
            {
                Log.Information("New User Flag is True, Running First Time Setup.");
                await _viewModel.FirstTimeSetup();
            }
            else
            {
                Log.Information("Returning User, Running Default Startup");
                await _viewModel.DefaultStartup();
            }
        }
    }
}
