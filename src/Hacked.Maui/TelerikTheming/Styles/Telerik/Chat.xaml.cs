using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Telerik.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Chat : ResourceDictionary
{
	public Chat(ResourceDictionary buttons, ResourceDictionary collectionView, ResourceDictionary promptInput)
	{
		this.MergedDictionaries.Add(buttons);
		this.MergedDictionaries.Add(collectionView);
		this.MergedDictionaries.Add(promptInput);
		InitializeComponent();
	}
}