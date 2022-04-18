using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Assist.MVVM.View.Authentication.ViewModels;
using Microsoft.Web.WebView2.Core;

namespace Assist.MVVM.View.Authentication.AuthenticationPages
{
    /// <summary>
    /// Interaction logic for RitoAuthentication.xaml
    /// </summary>
    public partial class RitoAuthentication : Page
    {
        private readonly RitoAuthViewModel _viewModel;
        private const string authUrl = "https://auth.riotgames.com/authorize?redirect_uri=https%3A%2F%2Fplayvalorant.com%2Fopt_in&client_id=play-valorant-web-prod&response_type=token%20id_token&nonce=1";
        private string cacheLoc = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Assist", "Cache");
        public RitoAuthentication()
        {
            DataContext = _viewModel = new RitoAuthViewModel();
            Directory.CreateDirectory(cacheLoc);
            InitializeComponent();
        }

        private void BackBTN_Click(object sender, RoutedEventArgs e)
        {
            Authentication.ContentFrame.GoBack();
        }

        private void SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
        {
            var redirectUrl = WebView.Source.ToString();

            if (redirectUrl.Contains("https://playvalorant.com/opt_in/#"))
                _viewModel.GetCookies(this.WebView);
        }

        void EnsureHttps(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
            String uri = args.Uri;
            if (!uri.StartsWith("https://"))
            {
                args.Cancel = true;
            }
        }

        private async void RitoAuth_Initialized(object sender, EventArgs e)
        {
            var webView2Environment = await CoreWebView2Environment.CreateAsync(null, cacheLoc);
            await WebView.EnsureCoreWebView2Async(webView2Environment);
            WebView.NavigationStarting += EnsureHttps;
            WebView.Source = new Uri(authUrl);
            WebView.SourceChanged += SourceChanged;
        }
    }
}
