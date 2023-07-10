using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Assist.Services;
using Assist.ViewModels;
using Assist.Views.Dashboard.ViewModels;
using AssistUser.Lib.Config.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Serilog;

namespace Assist.Views.Dashboard;

public partial class DashboardViewV2 : UserControl
{
    private readonly DashboardViewModel _viewModel;

    public DashboardViewV2()
    {
        DataContext = _viewModel = new DashboardViewModel();
        InitializeComponent();
        MainViewNavigationController.CurrentPage = Page.DASHBOARD;
        LoadArticles();
    }

    private async void LoadArticles()
    {
        // Testing

        var t = await AssistApplication.Current.AssistUser.Config.GetAllNewsNodes();

        try
        {
            var data = JsonSerializer.Deserialize<List<AssistArticleNewsNode>>(t.Data.ToString());

            if (data.Count > 0)
            {
                if (data.ElementAtOrDefault(0) != null)
                {
                    FeaturedArticle.ArticleName = data[0].Title;
                    FeaturedArticle.ArticleImage = data[0].ImageUrl;
                    FeaturedArticle.ArticleCategory = data[0].Tag;
                }
                else
                    SetupFailedArticles();

                    if (data.ElementAtOrDefault(1) != null)
                {
                    SmallArticle1.ArticleName = data[1].Title;
                    SmallArticle1.ArticleImage = data[1].ImageUrl;
                    SmallArticle1.ArticleCategory = data[1].Tag;
                }
                else
                    SmallArticle1.IsVisible = false;
                
                if (data.ElementAtOrDefault(2) != null)
                {
                    SmallArticle2.ArticleName = data[2].Title;
                    SmallArticle2.ArticleImage = data[2].ImageUrl;
                    SmallArticle2.ArticleCategory = data[2].Tag;
                }
                else
                    SmallArticle2.IsVisible = false;
                }
            else
            {
                SetupFailedArticles();
            }
        }
        catch (Exception exception)
        {
            Log.Error("Failed to get News Nodes");
            SetupFailedArticles();
            return;
        }
    }

    private void SetupFailedArticles()
    {
        FeaturedArticle.ArticleName = "No News Today.";
        FeaturedArticle.ArticleImage = "https://pbs.twimg.com/media/FzDqAKZacAA1Y8Z?format=png";
        FeaturedArticle.ArticleCategory = "???";

        SmallArticle1.IsVisible = false;
        SmallArticle2.IsVisible = false;
    }
    
}