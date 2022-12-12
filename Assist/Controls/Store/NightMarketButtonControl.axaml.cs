using Assist.Services;
using Assist.Views.Store;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Assist.Controls.Store
{
    public class NightMarketButtonControl : TemplatedControl
    {
        public NightMarketButtonControl()
        {
            AddHandler(PointerPressedEvent, delegate(object? sender, PointerPressedEventArgs args)
            {
                MainViewNavigationController.Change(new BonusMarket());
            });
        }
    }
}
