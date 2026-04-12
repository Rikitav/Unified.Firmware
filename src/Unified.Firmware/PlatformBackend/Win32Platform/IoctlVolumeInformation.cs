// The MIT License (MIT)
// 
// Unified.Firmware
// Copyright 2026 © Rikitav Tim4ik
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Unified.Firmware.PlatformBackend.Win32Platform;

/// <summary>
/// Provides methods for retrieving volume information using IOCTL commands.
/// </summary>
public static class IoctlVolumeInformation
{
    public static IEnumerable<PARTITION_INFORMATION_EX> EnumeratePartitions()
    {
        foreach (PARTITION_INFORMATION_EX partition in new IoctlVolumeEnumerable(0))
            yield return partition;
    }

    /// <summary>
    /// Retrieves extended partition information for the specified volume.
    /// </summary>
    /// <param name="volumePath">The path to the volume.</param>
    /// <returns>A <see cref="PARTITION_INFORMATION_EX"/> structure containing the partition information.</returns>
    /// <exception cref="Win32Exception">Thrown when the partition descriptor cannot be opened or the information cannot be retrieved.</exception>
    public static PARTITION_INFORMATION_EX GetPartition(Guid volumePath)
    {
        if (volumePath == Guid.Empty)
            return default;

        IntPtr partHandle = NativeMethods.CreateFile(string.Concat(@"\\?\Volume{", volumePath.ToString(), @"}\"), 0, FileShare.Read, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

        // Checking for INVALID_HANDLE_VALUE
        if (partHandle == new IntPtr(-1))
        {
            int lastError = Marshal.GetLastWin32Error();
            throw new Win32Exception(lastError, "Failed to open partition descriptor");
        }

        // Allocating structure memory
        int StructSize = Marshal.SizeOf<PARTITION_INFORMATION_EX>();
        IntPtr StructPtr = Marshal.AllocHGlobal(StructSize);
        Marshal.StructureToPtr(new PARTITION_INFORMATION_EX(), StructPtr, true);

        //Executing DeviceIoControl()
        if (!NativeMethods.DeviceIoControl(partHandle, NativeMethods.DiskGetPartitionInfoEx, IntPtr.Zero, 0, StructPtr, (uint)StructSize, out _, IntPtr.Zero))
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to obtain PARTITION_INFORMATION_EX structure");

        //creating new struct from allocated byte buffer
        PARTITION_INFORMATION_EX partInfoEx = Marshal.PtrToStructure<PARTITION_INFORMATION_EX>(StructPtr);
        Marshal.FreeHGlobal(StructPtr);

        return partInfoEx;
    }

    private static class NativeMethods
    {
        public const uint DiskGetPartitionInfoEx = 0x00000007 << 16 | 0x0012 << 2 | 0 | 0 << 14;

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(
            IntPtr hDevice,
            uint IoControlCode,
            [In] IntPtr InBuffer,
            uint nInBufferSize,
            [Out] IntPtr OutBuffer,
            uint nOutBufferSize,
            out uint pBytesReturned,
            [In] IntPtr Overlapped);

        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateFile(
            [MarshalAs(UnmanagedType.LPWStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            [In] IntPtr templateFile);
    }
}
