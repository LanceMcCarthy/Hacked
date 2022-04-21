namespace Hacked.Maui.ViewModels;

public static class ViewModelLocator
{
    private static MonitoredAccountsViewModel _accounts;
    private static AboutViewModel _about;
    private static SettingsViewModel _settings;

    public static MonitoredAccountsViewModel Accounts => _accounts ??= new MonitoredAccountsViewModel();
    public static AboutViewModel About => _about ??= new AboutViewModel();
    public static SettingsViewModel Settings => _settings ??= new SettingsViewModel();
}