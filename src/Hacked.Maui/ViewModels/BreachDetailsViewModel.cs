using CommonHelpers.Common;
using Hacked.Core.Models;

namespace Hacked.Maui.ViewModels;

public class BreachDetailsViewModel : ViewModelBase
{
    private Breach? _selectedBreach;

    public BreachDetailsViewModel()
    {
    }

    public Breach? SelectedBreach
    {
        get => _selectedBreach;
        set => SetProperty(ref _selectedBreach, value);
    }
}