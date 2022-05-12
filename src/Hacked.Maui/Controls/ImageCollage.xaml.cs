using Hacked.Core.Models;

namespace Hacked.Maui.Controls;

public partial class ImageCollage : ContentView
{
	public ImageCollage()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty AssociatedAccountProperty =
        BindableProperty.Create("IsExpanded", typeof(MonitoredAccount), typeof(ImageCollage), null, propertyChanged:OnAssociatedAccountChanged);

    private static void OnAssociatedAccountChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is ImageCollage self)
        {
            if (newvalue == null)
            {
                self.RootFlexLayout.Children.Clear();
            }

            if (newvalue is MonitoredAccount acct)
            {
                if (!acct.Breaches.Any())
                    return;
                
                for (var i = 0; i < acct.Breaches.Count; i++)
                {
                    if (i == 8)
                        break;

                    var img = new Image
                    {
                        Source = new UriImageSource { Uri = acct.Breaches[i].LogoPath },
                        Aspect = Aspect.AspectFill,

                        Margin = 5
                    };
                    
                    self.RootFlexLayout.Children.Add(img);
                }
            }
        }
    }

    public MonitoredAccount AssociatedAccount
    {
        get => (MonitoredAccount)GetValue(AssociatedAccountProperty);
        set => SetValue(AssociatedAccountProperty, value);
    }
}