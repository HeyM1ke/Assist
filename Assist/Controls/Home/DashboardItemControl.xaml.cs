using System;
using System.Collections.Generic;
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
using Assist.Controls.Home.ViewModels;
using Assist.MVVM.ViewModel;

namespace Assist.Controls.Home
{
    /// <summary>
    /// Interaction logic for DashboardItemControl.xaml
    /// </summary>
    public partial class DashboardItemControl : UserControl
    {
        private readonly ItemControlViewModel _viewModel;

        public DashboardItemControl()
        {
            DataContext = _viewModel = new ItemControlViewModel();
            InitializeComponent();
        }

        public DashboardItemControl(string skinId)
        {
            DataContext = _viewModel = new ItemControlViewModel();
            InitializeComponent();
            SetupControl(skinId);
        }

        private async void SetupControl(string skinId)
        {
            await _viewModel.SetupSkinAsync(skinId);
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AssistMainWindow.Current.GoToStore();
        }
    }
}
