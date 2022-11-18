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
        Wtsapi32.WTSCloseServer(handle);
        return true;
    }
}
