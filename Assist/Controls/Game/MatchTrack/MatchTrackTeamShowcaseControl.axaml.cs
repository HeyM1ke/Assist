using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Assist.Controls.Game.MatchTrack;

public class MatchTrackTeamShowcaseControl : TemplatedControl
{
    public string TeamId { get; set; }
    public static readonly StyledProperty<string?> TeamNameProperty = AvaloniaProperty.Register<MatchTrackTeamShowcaseControl, string?>("TeamName");
    public static readonly StyledProperty<IBrush?> TeamColorProperty = AvaloniaProperty.Register<MatchTrackTeamShowcaseControl, IBrush?>("TeamColor");
    public static readonly StyledProperty<ObservableCollection<MatchTrackTeammateDisplayControl>> TeammateControlsProperty = AvaloniaProperty.Register<MatchTrackTeamShowcaseControl, ObservableCollection<MatchTrackTeammateDisplayControl>>("TeammateControls");

    public string? TeamName
    {
        get { return (string?)GetValue(TeamNameProperty); }
        set { SetValue(TeamNameProperty, value); }
    }

    public IBrush? TeamColor
    {
        get { return (IBrush?)GetValue(TeamColorProperty); }
        set { SetValue(TeamColorProperty, value); }
    }

    public ObservableCollection<MatchTrackTeammateDisplayControl> TeammateControls
    {
        get { return (ObservableCollection<MatchTrackTeammateDisplayControl>)GetValue(TeammateControlsProperty); }
        set { SetValue(TeammateControlsProperty, value); }
    }
}