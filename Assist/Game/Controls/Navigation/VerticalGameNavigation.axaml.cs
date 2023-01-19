using System.Collections.Generic;
using Assist.Game.Services;
using Assist.Game.Views.Live;
using Assist.Game.Views.Live.Pages;
using Assist.Game.Views.Modules;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Assist.Game.Controls.Navigation
{
    public partial class VerticalGameNavigation : UserControl
    {
        
        public static VerticalGameNavigation Instance;
        public List<NavButton> NavigationButtons = new List<NavButton>();
        public VerticalGameNavigation()
        {
            Instance = this;
            InitializeComponent();
            NavigationButtons.Add(this.FindControl<NavButton>("DashboardBtn"));
            NavigationButtons.Add(this.FindControl<NavButton>("LiveBtn"));
            NavigationButtons.Add(this.FindControl<NavButton>("ModulesBtn"));
            NavigationButtons.Add(this.FindControl<NavButton>("LobbiesBtn"));
            NavigationButtons[0].IsSelected = true;
        }

        private void DashboardBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            
        }
        private void LiveBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            ClearSelected();

            if (GameViewNavigationController.CurrentPage != Services.Page.LIVE)
                GameViewNavigationController.Change(new LiveView());

            (sender as NavButton).IsSelected = true;
        }
        private void ModulesBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            ClearSelected();

            if (GameViewNavigationController.CurrentPage != Services.Page.MODULES)
                GameViewNavigationController.Change(new ModulesView());

            (sender as NavButton).IsSelected = true;
        }
        private void LobbiesBtn_OnClick(object? sender, RoutedEventArgs e)
        {
            ClearSelected();

            if (GameViewNavigationController.CurrentPage != Services.Page.MODULES)
                GameViewNavigationController.Change(new UnkownPageView());

            (sender as NavButton).IsSelected = true;
        }
        private void ClearSelected()
        {
            NavigationButtons.ForEach(btn => btn.IsSelected = false);
        }


        private void SupportBtn_Click(object? sender, RoutedEventArgs e)
        {
            
        }
    }
}
