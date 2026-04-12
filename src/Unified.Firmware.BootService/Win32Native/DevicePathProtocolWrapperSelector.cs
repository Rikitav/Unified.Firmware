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

using Unified.Firmware.BootService.UefiNative;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unified.Firmware.BootService.Protocols;

namespace Unified.Firmware.BootService.Win32Native;

/// <summary>
/// An assistant class that tells the marshaler what type to use to represent the native <see cref="EFI_DEVICE_PATH_PROTOCOL"/> structure
/// </summary>
public static class DevicePathProtocolWrapperSelector
{
    private static readonly Dictionary<short, Type> RegisteredWrapperTypes = new Dictionary<short, Type>()
    {
        { GetWrapperIdentificator(DeviceProtocolType.End, 0xFF), typeof(DevicePathProtocolEnd) }
    };

    private static readonly string[] KnownWrapperLibraries = [
        "Unified.Firmware.DevicePathProtocols.Hardware",
        "Unified.Firmware.DevicePathProtocols.Acpi",
        "Unified.Firmware.DevicePathProtocols.Messaging",
        "Unified.Firmware.DevicePathProtocols.Media",
        "Unified.Firmware.DevicePathProtocols.BiosBootSpecification"
    ];

    /// <summary>
    /// Tells if the marshaler should throw an exception when it encounters a protocol for which no wrapper is registered, instead of just returning a <see cref="RawMediaDevicePath"/> instance.
    /// </summary>
    public static bool DisallowCastingUnknownWrappersToRawProtocol { get; set; } = false;

    static DevicePathProtocolWrapperSelector()
    {
        EnumerateAssemblies();
    }

    private static void EnumerateAssemblies()
    {
        foreach (string assemblyName in KnownWrapperLibraries)
        {
            try
            {
                Assembly assembly = Assembly.Load(assemblyName);
                Type listType = assembly.GetType("Unified.Firmware.BootService.DevicePathProtocolTypeList");

                if (listType == null)
                    continue;

                RegisterTypesFromGeneratedList(listType);
            }
            catch
            {
                // Library doesnt exist in domain
            }
        }
    }

    private static void RegisterTypesFromGeneratedList(Type listType)
    {
        foreach (FieldInfo field in listType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.FieldType != typeof(Type))
                continue;

            Type wrapperType = (Type)field.GetValue(null)!;
            RegisterType(wrapperType);
        }
    }

    /// <summary>
    /// Retrieves the registered protocol wrapper type associated with the specified protocol type and subtype.
    /// </summary>
    /// <remarks>
    /// This method returns only types that have been previously registered using RegisterWrapperLibrary.
    /// If no matching type is found, the method returns null.
    /// </remarks>
    /// <param name="type">The protocol type for which to retrieve the registered wrapper type.</param>
    /// <param name="subType">The protocol subtype used to further identify the wrapper type. Typically represents a protocol variant or
    /// extension.</param>
    /// <returns>The registered wrapper type corresponding to the specified protocol type and subtype, or null if no matching type is registered.</returns>
    public static Type GetRegisteredType(DeviceProtocolType type, byte subType)
    {
        short wrapperIdentificator = GetWrapperIdentificator(type, subType);
        if (RegisteredWrapperTypes.TryGetValue(wrapperIdentificator, out Type? registered))
            return registered;

        if (!DisallowCastingUnknownWrappersToRawProtocol)
            return typeof(RawMediaDevicePath);

        throw new DeviceProtocolCastingException($"Failed to cast DevicePathProtocol of type {type} and subType {subType} to managed object. No wrapper found for this protocol.");
    }

    /// <summary>
    /// Registers a protocol wrapper type for later use in marshaling boot options
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void RegisterType<T>() where T : DevicePathProtocolBase
    {
        RegisterType(typeof(T));
    }

    /// <summary>
    /// Registers a protocol wrapper type for later use in marshaling boot options
    /// </summary>
    /// <param name="wrapperType"></param>
    /// <exception cref="InvalidInheritedClassException"></exception>
    /// <exception cref="MissingDevicePathProtocolWrapperAttributeException"></exception>
    public static void RegisterType(Type wrapperType)
    {
        if (!wrapperType.IsSubclassOf(typeof(DevicePathProtocolBase)))
            throw new InvalidInheritedClassException("The type of protocol wrapper must be inherited from class DevicePathProtocolBase");

        DefineDevicePathProtocolAttribute defineWrapperAttr = wrapperType.GetCustomAttribute<DefineDevicePathProtocolAttribute>();
        if (defineWrapperAttr == null)
            throw new MissingDevicePathProtocolWrapperAttributeException("The protocol wrapper does not have the required \'Unified.Firmware.BootService.DevicePathProtocols.DefineDevicePathProtocolAttribute\'");

        short wrapperIdentificator = GetWrapperIdentificator(defineWrapperAttr.Type, defineWrapperAttr.SubType);
        RegisteredWrapperTypes.Add(wrapperIdentificator, wrapperType);
    }

    private static short GetWrapperIdentificator(DeviceProtocolType type, byte subType)
        => (short)((byte)type | (subType << sizeof(byte)));
}
