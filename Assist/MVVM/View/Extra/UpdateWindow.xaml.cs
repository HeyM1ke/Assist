using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using Assist.MVVM.View.Extra.ViewModels;
using AutoUpdaterDotNET;

namespace Assist.MVVM.View.Extra
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        UpdateInfoEventArgs args { get; set; }
        private readonly UpdateWindowViewModel _viewModel;
        public UpdateWindow(UpdateInfoEventArgs _args)
        {
            DataContext = _viewModel = new UpdateWindowViewModel();
            args = _args;
            
            InitializeComponent();
            SetupPage();
        }

        private void SetupPage()
        {
            _viewModel.ChangeLog = args.ChangelogURL;
            _viewModel.Version = "v" + args.CurrentVersion;
            
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

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(AutoUpdater.DownloadUpdate(args))
                    Application.Current.Shutdown();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
