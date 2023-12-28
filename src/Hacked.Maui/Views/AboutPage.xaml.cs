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
        Gear1.RotateTo(1440, 18000, Easing.SinInOut);
        Gear2.RotateTo(-1440, 18000, Easing.SinInOut);
        Gear3.RotateTo(1440, 18000, Easing.SinInOut);
    }

    private async void ContactUsButton_OnClick(object sender, EventArgs e)
    {
        await Email.ComposeAsync(new EmailMessage
        {
            Subject = $"Hacked App - Feedback {DeviceInfo.Platform}",
            Body = "[enter your message here]",
            To = ["awesome.apps@outlook.com"]
        });
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

    //private void CloseModalButton_OnClicked(object sender, EventArgs e)
    //{
    //    Navigation.PopModalAsync();
    //}
}