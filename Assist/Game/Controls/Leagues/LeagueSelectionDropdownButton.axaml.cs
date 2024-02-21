using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Game.Controls.Leagues;

public class LeagueSelectionDropdownButton : Button
{
    public static readonly StyledProperty<string?> LeagueNameProperty = AvaloniaProperty.Register<LeagueSelectionDropdownButton, string?>("LeagueName");
    public static readonly StyledProperty<string?> LeagueIdProperty = AvaloniaProperty.Register<LeagueSelectionDropdownButton, string?>("LeagueId");

    public string? LeagueName
    {
        get { return (string?)GetValue(LeagueNameProperty); }
        set { SetValue(LeagueNameProperty, value); }
    }

    public string? LeagueId
    {
        get { return (string?)GetValue(LeagueIdProperty); }
        set { SetValue(LeagueIdProperty, value); }
    }
}