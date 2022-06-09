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
        
        if (BindingContext is Breach breach)
        {
            if (!breach.DataClasses.Any())
                return;

            var darkColor = Color.FromArgb("#3E8EED");
            var lightColor = Colors.White;

            foreach (var dataClass in breach.DataClasses)
            {
                var label = new Label
                {
                    Text = dataClass
                };

                label.SetAppThemeColor(Label.TextColorProperty, lightColor, darkColor);

                var border = new RadBorder
                {
                    Content = label,
                    Padding = new Thickness(5, 3, 5, 2),
                    CornerRadius = new Thickness(5),
                    Margin = new Thickness(0,0,5,0)
                };

                border.SetAppThemeColor(RadBorder.BackgroundColorProperty, darkColor, lightColor);
                
                WrapLayout.Children.Add(border);
            }
        }
    }
}