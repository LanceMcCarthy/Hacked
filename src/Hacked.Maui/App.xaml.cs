namespace Hacked.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        Application.Current.UserAppTheme = AppTheme.Unspecified;
        Application.Current.RequestedThemeChanged += (s, e) => ApplyTelerikTheme();
        this.ApplyTelerikTheme();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var win = new Window();
        win.Page ??= activationState?.Context.Services.GetRequiredService<AppShell>();
        return win;

        //this.MainPage ??= activationState?.Context.Services.GetRequiredService<AppShell>();
        //return base.CreateWindow(activationState);
    }

    private void ApplyTelerikTheme()
    {
        TelerikThemeResources.AppTheme = Application.Current.RequestedTheme == AppTheme.Dark 
            ? TelerikTheme.TelerikTurquoiseDark 
            : TelerikTheme.TelerikTurquoise;
    }
}
