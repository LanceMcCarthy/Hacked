using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Hacked.Core.Common;

namespace Hacked.Wpf.Helpers
{
    public static class DisplayMessageHelpers
    {
        public static async void ShowExceptionMessageOnUiThread(string callerName, Exception ex)
        {
            await DispatcherTaskExtensions.CallOnUiThreadAsync(() =>
            {
                if (ex is PwnedApiException pex)
                {
                    if (pex.StatusCode == HttpStatusCode.Forbidden)
                    {
                        MessageBox.Show($"The server is experiencing a high demand right now and the app was blocked from requesting more checks from your region. \n\n\nPlease contact the developer if this keeps happening so we can request the API service give us more bandwidth in your area. ", "Forbidden");
                    }

                    if (pex.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        MessageBox.Show($"The server rejected the request stating that the app is not authorized. Please contact the developer immediately.", "Not Authorized");
                    }
                }
                else
                {
                    MessageBox.Show($"An unexpected error has occurred. If this happens again, contact us at awesome.apps@outlook.com and share the error message below" +
                                    $"\r\n\n{callerName} Error:" +
                                    $"\r\n {ex.Message}");
                }

                return Task.CompletedTask;
            });
        }

        public static async void ShowUserMessageOnUiThread(string message, string title)
        {
            await DispatcherTaskExtensions.CallOnUiThreadAsync((() =>
            {
                MessageBox.Show(message, title);

                return Task.CompletedTask;
            }));
        }
    }
}
