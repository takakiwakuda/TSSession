using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;

namespace TSSession;

/// <summary>
/// Represents a Remote Desktop Session Host server.
/// </summary>
public sealed class TerminalServer : IDisposable
{
    private static readonly Lazy<TerminalServer> s_current = new(() => new());

    /// <summary>
    /// Gets a <see cref="TerminalServer"/> object that represents the server running the application.
    /// </summary>
    public static TerminalServer Current => s_current.Value;

    /// <summary>
    /// Gets a <see cref="SafeTerminalServerHandle"/> object that represents the Remote Desktop Session Host server
    /// that the current <see cref="TerminalServer"/> object encapsulates.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// The server object has already been dispoased.
    /// </exception>
    public SafeTerminalServerHandle Handle
    {
        get
        {
            ThrowIfDisposed();
            return _handle;
        }
    }

    /// <summary>
    /// Retrieves the name of the server.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// The server object has already been dispoased.
    /// </exception>
    public string Name
    {
        get
        {
            ThrowIfDisposed();
            return _serverName;
        }
    }

    private readonly bool _isRemoteServer;
    private readonly string _serverName;
    private SafeTerminalServerHandle _handle;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalServer"/> class to the specified server.
    /// </summary>
    /// <param name="serverName">The name of a RD Session Host server.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="serverName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="serverName"/> is empty.
    /// </exception>
    public TerminalServer(string serverName)
    {
        if (serverName is null)
        {
            throw new ArgumentNullException(nameof(serverName));
        }

        if (serverName.Length == 0)
        {
            throw new ArgumentException("The value cannot be an empty string.", nameof(serverName));
        }

        _handle = Wtsapi32.WTSOpenServerEx(serverName);
        _serverName = serverName;
        _isRemoteServer = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalServer"/> class to the server running the application.
    /// </summary>
    private TerminalServer()
    {
        _handle = Wtsapi32.WTS_CURRENT_SERVER_HANDLE;
        _serverName = Environment.MachineName;
        _isRemoteServer = false;
    }

    /// <summary>
    /// Disconnects the specified session identifier.
    /// </summary>
    /// <param name="sessionId">The session identifier of a RD Services session.</param>
    /// <param name="wait">
    /// <see langword="true"/> to wait for the session disconnection; <see langword="false"/> to return immediately.
    /// </param>
    /// <exception cref="IOException">
    /// <paramref name="sessionId"/> cannot be found.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The server object has already been dispoased.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="Win32Exception">
    /// An error occurred in Windows API.
    /// </exception>
    public void DisconnectSession(int sessionId, bool wait)
    {
        ThrowIfDisposed();

        if (!Wtsapi32.WTSDisconnectSession(_handle, sessionId, wait))
        {
            ThrowWin32Error();
        }
    }

    /// <summary>
    /// Releases all resources used by the current instance of the <see cref="TerminalServer"/> class.
    /// </summary>
    public void Dispose()
    {
        if (_handle is not null && _isRemoteServer)
        {
            _handle.Dispose();
            _handle = null!;
        }
    }

    /// <summary>
    /// Retrieves the session associated with the specified identifier on the server.
    /// </summary>
    /// <param name="sessionId">The session identifier of a RD Services session.</param>
    /// <returns>The session associated with the <paramref name="sessionId"/>.</returns>
    /// <exception cref="IOException">
    /// <paramref name="sessionId"/> cannot be found.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The server object has already been dispoased.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="Win32Exception">
    /// An error occurred in Windows API.
    /// </exception>
    public TerminalServicesSession GetSession(int sessionId)
    {
        ThrowIfDisposed();

        return new(_serverName, _isRemoteServer, GetSessionInformation(sessionId));
    }

    /// <summary>
    /// Retrieves an array of sessions on the server.
    /// </summary>
    /// <returns>An array of <see cref="TerminalServicesSession"/> objects.</returns>
    /// <exception cref="ObjectDisposedException">
    /// The server object has already been dispoased.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="Win32Exception">
    /// An error occurred in Windows API.
    /// </exception>
    public TerminalServicesSession[] GetSessions()
    {
        ThrowIfDisposed();

        if (!Wtsapi32.WTSEnumerateSessionsEx(_handle, 1, 0, out nint ptr, out int count))
        {
            ThrowWin32Error();
        }

        List<TerminalServicesSession> sessions = new(count);

        try
        {
            int size = Marshal.SizeOf<Wtsapi32.WTS_SESSION_INFO_1>();
            nint current = ptr;
            Wtsapi32.WTS_SESSION_INFO_1 sessionInfo;

            for (int i = 0; i < count; i++)
            {
                sessionInfo = Marshal.PtrToStructure<Wtsapi32.WTS_SESSION_INFO_1>(current);
                sessions.Add(new(_serverName, _isRemoteServer, GetSessionInformation(sessionInfo.SessionId)));
                current += size;
            }
        }
        finally
        {
            Debug.Assert(Wtsapi32.WTSFreeMemoryEx(Wtsapi32.WTS_TYPE_CLASS.WTSTypeSessionInfoLevel1, ptr, count));
        }

        return sessions.ToArray();
    }

    /// <summary>
    /// Logs off the specified session identifier.
    /// </summary>
    /// <param name="sessionId">The session identifier of a RD Services session.</param>
    /// <param name="wait">
    /// <see langword="true"/> to wait for the session logoff; <see langword="false"/> to return immediately.
    /// </param>
    /// <exception cref="IOException">
    /// <paramref name="sessionId"/> cannot be found.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// The server object has already been dispoased.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="Win32Exception">
    /// An error occurred in Windows API.
    /// </exception>
    public void LogoffSession(int sessionId, bool wait)
    {
        ThrowIfDisposed();

        if (!Wtsapi32.WTSLogoffSession(_handle, sessionId, wait))
        {
            ThrowWin32Error();
        }
    }

    /// <summary>
    /// Returns a string representing the server name.
    /// </summary>
    /// <returns>A string representing the server name.</returns>
    public override string ToString()
    {
        return _serverName;
    }

    /// <summary>
    /// Retrieves extended information for the specified session identifier.
    /// </summary>
    /// <param name="sessionId">The session identifier of a RD Services session.</param>
    /// <returns>
    /// The <see cref="Wtsapi32.WTSINFO"/> object that contains extended information for the session identifier.
    /// </returns>
    /// <exception cref="IOException">
    /// <paramref name="sessionId"/> cannot be found.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="Win32Exception">
    /// An error occurred in Windows API.
    /// </exception>
    private Wtsapi32.WTSINFO GetSessionInformation(int sessionId)
    {
        return QuerySessionInformation<Wtsapi32.WTSINFO>(sessionId, Wtsapi32.WTS_INFO_CLASS.WTSSessionInfo);
    }

    /// <summary>
    /// Retrieves session information for the specified session identifier.
    /// </summary>
    /// <typeparam name="T">The type of the object to which the data is to be copied.</typeparam>
    /// <param name="sessionId">The session identifier of a RD Services session.</param>
    /// <param name="infoClass">A value of the <see cref="Wtsapi32.WTS_INFO_CLASS"/> enumeration.</param>
    /// <returns>
    /// An object that contains session information associated with the session identifier.
    /// </returns>
    /// <exception cref="IOException">
    /// <paramref name="sessionId"/> cannot be found.
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// The caller does not have the required permission.
    /// </exception>
    /// <exception cref="Win32Exception">
    /// An error occurred in Windows API.
    /// </exception>
    private T QuerySessionInformation<T>(int sessionId, Wtsapi32.WTS_INFO_CLASS infoClass) where T : struct
    {
        if (!Wtsapi32.WTSQuerySessionInformation(_handle, sessionId, infoClass, out nint ptr, out uint _))
        {
            ThrowWin32Error();
        }

        try
        {
            return Marshal.PtrToStructure<T>(ptr);
        }
        finally
        {
            Wtsapi32.WTSFreeMemory(ptr);
        }
    }

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException"/> if this instance has been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// <see cref="_handle"/> is <see langword="null"/>.
    /// </exception>
    private void ThrowIfDisposed()
    {
        if (_handle is null)
        {
            throw new ObjectDisposedException(nameof(_handle));
        }
    }

    /// <summary>
    /// Throws an exception for Win32 error.
    /// </summary>
    /// <exception cref="IOException"/>
    /// <exception cref="UnauthorizedAccessException"/>
    /// <exception cref="Win32Exception"/>
    [SuppressMessage("csharp", "IDE0066")]
    private static void ThrowWin32Error()
    {
#if NET6_0_OR_GREATER
        int error = Marshal.GetLastPInvokeError();
#else
        int error = Marshal.GetLastWin32Error();
#endif

        switch (error)
        {
            case Errors.ERROR_FILE_NOT_FOUND:
            case Errors.ERROR_CTX_WINSTATION_NOT_FOUND:
                throw new IOException(StringResources.SessionNotFound, error);

            case Errors.ERROR_ACCESS_DENIED:
                throw new UnauthorizedAccessException(StringResources.UnauthorizedRDServicesAccess);

            default:
                throw new Win32Exception(error);
        }
    }
}
