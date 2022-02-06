using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Assist.MVVM.ViewModel;
using System.Net;
using System.IO;

namespace Assist.MVVM.View.LoginPage.Pages
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private const string authUrl = "https://auth.riotgames.com/authorize?redirect_uri=https%3A%2F%2Fplayvalorant.com%2Fopt_in&client_id=play-valorant-web-prod&response_type=token%20id_token&nonce=1";
        private string cacheLoc = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Assist", "Cache");
        public LoginWindow()
        {
            Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Assist", "Cache"));
            InitializeComponent();
            
        }

        private void SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
        {
            var redirectUrl = webView.Source.ToString();

            if (redirectUrl.Contains("https://playvalorant.com/opt_in/#"))
                GetCookies(redirectUrl);
        }

        void EnsureHttps(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
            String uri = args.Uri;
            if (!uri.StartsWith("https://"))
            {
                args.Cancel = true;
            }
        }

        private async void GetCookies(string url) {
            var result = await webView.CoreWebView2.CookieManager.GetCookiesAsync(null);
            foreach (var cookie in result)
            {
                //AssistApplication.AppInstance.Log.Debug(cookie.Name);
                if(cookie.Name.Equals("ssid"))
                {
                    AssistApplication.AppInstance.Log.Debug(cookie.Expires.ToString());

                    var cook = new Cookie
                    {
                        Name = cookie.Name,
                        Value = cookie.Value,
                        Path = cookie.Path,
                        Secure = cookie.IsSecure,
                        HttpOnly = cookie.IsHttpOnly,
                        Domain = cookie.Domain,
                    };

                    if (cookie.Expires.ToString().Contains("1/1/0001"))
                        cook.Expires = DateTime.Now.AddMonths(1);
                    else
                        cook.Expires = cookie.Expires;

                    AssistApplication.AppInstance.LoginPageViewModel.cookie_Ssid = cook;

                    webView.CoreWebView2.CookieManager.DeleteAllCookies();
                    AssistApplication.AppInstance.LoginPageViewModel.redirectUrl = url;
                    this.Close();
                }
            }

            
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            var webView2Environment = await CoreWebView2Environment.CreateAsync(null, cacheLoc);
            await webView.EnsureCoreWebView2Async(webView2Environment);
            webView.NavigationStarting += EnsureHttps;
            webView.Source = new Uri(authUrl);
            webView.SourceChanged += SourceChanged;
        }
    }
}
