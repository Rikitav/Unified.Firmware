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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Unified.Firmware.BootService.LoadOption;
using Unified.Firmware.BootService.Marshalling;
using Unified.Firmware.BootService.UefiNative;
using Unified.Firmware.EnvironmentVendor;

namespace Unified.Firmware.BootService;

/// <summary>
/// Provides methods to read, write, update, and delete boot entries from your computer's UEFI NVRAM
/// </summary>
public static class FirmwareBootService
{
    /// <summary>
    /// Gets the index of the boot record this computer was booted from. Use this index in methods of this <see cref="FirmwareBootService"/>
    /// </summary>
    public static BootOptionIndex CurrentLoadOptionIndex
    {
        get => FirmwareEnvironment.Global.BootCurrent;
    }

    /// <summary>
    /// Sets the index of the boot entry that will be loaded next time ONCE. Use this index in methods of this class
    /// </summary>
    public static BootOptionIndex NextLoadOptionIndex
    {
        set => FirmwareEnvironment.Global.BootNext = value;
    }

    /// <summary>
    /// Gets or sets the boot order. The array contains the indexes of all boot entries in the order in which they will be loaded. Use this index in methods of this class
    /// </summary>
    public static BootOptionIndex[] LoadOrder
    {
        get => [.. FirmwareEnvironment.Global.BootOrder];
        set => FirmwareEnvironment.Global.BootOrder = [.. value.Select(x => x)];
    }

    /// <summary>
    /// Resets the Boot#### variable at the specified index and removes it from the boot order
    /// </summary>
    /// <param name="loadOptionIndex">The boot entry index (<c>Boot####</c>) to delete.</param>
    public static void DeleteLoadOption(BootOptionIndex loadOptionIndex)
    {
        // Writing null variable to firmware
        FirmwareInterface.CurrentBackend.WriteEnvironmentVariable(
            loadOptionIndex.ToString(), // Boot####
            FirmwareVendors.GlobalVariable,
            VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS,
            IntPtr.Zero, 0);

        // Removing index from boot order
        LoadOrder = [.. LoadOrder.Where(x => x != loadOptionIndex)];
    }

    /// <summary>
    /// Lists all load options in boot order
    /// </summary>
    /// <returns>A sequence of boot options in the current boot order.</returns>
    public static IEnumerable<FirmwareBootOption> EnumerateBootOptions()
    {
        return LoadOrder.Select(ReadLoadOption);
    }

    /// <summary>
    /// Lists all load options in boot order
    /// </summary>
    /// <typeparam name="T">The concrete <see cref="LoadOptionBase"/> type to deserialize each entry as.</typeparam>
    /// <returns>A sequence of boot options in the current boot order, deserialized as <typeparamref name="T"/>.</returns>
    public static IEnumerable<T> EnumrateBootOptions<T>() where T : LoadOptionBase, new()
    {
        return LoadOrder.Select(ReadLoadOption<T>);
    }

    /// <summary>
    /// Lists all load options in boot order
    /// </summary>
    /// <returns>A sequence of raw EFI load option structures in the current boot order.</returns>
    public static IEnumerable<EFI_LOAD_OPTION> EnumrateRawBootOptions()
    {
        return LoadOrder.Select(ReadRawLoadOption);
    }

    /// <summary>
    /// Reads the boot option from NVRAM, issues it to the delegate for processing and updates the variable at the specified index
    /// </summary>
    /// <param name="bootOptionIndex">The boot entry index (<c>Boot####</c>) to read and write.</param>
    /// <param name="UpdateOptionAction">A callback that receives the deserialized option and mutates it before it is written back.</param>
    public static void EditLoadOption(BootOptionIndex bootOptionIndex, Action<FirmwareBootOption> UpdateOptionAction)
    {
        FirmwareBootOption bootOption = ReadLoadOption(bootOptionIndex);
        UpdateOptionAction(bootOption);
        WriteFirmwareLoadOption(bootOption, bootOptionIndex);
    }

