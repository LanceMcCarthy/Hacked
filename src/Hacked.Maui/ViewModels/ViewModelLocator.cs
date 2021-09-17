namespace Hacked.Maui.ViewModels
{
    public static class ViewModelLocator
    {
        private static MainViewModel _main;
        private static AboutViewModel _about;
        private static SettingsViewModel _settings;

        public static MainViewModel Main => _main ??= new MainViewModel();

        public static AboutViewModel About => _about ??= new AboutViewModel();

        public static SettingsViewModel Settings => _settings ??= new SettingsViewModel();

    }
}
