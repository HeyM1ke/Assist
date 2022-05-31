using Assist.MVVM.ViewModel;
using Assist.Objects.Valorant.Bp;

using System.Windows.Media.Imaging;

namespace Assist.Controls.Progression.Viewmodels
{
    public class BattlepassItemViewModel : ViewModelBase
    {

        private int _tierNumber;
        public int TierNumber
        {
            get => _tierNumber;
            set => SetProperty(ref _tierNumber, value);
        }

        private BitmapImage _rewardImage;
        public BitmapImage RewardImage
        {
            get => _rewardImage;
            set => SetProperty(ref _rewardImage, value);
        }

        private BattlepassLevel _level;
        public BattlepassLevel Level
        {
            get => _level;
            set => SetProperty(ref _level, value);
        }

        public void LoadItem()
        {
            if (_level.RewardDisplayIcon != null)
                RewardImage = App.LoadImageUrl(Level.RewardDisplayIcon, BitmapCacheOption.None);
        }

    }
}
