namespace Hacked.Services;

public interface INotificationService
{
    void ShowBreachNotification(string accountAddress, int newBreachCount);
}
