using Hacked.Forms.Portable.Helpers;
using Hacked.Forms.Portable.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Hacked.Forms.Portable
{
    public partial class App : Application
    {
        public App()
        {
#if DEBUG
            LiveReload.Init();
#endif
            InitializeComponent();
            
            MainPage = new RootPage();
        }

        protected override void OnStart()
        {
            ThemeHelper.LoadTheme();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}