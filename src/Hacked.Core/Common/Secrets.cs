using System;
using System.Collections.Generic;
using System.Text;

namespace Hacked.Core.Common
{
    public static class Secrets
    {
        // IAPs

        /// <summary>
        /// This is only for purchases or queries prior to 1607
        /// it is the readable product name, or product ID in the store
        /// </summary>
        public const string RemoveAdsProductId = "RemoveAds";

        /// <summary>
        /// Use this for AddOns when SDK is >= 1607
        /// </summary>
        public const string RemoveAdsStoreId = "9pjt6pdgp7tg";

        public static string HibpApiKey = "eed3414c0e504b7ba3a39fd63a1ce26d";
    }
}
