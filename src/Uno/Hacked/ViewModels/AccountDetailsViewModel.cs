namespace Hacked.ViewModels;

public partial class AccountDetailsViewModel : ObservableObject
{
    private readonly INavigator _navigator;

    [ObservableProperty]
    private MonitoredAccount _account;

    [ObservableProperty]
    private ObservableCollection<Breach> _filteredBreaches = new();

    [ObservableProperty]
    private string _filterText = string.Empty;

    public AccountDetailsViewModel(MonitoredAccount account, INavigator navigator)
    {
        _account = account;
        _navigator = navigator;

        foreach (var breach in _account.Breaches)
            FilteredBreaches.Add(breach);
    }

    partial void OnFilterTextChanged(string value)
    {
        FilteredBreaches.Clear();
        var filter = value?.Trim();
        foreach (var breach in _account.Breaches)
        {
            if (string.IsNullOrWhiteSpace(filter) ||
                breach.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                breach.Title.Contains(filter, StringComparison.OrdinalIgnoreCase))
            {
                FilteredBreaches.Add(breach);
            }
        }
    }

    [RelayCommand]
    private async Task NavigateToBreachDetails(Breach breach)
    {
        await _navigator.NavigateViewModelAsync<BreachDetailsViewModel>(this, data: breach);
    }

    [RelayCommand]
    private void ClearNewFlags()
    {
        foreach (var breach in _account.Breaches)
            breach.IsNew = false;
        _account.HasNewBreaches = false;
    }
}
