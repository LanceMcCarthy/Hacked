using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Platform.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class TreeDataGrid : ResourceDictionary
{
    public TreeDataGrid(ResourceDictionary datagrid)
    {
        this.MergedDictionaries.Add(datagrid);
        InitializeComponent();
    }
}