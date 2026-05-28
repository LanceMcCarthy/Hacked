using CommonHelpers.Maui.Mvvm;
using Hacked.Core.Common;
using Hacked.Core.Models;

namespace Hacked.Maui.Common;

public class PageBase : ContentPage, IQueryAttributable
{
    public MonitoredAccount? SelectedAccount { get; private set; }

    public Breach? SelectedBreach { get; private set; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("SelectedAccount", out var account) && account is MonitoredAccount monitoredAccount)
        {
            SelectedAccount = monitoredAccount;
            OnPropertyChanged(nameof(SelectedAccount));
        }

        if (query.TryGetValue("SelectedBreach", out var breach) && breach is Breach selectedBreach)
        {
            SelectedBreach = selectedBreach;
            OnPropertyChanged(nameof(SelectedBreach));
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if(BindingContext is PageViewModelBase viewModel)
        {
            viewModel.OnAppearing();
        }
    }

    protected override bool OnBackButtonPressed()
    {
        if (BindingContext is PageViewModelBase viewModel)
        {
            return viewModel.OnBackButtonRequested();
        }

        return base.OnBackButtonPressed();
    }
}
