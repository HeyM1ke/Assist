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
            UniformGrid ItemContainer = (UniformGrid) container;
            BattlepassContractData = await AssistApplication.AppInstance.CurrentUser.Contracts.GetCurrentBattlepass();
            BattlePassData = await AssistApplication.AppInstance.AssistApiController.GetBattlepassData();

            if(BattlePassData is null || BattlepassContractData is null || ItemContainer is null)
                return;

            int tier = 1;
            // Convert obj param to Uniform as 
            for (int i = 0; i < BattlePassData.chapters.Count; i++)
            {
                var listOfItems = BattlePassData.chapters[i].levels;
                foreach (var Item in listOfItems)
                {
                    var control = new BattlepassItem(Item, tier)
                        {
                            Margin = new Thickness(8, 6, 8, 6),
                            bIsEarned = BattlepassContractData.ProgressionLevelReached >= tier,
                            bCurrentItem = tier == BattlepassContractData.ProgressionLevelReached + 1,

                        };

                        control.PreviewMouseLeftButtonUp += Control_PreviewMouseLeftButtonUp;

                        ItemContainer.Children.Add(control);

                        if (tier == BattlepassContractData.ProgressionLevelReached + 1)
                            ChangeShowcase(control);

                        tier++;
                }
                

                
            }
        }

        private async void Control_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
                await GetShowcaseImage(item);
            }

            control.bIsSelected = true;
        }

        private async Task GetShowcaseImage(BattlePassObj.Level item)
        {
            if (item.reward.type == "Spray")
            {
                if(item.reward.sprayFullImage != null)
                    ShowcaseImage = await App.LoadImageUrl(item.reward.sprayFullImage);
                return;
            }

            if (item.reward.type == "PlayerCard")
            {
                if (item.reward.playercardLargeArt != null)
                    ShowcaseImage = await App.LoadImageUrl(item.reward.playercardLargeArt);
                return;
            }

            if (item.rewardDisplayIcon != null)
                ShowcaseImage = await App.LoadImageUrl(item.rewardDisplayIcon);
        }
    }
}
