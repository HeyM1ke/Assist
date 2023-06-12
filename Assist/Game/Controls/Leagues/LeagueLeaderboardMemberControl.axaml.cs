using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Game.Controls.Leagues;

public class LeagueLeaderboardMemberControl : TemplatedControl
{
    public static readonly StyledProperty<string?> PositionTextProperty = AvaloniaProperty.Register<LeagueLeaderboardMemberControl, string?>("PositionText");
    public static readonly StyledProperty<string?> PlayerTextProperty = AvaloniaProperty.Register<LeagueLeaderboardMemberControl, string?>("PlayerText");
    public static readonly StyledProperty<string?> LeaguePointTextProperty = AvaloniaProperty.Register<LeagueLeaderboardMemberControl, string?>("LeaguePointText");

    public string? PositionText
    {
        get { return (string?)GetValue(PositionTextProperty); }
        set { SetValue(PositionTextProperty, value); }
    }

    public string? PlayerText
    {
        get { return (string?)GetValue(PlayerTextProperty); }
        set { SetValue(PlayerTextProperty, value); }
    }

    public string? LeaguePointText
    {
        get { return (string?)GetValue(LeaguePointTextProperty); }
        set { SetValue(LeaguePointTextProperty, value); }
    }
}