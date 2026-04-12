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
/// PCI Device Path.
/// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#pci-device-path
/// </summary>
[DefineDevicePathProtocol(DeviceProtocolType.Hardware, 1)]
public sealed class PciProtocol() : DevicePathProtocolBase(DeviceProtocolType.Hardware, 1)
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
    /// Create new <see cref="PciProtocol"/> protocol instance from PCI function and device numbers
    /// </summary>
    /// <param name="function"></param>
    /// <param name="device"></param>
    public PciProtocol(byte function, byte device) : this()
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