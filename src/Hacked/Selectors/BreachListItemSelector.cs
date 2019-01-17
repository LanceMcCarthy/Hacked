using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Hacked.Core.Models;
using Microsoft.HockeyApp;

namespace Hacked.Selectors
{
    public class BreachListItemSelector : DataTemplateSelector
    {
        public DataTemplate BreachItemTemplate { get; set; }
        public DataTemplate AdItemTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            try
            {
                if (item is Breach breach)
                {
                    if (breach.Id == "AD")
                    {
                        return AdItemTemplate;
                    }
                    
                    return BreachItemTemplate;
                }
            }
            catch (Exception ex)
            {
                HockeyClient.Current.TrackException(ex);
            }

            return base.SelectTemplateCore(item, container);
        }
    }
}
