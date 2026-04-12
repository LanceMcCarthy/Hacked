using Hacked.Services;
using Hacked.Services.Interfaces;

namespace Hacked.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IAccountsService _accountsService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private string _exportResult = string.Empty;

    [ObservableProperty]
    private bool _notificationsEnabled;

    public string AppVersion => "1.0";

    public SettingsViewModel(IAccountsService accountsService, ISettingsService settingsService)
    {
        _accountsService = accountsService;
        _settingsService = settingsService;
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
