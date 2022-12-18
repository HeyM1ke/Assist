using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Assist.Controls.Store
{
    public class BundleContainer : TemplatedControl
    {
        public static readonly StyledProperty<IEnumerable> BundlesProperty = AvaloniaProperty.Register<BundleContainer, IEnumerable>("Bundles");
        public static readonly StyledProperty<bool?> isLoadingProperty = AvaloniaProperty.Register<BundleContainer, bool?>("isLoading", false);

        public IEnumerable Bundles
        {
            get { return (IEnumerable)GetValue(BundlesProperty); }
            set { SetValue(BundlesProperty, value); }
        }

        public bool? isLoading
        {
            get { return (bool?)GetValue(isLoadingProperty); }
            set { SetValue(isLoadingProperty, value); }
        }
    }
}
