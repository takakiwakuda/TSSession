Set-StrictMode -Version 3.0

Describe "Remove-TSSession" {
    BeforeAll {
        $PSDefaultParameterValues.Add("Remove-TSSession:Force", $true)
    }

    AfterAll {
        $PSDefaultParameterValues.Remove("Remove-TSSession:Force")
    }

    It "Throws an exception if a session with the specified ID does not exist" {
        $er = { Remove-TSSession -SessionId ([int]::MaxValue) } | Should -Throw -PassThru

        $er.FullyQualifiedErrorId | Should -Be "SessionNotFound,TSSession.RemoveTSSessionCommand"
    }

    It "Throws an exception if requested RD Services access is not allowed" -TestCases @(
        @{ SessionId = 0; ServerName = "localhost" }
        @{ SessionId = 1; ServerName = "." }
    ) {
        $er = { Remove-TSSession -SessionId $SessionId -ServerName $ServerName } | Should -Throw -PassThru

        $er.FullyQualifiedErrorId | Should -Be "UnauthorizedRDServicesAccess,TSSession.RemoveTSSessionCommand"
    }

    It "Throws an exception if the specified server is unavailable" {
        $er = { Remove-TSSession -SessionId 1 -ServerName DoesNotExist } | Should -Throw -PassThru

        $er.FullyQualifiedErrorId | Should -Be "RDSessionHostUnavailable,TSSession.RemoveTSSessionCommand"
    }
}
