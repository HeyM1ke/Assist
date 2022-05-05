using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Assist.MVVM.View.Settings
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
        }


        void UncheckBtns()
        {
            foreach (ToggleButton button in SettingsButtonPanel.Children)
            {
                button.IsChecked = false;
            }
        }

        private void GeneralBTN_Click(object sender, RoutedEventArgs e)
        {
            UncheckBtns();
            GeneralBTN.IsChecked = true;

            // Open Page
            SettingsContentFrame.Navigate(new Uri("/MVVM/View/Settings/SettingPages/Settings_General.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ValSetBTN_Click(object sender, RoutedEventArgs e)
        {
            UncheckBtns();
            ValSetBTN.IsChecked = true;

            // Open Page
            SettingsContentFrame.Navigate(new Uri("/MVVM/View/Settings/SettingPages/Settings_ValSettings.xaml", UriKind.RelativeOrAbsolute));
        }

        private void AbtBTN_Click(object sender, RoutedEventArgs e)
        {
            UncheckBtns();
            AbtBTN.IsChecked = true;

            // Open Page
            SettingsContentFrame.Navigate(new Uri("/MVVM/View/Settings/SettingPages/Settings_About.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            SettingsContentFrame.Navigate(new Uri("/MVVM/View/Settings/SettingPages/Settings_General.xaml", UriKind.RelativeOrAbsolute));
            GeneralBTN.IsChecked = true;
        }
    }
}
