using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
using Assist.MVVM.Model;
using Assist.MVVM.ViewModel;
using Assist.Settings;

namespace Assist.MVVM.View.Settings.SettingPages
{
    /// <summary>
    /// Interaction logic for Settings_General.xaml
    /// </summary>
    public partial class Settings_General : Page
    {
        public Settings_General()
        {
            InitializeComponent();

        }

        private async void Settings_General_Loaded(object sender, RoutedEventArgs e)
        {
            WindowSizeComboBox.SelectedIndex = (int)AssistSettings.Current.Resolution;
            LanguageChangeComboBox.SelectedIndex = (int)AssistSettings.Current.Language;
        }

        #region Language Selection Settings

        private async void LanguageChangeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageChangeComboBox.SelectedIndex == (int)AssistSettings.Current.Language)
                return;

            AssistSettings.Current.Language = (Enums.ELanguage)LanguageChangeComboBox.SelectedIndex;
            App.ChangeLanguage();
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }

        #endregion

        #region Window Size Settings

        private async void WindowSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(WindowSizeComboBox.SelectedIndex == (int)AssistSettings.Current.Resolution)
                return;

            Trace.WriteLine(WindowSizeComboBox.SelectedIndex);
            AssistSettings.Current.Resolution = (Enums.EWindowSize) WindowSizeComboBox.SelectedIndex;
            AssistApplication.AppInstance.OpenAssistMainWindowToSettings();
        }


        #endregion

        
    }
}
