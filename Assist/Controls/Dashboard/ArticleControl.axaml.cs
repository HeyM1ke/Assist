using System;
using System.Collections.Generic;
using System.Linq;
using Assist.Objects.AssistApi;
using Assist.Services;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Markup.Xaml;
using Serilog;

namespace Assist.Controls.Dashboard
{
    public partial class ArticleControl : UserControl
    {
        private Carousel _carousel;



        public ArticleControl()
        {
            InitializeComponent();
            _carousel = this.FindControl<Carousel>("ArticleCarousel");
        }


        private async void StyledElement_OnInitialized(object? sender, EventArgs e)
        {
            var randomNullArt = new ArticleNodeItem()
            {
                ArticleTitle = "Dead",
                ImageUrl = "https://images.contentstack.io/v3/assets/bltb6530b271fddd0b1/blt41138834252a9cbb/62d73ea33d042036dcb4d48e/1920x1080-KEY-ART-pearl_opt.jpg"
            };

            var articles = await AssistApplication.ApiService.GetNewsAsync();

            if (articles == null || articles.Length == 0)
            {
                var n = new List<ArticleNodeItem>()
                {
                    randomNullArt,
                    new ArticleNodeItem()
                    {
                        ArticleTitle = "node1",
                        ImageUrl = 
                            "https://images.contentstack.io/v3/assets/bltb6530b271fddd0b1/blt41138834252a9cbb/62d73ea33d042036dcb4d48e/1920x1080-KEY-ART-pearl_opt.jpg",
                        Width=747, 
                        Height=300
                    }
                };

                _carousel.ItemsSource = n;
                return;
            }
            
            // what the hell is this FIX LMFAOOO
            List<ArticleNodeItem> AI = articles.Select(x => new ArticleNodeItem(){
                Width=747,
                Height=300,
                ArticleTitle = x.title,
                ArticleDescription = x.description,
                ImageUrl = x.imageUrl,
                Url = x.nodeUrl,
            }).ToList();
            AI.Insert(0, randomNullArt);
            _carousel.ItemsSource = (AI);
            // To Fix Carousell not loading right
            _carousel.Next();
        }

        private void GoBackBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            if(_carousel.SelectedIndex - 1 == 0)
                return;
            
            Log.Information("" + _carousel.SelectedIndex);
            try
            {
                _carousel.Previous();
            }
            catch (Exception exception)
            {
                Log.Error("Error on Back BTN");
            }
        }

        private void GoNextBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            try
            {
                _carousel.Next();
            }
            catch (Exception exception)
            {
                Log.Error("Error on Back BTN");
            }
        }
    }
}
