using System;
using Assist.Game.Controls.Leagues.ViewModels;
using AssistUser.Lib.Parties.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Game.Controls.Leagues;

public partial class LeaguePartyMemberControl : UserControl
{
    private readonly LeaguePartyMemberViewModel _viewModel;

    public LeaguePartyMemberControl()
    {
        DataContext = _viewModel = new LeaguePartyMemberViewModel();
        InitializeComponent();
    }

    public LeaguePartyMemberControl(AssistPartyMember data)
    {
        DataContext = _viewModel = new LeaguePartyMemberViewModel();
        _viewModel.PartyMemberData = data;
        InitializeComponent();
    }

    public void UpdatePlayerData(AssistPartyMember data)
    {
        _viewModel.UpdatePartyMember(data);
    }

    private void PartyMemberControl_Init(object? sender, EventArgs e)
    {
        if (Design.IsDesignMode || _viewModel.PartyMemberData is null) 
        {
            return;
        }

        
        UpdatePlayerData(_viewModel.PartyMemberData);
    }
}