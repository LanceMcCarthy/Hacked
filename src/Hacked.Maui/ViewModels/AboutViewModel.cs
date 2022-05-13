using CommonHelpers.Common;
using CommonHelpers.Mvvm;

namespace Hacked.Maui.ViewModels;

public class AboutViewModel : ViewModelBase
{
    public AboutViewModel()
    {
        Title = "About";
        OpenWebCommand = new DelegateCommand(async()=> await Launcher.OpenAsync(new Uri("https://telerik.com")));
    }

    public DelegateCommand OpenWebCommand { get; }
}