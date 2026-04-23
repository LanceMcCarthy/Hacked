using CommonHelpers.Common;
using CommonHelpers.Mvvm;

namespace Hacked.Maui.ViewModels;

public class AboutViewModel : ViewModelBase
{
    public AboutViewModel()
    {
        Title = "About";
        OpenWebCommand = new DelegateCommand(async()=> await Launcher.OpenAsync(new Uri("https://www.telerik.com/maui-ui")));
        AppVersion = AppInfo.Current.VersionString;
    }

    public DelegateCommand OpenWebCommand { get; }

    public string AppVersion { get; }
}