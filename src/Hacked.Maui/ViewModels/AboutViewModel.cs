using CommonHelpers.Common;
using CommonHelpers.Mvvm;

namespace Hacked.Maui.ViewModels;

public class AboutViewModel : ViewModelBase
{
    public AboutViewModel()
    {
        Title = "About";
        OpenWebCommand = new DelegateCommand(async()=> await Launcher.OpenAsync(new Uri("https://xamarin.com/platform")));
    }

    public DelegateCommand OpenWebCommand { get; }
}