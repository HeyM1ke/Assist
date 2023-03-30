using System;
using System.Collections.ObjectModel;
using Assist.ViewModels;
using AssistUser.Lib.Parties.Models;
using AsyncImageLoader;
using ReactiveUI;

namespace Assist.Game.Controls.Leagues.ViewModels;

public class LeaguePartyMemberViewModel : ViewModelBase
{

    private string _playerName = "Loading..";
    public string PlayerName
    {
        get => _playerName;
        set => this.RaiseAndSetIfChanged(ref _playerName, value);
    }
    
    private string _playerRanking = "LP: 0";
    public string PlayerRanking
    {
        get => _playerRanking;
        set => this.RaiseAndSetIfChanged(ref _playerRanking, value);
    }
    
    private string _playerImage;
    public string PlayerImage
    {
        get => _playerImage;
        set => this.RaiseAndSetIfChanged(ref _playerImage, value);
    }

    private bool _isReady = false;
    public bool IsReady
    {
        get => _isReady;
        set => this.RaiseAndSetIfChanged(ref _isReady, value);
    }
    
    private ObservableCollection<AdvancedImage> _badgeImages = new ObservableCollection<AdvancedImage>();
    public ObservableCollection<AdvancedImage> BadgeImages
    {
        get => _badgeImages;
        set => this.RaiseAndSetIfChanged(ref _badgeImages, value);
    }

    public AssistPartyMember PartyMemberData;
    public async void UpdatePartyMember(AssistPartyMember data)
    {

        PartyMemberData = data;
        PlayerName = data.DisplayName;
        PlayerRanking = $"{data.LeaguePoints} LP";
        PlayerImage = data.ProfileImage;
        IsReady = data.IsReady;

        GenerateBadgeImages();
    }

    private void GenerateBadgeImages()
    {
        BadgeImages.Clear();
        
        for (int i = 0; i < PartyMemberData.Badges.Count; i++)
        {
            var imageObj =
                new AdvancedImage(new Uri($"https://content.assistapp.dev/badges/{PartyMemberData.Badges[i]}.png"))
                {
                    Width = 18,
                    Height = 18,
                    Source = $"https://content.assistapp.dev/badges/{PartyMemberData.Badges[i]}.png"
                };
            BadgeImages.Add(imageObj);
        }
    }

}