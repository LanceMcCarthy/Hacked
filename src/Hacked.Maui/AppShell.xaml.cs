using Hacked.Maui.Views;

namespace Hacked.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();

            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                CurrentItem = PhoneTabs;
            }
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("accounts/details", typeof(AccountDetailsPage));
            Routing.RegisterRoute("accounts/addaccount", typeof(AddAccountPage));
            //Routing.RegisterRoute("settings/about", typeof(AboutPage));
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            GoToAsync("//settings");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
            {
                CurrentItem = HomeTab;
            }
            else
            {
                CurrentItem = HomeFlyoutItem;
            }
        }
    }
}