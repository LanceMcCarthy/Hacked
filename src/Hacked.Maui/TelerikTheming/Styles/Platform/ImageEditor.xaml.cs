using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Platform.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ImageEditor : ResourceDictionary
{
    public ImageEditor(ResourceDictionary buttons, ResourceDictionary toolbar)
    {
        this.MergedDictionaries.Add(buttons);
        this.MergedDictionaries.Add(toolbar);
        InitializeComponent();
    }
}