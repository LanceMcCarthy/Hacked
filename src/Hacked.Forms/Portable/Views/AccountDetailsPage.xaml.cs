using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Hacked.Core.Models;
using Hacked.Core.Primitives;
using Hacked.Forms.Portable.ViewModels;
using Telerik.XamarinForms.DataControls.ListView;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Views
{
    public partial class AccountDetailsPage : ContentPage
    {
        private FilterType filterType = FilterType.Name;

        public AccountDetailsPage()
        {
            InitializeComponent();
        }

        #region ListView event handlers
        
        private async void BreachesListView_OnSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                ViewModelLocator.Main.SelectedBreach = e.NewItems[0] as Breach;

                Debug.WriteLine($"BreachesListView_OnSelectionChanged fired - SelectedItem: {ViewModelLocator.Main.SelectedBreach?.Title}");
                await ((Application.Current.MainPage as RootPage)?.Detail as NavigationPage)?.Navigation.PushAsync(new BreachDetailsPage());
            }
        }

        private async void BreachesListView_OnRefreshRequested(object sender, PullToRefreshRequestedEventArgs e)
        {
            try
            {
                await ViewModelLocator.Main.UpdateBreachesForAccountAsync(ViewModelLocator.Main.SelectedAccount);

                if (ViewModelLocator.Main.SelectedAccount.Breaches.Any(b => b.IsNew))
                    ViewModelLocator.Main.SaveAccounts();
            }
            finally
            {
                BreachesListView.EndRefresh();
            }
        }

        #endregion

        #region list filtering

        //private void FilterEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    BreachesListView.FilterDescriptors.Clear();
        //    BreachesListView.FilterDescriptors.Add(new DelegateFilterDescriptor { Filter = this.Filter });
        //}

        //private bool Filter(object arg)
        //{
        //    if (filterType == FilterType.Name)
        //    {
        //        var name = ((Breach) arg).Name.ToLowerInvariant();
        //        return name.Contains(FilterEntry.Text.ToLowerInvariant());
        //    }

        //    if (filterType == FilterType.DataStolen)
        //    {
        //        var classesList = ((Breach) arg).DataClasses;
        //        return classesList.Any(dataClass => dataClass.Contains(FilterEntry.Text.ToLowerInvariant()));
        //    }

        //    return false;
        //}

        //private async void FilterSwitch_OnToggled(object sender, ToggledEventArgs e)
        //{
        //    if (e.Value)
        //    {
        //        await AccountNameLabel.FadeTo(0, 100);

        //        FilteringGrid.IsVisible = true;
        //    }
        //    else
        //    {
        //        BreachesListView.FilterDescriptors.Clear();
        //        FilteringGrid.IsVisible = false;
                
        //        await AccountNameLabel.FadeTo(1, 100);
        //    }
        //}

        #endregion
    }
}
