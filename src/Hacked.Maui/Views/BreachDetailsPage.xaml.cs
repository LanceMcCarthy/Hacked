using Hacked.Maui.ViewModels;

namespace Hacked.Maui.Views;

public partial class BreachDetailsPage
{
    private readonly BreachDetailsViewModel _viewModel;

	public BreachDetailsPage(BreachDetailsViewModel vm)
	{
		InitializeComponent();
        this.BindingContext = _viewModel = vm;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        _viewModel.SelectedBreach ??= this.SelectedBreach;
    }
}