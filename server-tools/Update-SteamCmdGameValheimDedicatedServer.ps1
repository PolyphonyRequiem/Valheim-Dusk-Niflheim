param (
    [Parameter(Mandatory = $false)]
    [ValidateScript( {Test-Path $_})]
    [string]$SteamCmdExePath = "C:\SteamCmd\steamcmd.exe",
    [Parameter(Mandatory = $true)]
    [string]$ValheimPath
)


Start-Process -FilePath $SteamCmdExePath -ArgumentList " +login anonymous +force_install_dir $ValheimPath +app_update 896660 +quit"
