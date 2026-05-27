using Hacked.Core.Common;

namespace Hacked.Maui.Common;

public static class Settings
{
    public const string DefaultTheme = "Turquoise";

    public static bool IsFirstLaunch
    {
        get => Preferences.Get(Constants.IsFirstLaunchKey, true);
        set => Preferences.Set(Constants.IsFirstLaunchKey, value);
    }

    public static string SelectedTheme
    {
        get => Preferences.Get(Constants.SelectedThemeKey, DefaultTheme);
        set => Preferences.Set(Constants.SelectedThemeKey, value);
    }

    public static bool SwipeTipShown
    {
        get => Preferences.Get(Constants.SwipeTipShownKey, false);
        set => Preferences.Set(Constants.SwipeTipShownKey, value);
    }

    public static bool AccountRefreshShown
    {
        get => Preferences.Get(Constants.AccountsRefreshTipShownKey, false);
        set => Preferences.Set(Constants.AccountsRefreshTipShownKey, value);
    }

    public static bool AddAccountTipShown
    {
        get => Preferences.Get(Constants.AddAccountTipShownKey, false);
        set => Preferences.Set(Constants.AddAccountTipShownKey, value);
    }
}