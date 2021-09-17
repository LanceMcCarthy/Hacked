using Hacked.Maui.Models;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace Hacked.Maui.Views
{
    public partial class MenuPage : ContentPage
    {
        public ListView ListView;

        public MenuPage()
        {
            InitializeComponent();

            this.ListView = this.listView;

            listView.ItemsSource = new List<NavigationMenuItem>
            {
                new NavigationMenuItem
                {
                    Title = "Main",
                    IconSource = "ic_accounts.png",
                    TargetType = typeof(MainPage)
                },
                //new NavigationMenuItem
                //{
                //    Title = "Dashboard",
                //    IconSource = "ic_dashboard.png",
                //    TargetType = typeof(DashboardPage)
                //},
                //new NavigationMenuItem
                //{
                //    Title = "Backup",
                //    IconSource = "ic_backup.png",
                //    TargetType = typeof(BackupPage)
                //},
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