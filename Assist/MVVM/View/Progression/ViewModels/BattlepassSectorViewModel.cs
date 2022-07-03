using Assist.Controls.Progression;
using Assist.MVVM.View.Progression.Sectors;
using Assist.MVVM.ViewModel;
using Assist.Objects.Valorant.Bp;

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using Assist.Services;
using ValNet.Objects.Contacts;

namespace Assist.MVVM.View.Progression.ViewModels
{
    internal class BattlepassSectorViewModel : ViewModelBase
    {

        private ContactsFetchObj.Contract BattlepassContractData;

        private Battlepass _battlePass;


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

        public string BpActId { get; set; } = AssistApiService.BattlepassId;

        public async Task LoadBattlepass(object container)
        {
            var itemContainer = (UniformGrid) container;
            BattlepassContractData = await AssistApplication.AppInstance.CurrentUser.Contracts.GetContract(BpActId);
            _battlePass = await AssistApplication.ApiService.GetBattlepassAsync(BpActId);

            if(_battlePass == null || BattlepassContractData == null || itemContainer is null)
                return;

            var tier = 1;
            // Convert obj param to Uniform as 
            foreach (var chapter in _battlePass.Chapters)
            {
                var levels = chapter.Levels;
                foreach (var item in levels)
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
                ShowcaseName = item.RewardName;
                ShowcaseTier = $"Tier: {await control.GetTier()}";

                GetShowcaseImage(item);
                control.bIsSelected = true;
            }
        }

        private void GetShowcaseImage(BattlepassLevel item)
        {
            if (item.Reward.Type == "Spray")
            {
                if(item.Reward.SprayFullImage != null)
                    ShowcaseImage = App.LoadImageUrl(item.Reward.SprayFullImage);
                return;
            }

            if (item.Reward.Type == "PlayerCard")
            {
                if (item.Reward.PlayercardLargeArt != null)
                    ShowcaseImage = App.LoadImageUrl(item.Reward.PlayercardLargeArt);
                return;
            }

            if (item.RewardDisplayIcon != null)
                ShowcaseImage = App.LoadImageUrl(item.RewardDisplayIcon);
        }

    }
}
