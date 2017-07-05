param (
    [string]$cfg = "Release"
)
$invocation = (Get-Variable MyInvocation).Value
$invocationDir = Split-Path $invocation.MyCommand.Path
$packagesDir = "C:\Packages\"

$flt = "\\" + $cfg + "\\"
Get-ChildItem -Path $invocationDir -Filter *.nupkg -Recurse | where FullName -match $flt | Copy-Item -Destination $packagesDir