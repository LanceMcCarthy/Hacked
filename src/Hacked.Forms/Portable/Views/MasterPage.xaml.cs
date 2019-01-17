using System.Collections.Generic;
using Hacked.Forms.Portable.Models;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Views
{
    public partial class MasterPage : ContentPage
    {
        public ListView ListView => this.listView;

        public MasterPage()
        {
            InitializeComponent();
            
            listView.ItemsSource = new List<NavigationMenuItem>
            {
                new NavigationMenuItem
                {
                    Title = "Accounts",
                    IconSource = "ic_accounts.png",
                    TargetType = typeof(AccountsPage)
                },
                new NavigationMenuItem
                {
                    Title = "Dashboard",
                    IconSource = "ic_dashboard.png",
                    TargetType = typeof(DashboardPage)
                },
                new NavigationMenuItem
                {
                    Title = "Backup",
                    IconSource = "ic_backup.png",
                    TargetType = typeof(BackupPage)
                },
                new NavigationMenuItem
                {
                    Title = "Settings",
                    IconSource = "ic_settings.png",
                    TargetType = typeof(SettingsPage)
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
