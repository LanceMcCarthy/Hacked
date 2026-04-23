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

    private void BreachItemView_BindingContextChanged(object? sender, EventArgs e)
    {
        if (BindingContext is not Breach breach || breach.DataClasses.Count == 0)
            return;

        if (WrapLayout.Children.Count > 0)
            WrapLayout.Children.Clear();

        var resources = Application.Current?.Resources;
        object? borderStyleResource = null;
        object? labelStyleResource = null;
        resources?.TryGetValue("DataClassBorderStyle", out borderStyleResource);
        resources?.TryGetValue("DataClassLabelStyle", out labelStyleResource);

        var borderStyle = borderStyleResource as Style;
        var labelStyle = labelStyleResource as Style;

        foreach (var dataClass in breach.DataClasses)
        {
            var label = new Label
            {
                Text = dataClass
            };

            if (labelStyle != null)
                label.Style = labelStyle;

            var border = new RadBorder
            {
                Content = label
            };

            if (borderStyle != null)
                border.Style = borderStyle;

            WrapLayout.Children.Add(border);
        }
    }
}