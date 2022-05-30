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
using Assist.MVVM.Model;

namespace Assist.Controls.Home
{
    /// <summary>
    /// Interaction logic for NewsControl.xaml
    /// </summary>
    public partial class NewsControl : UserControl
    {
        private readonly NewsControlViewModel _viewModel;
        private readonly AssistNewsObj _newsData;
        public NewsControl()
        {
            DataContext = _viewModel = new NewsControlViewModel();
            InitializeComponent();
            
        }

        public NewsControl(AssistNewsObj newsData)
        {
            _newsData = newsData;
            DataContext = _viewModel = new NewsControlViewModel();
            InitializeComponent();
        }

        private void NewsControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_newsData != null) 
                _viewModel.LoadNews(_newsData);
        }

        private async void NewsControl_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.OpenNewsUrl();
        }

    }
}
