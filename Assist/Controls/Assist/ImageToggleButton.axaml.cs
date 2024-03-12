using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Assist.Controls.Assist;

public class ImageToggleButton : RadioButton
{
    public static readonly StyledProperty<IImage?> ImageIconProperty = AvaloniaProperty.Register<ImageToggleButton, IImage?>("ImageIcon");

    public IImage? ImageIcon
    {
        get { return (IImage?)GetValue(ImageIconProperty); }
        set { SetValue(ImageIconProperty, value); }
    }
}