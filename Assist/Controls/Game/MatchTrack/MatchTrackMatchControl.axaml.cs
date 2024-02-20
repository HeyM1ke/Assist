using System;
using Assist.Shared.Models.Assist;
using Assist.ViewModels.Game;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Game.MatchTrack;

public partial class MatchTrackMatchControl : UserControl
{
    private readonly MatchTrackMatchViewModel _viewModel;
    public string MatchId;

    
    public MatchTrackMatchControl(RecentMatch? data)
    {
        DataContext = _viewModel = new MatchTrackMatchViewModel();
        _viewModel.RecentMatchData = data;
        MatchId = data.MatchId;
        InitializeComponent();
    }
    
    public async void UpdateData(RecentMatch data)
    {
        _viewModel.RecentMatchData = data;
        MatchId = data.MatchId;
        await _viewModel.SetupDisplay();
    }

    private async void MatchTrackMatch_Init(object? sender, EventArgs e)
    {
        if (!Design.IsDesignMode)
            await _viewModel.SetupDisplay();
    }
}