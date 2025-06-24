namespace Hacked.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var win = new Window();
        win.Page ??= activationState?.Context.Services.GetRequiredService<AppShell>();
        return win;

        //this.MainPage ??= activationState?.Context.Services.GetRequiredService<AppShell>();
        //return base.CreateWindow(activationState);
    }
}
