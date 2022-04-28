using Hacked.Maui.Views;

namespace Hacked.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("accounts/accountdetails", typeof(AccountDetailsPage));
            Routing.RegisterRoute("accounts/addaccount", typeof(AddAccountPage));
            //Routing.RegisterRoute("settings/about", typeof(AboutPage));

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