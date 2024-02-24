using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Dashboard;

public class FeaturedArticleControl : Button
{
    public static readonly StyledProperty<string?> ArticleImageProperty = AvaloniaProperty.Register<FeaturedArticleControl, string?>("ArticleImage");
    public static readonly StyledProperty<string?> ArticleTitleProperty = AvaloniaProperty.Register<FeaturedArticleControl, string?>("ArticleTitle");

    public string? ArticleImage
    {
        get { return (string?)GetValue(ArticleImageProperty); }
        set { SetValue(ArticleImageProperty, value); }
    }

    public string? ArticleTitle
    {
        get { return (string?)GetValue(ArticleTitleProperty); }
        set { SetValue(ArticleTitleProperty, value); }
    }
}