using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Platform.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AIPrompt : ResourceDictionary
{
	public AIPrompt(ResourceDictionary editor)
	{
		this.MergedDictionaries.Add(editor);
		InitializeComponent();
	}
}