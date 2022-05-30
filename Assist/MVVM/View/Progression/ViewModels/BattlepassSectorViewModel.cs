using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using Assist.Controls;
using Assist.Controls.Progression;
using Assist.MVVM.Model;
using Assist.MVVM.View.Progression.Sectors;
using Assist.MVVM.ViewModel;
using ValNet;
using ValNet.Objects.Contacts;

namespace Assist.MVVM.View.Progression.ViewModels
{
    internal class BattlepassSectorViewModel : ViewModelBase
    {
        private ContactsFetchObj.Contract BattlepassContractData;
        private BattlePassObj BattlePassData = null;

        private string _showcaseName;

        public string ShowcaseName
        {
            get => _showcaseName;
            set => SetProperty(ref _showcaseName, value);
        }

        private string _showcaseTier;

        public string ShowcaseTier
        {
            get => _showcaseTier;
            set => SetProperty(ref _showcaseTier, value);
        }

        private BitmapImage _showcaseImage;

        public BitmapImage ShowcaseImage
        {
            get => _showcaseImage;
            set => SetProperty(ref _showcaseImage, value);
        }



        public async Task LoadBattlepass(object container)
        {
            var itemContainer = (UniformGrid) container;
            BattlepassContractData = await AssistApplication.AppInstance.CurrentUser.Contracts.GetContract(AssistApiController.currentBattlepassId);
            BattlePassData = await AssistApplication.AppInstance.AssistApiController.GetBattlepassData();

            if(BattlePassData == null || BattlepassContractData == null || itemContainer is null)
                return;

            var tier = 1;
            // Convert obj param to Uniform as 
            foreach (var chapter in BattlePassData.chapters)
            {
                var listOfItems = chapter.levels;
                foreach (var item in listOfItems)
                {
                    var control = new BattlepassItem(item, tier)
                    {
                        Margin = new Thickness(8, 6, 8, 6),
                        bIsEarned = BattlepassContractData.ProgressionLevelReached >= tier,
                        bCurrentItem = tier == BattlepassContractData.ProgressionLevelReached + 1,

                    };

                    control.PreviewMouseLeftButtonUp += Control_PreviewMouseLeftButtonUp;
                    itemContainer.Children.Add(control);

                    if (tier == BattlepassContractData.ProgressionLevelReached + 1)
                        ChangeShowcase(control);

                    tier++;
                }
            }
        }

        private void Control_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ProgressionBattlepass.ClearSelected();
            var control  = sender as BattlepassItem;

            ChangeShowcase(control);
        }

        private async void ChangeShowcase(BattlepassItem control)
        {
            if (control != null)
            {
                var item = await control.GetItem();
                ShowcaseName = item.rewardName;
                ShowcaseTier = $"Tier: {await control.GetTier()}";

                GetShowcaseImage(item);
                control.bIsSelected = true;
            }
        }

        private void GetShowcaseImage(BattlePassObj.Level item)
        {
            if (item.reward.type == "Spray")
            {
                if(item.reward.sprayFullImage != null)
                    ShowcaseImage = App.LoadImageUrl(item.reward.sprayFullImage);
                return;
            }

            if (item.reward.type == "PlayerCard")
            {
                if (item.reward.playercardLargeArt != null)
                    ShowcaseImage = App.LoadImageUrl(item.reward.playercardLargeArt);
                return;
            }

            if (item.rewardDisplayIcon != null)
                ShowcaseImage = App.LoadImageUrl(item.rewardDisplayIcon);
        }
    }
}
