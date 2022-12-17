using System;
using Assist.Controls.Global.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Assist.Controls.Global
{
    public partial class AssistImage : UserControl
    {
        public static readonly StyledProperty<IBrush> BackgroundProperty =
            AvaloniaProperty.Register<AssistImage, IBrush>(nameof(BackgroundColor));

        public IBrush BackgroundColor
        {
            get { return GetValue(BackgroundProperty); }
            set
            {
                SetValue(BackgroundProperty, value);
                _viewModel.BackgroundBrush = value;
            }
        }

        public static readonly StyledProperty<string?> ImageUrlProperty =
            AvaloniaProperty.Register<AssistImage, string?>(nameof(ImageUrl));

        public string? ImageUrl
        {
            get { return GetValue(ImageUrlProperty); }
            set
            {
                SetValue(ImageUrlProperty, value);
                LoadImage();
            }
        }

        private readonly AssistImageViewModel _viewModel = new AssistImageViewModel();
        public AssistImage()
        {
            DataContext = _viewModel;
            InitializeComponent();
        }

        public async void LoadImage()
        {
            if(string.IsNullOrEmpty(ImageUrl))
                return;
            try
            {
                await _viewModel.LoadImage(ImageUrl);
            }
            catch (Exception e)
            {
                
            }
           
        }
    }
}
