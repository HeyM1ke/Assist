using Assist.Settings;

using System.Windows;
using System.Windows.Input;

namespace Assist.MVVM.View.Extra
{
    /// <summary>
    /// Interaction logic for LanguageSelectWindow.xaml
    /// </summary>
    public partial class LanguageSelectWindow : Window
    {
        public LanguageSelectWindow()
        {
            DataContext = AssistSettings.Current;

            InitializeComponent();
            LanguageChangeComboBox.SelectedIndex = 0;
        }

        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            AssistSettings.Current.Language = (Enums.ELanguage)LanguageChangeComboBox.SelectedIndex;
            App.ChangeLanguage();
            AssistSettings.Current.SetupLangSelected = true;
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }

        #region basic
        private void minimizeBTN_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void windowBorder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        #endregion
    }
}
