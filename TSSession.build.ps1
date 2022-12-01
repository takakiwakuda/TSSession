<#
.SYNOPSIS
    Build script for TSSession module.
#>
[CmdletBinding()]
param (
    [Parameter()]
    [ValidateSet("Debug", "Release")]
    [string]
    $Configuration = (property Configuration Release),

    [Parameter()]
    [ValidateSet("net462", "net7.0")]
    [string]
    $Framework
)

if ($Framework.Length -eq 0) {
    $Framework = if ($PSEdition -eq "Core") { "net7.0" } else { "net462" }
}

<#
.SYNOPSIS
    Build TSSession assembly.
#>
task BuildTSSession @{
    Inputs  = {
        Get-ChildItem -Path TSSession\*.cs, TSSession\TSSession.csproj
    }
    Outputs = "TSSession\bin\$Configuration\$Framework\TSSession.dll"
    Jobs    = {
        exec { dotnet publish -c $Configuration -f $Framework TSSession }
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
    Run TSSession module tests.
#>
task RunModuleTest BuildModule, {
    $command = @"
    & {
        Import-Module -Name '$PSScriptRoot\out\$Configuration\$Framework\TSSession';
        Invoke-Pester -Path '$PSScriptRoot\test' -Output Detailed
    }
"@

    switch ($Framework) {
        "net7.0" {
            exec { pwsh -nop -c $command }
        }
        default {
            exec { powershell -noprofile -command $command }
        }
    }
}

<#
.SYNOPSIS
    Run default tasks.
#>
task . RunModuleTest
