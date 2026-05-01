using Hacked.Maui.Services;
using Hacked.Maui.ViewModels;
using Hacked.Maui.Views;
using Hacked.Services.Apis;
using Hacked.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.LifecycleEvents;
using System.Reflection;
using Telerik.Maui.Controls.Compatibility;

#if WINDOWS10_0_17763_0_OR_GREATER
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
using WinUIEx;
#elif MACCATALYST
using AppKit;
using CoreGraphics;
using Foundation;
using UIKit;
#endif

namespace Hacked.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseTelerik()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Raleway-Regular.ttf", "Raleway");
                fonts.AddFont("telerikfontexamples.ttf", "telerikfontexamples");
                fonts.AddFont("fa-solid-900.ttf", "Font Awesome 6 Free Regular");
            })
            .LoadConfigurations()
            .RegisterServices()
            .RegisterViewModels()
            .RegisterViews()
            .RegisterLifecycleEvents();


        return builder.Build();
    }

    extension(MauiAppBuilder builder)
    {
        public MauiAppBuilder LoadConfigurations()
        {
            // Load config from embedded resources
            var assembly = Assembly.GetExecutingAssembly();

            using var appSettingsStream = assembly.GetManifestResourceStream("Hacked.Maui.appsettings.json");

            if (appSettingsStream != null)
                builder.Configuration.AddJsonStream(appSettingsStream);

            using var devSettingsStream = assembly.GetManifestResourceStream("Hacked.Maui.appsettings.Development.json");

            if (devSettingsStream != null)
                builder.Configuration.AddJsonStream(devSettingsStream);

            return builder;
        }

        public MauiAppBuilder RegisterServices()
        {
            builder.Services.AddSingleton<IPwndBreachService>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var apiKey = config["HibpApiKey"];
#pragma warning disable CsWinRT1030
                return new BeenPwnedService(apiKey);
#pragma warning restore CsWinRT1030
            });
            builder.Services.AddSingleton<IPwndPasswordService, PwnedPasswordService>();
            builder.Services.AddSingleton<IAccountsService, FrontendAccountsService>();

            return builder;
        }

        public MauiAppBuilder RegisterViewModels()
        {
            builder.Services.AddSingleton<MonitoredAccountsViewModel>();
            builder.Services.AddTransient<AccountDetailsViewModel>();
            builder.Services.AddTransient<BreachDetailsViewModel>();
            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<AboutViewModel>();
            return builder;
        }

        public MauiAppBuilder RegisterViews()
        {
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<MonitoredAccountsPage>();
            builder.Services.AddTransient<AccountDetailsPage>();
            builder.Services.AddTransient<BreachDetailsPage>();
            builder.Services.AddSingleton<SettingsPage>();
            builder.Services.AddSingleton<AboutPage>();
            return builder;
        }

        public MauiAppBuilder RegisterLifecycleEvents()
        {
            builder.ConfigureLifecycleEvents(events =>
            {
#if WINDOWS10_0_17763_0_OR_GREATER

            events.AddWindows(wndLifeCycleBuilder =>
            {
                wndLifeCycleBuilder.OnWindowCreated(window =>
                {
                    //const int width = 1920;
                    //const int height = 1080;
                    //const int x = 3440 / 2 - width / 2;
                    //const int y = 1440 / 2 - height / 2;
                    //window.MoveAndResize(x, y, width, height);

                    //var manager = WinUIEx.WindowManager.Get(window);
                    //manager.PersistenceId = "HackedMauiId";
                    //manager.MinWidth = 640;
                    //manager.MinHeight = 480;

                    window.CenterOnScreen(1200, 900);

                    window.SystemBackdrop = new MicaBackdrop { Kind = MicaKind.Base };

                    // Style the caption buttons so they're visible at rest against the Mica backdrop
                    var titleBar = window.AppWindow.TitleBar;
                    titleBar.ExtendsContentIntoTitleBar = false;

                    // Foreground: app Primary purple
                    var fgColor = Windows.UI.Color.FromArgb(0xFF, 0x41, 0x36, 0x54);       // #413654 Primary
                    // Hover background: Secondary lavender at ~40% opacity
                    var hoverBg = Windows.UI.Color.FromArgb(0x66, 0xDF, 0xD8, 0xF7);       // #66DFD8F7
                    // Pressed background: Tertiary purple at ~60% opacity
                    var pressedBg = Windows.UI.Color.FromArgb(0x99, 0xA6, 0x8C, 0xD4);     // #99A68CD4
                    // Transparent resting background — lets Mica show through
                    var transparentBg = Windows.UI.Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF);

                    // Title bar area background (the non-button region)
                    titleBar.BackgroundColor         = transparentBg;
                    titleBar.InactiveBackgroundColor = transparentBg;

                    // Caption buttons
                    titleBar.ButtonForegroundColor         = fgColor;
                    titleBar.ButtonBackgroundColor         = transparentBg;
                    titleBar.ButtonHoverForegroundColor    = fgColor;
                    titleBar.ButtonHoverBackgroundColor    = hoverBg;
                    titleBar.ButtonPressedForegroundColor  = Windows.UI.Color.FromArgb(0xFF, 0xDF, 0xD8, 0xF7); // Secondary
                    titleBar.ButtonPressedBackgroundColor  = pressedBg;
                    titleBar.ButtonInactiveForegroundColor  = Windows.UI.Color.FromArgb(0x66, 0x41, 0x36, 0x54); // Primary @40%
                    titleBar.ButtonInactiveBackgroundColor = transparentBg;

                });
            });

#elif MACCATALYST
                
                events.AddiOS(wndLifeCycleBuilder =>
                {
                    wndLifeCycleBuilder.SceneWillConnect((scene, session, options) =>
                    {
                        if (scene is UIWindowScene { SizeRestrictions: { } } windowScene)
                        {
                            windowScene.SizeRestrictions.MaximumSize = new CGSize(1200, 900);
                            windowScene.SizeRestrictions.MinimumSize = new CGSize(600, 400);
                        }
                    });

                    // Force Dark UI style via UIKit so title text remains light on the purple title bar when the system switches to Light mode.
                    wndLifeCycleBuilder.OnActivated(application =>
                    {
                        foreach (var scene in application.ConnectedScenes)
                        {
                            if (scene is not UIWindowScene windowScene)
                                continue;

                            foreach (var window in windowScene.Windows)
                                window.OverrideUserInterfaceStyle = UIUserInterfaceStyle.Dark;
                        }
                    });

                });
#endif
            });

            return builder;
        }
    }
}