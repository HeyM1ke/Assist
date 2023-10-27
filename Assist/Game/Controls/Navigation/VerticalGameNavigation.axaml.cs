using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assist.Controls.Global;
using Assist.Game.Services;
using Assist.Game.Views.Live;
using Assist.Game.Views.GDashboard;
using Assist.Game.Views.Leagues;
using Assist.Game.Views.Live.Pages;
using Assist.Game.Views.Lobbies;
using Assist.Game.Views.Modules;
using Assist.Properties;
using Assist.Services.Popup;
using Assist.ViewModels;
using Assist.Views.Settings;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using ReactiveUI;

namespace Assist.Game.Controls.Navigation
{
    public partial class VerticalGameNavigation : UserControl
    {
        
        public static VerticalGameNavigation Instance;
        public List<NavButton> NavigationButtons = new List<NavButton>();
        private readonly VertGameNavVM _viewModel;

        public VerticalGameNavigation()
        {
            DataContext = _viewModel = new VertGameNavVM();
            Instance = this;
            InitializeComponent();
            
            NavigationButtons.Add(this.FindControl<NavButton>("DashboardBtn"));
            NavigationButtons.Add(this.FindControl<NavButton>("LiveBtn"));
            NavigationButtons.Add(this.FindControl<NavButton>("ModulesBtn"));
            NavigationButtons.Add(this.FindControl<NavButton>("LeaguesBtn"));
            NavigationButtons.Add(this.FindControl<NavButton>("LobbiesBtn"));
            NavigationButtons[1].IsSelected = true;

            if (AssistApplication.Current.AssistUser.Authentication.Roles.Contains("LeagueTester"))
            {
                var lBtn = this.FindControl<NavButton>("LeaguesBtn");
                lBtn.IsEnabled = true;
                lBtn.IsVisible = true;
            }
        }

        private void DashboardBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            ClearSelected();

            if (GameViewNavigationController.CurrentPage != Services.Page.DASHBOARD)
                GameViewNavigationController.Change(new Views.GDashboard.GDashboard());

            (sender as NavButton).IsSelected = true;
        }
        private void LiveBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            ClearSelected();

            if (GameViewNavigationController.CurrentPage != Services.Page.LIVE)
                GameViewNavigationController.Change(new LiveView());

            GC.Collect();
            
            (sender as NavButton).IsSelected = true;
        }
        private void ModulesBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            ClearSelected();

            if (GameViewNavigationController.CurrentPage != Services.Page.MODULES)
                GameViewNavigationController.Change(new ModulesView());

            
            GC.Collect();
            (sender as NavButton).IsSelected = true;
        }
        private void LobbiesBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            ClearSelected();

            if (GameViewNavigationController.CurrentPage != Services.Page.LOBBIES)
                GameViewNavigationController.Change(new LobbiesView());

            (sender as NavButton).IsSelected = true;
        }
        
        private void LeaguesBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            ClearSelected();

            if (GameViewNavigationController.CurrentPage != Services.Page.LEAGUES)
                GameViewNavigationController.Change(new LeagueMainPage());

            GC.Collect();
            
            (sender as NavButton).IsSelected = true;
        }
        
        private void ClearSelected()
        {
            NavigationButtons.ForEach(btn => btn.IsSelected = false);
        }

        public void DisableAll()
        {
            NavigationButtons.ForEach(btn => btn.IsEnabled = false);
        }

        private void SupportBtn_Click(object? sender, RoutedEventArgs e)
        {
            
        }

        private void VerticalGameNav_Init(object? sender, EventArgs e)
        {
            if (Design.IsDesignMode) return;
            
            _viewModel.SetupUserCount();
        }


        private void SettingsBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            if (Design.IsDesignMode) return;
            
            PopupSystem.SpawnCustomPopup(new SettingsPopup());
        }

        private void SocialBtn_Click(object? sender, RoutedEventArgs e)
        {
            var btn = sender as SocialButton;
        
            if (btn.LinkTo == null)
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = btn.LinkTo,
                UseShellExecute = true
            });
        }

        public void EnableAll()
        {
            NavigationButtons.ForEach(btn => btn.IsEnabled = true);
        }
    }

    public class VertGameNavVM : ViewModelBase
    {
        // Small Class for User Count Updating
        private string _currentAssistUserCount = "Online";

        public string CurrentAssistUserCount
        {
            get => _currentAssistUserCount;
            set => this.RaiseAndSetIfChanged(ref _currentAssistUserCount, value);
        }
        
        private string _currentAssistGameUserCount;
        
        public string CurrentAssistGameUserCount
        {
            get => _currentAssistGameUserCount;
            set => this.RaiseAndSetIfChanged(ref _currentAssistGameUserCount, value);
        }
        
        public void SetupUserCount()
        {
            AssistApplication.Current.ServerHub.RecieveMessageEvent += o =>
            {
                var number = o is int ? (int)o : (int?)null;

                if (number != null)
                {
                    CurrentAssistUserCount = $"{number} {Resources.Assist_UsersOnline}";
                }
            };
        }
    }
}
