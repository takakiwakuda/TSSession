<#
.SYNOPSIS
    Build script for TSSession module.
#>
[CmdletBinding()]
param (
    [Parameter()]
    [ValidateSet("Debug", "Release")]
    [string]
    $Configuration = "Debug",

    [Parameter()]
    [ValidateSet("net462", "net6.0")]
    [string]
    $Framework
)

if ($Framework.Length -eq 0) {
    $Framework = if ($PSEdition -eq "Core") { "net6.0" }else { "net462" }
}

<#
.SYNOPSIS
    Build TSSession assembly.
#>
task BuildTSSession @{
    Inputs  = Get-ChildItem -Path TSSession\*.cs, TSSession\TSSession.csproj
    Outputs = "TSSession\bin\$Configuration\$Framework\TSSession.dll"
    Jobs    = {
        exec { dotnet publish --no-self-contained -c $Configuration -f $Framework TSSession }
    }
}

<#
.SYNOPSIS
    Build TSSession module.
#>
task BuildModule BuildTSSession, {
    $version = (Import-PowerShellDataFile -LiteralPath TSSession\TSSession.psd1).ModuleVersion
    $destination = "$PSScriptRoot\out\$Configuration\$Framework\TSSession\$version"

    if (Test-Path -LiteralPath $destination -PathType Container) {
        Remove-Item -LiteralPath $destination -Recurse
    }
    $null = New-Item -Path $destination -ItemType Directory

    $parameters = @{
        Path        = "TSSession\bin\$Configuration\$Framework\TSSession.*", "TSSession\en-US", "TSSession\ja-JP"
        Destination = $destination
        Recurse     = $true
    }
    Copy-Item @parameters
}

<#
.SYNOPSIS
    Install TSSession module.
#>
task Install BuildModule, {
    $destination = switch ($Framework) {
        "net6.0" {
            "$HOME\Documents\PowerShell\Modules"
        }
        "net462" {
            "$HOME\Documents\WindowsPowerShell\Modules"
        }
    }

    if (Test-Path -LiteralPath $destination -PathType Container) {
        if (Test-Path -LiteralPath "$destination\TSSession" -PathType Container) {
            Remove-Item -LiteralPath "$destination\TSSession" -Recurse
        }
    }
    else {
        $null = New-Item -Path $destination -ItemType Directory
    }

    Copy-Item -LiteralPath out\$Configuration\$Framework\TSSession -Destination $destination -Recurse
}

<#
.SYNOPSIS
    Test TSSession module.
#>
task Test Install, {
    $command = "& { Invoke-Pester -Path '$PSScriptRoot\test' -Output Detailed }"

    switch ($Framework) {
        "net6.0" {
            exec { pwsh -nop -c $command }
        }
        "net462" {
            exec { powershell -noprofile -command $command }
        }
    }
}

<#
.SYNOPSIS
    Run default tasks.
#>
task . Test
