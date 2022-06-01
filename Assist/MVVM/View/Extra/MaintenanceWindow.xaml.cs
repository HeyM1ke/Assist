using Assist.Objects;

using System.Windows;
using System.Windows.Input;

namespace Assist.MVVM.View.Extra
{
    /// <summary>
    /// Interaction logic for MaintenanceWindow.xaml
    /// </summary>
    public partial class MaintenanceWindow
    {

        public MaintenanceWindow()
        {
            InitializeComponent();
        }

        public MaintenanceWindow(AssistMaintenance maintenance)
        {
            InitializeComponent();
            errorText.Text = maintenance.DownForMaintenanceMessage;
        }

        private void closeOkBTn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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

        private void windowBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        #endregion

    }
}
