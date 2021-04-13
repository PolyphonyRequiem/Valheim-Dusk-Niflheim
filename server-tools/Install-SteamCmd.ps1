#Requires -RunAsAdministrator
function Install-SteamCmd {
    <#
    .SYNOPSIS
    Install SteamCmd.

    .DESCRIPTION
    This cmdlet downloads SteamCmd and configures it in a custom or
    predefined location (C:\Program Files\SteamCmd).

    .PARAMETER InstallPath
    Specifies the install location of SteamCmd.

    .PARAMETER Force
    The Force parameter allows the user to skip the "Should Continue" box.

    .EXAMPLE
    Install-SteamCmd

    Installs SteamCmd in C:\Program Files\SteamCmd.

    .EXAMPLE
    Install-SteamCmd -InstallPath 'C:'

    Installs SteamCmd in C:\SteamCmd.
    #>

    [CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'Medium')]
    param (
        [Parameter(Mandatory = $false)]
        [ValidateScript( {
                if ($_.Substring(($_.Length -1)) -eq '\') {
                    throw "InstallPath may not end with a trailing slash."
                }
                $true
            })]
        [string]$InstallPath = "C:",

        [Parameter(Mandatory = $false)]
        [switch]$Force
    )

    process {
        if ($Force -or $PSCmdlet.ShouldContinue('Would you like to continue?', 'Install SteamCmd')) {
            # Ensures that SteamCmd is installed in a folder named SteamCmd.
            $InstallPath = $InstallPath + '\SteamCmd'

            if (-not ((Get-SteamPath).Path -eq $InstallPath)) {
                Write-Verbose -Message "Adding $($InstallPath) to Environment Variable PATH."
                Add-EnvPath -Path $InstallPath -Container Machine
            } else {
                Write-Verbose -Message "Path $((Get-SteamPath).Path) already exists."
            }

            $TempDirectory = 'C:\Temp'
            if (-not (Test-Path -Path $TempDirectory)) {
                Write-Verbose -Message 'Creating Temp directory.'
                New-Item -Path 'C:\' -Name 'Temp' -ItemType Directory | Write-Verbose
            }

            # Download SteamCmd.
            Invoke-WebRequest -Uri 'https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip' -OutFile "$($TempDirectory)\steamcmd.zip" -UseBasicParsing

            # Create SteamCmd directory if necessary.
            if (-not (Test-Path -Path $InstallPath)) {
                Write-Verbose -Message "Creating SteamCmd directory: $($InstallPath)"
                New-Item -Path $InstallPath -ItemType Directory | Write-Verbose
                Expand-Archive -Path "$($TempDirectory)\steamcmd.zip" -DestinationPath $InstallPath
            }

            # Doing some initial configuration of SteamCmd. The first time SteamCmd is launched it will need to do some updates.
            Write-Host -Object 'Configuring SteamCmd for the first time. This might take a little while.'
            Write-Host -Object 'Please wait' -NoNewline
            Start-Process -FilePath "$($InstallPath)\steamcmd.exe" -ArgumentList 'validate +quit' -WindowStyle Hidden
            do {
                Write-Host -Object "." -NoNewline
                Start-Sleep -Seconds 3
            }
            until (-not (Get-Process -Name "*steamcmd*"))
        }
    } # Process

    end {
        if (Test-Path -Path "$($TempDirectory)\steamcmd.zip") {
            Remove-Item -Path "$($TempDirectory)\steamcmd.zip" -Force
        }

        if (Test-Path -Path (Get-SteamPath).Executable) {
            Write-Output -InputObject "SteamCmd is now installed. Please close/open your PowerShell host."
        }
    } # End
} # Cmdlet