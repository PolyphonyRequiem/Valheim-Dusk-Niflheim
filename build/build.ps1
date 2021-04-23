param (
    [switch]$Debug = $false,
    [string]$Version = ""
)

$ErrorActionPreference = "Stop"

$scriptRoot = if ($PSScriptRoot) {$PSScriptRoot} else {".\"}
$root = Join-Path  $scriptRoot ..\
$mod_root = Join-Path $root .\mod_root
$modbins = Join-Path $root .\modbins
$modbase = Join-Path $root .\modbase
$out = Join-Path $root .\out

if (-not (Test-Path $mod_root))
{
    throw "Unable to locate mod_root at $mod_root"
}
if (-not (Test-Path $modbins))
{
    throw "Unable to locate modbins at $modbins"
}
if (-not (Test-Path $modbase))
{
    throw "Unable to locate modbase at $modbase"
}

if (Test-Path $out) 
{
    Write-Host "Found existing output in $out... deleting..."

    Remove-Item -Path $out -Recurse 
}

Write-Host "Creating output directory..."
 
New-Item -Path $out -ItemType Directory

Write-Host "Placing modbase..."
Copy-Item $modbase\* $out\ -Force -Recurse
Write-Host "Placing modbins..."
Copy-Item $modbins\* $out -Force -Recurse
Write-Host "Placing mod_root..."
Copy-Item $mod_root\* $out -Force -Recurse

if (-not $Debug)
{
    Write-Host "Stripping out debugging tools..."

    Remove-Item -Path (Join-Path $out "\BepInEx\plugins\SkToolboxValheim.dll")
    Remove-Item -Path (Join-Path $out "\BepInEx\plugins\ConfigurationManager\ConfigurationManager.dll")
}

Write-Host "Packaging"

$archiveName = if ($Version){"Niflheim-$Version.zip"} else {"Niflheim.zip"}

Compress-Archive -Path $out -DestinationPath $out\$archiveName