using System;
using Hacked.Forms.Portable.Models;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Views
{
    public partial class RootPage
    {
        public RootPage()
        {
            InitializeComponent();
            masterPage.ListView.ItemSelected += OnItemSelected;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is NavigationMenuItem masterPageItem)
            {
                var page = (ContentPage) Activator.CreateInstance(masterPageItem.TargetType);
                
                ((Application.Current.MainPage as RootPage)?.Detail as NavigationPage)?.Navigation.PushAsync(page);
                
                masterPage.ListView.SelectedItem = null;

                if(Device.RuntimePlatform != "UWP")
                    IsPresented = false;
            }
        }
    }
}
