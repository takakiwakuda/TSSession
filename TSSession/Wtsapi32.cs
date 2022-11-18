using System;
using System.Runtime.InteropServices;

namespace TSSession;

internal static partial class Wtsapi32
{
    internal enum WTS_CONNECTSTATE_CLASS
    {
        WTSActive,
        WTSConnected,
        WTSConnectQuery,
        WTSShadow,
        WTSDisconnected,
        WTSIdle,
        WTSListen,
        WTSReset,
        WTSDown,
        WTSInit
    }

    internal enum WTS_INFO_CLASS
    {
        WTSInitialProgram,
        WTSApplicationName,
        WTSWorkingDirectory,
        WTSOEMId,
        WTSSessionId,
        WTSUserName,
        WTSWinStationName,
        WTSDomainName,
        WTSConnectState,
        WTSClientBuildNumber,
        WTSClientName,
        WTSClientDirectory,
        WTSClientProductId,
        WTSClientHardwareId,
        WTSClientAddress,
        WTSClientDisplay,
        WTSClientProtocolType,
        WTSIdleTime,
        WTSLogonTime,
        WTSIncomingBytes,
        WTSOutgoingBytes,
        WTSIncomingFrames,
        WTSOutgoingFrames,
        WTSClientInfo,
        WTSSessionInfo,
        WTSSessionInfoEx,
        WTSConfigInfo,
        WTSValidationInfo,
        WTSSessionAddressV4,
        WTSIsRemoteSession
    }

    internal enum WTS_TYPE_CLASS
    {
        WTSTypeProcessInfoLevel0,
        WTSTypeProcessInfoLevel1,
        WTSTypeSessionInfoLevel1
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WTS_SESSION_INFO_1
    {
        internal int ExecEnvId;
        internal WTS_CONNECTSTATE_CLASS State;
        internal int SessionId;
        internal string pSessionName;
        internal string? pHostName;
        internal string? pUserName;
        internal string? pDomainName;
        internal string? pFarmName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WTSINFO
    {
        internal WTS_CONNECTSTATE_CLASS State;
        internal int SessionId;
        internal int IncomingBytes;
        internal int OutgoingBytes;
        internal int IncomingFrames;
        internal int OutgoingFrames;
        internal int IncomingCompressedBytes;
        internal int OutgoingCompressedBytes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = WINSTATIONNAME_LENGTH)]
        internal string WinStationName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DOMAIN_LENGTH)]
        internal string? Domain;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = USERNAME_LENGTH + 1)]
        internal string? UserName;
        internal long ConnectTime;
        internal long DisconnectTime;
        internal long LastInputTime;
        internal long LogonTime;
        internal long CurrentTime;
    }

    internal const int WTS_CURRENT_SESSION = -1;

    private const int DOMAIN_LENGTH = 17;
    private const int USERNAME_LENGTH = 20;
    private const int WINSTATIONNAME_LENGTH = 32;

    internal static SafeTerminalServerHandle WTS_CURRENT_SERVER_HANDLE = new(IntPtr.Zero, false);
}
