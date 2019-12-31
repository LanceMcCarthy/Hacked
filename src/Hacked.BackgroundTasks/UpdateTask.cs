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

            txtNodes[0].AppendChild(toastDescriptor.CreateTextNode("Updated to v3 API!"));
            txtNodes[1].AppendChild(toastDescriptor.CreateTextNode($"Hacked has been updated with major fixes and uses the new HIBP v3 API."));

            var toast = new ToastNotification(toastDescriptor);

            toastnotifier.Show(toast);
        }
    }
}
