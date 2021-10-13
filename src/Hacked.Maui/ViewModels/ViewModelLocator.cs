namespace Hacked.Maui.ViewModels
{
    public static class ViewModelLocator
    {
        private static AccountsViewModel _accounts;
        private static AboutViewModel _about;
        private static SettingsViewModel _settings;

        public static AccountsViewModel Accounts => _accounts ??= new AccountsViewModel();
        public static AboutViewModel About => _about ??= new AboutViewModel();
        public static SettingsViewModel Settings => _settings ??= new SettingsViewModel();
    }
}
