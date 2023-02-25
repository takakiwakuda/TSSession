using System;

namespace TSSession;

/// <summary>
/// Contains about information a RD Services session.
/// </summary>
public sealed class TerminalServicesSession
{
    /// <summary>
    /// Gets a <see cref="TerminalServicesSession"/> object that represents the session running the application.
    /// </summary>
    public static TerminalServicesSession Current => TerminalServer.Current.GetSession(Wtsapi32.WTS_CURRENT_SESSION);

    /// <summary>
    /// Gets the session connection date and time.
    /// </summary>
    public DateTime? ConnectTime
    {
        get
        {
            UpdateSessionInformation();
            return FileTimeToDateTime(_sessionInfo.ConnectTime);
        }
    }

    /// <summary>
    /// Gets the last session disconnection date and time.
    /// </summary>
    public DateTime? DisconnectTime
    {
        get
        {
            UpdateSessionInformation();
            return FileTimeToDateTime(_sessionInfo.DisconnectTime);
        }
    }

    /// <summary>
    /// Gets the domain name of the user logged on to the session.
    /// </summary>
    public string? DomainName
    {
        get
        {
            UpdateSessionInformation();
            return _sessionInfo.Domain;
        }
    }

    /// <summary>
    /// Gets the session idle time.
    /// </summary>
    public TimeSpan IdleTime
    {
        get
        {
            UpdateSessionInformation();

            if (_sessionInfo.LastInputTime == 0)
            {
                return TimeSpan.Zero;
            }
            return new TimeSpan(_sessionInfo.CurrentTime - _sessionInfo.LastInputTime);
        }
    }

    /// <summary>
    /// Gets the date and time of the last user input in the session.
    /// </summary>
    public DateTime? LastInputTime
    {
        get
        {
            UpdateSessionInformation();
            return FileTimeToDateTime(_sessionInfo.LastInputTime);
        }
    }

    /// <summary>
    /// Gets the date and time that the user logged on to the session.
    /// </summary>
    public DateTime? LogonTime
    {
        get
        {
            UpdateSessionInformation();
            return FileTimeToDateTime(_sessionInfo.LogonTime);
        }
    }

    /// <summary>
    /// Gets the name of the server hosting the session.
    /// </summary>
    public string ServerName => _serverName;

    /// <summary>
    /// Gets the session identifier.
    /// </summary>
    public int SessionId => _sessionId;

    /// <summary>
    /// Gets the name of the session.
    /// </summary>
    public string SessionName
    {
        get
        {
            UpdateSessionInformation();
            return _sessionInfo.WinStationName;
        }
    }

    /// <summary>
    /// Gets the state of the session.
    /// </summary>
    public SessionState SessionState
    {
        get
        {
            UpdateSessionInformation();
            return (SessionState)_sessionInfo.State;
        }
    }

    /// <summary>
    /// Gets the name of the user logged on to the session.
    /// </summary>
    public string? UserName
    {
        get
        {
            UpdateSessionInformation();
            return _sessionInfo.UserName;
        }
    }

    private readonly bool _isRemoteSession;
    private readonly int _sessionId;
    private readonly string _serverName;
    private Wtsapi32.WTSINFO _sessionInfo;
    private bool _sessionInfoUpdated;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalServicesSession"/> class
    /// with the specified session ID, server name, value indicating the remote session, and session information.
    /// </summary>
    /// <param name="sessionId">The session identifier of a RD Services session.</param>
    /// <param name="serverName">The name of the server hosting the session.</param>
    /// <param name="remote">
    /// A value that indicates the <paramref name="sessionInfo"/> parameter is a remote session.
    /// </param>
    /// <param name="sessionInfo">Information about the client session.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="serverName"/> is <see langword="null"/>.
    /// </exception>
    internal TerminalServicesSession(int sessionId, string serverName, bool remote, Wtsapi32.WTSINFO sessionInfo)
    {
        if (serverName is null)
        {
            throw new ArgumentNullException(nameof(serverName));
        }

        _serverName = serverName;
        _sessionId = sessionId != Wtsapi32.WTS_CURRENT_SESSION ? sessionId : sessionInfo.SessionId;
        _sessionInfo = sessionInfo;
        _sessionInfoUpdated = true;
        _isRemoteSession = remote;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalServicesSession"/> class
    /// with the specified session ID and terminal server.
    /// </summary>
    /// <param name="sessionId">The session identifier of a RD Services session.</param>
    /// <param name="server">The terminal server being opened.</param>
    internal TerminalServicesSession(int sessionId, TerminalServer server)
    {
        _serverName = server.Name;
        _sessionInfo = server.GetSessionInformation(sessionId);
        _sessionInfoUpdated = true;
        _sessionId = sessionId != Wtsapi32.WTS_CURRENT_SESSION ? sessionId : _sessionInfo.SessionId;
        _isRemoteSession = server.IsRemoteServer;
    }

    /// <summary>
    /// Refreshes property values by resetting the properties to their current values.
    /// </summary>
    public void Refresh() => _sessionInfoUpdated = false;

    /// <summary>
    /// Returns a string representing the session name.
    /// </summary>
    /// <returns>A string representing the session name.</returns>
    public override string ToString() => SessionName;

    /// <summary>
    /// Gets a <see cref="TerminalServer"/> object representing the server hosting the session.
    /// </summary>
    /// <returns>
    /// A <see cref="TerminalServer"/> object representing the server hosting the session.
    /// </returns>
    internal TerminalServer GetHostServer() => _isRemoteSession ? new(ServerName) : TerminalServer.Current;

    private static DateTime? FileTimeToDateTime(long fileTime)
        => fileTime == 0 ? null : DateTime.FromFileTime(fileTime);

    private void UpdateSessionInformation()
    {
        if (_sessionInfoUpdated)
        {
            return;
        }

        using (TerminalServer server = GetHostServer())
        {
            _sessionInfo = server.GetSessionInformation(_sessionId);
        }
        _sessionInfoUpdated = true;
    }
}
