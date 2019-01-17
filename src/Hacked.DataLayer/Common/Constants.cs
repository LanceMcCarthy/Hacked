namespace Hacked.DataLayer.Common
{
    public static class Constants
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

        
        // Filenames
        public const string LocalAccountsFileName = "AccountsJsonData.txt";
        public const string RoamingAccountsBackupFileName = "RoamingAccountsJsonData.txt";

        // Settings Keys
        public const string AreAdsRemovedSettingsKey = "AreAdsRemoved";
        public const string MonitoringStatusTaskSettingsKey = "MonitoringTaskStatus";

        // Background task names
        public const string MonitorTaskName = "MonitorTask";
    }
}
