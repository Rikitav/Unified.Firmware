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
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Unified.Firmware.PlatformBackend.Win32Platform;
using Unified.Firmware.SystemPartition;

namespace Unified.Firmware.PlatformBackend;

internal class Win32PlatformBackend : IFirmwareBackend
{
    /// <inheritdoc/>
    public bool CheckFirmwareAvailablity()
    {
        _ = NativeMethods.GetFirmwareEnvironmentVariableExW("", "{00000000-0000-0000-0000-000000000000}", IntPtr.Zero, 0, out _);
        return Marshal.GetLastWin32Error() != NativeMethods.ERROR_INVALID_FUNCTION;
    }

    /// <inheritdoc/>
    public VolumePath FindEfiSystemPartition()
    {
        if (!FirmwareInterface.Available)
            throw new PlatformNotSupportedException("Executing on non UEFI System");

        foreach (PARTITION_INFORMATION_EX partition in new IoctlVolumeEnumerable(0))
        {
            if (partition.PartitionStyle != PartitionStyle.GuidPartitionTable)
                throw new DriveNotFoundException("Drive signature is not GPT (Guid Partition Table)");

            if (partition.Gpt.PartitionType == EfiPartition.TypeID)
                return new VolumePath(partition.Gpt.PartitionId, string.Concat(@"\\?\Volume{", partition.Gpt.PartitionId.ToString(), @"}\"));
        }

        throw new DriveNotFoundException("Efi partition was not found");
    }

    /// <inheritdoc/>
    public IntPtr ReadEnvironmentVariable(string varName, Guid environmentIdentificator, out VariableAttributes attributes, int bufferSize, out uint dataSize)
    {
        // Data
        IntPtr varDataBuffer = IntPtr.Zero;
        if (!FirmwareInterface.Available)
            throw new PlatformNotSupportedException("This system does not support UEFI, or is loaded in LEGACY mode");

        try
        {
            // Allocate variable pointer
            varDataBuffer = Marshal.AllocHGlobal(bufferSize);

            // Reading variable
            using (new SystemEnvironmentPrivilege())
            {
                dataSize = NativeMethods.GetFirmwareEnvironmentVariableExW(varName,
                    "{" + environmentIdentificator.ToString() + "}",
                    varDataBuffer,
                    bufferSize,
                    out attributes);
            }

            // Error check
            if (dataSize == 0)
            {
                Marshal.FreeHGlobal(varDataBuffer);
                int errorCode = Marshal.GetLastWin32Error();

                /*
                if (errorCode == NativeMethods.ERROR_ENVVAR_NOT_FOUND)
                    return IntPtr.Zero;
                */

                throw new Win32Exception(errorCode);
            }

            return varDataBuffer;
        }
        catch (Exception ex)
        {
            // Something wrong happened
            Marshal.FreeHGlobal(varDataBuffer);
            throw new FirmwareEnvironmentException("Failed to read environment variable", ex);
        }
    }

    /// <inheritdoc/>
    public void WriteEnvironmentVariable(string varName, Guid environmentIdentificator, VariableAttributes attributes, IntPtr valueBuffer, int bufferSize)
    {
        if (!FirmwareInterface.Available)
            throw new PlatformNotSupportedException("This system does not support UEFI, or is loaded in LEGACY mode");

        try
        {
            // Execution and error check
            using (new SystemEnvironmentPrivilege())
            {
                if (!NativeMethods.SetFirmwareEnvironmentVariableExW(varName, "{" + environmentIdentificator.ToString() + "}", valueBuffer, bufferSize, attributes))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }
            }
        }
        catch (Exception ex)
        {
            // Something wrong happened
            throw new FirmwareEnvironmentException("Failed to write environment variable", ex);
        }
    }

    private static class NativeMethods
    {
        public const int ERROR_INVALID_FUNCTION = 1;
        public const int ERROR_ENVVAR_NOT_FOUND = 203;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint GetFirmwareEnvironmentVariableExW(
            string lpName, string lpGuid, IntPtr pBuffer, int nSize, out VariableAttributes Attributes);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SetFirmwareEnvironmentVariableExW(
            string lpName, string lpGuid, IntPtr pValue, int nSize, VariableAttributes Attributes);
    }
}
