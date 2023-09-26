using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Assist.Game.Controls.Live;

public class MatchReportTeamDisplayControl : TemplatedControl
{
    public string TeamId { get; set; }
    public static readonly StyledProperty<string?> TeamNameProperty = AvaloniaProperty.Register<MatchReportTeamDisplayControl, string?>("TeamName", "Team");
    public static readonly StyledProperty< IBrush?> TeamColorProperty = AvaloniaProperty.Register<MatchReportTeamDisplayControl,  IBrush?>("TeamColor", new SolidColorBrush(new Color(255, 222,227,232)));
    public static readonly StyledProperty<ObservableCollection<MatchReportTeammateDisplayControl>> TeammateControlsProperty = AvaloniaProperty.Register<MatchReportTeamDisplayControl, ObservableCollection<MatchReportTeammateDisplayControl>>("TeammateControls");

    public string? TeamName
    {
        get { return (string?)GetValue(TeamNameProperty); }
        set { SetValue(TeamNameProperty, value); }
    }

    public  IBrush? TeamColor
    {
        get { return ( IBrush?)GetValue(TeamColorProperty); }
        set { SetValue(TeamColorProperty, value); }
    }

    public ObservableCollection<MatchReportTeammateDisplayControl> TeammateControls
    {
        get { return (ObservableCollection<MatchReportTeammateDisplayControl>)GetValue(TeammateControlsProperty); }
        set { SetValue(TeammateControlsProperty, value); }
    }
}