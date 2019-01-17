using System;
using Windows.UI.Popups;

namespace Hacked.Helpers
{
    public static class DisplayMessageHelpers
    {
        public static async void ShowExceptionMessageOnUiThread(string callerName, Exception ex)
        {
            await DispatcherTaskExtensions.CallOnUiThreadAsync(async () =>
            {
                await new MessageDialog($"An unexpected error has occured. If this happens again, contact us at awesome.apps@outlook.com and share the error message below" +
                                        $"\r\n\n{callerName} Error:" +
                                        $"\r\n {ex.Message}").ShowAsync();
            });
        }

        public static async void ShowUserMessageOnUiThread(string message, string title)
        {
            await DispatcherTaskExtensions.CallOnUiThreadAsync(async () =>
            {
                await new MessageDialog(message, title).ShowAsync();
            });
        }
    }
}
