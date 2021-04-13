#Requires -RunAsAdministrator
function Update-SteamCmdGameValheimDedicatedServer {
    <#
    .SYNOPSIS
    Updates Valheim Dedicated Server via Steam.

    .DESCRIPTION
    This cmdlet downloads SteamCmd and configures it in a custom or
    predefined location (C:\Program Files\SteamCmd).

    .PARAMETER SteamCmdExePath
    Specifies the install location of SteamCmd.

    .PARAMETER ValheimPath
    Specifies the install location of the Valheim install base to update.

    .EXAMPLE
    Update-SteamCmdGameValheimDedicatedServer -SteamCmdPath 'C:\SteamCmd'

    Installs SteamCmd in C:\SteamCmd.
    #>

    [CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'Medium')]
    param (
        [Parameter(Mandatory = $false)]
        [ValidateScript( {Test-Path $_})]
        [string]$SteamCmdExePath = "C:\SteamCmd\steamcmd.exe",
        [Parameter(Mandatory = $true)]
        [ValidateScript( {Test-Path $_})]
        [string]$ValheimPath
    )

    process {
        Start-Process -FilePath $SteamCmdExePath -ArgumentList " +login anonymous +force_install_dir $ValheimPath +app_update 896660 +quit"
    }
}
