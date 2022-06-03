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
using Assist.Controls.Selector;
using Assist.Modules.Popup;
using Assist.MVVM.View.Selector.ViewModels;
using Assist.MVVM.ViewModel;
using Assist.Settings;
using Serilog;
using Application = System.Windows.Application;
using Rectangle = System.Drawing.Rectangle;

namespace Assist.MVVM.View.Selector
{
    /// <summary>
    /// Interaction logic for Startup.xaml
    /// </summary>
    public partial class Startup : Window
    {
        private readonly StartupViewModel _viewModel;
        public static Grid Container;
        public Startup()
        {
            DataContext = _viewModel = new StartupViewModel();
            InitializeComponent();
            Container = PopupContainer;
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
            Screen targetScreen = Screen.PrimaryScreen;

            Rectangle viewport = targetScreen.WorkingArea;
            Top = (viewport.Height - this.Height) / 2
                  + viewport.Top;
            Left = (viewport.Width - this.Width) / 2
                   + viewport.Left;

            AddProfileCards();
        }

        private string defaultName = string.Empty;
        public static System.Windows.Forms.Timer countdownTimer;
        private int counter = 0 + AssistApplication.SelectorSeconds;
        private void StartCountdownTimer()
        {
            countdownTimer = new Timer();
            countdownTimer.Tick += new EventHandler(timer1_Tick);
            countdownTimer.Interval = 1000; // 1 second
            countdownTimer.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Log.Information("Default Account Login Counter: {timeRemaining}", counter);
            _viewModel.TimerText = $"Logging into default account {defaultName} in {counter} seconds.";
            counter--;
            if (counter == 0)
            {
                Log.Information("Timer hit 0, Logging into Default Account");
                countdownTimer.Stop();
                _viewModel.DefaultAccountLogin();
                
            }
        }

        private void Startup_Loaded(object sender, RoutedEventArgs e)
        {
            var d  = AssistSettings.Current.FindProfileById(AssistSettings.Current.DefaultAccount);
            if (d != null)
                defaultName = d.Gamename;
            StartCountdownTimer();
        }



        private async void ProfileCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ProfileCard card)
            {
                if (!card.ViewModel.Profile.Equals(AssistApplication.AppInstance.CurrentProfile))
                {
                    if (card.ViewModel.Profile is null)
                        return;

                    MVVM.View.Selector.Startup.countdownTimer.Stop();

                    PopupSystem.SpawnPopup(new PopupSettings()
                    {
                        PopupTitle = "Logging in...",
                        PopupDescription = $"Logging into {card.ViewModel.Profile.Gamename}",
                        PopupType = PopupType.LOADING
                    });

                    try
                    {
                        await AssistApplication.AppInstance.AuthenticateWithProfileSetting(card.ViewModel.Profile);
                    }
                    catch (Exception exception)
                    {
                        CardPanel.Children.Clear();
                        AddProfileCards();
                        PopupSystem.KillPopups();

                        AssistApplication.AppInstance.OpenStartupWindow();
                        
                    }
                    
                }
            }


            
        }


        private void RestartTimer()
        {
            countdownTimer.Stop();
            counter = 0 + AssistApplication.SelectorSeconds;
            StartCountdownTimer();
        }

        private void AddProfileCards()
        {
            foreach (var profile in AssistSettings.Current.Profiles)
            {
                var card = (new ProfileCard(profile)
                {
                    Margin = new Thickness(5, 10, 5, 10),

                });

                card.Click += ProfileCard_Click;
                CardPanel.Children.Add(card);

                Log.Information("Loaded ProfileCard for User {gamename}", profile.Gamename);
            }
        }
    }
}
