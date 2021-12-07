using Hacked.Maui.Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Specialized;

namespace Hacked.Maui.Views;

public partial class RootPage : FlyoutPage
{
    public RootPage()
    {
        InitializeComponent();
        SideMenuPage.MenuListView.SelectionChanged += ListViewOnSelectionChanged;
    }

    private void ListViewOnSelectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is { Count: > 0 } && e.NewItems[0] is NavigationMenuItem menuItem)
        {
            var page = (ContentPage)Activator.CreateInstance(menuItem.TargetType);

            ((Application.Current.MainPage as RootPage)?.Detail as NavigationPage)?.Navigation.PushAsync(page);

            SideMenuPage.MenuListView.SelectedItem = null;

            if (Device.RuntimePlatform != "UWP")
            {
                IsPresented = false;
            }
        }
    }
}