using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Telerik.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ComboBox : ResourceDictionary
{
	public ComboBox(ResourceDictionary buttons)
	{
		this.MergedDictionaries.Add(buttons);
        InitializeComponent();
	}
}