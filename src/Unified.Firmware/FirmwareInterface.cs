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

using Unified.Firmware.SystemPartition;
using System;
using System.IO;
using Unified.Firmware.PlatformBackend;

namespace Unified.Firmware;

public static class FirmwareInterface
{
    public static IFirmwareBackend CurrentBackend
    {
        get => field ??= Environment.OSVersion.Platform switch
        {
            PlatformID.Win32NT => new Win32PlatformBackend(),
            PlatformID.Unix => new LinuxPlatfromBackend(),
            _ => throw new PlatformNotSupportedException()
        };
    }

    /// <summary>
    /// Checks whether the UEFI platform is available on this system
    /// </summary>
    /// <returns>If available, return <see langword="true"/>, else <see langword="false"/></returns>
    public static bool Available
    {
        get => CurrentBackend.CheckFirmwareAvailablity();
    }

    /// <summary>
    /// Searches among the partition for the one that is marked as EfiSystemPartition
    /// </summary>
    /// <returns><see cref="DirectoryInfo"/> of EfiSystemPartition</returns>
    public static DirectoryInfo SystemPartition
    {
        get
        {
            if (!Available)
                throw new PlatformNotSupportedException("This system does not support UEFI, or is loaded in LEGACY mode");

            return EfiPartition.VolumePath;
        }
    }

    /// <summary>
    /// Boot into the UEFI user interface after rebooting the computer. Does NOT reboot the computer, but sets the boot condition
    /// </summary>
    public static void BootToUserInterface()
    {
        if (!Available)
            throw new PlatformNotSupportedException("This system does not support UEFI, or is loaded in LEGACY mode");

        if (!FirmwareEnvironment.Global.OsIndicationsSupported.HasFlag(OsIndications.BOOT_TO_FW_UI))
            throw new PlatformNotSupportedException("Current UEFI platform does not support force reboot in Firmware UI");

        FirmwareEnvironment.Global.OsIndications |= OsIndications.BOOT_TO_FW_UI;
    }
}
