using Hacked.Maui.Helpers;
using Hacked.Maui.Views;
using Microsoft.Maui.Controls;

namespace Hacked.Maui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new RootPage();
        }

        protected override void OnStart()
        {
            ThemeHelper.LoadTheme();
        }
    }
}
