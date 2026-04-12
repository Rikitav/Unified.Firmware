// The MIT License(MIT)
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

namespace Unified.Firmware.PlatformBackend.LinuxPlatform;

internal record struct GptPartitionModel(
    GPT_PARTITION_ENTRY Entry,
    string BlockDevicePath,
    string VolumePath);

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct GPT_PARTITION_HEADER
{
    public ulong Signature; // "EFI PART"
    public uint Revision;
    public uint HeaderSize;
    public uint HeaderCrc32;
    public uint Reserved;
    public ulong MyLba;
    public ulong AlternateLba;
    public ulong FirstUsableLba;
    public ulong LastUsableLba;
    public Guid DiskGuid;
    public ulong PartitionEntryLba;
    public uint NumberOfPartitionEntries;
    public uint SizeOfPartitionEntry;
    public uint PartitionEntryArrayCrc32;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct GPT_PARTITION_ENTRY
{
    public Guid PartitionTypeGuid;
    public Guid UniquePartitionGuid;
    public ulong StartingLba;
    public ulong EndingLba;
    public ulong Attributes;
    public unsafe fixed char PartitionName[36]; // Uses UTF-16LE encoding. sizeof(char) = 2. 2 * 36 = 72 bytes of data
}