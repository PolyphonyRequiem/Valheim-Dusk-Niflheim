#Requires -RunAsAdministrator
function Start-ValheimDedicatedServer {
    <#
    .SYNOPSIS
    Starts a Valheim Dedicated Server configured for Steam.

    .DESCRIPTION
    This cmdlet downloads SteamCmd and configures it in a custom or
    predefined location (C:\Program Files\SteamCmd).

    .PARAMETER ValheimServerRootPath
    Specifies the install location of the ValheimServers.

    .EXAMPLE
    Update-SteamCmdGameValheimDedicatedServer -SteamCmdPath 'C:\SteamCmd'

    Installs SteamCmd in C:\SteamCmd.
    #>

    [CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'Medium')]
    param (
        [Parameter(Mandatory = $true)]
        [string]$WorldId
    )

    process {
        $ValheimServerRootPath=$PSScriptRoot
        $env:SteamAppId=892970
        $ValheimWorldsConfig = "$PSScriptRoot\Valheim-Worlds.psd1"
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

        $valheimServerExe = Join-Path (Join-Path "$ValheimServerRootPath" "$($config[$WorldId].InstallPath)") "valheim_server.exe"

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
    }
}
