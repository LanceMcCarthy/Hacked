using Hacked.Maui.Helpers;

namespace Hacked.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        UserAppTheme = AppTheme.Unspecified;
        RequestedThemeChanged += OnRequestedThemeChanged;
        ThemeHelper.ApplySavedTheme(GetEffectiveAppTheme(RequestedTheme));
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var win = new Window();
        win.Page ??= activationState?.Context.Services.GetRequiredService<AppShell>();

        win.Created += (_, _) => ThemeHelper.ApplySavedTheme(GetEffectiveAppTheme(RequestedTheme));

        return win;

        //this.MainPage ??= activationState?.Context.Services.GetRequiredService<AppShell>();
        //return base.CreateWindow(activationState);
    }

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        ThemeHelper.ApplySavedTheme(GetEffectiveAppTheme(e.RequestedTheme));
    }

    private static AppTheme GetEffectiveAppTheme(AppTheme requestedTheme)
    {
        return requestedTheme == AppTheme.Unspecified ? AppTheme.Light : requestedTheme;
    }
}
