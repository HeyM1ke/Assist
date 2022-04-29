using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Assist.Modules.XMPP;
using Assist.MVVM.Model;
using Assist.MVVM.View.Dashboard;
using Assist.MVVM.ViewModel;
using Assist.Services;
using Assist.Settings;
using ValNet;
using ValNet.Objects.Authentication;
using Application = System.Windows.Application;
using Rectangle = System.Drawing.Rectangle;

namespace Assist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AssistMainWindow : Window
    {
        private const string clickSoundHost = "https://cdn.assistapp.dev/Static/Click.mp3";
        public static AssistMainWindow Current;
        public static Grid PopupContainer;
        private MediaPlayer _mPlayer = new MediaPlayer();
        public AssistMainWindow()
        {
            if(AssistApplication.AppInstance.AssistApiController.bIsUpdate) // Fixes a bug with the auto updater taking too long and showing the main window.
                return;


            AssistApplication.AppInstance.TokenService = new TokenServiceBackgroundService();
            AssistSettings.Current.bNewUser = false;
            InitializeComponent();
            DetermineResolution();
            Current = this;
            PopupContainer = PopupHolder;
            Current._mPlayer.Open(new Uri(clickSoundHost));
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

            Screen targetScreen = Screen.PrimaryScreen;

            Rectangle viewport = targetScreen.WorkingArea;
            Top = (viewport.Height - this.Height) / 2
                                     + viewport.Top;
            Left = (viewport.Width - this.Width) / 2
                                      + viewport.Left; ;
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

            ScaleTransform scale = new ScaleTransform(myCanvas.LayoutTransform.Value.M11 * xChange, myCanvas.LayoutTransform.Value.M22 * yChange);
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

        public void UncheckBtns()
        {
            foreach (ToggleButton button in Buttonpanel.Children)
            {
                button.IsChecked = false;
            }

            Play();
        }

        private void DashboardBTN_Click(object sender, RoutedEventArgs e)
        {
            UncheckBtns();
            DashboardBTN.IsChecked = true;

            MemClear();
            // Open Page
            Current.ContentFrame.Navigate(new Uri("/MVVM/View/Dashboard/Dashboard.xaml", UriKind.RelativeOrAbsolute));
        }
        private void StoreBTN_Click(object sender, RoutedEventArgs e)
        {
            UncheckBtns();
            StoreBTN.IsChecked = true;

            MemClear();
            // Open Page

            Current.ContentFrame.Navigate(new Uri("/MVVM/View/Store/Store.xaml", UriKind.RelativeOrAbsolute));
        }
        private void ProgressionBTN_Click(object sender, RoutedEventArgs e)
        {
            UncheckBtns();
            ProgressionBTN.IsChecked = true;

            MemClear();
            // Open Page

            Current.ContentFrame.Navigate(new Uri("/MVVM/View/Progression/Progression.xaml", UriKind.RelativeOrAbsolute));
        }
        public void SettingsBTN_Click(object sender, RoutedEventArgs e)
        {
            UncheckBtns();
            SettingsBTN.IsChecked = true;
            MemClear();
            // Open Page

            Current.ContentFrame.Navigate(new Uri("/MVVM/View/Settings/Settings.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ProfilePC_Click(object sender, RoutedEventArgs e)
        {
            UncheckBtns();
            MemClear();
            // Open Page

            Current.ContentFrame.Navigate(new Uri("/MVVM/View/Profiles/Profiles.xaml", UriKind.RelativeOrAbsolute));
        }

        public void GoToStore()
        {
            UncheckBtns();
            StoreBTN.IsChecked = true;

            MemClear();
            // Open Page
            Current.ContentFrame.Navigate(new Uri("/MVVM/View/Store/Store.xaml", UriKind.RelativeOrAbsolute));
        }
        #endregion
        private void MemClear()
        {
            // This clears memory usage a bit.
            Current.ContentFrame.Content = null;
            GC.Collect(); // find finalizable objects
            GC.WaitForPendingFinalizers(); // wait until finalizers executed
            GC.Collect(); // collect finalized objects
        }
        private async void SetPicture()
        {
            ProfilePC.Content = await App.LoadImageUrl("https://media.valorant-api.com/playercards/" + AssistApplication.AppInstance.CurrentProfile.PCID + "/smallart.png",35,35);
        }

        public static void Play()
        {
            Current._mPlayer.Volume = AssistSettings.Current.SoundVolume;
            Current._mPlayer.Open(new Uri(clickSoundHost));
            Current._mPlayer.Play();
        }
       
    }
}
