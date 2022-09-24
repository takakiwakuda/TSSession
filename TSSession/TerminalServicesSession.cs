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
    public DateTime? ConnectTime { get; }

    /// <summary>
    /// Gets the last session disconnection date and time.
    /// </summary>
    public DateTime? DisconnectTime { get; }

    /// <summary>
    /// Gets the domain name of the user logged on to the session.
    /// </summary>
    public string? DomainName { get; }

    /// <summary>
    /// Gets the session idle time.
    /// </summary>
    public TimeSpan IdleTime { get; }

    /// <summary>
    /// Gets the date and time of the last user input in the session.
    /// </summary>
    public DateTime? LastInputTime { get; }

    /// <summary>
    /// Gets the date and time that the user logged on to the session.
    /// </summary>
    public DateTime? LogonTime { get; }

    /// <summary>
    /// Gets the name of the server hosting the session.
    /// </summary>
    public string ServerName { get; }

    /// <summary>
    /// Gets the session identifier.
    /// </summary>
    public int SessionId { get; }

    /// <summary>
    /// Gets the name of the session.
    /// </summary>
    public string SessionName { get; }

    /// <summary>
    /// Gets the state of the session.
    /// </summary>
    public SessionState SessionState { get; }

    /// <summary>
    /// Gets the name of the user logged on to the session.
    /// </summary>
    public string? UserName { get; }

    private readonly bool _isRemoteSession;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalServicesSession"/> class.
    /// </summary>
    /// <param name="serverName">The name of the server hosting the session.</param>
    /// <param name="remote">
    /// A value that indicates the <paramref name="sessionInfo"/> parameter is a remote session.
    /// </param>
    /// <param name="sessionInfo">Information about the client session.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="serverName"/> is <see langword="null"/>.
    /// </exception>
    internal TerminalServicesSession(string serverName, bool remote, Wtsapi32.WTSINFO sessionInfo)
    {
        if (serverName is null)
        {
            throw new ArgumentNullException(nameof(serverName));
        }

        ServerName = serverName;
        DomainName = sessionInfo.Domain;
        SessionId = sessionInfo.SessionId;
        SessionName = sessionInfo.WinStationName;
        SessionState = (SessionState)sessionInfo.State;
        UserName = sessionInfo.UserName;

        ConnectTime = FileTimeToDateTime(sessionInfo.ConnectTime);
        DisconnectTime = FileTimeToDateTime(sessionInfo.DisconnectTime);
        LogonTime = FileTimeToDateTime(sessionInfo.LogonTime);

        if (sessionInfo.LastInputTime > 0)
        {
            LastInputTime = DateTime.FromFileTime(sessionInfo.LastInputTime);
            IdleTime = DateTime.FromFileTime(sessionInfo.CurrentTime) - LastInputTime.Value;
        }

        _isRemoteSession = remote;
    }

    /// <summary>
    /// Returns a string representing the session name.
    /// </summary>
    /// <returns>A string representing the session name.</returns>
    public override string ToString()
    {
        return SessionName;
    }

    /// <summary>
    /// Gets a <see cref="TerminalServer"/> object representing the server hosting the session.
    /// </summary>
    /// <returns>
    /// A <see cref="TerminalServer"/> object representing the server hosting the session.
    /// </returns>
    internal TerminalServer GetHostServer()
    {
        return _isRemoteSession ? new(ServerName) : TerminalServer.Current;
    }

    /// <summary>
    /// Converts the specified Windows file time to an equivalent local time.
    /// </summary>
    /// <param name="fileTime">A Windows file time expressed in ticks.</param>
    /// <returns>
    /// <see langword="null"/> if the <paramref name="fileTime"/> parameter is zero;
    /// otherwise, <see cref="DateTime"/> object.
    /// </returns>
    private static DateTime? FileTimeToDateTime(long fileTime)
    {
        return fileTime == 0 ? null : DateTime.FromFileTime(fileTime);
    }
}
