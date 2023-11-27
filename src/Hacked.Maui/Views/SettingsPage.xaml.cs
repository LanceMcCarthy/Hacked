using Hacked.Maui.ViewModels;

namespace Hacked.Maui.Views;

public partial class SettingsPage
{
    public SettingsPage(SettingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}