namespace Hacked.Services;

public interface INotificationService
{
    Task ShowBreachNotificationAsync(string accountAddress, int newBreachCount);
}
