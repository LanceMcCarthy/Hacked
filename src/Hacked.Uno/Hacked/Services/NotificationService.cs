namespace Hacked.Services;

/// <summary>
/// Cross-platform notification service using the WinUI API surface that Uno maps natively.
/// Windows: Windows.UI.Notifications toast (WinUI/Uno mapped API).
/// iOS/macOS: UNUserNotificationCenter (Apple system notifications).
/// Android/WASM/Desktop: Logged only (extend as needed).
/// </summary>
public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task ShowBreachNotificationAsync(string accountAddress, int newBreachCount)
    {
#if WINDOWS
        await ShowWindowsToastAsync(accountAddress, newBreachCount);
#elif __IOS__ || __MACCATALYST__
        await ShowAppleNotificationAsync(accountAddress, newBreachCount);
#else
        _logger.LogInformation(
            "Breach notification: {Count} new breach(es) found for {Address}",
            newBreachCount, accountAddress);
        await Task.CompletedTask;
#endif
    }

#if WINDOWS
    private Task ShowWindowsToastAsync(string accountAddress, int newBreachCount)
    {
        try
        {
            var xml = Windows.UI.Notifications.ToastNotificationManager
                .GetTemplateContent(Windows.UI.Notifications.ToastTemplateType.ToastText02);
            var nodes = xml.GetElementsByTagName("text");
            nodes[0].AppendChild(xml.CreateTextNode("New Breaches Found"));
            nodes[1].AppendChild(xml.CreateTextNode(
                $"{newBreachCount} new breach(es) found for {accountAddress}."));
            Windows.UI.Notifications.ToastNotificationManager
                .CreateToastNotifier()
                .Show(new Windows.UI.Notifications.ToastNotification(xml));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show Windows toast for {Address}", accountAddress);
        }
        return Task.CompletedTask;
    }
#endif

#if __IOS__ || __MACCATALYST__
    private async Task ShowAppleNotificationAsync(string accountAddress, int newBreachCount)
    {
        try
        {
            var content = new UserNotifications.UNMutableNotificationContent
            {
                Title = "New Breaches Found",
                Body = $"{newBreachCount} new breach(es) found for {accountAddress}."
            };
            var request = UserNotifications.UNNotificationRequest.FromIdentifier(
                Guid.NewGuid().ToString(), content, null);
            await UserNotifications.UNUserNotificationCenter.Current
                .AddNotificationRequestAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show Apple notification for {Address}", accountAddress);
        }
    }
#endif
}