    /// <summary>
    /// Creates a new load option at the first free index, writes into it a serialized copy of the <see cref="LoadOptionBase"/> instance passed to the function, and returns the index of the new entry. Specify the <paramref name="AddFirst"/> parameter as <see langword="true"/> to add the option as the first boot option
    /// </summary>
    /// <param name="loadOption">The load option to serialize and store in NVRAM.</param>
    /// <param name="AddFirst">If <see langword="true"/>, inserts the new entry at the start of the boot order; otherwise appends it.</param>
    /// <returns>The <see cref="BootOptionIndex"/> assigned to the new <c>Boot####</c> variable.</returns>
    /// <exception cref="FreeLoadOptionIndexNotFound"></exception>
    public static BootOptionIndex CreateLoadOption(LoadOptionBase loadOption, bool AddFirst)
    {
        // Getting free variable name
        BootOptionIndex? freeLoadOptionIndex = GetFirstFreeLoadOptionName();
        if (!freeLoadOptionIndex.HasValue)
            throw new FreeLoadOptionIndexNotFound("Failed to find free loadOption name");

        // Creating variable
        BootOptionIndex newLoadOptionIndex = freeLoadOptionIndex.Value;
        WriteFirmwareLoadOption(loadOption, newLoadOptionIndex);

        // Setting new boot order
        FirmwareEnvironment.Global.BootOrder = AddFirst
            ? [newLoadOptionIndex, ..FirmwareEnvironment.Global.BootOrder]
            : [..FirmwareEnvironment.Global.BootOrder, newLoadOptionIndex];

        return newLoadOptionIndex;
    }

    /// <summary>
    /// Writes serialized copy of the <see cref="LoadOptionBase"/> instance passed to the function into existing load option variable at the specified index
    /// </summary>
    /// <param name="loadOption">The load option to serialize and write.</param>
    /// <param name="bootOptionIndex">The boot entry index (<c>Boot####</c>) to update.</param>
    public static void UpdateLoadOption(LoadOptionBase loadOption, BootOptionIndex bootOptionIndex)
    {
        // Updating variable
        WriteFirmwareLoadOption(loadOption, bootOptionIndex);
    }

    /// <summary>
    /// Reads the native representation of the boot option from NVRAM
    /// </summary>
    /// <param name="bootOptionIndex">The boot entry index (<c>Boot####</c>) to read.</param>
    /// <returns>The raw EFI load option structure for that entry.</returns>
    public static EFI_LOAD_OPTION ReadRawLoadOption(BootOptionIndex bootOptionIndex)
    {
        using BinaryReader reader = ReadFirmwareLoadOption(bootOptionIndex);
        return reader.ReadRawLoadOption();
    }

    /// <summary>
    /// Reads boot option from NVRAM
    /// </summary>
    /// <param name="bootOptionIndex">The boot entry index (<c>Boot####</c>) to read.</param>
    /// <returns>A deserialized <see cref="FirmwareBootOption"/> for that entry.</returns>
    public static FirmwareBootOption ReadLoadOption(BootOptionIndex bootOptionIndex)
    {
        using BinaryReader reader = ReadFirmwareLoadOption(bootOptionIndex);
        return reader.ReadLoadOption<FirmwareBootOption>();
    }

    /// <summary>
    /// Reads a boot option from NVRAM and converts it to the specified type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The concrete <see cref="LoadOptionBase"/> type to deserialize.</typeparam>
    /// <param name="bootOptionIndex">The boot entry index (<c>Boot####</c>) to read.</param>
    /// <returns>A deserialized instance of <typeparamref name="T"/> for that entry.</returns>
    public static T ReadLoadOption<T>(BootOptionIndex bootOptionIndex) where T : LoadOptionBase, new()
    {
        using BinaryReader reader = ReadFirmwareLoadOption(bootOptionIndex);
        return reader.ReadLoadOption<T>();
    }

    private static BinaryReader ReadFirmwareLoadOption(BootOptionIndex loadOptionIndex)
    {
        // Getting variable data
        IntPtr pointer = FirmwareInterface.CurrentBackend.ReadEnvironmentVariable(
            loadOptionIndex.ToString(), // Boot####
            FirmwareVendors.GlobalVariable,
            out _, 1024, out uint dataLength);

        return new BinaryReader(new MemoryPointerStream(pointer, (int)dataLength, false));
    }

    private static void WriteFirmwareLoadOption(LoadOptionBase loadOption, BootOptionIndex loadOptionIndex)
    {
        // Marshalling structure to unmanaged memory pointer
        int structureLength = loadOption.GetStructureLength();
        using MemoryPointerStream pointer = new MemoryPointerStream(structureLength);

        // Serializing structure to memory
        using (BinaryWriter writer = new BinaryWriter(pointer, Encoding.Unicode, true))
            writer.WriteLoadOption(loadOption);

        // Writing variable to firmware
        FirmwareInterface.CurrentBackend.WriteEnvironmentVariable(
            loadOptionIndex.ToString(), // Boot####
            FirmwareVendors.GlobalVariable,
            VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS,
            pointer.Buffer,
            structureLength);
    }

    private static BootOptionIndex? GetFirstFreeLoadOptionName()
    {
        for (ushort i = 0; i < ushort.MaxValue; i++)
        {
            if (!LoadOrder.Contains(i))
                return i;
        }

        return null;
    }
}
