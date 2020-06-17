param([string]$workingDirectory = $PSScriptRoot)

Write-Host 'Executing appcenter-pre-build.ps1. Working Directory:' $workingDirectory
Set-Location $workingDirectory

Write-Host 'Updating UWP Package Manifest'
$manifestFileName =  $workingDirectory + '/Hacked/Package.appxmanifest'

# Create version number
$major = Get-Date -Format "yyyy"
$minorRaw = Get-Date -Format "MMdd"
$minor = $minorRaw.TrimStart("0")
$revRaw = Get-Date -Format "HHmm"
$rev = $revRaw.TrimStart("0")

$verPrefix = $major + "." + $minor + "." + $rev

$content = (Get-Content $manifestFileName) -join "`r`n"

$callback = {
  param($match)
    # Keeping the same revision number (never change revision, only Microsoft changes revision number for emergency Store re-compilations)
    [string]$versionRevision = $match.Groups[5].Value

    # Set the version number (x.x.x.0)
    $match.Groups[1].Value + 'Version="' + $verPrefix + '.' + $versionRevision + '"'
}

# match and replace with Regex
$identityRegex = [regex]'(\<Identity[^\>]*)Version=\"([0-9])+\.([0-9]+)\.([0-9]+)\.([0-9]+)\.*\"'
$newContent = $identityRegex.Replace($content, $callback)

# Write the new version to the manifest
[io.file]::WriteAllText($manifestFileName, $newContent)