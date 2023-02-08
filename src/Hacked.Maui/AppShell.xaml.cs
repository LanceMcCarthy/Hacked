using Hacked.Maui.Views;

namespace Hacked.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registered in XAML
            //Routing.RegisterRoute("About", typeof(AboutPage));
            //Routing.RegisterRoute("Settings", typeof(SettingsPage));
            //Routing.RegisterRoute("MonitoredAccounts", typeof(MonitoredAccountsPage));
            Routing.RegisterRoute("MonitoredAccounts/AccountDetails", typeof(AccountDetailsPage));
            
            // separate page in nav menu for now.
            //Routing.RegisterRoute("Settings/About", typeof(AboutPage));

            //if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
            //{
            //    CurrentItem = PhoneTabs;
            //}
        }

        //private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        //{
        //    GoToAsync("//settings");
        //}

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    //if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet)
        //    //{
        //    //    CurrentItem = HomeTab;
        //    //}
        //    //else
        //    //{
        //    //    CurrentItem = HomeFlyoutItem;
        //    //}
        //}
    }
}