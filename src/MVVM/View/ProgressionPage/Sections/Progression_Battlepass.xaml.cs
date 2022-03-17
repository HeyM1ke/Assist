using Assist.MVVM.ViewModel;
using System;
using System.Collections.Generic;
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
using Assist.Controls;
using System.Diagnostics;

namespace Assist.MVVM.View.ProgressionPage.Sections
{
    /// <summary>
    /// Interaction logic for Progression_Battlepass.xaml
    /// </summary>
    public partial class Progression_Battlepass : Page
    {
        private ProgressionBattlepassViewmodel _viewModel;        
        public Progression_Battlepass()
        {
            DataContext = _viewModel  = AssistApplication.AppInstance.BattlepassViewModel = new ProgressionBattlepassViewmodel();
            InitializeComponent();
        }

        private async void BattlepassPage_Initialized(object sender, EventArgs e)
        {
            // Get Current Riotuser Battlepass Contract Data
            var data = await AssistApplication.AppInstance.CurrentUser.Contracts.GetCurrentBattlepass();

            // Get Assist Api data
            await _viewModel.GetBattlepassData();
            if(_viewModel.battlepass is not null)
            {
                int totalRewards = 1;
                for(int i = 0; i < _viewModel.battlepass.Count; i++)
                {
                    for (int j = 0; j < _viewModel.battlepass[i].itemsInChapter.Count; j++)
                    {
                        double dIsEarned = data.ProgressionLevelReached >= totalRewards ? 0.9 : 0;
                        bool bIsEarned = data.ProgressionLevelReached >= totalRewards ? true : false;
                        bool bCurrent = data.ProgressionLevelReached + 1 == totalRewards ? true : false;
                        var control = new AssistBattlepassItem(_viewModel.battlepass[i].itemsInChapter[j])
                        {
                            Margin = new Thickness(7),
                        };
                        control.MouseDown += Item_MouseDown;
                        control.earnedImage.Opacity = dIsEarned;
                        control.bIsEarned = bIsEarned;
                        control.bCurrentItem = bCurrent;
                        BattlepassItemContainer.Children.Add(control);
                        if (bCurrent)
                            ChangeFeaturedItem(control);
                        totalRewards++;
                    }
                }
            }
        }

        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AssistBattlepassItem item = (AssistBattlepassItem)sender;

            ChangeFeaturedItem(item);
        }

        private async void ChangeFeaturedItem(AssistBattlepassItem item)
        {
            if(_viewModel.currentItem is not null)
            {
                _viewModel.currentItem.bIsSelected = false;
            }

            _viewModel.currentItem = item;
            item.bIsSelected = true;
            ShowcaseDisplayName.Content = item.itemData.rewardName;
            if (item.bIsEarned)
                ShowcaseIsEarned.Visibility = Visibility.Visible;
            else
                ShowcaseIsEarned.Visibility = Visibility.Collapsed;

                ShowcaseTierLevel.Content = $"TIER {item.itemData.tierNumber}";
            loadingLabel.Visibility = Visibility.Visible;
            DetermineAndSetImage(item);
        }

        private async void DetermineAndSetImage(AssistBattlepassItem item)
        {
            if(item.itemData.extraData.rewardType == "Spray")
            {
                loadImage(item.itemData.extraData.spray_FullImage);
                loadingLabel.Visibility = Visibility.Collapsed;
                return;
            }

            if (item.itemData.extraData.rewardType == "PlayerCard")
            {
                loadImage(item.itemData.extraData.playercard_LargeArt);
                loadingLabel.Visibility = Visibility.Collapsed;
                return;
            }

            loadImage(item.itemData.imageUrl);
            loadingLabel.Visibility = Visibility.Collapsed;
        }

        private async void loadImage(string url)
        {
            // Allows the image to be loaded with the resolution it is intended to be used for.
            // Because the program is a solo resolution that doesnt change res, this is fine.

            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(url, UriKind.Absolute);
            image.EndInit();

            DisplayRewardImage.Source = image;
        }


    }
}
