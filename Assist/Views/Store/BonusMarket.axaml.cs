using System;
using Assist.Controls.Dashboard;
using Assist.Views.Store.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Views.Store
{
    public partial class BonusMarket : UserControl
    {
        private BonusMarketViewModel _viewModel;
        public BonusMarket()
        {
            DataContext = _viewModel = new BonusMarketViewModel();
            InitializeComponent();
        }

        private async void NightItems_Init(object? sender, EventArgs e)
        {
            if (Design.IsDesignMode)
                return;

            var obj = sender as UniformGrid;
            if (obj != null)
            {
                var r = await _viewModel.GetNightMarket();
                if (r is null)
                    return;

                var controls = await _viewModel.CreateMarketControls(r);

                obj.Children.AddRange(controls);
            }

        }
    }
}
