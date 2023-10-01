using System;
using System.Threading.Tasks;
using Assist.ViewModels;
using AssistUser.Lib.Leagues.Models;
using Avalonia.Media;
using Newtonsoft.Json;
using ReactiveUI;
using Serilog;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Assist.Game.Views.Leagues.ViewModels;

public class PostMatchPageViewModel : ViewModelBase
{

    private static IBrush DefaultWhite = new SolidColorBrush(new Color(255, 222, 227, 232));
    private static IBrush VictoryGreen = new SolidColorBrush(new Color(204, 92, 255, 177));
    private static IBrush LossRed = new SolidColorBrush(new Color(204, 255, 75, 75));
    private string _statusText = Properties.Resources.Global_Loading;

    public string StatusText
    {
        get => _statusText;
        set => this.RaiseAndSetIfChanged(ref _statusText, value);
    }
    
    private string _playerLpChange;

    public string PlayerLpChange
    {
        get => _playerLpChange;
        set => this.RaiseAndSetIfChanged(ref _playerLpChange, value);
    }

    private bool _gotMatch = false;

    public bool GotMatch
    {
        get => _gotMatch;
        set => this.RaiseAndSetIfChanged(ref _gotMatch, value);
    }

    private IBrush? _specialColor = DefaultWhite;

    public IBrush? SpecialColor
    {
        get => _specialColor;
        set => this.RaiseAndSetIfChanged(ref _specialColor, value);
    }
    
    public async Task Setup(string? matchId = null)
    {
        if (string.IsNullOrEmpty(matchId))
        {
            return;
        }

        AssistPostMatchPointPlayer? playerData = null;
        while (!GotMatch)
        {
            var resp = await AssistApplication.Current.AssistUser.League.POSTMATCH_GetResults(matchId);

            if (resp.Code == 200)
            {
                GotMatch = true;

                try
                {
                    playerData = JsonSerializer.Deserialize<AssistPostMatchPointPlayer>(resp.Data.ToString());
                    break;
                }
                catch (Exception e)
                {
                    Log.Error("failed to read json data.");
                }
            }
            else if (resp.Code == 202)
            {
                StatusText = "Match is calculating...";
            }

            await Task.Delay(5000);

        }

        if (playerData is null) return;

        if (playerData.PointAmountChange < 0)
        {
            SpecialColor = LossRed;
            PlayerLpChange = $"{playerData.PointAmountChange} LP";   
        }
        else if (playerData.PointAmountChange > 0)
        {
            SpecialColor = VictoryGreen;
            PlayerLpChange = $"+{playerData.PointAmountChange} LP";
        }
        else
            PlayerLpChange = $"{playerData.PointAmountChange} LP";
            
        
        
        

    }

    private async Task GetMatchResults()
    {
        
    }
}