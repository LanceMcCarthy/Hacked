using Hacked.Core.Models;
using Hacked.Maui.ViewModels;
using Telerik.Maui.Controls;

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

        // this.SelectedBreach is a navigation parameter defined in PageBase
        if (_viewModel.SelectedBreach == null)
        {
            _viewModel.SelectedBreach = this.SelectedBreach;
            //ArrangeLayout();
        }
    }

    //private void ArrangeLayout()
    //{
    //    WrapLayout.Children.Clear();

    //    if (BindingContext is not Breach breach) 
    //        return;

    //    if (!breach.DataClasses.Any())
    //        return;

    //    var darkColor = Color.FromArgb("#3E8EED");
    //    var lightColor = Colors.White;



    //    foreach (var dataClass in breach.DataClasses)
    //    {
    //        var label = new Label
    //        {
    //            Text = dataClass
    //        };

    //        //label.SetAppThemeColor(Label.TextColorProperty, lightColor, darkColor);

    //        var border = new RadBorder
    //        {
    //            Content = label,
    //            Padding = new Thickness(5, 1.5, 5, 2),
    //            CornerRadius = new Thickness(2),
    //            Margin = new Thickness(0,0,5,0)
    //        };

    //        border.SetAppThemeColor(RadBorder.BackgroundColorProperty, darkColor, lightColor);
                
    //        WrapLayout.Children.Add(border);
    //    }
    //}
}