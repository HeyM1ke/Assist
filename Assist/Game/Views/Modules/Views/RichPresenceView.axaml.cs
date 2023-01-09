using System;
using System.Collections.Generic;
using Assist.Game.Services;
using Assist.Objects.Discord;
using Assist.Settings;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Assist.Game.Views.Modules.Views
{
    public partial class RichPresenceView : UserControl
    {
        private CheckBox enableRpc;
        private ComboBox detailsComboBox;
        private ComboBox largeImageComboBox;
        private ComboBox smallImageComboBox;

        private CheckBox showPartySize;
        private CheckBox showAgent;
        private CheckBox showScore;
        private CheckBox showGamemode;
        private CheckBox showRank;
        private readonly RichPresViewModel _viewModel;

        public RichPresenceView()
        {
            DataContext = _viewModel = new RichPresViewModel();
            InitializeComponent();
        }

        private async void RichPresView_Init(object? sender, EventArgs e)
        {
            enableRpc = this.FindControl<CheckBox>("EnableRPC");
            detailsComboBox = this.FindControl<ComboBox>("DetailsComboBox");
            largeImageComboBox = this.FindControl<ComboBox>("LargeImageComboBox");
            smallImageComboBox = this.FindControl<ComboBox>("SmallImageComboBox");
            showPartySize = this.FindControl<CheckBox>("ShowPartySizeCheck");
            showAgent = this.FindControl<CheckBox>("ShowAgentCheck");
            showScore = this.FindControl<CheckBox>("ShowScoreCheck");
            showRank = this.FindControl<CheckBox>("ShowRankCheck");
            showGamemode = this.FindControl<CheckBox>("ShowGamemodeCheck");

            ApplyCurrentSettings();
        }

        private async void ApplyCurrentSettings()
        {
            enableRpc.IsChecked = GameSettings.Current.DiscordPresenceEnabled;
            showAgent.IsChecked = GameSettings.Current.RichPresenceSettings.ShowAgent;
            showScore.IsChecked = GameSettings.Current.RichPresenceSettings.ShowScore;
            showRank.IsChecked = GameSettings.Current.RichPresenceSettings.ShowRank;
            showGamemode.IsChecked = GameSettings.Current.RichPresenceSettings.ShowMode;
            showPartySize.IsChecked = GameSettings.Current.RichPresenceSettings.ShowParty;


            // TEMP SOLUTION
            try
            {
                ((List<ComboBoxItem>)detailsComboBox.Items).FindIndex(item => item.Content.ToString() == GameSettings.Current.RichPresenceSettings.DetailsTextData);

            }
            catch (Exception e)
            {
                detailsComboBox.SelectedIndex = 0;
                GameSettings.Current.RichPresenceSettings.DetailsTextData = detailsComboBox.SelectedItem.ToString();
            }


            // TEMP SOLUTION
            try
            {
                ((List<ComboBoxItem>)largeImageComboBox.Items).FindIndex(item => item.Content.ToString() == GameSettings.Current.RichPresenceSettings.LargeImageData);

            }
            catch (Exception e)
            {
                largeImageComboBox.SelectedIndex = 0;
                GameSettings.Current.RichPresenceSettings.LargeImageData = largeImageComboBox.SelectedItem.ToString();
            }


            // TEMP SOLUTION
            try
            {
                ((List<ComboBoxItem>)smallImageComboBox.Items).FindIndex(item => item.Content.ToString() == GameSettings.Current.RichPresenceSettings.SmallImageData);

            }
            catch (Exception e)
            {
                smallImageComboBox.SelectedIndex = 0;
                GameSettings.Current.RichPresenceSettings.DetailsTextData = smallImageComboBox.SelectedItem.ToString();
            }
        }


        private async void BackBtn_Click(object? sender, RoutedEventArgs e)
        {
            ModulesViewNavigationController.Change(new SelectionView());
        }

        private async void ApplyBtn_Click(object? sender, RoutedEventArgs e)
        {
            var model = new RichPresenceModel()
            {
                DetailsTextData = ((ComboBoxItem)detailsComboBox.SelectedItem).Content.ToString(),
                LargeImageData = ((ComboBoxItem)largeImageComboBox.SelectedItem).Content.ToString(),
                SmallImageData = ((ComboBoxItem)smallImageComboBox.SelectedItem).Content.ToString(),
                ShowAgent = (bool)showAgent.IsChecked,
                ShowParty = (bool)showPartySize.IsChecked,
                ShowRank = (bool)showRank.IsChecked,
                ShowScore = (bool)showScore.IsChecked,
                ShowMode = (bool)showGamemode.IsChecked,
            };

            _viewModel.SaveSettings((bool)enableRpc.IsChecked, model);
        }
    }


    class RichPresViewModel : ViewModelBase
    {
        public async void SaveSettings(bool rpcEnabled, RichPresenceModel model)
        {
            GameSettings.Current.RichPresenceSettings = model;

            if (rpcEnabled == false && GameSettings.Current.DiscordPresenceEnabled)
            {
                // RPc needs to turn off. 
                if (DiscordPresenceController.ControllerInstance.BDiscordPresenceActive)
                {
                    await DiscordPresenceController.ControllerInstance.Shutdown();
                }
            }

            if (rpcEnabled)
            {
                // RPc needs to turn off. 
                if (!DiscordPresenceController.ControllerInstance.BDiscordPresenceActive) {
                    await DiscordPresenceController.ControllerInstance.Initalize();
                }
            }

            GameSettings.Current.DiscordPresenceEnabled = rpcEnabled;

            GameSettings.Save();
        }
    }
}
