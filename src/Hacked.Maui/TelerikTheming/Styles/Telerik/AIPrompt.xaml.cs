using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Telerik.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AIPrompt : ResourceDictionary
{
	public AIPrompt(ResourceDictionary buttons, ResourceDictionary editor)
	{
		this.MergedDictionaries.Add(editor);
		this.MergedDictionaries.Add(buttons);
		InitializeComponent();
	}
}