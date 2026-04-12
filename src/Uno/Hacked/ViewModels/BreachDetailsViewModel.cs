namespace Hacked.ViewModels;

public partial class BreachDetailsViewModel : ObservableObject
{
    private readonly INavigator _navigator;

    [ObservableProperty]
    private Breach _breach;

    public BreachDetailsViewModel(Breach breach, INavigator navigator)
    {
        _breach = breach;
        _navigator = navigator;
    }

    [RelayCommand]
    private async Task OpenDomain()
    {
        if (!string.IsNullOrWhiteSpace(_breach.Domain))
        {
            var uri = new Uri($"https://{_breach.Domain}");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
