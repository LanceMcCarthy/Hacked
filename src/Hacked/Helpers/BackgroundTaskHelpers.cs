using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Popups;

namespace Hacked.Helpers
{
    public static class BackgroundTaskHelpers
    {
        public static async Task RegisterTaskAsync(string taskFriendlyName, string taskEntryPoint, uint taskRunFrequency, SystemConditionType condition = SystemConditionType.InternetAvailable)
        {
            try
            {
                //if task already exists, unregister it before adding it
                foreach (var task in BackgroundTaskRegistration.AllTasks.Where(cur => cur.Value.Name == taskFriendlyName))
                {
                    task.Value.Unregister(true);
                }

                var builder = new BackgroundTaskBuilder();
                builder.Name = taskFriendlyName;
                builder.TaskEntryPoint = taskEntryPoint;
                builder.SetTrigger(new TimeTrigger(taskRunFrequency, false));
                builder.AddCondition(new SystemCondition(condition));
                builder.Register();
            }
            catch (Exception ex)
            {
                await new MessageDialog($"RegisterTaskAsync Exception\r\n\nError: {ex.Message}").ShowAsync();
            }
        }

        public static async Task<bool> CheckBackgroundTasksAsync(string taskFriendlyName)
        {
            try
            {
                await BackgroundExecutionManager.RequestAccessAsync();

                return BackgroundTaskRegistration.AllTasks.Any(task => task.Value.Name == taskFriendlyName);
            }
            catch (Exception ex)
            {
                await new MessageDialog($"Something went wrong checking for background tasks. Error: {ex.Message}").ShowAsync();
                return false;
            }
        }

        public static async Task<bool> UnregisterTaskAsync(string taskFriendlyName)
        {
            try
            {
                await BackgroundExecutionManager.RequestAccessAsync();

                foreach (var task in BackgroundTaskRegistration.AllTasks.Where(cur => cur.Value.Name == taskFriendlyName))
                {
                    task.Value.Unregister(true);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                await new MessageDialog($"UnregisterTaskAsync Exception\r\n\nError: {ex.Message}").ShowAsync();
                return false;
            }
        }

    }
}
