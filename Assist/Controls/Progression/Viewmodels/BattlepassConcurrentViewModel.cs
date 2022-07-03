using Assist.MVVM.ViewModel;
using Assist.Properties.Languages;
using Assist.Services;

using System.Windows.Media.Imaging;
using Assist.MVVM.View.Progression.Sectors;

namespace Assist.Controls.Progression.Viewmodels
{
    internal class BattlepassConcurrentViewModel : ViewModelBase
    {

        private BitmapImage _contractRewardImage;
        public BitmapImage ContractRewardImage
        {
            get => _contractRewardImage;
            set => SetProperty(ref _contractRewardImage, value);
        }

        private string _contractRewardName = "Loading..";
        public string ContractRewardName
        {
            get => _contractRewardName;
            set => SetProperty(ref _contractRewardName, value);
        }

        private string _contractTier;
        public string ContractTier
        {
            get => _contractTier;
            set => SetProperty(ref _contractTier, value);
        }


        private int _contractTierNumber;
        public int ContractTierNumber
        {
            get => _contractTierNumber;
            set => SetProperty(ref _contractTierNumber, value);
        }

        private string _contractTierXp;
        public string ContractTierXp
        {
            get => _contractTierXp;
            set => SetProperty(ref _contractTierXp, value);
        }

        private int _currentXp;
        public int CurrentXp
        {
            get => _currentXp;
            set => SetProperty(ref _currentXp, value);
        }

        private int _neededXp;
        public int NeededXp
        {
            get => _neededXp;
            set => SetProperty(ref _neededXp, value);
        }

        private double _progression;
        public double Progression
        {
            get => _progression;
            set => SetProperty(ref _progression, value);
        }

        public async void SetupControl()
        {
            // todo: clean this
            var bpContract = await AssistApplication.AppInstance.CurrentUser.Contracts.GetContract(ProgressionBattlepass.Instance.CurrentBattlepassId);
            ContractTierNumber = bpContract.ProgressionLevelReached;
            var xpTier = bpContract.ProgressionLevelReached - 1;
            NeededXp = (xpTier * 750) + 2000;
            CurrentXp = bpContract.ProgressionTowardsNextLevel;
            ContractTierXp = $"{CurrentXp}XP / {NeededXp}XP";
            ContractTier = $"{Lang.Progression_Battlepass_CurrTier} {ContractTierNumber+1}";
            Progression = (double)CurrentXp / NeededXp * 100;

            //Get Reward Information
            var battlepass = await AssistApplication.ApiService.GetBattlepassAsync(ProgressionBattlepass.Instance.CurrentBattlepassId);

            if (ContractTierNumber == 55)
            {
                var levels = battlepass.Chapters[^1].Levels;
                var level = levels[^1];

                ContractRewardImage = App.LoadImageUrl(level.RewardDisplayIcon);
                ContractRewardName = level.RewardName;
                ContractTierXp = string.Empty;
                ContractTier = Lang.Progression_Battlepass_Completed;
                Progression = 100;

                return;
            }

            var contactLevel = ContractTierNumber / 5;
            var contactLevelTier = ContractTierNumber - (contactLevel * 5);
            var itemData = battlepass.Chapters[contactLevel].Levels[contactLevelTier];
            ContractRewardImage = App.LoadImageUrl(itemData.RewardDisplayIcon);
            ContractRewardName = itemData.RewardName;
        }

    }
}