using System;
using Assist.Controls.Store.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ValNet.Objects.Store;

namespace Assist.Controls.Store
{
    public partial class BundleItem : UserControl
    {
        private readonly BundleItemViewModel _viewModel;

        public BundleItem()
        {
            DataContext = _viewModel = new BundleItemViewModel();
            InitializeComponent();
        }

        public BundleItem(Bundle b)
        {
            DataContext = _viewModel = new BundleItemViewModel();
            _viewModel.Bundle = b;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void BundleItem_Initialized(object? sender, EventArgs e)
        {
            if (Design.IsDesignMode)
                return;
            await _viewModel.Setup();
        }
    }
}
