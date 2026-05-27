using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Platform.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Entry : ResourceDictionary
{
    public Entry(ResourceDictionary inputView)
    {
        this.MergedDictionaries.Add(inputView);
        InitializeComponent();
    }
}