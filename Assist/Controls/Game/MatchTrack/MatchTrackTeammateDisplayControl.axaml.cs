using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Game.MatchTrack;

public class MatchTrackTeammateDisplayControl : TemplatedControl
{
    public static readonly StyledProperty<string?> TeammateNameProperty = AvaloniaProperty.Register<MatchTrackTeammateDisplayControl, string?>("TeammateName", "User#NA1");
    public static readonly StyledProperty<string?> AgentIconProperty = AvaloniaProperty.Register<MatchTrackTeammateDisplayControl, string?>("AgentIcon");
    public static readonly StyledProperty<string?> RankIconProperty = AvaloniaProperty.Register<MatchTrackTeammateDisplayControl, string?>("RankIcon");
    public static readonly StyledProperty<string?> StatlineProperty = AvaloniaProperty.Register<MatchTrackTeammateDisplayControl, string?>("Statline", "00 // 00 // 00");

    public string? TeammateName
    {
        get { return (string?)GetValue(TeammateNameProperty); }
        set { SetValue(TeammateNameProperty, value); }
    }

    public string? AgentIcon
    {
        get { return (string?)GetValue(AgentIconProperty); }
        set { SetValue(AgentIconProperty, value); }
    }

    public string? RankIcon
    {
        get { return (string?)GetValue(RankIconProperty); }
        set { SetValue(RankIconProperty, value); }
    }

    public string? Statline
    {
        get { return (string?)GetValue(StatlineProperty); }
        set { SetValue(StatlineProperty, value); }
    }
}