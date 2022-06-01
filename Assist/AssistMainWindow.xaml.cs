using Assist.MVVM.ViewModel;
using Assist.Services;
using Assist.Settings;

using Serilog;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

using Application = System.Windows.Application;

namespace Assist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AssistMainWindow : Window
    {

        private const string ClickSoundHost = "https://cdn.assistapp.dev/Static/Click.mp3";

        public static AssistMainWindow Current;
        public static Grid PopupContainer;

        private readonly MediaPlayer _mediaPlayer = new();

        public AssistMainWindow()
        {
            DataContext = new MainViewModel();
            AssistApplication.AppInstance.TokenService = new TokenServiceBackgroundService();
            AssistSettings.Current.bNewUser = false;

            InitializeComponent();
            DetermineResolution();

            Current = this;
            PopupContainer = PopupHolder;
            Current._mediaPlayer.Open(new Uri(ClickSoundHost));

            _navigationButtons["MVVM/View/Dashboard/Dashboard.xaml"] = DashboardBTN;
            _navigationButtons["MVVM/View/Progression/Progression.xaml"] = ProgressionBTN;
            _navigationButtons["MVVM/View/Settings/Settings.xaml"] = SettingsBTN;
            _navigationButtons["MVVM/View/Store/Store.xaml"] = StoreBTN;
        }

        private void DetermineResolution()
        {
            switch (AssistSettings.Current.Resolution)
            {
                case Enums.EWindowSize.R720:
                    Width = 1280;
                    Height = 745;
                    AssistApplication.GlobalScaleRate = 1.25;
                    myCanvas.LayoutTransform = new ScaleTransform(AssistApplication.GlobalScaleRate, AssistApplication.GlobalScaleRate);
                    break;
                case Enums.EWindowSize.R576:
                    Width = 1024;
                    Height = 601;
                    AssistApplication.GlobalScaleRate = 1;
                    myCanvas.LayoutTransform = new ScaleTransform(AssistApplication.GlobalScaleRate, AssistApplication.GlobalScaleRate);
                    break;
                default:
                    Width = 1024;
                    Height = 601;
                    AssistApplication.GlobalScaleRate = 1;
                    myCanvas.LayoutTransform = new ScaleTransform(AssistApplication.GlobalScaleRate, AssistApplication.GlobalScaleRate);
                    break;
            }

            var targetScreen = Screen.PrimaryScreen;
            var viewport = targetScreen.WorkingArea;

            Top = (viewport.Height - Height) / 2 + viewport.Top;
            Left = (viewport.Width - Width) / 2 + viewport.Left;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            myCanvas.Width = e.NewSize.Width;
            myCanvas.Height = e.NewSize.Height;

            double xChange = 1, yChange = 1;

            if (e.PreviousSize.Width != 0)
                xChange = (e.NewSize.Width / e.PreviousSize.Width);

            if (e.PreviousSize.Height != 0)
                yChange = (e.NewSize.Height / e.PreviousSize.Height);

            var scale = new ScaleTransform(myCanvas.LayoutTransform.Value.M11 * xChange, myCanvas.LayoutTransform.Value.M22 * yChange);
            Trace.WriteLine(myCanvas.LayoutTransform.Value.M11);
            myCanvas.LayoutTransform = scale;
            myCanvas.UpdateLayout();
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Current.ContentFrame.Navigate(new Uri("/MVVM/View/Dashboard/Dashboard.xaml", UriKind.RelativeOrAbsolute));
            DashboardBTN.IsChecked = true;
            SetPicture();
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

        #region Navigation Handling

        private readonly Dictionary<string, ToggleButton> _navigationButtons = new();

        private void DashboardBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo("MVVM/View/Dashboard/Dashboard.xaml");
        }

        private void StoreBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo("MVVM/View/Store/Store.xaml");
        }

        private void ProgressionBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo("MVVM/View/Progression/Progression.xaml");
        }

        public void SettingsBTN_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo("MVVM/View/Settings/Settings.xaml");
        }

        private void ProfilePC_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo("MVVM/View/Profiles/Profiles.xaml");
        }

        public void GoToStore()
        {
            NavigateTo("MVVM/View/Store/Store.xaml");
        }

        private void NavigateTo(string uri)
        {
            Current.ContentFrame.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        public void UncheckNavigationButtons()
        {
            foreach (ToggleButton button in Buttonpanel.Children)
                button.IsChecked = false;

            PlayClickSound();
        }

        #endregion

        private void SetPicture()
        {
            ProfilePC.Content = App.LoadImageUrl($"https://cdn.assistapp.dev/PlayerCards/" + AssistApplication.AppInstance.CurrentProfile.PCID + "_DisplayIcon.png", 60, 60);
        }

        public static void PlayClickSound()
        {
            Current._mediaPlayer.Volume = AssistSettings.Current.SoundVolume;
            Current._mediaPlayer.Open(new Uri(ClickSoundHost));
            Current._mediaPlayer.Play();
        }

        private void ContentFrame_OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri == null)
                return;

            var uri = e.Uri.ToString();
            var button = _navigationButtons.GetValueOrDefault(uri);
            if (button == null)
            {
                Log.Warning("Couldn't find navigation button for URI: {Uri}", uri);
                return;
            }

            UncheckNavigationButtons();
            button.IsChecked = true;
        }

    }
}
