using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Assist;

public class MenuSelectorButton : RadioButton
{
    public static readonly StyledProperty<bool?> IsExternalProperty = AvaloniaProperty.Register<MenuSelectorButton, bool?>("IsExternal", false);

    public bool? IsExternal
    {
        get { return (bool?)GetValue(IsExternalProperty); }
        set { SetValue(IsExternalProperty, value); }
    }
}