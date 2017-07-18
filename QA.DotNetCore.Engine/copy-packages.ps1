param (
    [string]$cfg = "Release"
)
$invocation = (Get-Variable MyInvocation).Value
$invocationDir = Split-Path $invocation.MyCommand.Path
$packagesDir = [System.IO.Path]::Combine($invocationDir, "Packages\$cfg")

if (!(Test-Path -PathType Container $packagesDir))
{
    Write-Host "Create '$packagesDir' directory"
    New-Item -ItemType Directory -Force -Path $packagesDir
}


Write-Host "Remove all packages from $packagesDir"
get-childItem $packagesDir -Filter "*.nupkg" | ForEach-Object { remove-item $_.FullName } 
$flt = "\\" + $cfg + "\\"

Write-Host "Create $packagesDir"
Get-ChildItem -Path $invocationDir -Filter *.nupkg -Recurse | where FullName -match $flt | Copy-Item -Destination $packagesDir