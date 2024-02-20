using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Assist.Controls.Dashboard;
using AssistUser.Lib.Config.Models;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.IdentityModel.Tokens;

namespace Assist.ViewModels.Dashboard;

public partial class ArticleContainerViewModel : ViewModelBase
{
    private List<AssistArticleNewsNode> _articles = new List<AssistArticleNewsNode>();
    
    [ObservableProperty]
    private ObservableCollection<ArticleContainerButton> _articleButtons =
        new ObservableCollection<ArticleContainerButton>();
    
    [ObservableProperty]
    private ObservableCollection<FeaturedArticleControl> _featuredArticlesControls =
        new ObservableCollection<FeaturedArticleControl>();

    [ObservableProperty] private Control _featuredArticle;
    
    [ObservableProperty] private string _articleTitle;
    
    [ObservableProperty] private string _articleImage;
    [ObservableProperty] private string _articleTag;
    [ObservableProperty] private string _articleDescription;
    public async void Setup()
    {
        // Get Articles
        await GetArticles();

        // Create Button for Articles
        CreateArticleButtons();

        if (!_articles.IsNullOrEmpty())
            SwapArticle(_articles[0].RedirectUrl);
    }

    public void Refresh()
    {
        if (!_articles.IsNullOrEmpty())
        {
            ArticleButtons[0].IsChecked = true;
            SwapArticle(_articles[0].RedirectUrl);
        }
            
    }

    private async Task GetArticles()
    {
        var resp = await AssistApplication.AssistUser.Config.GetAllNewsNodes();

        if (resp.Code != 200)
            return;

        _articles = JsonSerializer.Deserialize<List<AssistArticleNewsNode>>(resp.Data.ToString());
        
    }
    
    
    private async void CreateArticleButtons()
    {
        for (int i = 0; i < _articles.Count; i++)
        {
            ArticleButtons.Add(new ArticleContainerButton()
            {
                Content = $"{i+1}",
                IsChecked = i == 0,
                CommandParameter = _articles[i].RedirectUrl,
                Command = SwapArticleCommand
            });
        }
    }

    [RelayCommand]
    public async void SwapArticle(string data)
    {
        var art = _articles.Find(x => x.RedirectUrl == data);

        if (art is null)
            return;
        var feat = FeaturedArticlesControls.FirstOrDefault(x => x.ArticleTitle == art.Title);

        if (feat is null)
            ChangeFeaturedArticle(art);
        else
            ChangeFeaturedArticle(feat);
    }

    [RelayCommand]
    public void OpenArticle(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    private void ChangeFeaturedArticle(AssistArticleNewsNode data)
    {
        var d = new FeaturedArticleControl()
        {
            ArticleImage = data.ImageUrl,
            ArticleTitle = data.Title,
            Command = OpenArticleCommand,
            CommandParameter = data.RedirectUrl
        };
        
        FeaturedArticlesControls.Add(d);

        FeaturedArticle = d;
    }
    
    private void ChangeFeaturedArticle(Control data)
    {
        FeaturedArticle = null;
        FeaturedArticle = data;
    }
}