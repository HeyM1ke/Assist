using System;
using Assist.Game.Controls.Live.ViewModels;
using AssistUser.Lib.Reputations.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ValNet.Objects.Player;

namespace Assist.Game.Controls.Live;

public partial class EndorsementCard : UserControl
{
    private readonly EndorsementCardViewModel _viewModel;

    public EndorsementCard()
    {
        DataContext = _viewModel = new EndorsementCardViewModel();
        InitializeComponent();
    }
    
    public EndorsementCard(MatchDetailsObj.Player playerData, string MatchId)
    {
        DataContext = _viewModel = new EndorsementCardViewModel();
        _viewModel.PlayerData = playerData;
        _viewModel.MatchId = MatchId;
        InitializeComponent();
    }

    private async void EndorsementCard_Init(object? sender, EventArgs e)
    {
        await _viewModel.Setup();
    }
    
    private async void GoodTeammateBtn_Click(object? sender, RoutedEventArgs e)
    {
        await _viewModel.EndorsePlayer(EndorsementTypeV2.POSITIVE);
    }
    

    private async void BadTeammateBtn_Click(object? sender, RoutedEventArgs e)
    {
        await _viewModel.EndorsePlayer(EndorsementTypeV2.NEGATIVE);
    }
}