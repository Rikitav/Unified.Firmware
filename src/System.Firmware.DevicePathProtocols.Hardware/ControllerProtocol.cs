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
/// Controller Device Path.
/// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#controller-device-path
/// </summary>
[DefineDevicePathProtocol(DeviceProtocolType.Hardware, 5)]
public sealed class ControllerProtocol() : DevicePathProtocolBase(DeviceProtocolType.Hardware, 5)
{
    /// <summary>
    /// Controller Number.
    /// </summary>
    public uint ControllerNumber { get; set; }

    /// <summary>
    /// Create new <see cref="ControllerProtocol"/> protocol instance from controller number
    /// </summary>
    /// <param name="controllerNumber"></param>
    public ControllerProtocol(uint controllerNumber) : this()
    {
        ControllerNumber = controllerNumber;
    }

    /// <inheritdoc/>
    public override ushort GetSerializationDataLength() => 4;

    /// <inheritdoc/>
    public override void Deserialize(BinaryReader reader, ushort length)
    {
        ControllerNumber = reader.ReadUInt32();
    }

    /// <inheritdoc/>
    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(ControllerNumber);
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"Ctrl({ControllerNumber})";
}