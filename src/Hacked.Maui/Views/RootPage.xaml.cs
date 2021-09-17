using Hacked.Maui.Models;
using Microsoft.Maui.Controls;
using System;

namespace Hacked.Maui.Views
{
    public partial class RootPage : FlyoutPage
    {
        public RootPage()
        {
            InitializeComponent();
            menuPage.ListView.ItemSelected += OnItemSelected;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is NavigationMenuItem masterPageItem)
            {
                var page = (ContentPage)Activator.CreateInstance(masterPageItem.TargetType);

                ((Application.Current.MainPage as RootPage)?.Detail as NavigationPage)?.Navigation.PushAsync(page);

                menuPage.ListView.SelectedItem = null;

                if (Device.RuntimePlatform != "UWP")
                    IsPresented = false;
            }
        }
    }
}