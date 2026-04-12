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

using Unified.Firmware.Win32Native;
using System.IO;
using System;

namespace Unified.Firmware.BootService.Protocols;

/// <summary>
/// The Media Protocol Device Path is used to denote the protocol that is being used in a device path at the location of the path specified. Many protocols are inherent to the style of device path.
/// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#media-protocol-device-path
/// </summary>
[DefineDevicePathProtocol(DeviceProtocolType.Media, 5)]
public sealed class MediaProtocol() : DevicePathProtocolBase(DeviceProtocolType.Media, 5)
{
    /// <summary>
    /// The ID of the protocol
    /// </summary>
    public Guid ProtocolGUID { get; set; }

    /// <summary>
    /// Create new <see cref="MediaProtocol"/> protocol instance from protocol GUID identificator
    /// </summary>
    public MediaProtocol(Guid protocolGUID) : this()
        => ProtocolGUID = protocolGUID;

    /// <inheritdoc/>
    public override ushort GetSerializationDataLength()
        => 16; // GUID is 16 bytes long

    /// <inheritdoc/>
    public override void Deserialize(BinaryReader reader, ushort length)
        => ProtocolGUID = reader.ReadGuid();

    /// <inheritdoc/>
    public override void Serialize(BinaryWriter writer)
        => writer.WriteGuid(ProtocolGUID);

    /// <inheritdoc/>
    public override string ToString()
        => ProtocolGUID.ToString();
}
