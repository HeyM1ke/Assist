using System;
using System.Collections.Generic;
using Assist.Controls.Dashboard.ViewModels;
using Assist.Controls.Progression;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Dashboard;

public partial class ProgressionOverview : UserControl
{
    private readonly ProgressionOverviewViewModel _viewModel;

    public ProgressionOverview()
    {
        DataContext = _viewModel = new ProgressionOverviewViewModel();
        InitializeComponent();
    }
    
    private async void ProgressionOverview_Init(object? sender, EventArgs e)
    {
        if (Design.IsDesignMode)
        {
            return;
        }
        
        await _viewModel.Setup();
        await _viewModel.HandleDailyTicket(); // Stores the ticket.
        UpdateTicketData();
    }

    private async void UpdateTicketData()
    {
        var t = new List<AssistDailyDiamondControl>() { DiamondControl1, DiamondControl2, DiamondControl3, DiamondControl4 };
        
        for (int i = 0; i < ProgressionOverviewViewModel.UserTicket.DailyRewards.Milestones.Count; i++)
        {
            var currMilestone = ProgressionOverviewViewModel.UserTicket.DailyRewards.Milestones[i];

            if (currMilestone.Progress == 4)
            {
                t[i].IsCompleted = true;
                continue;
            }
            
            if (currMilestone.Progress != 4 && currMilestone.Progress > 0)
            {
                t[i].IsCurrent = true;
                t[i].ProgressText = $"{currMilestone.Progress}/4";
                break;
            }
        }
    }

    private async void ProgressionOverview_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode)
        {
            return;
        }
        
        await _viewModel.Setup();
        await _viewModel.HandleDailyTicket(); // Stores the ticket.
        UpdateTicketData();
    }
}