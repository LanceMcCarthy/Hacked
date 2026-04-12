namespace Hacked.Services;

public interface IBackgroundMonitorService
{
    bool IsMonitoring { get; }
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}
