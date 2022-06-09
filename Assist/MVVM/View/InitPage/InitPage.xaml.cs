using Assist.MVVM.ViewModel;
using Assist.Settings;
using Serilog;
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
            Log.Information("InitPage Constructor Called");
            DataContext = _viewModel = new InitPageViewModel();
            InitializeComponent();
        }

        private async void InitWindow_Loaded(object sender, RoutedEventArgs e)
        {

            var a = await AssistApplication.ApiService.GetAgent();
            if(!string.IsNullOrEmpty(a.Agent))
                AssistApplication.AgentFormat = a.Agent;
#if RELEASE

            var maintenanceStatus = await AssistApplication.ApiService.GetMaintenanceStatus();
            if (maintenanceStatus.DownForMaintenance)
            {
                var window = new MaintenanceWindow(maintenanceStatus);
                window.Show();
                
                return;
            }

            var shouldUpdate = await App.CheckForUpdatesAsync();
            if (shouldUpdate)
                return;
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
