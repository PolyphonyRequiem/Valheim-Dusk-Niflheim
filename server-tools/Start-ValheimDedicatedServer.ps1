#Requires -RunAsAdministrator
function Update-SteamCmdGameValheimDedicatedServer {
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
        [ValidateScript( {Test-Path $_})]
        [string]$ValheimServerRootPath,
        [Parameter(Mandatory = $true)]
        [ValidateScript( {Test-Path $_})]
        [string]$WorldId
    )

    process {
        $env:SteamAppId=892970
        $ValheimWorldsConfig = "Valheim-Worlds.psd1"
        if (-not Test-Path .\$ValheimWorldsConfig)
        {
            throw "The Valheim World Configuration Source .\$ValheimWorldsConfig does not exist"
        }

        $config = Import-PowerShellDataFile .\$ValheimWorldsConfig

        if (-not $config.ContainsKey($WorldId))
        {
            throw "The worldId $WorldId was not found in the configuration source provided by $ValheimWorldsConfig"
        }

        $valheimServerExe = Join-Path $ValheimServerRootPath $config[$WorldId].InstallPath "valheim_server.exe"

        if (-not (Test-Path $valheimServerExe))
        {
            throw "Unable to find $valheimServerExe"
        }

        $argumentList = @('-no graphics', '-batchmode', "-name \"$config[$WorldId].ServerPublicName\"", "-port $config[$WorldId].Port", "-world \"$config[$WorldId].WorldDbName\"", "-password \"$config[$WorldId].Password\"" )
        Start-Process -FilePath $valheimServerExe -ArgumentList $argumentList
    }
}
