# Hacked — Copilot Instructions

## What This Project Is

"Hacked?" is a multi-platform data breach notification app that queries the [HaveIBeenPwned (HIBP) v3 API](https://haveibeenpwned.com/api/v3/) to alert users when their email addresses appear in known data breaches or pastes.

## Repository Layout

There are **three distinct UI implementations** plus shared library projects under `src/`:

| Project | Type | Target |
|---------|------|--------|
| `src/Hacked` | UWP (legacy .csproj) | Windows 10/11 (Store) |
| `src/Hacked.Maui` | .NET MAUI | Android, iOS, macCatalyst, Windows (net10.0) |
| `src/Hacked.Uno/Hacked` | Uno Platform | Android, iOS, macCatalyst, Windows, WASM, Desktop (net9.0) |
| `src/Hacked.Core` | netstandard2.0 | Shared models & constants |
| `src/Hacked.Services` | netstandard2.0 | HIBP API service layer |
| `src/Hacked.BackgroundTasks` | UWP component | Windows background monitoring |

### Solution files
- `src/Hacked.slnx` — umbrella solution for the repo
- `src/Hacked_Maui.slnx` — MAUI app + shared libs
- `src/Hacked_Uno.slnx` — Uno app + tests
- `src/Hacked_Uwp.slnx` — UWP app + shared libs

## Build Commands

All restores must use the custom NuGet config (requires `TELERIK_NUGET_KEY` env var):

```sh
# Restore Uno project
dotnet restore src/Hacked.Uno/Hacked/Hacked.csproj --configfile src/nuget.config

# Build shared libs (Core + Services)
dotnet build src/Hacked.Services/Hacked.Services.csproj -c Debug --no-restore

# Build Uno (net9.0 desktop target — fastest for CI/dev)
dotnet build src/Hacked.Uno/Hacked/Hacked.csproj -c Debug -f net9.0

# Build Uno (Windows target)
dotnet build src/Hacked.Uno/Hacked/Hacked.csproj -c Debug -f net9.0-windows10.0.19041

# Build MAUI (Windows target)
dotnet build src/Hacked.Maui/Hacked.Maui.csproj -c Debug -f net10.0-windows10.0.19041.0

# Build MAUI (Android target)
dotnet build src/Hacked.Maui/Hacked.Maui.csproj -c Debug -f net10.0-android

# Build UWP project directly (legacy)
dotnet build src/Hacked/Hacked.csproj -c Debug
```

## Running Tests

Tests live in `src/Hacked.Uno/Hacked.Tests` (NUnit + FluentAssertions, net9.0):

```sh
# Run all tests
dotnet test src/Hacked.Uno/Hacked.Tests/Hacked.Tests.csproj --nologo

# Run a single test by name filter
dotnet test src/Hacked.Uno/Hacked.Tests/Hacked.Tests.csproj --filter "FullyQualifiedName~MyTestName"
```

## NuGet Configuration

`src/nuget.config` configures two feeds:
- **nuget.org** — standard packages
- **Telerik_v3_Feed** — Telerik packages (UI controls for UWP and MAUI)

The Telerik feed requires the `TELERIK_NUGET_KEY` environment variable. Without it, restores will fail for any project referencing `Telerik.*` packages.

The Uno project does NOT use Telerik packages; it can restore without the env var.

## Architecture

### Shared Layer (`Hacked.Core` + `Hacked.Services`)

- **`Hacked.Core`** holds all models (`Breach`, `MonitoredAccount`, `Paste`, `User`, etc.), constants (`HibpConstants`), and `Secrets.cs` (API key for development).
- **`Hacked.Services`** implements the HIBP API calls behind interfaces in `Hacked.Services.Interfaces` (`IPwndBreachService`, `IPwndPasswordService`, `IAccountsService`). Services are injected into all UI layers via DI.

### HIBP API Behavior

- HTTP **404** from HIBP means *no breaches found* — it is a valid, non-error response.
- HTTP **429** (rate limit) is handled with `RetryAfter` header back-off; the services retry automatically up to 50 times.
- The `truncateResponse` query parameter **must be lowercase** (`"true"` / `"false"` strings) — HIBP/Cloudflare does not accept `"True"` (see comment in `BeenPwnedService.cs`).

### Uno Platform App (`src/Hacked.Uno/Hacked`)

Uses **Uno SDK 6.5.31** single-project targeting `net9.0-*` across all platforms.

#### Project Structure

```
src/Hacked.Uno/Hacked/
  Views/           - Shell + 6 pages (MonitoredAccountsPage, AddAccountPage,
                     AccountDetailsPage, BreachDetailsPage, PasswordCheckPage, SettingsPage)
  ViewModels/      - One ViewModel per View (CommunityToolkit.Mvvm ObservableObject)
  Services/        - App-level service interfaces + implementations
  Converters/      - BoolToVisibilityConverter (supports Inverse property)
  Models/          - AppConfig (appsettings binding)
  Strings/         - Localization resources
  Styles/          - Material color overrides
  App.xaml.cs      - DI host, route registry (Uno.Extensions.Navigation)
  GlobalUsings.cs  - Project-wide global usings
```

#### Features enabled via `<UnoFeatures>` in Hacked.csproj
`Material`, `Dsp`, `Hosting`, `Toolkit`, `Logging`, `Mvvm`, `Configuration`, `Serialization`, `Localization`, `Navigation`, `ThemeService`

#### Services Layer (Uno app — `Hacked.Services` namespace)

| Interface | Implementation | Purpose |
|-----------|---------------|---------|
| `ISettingsService` | `SettingsService` | Persists app settings (notifications toggle, last check time) |
| `INotificationService` | `NotificationService` | Shows toast on Windows; logs on other platforms |
| `IBackgroundMonitorService` | `BackgroundMonitorService` | Periodic 30-min breach checks; also implements `IHostedService` |

`BackgroundMonitorService` is registered as both `IBackgroundMonitorService` and `IHostedService`:
```csharp
services.AddSingleton<BackgroundMonitorService>();
services.AddSingleton<IBackgroundMonitorService>(sp => sp.GetRequiredService<BackgroundMonitorService>());
services.AddHostedService(sp => sp.GetRequiredService<BackgroundMonitorService>());
```

#### MVVM Conventions
- ViewModels are `partial class` extending `ObservableObject` (CommunityToolkit.Mvvm)
- Use `[ObservableProperty]` for bindable fields (generates public property + change notification)
- Use `[RelayCommand]` for async commands
- Navigation via `INavigator.NavigateViewModelAsync<TVM>()` and `GoBack()`
- All common usings in `GlobalUsings.cs`

#### Platform-conditional code
Use `#if WINDOWS` for Windows-specific APIs (e.g., `Windows.UI.Notifications` for toasts).
Other platforms (`#else`) use logging stubs until native notification support is added.

### MAUI App (`src/Hacked.Maui`)

- Uses `MauiProgram.cs` for DI registration (services as singletons, views/viewmodels as singletons or transient).
- MVVM via **CommunityToolkit.Mvvm** (`ObservableObject`, `[ObservableProperty]`, `AsyncRelayCommand`).
- Platform-specific lifecycle code uses `#if` preprocessor constants (e.g., `WINDOWS10_0_17763_0_OR_GREATER`, `MACCATALYST`, `ANDROID`).
- Telerik UI for MAUI components — call `.UseTelerik()` in builder.

### UWP App (`src/Hacked`)

- The UWP `MainPage` is split across four partial-class files: `MainPage.xaml.cs`, `MainPage_Filtering.cs`, `MainPage_Methods.cs`, `MainPage_EventHandlers.cs`, `MainPage_Background.cs`.
- Background tasks are in the separate `Hacked.BackgroundTasks` project and registered via `Package.appxmanifest`.
- Uses **Windows Community Toolkit** (v7.x) and **Telerik UI for UWP**.

## Key Conventions

- **C# LangVersion 10** is set in `Hacked.Core` and `Hacked.Services` (netstandard2.0). The Uno project uses net9.0 and supports modern C# features.
- **netstandard2.0 constraints**: `Hacked.Core` and `Hacked.Services` must not use range operators, `File.ReadAllTextAsync`, or `string.Split(char, StringSplitOptions)`.
- **CPM (Central Package Management)**: All NuGet package versions go in `src/Hacked.Uno/Directory.Packages.props`. Never add `Version=` to `<PackageReference>` in `.csproj` files under `src/Hacked.Uno/`.
- **`Secrets.cs`** (`Hacked.Core/Common/Secrets.cs`) holds the dev HIBP API key hardcoded. Do not rotate this in code — use environment injection for CI/production.
- **`HibpConstants.cs`** is the single source of truth for all HIBP API routes and header names.
- Telerik UI components are used in both the UWP and MAUI apps. UWP uses `Telerik.UI.for.UniversalWindowsPlatform`; MAUI uses `Telerik.UI.for.Maui`. The Uno app does **not** use Telerik.

