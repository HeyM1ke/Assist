using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assist.ViewModels;
using ReactiveUI;

namespace Assist.Controls.Dashboard.ViewModels
{
    internal class ArticleItemViewModel : ViewModelBase
    {
        private string _articleTitle = "Valorant Patch Notes 5.01";
        public string ArticleTitle
        {
            get => _articleTitle.ToUpper();
            set => this.RaiseAndSetIfChanged(ref _articleTitle, value);
        }

        private string _articleDescription = "Phoenix, KAY/O, and Yoru on on the balance beams and some updated smurf detection.";
        public string ArticleDescription
        {
            get => _articleDescription;
            set => this.RaiseAndSetIfChanged(ref _articleDescription, value);
        }


        private string _articleUrl = "https://playvalorant.com/en-us/news/game-updates/valorant-patch-notes-5-01/";
        public string ArticleUrl
        {
            get => _articleUrl;
            set => this.RaiseAndSetIfChanged(ref _articleUrl, value);
        }

        private string _articleImageUrl = "https://images.contentstack.io/v3/assets/bltb6530b271fddd0b1/bltc14e8bf5f4110a39/635c20756b46ec106a2eb772/patch_509_Banner.jpg?auto=webp&disable=upscale&width=790";
        public string ArticleImageUrl
        {
            get => _articleImageUrl;
            set => this.RaiseAndSetIfChanged(ref _articleImageUrl, value);
        }

        public async Task OpenNewsUrl()
        {

            if(ArticleUrl == null)
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = ArticleUrl,
                UseShellExecute = true
            });
        }
    }
}
