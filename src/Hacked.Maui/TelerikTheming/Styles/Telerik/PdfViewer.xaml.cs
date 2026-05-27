using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Telerik.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class PdfViewer : ResourceDictionary
{
	public PdfViewer(ResourceDictionary busyindicator, ResourceDictionary entry, ResourceDictionary toolbar)
	{
		this.MergedDictionaries.Add(busyindicator);
		this.MergedDictionaries.Add(entry);
		this.MergedDictionaries.Add(toolbar);
		InitializeComponent();
	}
}