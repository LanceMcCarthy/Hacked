using CommunityToolkit.Mvvm.Messaging;
using Hacked.Core.Common;
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

    //public static void ShowExceptionMessage2(string callerName, Exception ex)
    //{
    //    TaskHelpers.RunOnUiThread(async () =>
    //    {
    //        var message = "An unexpected error has occurred. If this happens again, contact us at awesome.apps@outlook.com and share the error message below" +
    //                      $"\r\n\n{callerName} Error:" +
    //                      $"\r\n {ex.Message}";

    //        if (Current?.MainPage != null)
    //        {
    //            WeakReferenceMessenger.Default.Send(new MessagingCenterAlert
    //            {
    //                Title = "Unexpected Error",
    //                Message = message,
    //                Cancel = "close"
    //            });
    //        }
    //    });
    //}

    //public static void ShowMessage2(string title,string message, string buttonText = "ok")
    //{
    //    TaskHelpers.RunOnUiThread(() =>
    //    {
    //        Shell.Current.DisplayAlert(title, message, buttonText);
    //    });
    //}
}
