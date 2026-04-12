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
using System.Runtime.InteropServices;

namespace Unified.Firmware.PlatformBackend.Win32Platform;
#pragma warning disable CS1591, RCS1135, RCS1154

/// <summary>
/// The Extensible Firmware Interface (EFI) attributes of the partition.
/// </summary>
[Flags]
public enum EfiPartitionAttribute : ulong
{
    /// <summary>
    /// If this attribute is set, the partition is required by a computer to function properly. 
    /// </summary>
    PlatformRequired = 0x0000000000000001,

    /// <summary>
    /// If this attribute is set, the partition does not receive a drive letter by default when the disk is moved to another computer or when the disk is seen for the first time by a computer. 
    /// </summary>
    NoDriveLetter = 0x8000000000000000,

    /// <summary>
    /// If this attribute is set, the partition is not detected by the Mount Manager. 
    /// </summary>
    Hidden = 0x4000000000000000,

    /// <summary>
    /// If this attribute is set, the partition is a shadow copy of another partition.
    /// </summary>
    ShadowCopy = 0x2000000000000000,

    /// <summary>
    /// If this attribute is set, the partition is read-only. 
    /// </summary>
    ReadOnly = 0x1000000000000000
}

/// <summary>
/// Specifies the partition style used on a disk, such as Master Boot Record (MBR), GUID Partition Table (GPT), or raw (uninitialized).
/// </summary>
public enum PartitionStyle : uint
{
    MasterBootRecord,
    GuidPartitionTable,
    Raw
}

/// <summary>
/// Contains extended information about a drive's partitions.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct DRIVE_LAYOUT_INFORMATION_EX
{
    [FieldOffset(0)] public PartitionStyle PartitionStyle;
    [FieldOffset(4)] public uint PartitionCount;
    [FieldOffset(8)] public IntPtr Mbr;
    [FieldOffset(8)] public IntPtr Gpt;
    [FieldOffset(48)] public PARTITION_INFORMATION_EX PartitionEntry;
}

/// <summary>
/// Contains GUID partition table (GPT) partition information.
/// </summary>
[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
public struct PARTITION_INFORMATION_GPT
{
    [FieldOffset(0)] public Guid PartitionType;
    [FieldOffset(16)] public Guid PartitionId;
    [FieldOffset(32)] public ulong Attributes;
    [FieldOffset(40), MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)] public string Name;
}

/// <summary>
/// Contains information about a disk partition.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct PARTITION_INFORMATION_EX
{
    [FieldOffset(0)] public PartitionStyle PartitionStyle;
    [FieldOffset(8)] public long StartingOffset;
    [FieldOffset(16)] public long PartitionLength;
    [FieldOffset(24)] public uint PartitionNumber;
    [FieldOffset(28), MarshalAs(UnmanagedType.I1)] public bool RewritePartition;
    [FieldOffset(32)] public IntPtr Mbr;
    [FieldOffset(32)] public PARTITION_INFORMATION_GPT Gpt;
}
