using Assist.Controls.Progression.Viewmodels;
using Assist.MVVM.Model;
using Assist.Objects.Valorant.Bp;

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            get => (bool)GetValue(bIsEarnedProperty);
            set => SetValue(bIsEarnedProperty, value);
        }

        public static readonly DependencyProperty bIsEarnedProperty =
            DependencyProperty.Register("bIsEarned", typeof(bool), typeof(BattlepassItem));


        public bool bIsSelected
        {
            get { return (bool)GetValue(bIsSelectedProperty); }
            set { SetValue(bIsSelectedProperty, value); }
        }
        
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

        public BattlepassItem(BattlepassLevel item, int tier)
        {
            DataContext = _viewModel = new BattlepassItemViewModel();
            _viewModel.TierNumber = tier;
            _viewModel.Level = item;
            InitializeComponent();

        }


        public async Task<BattlepassLevel> GetItem()
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
