param([string]$workingDirectory = $PSScriptRoot)

Write-Host 'Executing appcenter-pre-build.ps1. Working Directory:' $workingDirectory
Set-Location $workingDirectory

Write-Host 'Updating UWP Package Manifest'
$manifestFileName =  $workingDirectory + '/Hacked/Package.appxmanifest'

# Create version number using Year.DayOfYear.MinutesInDay (ex. 2020.225.65)
$now = Get-Date
$versionMajor = $now.Year
$versionMinor = $now.DayOfYear
$versionBuild = ($now.Hour * 60) + $now.Minute

$content = (Get-Content $manifestFileName) -join "`r`n"

$callback = {
  param($match)
    # Keeping the same revision number (never change revision, only Microsoft changes revision number for emergency Store re-compilations)
    [string]$versionRevision = $match.Groups[5].Value

    # Set the version number (x.x.x.0)
    $match.Groups[1].Value + 'Version="' + $versionMajor + '.' + $versionMinor + '.' + $versionBuild + '.' + $versionRevision + '"'
}

# match and replace with Regex
$identityRegex = [regex]'(\<Identity[^\>]*)Version=\"([0-9])+\.([0-9]+)\.([0-9]+)\.([0-9]+)\.*\"'
$newContent = $identityRegex.Replace($content, $callback)

# Write the new version to the manifest
Write-Host 'Saving new version number: ' + $versionMajor + '.' + $versionMinor + '.' + $versionBuild
[io.file]::WriteAllText($manifestFileName, $newContent)

Write-Host 'appcenter-pre-build.ps1 done!'