using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Game.Controls.Live;

public class MatchReportTeammateDisplayControl : TemplatedControl
{
    public static readonly StyledProperty<string?> TeammateNameProperty = AvaloniaProperty.Register<MatchReportTeammateDisplayControl, string?>("TeammateName", "User#NA1");
    public static readonly StyledProperty<string?> AgentIconProperty = AvaloniaProperty.Register<MatchReportTeammateDisplayControl, string?>("AgentIcon");
    public static readonly StyledProperty<string?> RankIconProperty = AvaloniaProperty.Register<MatchReportTeammateDisplayControl, string?>("RankIcon");
    public static readonly StyledProperty<string?> StatlineProperty = AvaloniaProperty.Register<MatchReportTeammateDisplayControl, string?>("Statline", "00 // 00 // 00");

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