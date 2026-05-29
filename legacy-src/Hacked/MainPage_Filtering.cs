using Hacked.Core.Models;
using Hacked.Core.Primitives;
using Hacked.Helpers;
using Hacked.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Hacked;

public sealed partial class MainPage
{
    #region Filtering

    private async void FilterTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        await FilterBreachesListAsync();
    }

    private void ClearFilterButton_OnClick(object sender, RoutedEventArgs e)
    {
        FilterTextBox.Text = "";
    }

    private async void CommitFilterButton_OnClick(object sender, RoutedEventArgs e)
    {
        await FilterBreachesListAsync();
    }

    private async Task FilterBreachesListAsync()
    {
        await DispatcherTaskExtensions.CallOnUiThreadAsync(() =>
        {
            var breaches = ((MainViewModel)DataContext)?.SelectedAccount?.Breaches;

            BreachesListView.ItemsSource = string.IsNullOrEmpty(FilterTextBox.Text)
                ? breaches
                : breaches?.Where(Filter);
        });
    }

    private bool Filter(object arg)
    {
        switch (filterType)
        {
            default:
            case FilterType.Name:
                var name = ((Breach)arg).Name.ToLowerInvariant();
                return name.Contains(FilterTextBox?.Text.ToLowerInvariant() ?? string.Empty);
            case FilterType.DataStolen:
                var classesList = ((Breach)arg).DataClasses;
                return classesList.Any(dataClass => dataClass.Contains(FilterTextBox?.Text.ToLowerInvariant() ?? string.Empty));
        }
    }

    private void ClearFilterToggleButton_OnClick(object sender, RoutedEventArgs e)
    {
        var toggleButton = sender as ToggleButton;

        switch (toggleButton?.IsChecked)
        {
            case null:
                return;
            case true:
                toggleButton.Content = new SymbolIcon(Symbol.Clear);
                break;
            default:
                toggleButton.Content = new SymbolIcon(Symbol.Filter);
                FilterTextBox.Text = "";
                BreachesListView.ItemsSource = ((MainViewModel)DataContext)?.SelectedAccount?.Breaches;
                break;
        }
    }

    #endregion
}
