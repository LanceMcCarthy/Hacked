using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Telerik.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Editor : ResourceDictionary
{
    public Editor(ResourceDictionary inputView)
    {
        this.MergedDictionaries.Add(inputView);
        InitializeComponent();
    }
}