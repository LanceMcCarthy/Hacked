using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace Hacked.BackgroundTasks
{
    public sealed class UpdateTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var toastnotifier = ToastNotificationManager.CreateToastNotifier();

            var toastDescriptor = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            var txtNodes = toastDescriptor.GetElementsByTagName("text");

            txtNodes[0].AppendChild(toastDescriptor.CreateTextNode("Updated!"));
            txtNodes[1].AppendChild(toastDescriptor.CreateTextNode($"Hacked? has been updated with improvements and fixes. Tap this notification to check it out."));

            var toast = new ToastNotification(toastDescriptor);

            toastnotifier.Show(toast);
        }
    }
}
