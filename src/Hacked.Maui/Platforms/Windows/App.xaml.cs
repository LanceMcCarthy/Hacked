using Microsoft.Maui;
using Microsoft.UI.Xaml;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Hosting;

namespace Hacked.Maui.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        this.InitializeComponent();
        this.RequestedTheme = ApplicationTheme.Light;
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        Platform.OnLaunched(args);
    }
}