using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.Objects.AssistApi.Valorant.Battlepass;
using Assist.ViewModels;
using Avalonia.Controls;
using ReactiveUI;

namespace Assist.Controls.Progression.ViewModels
{
    internal class BPConcurrentPreviewViewModel : ViewModelBase
    {
        private string _rewardImage = "";
        public string RewardImage
        {
            get => _rewardImage;
            set => this.RaiseAndSetIfChanged(ref _rewardImage, value);
        }


        private string _rewardName = "Loading...";
        public string RewardName
        {
            get => _rewardName;
            set => this.RaiseAndSetIfChanged(ref _rewardName, value);
        }

        private string _rewardTier = "Tier";

        public string RewardTier
        {
            get => _rewardTier;
            set => this.RaiseAndSetIfChanged(ref _rewardTier, value);
        }

        private string _nextRewardTier;

        public string NextRewardTier
        {
            get => _nextRewardTier;
            set => this.RaiseAndSetIfChanged(ref _nextRewardTier, value);
        }

        private double _currentXp = 0;
        public double CurrentXp
        {
            get => _currentXp;
            set => this.RaiseAndSetIfChanged(ref _currentXp, value);
        }

        private double _neededXp = 200;
        public double NeededXp
        {
            get => _neededXp;
            set => this.RaiseAndSetIfChanged(ref _neededXp, value);
        }


        public async Task GetBattlepassData()
        {

            if (Design.IsDesignMode)
                return;

            var vPData =
                await AssistApplication.Current.CurrentUser.Contracts.GetContract(AssistApplication
                    .CurrentBattlepassId);
            var bpData = await AssistApplication.ApiService.GetBattlepassAsync(AssistApplication.CurrentBattlepassId);

            var currTier = vPData.ProgressionLevelReached;
            CurrentXp = vPData.ProgressionTowardsNextLevel;
            NeededXp = DetermineNeededXp(currTier);

            List<BattlepassLevel> t = new List<BattlepassLevel>();
            foreach (var chap in bpData.Chapters)
            {
                foreach (var lvl in chap.Levels)
                {
                    t.Add(lvl);
                }
            }

            var nxtTier = currTier + 1;
            RewardName = t[currTier].RewardName;
            RewardTier = "Tier " + nxtTier;
            RewardImage = GetShowcaseImage(t[currTier]);
            NextRewardTier = GetShowcaseImage(t[nxtTier]);
        }

        private int DetermineNeededXp(int currentTier)
        {
            return 2000 + (currentTier - 1) * 750;
        }

        private string GetShowcaseImage(BattlepassLevel item)
        {
            if (item.Reward.Type == "Spray")
            {
                if (item.Reward.SprayFullImage != null)
                    return item.Reward.SprayFullImage;
            }

            if (item.Reward.Type == "PlayerCard")
            {
                if (item.Reward.PlayercardLargeArt != null)
                    return item.Reward.PlayercardLargeArt;
            }

            if (item.RewardDisplayIcon != null)
                return item.RewardDisplayIcon;

            return null;
        }
    }
}
