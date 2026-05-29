using Hacked.Core.Common;
//using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Net;
using Windows.UI.Popups;
using Microsoft.Services.Store.Engagement;

namespace Hacked.Helpers;

public static class DisplayMessageHelpers
{
    public static async void ShowExceptionMessageOnUiThread(string callerName, Exception ex)
    {
        StoreServicesCustomEventLogger.GetDefault().Log($"ExceptionMessageOnUiThread: {callerName}");

        //Crashes.TrackError(ex, new Dictionary<string, string>
        //    { 
        //        { "Caller Name", callerName }
        //    });

        await DispatcherTaskExtensions.CallOnUiThreadAsync(async () =>
        {
            if (ex is PwnedApiException pex)
            {
                if (pex.StatusCode == HttpStatusCode.Forbidden)
                {
                    await new MessageDialog($"The server is experiencing a high demand right now and the app was blocked from requesting more checks from your region. \n\n\nPlease contact the developer if this keeps happening so we can request the API service give us more bandwidth in your area. ", "Forbidden").ShowAsync();
                }

                if (pex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await new MessageDialog($"The server rejected the request stating that the app is not authorized. Please contact the developer immediately.", "Not Authorized").ShowAsync();
                }
            }
            else
            {
                await new MessageDialog($"An unexpected error has occured. If this happens again, contact us at awesome.apps@outlook.com and share the error message below" +
                                        $"\r\n\n{callerName} Error:" +
                                        $"\r\n {ex.Message}").ShowAsync();
            }
        });
    }

    //public static async void ShowUserMessageOnUiThread(string message, string title)
    //{
    //    await DispatcherTaskExtensions.CallOnUiThreadAsync(async () =>
    //    {
    //        await new MessageDialog(message, title).ShowAsync();
    //    });
    //}
}
