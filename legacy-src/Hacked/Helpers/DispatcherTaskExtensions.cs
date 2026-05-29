using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Hacked.Helpers;

public static class DispatcherTaskExtensions
{
    public static async Task<T> RunTaskAsync<T>(this CoreDispatcher dispatcher, Func<Task<T>> func, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
    {
        var taskCompletionSource = new TaskCompletionSource<T>();

        await dispatcher.RunAsync(priority, async () =>
        {
            try
            {
                taskCompletionSource.SetResult(await func());
            }
            catch (Exception ex)
            {
                taskCompletionSource.SetException(ex);
            }
        });

        return await taskCompletionSource.Task;
    }

    public static async Task RunTaskAsync(this CoreDispatcher dispatcher, Func<Task> func, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
    {
        await RunTaskAsync(dispatcher, async () =>
        {
            await func();
            return false;

        }, priority);
    }


    public static async Task CallOnUiThreadAsync(DispatchedHandler handler)
    {
        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, handler);
    }
}
