using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Game.Controls.Match;

public class MatchPlayerControl : TemplatedControl
{
    public static readonly StyledProperty<string?> PlayerNameProperty = AvaloniaProperty.Register<MatchPlayerControl, string?>("PlayerName", "Player");
    public static readonly StyledProperty<string?> LeaguePointTextProperty = AvaloniaProperty.Register<MatchPlayerControl, string?>("LeaguePointText", "LP: 0,000");
    public static readonly StyledProperty<string?> ImageUrlProperty = AvaloniaProperty.Register<MatchPlayerControl, string?>("ImageUrl");
    public static readonly StyledProperty<bool?> IsReadyProperty = AvaloniaProperty.Register<MatchPlayerControl, bool?>("IsReady", false);

    public string? PlayerName
    {
        get { return (string?)GetValue(PlayerNameProperty); }
        set { SetValue(PlayerNameProperty, value); }
    }

    public string? LeaguePointText
    {
        get { return (string?)GetValue(LeaguePointTextProperty); }
        set { SetValue(LeaguePointTextProperty, value); }
    }

    public string? ImageUrl
    {
        get { return (string?)GetValue(ImageUrlProperty); }
        set { SetValue(ImageUrlProperty, value); }
    }

    public bool? IsReady
    {
        get { return (bool?)GetValue(IsReadyProperty); }
        set { SetValue(IsReadyProperty, value); }
    }
}