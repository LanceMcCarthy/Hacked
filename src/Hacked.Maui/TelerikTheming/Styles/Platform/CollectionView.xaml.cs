using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Telerik.Theming.Platform.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CollectionView : ResourceDictionary
{
    public CollectionView()
    {
        this.InitializeComponent();
    }
}