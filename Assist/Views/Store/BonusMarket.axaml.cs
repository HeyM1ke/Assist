using System;
using Assist.Controls.Dashboard;
using Assist.Services;
using Assist.Views.Store.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Assist.Views.Store
{
    public partial class BonusMarket : UserControl
    {
        private StoreViewModel _viewModel;
        public BonusMarket()
        {
            DataContext = _viewModel = new StoreViewModel();
            InitializeComponent();
        }

        private async void NightItems_Init(object? sender, EventArgs e)
        {
            if (Design.IsDesignMode)
                return;

            var obj = sender as UniformGrid;
            if (obj != null)
            {
                var r = await _viewModel.GetPlayerStore();
                if (r is null)
                    return;

                var controls = await _viewModel.CreateMarketControls(r);

                obj.Children.AddRange(controls);
            }

        }

        private void BackBtn_Click(object? sender, RoutedEventArgs e)
        {
            MainViewNavigationController.Change(new StoreView());
        }
    }
}
