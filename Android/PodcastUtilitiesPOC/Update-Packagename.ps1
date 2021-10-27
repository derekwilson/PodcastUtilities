param ([string] $ProjectDir, [string] $ConfigurationName)
Write-Host "ProjectDir: $ProjectDir"
Write-Host "ConfigurationName: $ConfigurationName"

$ManifestPath = $ProjectDir + "Properties\AndroidManifest.xml"

Write-Host "ManifestPath: $ManifestPath"

[xml] $xdoc = Get-Content $ManifestPath

$package = $xdoc.manifest.package

If ($ConfigurationName -eq "Release" -and $package.EndsWith(".debug")) 
{ 
    $package = $package.Replace(".debug", "") 
}
If ($ConfigurationName -eq "Debug" -and -not $package.EndsWith(".debug")) 
{ 
    $package = $package + ".debug" 
}

If ($package -ne $xdoc.manifest.package) 
{
    $xdoc.manifest.package = $package
    $xdoc.Save($ManifestPath)
    Write-Host "AndroidManifest.xml package name updated to $package"
}