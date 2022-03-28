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
using Assist.Controls.Progression.Viewmodels;
using Assist.MVVM.Model;

namespace Assist.Controls.Progression
{
    /// <summary>
    /// Interaction logic for BattlepassItem.xaml
    /// </summary>
    public partial class BattlepassItem : UserControl
    {
        private BattlepassItemViewModel _viewModel;
        public BattlepassItem()
        {
            InitializeComponent();
        }

        public BattlepassItem(BattlePassObj.RewardItem item)
        {
            DataContext = _viewModel = new BattlepassItemViewModel();
            _viewModel.Reward = item;
            InitializeComponent();
            _viewModel.LoadItem();
        }
    }
}
