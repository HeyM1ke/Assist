using Assist.MVVM.ViewModel;
using Assist.Settings;

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Assist.MVVM.View.Settings.SettingPages
{
    /// <summary>
    /// Interaction logic for Settings_General.xaml
    /// </summary>
    public partial class Settings_General : Page
    {

        public Settings_General()
        {
            DataContext = AssistSettings.Current;
            InitializeComponent();
        }

        private void Settings_General_Loaded(object sender, RoutedEventArgs e)
        {
            WindowSizeComboBox.SelectedIndex = (int)AssistSettings.Current.Resolution;
            LanguageChangeComboBox.SelectedIndex = (int)AssistSettings.Current.Language;
            SoundVol_Slider.Value = AssistSettings.Current.SoundVolume;
            SoundVol_Label.Content = Convert.ToInt32(SoundVol_Slider.Value * 100) + "%";

        }

        #region Language Selection Settings

        private bool _initialLanguageSelectionChange = true;

        private void LanguageChangeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initialLanguageSelectionChange)
            {
                _initialLanguageSelectionChange = false;
                return;
            }

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

        private void SoundVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            AssistSettings.Current.SoundVolume = SoundVol_Slider.Value;
            SoundVol_Label.Content = Convert.ToInt32(SoundVol_Slider.Value * 100) + "%";
        }

    }
}
