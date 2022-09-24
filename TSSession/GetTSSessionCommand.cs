using System;
using System.ComponentModel;
using System.Management.Automation;

namespace TSSession;

/// <summary>
/// The Get-TSSession cmdlet gets Remote Desktop Services sessions.
/// </summary>
[Cmdlet(VerbsCommon.Get, "TSSession",
        HelpUri = "https://github.com/takakiwakuda/TSSession/blob/main/TSSession/docs/Get-TSSession.md")]
[OutputType(typeof(TerminalServicesSession))]
[Alias("Get-RDSession")]
public sealed class GetTSSessionCommand : TSSessionCommandBase
{
    /// <summary>
    /// Gets or sets the ServerName parameter.
    /// </summary>
    [Parameter(Position = 0)]
    [ValidateNotNullOrEmpty]
    [Alias("ComputerName", "SessionHostName")]
    public override string? ServerName { get; set; }

    /// <summary>
    /// ProcessRecord override.
    /// </summary>
    protected override void ProcessRecord()
    {
        using TerminalServer server = GetTerminalServer();

        try
        {
            WriteObject(server.GetSessions(), true);
        }
        catch (UnauthorizedAccessException ex)
        {
            ErrorRecord errorRecord = new(ex, "UnauthorizedRDServicesAccess", ErrorCategory.PermissionDenied, null);
            ThrowTerminatingError(errorRecord);
        }
        catch (Win32Exception ex)
        {
            ErrorRecord errorRecord = new(ex, "RDSessionHostUnavailable", ErrorCategory.OpenError, server.Name);
            ThrowTerminatingError(errorRecord);
        }
    }
}
