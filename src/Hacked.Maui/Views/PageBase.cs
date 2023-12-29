using Hacked.Core.Common;
using Hacked.Core.Models;

namespace Hacked.Maui.Views;

public class PageBase : ContentPage, IQueryAttributable
{
    public MonitoredAccount SelectedAccount { get; private set; }

    public Breach SelectedBreach { get; private set; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("SelectedAccount", out var account))
        {
            SelectedAccount = account as MonitoredAccount;
            OnPropertyChanged(nameof(SelectedAccount));
        }

        if (query.TryGetValue("SelectedBreach", out var breach))
        {
            SelectedBreach = breach as Breach;
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
