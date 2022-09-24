using System.Management.Automation;

namespace TSSession;

/// <summary>
/// Base command for TSSession commands.
/// </summary>
public abstract class TSSessionCommandBase : PSCmdlet
{
    /// <summary>
    /// Gets or sets the ServerName parameter.
    /// Override this property in derived classes.
    /// </summary>
    public virtual string? ServerName { get; set; }

    /// <summary>
    /// Gets a <see cref="TerminalServer"/> object.
    /// </summary>
    /// <remarks>
    /// Derived classes can override this method.
    /// </remarks>
    /// <returns>
    /// If the ServerName parameter is <see langword="null"/>, the server running the application is returned;
    /// otherwise, the server of the ServerName parameter is returned.
    /// </returns>
    protected virtual TerminalServer GetTerminalServer()
    {
        return ServerName is null ? TerminalServer.Current : new(ServerName);
    }
}
