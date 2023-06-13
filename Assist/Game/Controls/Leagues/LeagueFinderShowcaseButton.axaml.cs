using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Game.Controls.Leagues;

public class LeagueFinderShowcaseButton : Button
{
    public static readonly StyledProperty<string?> LeagueImageProperty = AvaloniaProperty.Register<LeagueFinderShowcaseButton, string?>("LeagueImage");

    public string? LeagueImage
    {
        get { return (string?)GetValue(LeagueImageProperty); }
        set { SetValue(LeagueImageProperty, value); }
    }
}