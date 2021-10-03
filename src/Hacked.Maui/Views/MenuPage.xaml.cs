using Hacked.Maui.Models;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using Telerik.Maui.Controls;
using Telerik.XamarinForms.DataControls;

namespace Hacked.Maui.Views
{
    public partial class MenuPage : ContentPage
    {
        public RadListView MenuListView;

        public MenuPage()
        {
            InitializeComponent();

            MenuListView = this.listView;

            listView.ItemsSource = new List<NavigationMenuItem>
            {
                new NavigationMenuItem
                {
                    Title = "Main",
                    IconSource = "ic_accounts.png",
                    TargetType = typeof(AccountsPage)
                },
                new NavigationMenuItem
                {
                    Title = "Settings",
                    IconSource = "ic_settings.png",
                    TargetType = typeof(AddAccountPage)
                },
                new NavigationMenuItem
                {
                    Title = "About",
                    IconSource = "ic_about.png",
                    TargetType = typeof(AboutPage)
                }
            };
        }
    }
}