using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace Hacked;

public sealed partial class MainPage
{
    private void NotifyUserOfUpdatesOrChanges()
    {
        var noticeShown = false;

        if (localSettings.Values.TryGetValue($"{ViewModel.AppVersion}NoticeShown", out var hasShownNoticeSetting))
        {
            noticeShown = (bool)hasShownNoticeSetting;
        }

        if (!noticeShown)
        {
            NoticeTitle.Text = $"Updated! v.{ViewModel.AppVersion}";

            NoticeFeaturesTextBlock.Text =
                "- Major: Add support for monitoring a phone number.\n" +
                "- Major: Export and Import! You can now backup your accounts list to a small file.\n" +
                "- Major: Upgraded API key, allows global access, no more restricted regions due to API abuse.\n" +
                "- Upgraded UWP SDK version, with modernized UI controls (Windows 11).\n" +
                "- More modernized Fluent Design elements throughout the app; shadow, light and blur effects.\n";
            NoticeFixesTextBlock.Text =
                "- Removed annoying spell check when adding new monitoring account.\n" +
                "- Significantly faster UI loading times\n" +
                "- Many more smaller improvements, too numerous to list here";

            NoticeOverlay.Visibility = Visibility.Visible;
        }
    }

    private async Task SendEmail()
    {
        try
        {
            ViewModel.IsBusy = true;
            ViewModel.IsBusyMessage = "opening email...";

            var email = new EmailMessage();
            email.To.Add(new EmailRecipient("awesome.apps@outlook.com", "Lancelot Software"));
            email.Subject = $"Hacked {ViewModel.AppVersion}";
            email.Body = "[write your message here]\r\n\n";

            await EmailManager.ShowComposeNewEmailAsync(email);
        }
        catch (Exception ex)
        {
            await new MessageDialog(
                $"Something went wrong trying to open your email application automatically. You can still manually send an email to awesome.apps@outlook.com. /r/n/nError: {ex.Message}")
                .ShowAsync();
        }
        finally
        {
            ViewModel.IsBusy = false;
            ViewModel.IsBusyMessage = "";
        }
    }
}
