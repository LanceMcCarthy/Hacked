using System;
using Hacked.Forms.Portable.ViewModels;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Views
{
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModelLocator.Main.UpdateStatistics();
            //SelectionBehavior.ClearSelection();
        }

        private void ChartSelectionBehavior_OnSelectionChanged(object sender, EventArgs e)
        {
            //if (SelectionBehavior.SelectedPoints.Any())
            //{
            //    var selectedPoint = SelectionBehavior.SelectedPoints?.FirstOrDefault();

            //    var accountAddress = selectedPoint?.DataItem as CategoricalChartData;

            //    if (accountAddress != null)
            //    {
            //        ViewModelLocator.Main.SelectedAccount =
            //            ViewModelLocator.Main.Accounts.FirstOrDefault(a => a.Address == accountAddress.Category);
            //    }
            //}
            //else
            //{
            //    ViewModelLocator.Main.SelectedAccount = null;
            //}
        }

        private async void SelectedAccountBreachDetailsButton_clicked(object sender, EventArgs e)
        {
            if(ViewModelLocator.Main.SelectedAccount != null)
            {
                await ((Application.Current.MainPage as RootPage)?.Detail as NavigationPage)?.Navigation.PushAsync(new AccountDetailsPage());
            }
        }
    }
}