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

$PSCmdlet.WriteVerbose("Configuration : $Configuration")
$PSCmdlet.WriteVerbose("Framework     : $Framework")

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
task BuildModule BuildTSSession, NewModuleHelp, {
    $version = (Import-PowerShellDataFile -LiteralPath TSSession\TSSession.psd1).ModuleVersion
    $destination = "$PSScriptRoot\out\$Configuration\$Framework\TSSession\$version"

    if (Test-Path -LiteralPath $destination -PathType Container) {
        Remove-Item -LiteralPath $destination -Recurse
    }
    $null = New-Item -Path $destination -ItemType Directory -Force

    $source = "TSSession\bin\$Configuration\$Framework"

    Copy-Item -LiteralPath $source\TSSession.dll -Destination $destination
    Copy-Item -LiteralPath $source\TSSession.format.ps1xml -Destination $destination
    Copy-Item -LiteralPath $source\TSSession.psd1 -Destination $destination
    Copy-Item -LiteralPath TSSession\en-US -Destination $destination -Recurse
    Copy-Item -LiteralPath TSSession\ja-JP -Destination $destination -Recurse
}

<#
.SYNOPSIS
    Create TSSession module helps.
#>
task NewModuleHelp @{
    Inputs  = {
        Get-ChildItem -Path TSSession\docs\*.md
    }
    Outputs = "TSSession\en-US\TSSession.dll-Help.xml"
    Jobs    = {
        $null = New-ExternalHelp -Path TSSession\docs -OutputPath TSSession\en-US -Force
        $null = New-ExternalHelp -Path TSSession\docs -OutputPath TSSession\ja-JP -Force
    }
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
