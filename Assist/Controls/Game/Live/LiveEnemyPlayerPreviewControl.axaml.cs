using System;
using System.Threading.Tasks;
using Assist.ViewModels.Game;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ValNet.Objects.Coregame;
using ValNet.Objects.Pregame;

namespace Assist.Controls.Game.Live;

public partial class LiveEnemyPlayerPreviewControl : UserControl
{
    public readonly LivePlayerPreviewViewModel _viewModel;
    public string? PlayerId = null;
    public LiveEnemyPlayerPreviewControl()
    {
        DataContext = _viewModel = new LivePlayerPreviewViewModel();
        InitializeComponent();
    }
    
    public LiveEnemyPlayerPreviewControl(PregameMatch.Player player, IBrush color = null)
    {
        DataContext = _viewModel = new LivePlayerPreviewViewModel();
        _viewModel.Player = player;
        
        if (_viewModel.PlayerBrush == null)
            _viewModel.PlayerBrush = color;
        
        PlayerId = player.Subject;
        InitializeComponent();
    }
    
    public LiveEnemyPlayerPreviewControl(CoregameMatch.Player player, IBrush color = null)
    {
        DataContext = _viewModel = new LivePlayerPreviewViewModel();
        _viewModel.CorePlayer = player;

        if (_viewModel.PlayerBrush == null)
            _viewModel.PlayerBrush = color;

        PlayerId = player.Subject;
        InitializeComponent();
    }

    public async Task UpdatePlayer(PregameMatch.Player player, IBrush playerColor = null)
    {
        _viewModel.Player = player;

        if (_viewModel.PlayerBrush == null)
            _viewModel.PlayerBrush = playerColor;

        await _viewModel.UpdatePlayerData();
    }

    public async Task UpdatePlayer(CoregameMatch.Player player, IBrush playerColor = null)
    {
        _viewModel.CorePlayer = player;

        if (_viewModel.PlayerBrush == null)
            _viewModel.PlayerBrush = playerColor;

        await _viewModel.UpdateCorePlayerData();
    }

    private async void LivePlayerPreview_Init(object? sender, EventArgs e)
    {
        if(Design.IsDesignMode)
            return;

        if (_viewModel.CorePlayer != null)
            await _viewModel.UpdateCorePlayerData();
        else
            await _viewModel.UpdatePlayerData();
    }
}