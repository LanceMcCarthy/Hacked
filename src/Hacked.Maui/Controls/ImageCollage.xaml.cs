using Hacked.Core.Models;

namespace Hacked.Maui.Controls;

public partial class ImageCollage : ContentView
{
	public ImageCollage()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty AssociatedAccountProperty =
        BindableProperty.Create("AssociatedAccount", typeof(MonitoredAccount), typeof(ImageCollage), null, propertyChanged:OnAssociatedAccountChanged);

    private static void OnAssociatedAccountChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is not ImageCollage self) 
            return;

        switch (newvalue)
        {
            case null:
                self.WrapLayout.Children.Clear();
                break;
            case MonitoredAccount acct when !acct.Breaches.Any():
                return;
            case MonitoredAccount acct:
            {
                self.WrapLayout.Children.Clear();

                for (var i = 0; i < acct.Breaches.Count; i++)
                {
                    // No more than 8
                    if (i == 9)
                        break;

                    self.WrapLayout.Children.Add(new Image
                    {
                        Source = new UriImageSource { Uri = acct.Breaches[i].LogoPath },
                        WidthRequest = 75,
                        HeightRequest = 75
                    });
                }

                break;
            }
        }
    }

    public MonitoredAccount AssociatedAccount
    {
        get => (MonitoredAccount)GetValue(AssociatedAccountProperty);
        set => SetValue(AssociatedAccountProperty, value);
    }
}