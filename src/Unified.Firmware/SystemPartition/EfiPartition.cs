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

namespace Unified.Firmware.SystemPartition;

/// <summary>
/// System partition containing boot files of operating systems
/// </summary>
public static class EfiPartition
{
    private static readonly VolumePath _partitionInfo = FirmwareInterface.CurrentBackend.FindEfiSystemPartition();
    private static readonly Guid _partitionInfoTypeId = Guid.Parse("c12a7328-f81f-11d2-ba4b-00a0c93ec93b");

    /// <summary>
    /// GUID identifier of the system EFI partition.
    /// This is a unique identifier assigned to the partition when it is created,
    /// and can be used to identify the partition among other partitions on the same disk.
    /// It is not related to the partition type, and is not a constant value.
    /// It can be used to access the partition's files and directories or passed to Win32 API functions that require a volume path.
    /// </summary>
    public static Guid Identificator => _partitionInfo.Identificator;

    /// <summary>
    /// GUID identifier of the system EFI partition type.
    /// This is a constant value defined by the UEFI specification for EFI System Partitions,
    /// and is used to identify partitions that are intended to be used as EFI System Partitions.
    /// </summary>
    public static Guid TypeID => _partitionInfoTypeId;

    /// <summary>
    /// Full path to the system EFI partition, in the form of "\\?\Volume{GUID}\".
    /// This path can be used to access the partition's files and directories or passed to Win32 API functions that require a volume path.
    /// Note that this path is not a drive letter and cannot be used with functions that expect a drive letter.
    /// </summary>
    public static VolumePath VolumePath => _partitionInfo;
}
