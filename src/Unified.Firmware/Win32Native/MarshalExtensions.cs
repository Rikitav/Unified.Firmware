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
using System.Text;

namespace Unified.Firmware.Win32Native;

/// <summary>
/// Provides extension methods for marshalling data.
/// </summary>
public static partial class MarshalExtensions
{
    /// <summary>
    /// Converts a pointer to pinned memory into a managed array of type T.
    /// </summary>
    /// <typeparam name="T">The type of the struct.</typeparam>
    /// <param name="memory">The pointer to the memory.</param>
    /// <param name="length">The number of elements to read.</param>
    /// <returns>An array containing the data from the memory.</returns>
    public static unsafe T[] PinnedMemoryToArray<T>(this IntPtr memory, int length) where T : struct
    {
        ReadOnlySpan<T> span = new ReadOnlySpan<T>(memory.ToPointer(), length);
        return span.ToArray();
    }

    /// <summary>
    /// Writes a C-style null-terminated wide string to the binary writer.
    /// </summary>
    /// <param name="writer">The binary writer.</param>
    /// <param name="value">The string to write.</param>
    /// <returns>The binary writer.</returns>
    public static BinaryWriter WriteCstyleWideString(this BinaryWriter writer, string value)
    {
        if (value == null)
            return writer;

        byte[] bytes = Encoding.Unicode.GetBytes(value);
        byte[] terminator = Encoding.Unicode.GetBytes("\0");

        writer.Write(bytes);
        writer.Write(terminator);

        return writer;
    }

    /// <summary>
    /// Reads a C-style null-terminated wide string from the binary reader.
    /// </summary>
    /// <param name="reader">The binary reader.</param>
    /// <returns>The read string.</returns>
    public static string ReadCstyleWideString(this BinaryReader reader)
    {
        StringBuilder builder = new StringBuilder();
        for (ushort chr = reader.ReadUInt16(); chr != 0; chr = reader.ReadUInt16())
            builder.Append((char)chr);

        return builder.ToString();
    }

    /// <summary>
    /// Calculates the byte length of a C-style wide string, including the null terminator.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>The length in bytes.</returns>
    public static ushort GetCstyleWideStringLength(this string value)
    {
        if (value == null)
            return 0;
        
        return (ushort)((value.Length + 1) * sizeof(ushort));
    }

    /// <summary>
    /// Reads all remaining bytes from the current stream position.
    /// </summary>
    /// <param name="reader">The binary reader.</param>
    /// <returns>The remaining bytes.</returns>
    public static byte[] ReadRemainingBytes(this BinaryReader reader)
    {
        int remainingBytesLength = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return reader.ReadBytes(remainingBytesLength);
    }

    /// <summary>
    /// Writes a GUID to the binary writer.
    /// </summary>
    /// <param name="writer">The binary writer.</param>
    /// <param name="guid">The GUID to write.</param>
    /// <returns>The binary writer.</returns>
    public static BinaryWriter WriteGuid(this BinaryWriter writer, Guid guid)
    {
        writer.Write(guid.ToByteArray());
        return writer;
    }

    /// <summary>
    /// Reads a GUID from the binary reader.
    /// </summary>
    /// <param name="reader">The binary reader.</param>
    /// <returns>The read GUID.</returns>
    public static Guid ReadGuid(this BinaryReader reader)
    {
        byte[] guidBytes = reader.ReadBytes(16);
        return new Guid(guidBytes);
    }
}
