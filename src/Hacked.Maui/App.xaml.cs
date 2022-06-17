using Hacked.Maui.Helpers;

namespace Hacked.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

    protected override void OnStart()
    {
    }

    public static void ShowExceptionMessage(string callerName, Exception ex)
    {
        TaskHelpers.RunOnUiThread(async () =>
        {
            var message = "An unexpected error has occurred. If this happens again, contact us at awesome.apps@outlook.com and share the error message below" +
                          $"\r\n\n{callerName} Error:" +
                          $"\r\n {ex.Message}";

            if (Current?.MainPage != null)
            {
                await Shell.Current.DisplayAlert(message, "Unexpected Error", "close");
            }
        });
    }
}
