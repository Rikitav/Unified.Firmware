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

using System.Firmware.Win32Native;
using System.IO;

namespace System.Firmware.BootService.Protocols;

/// <summary>
/// Vendor Defined Device Path.
/// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#vendor-defined-device-path
/// </summary>
[DefineDevicePathProtocol(DeviceProtocolType.Hardware, 4)]
public sealed class VendorProtocol() : DevicePathProtocolBase(DeviceProtocolType.Hardware, 4)
{
    /// <summary>
    /// Vendor GUID.
    /// </summary>
    public Guid VendorGuid { get; set; }

    /// <summary>
    /// Vendor Defined Data.
    /// </summary>
    public byte[] VendorDefinedData { get; set; } = [];

    /// <inheritdoc/>
    public override ushort GetSerializationDataLength() => (ushort)(16 + VendorDefinedData.Length);

    /// <inheritdoc/>
    public override void Deserialize(BinaryReader reader, ushort length)
    {
        VendorGuid = reader.ReadGuid();

        // Total Length - Header(4) - Guid(16)
        int dataLength = length - 4 - 16;
        
        if (dataLength > 0)
            VendorDefinedData = reader.ReadBytes(dataLength);
    }

    /// <inheritdoc/>
    public override void Serialize(BinaryWriter writer)
    {
        writer.WriteGuid(VendorGuid);
        writer.Write(VendorDefinedData);
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"Vendor({VendorGuid})";
}