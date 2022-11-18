#if NET7_0_OR_GREATER
using System;
using System.Runtime.InteropServices;

namespace TSSession;

internal static partial class Wtsapi32
{
    [LibraryImport(nameof(Wtsapi32), SetLastError = false)]
    internal static partial void WTSCloseServer(IntPtr hServer);

    [LibraryImport(nameof(Wtsapi32), SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool WTSDisconnectSession(
        SafeTerminalServerHandle hServer,
        int SessionId,
        [MarshalAs(UnmanagedType.Bool)] bool bWait);

    [LibraryImport(nameof(Wtsapi32), EntryPoint = "WTSEnumerateSessionsExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool WTSEnumerateSessionsEx(
        SafeTerminalServerHandle hServer,
        in uint pLevel,
        uint Filter,
        out IntPtr ppSessionInfo,
        out int pCount);

    [LibraryImport(nameof(Wtsapi32), SetLastError = false)]
    internal static partial void WTSFreeMemory(IntPtr pMemory);

    [LibraryImport(nameof(Wtsapi32), EntryPoint = "WTSFreeMemoryExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool WTSFreeMemoryEx(WTS_TYPE_CLASS WTSTypeClass, IntPtr pMemory, int NumberOfEntries);

    [LibraryImport(nameof(Wtsapi32), SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool WTSLogoffSession(
        SafeTerminalServerHandle hServer,
        int SessionId,
        [MarshalAs(UnmanagedType.Bool)] bool bWait);

    [LibraryImport(nameof(Wtsapi32), EntryPoint = "WTSOpenServerExW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial SafeTerminalServerHandle WTSOpenServerEx(string pServerName);

    [LibraryImport(nameof(Wtsapi32), EntryPoint = "WTSQuerySessionInformationW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool WTSQuerySessionInformation(
        SafeTerminalServerHandle hServer,
        int SessionId,
        WTS_INFO_CLASS WTSInfoClass,
        out IntPtr ppBuffer,
        out uint pBytesReturned);
}
#endif
