using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Services.Store;

namespace Hacked.Helpers;

public static class StoreHelpers
{
    public static async Task<bool> PurchaseAsync(string productId)
    {
        var purchaseStatus = false;

        try
        {
            var context = StoreContext.GetDefault();

            var result = await context.RequestPurchaseAsync(productId);

            Analytics.TrackEvent("PurchaseAdUnlockAsync", new Dictionary<string, string>()
                {
                    {"Purchase Result", result.Status.ToString("G")}
                });

            switch (result.Status)
            {
                case StorePurchaseStatus.Succeeded:
                case StorePurchaseStatus.AlreadyPurchased:
                    purchaseStatus = true;
                    break;
                case StorePurchaseStatus.NotPurchased:
                case StorePurchaseStatus.NetworkError:
                case StorePurchaseStatus.ServerError:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception ex)
        {
            DisplayMessageHelpers.ShowExceptionMessageOnUiThread($"PurchaseAsync_{productId}", ex);
            purchaseStatus = false;
        }

        return purchaseStatus;
    }
}
