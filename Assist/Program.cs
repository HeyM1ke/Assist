using Avalonia;
using System;
using Assist.Shared.Settings;
using Assist.Shared.Settings.Accounts;
using Assist.ViewModels;
using Avalonia.Svg.Skia;
using Serilog;
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
        try
        {
            VelopackApp.Build()
                .WithAfterInstallFastCallback((v) => new Shortcuts().CreateShortcutForThisExe(ShortcutLocation.Desktop))
                .Run();
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Fatal("Fatal Error");
            Log.Fatal(e.Message);
            Log.Fatal("Fatal Error STACK == ");
            Log.Fatal(e.StackTrace);
            Log.CloseAndFlush();
        }
        finally
        {
#if (!DEBUG)
                Log.CloseAndFlush();
                AssistSettings.Save();
                AccountSettings.Save();
                AssistApplication.CurrentApplication.Shutdown();
#endif

        }
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