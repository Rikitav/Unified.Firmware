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
using System.IO;
using Unified.Firmware.BootService.Marshalling;

namespace Unified.Firmware.BootService.Protocols;

/// <summary>
/// Vendor-Defined Media Device Path
/// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#vendor-defined-media-device-path
/// </summary>
[DefineDevicePathProtocol(DeviceProtocolType.Media, 3)]
public class VendorDefinedProtocol() : DevicePathProtocolBase(DeviceProtocolType.Media, 3)
{
    /// <summary>
    /// Vendor-assigned GUID that defines the data that follows
    /// </summary>
    public Guid VendorGuid { get; set; }

    /// <summary>
    /// Vendor-defined variable size data.
    /// </summary>
    public byte[] VendorDefinedData { get; set; } = [];

    /// <inheritdoc/>
    public override ushort GetSerializationDataLength()
    {
        // VendorGuid (16 bytes) + VendorDefinedData
        return (ushort)(16 + VendorDefinedData.Length);
    }

    /// <inheritdoc/>
    public override void Deserialize(BinaryReader reader, ushort length)
    {
        VendorGuid = reader.ReadGuid();
        VendorDefinedData = reader.ReadBytes(length - 16);
    }

    /// <inheritdoc/>
    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(VendorGuid.ToByteArray());
        writer.Write(VendorDefinedData);
    }

    /// <inheritdoc/>
    public override string ToString()
        => VendorGuid.ToString();
}
