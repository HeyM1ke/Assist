using System;
using Assist.Controls.Dashboard.ViewModels;
using Assist.Objects.AssistApi;
using AsyncImageLoader;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Dashboard
{
    public partial class ArticleItem : UserControl
    {
        private readonly ArticleItemViewModel _viewModel;

        public ArticleItem()
        {
            DataContext = _viewModel = new ArticleItemViewModel();
            InitializeComponent();
        }

        internal ArticleItem(NewsArticle node)
        {
            DataContext = _viewModel = new ArticleItemViewModel();

            _viewModel.ArticleTitle = node.Title;
            _viewModel.ArticleDescription = node.Description;
            _viewModel.ArticleUrl = node.Url;
            _viewModel.ArticleImageUrl = node.ImageUrl;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void ArticleItem_Click(object? sender, RoutedEventArgs e)
        {
            await _viewModel.OpenNewsUrl();
        }

        private void StyledElement_OnInitialized(object? sender, EventArgs e)
        {
        }
    }
}
