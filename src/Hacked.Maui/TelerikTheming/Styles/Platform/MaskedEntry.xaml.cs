using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Platform.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MaskedEntry : ResourceDictionary
{
	public MaskedEntry(ResourceDictionary entry)
	{
		this.MergedDictionaries.Add(entry);
		InitializeComponent();
	}
}