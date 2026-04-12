using Hacked.Core.Common;
using Hacked.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace Hacked.Services;

public class BackgroundMonitorService : IBackgroundMonitorService, IHostedService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(30);

    private readonly IAccountsService _accountsService;
    private readonly IPwndBreachService _breachService;
    private readonly INotificationService _notificationService;
    private readonly ISettingsService _settingsService;
    private readonly ILogger<BackgroundMonitorService> _logger;

    private CancellationTokenSource? _cts;
    private Task? _monitorTask;
    private volatile bool _isMonitoring;

    public bool IsMonitoring => _isMonitoring;

    public BackgroundMonitorService(
        IAccountsService accountsService,
        IPwndBreachService breachService,
        INotificationService notificationService,
        ISettingsService settingsService,
        ILogger<BackgroundMonitorService> logger)
    {
        _accountsService = accountsService;
        _breachService = breachService;
        _notificationService = notificationService;
        _settingsService = settingsService;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _isMonitoring = true;
        _monitorTask = RunMonitorLoopAsync(_cts.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _isMonitoring = false;
        if (_cts is { } cts)
        {
            cts.Cancel();
            cts.Dispose();
            _cts = null;
        }
        if (_monitorTask is { } task)
        {
            try { await task.ConfigureAwait(false); }
            catch (OperationCanceledException) { }
            catch (Exception ex) { _logger.LogError(ex, "Error while stopping background monitor"); }
        }
    }

    private async Task RunMonitorLoopAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(CheckInterval);
        try
        {
            while (await timer.WaitForNextTickAsync(token).ConfigureAwait(false))
            {
                await CheckAllAccountsAsync(token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Background monitor loop encountered an unexpected error");
        }
        finally
        {
            _isMonitoring = false;
        }
    }

    private async Task CheckAllAccountsAsync(CancellationToken token)
    {
        try
        {
            await _accountsService.LoadAccountsAsync().ConfigureAwait(false);
            var accounts = _accountsService.CurrentAccounts.ToList();

            foreach (var account in accounts)
            {
                if (token.IsCancellationRequested) break;
                await CheckAccountAsync(account).ConfigureAwait(false);
            }

            _settingsService.LastBackgroundCheck = DateTime.UtcNow;
            await _accountsService.SaveAccountsAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during background check cycle");
        }
    }

    private async Task CheckAccountAsync(MonitoredAccount account)
    {
        try
        {
            var existingNames = account.Breaches
                .Select(b => b.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            ObservableCollection<Breach> results;
            try
            {
                results = await _breachService.CheckForBreachesAsync(account).ConfigureAwait(false);
            }
            catch (PwnedApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                results = new ObservableCollection<Breach>();
            }

            int newCount = 0;
            account.Breaches.Clear();
            foreach (var breach in results)
            {
                breach.IsNew = !existingNames.Contains(breach.Name);
                if (breach.IsNew) newCount++;
                account.Breaches.Add(breach);
            }
            account.LastUpdated = DateTime.UtcNow;

            if (newCount > 0 && _settingsService.NotificationsEnabled)
            {
                _notificationService.ShowBreachNotification(account.Address, newCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking account {Address} in background", account.Address);
        }
    }
}
