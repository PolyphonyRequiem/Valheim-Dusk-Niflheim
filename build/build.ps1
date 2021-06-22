param (
    [switch]$Debug = $false,
    [string]$Version = "",
    [string]$NexusKey = $null
)
if ($NexusKey -eq $null)
{
    throw "No Nexus Key."
}
$ErrorActionPreference = "Stop"

$scriptRoot = if ($PSScriptRoot) {$PSScriptRoot} else {".\"}
$root = Join-Path  $scriptRoot ..\
$mod_root = Join-Path $root .\mod_root

$out = Join-Path $root .\out

if (-not (Test-Path $mod_root))
{
    throw "Unable to locate mod_root at $mod_root"
}

if (Test-Path $out) 
{
    Write-Host "Found existing output in $out... deleting..."

    Remove-Item -Path $out -Recurse 
}

Write-Host "Creating output directory..."
 
New-Item -Path $out -ItemType Directory

# Write-Host "Placing modbase..."
# Copy-Item $modbase\* $out\ -Force -Recurse
# Write-Host "Placing modbins..."
# Copy-Item $modbins\* $out -Force -Recurse
Write-Host "Fetching Mod Binaries..."
dotnet run --project $root\src\packager\Niflheim.Packager.csproj $mod_root\manifest.json $NexusKey  $out $root\downloadedarchives

Write-Host "Placing mod_root..."
Copy-Item $mod_root\* $out -Force -Recurse

if (-not $Debug)
{
    Write-Host "Stripping out debugging tools..."

    Remove-Item -Path (Join-Path $out "\BepInEx\plugins\Valheim.WhereAmI.dll")
    Remove-Item -Path (Join-Path $out "\BepInEx\plugins\SkToolboxValheim.dll")
    Remove-Item -Path (Join-Path $out "\BepInEx\plugins\UnityExplorer.BIE5.Mono.dll")
    Remove-Item -Path (Join-Path $out "\BepInEx\plugins\ConfigurationManager\ConfigurationManager.dll")
}

Write-Host "Packaging"

if ($Debug -and $Version)
{
    $Version = "$Version-debug"
}

$archiveName = if ($Version){"Niflheim-$Version.zip"} else {"Niflheim.zip"}

Compress-Archive -Path $out\* -DestinationPath $out\$archiveName