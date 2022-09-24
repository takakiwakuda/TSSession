using System;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;

namespace TSSession;

/// <summary>
/// The Disconnect-TSSession cmdlet disconnects a Remote Desktop Services session.
/// </summary>
[Cmdlet(VerbsCommunications.Disconnect, "TSSession", DefaultParameterSetName = SessionIdSetName, SupportsShouldProcess = true,
        HelpUri = "https://github.com/takakiwakuda/TSSession/blob/main/TSSession/docs/Disconnect-TSSession.md")]
[Alias("Disconnect-RDSession")]
public sealed class DisconnectTSSessionCommand : EndTSSessionCommandBase
{
    /// <summary>
    /// ProcessRecord override.
    /// </summary>
    protected override void ProcessRecord()
    {
        using TerminalServer server = GetTerminalServer();

        try
        {
            if (Force || ShouldContinue(string.Format(StringResources.DisconnectSessionConfirmation, SessionId, server), "Disconnecting..."))
            {
                WriteVerbose(string.Format(StringResources.DisconnectSession, SessionId, server));
                server.DisconnectSession(SessionId, true);
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
