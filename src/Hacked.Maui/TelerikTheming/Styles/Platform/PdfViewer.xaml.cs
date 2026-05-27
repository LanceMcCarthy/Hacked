using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Platform.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class PdfViewer : ResourceDictionary
{
	public PdfViewer(ResourceDictionary entry, ResourceDictionary toolbar)
	{
		this.MergedDictionaries.Add(entry);
		this.MergedDictionaries.Add(toolbar);
		InitializeComponent();
	}
}