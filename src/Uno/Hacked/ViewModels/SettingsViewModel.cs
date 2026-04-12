using Hacked.Services;
using Hacked.Services.Interfaces;

namespace Hacked.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IAccountsService _accountsService;
    private readonly ISettingsService _settingsService;
    private readonly IBackgroundMonitorService _backgroundMonitor;

    [ObservableProperty]
    private string _exportResult = string.Empty;

    [ObservableProperty]
    private bool _notificationsEnabled;

    public string AppVersion => "1.0";

    public bool IsMonitoringActive => _backgroundMonitor.IsMonitoring;

    public string LastCheckTime => _settingsService.LastBackgroundCheck.HasValue
        ? _settingsService.LastBackgroundCheck.Value.ToLocalTime().ToString("g")
        : "Never";

    public SettingsViewModel(
        IAccountsService accountsService,
        ISettingsService settingsService,
        IBackgroundMonitorService backgroundMonitor)
    {
        _accountsService = accountsService;
        _settingsService = settingsService;
        _backgroundMonitor = backgroundMonitor;
        _notificationsEnabled = _settingsService.NotificationsEnabled;
    }

    partial void OnNotificationsEnabledChanged(bool value)
    {
        _settingsService.NotificationsEnabled = value;
    }

    [RelayCommand]
    private async Task ExportAccounts()
    {
        ExportResult = string.Empty;
        var (success, message) = await _accountsService.ExportBackupAsync();
        ExportResult = success ? $"Exported to: {message}" : $"Export failed: {message}";
    }

    [RelayCommand]
    private async Task OpenHibpWebsite()
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://haveibeenpwned.com"));
    }
}
