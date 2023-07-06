using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Dashboard;

public class FeaturedArticleControlV2 : TemplatedControl
{
    public static readonly StyledProperty<string?> ArticleImageProperty = AvaloniaProperty.Register<FeaturedArticleControlV2, string?>("ArticleImage");
    public static readonly StyledProperty<string?> ArticleNameProperty = AvaloniaProperty.Register<FeaturedArticleControlV2, string?>("ArticleName");
    public static readonly StyledProperty<string?> ArticleCategoryProperty = AvaloniaProperty.Register<FeaturedArticleControlV2, string?>("ArticleCategory");

    public string? ArticleImage
    {
        get { return (string?)GetValue(ArticleImageProperty); }
        set { SetValue(ArticleImageProperty, value); }
    }

    public string? ArticleName
    {
        get { return (string?)GetValue(ArticleNameProperty); }
        set { SetValue(ArticleNameProperty, value.ToUpper()); }
    }

    public string? ArticleCategory
    {
        get { return (string?)GetValue(ArticleCategoryProperty); }
        set { SetValue(ArticleCategoryProperty, value); }
    }
}