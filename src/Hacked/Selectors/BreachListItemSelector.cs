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

        public DataTemplate MsftAdTemplate { get; set; }

        public DataTemplate VungleAdTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            try
            {
                if (item is Breach breach)
                {
                    switch (breach.Id)
                    {
                        case "AD":
                            return MsftAdTemplate;
                        case "VUNGLE":
                            return VungleAdTemplate;
                        default:
                            return BreachItemTemplate;
                    }
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
