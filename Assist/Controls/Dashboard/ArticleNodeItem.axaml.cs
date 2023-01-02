using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Dashboard
{
    public class ArticleNodeItem : TemplatedControl
    {
        public static readonly StyledProperty<string?> ArticleTitleProperty = AvaloniaProperty.Register<ArticleNodeItem, string?>("ArticleTitle");
        public static readonly StyledProperty<string?> ArticleDescriptionProperty = AvaloniaProperty.Register<ArticleNodeItem, string?>("ArticleDescription");
        public static readonly StyledProperty<string?> UrlProperty = AvaloniaProperty.Register<ArticleNodeItem, string?>("Url");
        public static readonly StyledProperty<string?> ImageUrlProperty = AvaloniaProperty.Register<ArticleNodeItem, string?>("ImageUrl");

        public string? ArticleTitle
        {
            get { return (string?)GetValue(ArticleTitleProperty); }
            set { SetValue(ArticleTitleProperty, value); }
        }

        public string? ArticleDescription
        {
            get { return (string?)GetValue(ArticleDescriptionProperty); }
            set { SetValue(ArticleDescriptionProperty, value); }
        }

        public string? Url
        {
            get { return (string?)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        public string? ImageUrl
        {
            get { return (string?)GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }
    }
}
