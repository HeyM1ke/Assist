using System;
using System.Collections.Generic;
using System.Drawing;
using Assist.Controls.Dashboard;
using Assist.Controls.Global.Navigation;
using Assist.Controls.Progression;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Assist.Services;
using Assist.Views.Dashboard.ViewModels;
using Serilog;

namespace Assist.Views.Dashboard
{
    public partial class DashboardView : UserControl
    {
        private readonly DashboardViewModel _viewModel;

        public DashboardView()
        {
            DataContext = _viewModel = new DashboardViewModel();
            InitializeComponent();
            MainViewNavigationController.CurrentPage = Page.DASHBOARD;
            NavigationBar.Instance.SetSelected(0);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void WeeklyMissions_Init(object? sender, EventArgs e)
        {
            if(Design.IsDesignMode)
                return;


            var obj = sender as MissionsView;

            if (obj != null)
            {
                var r = await _viewModel.GetWeeklyMissions();
                var s = new StackPanel()
                {
                    Spacing = 16
                };

                s.Children.AddRange(r);

                obj.Content = s;


            }
        }

        private async void DailyMissions_Init(object? sender, EventArgs e)
        {
            if (Design.IsDesignMode)
                return;

            var obj = sender as MissionsView;

            if (obj != null)
            {
                var r = await _viewModel.GetDailyMissions();
                var s = new StackPanel()
                {
                    Spacing = 16
                };

                s.Children.AddRange(r);

                obj.Content = s;

            }
        }

        private async void Stats_Init(object? sender, EventArgs e)
        {
            if (Design.IsDesignMode)
                return;

            var obj = sender as PlayerStatisticsView;
            if (obj != null)
            {
                obj.isLoading = true;
                var r = await _viewModel.GetMatchHistory();
                if(r is null)
                    return;

                var details = await _viewModel.GetMatchDetails(r);
                var getMatchHistoryControls = await _viewModel.CreateMatchControls(details);
                var MostPlayed = await _viewModel.GetMostCommonAgent(details);
                obj.FeaturedAgent = $"https://content.assistapp.dev/agents/{MostPlayed}_fullportrait.png";
                await _viewModel.SetupCompetitiveDetails(obj);
                var s = new StackPanel()
                {
                    Spacing = 9
                };

                s.Children.AddRange(getMatchHistoryControls);

                obj.Content = s;

                obj.isLoading = false;
            }
        }
    }
}
