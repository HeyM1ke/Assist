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

namespace Assist.Controls.Home
{
    /// <summary>
    /// Interaction logic for NewsControl.xaml
    /// </summary>
    public partial class NewsControl : UserControl
    {
        private readonly NewsControlViewModel _viewModel;
        private readonly object _newsData;
        public NewsControl()
        {
            DataContext = _viewModel = new NewsControlViewModel();
            InitializeComponent();
        }

        public NewsControl(object newsData)
        {
            DataContext = _viewModel = new NewsControlViewModel();
            InitializeComponent();
        }

        private async void NewsControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_newsData != null)
                _viewModel.LoadNews();
        }
    }
}
