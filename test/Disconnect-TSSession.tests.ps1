Set-StrictMode -Version 3.0

Describe "Disconnect-TSSession" {
    BeforeAll {
        $PSDefaultParameterValues.Add("Disconnect-TSSession:Force", $true)
    }

    AfterAll {
        $PSDefaultParameterValues.Remove("Disconnect-TSSession:Force")
    }

    It "Throws an exception if a session with the specified ID does not exist" {
        $er = { Disconnect-TSSession -SessionId ([int]::MaxValue) } | Should -Throw -PassThru

        $er.FullyQualifiedErrorId | Should -Be "SessionNotFound,TSSession.DisconnectTSSessionCommand"
    }

    It "Throws an exception if requested RD Services access is not allowed" {
        $er = { Disconnect-TSSession -SessionId 0 -ServerName . } | Should -Throw -PassThru

        $er.FullyQualifiedErrorId | Should -Be "UnauthorizedRDServicesAccess,TSSession.DisconnectTSSessionCommand"
    }

    It "Throws an exception if the specified server is unavailable" {
        $er = { Disconnect-TSSession -SessionId 1 -ServerName DoesNotExist } | Should -Throw -PassThru

        $er.FullyQualifiedErrorId | Should -Be "RDSessionHostUnavailable,TSSession.DisconnectTSSessionCommand"
    }
}
