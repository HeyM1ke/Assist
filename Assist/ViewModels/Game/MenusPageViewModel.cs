﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Assist.Controls.Game.Live;
using Assist.Core.Helpers;
using Assist.Models.Game;
using Assist.Models.Socket;
using Assist.Services.Assist;
using Assist.Shared.Models.Assist;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Serilog;
using ValNet.Objects.Exceptions;
using ValNet.Objects.Local;
using ValNet.Objects.Parties;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace Assist.ViewModels.Game;

public partial class MenusPageViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<MenuPartyPlayerControl> _currentUsers = new ObservableCollection<MenuPartyPlayerControl>();
    [ObservableProperty] private bool _partyCodeButtonVisible = false;
    [ObservableProperty] private bool _matchHistory = false;
    [ObservableProperty] private string _currentPartyId;
    [ObservableProperty] private string _queueName;
    [ObservableProperty] private string _partySize;
    
    public async Task Setup(PresenceV4Message start = null)
    {
        AssistApplication.RiotWebsocketService.UserPresenceMessageEvent += RiotWebsocketServiceOnUserPresenceMessageEvent;

        if (start != null)
        {
            RiotWebsocketServiceOnUserPresenceMessageEvent(start);
        }
        
        //CheckAndHandleRecentMatchTracking();
        //EndorseEnabled = true;
    }

     private async void RiotWebsocketServiceOnUserPresenceMessageEvent(PresenceV4Message obj)
        {
            // On User Update 
            var data = await ValorantHelper.GetPresenceData(obj.MessageData.Presences[0]);

            if (data != null)
            {
                if(data.sessionLoopState != "MENUS")
                    return;
                
                CurrentPartyId = data.partyId;
                UpdateGeneralPartyInformation(data);

                if (data.partySize > 1)
                {
                    await Dispatcher.UIThread.InvokeAsync(() => { HandleMoreThanOneParty(); });
                }
                else
                {
                    /*var alreadyHere = LiveViewViewModel.ReputationUserV2s.ContainsKey(obj.data.presences[0].puuid);
                    if (!alreadyHere)
                    {
                        await LiveViewViewModel.GetUserReputations(new List<string>() { obj.data.presences[0].puuid });
                    }
                    
                    var profileAlreadyHere = LiveViewViewModel.AssistProfiles.ContainsKey(obj.data.presences[0].puuid);
                    if (!profileAlreadyHere)
                    {
                        await LiveViewViewModel.GetUserProfile(obj.data.presences[0].puuid);
                    }*/
                    
                    if (CurrentUsers.Count == 0)
                    {
                        /*if (LiveViewViewModel.AssistProfiles.TryGetValue(obj.MessageData.Presences[0].puuid, out var profileData))
                        {
                            
                            AddUserToList(
                                new MenuPartyPlayerControl()
                                {
                                    LevelText = $"{data.accountLevel:n0}",
                                    PlayerId = obj.MessageData.Presences[0].puuid,
                                    PlayerName = string.IsNullOrEmpty(profileData.DisplayName) ? obj.MessageData.Presences[0].game_name : profileData.DisplayName ,
                                    PlayerTitle = !string.IsNullOrEmpty(profileData.DisplayName) ?  $"{obj.MessageData.Presences[0].game_name}#{obj.MessageData.Presences[0].game_tag}" : "", 
                                    PlayercardImage = $"https://cdn.assistval.com/playercards/{data.playerCardId}_LargeArt.png",
                                    PlayerRankIcon = $"https://cdn.assistval.com/ranks/TX_CompetitiveTier_Large_{data.competitiveTier}.png"
                                }
                            );
                        }
                        else
                        {*/
                            AddUserToList(
                                new MenuPartyPlayerControl()
                                {
                                    LevelText = $"{data.accountLevel:n0}",
                                    PlayerId = obj.MessageData.Presences[0].puuid,
                                    PlayerName = obj.MessageData.Presences[0].game_name ,
                                    PlayerTitle = obj.MessageData.Presences[0].game_tag, 
                                    PlayercardImage = $"https://cdn.assistval.com/playercards/{data.playerCardId}_LargeArt.png",
                                    PlayerRankIcon = $"https://cdn.assistval.com/ranks/TX_CompetitiveTier_Large_{data.competitiveTier}.png"
                                }
                            );    
                        //}
                    }

                    if (data.partySize == 1)
                    {
                        
                        for (int i = 0; i < CurrentUsers.Count; i++)
                        {
                            if (CurrentUsers[i].PlayerId != AssistApplication.ActiveUser.UserData.sub)
                                RemoveUserToList(CurrentUsers[i]);
                        }
                    }
                }
            }
            
        }
     
      public async void HandleMoreThanOneParty()
        {
            try
            {
                var partyData = await AssistApplication.ActiveUser.Party.FetchParty();
                var pres = await AssistApplication.ActiveUser.Presence.GetPresences();
                if (partyData != null)
                {
                    if(partyData.Members.Count <= 1)
                        return;
                    
                    var allIds = partyData.Members.Select(x => x.Subject).ToList();
                    
                    for (int i = 0; i < partyData.Members.Count; i++)
                    {
                        var member = partyData.Members[i];
                        var currentUserBtn = CurrentUsers.FirstOrDefault(member => member.PlayerId == partyData.Members[i].Subject);
                        var pData = pres.presences.Find(pres => pres.puuid == member.Subject);
                        
                        if (pData is null && currentUserBtn is null)
                        {
                            await CreatePartyPlayerControl(member);
                            continue;
                        }
                        
                        if (currentUserBtn == null )
                        {
                            var privatePres = await ValorantHelper.GetPresenceData(pData);
                            Dispatcher.UIThread.InvokeAsync(async () =>
                            {
                                AddUserToList(
                                    new MenuPartyPlayerControl()
                                    {
                                        LevelText = $"{privatePres.accountLevel:n0}",
                                        PlayerId = pData.puuid,
                                        PlayerName = pData.game_name ,
                                        PlayerTitle = pData.game_tag, 
                                        PlayercardImage = $"https://cdn.assistval.com/playercards/{privatePres.playerCardId}_LargeArt.png",
                                        PlayerRankIcon = $"https://cdn.assistval.com/ranks/TX_CompetitiveTier_Large_{privatePres.competitiveTier}.png"
                                    }
                                );
                            });
                        }
                    }

                    for (int i = 0; i < CurrentUsers.Count; i++)
                    {
                        var d = partyData.Members.Find(member => member.Subject == CurrentUsers[i].PlayerId);
                        if (d == null)
                        {
                            RemoveUserToList(CurrentUsers[i]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Fatal("Failed to get Party");
                Log.Fatal(e.Message);

                if (e is RequestException)
                {
                    var test = e as RequestException;

                    if (test != null)
                    {
                        Log.Fatal(test.StatusCode.ToString());
                        Log.Fatal(test.Content);
                    }
                }
            }
        }

        private async Task CreatePartyPlayerControl(ValorantPartyPlayer member)
        {
            Log.Information("Party member information is not available from the presence.");
            if (LiveViewViewModel.ValorantPlayers.TryGetValue(member.Subject, out var possiblePlayerData))
            {
                if (!possiblePlayerData.IsOld()) { Log.Information("Previously found playerdata is not null, continuing."); }
                else { await possiblePlayerData.Setup(); }
                
                Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    AddUserToList(
                        new MenuPartyPlayerControl()
                        {
                            LevelText = $"{member.AccountLevel:n0}",
                            PlayerId = member.Subject,
                            PlayerName = possiblePlayerData.GameName ,
                            PlayerTitle = possiblePlayerData.Tagline, 
                            PlayercardImage = $"https://cdn.assistval.com/playercards/{member.PlayerCardID}_LargeArt.png",
                            PlayerRankIcon = $"https://cdn.assistval.com/ranks/TX_CompetitiveTier_Large_{possiblePlayerData.CompetitiveTier}.png"
                        }
                    ); 

                });

                return;
            }

            // If it doesnt exist create and set, and add to dictionary.
            var ply = new ValorantPlayerStorage(member.Subject);
            await ply.Setup();
            LiveViewViewModel.ValorantPlayers.TryAdd(member.Subject, ply);
            
            Log.Information("Party Member Info: SUB : {0} | PCARD : {1}", member.Subject, member.PlayerCardID);
            
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                AddUserToList(
                    new MenuPartyPlayerControl()
                    {
                        LevelText = $"{member.AccountLevel:n0}",
                        PlayerId = member.Subject,
                        PlayerName = possiblePlayerData.GameName ,
                        PlayerTitle = possiblePlayerData.Tagline, 
                        PlayercardImage = $"https://cdn.assistval.com/playercards/{member.PlayerCardID}_LargeArt.png",
                        PlayerRankIcon = $"https://cdn.assistval.com/ranks/TX_CompetitiveTier_Large_{possiblePlayerData.CompetitiveTier}.png"
                    }
                ); 
            });
        }

        public async void AddUserToList(MenuPartyPlayerControl u)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                CurrentUsers.Add(u);
            });
        }

        public async void RemoveUserToList(MenuPartyPlayerControl u)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                CurrentUsers.Remove(u);
            });
        }
        
        public async void UpdateGeneralPartyInformation(PlayerPresence data)
        {
            var currentUserBtn = CurrentUsers.FirstOrDefault(member => member.PlayerId == AssistApplication.ActiveUser.UserData.sub);
            Log.Information($"QUEUE ID {data.queueId}");
            QueueName = ValorantHelper.DetermineQueueKey(data.queueId).ToUpper();
            PartySize = $"{data.partySize}/{data.maxPartySize}";

            if (data.maxPartySize > 5)
            {
                QueueName = "CUSTOM GAME (Not currently supported within Assist)";
            }

            if (currentUserBtn != null)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    currentUserBtn.PlayercardImage = $"https://cdn.assistval.com/playercards/{data.playerCardId}_LargeArt.png";
                    
                });
            }
        }

        public async Task SetupWithLocalPresence(ChatV4PresenceObj.Presence obj = null)
        {
            var data = await ValorantHelper.GetPresenceData(obj);
            if (data != null)
            {
                if(data.sessionLoopState != "MENUS")
                    return;
                
                CurrentPartyId = data.partyId;
                UpdateGeneralPartyInformation(data);

                if (data.partySize > 1)
                {
                    await Dispatcher.UIThread.InvokeAsync(() => { HandleMoreThanOneParty(); });
                }
                else
                {
                    if (CurrentUsers.Count == 0)
                    {
                        var pData = await ValorantHelper.GetPresenceData(obj);
                        
                        AddUserToList(new MenuPartyPlayerControl()
                            {
                                LevelText = $"{data.accountLevel:n0}",
                                PlayerId = obj.puuid,
                                PlayerName = obj.game_name ,
                                PlayerTitle = obj.game_tag, 
                                PlayercardImage = $"https://cdn.assistval.com/playercards/{data.playerCardId}_LargeArt.png",
                                PlayerRankIcon = $"https://cdn.assistval.com/ranks/TX_CompetitiveTier_Large_{data.competitiveTier}.png"
                            });    
                    }

                    if (data.partySize == 1)
                    {
                        for (int i = 0; i < CurrentUsers.Count; i++)
                        {
                            if (CurrentUsers[i].PlayerId != AssistApplication.ActiveUser.UserData.sub)
                                RemoveUserToList(CurrentUsers[i]);
                        }
                    }
                }
            }

            await Setup(null);
        }

        public void UnsubscribeFromEvents()
        {
            Log.Information("Page is Unloaded, Unsubbing from Events from MenusPageView");
            AssistApplication.RiotWebsocketService.UserPresenceMessageEvent -= RiotWebsocketServiceOnUserPresenceMessageEvent;
        }
        
        public async void CheckAndHandleRecentMatchTracking()
        {
            var allUnfinished = RecentService.Current.RecentMatches.FindAll(x => !x.IsCompleted);

            foreach (var unfinishedMatch in allUnfinished)
            {
                // This is ran in the menus, meaning this is assuming no game is currently going on within the current CLient.

                if (unfinishedMatch.Players.Count <= 1 && string.IsNullOrEmpty(unfinishedMatch.Gamemode)){
                    Log.Information("Deleted Faulty Custom Game Match");
                    RecentService.Current.RemoveMatch(unfinishedMatch.MatchId);
                    continue;
                }
                
                if (unfinishedMatch.Result != RecentMatch.MatchResult.REMAKE)
                {
                    /*if (!unfinishedMatch.OwningPlayer.Equals(AssistApplication.Current.CurrentUser.UserData.sub))
                    {
                        if (DateTime.Now.ToUniversalTime() > unfinishedMatch.DateOfMatch.ToUniversalTime().AddMinutes(5)) RecentService.Current.RemoveMatch(unfinishedMatch.MatchId);
                        return;
                    }*/

                   
                    
                    await RecentService.Current.UpdateMatch(unfinishedMatch.MatchId);
                    
                    var updatedMatch =
                        RecentService.Current.RecentMatches.Find(x => x.MatchId.Equals(unfinishedMatch.MatchId));

                    if (!updatedMatch.IsCompleted && updatedMatch.MatchTrack_LastState.Equals("PREGAME", StringComparison.OrdinalIgnoreCase))  // This means that the match is still not finished while the player is in the match.
                    {
                        Log.Information("Found match that is not valid. Marking as Remake");
                        updatedMatch.MatchTrack_LastState = "MENUS";
                        updatedMatch.Result = RecentMatch.MatchResult.REMAKE;
                        RecentService.Current.UpdateMatch(updatedMatch);
                    }
                }
            }
        }
        
}
