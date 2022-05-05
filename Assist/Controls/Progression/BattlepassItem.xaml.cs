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

        public bool bIsEarned
        {
            get { return (bool)GetValue(bIsEarnedProperty); }
            set { SetValue(bIsEarnedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for isEarned.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty bIsEarnedProperty =
            DependencyProperty.Register("bIsEarned", typeof(bool), typeof(BattlepassItem));


        public bool bIsSelected
        {
            get { return (bool)GetValue(bIsSelectedProperty); }
            set { SetValue(bIsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for bIsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty bIsSelectedProperty =
            DependencyProperty.Register("bIsSelected", typeof(bool), typeof(BattlepassItem));

        public bool bCurrentItem
        {
            get { return (bool)GetValue(bCurrentItemProperty); }
            set { SetValue(bCurrentItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for bCurrentItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty bCurrentItemProperty =
            DependencyProperty.Register("bCurrentItem", typeof(bool), typeof(BattlepassItem));

        public BattlepassItem()
        {
            InitializeComponent();
        }

        public BattlepassItem(BattlePassObj.Level item, int tier)
        {
            DataContext = _viewModel = new BattlepassItemViewModel();
            _viewModel.TierNumber = tier;
            _viewModel.Level = item;
            InitializeComponent();

        }


        public async Task<BattlePassObj.Level> GetItem()
        {
            return _viewModel.Level;
        }

        public async Task<int> GetTier() => _viewModel.TierNumber;

        private void BattlepassItem_Loaded(object sender, RoutedEventArgs e)
        {
            if(_viewModel.Level != null)
                _viewModel.LoadItem();
        }
    }
}
