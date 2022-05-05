using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Assist.Controls.Extra;
using Assist.Controls.Home.ViewModels;
using Assist.Modules.Popup;
using Assist.MVVM.ViewModel;
using Assist.Settings;
using ValNet;
using ValNet.Objects;

namespace Assist.Controls.Home
{
    /// <summary>
    /// Interaction logic for ProfileLaunchControl.xaml
    /// </summary>
    public partial class ProfileLaunchControl : UserControl
    {
        private ProfileLaunchViewModel _viewModel { get; set; }
        private ProfileSetting _associatedProfile { get; set; }
        public ProfileLaunchControl()
        {
            DataContext = _viewModel = new ProfileLaunchViewModel();
            if (AssistApplication.AppInstance.CurrentProfile is not null)
                _viewModel.Profile = AssistApplication.AppInstance.CurrentProfile;
            InitializeComponent();
            SetupControl();
        }
        private async void SetupControl()
        {
            _viewModel.ProfilePlayercard = await App.LoadImageUrl("https://media.valorant-api.com/playercards/" + _viewModel.Profile.PCID + "/smallart.png", 80, 80);
            _viewModel.BackingImage = await App.LoadImageUrl("https://cdn.rumblemike.com/Maps/2FB9A4FD-47B8-4E7D-A969-74B4046EBD53_splash.png", 720, 480);
        }
        private async void PlayBTN_Click(object sender, RoutedEventArgs e)
        {
            PopupSystem.SpawnPopup(new PopupSettings()
            {
                PopupTitle = "Launching Valorant",
                PopupDescription = "One Moment...",
                PopupType = PopupType.LOADING
            });

            await AssistApplication.AppInstance.CreateAuthenticationFile();
            var worker = new BackgroundWorker();
            worker.DoWork += WorkerOnDoWork;
            worker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;
            worker.RunWorkerAsync();
        }
        private void WorkerOnRunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private async void WorkerOnDoWork(object? sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            await _viewModel.LaunchGame();
        }

        private void Switch_Click(object sender, RoutedEventArgs e)
        {
            AssistMainWindow.Current.UncheckBtns();
            AssistMainWindow.Current.ContentFrame.Navigate(new Uri("/MVVM/View/Profiles/Profiles.xaml", UriKind.RelativeOrAbsolute));
        }

        private void LS_Click(object sender, RoutedEventArgs e)
        {
            PopupSystem.SpawnCustomPopup(new LaunchSettingsPopup());
        }
    }
}
