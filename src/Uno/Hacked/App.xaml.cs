using Hacked.Services.Apis;
using Hacked.Services.Interfaces;
using Hacked.ViewModels;
using Hacked.Views;
using Uno.Resizetizer;

namespace Hacked;
public partial class App : Application
{
    public App()
    {
        this.InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    logBuilder
                        .SetMinimumLevel(context.HostingEnvironment.IsDevelopment()
                            ? LogLevel.Information
                            : LogLevel.Warning)
                        .CoreLogLevel(LogLevel.Warning);
                }, enableUnoLogging: true)
                .UseConfiguration(configure: configBuilder => configBuilder
                    .EmbeddedSource<App>()
                    .Section<AppConfig>()
                )
                .UseLocalization()
                .UseSerialization()
                .ConfigureServices((context, services) =>
                {
                    // HIBP API services
                    services.AddSingleton<IPwndBreachService, BeenPwnedService>();
                    services.AddSingleton<IPwndPasswordService, PwnedPasswordService>();
                    services.AddSingleton<IAccountsService, AccountsService>();
                })
                .UseNavigation(RegisterRoutes)
            );
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.EnableHotReload();
#endif
        MainWindow.SetWindowIcon();

        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellViewModel))
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellViewModel>())
        );
    }
}

