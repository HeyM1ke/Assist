using System;
using Assist.Controls.Progression.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Assist.Controls.Progression
{
    public partial class BattlepassConcurrentPreview : UserControl
    {
        private readonly BPConcurrentPreviewViewModel _viewModel;

        public BattlepassConcurrentPreview()
        {
            DataContext = _viewModel = new BPConcurrentPreviewViewModel();
            InitializeComponent();
        }



        private async void ConcurrentControl_Initialized(object? sender, EventArgs e)
        {
            await _viewModel.GetBattlepassData();
        }
    }
}
