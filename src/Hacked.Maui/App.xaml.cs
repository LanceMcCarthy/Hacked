namespace Hacked.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        UserAppTheme = AppTheme.Unspecified;
        RequestedThemeChanged += OnRequestedThemeChanged;
        ApplyTelerikTheme(RequestedTheme);
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var win = new Window();
        win.Page ??= activationState?.Context.Services.GetRequiredService<AppShell>();

        win.Created += (_, _) => ApplyTelerikTheme(RequestedTheme);

        return win;

        //this.MainPage ??= activationState?.Context.Services.GetRequiredService<AppShell>();
        //return base.CreateWindow(activationState);
    }

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        ApplyTelerikTheme(e.RequestedTheme);
    }

    private static void ApplyTelerikTheme(AppTheme requestedTheme)
    {
        TelerikThemeResources.AppTheme = requestedTheme == AppTheme.Dark 
            ? TelerikTheme.TelerikTurquoiseDark 
            : TelerikTheme.TelerikTurquoise;
    }
}
