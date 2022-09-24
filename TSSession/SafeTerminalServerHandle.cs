using System;
using System.Runtime.InteropServices;

namespace TSSession;

/// <summary>
/// Represents a safe handle to the RD Session Host server.
/// </summary>
public sealed class SafeTerminalServerHandle : SafeHandle
{
    /// <summary>
    /// Gets a value that indicates whether the handle is invalid.
    /// </summary>
    public override bool IsInvalid => handle == IntPtr.Zero;

    /// <summary>
    /// Creates a <see cref="SafeTerminalServerHandle"/>.
    /// </summary>
    public SafeTerminalServerHandle() : base(IntPtr.Zero, true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeTerminalServerHandle"/> class.
    /// </summary>
    /// <param name="preexistingHandle">An object that represents the pre-existing handle to use.</param>
    /// <param name="ownsHandle">
    /// <see langword="true"/> to reliably release the handle during the finalization phase;
    /// <see langword="false"/> to prevent reliable release.
    /// </param>
    public SafeTerminalServerHandle(IntPtr preexistingHandle, bool ownsHandle) : base(IntPtr.Zero, ownsHandle)
    {
        SetHandle(preexistingHandle);
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        WTSCloseServer(handle);
        return true;
    }

    /// <summary>
    /// Closes an open handle to a Remote Desktop Session Host (RD Session Host) server.
    /// </summary>
    /// <param name="hServer">
    /// A handle to an RD Session Host server opened by a call to the WTSOpenServer or WTSOpenServerEx function.
    /// </param>
    [DllImport("wtsapi32.dll", SetLastError = false)]
    private static extern void WTSCloseServer(IntPtr hServer);
}
