using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;

namespace Assist.Controls.Global.Popup
{
    public class PopupMaster : TemplatedControl
    {
        public static readonly StyledProperty<Avalonia.Controls.Controls> ChildrenProperty = AvaloniaProperty.Register<PopupMaster, Avalonia.Controls.Controls>("Children", new Avalonia.Controls.Controls());

        
        public Avalonia.Controls.Controls Children
        {
            get { return (Avalonia.Controls.Controls)GetValue(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }
    }
}
