using System;
using System.Collections.Generic;
using System.Linq;
using Assist.Controls.Store;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Assist.Services;
using Assist.Views.Store.ViewModels;

namespace Assist.Views.Store
{
    public partial class StoreView : UserControl
    {
        private StoreViewModel _viewModel;

        public StoreView()
        {
            DataContext = _viewModel = new StoreViewModel();
            InitializeComponent();
            MainViewNavigationController.CurrentPage = Page.STORE;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void BundleContainer_Init(object? sender, EventArgs e)
        {
            if (Design.IsDesignMode)
                return;

            var container = sender as BundleContainer;
            if (container != null)
            {
                var r = await _viewModel.GetPlayerStore();

                if (r == null)
                    return;

                var controls = await _viewModel.CreateBundleControls(r);

                container.Bundles = controls;
                container.isLoading = false;
            }
        }
    }
}
