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

using System.IO;

namespace Unified.Firmware.BootService.Protocols;

/// <summary>
/// The CD-ROM Media Device Path is used to define a system partition that exists on a CD-ROM. The CD-ROM is assumed to contain an ISO-9660 file system and follow the CD-ROM “El Torito” format
/// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#cd-rom-media-device-path
/// </summary>
[DefineDevicePathProtocol(DeviceProtocolType.Media, 2)]
public sealed class CdRomProtocol() : DevicePathProtocolBase(DeviceProtocolType.Media, 2)
{
    /// <summary>
    /// Boot Entry number from the Boot Catalog. The Initial/Default entry is defined as zero.
    /// </summary>
    public uint BootEntry { get; set; }

    /// <summary>
    /// Starting RBA of the partition on the medium. CD-ROMs use Relative logical Block Addressing.
    /// </summary>
    public ulong PartitionStart { get; set; }

    /// <summary>
    /// Size of the partition in units of Blocks, also called Sectors.
    /// </summary>
    public ulong PartitionSize { get; set; }

    /// <summary>
    /// Create new <see cref="CdRomProtocol"/> protocol instance from boot entry and partition offset
    /// </summary>
    public CdRomProtocol(uint bootEntry, ulong partitionStart, ulong partitionSize) : this()
    {
        BootEntry = bootEntry;
        PartitionStart = partitionStart;
        PartitionSize = partitionSize;
    }

    /// <inheritdoc/>
    public override ushort GetSerializationDataLength()
    {
        // BootEntry (4 bytes) + PartitionStart (8 bytes) + PartitionSize (8 bytes)
        return sizeof(uint) + sizeof(ulong) + sizeof(ulong);
    }

    /// <inheritdoc/>
    public override void Deserialize(BinaryReader reader, ushort length)
    {
        BootEntry = reader.ReadUInt32();
        PartitionStart = reader.ReadUInt64() * 512;
        PartitionSize = reader.ReadUInt64() * 512;
    }

    /// <inheritdoc/>
    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(BootEntry);
        writer.Write(PartitionStart / 512);
        writer.Write(PartitionSize / 512);
    }

    /// <inheritdoc/>
    public override string ToString()
        => BootEntry.ToString();
}
