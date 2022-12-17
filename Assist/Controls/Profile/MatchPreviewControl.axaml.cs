using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Assist.Controls.Profile
{
    public class MatchPreviewControl : TemplatedControl
    {
        public static readonly StyledProperty<IBrush?> ResultColorProperty = AvaloniaProperty.Register<MatchPreviewControl, IBrush?>("ResultColor");
        public static readonly StyledProperty<string?> ResultTextProperty = AvaloniaProperty.Register<MatchPreviewControl, string?>("ResultText", "WIN");
        public static readonly StyledProperty<string> MatchScoreProperty = AvaloniaProperty.Register<MatchPreviewControl, string>("MatchScore", "11 - 2");
        public static readonly StyledProperty<string?> MatchMapProperty = AvaloniaProperty.Register<MatchPreviewControl, string?>("MatchMap", "ASCENT");
        public static readonly StyledProperty<string?> PlayerAgentProperty = AvaloniaProperty.Register<MatchPreviewControl, string?>("PlayerAgent");
        public static readonly StyledProperty<string?> ExtraDataProperty = AvaloniaProperty.Register<MatchPreviewControl, string?>("ExtraData");
        public static readonly StyledProperty<bool?> MatchWinProperty = AvaloniaProperty.Register<MatchPreviewControl, bool?>("MatchWin", false);
        public static readonly StyledProperty<string?> MatchMapImageProperty = AvaloniaProperty.Register<MatchPreviewControl, string?>("MatchMapImage");

        public IBrush? ResultColor
        {
            get { return (IBrush?)GetValue(ResultColorProperty); }
            set { SetValue(ResultColorProperty, value); }
        }

        public string? ResultText
        {
            get { return (string?)GetValue(ResultTextProperty); }
            set { SetValue(ResultTextProperty, value); }
        }

        public string? MatchScore
        {
            get { return (string)GetValue(MatchScoreProperty); }
            set { SetValue(MatchScoreProperty, value); }
        }

        public string? MatchMap
        {
            get { return (string?)GetValue(MatchMapProperty); }
            set { SetValue(MatchMapProperty, value); }
        }

        public string? PlayerAgent
        {
            get { return (string?)GetValue(PlayerAgentProperty); }
            set { SetValue(PlayerAgentProperty, value); }
        }

        public string? ExtraData
        {
            get { return (string?)GetValue(ExtraDataProperty); }
            set { SetValue(ExtraDataProperty, value); }
        }

        public bool? MatchWin
        {
            get { return (bool?)GetValue(MatchWinProperty); }
            set { SetValue(MatchWinProperty, value); }
        }

        public string? MatchMapImage
        {
            get { return (string?)GetValue(MatchMapImageProperty); }
            set { SetValue(MatchMapImageProperty, value); }
        }
    }
}
