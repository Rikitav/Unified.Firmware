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
/// The default instance for protocols, has no abstractions, but only raw data
/// </summary>
public class RawMediaDevicePath(DeviceProtocolType type, byte subType) : DevicePathProtocolBase(type, subType)
{
    /// <summary>
    /// <para>GUID - An EFI GUID in standard format xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx.</para>
    /// <para>Keyword - In some cases, one of a series of keywords must be listed.</para>
    /// <para>Integer - Unless otherwise specified, this indicates an unsigned integer in the range of 0 to 2^32-1. The value is decimal, unless preceded by �0x� or �0X�, in which case it is hexadecimal.</para>
    /// <para>EISAID - A seven character text identifier in the format used by the ACPI specification. The first three characters must be alphabetic, either upper or lower case. The second four characters are hexadecimal digits, either numeric, upper case or lower case. Optionally, it can be the number 0.</para>
    /// <para>String - Series of alphabetic, numeric and punctuation characters not including a right parenthesis �)�, bar �</para>
    /// <para>HexDump - Series of bytes, represented by two hexadecimal characters per byte. Unless otherwise indicated, the size is only limited by the length of the device node.</para>
    /// <para>IPv4 Address - Series of four integer values (each between 0-255), separated by a �.� Optionally, followed by a �:� and an integer value between 0-65555. If the �:� is not present, then the port value is zero.</para>
    /// <para>IPv6 Address - IPv6 Address is expressed in the format [address]:port. The �address� is expressed in the way defined in RFC4291 Section 2.2. The �:port� after the [address] is optional. If present, the �port� is an integer value between 0-65535 to represent the port number, or else, port number is zero.</para>
    /// </summary>
    public byte[] ProtocolData { get; private set; } = [];

    /// <summary>
    /// Create new <see cref="RawMediaDevicePath"/> protocol instance from raw protocol data
    /// </summary>
    /// <param name="type"></param>
    /// <param name="subType"></param>
    /// <param name="protocolData"></param>
    public RawMediaDevicePath(DeviceProtocolType type, byte subType, byte[] protocolData) : this(type, subType)
    {
        ProtocolData = protocolData;
    }

    /// <inheritdoc/>
    public override ushort GetSerializationDataLength()
        => (ushort)ProtocolData.Length;

    /// <inheritdoc/>
    public override void Deserialize(BinaryReader reader, ushort length)
        => ProtocolData = reader.ReadBytes(length);

    /// <inheritdoc/>
    public override void Serialize(BinaryWriter writer)
        => writer.Write(ProtocolData);
}
