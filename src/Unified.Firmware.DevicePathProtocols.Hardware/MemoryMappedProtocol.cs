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
/// Memory Mapped Device Path.
/// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#memory-mapped-device-path
/// </summary>
[DefineDevicePathProtocol(DeviceProtocolType.Hardware, 3)]
public sealed class MemoryMappedProtocol() : DevicePathProtocolBase(DeviceProtocolType.Hardware, 3)
{
    /// <summary>
    /// Memory Type.
    /// </summary>
    public uint MemoryType { get; set; }

    /// <summary>
    /// Starting Memory Address.
    /// </summary>
    public ulong StartAddress { get; set; }

    /// <summary>
    /// Ending Memory Address.
    /// </summary>
    public ulong EndAddress { get; set; }

    /// <inheritdoc/>
    public override ushort GetSerializationDataLength() => 20;

    /// <inheritdoc/>
    public override void Deserialize(BinaryReader reader, ushort length)
    {
        MemoryType = reader.ReadUInt32();
        StartAddress = reader.ReadUInt64();
        EndAddress = reader.ReadUInt64();
    }

    /// <inheritdoc/>
    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(MemoryType);
        writer.Write(StartAddress);
        writer.Write(EndAddress);
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"MemoryMapped(0x{MemoryType:X}, 0x{StartAddress:X}, 0x{EndAddress:X})";
}