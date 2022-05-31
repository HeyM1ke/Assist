using Assist.Controls.Progression;
using Assist.MVVM.View.Progression.ViewModels;

using System.Windows;

namespace Assist.MVVM.View.Progression.Sectors
{
    /// <summary>
    /// Interaction logic for ProgressionBattlepass.xaml
    /// </summary>
    public partial class ProgressionBattlepass
    {

        public static ProgressionBattlepass Instance;

        private readonly BattlepassSectorViewModel _viewModel;

        public ProgressionBattlepass()
        {
            Instance = this;
            DataContext = _viewModel = new BattlepassSectorViewModel();
            InitializeComponent();
        }

        private async void Battlepass_Loaded(object sender, RoutedEventArgs e)
        {
           await _viewModel.LoadBattlepass(BattlepassContainer);

        }

        public static void ClearSelected()
        {
            if (Instance == null)
                return;

            foreach (var obj in Instance.BattlepassContainer.Children)
            {
                if(obj is BattlepassItem item)
                    item.bIsSelected = false;
            }
        }

    }
}
