using System.IO;
using System.Text.Json;

namespace Hacked.Services;

public class SettingsService : ISettingsService
{
    private static readonly JsonSerializerOptions s_jsonOptions = new() { WriteIndented = true };

    private static string DataFilePath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LancelotSoftware", "Hacked", "settings.json");

    private SettingsData _data = new();

    public SettingsService()
    {
        Load();
    }

    public DateTime? LastBackgroundCheck
    {
        get => _data.LastBackgroundCheck;
        set { _data.LastBackgroundCheck = value; Save(); }
    }

    public string AppVersion
    {
        get => _data.AppVersion;
        set { _data.AppVersion = value; Save(); }
    }

    public bool NotificationsEnabled
    {
        get => _data.NotificationsEnabled;
        set { _data.NotificationsEnabled = value; Save(); }
    }

    private void Load()
    {
        try
        {
            var path = DataFilePath;
            if (!File.Exists(path)) return;
            var json = File.ReadAllText(path);
            _data = JsonSerializer.Deserialize<SettingsData>(json, s_jsonOptions) ?? new();
        }
        catch { _data = new(); }
    }

    private void Save()
    {
        try
        {
            var path = DataFilePath;
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, JsonSerializer.Serialize(_data, s_jsonOptions));
        }
        catch { }
    }

    private class SettingsData
    {
        public DateTime? LastBackgroundCheck { get; set; }
        public string AppVersion { get; set; } = "1.0";
        public bool NotificationsEnabled { get; set; } = true;
    }
}
