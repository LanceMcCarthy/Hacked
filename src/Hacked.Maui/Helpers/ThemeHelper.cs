using Hacked.Maui.Common;

namespace Hacked.Maui.Helpers;

public static class ThemeHelper
{
    private static readonly string[] _availableThemes = ["Platform", "Main", "OceanBlue", "Purple", "Turquoise"];

    public static IReadOnlyList<string> AvailableThemes => _availableThemes;

    public static string NormalizeTheme(string? themeName)
    {
        if (string.IsNullOrWhiteSpace(themeName))
        {
            return Settings.DefaultTheme;
        }

        foreach (var option in _availableThemes)
        {
            if (string.Equals(option, themeName, StringComparison.OrdinalIgnoreCase))
            {
                return option;
            }
        }

        return Settings.DefaultTheme;
    }

    public static void ApplyTheme(string? selectedTheme, AppTheme requestedTheme)
    {
        var normalizedTheme = NormalizeTheme(selectedTheme);
        TelerikThemeResources.AppTheme = ResolveTelerikTheme(normalizedTheme, requestedTheme);
        Settings.SelectedTheme = normalizedTheme;
    }

    public static void ApplySavedTheme(AppTheme requestedTheme)
    {
        ApplyTheme(Settings.SelectedTheme, requestedTheme);
    }

    public static TelerikTheme ResolveTelerikTheme(string selectedTheme, AppTheme requestedTheme)
    {
        var normalizedTheme = NormalizeTheme(selectedTheme);
        var isDarkMode = requestedTheme == AppTheme.Dark;

        return normalizedTheme switch
        {
            "Platform" => isDarkMode ? TelerikTheme.PlatformDark : TelerikTheme.PlatformLight,
            "Main" => isDarkMode ? TelerikTheme.TelerikMainDark : TelerikTheme.TelerikMain,
            "OceanBlue" => isDarkMode ? TelerikTheme.TelerikOceanBlueDark : TelerikTheme.TelerikOceanBlue,
            "Purple" => isDarkMode ? TelerikTheme.TelerikPurpleDark : TelerikTheme.TelerikPurple,
            _ => isDarkMode ? TelerikTheme.TelerikTurquoiseDark : TelerikTheme.TelerikTurquoise
        };
    }
}