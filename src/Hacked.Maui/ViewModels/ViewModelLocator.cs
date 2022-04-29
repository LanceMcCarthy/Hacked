namespace Hacked.Maui.ViewModels;

public static class ViewModelLocator
{
    private static MonitoredAccountsViewModel _monitoredAccounts;
    private static AboutViewModel _about;
    private static SettingsViewModel _settings;

    public static MonitoredAccountsViewModel MonitoredAccounts => _monitoredAccounts ??= new MonitoredAccountsViewModel();
    public static AboutViewModel About => _about ??= new AboutViewModel();
    public static SettingsViewModel Settings => _settings ??= new SettingsViewModel();
}