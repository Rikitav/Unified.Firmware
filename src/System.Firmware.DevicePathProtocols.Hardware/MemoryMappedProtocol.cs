// System.Firmware
// Copyright (C) 2024 Rikitav
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using System.IO;

namespace System.Firmware.BootService.Protocols;

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