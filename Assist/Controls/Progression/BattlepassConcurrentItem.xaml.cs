using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Assist.Controls.Progression
{
    /// <summary>
    /// Interaction logic for BattlepassConcurrentItem.xaml
    /// </summary>
    public partial class BattlepassConcurrentItem : UserControl
    {
        private BattlepassConcurrentViewModel _viewModel;
        public BattlepassConcurrentItem()
        {
            DataContext = _viewModel = new BattlepassConcurrentViewModel();
            _viewModel.SetupControl();
            InitializeComponent();
        }
    }
}
