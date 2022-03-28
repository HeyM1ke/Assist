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
using Assist.Controls.Store.ViewModels;
using Assist.MVVM.ViewModel;
using ValNet.Objects.Store;

namespace Assist.Controls.Store
{
    /// <summary>
    /// Interaction logic for BundleView.xaml
    /// </summary>
    public partial class BundleView : UserControl
    {
        private BundleViewViewModel _viewModel;

        public BundleView()
        {
            DataContext = _viewModel = new BundleViewViewModel();
            _viewModel.Bundle = AssistApplication.AppInstance.CurrentUser.Store.PlayerStore.FeaturedBundle.Bundle;

            InitializeComponent();
        }

        private async void Bundle_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.SetupBundle();
        }
    }
}
