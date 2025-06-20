using CommunityToolkit.Mvvm.Messaging;
using Hacked.Core.Common;
using Hacked.Maui.ViewModels;

namespace Hacked.Maui.Views;

public partial class AboutPage
{
    public AboutPage(AboutViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        StartAnimations();
    }

    private void StartAnimations()
    {
        Gear1.RotateTo(1440, 18000, Easing.CubicInOut);
        Gear2.RotateTo(-1440, 18000, Easing.CubicInOut);
        Gear3.RotateTo(1440, 18000, Easing.CubicInOut);
    }

    private async void ContactUsButton_OnClick(object sender, EventArgs e)
    {
        try
        {
            await Email.ComposeAsync(new EmailMessage
            {
                Subject = $"Hacked App - Feedback {DeviceInfo.Platform}",
                Body = "[enter your message here]",
                To = ["awesome.apps@outlook.com"]
            });
        }
        catch (Exception ex)
        {
            WeakReferenceMessenger.Default.Send(new MessagingCenterError
            {
                Caller = "ContactUsButton_OnClick",
                Exception = ex
            });
        }
    }

    private void ReviewButton_OnClickButton_OnClick(object sender, EventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new MessagingCenterAlert
        {
            Title = "Coming soon",
            Message = "The ability to leave a review will be implemented once the app is out of beta.",
            Cancel = "OK"
        });
    }
}