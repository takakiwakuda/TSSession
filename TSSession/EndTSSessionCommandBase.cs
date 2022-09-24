using System;
using System.Management.Automation;

namespace TSSession;

/// <summary>
/// Base command for TSSession commands to end Remote Desktop Services session.
/// </summary>
public class EndTSSessionCommandBase : TSSessionCommandBase
{
    /// <summary>
    /// The parameter set name is SessionId;
    /// </summary>
    protected const string SessionIdSetName = "SessionId";

    /// <summary>
    /// The parameter set name is Session;
    /// </summary>
    protected const string SessionSetName = "Session";

    /// <summary>
    /// Gets or sets the SessionId parameter.
    /// </summary>
    [Parameter(ParameterSetName = SessionIdSetName, Mandatory = true, Position = 0)]
    [ValidateRange(0, int.MaxValue)]
    [Alias("Id")]
    public int SessionId { get; set; }

    /// <summary>
    /// Gets or sets the ServerName parameter.
    /// </summary>
    [Parameter(ParameterSetName = SessionIdSetName, Position = 1)]
    [ValidateNotNullOrEmpty]
    [Alias("ComputerName", "SessionHostName")]
    public override string? ServerName { get; set; }

    /// <summary>
    /// Gets or sets the Session parameter.
    /// </summary>
    [Parameter(ParameterSetName = SessionSetName, Mandatory = true, ValueFromPipeline = true)]
    [ValidateNotNull]
    public TerminalServicesSession Session { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Force parameter.
    /// </summary>
    [Parameter]
    public SwitchParameter Force { get; set; }

    /// <summary>
    /// Gets a <see cref="TerminalServer"/> object.
    /// </summary>
    /// <returns>
    /// If the Session parameter is specified, returns the server hosting the session;
    /// otherwise, calls the <see cref="TSSessionCommandBase.GetTerminalServer"/> method.
    /// </returns>
    protected override TerminalServer GetTerminalServer()
    {
        if (ParameterSetName.Equals(SessionSetName, StringComparison.Ordinal))
        {
            SessionId = Session.SessionId;
            return Session.GetHostServer();
        }

        return base.GetTerminalServer();
    }
}
