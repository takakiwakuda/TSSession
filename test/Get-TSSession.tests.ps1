Set-StrictMode -Version 3.0

Describe "Get-TSSession" {
    It "Throws an exception if requested RD Services access is not allowed" {
        $er = { Get-TSSession -ServerName . } | Should -Throw -PassThru

        $er.FullyQualifiedErrorId | Should -Be "UnauthorizedRDServicesAccess,TSSession.GetTSSessionCommand"
    }

    It "Throws an exception if the specified server is unavailable" {
        $er = { Get-TSSession -ServerName DoesNotExist } | Should -Throw -PassThru

        $er.FullyQualifiedErrorId | Should -Be "RDSessionHostUnavailable,TSSession.GetTSSessionCommand"
    }

    It "Should retrieve sessions" {
        $sessions = Get-TSSession

        $sessions | Should -BeOfType TSSession.TerminalServicesSession
    }

    It "Should retrieve sessions on the specified server" {
        $sessions = Get-TSSession -ServerName localhost

        $sessions | Should -BeOfType TSSession.TerminalServicesSession
    }
}
