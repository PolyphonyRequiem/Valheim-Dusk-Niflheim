param (
    [switch]$Debug = $false,
    [switch]$Package = $false,
    [string]$Version = "",
    [string]$NexusKey = $null,
    [switch]$ConfigOnly = $false
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

if ((Test-Path $out) -and (-not $ConfigOnly)) 
{
    Write-Host "Found existing output in $out... "

    Remove-Item -Path $out -Recurse 
    
    Write-Host "Creating output directory..."
 
    New-Item -Path $out -ItemType Directory
}
elseif (-not (Test-Path $out))
{
    Write-Host "Creating output directory..."
 
    New-Item -Path $out -ItemType Directory
}

if (-not $ConfigOnly)
{
    Write-Host "Fetching Mod Binaries..."
    dotnet run --project $root/src/packager/Niflheim.Packager.csproj $mod_root/manifest.json $NexusKey  $out $root/downloadedarchives ($Debug.ToString())

    If ($lastExitCode -ne "0") {
        throw "Package download failed.  This happens a lot, stay cool, wait a moment, then run the build again.  Both Nexus and thunderstore have flakey content delivery networks."
    }

    # dotnet restore $root/src/Niflheim.sln

    # If ($lastExitCode -ne "0") {
    #     throw "dotnet restore failed.  This is rare."
    # }

    # nuget restore $root/src/Niflheim.sln

    # If ($lastExitCode -ne "0") {
    #     throw "NugetRestore failed.  This is rare"
    # }

    # dotnet build $root/src/Niflheim.sln -property:Configuration=Release

    # If ($lastExitCode -ne "0") {
    #     throw "Build failed.  This is rare"
    # }

    # Copy-Item $root/src/PatchNotesExtender/bin/Release/PatchNotesExtender.dll -Destination $out/Bepinex/plugins/PatchNotesExtender.dll

    # Copy-Item $root/src/NiflheimBespoke/bin/Release/NiflheimBespoke.dll -Destination $out/Bepinex/plugins/NiflheimBespoke.dll
}


Write-Host "Placing mod_root..."
Copy-Item $mod_root/* $out -Force -Recurse

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