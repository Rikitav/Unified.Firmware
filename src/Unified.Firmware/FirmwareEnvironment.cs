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
using System.Runtime.InteropServices;
using Unified.Firmware.EnvironmentVendor;

namespace Unified.Firmware;

/// <summary>
/// Provides read and write access to UEFI firmware environment variables in a vendor-specific namespace.
/// </summary>
public class FirmwareEnvironment(IFirmwareBackend backend, Guid vendorGuid)
{
    /// <summary>
    /// Gets the firmware backend used to access environment variables.
    /// </summary>
    public IFirmwareBackend Backend => backend;

    /// <summary>
    /// Gets the vendor GUID that identifies the variable namespace for this environment.
    /// </summary>
    public Guid VendorGuid => vendorGuid;

    /// <summary>
    /// Gets the shared global firmware environment, backed by <see cref="FirmwareInterface.CurrentBackend"/>.
    /// </summary>
    public static GlobalFirmwareEnvironment Global => field ??= new GlobalFirmwareEnvironment(FirmwareInterface.CurrentBackend);

    /// <summary>
    /// Retrieves the value of a environment variable as a string.
    /// </summary>
    /// <remarks>
    /// The returned value is read from the environment. If the specified variable is not found, the method returns null.
    /// This method allocates and frees unmanaged memory internally, callers do not need to manage memory for the returned string.
    /// </remarks>
    /// <param name="varName">The name of the environment variable to retrieve. Cannot be null or empty.</param>
    /// <param name="attributes">When this method returns, contains the attributes of the variable.</param>
    /// <returns>A string containing the value of the specified environment variable, or null if the variable does not exist.</returns>
    public string ReadStringVariable(string varName, out VariableAttributes attributes)
    {
        IntPtr pointer = Backend.ReadEnvironmentVariable(varName, VendorGuid, out attributes, 2 * 1024, out _);
        string varVal = Marshal.PtrToStringUni(pointer);

        Marshal.FreeHGlobal(pointer);
        return varVal;
    }

    /// <summary>
    /// Sets a environment variable with the specified name to the provided string value.
    /// </summary>
    /// <remarks>
    /// The variable is set globally and may affect other processes or components that read environment variables.
    /// The value is stored as a Unicode string. Ensure that the variable name and value conform to any platform-specific restrictions.
    /// </remarks>
    /// <param name="varName">The name of the environment variable to set. Cannot be null or empty.</param>
    /// <param name="Value">The string value to assign to the environment variable. Cannot be null.</param>
    /// <param name="attributes">The attributes to store with the variable (for example, non-volatile, runtime access).</param>
    public void WriteStringVariable(string varName, string Value, VariableAttributes attributes)
    {
        IntPtr pointer = Marshal.StringToHGlobalUni(Value);
        Backend.WriteEnvironmentVariable(varName, VendorGuid, attributes, pointer, Value.Length * 2);
        Marshal.FreeHGlobal(pointer);
    }

    /// <summary>
    /// Retrieves the value of a environment variable and returns it as a value type.
    /// </summary>
    /// <remarks>
    /// This method reads the variable from the environment and marshals it to the specified value type.
    /// The caller must ensure that the variable exists and is compatible with the requested type.
    /// If the variable does not exist or cannot be marshaled to type T, the result may be undefined.
    /// </remarks>
    /// <typeparam name="T">The value type to which the environment variable will be marshaled. Must be a struct.</typeparam>
    /// <param name="varName">The name of the environment variable to read. Cannot be null or empty.</param>
    /// <returns>The value of the specified environment variable, marshaled to type T.</returns>
    /// <param name="attributes">When this method returns, contains the attributes of the variable.</param>
    public T ReadVariable<T>(string varName, out VariableAttributes attributes) where T : struct
    {
        // Getting variable data
        int ptrSize = Marshal.SizeOf<T>();
        IntPtr pointer = Backend.ReadEnvironmentVariable(varName, VendorGuid, out attributes, ptrSize, out _);

        T varVal = Marshal.PtrToStructure<T>(pointer);
        Marshal.FreeHGlobal(pointer);
        return varVal;
    }

