using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using AssistWPFTest.MVVM.ViewModel;

namespace Assist.Controls.Progression.Viewmodels
{
    internal class BattlepassConcurrentViewModel : ViewModelBase
    {
        private BitmapImage _contractRewardImage = App.LoadImageUrl("https://cdn.rumblemike.com/PlayerCards/6a82ba95-436f-9d3c-8710-d3a9e09e8445_DisplayIcon.png").Result;

        public BitmapImage ContractRewardImage
        {
            get => _contractRewardImage;
            set => SetProperty(ref _contractRewardImage, value);
        }

        private string _contractRewardName = "Poggers";
        public string ContractRewardName
        {
            get => _contractRewardName;
            set => SetProperty(ref _contractRewardName, value);
        }

        public string ContractTier => $"{Properties.Languages.Lang.Progression_Battlepass_CurrTier} {ContractTierNumber}";

        private int _contractTierNumber = 5;

        public int ContractTierNumber
        {
            get => _contractTierNumber;
            set => SetProperty(ref _contractTierNumber, value);
        }

        public string ContractTierXp => $"{CurrentXp}XP / {NeededXp}XP";

        private int _currentXp = 400;

        public int CurrentXp
        {
            get => _currentXp;
            set => SetProperty(ref _currentXp, value);
        }

        private int _neededXp = 500;

        public int NeededXp
        {
            get => _neededXp;
            set => SetProperty(ref _neededXp, value);
        }
    }
}
