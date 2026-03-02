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
/// PCI Device Path.
/// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#pci-device-path
/// </summary>
[DefineDevicePathProtocol(DeviceProtocolType.Hardware, 1)]
public sealed class PciDevicePath() : DevicePathProtocolBase(DeviceProtocolType.Hardware, 1)
{
    /// <summary>
    /// PCI Function Number.
    /// </summary>
    public byte Function { get; set; }

    /// <summary>
    /// PCI Device Number.
    /// </summary>
    public byte Device { get; set; }

    /// <summary>
    /// Create new <see cref="PciDevicePath"/> protocol instance from PCI function and device numbers
    /// </summary>
    /// <param name="function"></param>
    /// <param name="device"></param>
    public PciDevicePath(byte function, byte device) : this()
    {
        Function = function;
        Device = device;
    }

    /// <inheritdoc/>
    public override ushort GetSerializationDataLength() => 2;

    /// <inheritdoc/>
    public override void Deserialize(BinaryReader reader, ushort length)
    {
        Function = reader.ReadByte();
        Device = reader.ReadByte();
    }

    /// <inheritdoc/>
    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(Function);
        writer.Write(Device);
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"Pci(0x{Device:X}, 0x{Function:X})";
}