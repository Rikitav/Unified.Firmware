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
/// A <see href="https://uefi.org/specs/UEFI/2.10/10_Protocols_Device_Path_Protocol.html#generic-device-path-structures">Device Path</see> is used to define the programmatic path to a device. The primary purpose of a Device Path is to allow an application, such as an OS loader, to determine the physical device that the interfaces are abstracting.
/// </summary>
/// <remarks>
/// Abstract constructor
/// </remarks>
public abstract class DevicePathProtocolBase(DeviceProtocolType type, byte subType)
{
    private readonly DeviceProtocolType _Type = type;
    private readonly byte _SubType = subType;

    /// <summary>
    /// Protocol type
    /// </summary>
    public DeviceProtocolType Type => _Type;

    /// <summary>
    /// Protocol Sub-Type - Varies by Type
    /// </summary>
    public byte SubType => _SubType;

    // TODO: move from base class to a helper class
    /*
    /// <summary>
    /// Create protocol wrapper of Type and deserailize structure
    /// </summary>
    /// <param name="protocolType"></param>
    /// <param name="protocol"></param>
    /// <returns></returns>
    internal static DevicePathProtocolBase CreateProtocol(Type protocolType, EFI_DEVICE_PATH_PROTOCOL protocol)
    {
        DevicePathProtocolBase? protocolWrapper = (DevicePathProtocolBase?)Activator.CreateInstance(protocolType);
        if (protocolWrapper == null)
            throw new DeviceProtocolCastingException("Failed to cast DevicePathProtocol of type {0} and subType {1} to managed object");

        protocolWrapper.Deserialize(protocol.Data);
        return protocolWrapper;
    }

    internal byte[] GetSerializingData()
    {
        return Serialize();
    }
    */

    /// <summary>
    /// Returns the length, in bytes, of the data required to serialize the current object.
    /// </summary>
    /// <remarks>
    /// Implementations should ensure that the returned length accurately reflects the size of the serialized data for the current object state.
    /// This value can be used to allocate buffers or validate storage requirements before serialization.
    /// </remarks>
    /// <returns>The number of bytes needed to serialize the object. The value is always non-negative.</returns>
    public abstract ushort GetSerializationDataLength();

    /// <summary>
    /// Deserialize raw data into managed data. 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="length"></param>
    public abstract void Deserialize(BinaryReader reader, ushort length);

    /// <summary>
    /// Serialize managed data into raw data
    /// </summary>
    /// <returns></returns>
    public abstract void Serialize(BinaryWriter writer);
}
