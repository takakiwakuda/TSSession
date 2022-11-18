#if !NET7_0_OR_GREATER
using System;
using System.Runtime.InteropServices;

namespace TSSession;

internal static partial class Wtsapi32
{
    [DllImport(nameof(Wtsapi32), SetLastError = false)]
    internal static extern void WTSCloseServer(IntPtr hServer);

    [DllImport(nameof(Wtsapi32), SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSDisconnectSession(
        SafeTerminalServerHandle hServer,
        int SessionId,
        [MarshalAs(UnmanagedType.Bool)] bool bWait);

    [DllImport(nameof(Wtsapi32), EntryPoint = "WTSEnumerateSessionsExW", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSEnumerateSessionsEx(
        SafeTerminalServerHandle hServer,
        in uint pLevel,
        uint Filter,
        out IntPtr ppSessionInfo,
        out int pCount);

    [DllImport(nameof(Wtsapi32), SetLastError = false)]
    internal static extern void WTSFreeMemory(IntPtr pMemory);

    [DllImport(nameof(Wtsapi32), EntryPoint = "WTSFreeMemoryExW", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSFreeMemoryEx(WTS_TYPE_CLASS WTSTypeClass, IntPtr pMemory, int NumberOfEntries);

    [DllImport(nameof(Wtsapi32), SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSLogoffSession(
        SafeTerminalServerHandle hServer,
        int SessionId,
        [MarshalAs(UnmanagedType.Bool)] bool bWait);

    [DllImport(nameof(Wtsapi32), EntryPoint = "WTSOpenServerExW", SetLastError = false, CharSet = CharSet.Unicode)]
    internal static extern SafeTerminalServerHandle WTSOpenServerEx(string pServerName);

    [DllImport(nameof(Wtsapi32), EntryPoint = "WTSQuerySessionInformationW", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WTSQuerySessionInformation(
        SafeTerminalServerHandle hServer,
        int SessionId,
        WTS_INFO_CLASS WTSInfoClass,
        out IntPtr ppBuffer,
        out uint pBytesReturned);
}
#endif
