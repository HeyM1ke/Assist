using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Assist.Controls.Dashboard;

public class RankPreviewControl : TemplatedControl
{
    public static readonly StyledProperty<string> PlayerRankIconProperty = AvaloniaProperty.Register<PlayerStatisticsView, string>("PlayerRankIcon", "https://cdn.assistapp.dev/Ranks/0.png");
    public static readonly StyledProperty<string?> RankNameProperty = AvaloniaProperty.Register<PlayerStatisticsView, string?>("RankName", "UNRANKED");
    public static readonly StyledProperty<string?> PlayerRRProperty = AvaloniaProperty.Register<PlayerStatisticsView, string?>("PlayerRR", "");
         static readonly StyledProperty<string?> SeasonWinsProperty = AvaloniaProperty.Register<PlayerStatisticsView, string?>("SeasonWins", "");
    public static readonly StyledProperty<bool?> isLoadingProperty = AvaloniaProperty.Register<PlayerStatisticsView, bool?>("isLoading", false);
    public static readonly StyledProperty<IBrush?> RankColorTextProperty = AvaloniaProperty.Register<RankPreviewControl, IBrush?>("RankColorText");

    public string PlayerRankIcon
    { 
        get { return (string)GetValue(PlayerRankIconProperty); } 
        set { SetValue(PlayerRankIconProperty, value); }
    }
    
    public string? RankName
    {
        get { return (string?)GetValue(RankNameProperty); }
        set { SetValue(RankNameProperty, value); }
    }

    public string? PlayerRR
    {
        get { return (string?)GetValue(PlayerRRProperty); }
        set { SetValue(PlayerRRProperty, value); }
    }

    public string? SeasonWins
    {
        get { return (string?)GetValue(SeasonWinsProperty); }
        set { SetValue(SeasonWinsProperty, value); }
    }

    public bool? isLoading
    {
        get { return (bool?)GetValue(isLoadingProperty); }
        set { SetValue(isLoadingProperty, value); }
    }

    public IBrush? RankColorText
    {
        get { return (IBrush?)GetValue(RankColorTextProperty); }
        set { SetValue(RankColorTextProperty, value); }
    }
}