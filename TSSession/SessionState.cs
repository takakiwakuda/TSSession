namespace TSSession;

/// <summary>
/// Defines values that indicates the current state of the session.
/// </summary>
public enum SessionState
{
    /// <summary>
    /// A user is logged on the session.
    /// </summary>
    Active,

    /// <summary>
    /// The session is connected to the client.
    /// </summary>
    Connected,

    /// <summary>
    /// The session is in the process of connecting to the client.
    /// </summary>
    ConnectQuery,

    /// <summary>
    /// The session is shadowing another session.
    /// </summary>
    Shadow,

    /// <summary>
    /// The session is active but the client is disconnected.
    /// </summary>
    Disconnected,

    /// <summary>
    /// The session is waiting for a client to connect.
    /// </summary>
    Idle,

    /// <summary>
    /// The session is listening for a connection.
    /// </summary>
    Listen,

    /// <summary>
    /// The session is being reset.
    /// </summary>
    Reset,

    /// <summary>
    /// The session is down due to an error.
    /// </summary>
    Down,

    /// <summary>
    /// The session is initializing.
    /// </summary>
    Init
}
