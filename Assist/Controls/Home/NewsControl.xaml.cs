using Assist.Controls.Home.ViewModels;
using Assist.Objects.Valorant;

using System.Windows;
using System.Windows.Controls;

namespace Assist.Controls.Home
{
    /// <summary>
    /// Interaction logic for NewsControl.xaml
    /// </summary>
    public partial class NewsControl
    {

        private readonly NewsArticle _article;
        private NewsControlViewModel _viewModel;

        public NewsControl()
        { 
            Initialize();
        }

        public NewsControl(NewsArticle article)
        {
            _article = article;
            Initialize();
        }

        private void Initialize()
        {
            _viewModel = new NewsControlViewModel();
            DataContext = _viewModel = new NewsControlViewModel();
            InitializeComponent();
        }

        private void NewsControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_article != null) 
                _viewModel.LoadNews(_article);
        }

        private async void NewsControl_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.OpenNewsUrl();
        }

    }
}
