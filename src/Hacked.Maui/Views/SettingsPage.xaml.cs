using Hacked.Maui.Common;
using Hacked.Maui.Helpers;
using Telerik.XamarinForms.Common;

namespace Hacked.Maui.Views;

public partial class SettingsPage : ContentPage
{
    private readonly List<string> _themeNames = new() { "Blue", "Green", "Red" };

    public SettingsPage()
    {
        InitializeComponent();

        segmentControl.ItemsSource = _themeNames;
        segmentControl.SelectedIndex = _themeNames.IndexOf(Settings.SelectedTheme);
    }

    private void SegmentControl_OnSelectionChanged(object sender, ValueChangedEventArgs<int> e)
    {
        if (e?.NewValue == null || _themeNames[e.NewValue] == Settings.SelectedTheme)
            return;
        
        ThemeHelper.SetTheme(_themeNames[e.NewValue]);
    }
}