# Hacked 

An application so you can be notified of any data breaches that included your personal information. Available on Desktop (Windows 10 and Mac), Mobile (iOS, Android and iPad) and others (Xbox, Samsung TV).

![image](https://user-images.githubusercontent.com/3520532/103462956-e4d8a080-4cf6-11eb-8779-ed3cbf600977.png)

## Installation Options

* [Prelease download](https://dvlup.blob.core.windows.net/hacked-app-files/distributions/uwp-drop/index.html) (sideload w/automatic updates)
* [Microsoft Store](https://www.microsoft.com/store/productId/9NBLGGH6850J)

### GitHub Actions

| Branch | Status |
|--------|--------|
| `main` | [![Build .NET MAUI](https://github.com/LanceMcCarthy/Hacked/actions/workflows/build-maui.yml/badge.svg)](https://github.com/LanceMcCarthy/Hacked/actions/workflows/build-maui.yml) |
| `main` | [![Build UWP](https://github.com/LanceMcCarthy/Hacked/actions/workflows/build-uwp.yml/badge.svg)](https://github.com/LanceMcCarthy/Hacked/actions/workflows/build-uwp.yml) |

### Azure DevOps Build Pipeline

| Pipeline | Branch | Status      |
|----------|--------|-------------|
| Build MAUI | `main` | [![build maui](https://dev.azure.com/lance/Hacked/_apis/build/status/Build%20MAUI)](https://dev.azure.com/lance/Hacked/_build/latest?definitionId=75) |
| UWP Main | `main` | [![uwp main](https://dev.azure.com/lance/Hacked/_apis/build/status/UWP%20Main)](https://dev.azure.com/lance/Hacked/_build/latest?definitionId=49) |
| UWP Beta | `release-preview` | [![uwp beta](https://dev.azure.com/lance/Hacked/_apis/build/status/UWP%20Beta)](https://dev.azure.com/lance/Hacked/_build/latest?definitionId=63) |
| UWP Release | `release` | [![uwp release](https://dev.azure.com/lance/Hacked/_apis/build/status/UWP%20Release)](https://dev.azure.com/lance/Hacked/_build/latest?definitionId=48) |

### Azure DevOps Release Pipeline

| Pipeline        | Status         |
|-----------------|----------------|
| UWP Prerelease      | ![Prerelease badge](https://vsrm.dev.azure.com/lance/_apis/public/Release/badge/162ec65c-f681-4f5b-9aca-227480581bf5/2/2) |
| UWP Microsoft Store | ![Microsoft Store badge](https://vsrm.dev.azure.com/lance/_apis/public/Release/badge/162ec65c-f681-4f5b-9aca-227480581bf5/1/1) |
