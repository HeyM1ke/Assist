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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Assist.Controls
{
    /// <summary>
    /// Interaction logic for AssistNewsSlide.xaml
    /// </summary>
    public partial class AssistNewsSlide : UserControl
    {
        private string imageUrl;

        public AssistNewsSlide(string url)
        {
            imageUrl = url;
            InitializeComponent();
            SetupImage();

        }

        private void SetupImage()
        {
            // Allows the image to be loaded with the resolution it is intended to be used for.
            // Because the program is a solo resolution that doesnt change res, this is fine.

            var image = new BitmapImage();
            image.BeginInit();
            image.DecodePixelWidth = 800;
            image.DecodePixelHeight = 400;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(imageUrl, UriKind.Absolute);
            image.EndInit();

            imageContainer.Source = image;
        }


        public string newsTitle
        {
            get { return (string)GetValue(newsTitleProperty); }
            set { SetValue(newsTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for newsTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty newsTitleProperty =
            DependencyProperty.Register("newsTitle", typeof(string), typeof(AssistNewsSlide));



        public string newsDescription
        {
            get { return (string)GetValue(newsDescriptionProperty); }
            set { SetValue(newsDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for newsDescription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty newsDescriptionProperty =
            DependencyProperty.Register("newsDescription", typeof(string), typeof(AssistNewsSlide));



        public string newsUrl
        {
            get { return (string)GetValue(newsUrlProperty); }
            set { SetValue(newsUrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for newsUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty newsUrlProperty =
            DependencyProperty.Register("newsUrl", typeof(string), typeof(AssistNewsSlide));

        private void controlBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", this.newsUrl);

        }
    }
}
