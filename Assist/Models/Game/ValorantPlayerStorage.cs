using System;
using System.Net;
using System.Threading.Tasks;
using Assist.ViewModels;
using AssistUser.Lib.Base.Exceptions;
using Serilog;
using ValNet.Objects.DisplayNameService;
using ValNet.Objects.Player;

namespace Assist.Models.Game;

public class ValorantPlayerStorage
{
    private const int EXPIRETIME_MINS = 30;
    private DateTime LastUpdated { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public string GameName { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public int PlayerLevel { get; set; } = 0;
    public int CompetitiveTier { get; set; } = 0;
    public int RankRating { get; set; } = 0;
    public int LeaderboardPosition { get; set; } = 0;
    private PlayerMMR? _playerMmr;
    private NameServicePlayerV2? _nameServicePlayerV2;
    public ValorantPlayerStorage(string playerId)
    {
        PlayerId = playerId;
    }
    
    public async Task Setup()
    {
        Log.Information("Created new ValorantPlayerStorage Obj");
        await GetPlayername();
        await GetPlayerMMR();
        UpdateFields();
    }

    private async Task GetPlayername()
    {
        try
        {
            var t = await AssistApplication.ActiveUser.DisplayNameService.FetchPlayersV2(PlayerId);
            _nameServicePlayerV2 = t[0];
        }
        catch (RequestException e)
        {
            if(e.StatusCode == HttpStatusCode.BadRequest){
                Log.Fatal("(ValorantPlayerStorage) FetchPlayer Names Expired.: ");
                await AssistApplication.RefreshService.CurrentUserOnTokensExpired();
            }
        }
        
        
    }
    
    private async Task GetPlayerMMR()
    {
        try
        {
            var t = await AssistApplication.ActiveUser.Player.GetPlayerMmr(PlayerId);
            _playerMmr = t;
        }
        catch (RequestException e)
        {
            if(e.StatusCode == HttpStatusCode.BadRequest){
                Log.Fatal("(ValorantPlayerStorage) FetchPlayer Names Expired.: ");
                await AssistApplication.RefreshService.CurrentUserOnTokensExpired();
            }
        }
        
        
    }

    private void UpdateFields()
    {
        if (_playerMmr is not null)
        {
            if (_playerMmr.LatestCompetitiveUpdate != null)
            {
                CompetitiveTier = _playerMmr.LatestCompetitiveUpdate.TierAfterUpdate;
                RankRating = _playerMmr.LatestCompetitiveUpdate.RankedRatingAfterUpdate;
                if (_playerMmr.QueueSkills.competitive.SeasonalInfoBySeasonID is not null)
                {
                    _playerMmr.QueueSkills.competitive.SeasonalInfoBySeasonID.TryGetValue(_playerMmr.LatestCompetitiveUpdate
                        .SeasonID, out var seasonMatches);

                    LeaderboardPosition = seasonMatches?.LeaderboardRank ?? -1;   
                }
                
            }
        }

        if (_nameServicePlayerV2 is not null)
        {
            GameName = _nameServicePlayerV2.GameName;
            Tagline = _nameServicePlayerV2.TagLine;
        }
    }
    
    public bool IsOld()
    {
        return LastUpdated.ToUniversalTime().AddMinutes(EXPIRETIME_MINS) < DateTime.UtcNow;
    }
    
}