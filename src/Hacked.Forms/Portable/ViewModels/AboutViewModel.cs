using System;
using System.Windows.Input;
using CommonHelpers.Common;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel()
        {
            Title = "About";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
        }

        public ICommand OpenWebCommand { get; }
    }
}