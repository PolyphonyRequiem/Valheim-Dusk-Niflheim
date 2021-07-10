param (
    [switch]$Debug = $false,
    [switch]$Package = $false,
    [string]$Version = "",
    [string]$NexusKey = $null
)
if ($NexusKey -eq $null)
{
    throw "No Nexus Key."
}

$ErrorActionPreference = "Stop"

$scriptRoot = if ($PSScriptRoot) {$PSScriptRoot} else {"./"}
$root = Join-Path  $scriptRoot ../
$mod_root = Join-Path $root ./mod_root

$out = Join-Path $root ./out

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
dotnet run --project $root/src/packager/Niflheim.Packager.csproj $mod_root/manifest.json $NexusKey  $out $root/downloadedarchives

If ($lastExitCode -ne "0") {
    throw "Package download failed.  This happens a lot, stay cool, wait a moment, then run the build again.  Both Nexus and thunderstore have flakey content delivery networks."
}

dotnet restore $root/src/Niflheim.sln

If ($lastExitCode -ne "0") {
    throw "dotnet restore failed.  This is rare."
}

nuget restore $root/src/Niflheim.sln

If ($lastExitCode -ne "0") {
    throw "NugetRestore failed.  This is rare"
}

dotnet build $root/src/Niflheim.sln -property:Configuration=Release

If ($lastExitCode -ne "0") {
    throw "Build failed.  This is rare"
}

Copy-Item $root/src/PatchNotesExtender/bin/Release/PatchNotesExtender.dll -Destination $out/Bepinex/plugins/PatchNotesExtender.dll

Write-Host "Placing mod_root..."
Copy-Item $mod_root/* $out -Force -Recurse

if (-not $Debug)
{
    Write-Host "Stripping out debugging tools..."

    $DebugTools = @(
        (Join-Path $out "/BepInEx/plugins/Valheim.WhereAmI.dll"),
        (Join-Path $out "/BepInEx/plugins/SkToolboxValheim.dll"),
        (Join-Path $out "/BepInEx/plugins/UnityExplorer.BIE5.Mono.dll"),
        (Join-Path $out "/BepInEx/plugins/ConfigurationManager/ConfigurationManager.dll")
    )

    foreach ($item in $DebugTools) {
        if (Test-Path($item))
        {
            Remove-Item -Path $item
        }
    }    
}

Write-Host "Packaging"

if ($Debug -and $Version)
{
    $Version = "$Version-debug"
}

###$archiveName = if ($Version){"Niflheim-$Version.zip"} else {"Niflheim.zip"}
###Compress-Archive -Path $out\* -DestinationPath $out\$archiveName
if ($Package)
{
    Compress-Archive -Path $out/* -DestinationPath $out/Niflheim.zip
}