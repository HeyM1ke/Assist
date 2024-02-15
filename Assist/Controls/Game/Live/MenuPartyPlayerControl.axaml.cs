using System.Collections.Generic;
using AsyncImageLoader;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;

namespace Assist.Controls.Game.Live;

public class MenuPartyPlayerControl : TemplatedControl
{
    public string? PlayerId;
    public static readonly StyledProperty<string?> PlayerNameProperty = AvaloniaProperty.Register<MenuPartyPlayerControl, string?>("PlayerName", "");
    public static readonly StyledProperty<string?> PlayercardImageProperty = AvaloniaProperty.Register<MenuPartyPlayerControl, string?>("PlayercardImage", "");
    public static readonly StyledProperty<string?> PlayerTitleProperty = AvaloniaProperty.Register<MenuPartyPlayerControl, string?>("PlayerTitle", "");
    public static readonly StyledProperty<string?> PlayerRankIconProperty = AvaloniaProperty.Register<MenuPartyPlayerControl, string?>("PlayerRankIcon");
    public static readonly StyledProperty<Bitmap?> PlayerReputationLevelProperty = AvaloniaProperty.Register<MenuPartyPlayerControl, Bitmap?>("PlayerReputationLevel");
    public static readonly StyledProperty<List<AdvancedImage>?> BadgeObjectsProperty = AvaloniaProperty.Register<MenuPartyPlayerControl, List<AdvancedImage>?>("BadgeObjects");
    public static readonly StyledProperty<string?> LevelTextProperty = AvaloniaProperty.Register<MenuPartyPlayerControl, string?>("LevelText");

    public string? PlayerName
    {
        get { return (string?)GetValue(PlayerNameProperty); }
        set { SetValue(PlayerNameProperty, value); }
    }

    public string? PlayercardImage
    {
        get { return (string?)GetValue(PlayercardImageProperty); }
        set { SetValue(PlayercardImageProperty, value); }
    }

    public string? PlayerTitle
    {
        get { return (string?)GetValue(PlayerTitleProperty); }
        set { SetValue(PlayerTitleProperty, value); }
    }

    public string? PlayerRankIcon
    {
        get { return (string?)GetValue(PlayerRankIconProperty); }
        set { SetValue(PlayerRankIconProperty, value); }
    }

    public Bitmap? PlayerReputationLevel
    {
        get { return (Bitmap?)GetValue(PlayerReputationLevelProperty); }
        set { SetValue(PlayerReputationLevelProperty, value); }
    }

    public List<AdvancedImage>? BadgeObjects
    {
        get { return (List<AdvancedImage>?)GetValue(BadgeObjectsProperty); }
        set { SetValue(BadgeObjectsProperty, value); }
    }

    public string? LevelText
    {
        get { return (string?)GetValue(LevelTextProperty); }
        set { SetValue(LevelTextProperty, value); }
    }
}