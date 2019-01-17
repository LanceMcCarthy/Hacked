using System;
using Hacked.Core.Models;
using Hacked.Forms.Portable.Helpers;
using Hacked.Forms.Portable.ViewModels;
using Telerik.XamarinForms.DataControls;
using Telerik.XamarinForms.DataControls.ListView;
using Telerik.XamarinForms.Primitives.CheckBox;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Views
{
    public partial class AccountsPage : ContentPage
    {
        public AccountsPage()
        {
            InitializeComponent();
        }

        private async void RefreshSwipeButton_Clicked(object sender, EventArgs e)
        {
            if (AccountsListView == null)
                return;

            try
            {
                if (sender is Button button && button.BindingContext is MonitoredAccount account)
                {
                    var lastCount = account.Breaches.Count;

                    await ViewModelLocator.Main.UpdateBreachesForAccountAsync(account);

                    if (account.Breaches.Count > lastCount)
                    {
                        ViewModelLocator.Main.SaveAccounts();
                        await this.DisplayAlert("New breaches have been detected", "Alert", "close");
                    }
                }
            }
            finally
            {
                AccountsListView.EndItemSwipe();
            }
        }

        private void DeleteSwipeButton_Clicked(object sender, EventArgs e)
        {
            if (AccountsListView == null)
                return;

            try
            {
                if (sender is Button button && button?.BindingContext is MonitoredAccount account)
                {
                    ViewModelLocator.Main.RemoveAccount(account);
                }
            }
            finally
            {
                AccountsListView.EndItemSwipe();
            }
        }

        private async void AccountsListView_OnRefreshRequested(object sender, PullToRefreshRequestedEventArgs e)
        {
            if (AccountsListView == null)
                return;

            try
            {
                if (!ViewModelLocator.Main.HasAccounts)
                    return;

                await ViewModelLocator.Main.FindAllAccountsBreachesAsync();

                //if first time, hide tip and persist via settings
                if (AccountRefreshTip.IsVisible)
                {
                    await AccountRefreshTip.FadeTo(0, 500, Easing.CubicInOut);
                    AccountRefreshTip.IsVisible = false;
                    Settings.AccountRefreshShown = true;
                }
            }
            finally
            {
                AccountsListView.EndRefresh();
            }
        }

        private async void AccountsListView_OnItemSwipeCompleted(object sender, ItemSwipeCompletedEventArgs e)
        {
            var listView = sender as RadListView;

            if (e == null || listView == null)
                return;

            if (e.Item is MonitoredAccount account)
            {
                if (e.Offset > 201)
                {
                    var lastCount = account.Breaches.Count;

                    await ViewModelLocator.Main.UpdateBreachesForAccountAsync(account);

                    if (account.Breaches.Count > lastCount)
                    {
                        ViewModelLocator.Main.SaveAccounts();
                        await Application.Current.MainPage.DisplayAlert("New breaches have been detected", "Alert", "close");
                    }

                    listView?.EndItemSwipe();
                }
                else if (e.Offset < -200)
                {
                    ViewModelLocator.Main.RemoveAccount(account);
                    listView.EndItemSwipe();
                }
            }
        }

        private async void AddAccountModalButton_OnClicked(object sender, EventArgs e)
        {
            await ((Application.Current.MainPage as RootPage)?.Detail as NavigationPage)?.Navigation.PushAsync(new AddAccountPage());
        }

        private async void AccountsListView_OnItemTapped(object sender, ItemTapEventArgs e)
        {
            if (sender == null || AccountsListView.IsSwipingInProgress)
                return;

            if (e?.Item is MonitoredAccount account)
            {
                ViewModelLocator.Main.SelectedAccount = account;

                await ((Application.Current.MainPage as RootPage)?.Detail as NavigationPage)?.Navigation.PushAsync(new AccountDetailsPage());
            }
        }

        private async void RefreshTipCheckBox_OnIsCheckedChanged(object sender, IsCheckedChangedEventArgs e)
        {
            if (e.NewValue == false)
            {
                await AccountRefreshTip.FadeTo(0, 300, Easing.CubicInOut);
                AccountRefreshTip.IsVisible = false;
                Settings.AccountRefreshShown = true;

                await SwipeTip.FadeTo(0, 300, Easing.CubicInOut);
                SwipeTip.IsVisible = false;
                Settings.SwipeTipShown = true;
            }
            else if (e.NewValue == true)
            {
                await AccountRefreshTip.FadeTo(1, 300, Easing.CubicInOut);
                AccountRefreshTip.IsVisible = true;
                Settings.AccountRefreshShown = false;

                await SwipeTip.FadeTo(1, 300, Easing.CubicInOut);
                SwipeTip.IsVisible = true;
                Settings.SwipeTipShown = false;
            }
        }
    }
}