    /// <summary>
    /// Writes a value of a specified struct type to a environment variable identified by name.
    /// </summary>
    /// <remarks>
    /// This method stores the value as a binary representation using unmanaged memory.
    /// The caller should ensure that the variable name is unique and valid within the environment.
    /// Only value types (structs) are supported.</remarks>
    /// <typeparam name="T">The struct type of the value to write to the environment variable.</typeparam>
    /// <param name="varName">The name of the environment variable to set. Cannot be null or empty.</param>
    /// <param name="Value">The value to assign to the environment variable.</param>
    /// <param name="attributes">The attributes to store with the variable (for example, non-volatile, runtime access).</param>
    public void WriteVariable<T>(string varName, T Value, VariableAttributes attributes) where T : struct
    {
        // Getting variable data
        int ptrSize = Marshal.SizeOf<T>();
        IntPtr pointer = Marshal.AllocHGlobal(ptrSize);

        // Writing data
        Marshal.StructureToPtr(Value, pointer, false);
        Backend.WriteEnvironmentVariable(varName, VendorGuid, attributes, pointer, ptrSize);

        // Freeing
        Marshal.FreeHGlobal(pointer);
    }

    /// <summary>
    /// Retrieves the value of a environment variable as an array of value type elements.
    /// </summary>
    /// <remarks>
    /// The method reads the raw memory of the environment variable and interprets it as an array of the specified value type.
    /// Use this method only when the variable is known to contain binary data compatible with the type T.
    /// The caller is responsible for ensuring the variable's format matches the expected type.
    /// </remarks>
    /// <typeparam name="T">The value type of the elements to read from the environment variable.</typeparam>
    /// <param name="varName">The name of the environment variable to retrieve. Cannot be null or empty.</param>
    /// <param name="attributes">When this method returns, contains the attributes of the variable.</param>
    /// <returns>An array of elements of type T containing the values stored in the specified environment variable. The array
    /// will be empty if the variable contains no data.</returns>
    public T[] ReadArrayVariable<T>(string varName, out VariableAttributes attributes) where T : struct
    {
        // Getting variable data
        int ptrSize = Marshal.SizeOf<T>();
        IntPtr dataBuffer = Backend.ReadEnvironmentVariable(varName, VendorGuid, out attributes, ptrSize * 256, out uint dataSize);

        // Parsing values
        int length = (int)dataSize / ptrSize;
        T[] varVal = PinnedMemoryToArray<T>(dataBuffer, length);

        // Freeing
        Marshal.FreeHGlobal(dataBuffer);
        return varVal;
    }

    /// <summary>
    /// Writes a environment variable as an array of value type elements.
    /// </summary>
    /// <remarks>
    /// This method pins the array in memory and writes its contents to the specified environment variable.
    /// The variable is set as a contiguous block of memory representing the array.
    /// Use caution when passing large arrays, as this operation involves unmanaged memory handling.
    /// </remarks>
    /// <typeparam name="T">The value type of the elements in the array to be written.</typeparam>
    /// <param name="varName">The name of the environment variable to set. Cannot be null or empty.</param>
    /// <param name="Value">The array of value type elements to write. Cannot be null and must contain at least one element.</param>
    /// <param name="attributes">The attributes to store with the variable (for example, non-volatile, runtime access).</param>
    public void WriteArrayVariable<T>(string varName, T[] Value, VariableAttributes attributes) where T : struct
    {
        // Formating new value
        int ptrSize = Marshal.SizeOf<T>();
        IntPtr handle = Marshal.UnsafeAddrOfPinnedArrayElement(Value, 0);

        // Setting variable
        Backend.WriteEnvironmentVariable(varName, VendorGuid, attributes, handle, Value.Length * ptrSize);
    }

    /// <summary>
    /// Converts a pointer to pinned memory into a managed array of type T.
    /// </summary>
    /// <typeparam name="T">The type of the struct.</typeparam>
    /// <param name="memory">The pointer to the memory.</param>
    /// <param name="length">The number of elements to read.</param>
    /// <returns>An array containing the data from the memory.</returns>
    private static unsafe T[] PinnedMemoryToArray<T>(IntPtr memory, int length) where T : struct
    {
        ReadOnlySpan<T> span = new ReadOnlySpan<T>(memory.ToPointer(), length);
        return span.ToArray();
    }
}

/// <summary>
/// Represents an error that occurred while working with UEFI
/// </summary>
public class FirmwareEnvironmentException(string message, Exception? innerException = null) : Exception(message, innerException)
{
    /// <inheritdoc/>
    public FirmwareEnvironmentException()
        : this(string.Empty, null) { }

    /// <inheritdoc/>
    public FirmwareEnvironmentException(string message)
        : this(message, null) { }
}
