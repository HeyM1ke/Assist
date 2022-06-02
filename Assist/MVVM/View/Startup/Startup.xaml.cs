using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Assist.Controls.Startup;
using Assist.MVVM.View.Startup.ViewModels;
using Assist.Settings;
using Serilog;
using Application = System.Windows.Application;

namespace Assist.MVVM.View.Startup
{
    /// <summary>
    /// Interaction logic for Startup.xaml
    /// </summary>
    public partial class Startup : Window
    {
        private readonly StartupViewModel _viewModel;
        public Startup()
        {
            DataContext = _viewModel = new StartupViewModel();
            InitializeComponent();
        }

        #region Window Bar

        private void windowBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void minimizeBTN_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        private void CardPanel_Initialized(object sender, EventArgs e)
        {
            foreach (var profile in AssistSettings.Current.Profiles)
            {
                CardPanel.Children.Add(new ProfileCard(profile)
                {
                    Margin = new Thickness(5, 10, 5, 10)
                });
            }

            
        }

        private string defaultName = AssistSettings.Current.FindProfileById(AssistSettings.Current.DefaultAccount).Gamename;
        private System.Windows.Forms.Timer countdownTimer;
        private int counter = 20;
        private void StartCountdownTimer()
        {
            countdownTimer = new Timer();
            countdownTimer.Tick += new EventHandler(timer1_Tick);
            countdownTimer.Interval = 1000; // 1 second
            countdownTimer.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Log.Debug("Default Account Login Counter: {timeRemaining}", counter);
            _viewModel.TimerText = $"Logging into default account {defaultName} in {counter} seconds.";
            counter--;
            if (counter == 0)
            {
                _viewModel.DefaultAccountLogin();
            }



        }

        private void Startup_Loaded(object sender, RoutedEventArgs e)
        {
            StartCountdownTimer();
        }
    }
}
