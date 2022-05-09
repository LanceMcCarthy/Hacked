using Hacked.Maui.Common;
using System.Diagnostics;

namespace Hacked.Maui.Helpers;

public static class ThemeHelper
{
    public static void SetTheme(string themeName)
    {
        try
        {
            switch (themeName)
            {
                case "Blue":
                    SetBlueTheme();
                    break;
                case "Green":
                    SetGreenTheme();
                    break;
                case "Red":
                    SetRedTheme();
                    break;
                default:
                    SetBlueTheme();
                    break;
            }

            Settings.SelectedTheme = themeName;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ThemeHelper SetTheme Exception: {ex}");
        }
    }

    public static void LoadTheme()
    {
        SetTheme(Settings.SelectedTheme);
    }

    // https://www.colorbox.io/#steps=7#hue_start=173#hue_end=220#hue_curve=easeInQuad#sat_start=4#sat_end=90#sat_curve=easeOutQuad#sat_rate=130#lum_start=100#lum_end=53#lum_curve=easeOutQuad
        
    private static void SetBlueTheme()
    {
        if (Application.Current == null)
            return;

        Application.Current.Resources["ThemeBackgroundColor"] = Color.FromArgb("#f2fffd");
        Application.Current.Resources["ThemeTextLightColor"] = Color.FromArgb("#a5a6a6");
        Application.Current.Resources["ThemeTextColor"] = Color.FromArgb("#6d6d6d");
        Application.Current.Resources["ThemeTextDarkColor"] = Color.FromArgb("#2b2b2b");
        Application.Current.Resources["ThemeAccentBrightColor"] = Color.FromArgb("#13d2e4");
        Application.Current.Resources["ThemeAccentLightColor"] = Color.FromArgb("#009acd");
        Application.Current.Resources["ThemeAccentColor"] = Color.FromArgb("#005dac");
        Application.Current.Resources["ThemeAccentDarkColor"] = Color.FromArgb("#002d87");
    }

    private static void SetRedTheme()
    {
        if (Application.Current == null)
            return;

        Application.Current.Resources["ThemeBackgroundColor"] = Color.FromArgb("#fff2f2");
        Application.Current.Resources["ThemeTextLightColor"] = Color.FromArgb("#a5a6a6");
        Application.Current.Resources["ThemeTextColor"] = Color.FromArgb("#6d6d6d");
        Application.Current.Resources["ThemeTextDarkColor"] = Color.FromArgb("#2b2b2b");
        Application.Current.Resources["ThemeAccentBrightColor"] = Color.FromArgb("#fb9897");
        Application.Current.Resources["ThemeAccentLightColor"] = Color.FromArgb("#cd1900");
        Application.Current.Resources["ThemeAccentColor"] = Color.FromArgb("#ac2200");
        Application.Current.Resources["ThemeAccentDarkColor"] = Color.FromArgb("#872400");
    }

    private static void SetGreenTheme()
    {
        if (Application.Current == null)
            return;

        Application.Current.Resources["ThemeBackgroundColor"] = Color.FromArgb("#fff2f2");
        Application.Current.Resources["ThemeTextLightColor"] = Color.FromArgb("#a5a6a6");
        Application.Current.Resources["ThemeTextColor"] = Color.FromArgb("#6d6d6d");
        Application.Current.Resources["ThemeTextDarkColor"] = Color.FromArgb("#2b2b2b");
        Application.Current.Resources["ThemeAccentBrightColor"] = Color.FromArgb("#97fb99");
        Application.Current.Resources["ThemeAccentLightColor"] = Color.FromArgb("#00cd3b");
        Application.Current.Resources["ThemeAccentColor"] = Color.FromArgb("#00ac4e");
        Application.Current.Resources["ThemeAccentDarkColor"] = Color.FromArgb("#008753");
    }
}