using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Assist.MVVM.Model;
using Assist.MVVM.ViewModel;

namespace Assist.Controls.Progression.Viewmodels
{
    public class BattlepassItemViewModel : ViewModelBase
    {
        private int _tierNumber = 00;

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

        private BattlePassObj.Level _level;
        public BattlePassObj.Level Level
        {
            get => _level;
            set => SetProperty(ref _level, value);
        }

        public async void LoadItem()
        {
            if (Level.rewardDisplayIcon != null)
            {
                RewardImage = await App.LoadImageUrl(Level.rewardDisplayIcon, BitmapCacheOption.None);
            }
            
        }

    }
}
