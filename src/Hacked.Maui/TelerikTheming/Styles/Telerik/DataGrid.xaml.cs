using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Telerik.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class DataGrid : ResourceDictionary
{
	public DataGrid(ResourceDictionary buttons, ResourceDictionary collectionView)
	{
		this.MergedDictionaries.Add(buttons);
		this.MergedDictionaries.Add(collectionView);
		InitializeComponent();
	}
}