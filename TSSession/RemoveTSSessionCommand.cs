using System;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;

namespace TSSession;

/// <summary>
/// The Remove-TSSession cmdlet logs off a Remote Desktop Services session.
/// </summary>
[Cmdlet(VerbsCommon.Remove, "TSSession", DefaultParameterSetName = SessionIdSetName, SupportsShouldProcess = true,
        HelpUri = "https://github.com/takakiwakuda/TSSession/blob/main/TSSession/docs/Remove-TSSession.md")]
[Alias("Remove-RDSession")]
public sealed class RemoveTSSessionCommand : EndTSSessionCommandBase
{
    /// <summary>
    /// ProcessRecord override.
    /// </summary>
    protected override void ProcessRecord()
    {
        using TerminalServer server = GetTerminalServer();

        try
        {
            if (Force || ShouldContinue(string.Format(StringResources.LogoffSessionConfirmation, SessionId, server), "Logginng off..."))
            {
                WriteVerbose(string.Format(StringResources.LogoffSession, SessionId, server));
                server.LogoffSession(SessionId, true);
            }
        }
        catch (IOException ex)
        {
            ErrorRecord errorRecord = new(ex, "SessionNotFound", ErrorCategory.ObjectNotFound, SessionId);
            ThrowTerminatingError(errorRecord);
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
