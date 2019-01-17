using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Hacked.Core.Models;

namespace Hacked.Controls
{
    public sealed partial class Help : UserControl
    {

        //const string AddAccountGifUrl = "https://hacked.blob.core.windows.net:443/help-files/AddAccount.gif";
        //const string DeleteAccountGifUrl = "https://hacked.blob.core.windows.net:443/help-files/DeleteAccount.gif";
        //const string FilteringGifUrl = "https://hacked.blob.core.windows.net:443/help-files/Filtering.gif";
        //const string BackgroundMonitoringGifUrl = "https://hacked.blob.core.windows.net:443/help-files/BackgroundMonitoring.gif";
        //const string BackupAccountsGifUrl = "https://hacked.blob.core.windows.net:443/help-files/BackupAccounts.gif";
        //const string RestoreAccountsGifUrl = "https://hacked.blob.core.windows.net:443/help-files/RestoreAccounts.gif";
        //const string FeedbackHubGifUrl = "https://hacked.blob.core.windows.net:443/help-files/FeedbackHub.gif";

        public ObservableCollection<HelpArticle> Articles { get; private set; }
        
        public Help()
        {
            this.InitializeComponent();
            Articles = new ObservableCollection<HelpArticle>();

            Loaded += Help_Loaded;
        }

        private async void Help_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            Articles.Add(new HelpArticle { Title = "How to add a Monitored Account", ImageUrl = await GetImagePathAsync("AddAccount.gif"), Summary = "Open the side pane using the 'hamburger' button and select the 'Add Account' button. When the popup appears, enter the email address or username for the account you want to monitor and click 'add'.", IsExpanded = true });
            Articles.Add(new HelpArticle { Title = "How to remove a Monitored Account", ImageUrl = await GetImagePathAsync("DeleteAccount.gif"), Summary = "Next to each account in the Monitored Accounts list there is a delete button. Tap that and confirm to remove the account." });
            Articles.Add(new HelpArticle { Title = "How to filter breaches list", ImageUrl = await GetImagePathAsync("Filtering.gif"), Summary = "When a MonitoredAccount is selected, you can filter the breaches list by selecting the funnel icon at the top. While the button is toggled, you will see a box to type in your filter term. Deselect the funnel icon to stop filtering." });
            Articles.Add(new HelpArticle { Title = "How to enable Background Monitoring", ImageUrl = await GetImagePathAsync("BackgroundMonitoring.gif"), Summary = "To enable background monitoring, press the Background Monitor button at the top right of the main area. You will then see a toggle switch to enable or disable background monitoring. The Background Monitoring button will be green if it is currently monitoring or red if it is not." });
            Articles.Add(new HelpArticle { Title = "How to backup accounts list", ImageUrl = await GetImagePathAsync("BackupAccounts.gif"), Summary = "To backup your current list of Monitored Accounts, open the side pane and press the Backup/Restore button. Next, select the Backup button." });
            Articles.Add(new HelpArticle { Title = "How to restore from backup", ImageUrl = await GetImagePathAsync("RestoreAccounts.gif"), Summary = "To restore your backed up Monitored Accounts, open the side pane, press the Backup/Restore button and finally press the Restore button." });
            Articles.Add(new HelpArticle { Title = "Feedback Hub integration", ImageUrl = await GetImagePathAsync("FeedbackHub.gif"), Summary = "If you are running Windows 10 v 1607 (aka Anniversary Update), you will see a button on the 'About' popup. This will open the Windows 10 Feedback Hub. Anything you put in here will go directly to the developers and can be 'upvoted' by other app users." });
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        //public static readonly DependencyProperty ArticlesProperty = DependencyProperty.Register(
        //    "Articles", typeof(List<HelpArticle>), typeof(Help), new PropertyMetadata(default(List<HelpArticle>)));

        //public List<HelpArticle> Articles
        //{
        //    get { return (List<HelpArticle>) GetValue(ArticlesProperty); }
        //    set { SetValue(ArticlesProperty, value); }
        //}

        //private new ObservableCollection<HelpArticle> LoadArticles()
        //{
        //    return new ObservableCollection<HelpArticle>
        //    {
        //        //new HelpArticle {Title = "How to add a Monitored Account", ImageUrl = AddAccountGifUrl, Summary = "Open the side pane using the 'hamburger' button and select the 'Add Account' button. When the popup appears, enter the email address or username for the account you want to monitor and click 'add'."},
        //        //new HelpArticle {Title = "How to remove a Monitored Account", ImageUrl = DeleteAccountGifUrl, Summary = "Next to each account in the Monitored Accounts list there is a delete button. Tap that and confirm to remove the account."},
        //        //new HelpArticle {Title = "How to filter breaches list", ImageUrl = FilteringGifUrl, Summary = "When a MonitoredAccount is selected, you can filter the breaches list by selecting the funnel icon at the top. While the button is toggled, you will see a box to type in your filter term. Deselect the funnel icon to stop filtering."},
        //        //new HelpArticle {Title = "How to enable Background Monitoring", ImageUrl = BackgroundMonitoringGifUrl, Summary = "To enable background monitoring, press the Background Monitor button at the top right of the main area. You will then see a toggle switch to enable or disable background monitoring. The Background Monitoring button will be green if it is currently monitoring or red if it is not."},
        //        //new HelpArticle {Title = "How to backup accounts list", ImageUrl = BackupAccountsGifUrl, Summary = "To backup your current list of Monitored Accounts, open the side pane and press the Backup/Restore button. Next, select the Backup button."},
        //        //new HelpArticle {Title = "How to restore from backup", ImageUrl = RestoreAccountsGifUrl, Summary = "To restore your backed up Monitored Accounts, open the side pane, press the Backup/Restore button and finally press the Restore button."},
        //        //new HelpArticle {Title = "Feedback Hub integration", ImageUrl = FeedbackHubGifUrl, Summary = "If you are running Windows 10 v 1607 (aka Anniversary Update), you will see a button on the 'About' popup. This will open the Windows 10 Feedback Hub. Anything you put in here will go directly to the developers and can be 'upvoted' by other app users."}
        //    };
        //}

        private static async Task<string> GetImagePathAsync(string fileName)
        {
            if (DesignMode.DesignModeEnabled)
            {
                return $"";
            }

            var localFolder = ApplicationData.Current.LocalFolder;

            try
            {
                var fileItem = await localFolder.TryGetItemAsync(fileName);

                if (fileItem != null)
                {
                    Debug.WriteLine($"Help GIF File EXISTS: {fileItem.Path}");
                    return fileItem.Path;
                }

                using(var client = new HttpClient())
                using (var stream = await client.GetStreamAsync($"https://hacked.blob.core.windows.net:443/help-files/{fileName}"))
                {
                    var file = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                    using (var fileStream = await file.OpenStreamForWriteAsync())
                    {
                        await stream.CopyToAsync(fileStream);

                        Debug.WriteLine($"Help GIF File Saved: {file.Path}");
                        return file?.Path;
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetImagePath exception. FileName : {fileName}, Exception: {ex.Message}");
                return $"https://hacked.blob.core.windows.net:443/help-files/{fileName}";
            }
        }
    }
}
