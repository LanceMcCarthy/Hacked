using Hacked.Maui.ViewModels;

namespace Hacked.Maui.Views;

public partial class AboutPage : ContentPage
{
	public AboutPage()
	{
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        StartAnimations();
    }

    private void StartAnimations()
    {
        RedGear.RotateTo(1440, 18000);
        GreenGear.RotateTo(-1440, 18000);
        BlueGear.RotateTo(1440, 18000);
    }

    private async void ContactUsButton_OnClick(object sender, EventArgs e)
    {
        var message = new EmailMessage
        {
            Subject = "Hacked App - Feedback",
            Body = "[enter your message here]",
            To = new List<string> { "awesome.apps@outlook.com" }
        };

        await Email.ComposeAsync(message);
    }

    private async void ReviewButton_OnClickButton_OnClick(object sender, EventArgs e)
    {
        await Application.Current.MainPage.DisplayAlert("Coming soon",
            "The ability to leave a review will be implemented once the app is out of beta.", "ok");
    }

    //private void CloseModalButton_OnClicked(object sender, EventArgs e)
    //{
    //    Navigation.PopModalAsync();
    //}
}