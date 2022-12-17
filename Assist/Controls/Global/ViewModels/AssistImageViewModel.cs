using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Assist.ViewModels;
using Avalonia.Media.Imaging;
using ReactiveUI;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Image = System.Drawing.Image;

namespace Assist.Controls.Global.ViewModels
{
    internal class AssistImageViewModel : ViewModelBase
    {

        private Bitmap? _imageBitmap;

        public Bitmap? ImageBitmap
        {
            get => _imageBitmap;
            set => this.RaiseAndSetIfChanged(ref _imageBitmap, value);
        }

        private string? _sourceUrl = "https://cdn.assistapp.dev/Skin/Chroma/9667983e-4c8c-e5b2-68d7-be84f9b3d46c_DisplayIcon.png";

        public string? SourceUrl
        {
            get => _sourceUrl;
            set
            {
                this.RaiseAndSetIfChanged(ref _sourceUrl, value);
            }
        }

        private IBrush? _backgroundBrush;
        public IBrush? BackgroundBrush { 
            get => _backgroundBrush;
            set => this.RaiseAndSetIfChanged(ref _backgroundBrush, value);
        }

        private CornerRadius _cornerRadius;
        public CornerRadius CornerRadius    
        {
            get => _cornerRadius;
            set => this.RaiseAndSetIfChanged(ref _cornerRadius, value);
        }
        public async Task LoadImage(string url)
        {
            try
            {
                await using (var imageStream = await LoadImageBitmapAsync(url))
                {
                    ImageBitmap = await Task.Run(() => new Bitmap(imageStream));
                }
            }
            catch
            {

            }
            
        }


        public async Task<Stream> LoadImageBitmapAsync(string url)
        {
            var data = await new HttpClient().GetByteArrayAsync(url);

            return new MemoryStream(data);
        }
    }
}
