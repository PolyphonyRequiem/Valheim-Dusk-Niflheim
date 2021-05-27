param (
    [string]$DeployPath
)

$ErrorActionPreference = "Stop"

$scriptRoot = if ($PSScriptRoot) {$PSScriptRoot} else {".\"}
$root = Join-Path  $scriptRoot ..\
$out = Join-Path $root .\out

if (-not (Test-Path $DeployPath))
{
    throw "Unable to locate $DeployPath"
}

if (Test-Path $out) 
{
    Write-Host "Found ouptut in $out... deploying..."
    $bepinexpath = (Join-Path -Path $DeployPath -ChildPath '.\BepInEx')
    if (Test-Path $bepinexpath)
    {
        Write-Host Deleting BepInEx at deploy site...  
        Remove-Item -Path $bepinexpath -Recurse -Force
    }

    Copy-Item -Path $out\* -Destination $DeployPath -Recurse -Force
}
else
{
    Write-Host "Couldn't find $out".
}
