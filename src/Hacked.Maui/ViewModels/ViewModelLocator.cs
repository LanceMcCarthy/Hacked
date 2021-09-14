namespace Hacked.Maui.ViewModels
{
    public static class ViewModelLocator
    {
        private static MainViewModel _main;
        private static AboutViewModel _about;

        public static MainViewModel Main => _main ?? (_main = new MainViewModel());

        public static AboutViewModel About => _about ?? (_about = new AboutViewModel());

    }
}
