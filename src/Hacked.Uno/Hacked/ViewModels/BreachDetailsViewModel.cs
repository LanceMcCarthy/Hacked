namespace Hacked.ViewModels;

public partial class BreachDetailsViewModel : ObservableObject
{
    private readonly INavigator _navigator;

    [ObservableProperty]
    private Breach _breach;

    public BreachDetailsViewModel(Breach breach, INavigator navigator)
    {
        Breach = breach;
        _navigator = navigator;
    }

    [RelayCommand]
    private async Task OpenDomain()
    {
        if (!string.IsNullOrWhiteSpace(Breach.Domain))
        {
            var uri = new Uri($"https://{Breach.Domain}");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
