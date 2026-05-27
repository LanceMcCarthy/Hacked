using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Platform.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Toolbar : ResourceDictionary
{
    public Toolbar(ResourceDictionary entry)
    {
        this.MergedDictionaries.Add(entry);
        InitializeComponent();
    }
}