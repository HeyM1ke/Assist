using System;
using System.Threading.Tasks;
using Assist.Game.Controls.Live.ViewModels;
using Assist.Game.Models.Recent;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Controls.Live;

public partial class MatchReportDisplayControl : UserControl
{
    private readonly MatchReportDisplayViewModel _viewModel;
    public string MatchId { get; set; } = string.Empty;
    public MatchReportDisplayControl()
    {
        DataContext = _viewModel = new MatchReportDisplayViewModel();
        InitializeComponent();
    }
    
    public MatchReportDisplayControl(RecentMatch? data)
    {
        DataContext = _viewModel = new MatchReportDisplayViewModel();
        _viewModel.RecentMatchData = data;
        MatchId = data.MatchId;
        InitializeComponent();
    }
    
    private async void ReportDisplay_Initialized(object? sender, EventArgs e)
    {
        if (!Design.IsDesignMode)
            await _viewModel.SetupDisplay();
    }

    public async void UpdateData(RecentMatch data)
    {
        _viewModel.RecentMatchData = data;
        MatchId = data.MatchId;
        await _viewModel.SetupDisplay();
    }
}