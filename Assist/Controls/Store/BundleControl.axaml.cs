using System;
using Assist.Controls.Store.ViewModels;
using Assist.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Store
{
    public partial class BundleControl : UserControl
    {
        private readonly BundleControlViewModel _viewModel;
        private readonly Carousel _carousel;

        public BundleControl()
        {
            DataContext = _viewModel = new BundleControlViewModel();
            
            InitializeComponent();

            _carousel = this.FindControl<Carousel>("BundleCarousel");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void BundleControl_Initialized(object? sender, EventArgs e)
        {
            
            if (Design.IsDesignMode)
                return;
            await AssistApplication.Current.CurrentUser.Store.GetPlayerStore();

            await _viewModel.Setup();
        }
    }
}
