using Assist.Game.Services;
using Assist.Game.ViewModels;
using Assist.Game.Views.Live;
using Assist.Services;
using Assist.Views.Dashboard;
using Avalonia.Controls;

namespace Assist.Game.Views
{
    public partial class GameView : UserControl
    {
        private readonly GameViewViewModel _viewModel;

        public GameView()
        {
            DataContext = _viewModel = new GameViewViewModel();
            InitializeComponent();
            GameViewNavigationController.ContentControl = this.FindControl<TransitioningContentControl>("ContentView");
            GameViewNavigationController.Change(new GDashboard.GDashboard());
        }

        public GameView(UserControl pageToLoad)
        {
            DataContext = _viewModel = new GameViewViewModel();
            InitializeComponent();
            GameViewNavigationController.ContentControl = this.FindControl<TransitioningContentControl>("ContentView");
            GameViewNavigationController.Change(pageToLoad);
        }
    }
}
