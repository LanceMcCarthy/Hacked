using Hacked.Core.Models;
using Telerik.Maui.Controls;

namespace Hacked.Maui.Controls;

public partial class BreachItemView : ContentView
{
	public BreachItemView()
	{
		InitializeComponent();
        BindingContextChanged += BreachItemView_BindingContextChanged;
	}

    private void BreachItemView_BindingContextChanged(object sender, EventArgs e)
    {
        WrapLayout.Children.Clear();

        if (BindingContext is not Breach breach) 
            return;

        if (!breach.DataClasses.Any())
            return;

        var darkColor = Color.FromArgb("#3E8EED");
        var lightColor = Colors.White;

        foreach (var dataClass in breach.DataClasses)
        {
            WrapLayout.Children.Add(new RadBorder
            {
                Style = (Style)this.Resources["DataClassBorderStyle"],
                Content = new Label
                {
                    Style = (Style)this.Resources["DataClassLabelStyle"],
                    Text = dataClass
                }
            });
        }
    }
}