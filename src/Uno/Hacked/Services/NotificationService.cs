namespace Hacked.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public void ShowBreachNotification(string accountAddress, int newBreachCount)
    {
#if WINDOWS
        try
        {
            var template = Windows.UI.Notifications.ToastNotificationManager.GetTemplateContent(
                Windows.UI.Notifications.ToastTemplateType.ToastText02);
            var textNodes = template.GetElementsByTagName("text");
            textNodes[0].AppendChild(template.CreateTextNode("New Breaches Found"));
            textNodes[1].AppendChild(template.CreateTextNode(
                $"{newBreachCount} new breach(es) found for {accountAddress}."));
            var notification = new Windows.UI.Notifications.ToastNotification(template);
            Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().Show(notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show toast notification for {Address}", accountAddress);
        }
#else
        _logger.LogInformation(
            "Breach notification: {Count} new breach(es) found for {Address}",
            newBreachCount, accountAddress);
#endif
    }
}
