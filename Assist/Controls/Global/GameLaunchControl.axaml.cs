using System;
using Assist.Controls.Global.Popup;
using Assist.Controls.Global.ViewModels;
using Assist.Services.Popup;
using Assist.Services.Riot;
using Assist.ViewModels;
using AsyncImageLoader;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Global
{
    public partial class GameLaunchControl : UserControl
    {
        private readonly GameLaunchViewModel _viewModel;

        public GameLaunchControl()
        {
            DataContext = _viewModel = new GameLaunchViewModel();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _viewModel.CheckEnable();
        }

        private async void LaunchBtn_Click(object? sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            btn.IsEnabled = false;
            btn.Content = Properties.Resources.Global_Loading;
            await new RiotClientService().LaunchClient();
        }

        private async void GameLaunch_Init(object? sender, EventArgs e)
        {
            if(Design.IsDesignMode)
                return;

            var inv = await _viewModel.SetPlayercard();

            if (inv != null)
            {
                _viewModel.ProfilePlayercard = $"https://content.assistapp.dev/playercards/{inv.PlayerData.PlayerCardID}_LargeArt.png";
            }
        }

        private async void OpenSettingsBtn_Click(object? sender, RoutedEventArgs e)
        {
            PopupSystem.SpawnCustomPopup(new GameLaunchSettingsPopup());
        }
    }
}
