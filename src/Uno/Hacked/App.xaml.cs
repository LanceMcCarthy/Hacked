using Hacked.Models;
using Hacked.Services;
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
                    services.AddSingleton<ISettingsService, SettingsService>();

                    // Background monitoring + notifications
                    services.AddSingleton<INotificationService, NotificationService>();
                    services.AddSingleton<BackgroundMonitorService>();
                    services.AddSingleton<IBackgroundMonitorService>(sp => sp.GetRequiredService<BackgroundMonitorService>());
                    services.AddHostedService(sp => sp.GetRequiredService<BackgroundMonitorService>());
                })
                .UseNavigation(RegisterRoutes)
            );
        MainWindow = builder.Window;

#if DEBUG
        builder.Window.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellViewModel)),
            new ViewMap<MonitoredAccountsPage, MonitoredAccountsViewModel>(),
            new DataViewMap<AccountDetailsPage, AccountDetailsViewModel, MonitoredAccount>(),
            new DataViewMap<BreachDetailsPage, BreachDetailsViewModel, Breach>(),
            new ViewMap<AddAccountPage, AddAccountViewModel>(),
            new ViewMap<PasswordCheckPage, PasswordCheckViewModel>(),
            new ViewMap<SettingsPage, SettingsViewModel>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellViewModel>(),
                Nested: new RouteMap[]
                {
                    new RouteMap("Accounts", View: views.FindByViewModel<MonitoredAccountsViewModel>(), IsDefault: true),
                    new RouteMap("AddAccount", View: views.FindByViewModel<AddAccountViewModel>()),
                    new RouteMap("AccountDetails", View: views.FindByViewModel<AccountDetailsViewModel>()),
                    new RouteMap("BreachDetails", View: views.FindByViewModel<BreachDetailsViewModel>()),
                    new RouteMap("PasswordCheck", View: views.FindByViewModel<PasswordCheckViewModel>()),
                    new RouteMap("Settings", View: views.FindByViewModel<SettingsViewModel>()),
                }
            )
        );
    }
}

