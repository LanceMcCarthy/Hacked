namespace Hacked.Core.Common;

public static class Constants
{
    // Filenames
    public const string LocalAccountsFileName = "AccountsJsonData.txt";
    public const string RoamingAccountsBackupFileName = "RoamingAccountsJsonData.txt";

    // Settings Keys
    public const string AreAdsRemovedSettingsKey = "AreAdsRemoved";
    public const string MonitoringStatusTaskSettingsKey = "MonitoringTaskStatus";
    public const string RefreshDelaySettingsKey = "RefreshDelay";

    // Background task names
    public const string MonitorTaskName = "MonitorTask";

    //general keys
    public const string IsFirstLaunchKey = "IsFirstLaunch";
    public const string SelectedThemeKey = "SelectedThemeKey";

    //Tutorial tip keys
    public const string SwipeTipShownKey = "SwipeTipShown";
    public const string AccountsRefreshTipShownKey = "AccountsRefreshTipShown";
    public const string AddAccountTipShownKey = "AddAccountTipShownShown";

}
