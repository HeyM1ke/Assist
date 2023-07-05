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
using Avalonia.Layout;
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
        }


        private async void WeeklyMissions_Init(object? sender, EventArgs e)
        {
            
            if (Design.IsDesignMode)
                return;

            try
            {

                var obj = sender as MissionsView;

                if (obj != null)
                {
                    var r = await _viewModel.GetWeeklyMissions();
                    var s = new StackPanel()
                    {
                        Spacing = 16
                    };

                    s.Children.AddRange(r);

                    if (s.Children.Count == 0)
                    {
                        // There is no missions available. 
                        s.Children.Add(new TextBlock()
                        {
                            Text = "All Missions Completed!",
                        });
                    }
                    obj.Content = s;


                }
            }
            catch (Exception exception)
            {
                var obj = sender as MissionsView;

                if (obj != null)
                {
                    var s = new StackPanel()
                    {
                        Spacing = 16
                    };

                    s.Children.Add(new MissionControl()
                    {
                        Title = "Error Getting Missions...",
                        XpGrantAmount = "",
                        CurrentProgress = 1,
                        MaxProgress = 1,
                        PreviewText = ""
                    });

                    obj.Content = s;
                }
            }
        }

        private async void DailyMissions_Init(object? sender, EventArgs e)
        {
            
            if (Design.IsDesignMode)
                return;

            try
            {

                var obj = sender as MissionsView;

                if (obj != null)
                {
                    var r = await _viewModel.GetDailyMissions();
                    var s = new StackPanel()
                    {
                        Spacing = 16
                    };

                    s.Children.AddRange(r);

                    if (s.Children.Count == 0)
                    {
                        // There is no missions available. 
                        s.Children.Add(new TextBlock()
                        {
                            Text = "All Missions Completed!",
                        });
                    }
                    
                    obj.Content = s;


                }
            }
            catch (Exception exception)
            {
                var obj = sender as MissionsView;

                if (obj != null)
                {
                    var s = new StackPanel()
                    {
                        Spacing = 16
                    };

                    s.Children.Add(new MissionControl()
                    {
                        Title = "Error Getting Missions...",
                        XpGrantAmount = "",
                        CurrentProgress = 1,
                        MaxProgress = 1,
                        PreviewText = ""
                    });

                    obj.Content = s;
                }
            }
        }

        /// <summary>
        /// Old RecentMatchesView Init 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Stats_Init(object? sender, EventArgs e)
        {
            if (Design.IsDesignMode)
                return;

            try
            {
                var obj = sender as PlayerStatisticsView;
                if (obj != null)
                {
                    obj.isLoading = true;
                    var r = await _viewModel.GetMatchHistory();
                    if (r is null)
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
            catch (Exception exception)
            {
                var obj = sender as PlayerStatisticsView;
                if (obj != null)
                {
                    obj.isLoading = true;
                }
            }
        }


        private async void RecentMatchesView_Init(object? sender, EventArgs e)
        {
            if (Design.IsDesignMode)
                return;
            
            try
            {
                var obj = sender as RecentMatchesView;
                if (obj != null)
                {
                    obj.isLoading = true;
                    var r = await _viewModel.GetMatchHistory();
                    if (r is null)
                        return;

                    var details = await _viewModel.GetMatchDetails(r);
                    var getMatchHistoryControls = await _viewModel.CreateMatchControlsV2(details);
                    
                    var s = new StackPanel()
                    {
                        Spacing = 6,
                        Orientation = Orientation.Horizontal
                    };

                    s.Children.AddRange(getMatchHistoryControls);

                    obj.Content = s;

                    obj.isLoading = false;
                }
            }
            catch (Exception exception)
            {
                var obj = sender as RecentMatchesView;
                if (obj != null)
                {
                    obj.isLoading = true;
                }
            }
        }

        private async void RankPreviewControl_Init(object? sender, EventArgs e)
        {
            if (Design.IsDesignMode)
                return;
            
            try
            {
                var obj = sender as RankPreviewControl;
                if (obj != null)
                {
                    obj.isLoading = true;
                    await _viewModel.SetupCompetitiveDetails(obj);

                    obj.isLoading = false;
                }
            }
            catch (Exception exception)
            {
                var obj = sender as RankPreviewControl;
                if (obj != null)
                {
                    obj.isLoading = true;
                }
            }
        }
    }
}
