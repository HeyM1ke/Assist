using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Assist.MVVM.View.HomePage;
using Assist.MVVM.ViewModel;
namespace Assist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow MainWindowInstance;
        AssistApplication _viewModel => AssistApplication.AppInstance;

        
        public MainWindow()
        {
            DataContext = _viewModel;
            MainWindowInstance = this;

            InitializeComponent();
        }
        private void minimizeBTN_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void mainContentFrame_Loaded(object sender, RoutedEventArgs e)
        {
            navHomeBTN.isChecked = true;
            mainContentFrame.Navigate(new Uri("/MVVM/View/HomePage/HomePage.xaml", UriKind.RelativeOrAbsolute));
        }
        private void windowBorder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }


        #region Navigation Handling      
        void UncheckBtns()
        {
            foreach(Assist.Controls.AssistNavButton button in navContainer.Children)
            {
                button.isChecked = null;
            }
        }
        private void navHomeBTN_ButtonClick(object sender, EventArgs e)
        {
            UncheckBtns();
            navHomeBTN.isChecked = true;

            MemClear();
            // Open Page
            mainContentFrame.Navigate(new Uri("/MVVM/View/HomePage/HomePage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void navAccSwitchBTN_ButtonClick(object sender, EventArgs e)
        {
            navAccSwitchBTN.isChecked = true;
            var accSwitchWindow = new MVVM.View.Extra.AccountSwitch();
            accSwitchWindow.ShowDialog();
            navAccSwitchBTN.isChecked = null;
        }

        private void navProgressBTN_ButtonClick(object sender, EventArgs e)
        {
            UncheckBtns();
            navProgressBTN.isChecked = true;
            MemClear();
            // Open Page
            mainContentFrame.Navigate(new Uri("/MVVM/View/ProgressionPage/ProgressionPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void navStoreBTN_ButtonClick(object sender, EventArgs e)
        {
            UncheckBtns();
            navStoreBTN.isChecked = true;

            MemClear();
            // Open Page

            mainContentFrame.Navigate(new Uri("/MVVM/View/StorePage/StorePage.xaml", UriKind.RelativeOrAbsolute));
        }
        private void navSettingsBTN_ButtonClick(object sender, EventArgs e)
        {
            UncheckBtns();
            navSettingsBTN.isChecked = true;
            MemClear();
            // Open Page

            mainContentFrame.Navigate(new Uri("/MVVM/View/SettingsPage/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
        }
        #endregion
        private void MemClear()
        {
            // This clears memory usage a bit.
            mainContentFrame.Content = null;
            GC.Collect();
        }

        #region Properties





        #endregion

        private void NavBar_MouseEnter(object sender, MouseEventArgs e)
        {
            supportPanel.Visibility = Visibility.Visible;
        }

        private void NavBar_MouseLeave(object sender, MouseEventArgs e)
        {
            supportPanel.Visibility = Visibility.Collapsed;
        }

        private void socialButton1_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", "https://twitter.com/Hey_M1ke");
        }

        private void socialButton2_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", "https://discord.gg/C3AbvyM3dj");
        }

        private void socialButton3_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", "https://www.patreon.com/Valorleaks");
        }
    }
}
