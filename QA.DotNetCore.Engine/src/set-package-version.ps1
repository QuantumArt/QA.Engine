param (
    [Parameter(Mandatory = $true)]
    [string]$newVersion,
    [string]$suffix
)
$invocation = (Get-Variable MyInvocation).Value
$srcDir = Split-Path $invocation.MyCommand.Path

$csprojFiles = Get-ChildItem -Path $srcDir -Filter QA.DotNetCore.*.csproj -File -Recurse
foreach($csprojFile in $csprojFiles)
{
    $csprojFilePath = $csprojFile.FullName

    [xml]$xml = Get-Content $csprojFilePath
    $ns = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
    $ns.AddNamespace("ns", $xml.DocumentElement.NamespaceURI)

    $VersionNode = $xml.SelectSingleNode("//Version", $ns)
    $AssemblyVersionNode = $xml.SelectSingleNode("//AssemblyVersion", $ns)
    $FileVersionNode = $xml.SelectSingleNode("//FileVersion", $ns)

    if($VersionNode -and $AssemblyVersionNode -and $FileVersionNode)
    {
        $VersionNode.InnerText = $newVersion
        $AssemblyVersionNode.InnerText = $newVersion + ".0"
        $FileVersionNode.InnerText = $newVersion + ".0"
        if ($suffix) {
            $VersionNode.InnerText += "-" + $suffix
            Write-Host "Package version $newVersion-$suffix"
        }
        $xml.Save($csprojFilePath)
        Write-Host "File $csprojFile updated to $newVersion"
    }
}