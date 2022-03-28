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
using Assist.Modules.Popup;

namespace Assist.Controls.Store
{
    /// <summary>
    /// Interaction logic for DailyItemView.xaml
    /// </summary>
    public partial class DailyItemView : UserControl
    {
        private DailyItemViewModel _viewModel;
        public DailyItemView()
        {
            DataContext = _viewModel = new DailyItemViewModel();
            InitializeComponent();
        }

        public DailyItemView(string skinId)
        {
            DataContext = _viewModel = new DailyItemViewModel();
            InitializeComponent();
            SetupControl(skinId);
        }

        private async void SetupControl(string skinId)
        {
            await _viewModel.SetupSkin(skinId);
        }

    }
}
