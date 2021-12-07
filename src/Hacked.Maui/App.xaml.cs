using System;
using Hacked.Maui.Helpers;
using Hacked.Maui.Views;
using Microsoft.Maui.Controls;

namespace Hacked.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new RootPage();
    }

    protected override void OnStart()
    {
        ThemeHelper.LoadTheme();
    }

    public static void ShowExceptionMessage(string callerName, Exception ex)
    {
        TaskHelpers.RunOnUiThread(async () =>
        {
            var message = $"An unexpected error has occurred. If this happens again, contact us at awesome.apps@outlook.com and share the error message below" +
                          $"\r\n\n{callerName} Error:" +
                          $"\r\n {ex.Message}";

            await Current.MainPage.DisplayAlert(message, "Unexpected Error", "close");
        });
    }
}