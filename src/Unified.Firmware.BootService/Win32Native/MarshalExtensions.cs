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

using Unified.Firmware.BootService.LoadOption;
using Unified.Firmware.BootService.UefiNative;
using Unified.Firmware.Win32Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unified.Firmware.BootService.Protocols;

namespace Unified.Firmware.BootService.Win32Native;

/// <summary>
/// Provides extension methods for marshalling UEFI boot service structures.
/// </summary>
public static class MarshalExtensions
{
    private const int DevicePathProtocolHeaderLength = sizeof(byte) + sizeof(byte) + sizeof(ushort); // Type + SubType + DataLength
    private const int LoadOptionBaseHeaderLength = sizeof(LoadOptionAttributes) + sizeof(ushort); // Attributes + FilePathListLength
    
    /// <summary>
    /// Reads an EFI load option from the binary reader without processing device path protocols into specific types.
    /// </summary>
    /// <param name="reader">The binary reader.</param>
    /// <returns>The raw EFI load option.</returns>
    public static EFI_LOAD_OPTION ReadRawLoadOption(this BinaryReader reader)
    {
        // Starting manual marshalling strcture to managed
        EFI_LOAD_OPTION loadOption = new EFI_LOAD_OPTION();

        // Reading general data
        loadOption.Attributes = (LoadOptionAttributes)reader.ReadUInt32();  // Reading Attributes field
        loadOption.FilePathListLength = reader.ReadUInt16();                // Reading length of filepath list
        loadOption.Description = reader.ReadCstyleWideString();             // Reading Description (Load option name)
        loadOption.FilePathList = reader.ReadUntilRawEndProtocol();         // Reading Device path list

        // Manually seek to optional data position because EFI_DEVICE_PATH_PROTOCOL sequence not always property aligned
        int SeekLength = LoadOptionBaseHeaderLength + loadOption.Description.GetCstyleWideStringLength() + loadOption.FilePathListLength;
        reader.BaseStream.Seek(SeekLength, SeekOrigin.Begin);

        // Reading OptionalData field
        loadOption.OptionalData = reader.ReadRemainingBytes();
        return loadOption;
    }

    /// <summary>
    /// Reads a raw EFI device path protocol from the binary reader.
    /// </summary>
    /// <param name="reader">The binary reader.</param>
    /// <returns>The raw EFI device path protocol.</returns>
    public static EFI_DEVICE_PATH_PROTOCOL ReadRawDevicePathProtocol(this BinaryReader reader)
    {
        // Starting manual marshalling strcture to managed
        EFI_DEVICE_PATH_PROTOCOL protocol = new EFI_DEVICE_PATH_PROTOCOL();

        // Reading general data
        protocol.Type = (DeviceProtocolType)reader.ReadByte();                                  // Reading device type
        protocol.SubType = reader.ReadByte();                                                   // Reading device subType
        protocol.Length = reader.ReadUInt16();                                                  // Reading structure length
        protocol.Data = reader.ReadBytes(protocol.Length - DevicePathProtocolHeaderLength);     // Reading protocol data

        // Done
        return protocol;
    }

    /// <summary>
    /// Reads a firmware boot option from the binary reader.
    /// </summary>
    /// <param name="reader">The binary reader.</param>
    /// <returns>The firmware boot option.</returns>
    public static FirmwareBootOption ReadLoadOption(this BinaryReader reader)
    {
        return (FirmwareBootOption)reader.ReadLoadOption(typeof(FirmwareBootOption));
    }

    /// <summary>
    /// Reads a load option of the specified type from the binary reader.
    /// </summary>
    /// <param name="reader">The binary reader.</param>
    /// <param name="loadOptionType">The type of the load option to read.</param>
    /// <returns>The read load option.</returns>
    public static LoadOptionBase ReadLoadOption(this BinaryReader reader, Type loadOptionType)
    {
        // Starting manual marshalling strcture to managed
        LoadOptionBase loadOption = (LoadOptionBase)Activator.CreateInstance(loadOptionType);

        // Reading general data
        loadOption.Attributes = (LoadOptionAttributes)reader.ReadUInt32();  // Reading Attributes field
        ushort devicePathListLength = reader.ReadUInt16();                  // Reading length of filepath list
        loadOption.Description = reader.ReadCstyleWideString();             // Reading Description (Load option name)
        loadOption.Protocols = reader.ReadUntilEndProtocol();               // Reading Device path list

        // Manually seek to optional data position because EFI_DEVICE_PATH_PROTOCOL sequence not always property aligned
        int SeekLength = LoadOptionBaseHeaderLength + loadOption.Description.GetCstyleWideStringLength() + devicePathListLength;
        reader.BaseStream.Seek(SeekLength, SeekOrigin.Begin);

        // Reading OptionalData field
        loadOption.OptionalData = reader.ReadRemainingBytes();
        
        return loadOption;
    }

