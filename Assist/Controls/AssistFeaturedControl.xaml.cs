using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using Assist.MVVM.ViewModel;
namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistFeaturedControl.xaml
    /// </summary>
    public partial class AssistFeaturedControl : UserControl
    {
        AssistFeaturedViewModel _viewmodel;

        int currentIndex = 0;
        private int maxIndex;

        //doing this to save memory instead of generating a new obj every time.
        List<AssistNewsSlide> slides = new List<AssistNewsSlide>();


        public AssistFeaturedControl()
        {
            if (AssistApplication.AppInstance.assistFeaturedViewModel is null)
                DataContext = _viewmodel = AssistApplication.AppInstance.assistFeaturedViewModel = new AssistFeaturedViewModel();
            else
                DataContext = _viewmodel = AssistApplication.AppInstance.assistFeaturedViewModel;
            InitializeComponent();
        }


        private async void _this_Initialized(object sender, EventArgs e)
        {
            await _viewmodel.GetFeaturedNews();
            maxIndex = _viewmodel.newsList.Count;
            await CreateSelectionButtons();
            await CreateSlides();
            NavToSlide(currentIndex);
        }

        private async Task CreateSelectionButtons()
        {
            for (int i = 0; i < maxIndex; i++)
                featuredButtonPanel.Children.Add(new AssistFeaturedButton()
                {
                    Name = "selection" + i,
                    isSelected = Visibility.Hidden
                });
        }

        private async Task CreateSlides()
        {
            for(int i = 0;i < maxIndex; i++)
            {
                var newsObj = _viewmodel.newsList[i];
                var slide = new AssistNewsSlide(newsObj.featureimage)
                {
                    newsTitle = newsObj.title,
                    newsUrl = newsObj.newslink,
                    newsDescription = newsObj.description
                };
                
                slides.Add(slide);
            }
            
        }
        
        private void NavToSlide(int index)
        {
            currentIndex = index;
            newsContainer.Children.Clear();
            newsContainer.Children.Add(slides[currentIndex]);
            SetSelectedButton();
        }

        private void SetSelectedButton()
        {
            foreach (AssistFeaturedButton button in featuredButtonPanel.Children)
            {
                if (button.isSelected == Visibility.Visible)
                    button.isSelected = Visibility.Hidden;

                if (button.Name.Equals("selection" + currentIndex))
                    button.isSelected = Visibility.Visible;

            }
        }

        private void rightSlideBTN_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex < maxIndex-1)
            {
                currentIndex++;
                NavToSlide(currentIndex);
            }
        }
        private void leftSlideBTN_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex != 0)
            {
                currentIndex--;
                NavToSlide(currentIndex);
            }
        }
    }
}
