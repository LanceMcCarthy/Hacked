namespace Hacked.Services;

public interface ISettingsService
{
    DateTime? LastBackgroundCheck { get; set; }
    string AppVersion { get; set; }
    bool NotificationsEnabled { get; set; }
}
