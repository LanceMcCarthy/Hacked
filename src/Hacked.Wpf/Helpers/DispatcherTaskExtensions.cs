using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Hacked.Wpf.Helpers
{
    public static class DispatcherTaskExtensions
    {
        public static async Task<T> RunTaskAsync<T>(this Dispatcher dispatcher, Func<Task<T>> func, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();

            await dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    taskCompletionSource.SetResult(await func());
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            }, priority);
            
            return await taskCompletionSource.Task;
        }
        
        public static async Task RunTaskAsync(this Dispatcher dispatcher, Func<Task> func, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            await RunTaskAsync(dispatcher, async () =>
            {
                await func();
                return false;

            }, priority);
        }


        public static async Task CallOnUiThreadAsync(Func<Task> func)
        {
            if(func != null && System.Windows.Application.Current.MainWindow != null)
            {
                await System.Windows.Application.Current.MainWindow.Dispatcher.RunTaskAsync(func);
            }
        }
    }
}
