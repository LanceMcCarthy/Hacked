using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Platform.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Chat : ResourceDictionary
{
	public Chat(ResourceDictionary collectionView, ResourceDictionary promptInput)
	{
		this.MergedDictionaries.Add(collectionView);
		this.MergedDictionaries.Add(promptInput);
		InitializeComponent();
	}
}