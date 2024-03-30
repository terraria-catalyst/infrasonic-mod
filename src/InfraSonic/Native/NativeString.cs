using System;
using System.Runtime.InteropServices;

namespace InfraSonic.Src.Native;

internal unsafe struct NativeString : IDisposable
{
    public IntPtr Handle;

    public void Dispose()
    {
        if (Handle != IntPtr.Zero) {
            Marshal.FreeHGlobal(Handle);
        }

        this = default;
    }

    /*
    public static NativeString AllocFromPtr(IntPtr ptr, int lengthNonTerminated)
    {
        NativeString result;

        result.Handle = Marshal.AllocHGlobal(lengthNonTerminated + 1); // Null-terminated.

        Buffer.MemoryCopy((void*)ptr, (void*)result.Handle, lengthNonTerminated, lengthNonTerminated);

        return result;
    }
    */

    public static NativeString AllocFromString(string managedString)
    {
        NativeString result;

        result.Handle = Marshal.StringToHGlobalAnsi(managedString);

        return result;
    }
}
