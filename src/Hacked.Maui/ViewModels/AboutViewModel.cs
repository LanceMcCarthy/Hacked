using System;
using System.Windows.Input;
using CommonHelpers.Common;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;

namespace Hacked.Maui.ViewModels;

public class AboutViewModel : ViewModelBase
{
    public AboutViewModel()
    {
        Title = "About";
        OpenWebCommand = new Command(async()=> await Launcher.OpenAsync(new Uri("https://xamarin.com/platform")));
    }

    public ICommand OpenWebCommand { get; }
}