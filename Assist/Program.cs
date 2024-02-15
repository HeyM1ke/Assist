using Avalonia;
using Avalonia.ReactiveUI;
using System;
using Avalonia.Svg.Skia;
using Velopack;
using Velopack.Windows;


namespace Assist;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        VelopackApp.Build().WithAfterInstallFastCallback((v) => new Shortcuts().CreateShortcutForThisExe(ShortcutLocation.Desktop))
            .Run();
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    
    public static AppBuilder BuildAvaloniaApp()
    {
        GC.KeepAlive(typeof(SvgImageExtension).Assembly); // Preview Code
        GC.KeepAlive(typeof(Avalonia.Svg.Skia.Svg).Assembly); // Preview Code
        
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseSkia().With(new SkiaOptions { UseOpacitySaveLayer = true })
            .LogToTrace();
    }
}