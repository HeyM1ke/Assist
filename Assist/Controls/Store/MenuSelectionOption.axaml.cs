using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Assist.Controls.Store;

public class MenuSelectionOption : RadioButton
{
    public static readonly StyledProperty<IImage?> IconProperty = AvaloniaProperty.Register<MenuSelectionOption, IImage?>("Icon");

    public IImage? Icon
    {
        get { return (IImage?)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }
}