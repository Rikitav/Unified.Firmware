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

namespace Unified.Firmware.BootService.Protocols;

/// <summary>
/// File Path Media Device Path
/// https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#file-path-media-device-path
/// </summary>
[DefineDevicePathProtocol(DeviceProtocolType.Media, 4)]
public sealed class FilePathProtocol() : DevicePathProtocolBase(DeviceProtocolType.Media, 4)
{
    /// <summary>
    /// A NULL-terminated Path string including directory and file name.
    /// </summary>
    public string PathName { get; set; } = string.Empty;

    /// <summary>
    /// Create new <see cref="FilePathProtocol"/> protocol instance from file path
    /// </summary>
    public FilePathProtocol(string pathName) : this()
        => PathName = pathName;

    /// <inheritdoc/>
    public override ushort GetSerializationDataLength()
        => PathName.GetCstyleWideStringLength();

    /// <inheritdoc/>
    public override void Deserialize(BinaryReader reader, ushort length)
        => PathName = reader.ReadCstyleWideString();

    /// <inheritdoc/>
    public override void Serialize(BinaryWriter writer)
        => writer.WriteCstyleWideString(PathName);

    /// <inheritdoc/>
    public override string ToString()
        => PathName;
}
