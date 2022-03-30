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
using Assist.Controls.Store;

namespace Assist.Controls.Home
{
    /// <summary>
    /// Interaction logic for DashboardStoreControl.xaml
    /// </summary>
    public partial class DashboardStoreControl : UserControl
    {
        private readonly StoreControlViewModel _viewModel;

        public DashboardStoreControl()
        {
            DataContext = _viewModel = new StoreControlViewModel();
            
            InitializeComponent();
        }

        private async void StoreControl_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.GetShop();
            _viewModel.SetupControl();
            SetupItems();

        }

        private async Task SetupItems()
        {
            for (int i = 0; i < _viewModel.StoreItemOffers.Count; i++)
            {
                ItemsGrid.Children.Add(new DashboardItemControl(_viewModel.StoreItemOffers[i])
                {
                    Width = 170,
                    Height = 95
                });
                

            }
        }
    }
}
