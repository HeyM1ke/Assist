using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Dashboard
{
    public class PlayerStatisticsView : TemplatedControl
    {
        public static readonly StyledProperty<string> FeaturedAgentProperty = AvaloniaProperty.Register<PlayerStatisticsView, string>("FeaturedAgent", "https://content.assistapp.dev/agents/7f94d92c-4234-0a36-9646-3a87eb8b5c89_fullportrait.png");
        public static readonly StyledProperty<string> PlayerRankIconProperty = AvaloniaProperty.Register<PlayerStatisticsView, string>("PlayerRankIcon", "https://cdn.assistapp.dev/Ranks/0.png");
        public static readonly StyledProperty<object?> ContentProperty = AvaloniaProperty.Register<PlayerStatisticsView, object?>("Content");
        public static readonly StyledProperty<string?> RankNameProperty = AvaloniaProperty.Register<PlayerStatisticsView, string?>("RankName", "UNRANKED");
        public static readonly StyledProperty<string?> PlayerRRProperty = AvaloniaProperty.Register<PlayerStatisticsView, string?>("PlayerRR", "");
        public static readonly StyledProperty<string?> SeasonWinsProperty = AvaloniaProperty.Register<PlayerStatisticsView, string?>("SeasonWins", "");
        public static readonly StyledProperty<bool?> isLoadingProperty = AvaloniaProperty.Register<PlayerStatisticsView, bool?>("isLoading", false);

        public string FeaturedAgent
        {
            get { return (string)GetValue(FeaturedAgentProperty); }
            set { SetValue(FeaturedAgentProperty, value); }
        }

        public string PlayerRankIcon
        {
            get { return (string)GetValue(PlayerRankIconProperty); }
            set { SetValue(PlayerRankIconProperty, value); }
        }

        public object? Content
        {
            get { return (object?)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
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
    }
}
