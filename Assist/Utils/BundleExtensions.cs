using System.Linq;

using ValNet.Objects.Store;

namespace Assist.Utils;

public static class BundleExtensions
{

    public static string GetFormattedPrice(this Bundle bundle)
    {
        var price = bundle.Items.Sum(x => x.DiscountedPrice);
        return $"{price:n0}";
    }

}
