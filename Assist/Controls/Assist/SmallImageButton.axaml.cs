using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Assist.Controls.Assist;

public class SmallImageButton : Button
{
    public static readonly StyledProperty<IImage?> IconProperty = AvaloniaProperty.Register<SmallImageButton, IImage?>("Icon");
    
    public IImage? Icon
    {
        get { return (IImage?)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }
}