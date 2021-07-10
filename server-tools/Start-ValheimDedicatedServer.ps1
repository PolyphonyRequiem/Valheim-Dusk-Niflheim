param (
    [Parameter(Mandatory = $true)]
    [string]$WorldId,
    [Parameter(Mandatory = $true)]
    [string]$ValheimWorldsConfig
)

$env:SteamAppId=892970
Write-Host "Opening WorldsConfig at $ValheimWorldsConfig"
if (-not (Test-Path $ValheimWorldsConfig))
{
    throw "The Valheim World Configuration Source .\$ValheimWorldsConfig does not exist"
}

$config = Import-PowerShellDataFile $ValheimWorldsConfig

if (-not $config.ContainsKey($WorldId))
{
    throw "The worldId $WorldId was not found in the configuration source provided by $ValheimWorldsConfig"
}

$valheimServerExe = Join-Path (Join-Path (Split-Path $ValheimWorldsConfig) "$($config[$WorldId].InstallPath)") "valheim_server.exe"

if (-not (Test-Path $valheimServerExe))
{
    throw "Unable to find $valheimServerExe"
}

$argumentList = @('-no graphics', '-batchmode')
$argumentList += "-name `"$($config[$WorldId].ServerPublicName)`""
$argumentList += "-port $($config[$WorldId].Port)"
$argumentList += "-world `"$($config[$WorldId].WorldDbName)`""
$argumentList += "-password `"$($config[$WorldId].Password)`""

Write-Host "Starting $valheimServerExe with arguments $($argumentList -join ' ')"

Start-Process -FilePath $valheimServerExe -ArgumentList $argumentList
