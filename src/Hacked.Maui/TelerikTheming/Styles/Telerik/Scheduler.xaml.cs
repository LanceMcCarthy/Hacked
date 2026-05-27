using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Telerik.Theming.Telerik.Styles;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Scheduler : ResourceDictionary
{
	public Scheduler(ResourceDictionary buttons)
	{
		this.MergedDictionaries.Add(buttons);
		InitializeComponent();
	}
}