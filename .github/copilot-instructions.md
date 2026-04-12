# Hacked — Copilot Instructions

## What This Project Is

"Hacked?" is a multi-platform data breach notification app that queries the [HaveIBeenPwned (HIBP) v3 API](https://haveibeenpwned.com/api/v3/) to alert users when their email addresses appear in known data breaches or pastes.

## Repository Layout

There are **three distinct UI implementations** plus shared library projects under `src/`:

| Project | Type | Target |
|---------|------|--------|
| `src/Hacked` | UWP (legacy .csproj) | Windows 10/11 (Store) |
| `src/Hacked.Maui` | .NET MAUI | Android, iOS, macCatalyst, Windows (net10.0) |
| `src/Uno/Hacked` | Uno Platform | Android, iOS, macCatalyst, Windows, WASM, Desktop (net8.0) |
| `src/Hacked.Core` | netstandard2.0 | Shared models & constants |
| `src/Hacked.Services` | netstandard2.0 | HIBP API service layer |
| `src/Hacked.BackgroundTasks` | UWP component | Windows background monitoring |

### Solution files
- `src/Hacked_Uwp.sln` — UWP app + shared libs
- `src/Hacked_Maui.sln` — MAUI app + shared libs
- `src/Uno/Hacked_Uno.sln` — Uno app + tests

## Build Commands

All restores must use the custom NuGet config (requires `TELERIK_NUGET_KEY` env var):

```sh
# Restore any project
dotnet restore <project.csproj> --configfile src/nuget.config

# Build shared libs (Core + Services)
dotnet build src/Hacked.Services/Hacked.Services.csproj -c Debug --no-restore

# Build MAUI (Windows target)
dotnet build src/Hacked.Maui/Hacked.Maui.csproj -c Debug -f net10.0-windows10.0.19041.0

# Build MAUI (Android target)
dotnet build src/Hacked.Maui/Hacked.Maui.csproj -c Debug -f net10.0-android

# Build UWP (requires msbuild, not dotnet build)
msbuild src/Hacked_Uwp.sln /t:Restore /p:Configuration=Debug /p:Platform=x64 /p:UapAppxPackageBuildMode=CI /p:AppxBundle=Never /p:AppxPackageSigningEnabled=False
```

## Running Tests

Tests live in `src/Uno/Hacked.Tests` (NUnit + FluentAssertions, net8.0):

```sh
# Run all tests
dotnet test src/Uno/Hacked.Tests/Hacked.Tests.csproj

# Run a single test by name filter
dotnet test src/Uno/Hacked.Tests/Hacked.Tests.csproj --filter "FullyQualifiedName~MyTestName"
```

## NuGet Configuration

`src/nuget.config` configures two feeds:
- **nuget.org** — standard packages
- **Telerik_v3_Feed** — Telerik packages (UI controls for UWP and MAUI)

The Telerik feed requires the `TELERIK_NUGET_KEY` environment variable. Without it, restores will fail for any project referencing `Telerik.*` packages.

## Architecture

### Shared Layer (`Hacked.Core` + `Hacked.Services`)

- **`Hacked.Core`** holds all models (`Breach`, `MonitoredAccount`, `Paste`, `User`, etc.), constants (`HibpConstants`), and `Secrets.cs` (API key for development).
- **`Hacked.Services`** implements the HIBP API calls behind interfaces in `Hacked.Services.Interfaces` (`IPwndBreachService`, `IPwndPasswordService`, `IAccountsService`). Services are injected into all UI layers via DI.

### HIBP API Behavior

- HTTP **404** from HIBP means *no breaches found* — it is a valid, non-error response.
- HTTP **429** (rate limit) is handled with `RetryAfter` header back-off; the services retry automatically up to 50 times.
- The `truncateResponse` query parameter **must be lowercase** (`"true"` / `"false"` strings) — HIBP/Cloudflare does not accept `"True"` (see comment in `BeenPwnedService.cs`).

### MAUI App (`src/Hacked.Maui`)

- Uses `MauiProgram.cs` for DI registration (services as singletons, views/viewmodels as singletons or transient).
- MVVM via **CommunityToolkit.Mvvm** (`ObservableObject`, `[ObservableProperty]`, `AsyncRelayCommand`).
- Platform-specific lifecycle code uses `#if` preprocessor constants (e.g., `WINDOWS10_0_17763_0_OR_GREATER`, `MACCATALYST`, `ANDROID`).
- Telerik UI for MAUI components — call `.UseTelerik()` in builder.

### UWP App (`src/Hacked`)

- The UWP `MainPage` is split across four partial-class files: `MainPage.xaml.cs`, `MainPage_Filtering.cs`, `MainPage_Methods.cs`, `MainPage_EventHandlers.cs`, `MainPage_Background.cs`.
- Background tasks are in the separate `Hacked.BackgroundTasks` project and registered via `Package.appxmanifest`.
- Uses **Windows Community Toolkit** (v7.x) and **Telerik UI for UWP**.

### Uno Platform App (`src/Uno/Hacked`)

- Uses `Uno.Sdk` single-project targeting net8.0 across all platforms.
- Features enabled via `<UnoFeatures>` in the csproj (Material, Mvvm, Navigation, Hosting, etc.).
- Configuration loaded from `appsettings.json` / `appsettings.development.json`.

## Key Conventions

- **C# LangVersion 10** is set in all projects.
- **`Secrets.cs`** (`Hacked.Core/Common/Secrets.cs`) holds the dev HIBP API key hardcoded. Do not rotate this in code — use environment injection for CI/production.
- **`HibpConstants.cs`** is the single source of truth for all HIBP API routes and header names.
- Telerik UI components are used in both the UWP and MAUI apps. UWP uses `Telerik.UI.for.UniversalWindowsPlatform`; MAUI uses `Telerik.UI.for.Maui`.
- The Uno project is a separate, more experimental implementation and does not share UI code with UWP or MAUI.
