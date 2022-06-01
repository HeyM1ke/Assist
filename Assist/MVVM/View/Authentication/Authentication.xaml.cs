using System;
using System.Collections.Generic;
using System.Drawing;
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

using Assist.MVVM.View.Authentication.AuthenticationPages;
using Assist.MVVM.ViewModel;
using Assist.Settings;

using Application = System.Windows.Application;


namespace Assist.MVVM.View.Authentication
{
    /// <summary>
    /// Interaction logic for Authentication.xaml
    /// </summary>
    public partial class Authentication : Window
    {
        public static Frame ContentFrame;
        public static bool bAddMode;
        public Authentication()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
            DetermineResolution();
            ContentFrame = LoginFrame;

        }

        public Authentication(bool addMode)
        {
            DataContext = new MainViewModel();
            InitializeComponent();
            DetermineResolution();
            ContentFrame = LoginFrame;
            bAddMode = addMode;
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
                case Enums.EWindowSize.R900:
                    Width = 1600;
                    Height = 900;
                    AssistApplication.GlobalScaleRate = 1.5;
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

        private void AuthenticationLoaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Uri("MVVM/View/Authentication/AuthenticationPages/InitialAuthentication.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
