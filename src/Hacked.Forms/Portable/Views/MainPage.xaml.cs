using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hacked.Core.Models;
using Hacked.Core.Primitives;
using Hacked.Forms.Portable.Helpers;
using Hacked.Forms.Portable.ViewModels;
using Telerik.XamarinForms.DataControls.ListView;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Views
{
    public partial class MainPage : ContentPage
    {
        //private bool hasTutorialBeenShown;
        //FilterType filterType = FilterType.Name;

        public MainPage()
        {
            InitializeComponent();
        }

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();
            
        //    //---Bug in SideDrawer, commented out first launch opening---//
        //    //check if it's the first launch (IsFirstLaunchKey shouldnt exist, so we have a default value of true)
        //    //if (!hasTutorialBeenShown && Settings.IsFirstLaunch)
        //    //    FirstRunTutorial();

        //    //if there are accounts listed AND...
        //    if (ViewModelLocator.Main.Accounts.Count > 0)
        //    {
        //        //the swipe tip have not already been shown
        //        if (!Settings.SwipeTipShown)
        //            SwipeTip.IsVisible = true;

        //        if (!Settings.AccountRefreshShown)
        //            AccountRefreshTip.IsVisible = true;
        //    }
            
        //}
        

        //private async void FirstRunTutorial()
        //{
        //    //Drawer.IsOpen = true;

        //    //hide main burger button when the drawer is opened
        //    await MainBurgerButton.FadeTo(0, 200);

        //    hasTutorialBeenShown = true;

        //    Settings.IsFirstLaunch = false;

        //    await Application.Current.MainPage.DisplayAlert("Welcome!",
        //        "To get started, you'll want to add an account.\r\n\nClick 'add account' and enter the email address you want to monitor.",
        //        "OK");
                

        //    //change color, animate and change color back
        //    AddAccountModalButton.BackgroundColor = Color.Green;
        //    await AddAccountModalButton.ScaleTo(1.5, 500, Easing.BounceOut);
        //    await AddAccountModalButton.ScaleTo(1, 500, Easing.BounceIn);
            
        //}

        //#region button click handlers

        //private async void AboutButton_OnClicked(object sender, EventArgs e)
        //{
        //    await App.RootNavigationPage.Navigation.PushAsync(new AboutPage());
        //}

        //private async void BackupAccountsButton_OnClick(object sender, EventArgs e)
        //{
        //    await App.RootNavigationPage.Navigation.PushAsync(new BackupPage());
        //}

        //private void BurgerButton_OnClicked(object sender, EventArgs e)
        //{
        //    Drawer.IsOpen = !Drawer.IsOpen;
        //}

        //private async void SettingsModalButton_OnClicked(object sender, EventArgs e)
        //{
        //    await App.RootNavigationPage.Navigation.PushAsync(new SettingsPage());
        //}

        //private async void AddAccountModalButton_OnClicked(object sender, EventArgs e)
        //{
        //    await AddAccountGrid.FadeTo(1);
        //    AddAccountGrid.IsVisible = true;
        //}

        //#endregion

        //#region ListView event handlers

        //private async void RefreshSwipeButton_Clicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var button = sender as Button;
        //        var account = button?.BindingContext as MonitoredAccount;

        //        if (account == null)
        //            return;

        //        var lastCount = account.Breaches.Count;

        //        await ViewModelLocator.Main.UpdateBreachesForAccountAsync(account);

        //        if (account.Breaches.Count > lastCount)
        //        {
        //            ViewModelLocator.Main.SaveAccounts();
        //            await this.DisplayAlert("New breaches have been detected", "Alert", "close");
        //        }
        //    }
        //    finally
        //    {
        //        AccountsListView.EndItemSwipe();
        //    }
        //}

        //private void DeleteSwipeButton_Clicked(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        ViewModelLocator.Main.IsBusy = true;
        //        ViewModelLocator.Main.IsBusyMessage = "deleting account...";

        //        var button = sender as Button;

        //        if (button?.BindingContext is MonitoredAccount account)
        //            ViewModelLocator.Main.RemoveAccount(account);
        //    }
        //    finally
        //    {
        //        ViewModelLocator.Main.IsBusy = false;
        //        ViewModelLocator.Main.IsBusyMessage = "";
        //        AccountsListView.EndItemSwipe();
        //    }
        //}

        //private void AccountsListView_OnSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (!AccountsListView.IsSwipingInProgress)
        //        Drawer.IsOpen = false;
        //}

        //private async void AccountsListView_OnRefreshRequested(object sender, PullToRefreshRequestedEventArgs e)
        //{
        //    try
        //    {
        //        if (!ViewModelLocator.Main.HasAccounts)
        //            return;

        //        await ViewModelLocator.Main.FindAllAccountsBreachesAsync();
                
        //        //if first time, hide tip and persist via settings
        //        if (AccountRefreshTip.IsVisible)
        //        {
        //            await AccountRefreshTip.FadeTo(0, 500, Easing.CubicInOut);
        //            AccountRefreshTip.IsVisible = false;
        //            Settings.AccountRefreshShown = true;
        //        }
        //    }
        //    finally
        //    {
        //        AccountsListView.EndRefresh();
        //    }
        //}

        //private async void AccountsListView_OnItemSwipeCompleted(object sender, ItemSwipeCompletedEventArgs e)
        //{
        //    //if first time, hide tip and save
        //    if (SwipeTip.IsVisible)
        //    {
        //        await SwipeTip.FadeTo(0, 300, Easing.CubicInOut);
        //        SwipeTip.IsVisible = false;
        //        Settings.SwipeTipShown = true;
        //    }
        //}

        //private void BreachesListView_OnSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.NewItems != null && e.NewItems.Count > 0)
        //    {
        //        ViewModelLocator.Main.SelectedBreach = e.NewItems[0] as Breach;

        //        Debug.WriteLine($"BreachesListView_OnSelectionChanged fired - SelectedItem: {ViewModelLocator.Main.SelectedBreach?.Title}");
        //        App.RootNavigationPage.Navigation.PushAsync(new BreachDetailsPage());
        //    }
        //}

        //private async void BreachesListView_OnRefreshRequested(object sender, PullToRefreshRequestedEventArgs e)
        //{
        //    try
        //    {
        //        await ViewModelLocator.Main.UpdateBreachesForAccountAsync(ViewModelLocator.Main.SelectedAccount);

        //        if (ViewModelLocator.Main.SelectedAccount.Breaches.Any(b => b.IsNew))
        //            ViewModelLocator.Main.SaveAccounts();
        //    }
        //    finally
        //    {
        //        BreachesListView.EndRefresh();
        //    }
        //}

        //#endregion

        //#region drawer event handlers
        
        //private void Drawer_OnDrawerClosing(object sender, EventArgs e)
        //{
        //    Debug.WriteLine($"DrawerClosing");
        //    //MainBurgerButton.FadeTo(1, 200);
        //}

        //private void Drawer_OnDrawerOpening(object sender, EventArgs e)
        //{
        //    Debug.WriteLine($"DrawerOpening");
        //    //MainBurgerButton.FadeTo(0, 200);

        //    //if (!hasTutorialBeenShown && Settings.IsFirstLaunch)
        //    //    FirstRunTutorial();
        //}
        
        //#endregion

        //#region add account overlay handlers

        //private async void AddAccount_OnClicked(object sender, EventArgs e)
        //{
        //    await AttemptAddAsync(this.EmailEntry?.Text);
        //}

        //private async void EmailEntry_OnCompleted(object sender, EventArgs e)
        //{
        //    await AttemptAddAsync(this.EmailEntry?.Text);
        //}

        //private void EmailEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    var entry = sender as Entry;
        //    var text = entry?.Text;

        //    var isValid = !string.IsNullOrEmpty(text);

        //    AddAccountButton.BackgroundColor = isValid ? Color.FromHex("#009C46") : Color.FromHex("#BB0000");
        //    //AddAccountButton.TextColor = isValid ? Color.White : Color.Black;
        //    AddAccountButton.IsEnabled = isValid;
        //}

        //private async Task AttemptAddAsync(string emailAddress)
        //{
        //    if (string.IsNullOrEmpty(emailAddress))
        //        return;

        //    if (await ViewModelLocator.Main.AddAccount(emailAddress) != null)
        //    {
        //        await AddAccountGrid.FadeTo(0);

        //        AddAccountGrid.IsVisible = false;
        //        Drawer.IsOpen = false;

        //        //await AccountNameLabel.FadeTo(0);
        //        //await AccountNameLabel.FadeTo(1);
                
        //        //await AccountNameLabel.ScaleTo(1.5, 500, Easing.BounceOut);
        //        //await AccountNameLabel.ScaleTo(1, 500, Easing.BounceIn);

        //        //await AccountNameLabel.TranslateTo(100,0,500, Easing.SpringIn);
        //        //await AccountNameLabel.TranslateTo(0,0,500, Easing.SpringOut);

        //        //reset entry field
        //        EmailEntry.Text = "";
        //    }
        //}

        //private async void CancelButton_OnClicked(object sender, EventArgs e)
        //{
        //    await AddAccountGrid.FadeTo(0);
        //    AddAccountGrid.IsVisible = false;
        //}

        //#endregion

        //#region list filtering

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
        //        return name.Contains(FilterEntry?.Text.ToLowerInvariant());
        //    }

        //    if (filterType == FilterType.DataStolen)
        //    {
        //        var classesList = ((Breach) arg).DataClasses;
        //        return classesList.Any(dataClass => dataClass.Contains(FilterEntry?.Text.ToLowerInvariant()));
        //    }

        //    return false;
        //}

        //private async void FilterSwitch_OnToggled(object sender, ToggledEventArgs e)
        //{
        //    if (e.Value)
        //    {
        //        await AccountNameLabel.FadeTo(0, 100);
        //        await AccountNamePrefixLabel.FadeTo(0, 100);

        //        FilteringGrid.IsVisible = true;
        //    }
        //    else
        //    {
        //        BreachesListView.FilterDescriptors.Clear();
        //        FilteringGrid.IsVisible = false;

        //        await AccountNamePrefixLabel.FadeTo(1, 100);
        //        await AccountNameLabel.FadeTo(1, 100);
        //    }
        //}

        //#endregion
    }
}