    /// <summary>
    /// Reads a device path protocol from the binary reader, automatically determining the specific protocol type.
    /// </summary>
    /// <param name="reader">The binary reader.</param>
    /// <returns>The device path protocol.</returns>
    /// <exception cref="InvalidDataException">Thrown when the protocol data length does not match the expected length.</exception>
    public static DevicePathProtocolBase ReadDevicePathProtocol(this BinaryReader reader)
    {
        // Starting manual marshalling strcture to managed
        DeviceProtocolType type = (DeviceProtocolType)reader.ReadByte();    // Reading device type
        byte subType = reader.ReadByte();                                   // Reading device subType
        ushort length = reader.ReadUInt16();                                // Reading structure length

        // Getting protocol wrapper type and creating protocol wrapper and deserializing protocol data
        Type protocolWrapperType = DevicePathProtocolWrapperSelector.GetRegisteredType(type, subType);
        if (protocolWrapperType == typeof(RawMediaDevicePath))
        {
            RawMediaDevicePath rawMediaDevice = new RawMediaDevicePath(type, subType);
            rawMediaDevice.Deserialize(reader, (ushort)(length - DevicePathProtocolHeaderLength));
            return rawMediaDevice;
        }
        else
        {
            DevicePathProtocolBase protocol = (DevicePathProtocolBase)Activator.CreateInstance(protocolWrapperType);

            // Deserializing protocol data
            protocol.Deserialize(reader, length);

            // Validating protocol data length
            if (protocol.GetSerializationDataLength() + DevicePathProtocolHeaderLength != length)
                throw new InvalidDataException("");

            return protocol;
        }
    }

    /// <summary>
    /// Reads a device path protocol of the specified type from the binary reader.
    /// </summary>
    /// <param name="reader">The binary reader.</param>
    /// <param name="devicePathType">The type of the device path protocol.</param>
    /// <returns>The device path protocol.</returns>
    /// <exception cref="DeviceProtocolCastingException">Thrown when the protocol type or subtype does not match the expected type.</exception>
    /// <exception cref="InvalidDataException">Thrown when the protocol data length does not match the expected length.</exception>
    public static DevicePathProtocolBase ReadDevicePathProtocol(this BinaryReader reader, Type devicePathType)
    {
        // Starting manual marshalling strcture to managed
        DeviceProtocolType type = (DeviceProtocolType)reader.ReadByte();    // Reading device type
        byte subType = reader.ReadByte();                                   // Reading device subType
        ushort length = reader.ReadUInt16();                                // Reading structure length

        // Creating protocol wrapper and deserializing protocol data
        DevicePathProtocolBase protocol = (DevicePathProtocolBase)Activator.CreateInstance(devicePathType);

        // Validating protocol wrapper type
        if (protocol.Type != type || protocol.SubType != subType)
            throw new DeviceProtocolCastingException($"Failed to cast DevicePathProtocol of type {type} and subType {subType} to managed object of type {devicePathType}. The wrapper has different Type or SubType.");

        // Deserializing protocol data
        protocol.Deserialize(reader, length);

        // Validating protocol data length
        if (protocol.GetSerializationDataLength() + DevicePathProtocolHeaderLength != length)
            throw new InvalidDataException("");

        return protocol;
    }

    /// <summary>
    /// Reads a load option of type T from the binary reader.
    /// </summary>
    /// <typeparam name="T">The type of the load option.</typeparam>
    /// <param name="reader">The binary reader.</param>
    /// <returns>The read load option.</returns>
    public static T ReadLoadOption<T>(this BinaryReader reader) where T : LoadOptionBase, new()
    {
        return (T)reader.ReadLoadOption(typeof(T));
    }

    /// <summary>
    /// Reads a device path protocol of type T from the binary reader.
    /// </summary>
    /// <typeparam name="T">The type of the device path protocol.</typeparam>
    /// <param name="reader">The binary reader.</param>
    /// <returns>The device path protocol.</returns>
    public static T ReadDevicePathProtocol<T>(this BinaryReader reader) where T : DevicePathProtocolBase, new()
    {
        return (T)reader.ReadDevicePathProtocol(typeof(T));
    }

    /// <summary>
    /// Writes a raw EFI load option to the binary writer.
    /// </summary>
    /// <param name="writer">The binary writer.</param>
    /// <param name="loadOption">The load option to write.</param>
    /// <returns>The binary writer.</returns>
    public static BinaryWriter WriteLoadOption(this BinaryWriter writer, EFI_LOAD_OPTION loadOption)
    {
        // Writing general data
        writer.Write((uint)loadOption.Attributes); // Writing attributes field
        writer.Write(loadOption.FilePathListLength); // Writing length of filepath list
        writer.WriteCstyleWideString(loadOption.Description); // Writing description (Load option name)

        // Writing filepath list
        foreach (EFI_DEVICE_PATH_PROTOCOL edpp in loadOption.FilePathList)
            writer.WriteDevicePathProtocol(edpp);

        // Writing optional data
        writer.Write(loadOption.OptionalData);

        // Flushing
        writer.Flush();

        // Done
        return writer;
    }

