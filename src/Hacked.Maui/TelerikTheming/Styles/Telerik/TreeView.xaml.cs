using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Telerik.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class TreeView : ResourceDictionary
{
	public TreeView(ResourceDictionary checkbox)
	{
		this.MergedDictionaries.Add(checkbox);
		InitializeComponent();
	}
}