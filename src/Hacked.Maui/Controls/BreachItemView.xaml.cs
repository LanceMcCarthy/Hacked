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

    private async void BreachItemView_BindingContextChanged(object sender, EventArgs e)
    {
        if (BindingContext is not Breach breach || breach.DataClasses.Count == 0)
            return;

        await Task.Run(() =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (WrapLayout.Children.Count > 0)
                    WrapLayout.Children.Clear();
            });

            foreach (var dataClass in breach.DataClasses)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    WrapLayout.Children.Add(new RadBorder
                    {
                        Style = (Style)Application.Current?.Resources["DataClassBorderStyle"],
                        Content = new Label
                        {
                            Style = (Style)Application.Current?.Resources["DataClassLabelStyle"],
                            Text = dataClass
                        }
                    });
                });

            }
        });
    }
}