    /// <summary>
    /// Writes a raw EFI device path protocol to the binary writer.
    /// </summary>
    /// <param name="writer">The binary writer.</param>
    /// <param name="protocol">The protocol to write.</param>
    /// <returns>The binary writer.</returns>
    public static BinaryWriter WriteDevicePathProtocol(this BinaryWriter writer, EFI_DEVICE_PATH_PROTOCOL protocol)
    {
        // Writing general data
        writer.Write((byte)protocol.Type);  // Writing device type
        writer.Write(protocol.SubType);     // Writing device subType
        writer.Write(protocol.Length);      // Writing structure length
        writer.Write(protocol.Data);        // Writing protocol data

        // Done
        return writer;
    }

    /// <summary>
    /// Writes a load option to the binary writer.
    /// </summary>
    /// <typeparam name="T">The type of the load option.</typeparam>
    /// <param name="writer">The binary writer.</param>
    /// <param name="loadOption">The load option to write.</param>
    /// <returns>The binary writer.</returns>
    public static BinaryWriter WriteLoadOption<T>(this BinaryWriter writer, T loadOption) where T : LoadOptionBase
    {
        // Writing general data
        writer.Write((uint)loadOption.Attributes);                                                                              // Writing attributes field
        writer.Write((ushort)loadOption.Protocols.Sum(p => p.GetSerializationDataLength() + DevicePathProtocolHeaderLength));   // Writing length of filepath list
        writer.Write(loadOption.Description.GetCstyleWideStringLength());                                                       // Writing option description

        // Writing filepath list
        foreach (DevicePathProtocolBase edpp in loadOption.Protocols)
            writer.WriteDevicePathProtocol(edpp);

        if (loadOption.Protocols.Last() is not DevicePathProtocolEnd)
            writer.WriteDevicePathProtocol(new DevicePathProtocolEnd());

        // Writing optional data
        writer.Write(loadOption.OptionalData);

        // Done
        return writer;
    }

    /// <summary>
    /// Writes a device path protocol to the binary writer.
    /// </summary>
    /// <typeparam name="T">The type of the device path protocol.</typeparam>
    /// <param name="writer">The binary writer.</param>
    /// <param name="protocol">The protocol to write.</param>
    /// <returns>The binary writer.</returns>
    public static BinaryWriter WriteDevicePathProtocol<T>(this BinaryWriter writer, T protocol) where T : DevicePathProtocolBase
    {
        // Writing general data
        writer.Write((byte)protocol.Type);
        writer.Write(protocol.SubType);
        writer.Write(protocol.GetSerializationDataLength() + DevicePathProtocolHeaderLength);

        // Serializing protocol data
        protocol.Serialize(writer);

        // Done
        return writer;
    }

    /// <summary>
    /// Calculates the total structure length of the load option in bytes.
    /// </summary>
    /// <param name="loadOption">The load option.</param>
    /// <returns>The length in bytes.</returns>
    public static int GetStrcutureLength(this LoadOptionBase loadOption)
    {
        int structLength = LoadOptionBaseHeaderLength + loadOption.Description.GetCstyleWideStringLength();
        foreach (DevicePathProtocolBase devicePathProtocol in loadOption.Protocols)
            structLength += devicePathProtocol.GetStrctureLength();

        structLength += loadOption.OptionalData.Length;
        return structLength;
    }

    /// <summary>
    /// Calculates the total structure length of the device path protocol in bytes.
    /// </summary>
    /// <param name="devicePathProtocol">The device path protocol.</param>
    /// <returns>The length in bytes.</returns>
    public static int GetStrctureLength(this DevicePathProtocolBase devicePathProtocol)
    {
        return DevicePathProtocolHeaderLength + devicePathProtocol.GetSerializationDataLength();
    }

    private static EFI_DEVICE_PATH_PROTOCOL[] ReadUntilRawEndProtocol(this BinaryReader reader)
    {
        List<EFI_DEVICE_PATH_PROTOCOL> filePathListBuilder = new List<EFI_DEVICE_PATH_PROTOCOL>();
        while (!filePathListBuilder.LastOrDefault().IsEndProtocol())
            filePathListBuilder.Add(reader.ReadRawDevicePathProtocol());

        return filePathListBuilder.ToArray();
    }

    private static DevicePathProtocolBase[] ReadUntilEndProtocol(this BinaryReader reader)
    {
        List<DevicePathProtocolBase> filePathListBuilder = new List<DevicePathProtocolBase>();
        while (true)
        {
            DevicePathProtocolBase protocol = reader.ReadDevicePathProtocol();
            if (protocol.IsEndProtocol())
                break;

            filePathListBuilder.Add(protocol);
        }

        return filePathListBuilder.ToArray();
    }

    private static bool IsEndProtocol(this EFI_DEVICE_PATH_PROTOCOL protocol)
    {
        return protocol.Type == DeviceProtocolType.End && protocol.SubType == 0xFF;
    }

    private static bool IsEndProtocol(this DevicePathProtocolBase? protocol)
    {
        if (protocol == null)
            return false;

        return protocol.Type == DeviceProtocolType.End && protocol.SubType == 0xFF;
    }
}
