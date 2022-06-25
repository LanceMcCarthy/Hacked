# Hacked 

An application so you can be notified of any data breaches that included your personal information. Available on Desktop (Windows 10 and Mac), Mobile (iOS, Android and iPad) and others (Xbox, Samsung TV).

![image](https://user-images.githubusercontent.com/3520532/103462956-e4d8a080-4cf6-11eb-8779-ed3cbf600977.png)

## Installation Options

* [Microsoft Store](https://www.microsoft.com/store/productId/9NBLGGH6850J)
* [Prerelease (appinstaller)](https://dvlup.blob.core.windows.net/hacked-app-files/distributions/uwp-drop/index.html) - 

> Microsoft has changed the behavior of appinstaller webpage, see [these instructions](#appinstaller) below.

## Builds

### GitHub Actions

| Branch | Status |
|--------|--------|
| `main` | [![Build Common](https://github.com/LanceMcCarthy/Hacked/actions/workflows/build-common.yml/badge.svg)](https://github.com/LanceMcCarthy/Hacked/actions/workflows/build-common.yml) |
| `main` | [![Build .NET MAUI](https://github.com/LanceMcCarthy/Hacked/actions/workflows/build-maui.yml/badge.svg)](https://github.com/LanceMcCarthy/Hacked/actions/workflows/build-maui.yml) |
| `main` | [![Build UWP](https://github.com/LanceMcCarthy/Hacked/actions/workflows/build-uwp.yml/badge.svg)](https://github.com/LanceMcCarthy/Hacked/actions/workflows/build-uwp.yml) |

### Azure DevOps

| Build Pipeline | Branch | Status |
|----------------|--------|--------|
| Build MAUI | `main` | [![build maui](https://dev.azure.com/lance/Hacked/_apis/build/status/Build%20MAUI)](https://dev.azure.com/lance/Hacked/_build/latest?definitionId=75) |
| UWP Main | `main` | [![uwp main](https://dev.azure.com/lance/Hacked/_apis/build/status/UWP%20Main)](https://dev.azure.com/lance/Hacked/_build/latest?definitionId=49) |
| UWP Beta | `release-preview` | [![uwp beta](https://dev.azure.com/lance/Hacked/_apis/build/status/UWP%20Beta)](https://dev.azure.com/lance/Hacked/_build/latest?definitionId=63) |
| UWP Release | `release` | [![uwp release](https://dev.azure.com/lance/Hacked/_apis/build/status/UWP%20Release)](https://dev.azure.com/lance/Hacked/_build/latest?definitionId=48) |

| Release Pipeline | Status |
|------------------|--------|
| UWP Prerelease      | ![Prerelease badge](https://vsrm.dev.azure.com/lance/_apis/public/Release/badge/162ec65c-f681-4f5b-9aca-227480581bf5/2/2) |
| UWP Microsoft Store | ![Microsoft Store badge](https://vsrm.dev.azure.com/lance/_apis/public/Release/badge/162ec65c-f681-4f5b-9aca-227480581bf5/1/1) |

## AppInstaller

Microsoft disabled the appinstaller protocol, so the "Get this app" button doesn't work. However, you can do it yourself with just one additional step.

1. Save the AppInstaller file to your machine (this is what the 'Get this app' button used to do for you)
  1. Go to [the download site](https://dvlup.blob.core.windows.net/hacked-app-files/distributions/uwp-drop/index.html)
  2. Expand the "Additonal Links" section
  3. Right-click on the "App Installer file" link and select "Save link as..." to save it locally
      - ![image](https://user-images.githubusercontent.com/3520532/175792228-29bf61bc-9213-4aa4-bc61-f0de7eef5366.png)
  4. Save the file somewhere, the default **Downloads** folder is a good place.
2. Double-click on the downloaded `Hacked.appinstaller` file, this will run the installer.
  ![image](https://user-images.githubusercontent.com/3520532/175792230-7f310d46-cff4-4c81-ab1b-b09402df6395.png)

> If you want to install a newer version before the automatic update happens, you can just repeat the same steps to force the update.
