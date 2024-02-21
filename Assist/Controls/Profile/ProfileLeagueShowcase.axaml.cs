using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Profile;

public class ProfileLeagueShowcase : TemplatedControl
{
    public static readonly StyledProperty<string?> LeagueNameProperty = AvaloniaProperty.Register<ProfileLeagueShowcase, string?>("LeagueName");
    public static readonly StyledProperty<string?> LeagueStatTextProperty = AvaloniaProperty.Register<ProfileLeagueShowcase, string?>("LeagueStatText");

    public string? LeagueName
    {
        get { return (string?)GetValue(LeagueNameProperty); }
        set { SetValue(LeagueNameProperty, value); }
    }

    public string? LeagueStatText
    {
        get { return (string?)GetValue(LeagueStatTextProperty); }
        set { SetValue(LeagueStatTextProperty, value); }
    }
}