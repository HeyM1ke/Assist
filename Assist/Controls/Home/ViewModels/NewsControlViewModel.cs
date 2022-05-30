using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Assist.MVVM.Model;
using Assist.MVVM.ViewModel;

namespace Assist.Controls.Home.ViewModels
{
    internal class NewsControlViewModel : ViewModelBase
    {
        private string _newsTitle;

        public string NewsTitle
        {
            get => _newsTitle;
            set => SetProperty(ref _newsTitle, value);
        }
        private string _newsDescription;

        public string NewsDescription
        {
            get => _newsDescription;
            set => SetProperty(ref _newsDescription, value);
        }

        private BitmapImage _newsImage;
        public BitmapImage NewsImage
        {
            get => _newsImage;
            set => SetProperty(ref _newsImage, value);
        }

        private string _newsUrl;

        public string NewsUrl
        {
            get => _newsUrl;
            set => SetProperty(ref _newsUrl, value);
        }





        public async Task OpenNewsUrl()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = NewsUrl,
                UseShellExecute = true
            });
        }

        public void LoadNews(AssistNewsObj data)
        {
            NewsTitle = data.NewsTitle;
            NewsDescription = data.NewsDescription;
            NewsImage = App.LoadImageUrl(data.NewsImage, 185, 95);
            NewsUrl = data.NewsUrl;
        }
    }
}
