using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Platform.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class TabView : ResourceDictionary
{
    public TabView()
    {
        this.InitializeComponent();
    }
}