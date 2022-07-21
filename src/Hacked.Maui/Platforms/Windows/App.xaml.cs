using Microsoft.UI.Xaml;

namespace Hacked.Maui.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        this.InitializeComponent();
        this.RequestedTheme = ApplicationTheme.Light;
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

