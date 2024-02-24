using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Views.Store.Pages;

public class MainStorePageView : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable?> BundleControlsProperty = AvaloniaProperty.Register<MainStorePageView, IEnumerable?>("BundleControls");
    public static readonly StyledProperty<IEnumerable?> OfferControlsProperty = AvaloniaProperty.Register<MainStorePageView, IEnumerable?>("OfferControls");

    public IEnumerable? BundleControls
    {
        get { return (IEnumerable?)GetValue(BundleControlsProperty); }
        set { SetValue(BundleControlsProperty, value); }
    }

    public IEnumerable? OfferControls
    {
        get { return (IEnumerable?)GetValue(OfferControlsProperty); }
        set { SetValue(OfferControlsProperty, value); }
    }
}