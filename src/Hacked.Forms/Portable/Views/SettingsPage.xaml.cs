using System.Collections.Generic;
using Hacked.Forms.Portable.Helpers;
using Telerik.XamarinForms.Common;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Views
{
    public partial class SettingsPage : ContentPage
    {
        private readonly List<string> _themeNames = new List<string> { "Blue", "Green", "Red" };

        public SettingsPage()
        {
            InitializeComponent();

            segmentControl.ItemsSource = _themeNames;
            segmentControl.SelectedIndex = _themeNames.IndexOf(Settings.SelectedTheme);
        }

        private void SegmentControl_OnSelectionChanged(object sender, ValueChangedEventArgs<int> e)
        {
            if (e?.NewValue != null)
            {
                var selectedThemeName = _themeNames[e.NewValue];

                if (Settings.SelectedTheme == selectedThemeName)
                    return;

                ThemeHelper.SetTheme(selectedThemeName);
            }
        }
    }
}