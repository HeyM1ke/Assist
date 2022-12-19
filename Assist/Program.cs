using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using System;
using Assist.Settings;
using Assist.ViewModels;
using Serilog;

namespace Assist
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception e)
            {
                Log.Fatal("Fatal Error");
                Log.Fatal(e.Message);
                Log.Fatal("Fatal Error STACK == ");
                Log.Fatal(e.StackTrace);
                
            }
            finally
            {
                Log.CloseAndFlush();
                AssistSettings.Save();
                AssistApplication.CurrentApplication.Shutdown();
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect().With(new Win32PlatformOptions
                {
                    AllowEglInitialization = false
                })
                .UseReactiveUI();

    }
}
