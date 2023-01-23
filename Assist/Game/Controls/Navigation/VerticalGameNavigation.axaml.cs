using System;
using System.Collections.Generic;
using Assist.Game.Services;
using Assist.Game.Views.Live;
using Assist.Game.Views.Live.Pages;
using Assist.Game.Views.Lobbies;
using Assist.Game.Views.Modules;
using Assist.Properties;
using Assist.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
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
            NavigationButtons.Add(this.FindControl<NavButton>("LobbiesBtn"));
            NavigationButtons[0].IsSelected = true;
        }

        private void DashboardBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            
        }
        private void LiveBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            ClearSelected();

            if (GameViewNavigationController.CurrentPage != Services.Page.LIVE)
                GameViewNavigationController.Change(new LiveView());

            (sender as NavButton).IsSelected = true;
        }
        private void ModulesBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            ClearSelected();

            if (GameViewNavigationController.CurrentPage != Services.Page.MODULES)
                GameViewNavigationController.Change(new ModulesView());

            (sender as NavButton).IsSelected = true;
        }
        private void LobbiesBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            ClearSelected();

            if (GameViewNavigationController.CurrentPage != Services.Page.LOBBIES)
                GameViewNavigationController.Change(new LobbiesView());

            (sender as NavButton).IsSelected = true;
        }
        private void ClearSelected()
        {
            NavigationButtons.ForEach(btn => btn.IsSelected = false);
        }


        private void SupportBtn_Click(object? sender, RoutedEventArgs e)
        {
            
        }

        private void VerticalGameNav_Init(object? sender, EventArgs e)
        {
            _viewModel.SetupUserCount();
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
