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

namespace Unified.Firmware;

/// <summary>
/// Represents a boot option index, which is a 16-bit unsigned integer index used to identify a specific boot option in the system's firmware.
/// The structure provides an implicit conversion to and from ushort, as well as a string representation in the format "BootXXXX", where XXXX is the hexadecimal representation of the index index.
/// </summary>
/// <param name="index"></param>
public readonly struct BootOptionIndex(ushort index) : IEquatable<BootOptionIndex>, IEquatable<ushort>, IComparable<BootOptionIndex>, IComparable<ushort>
{
    private readonly ushort _index = index;

    /// <inheritdoc/>
    public int CompareTo(BootOptionIndex other) => _index.CompareTo(other._index);

    /// <inheritdoc/>
    public int CompareTo(ushort other) => _index.CompareTo(other);

    /// <inheritdoc/>
    public bool Equals(BootOptionIndex other) => _index.Equals(other._index);

    /// <inheritdoc/>
    public bool Equals(ushort other) => _index.Equals(other);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is BootOptionIndex other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => _index.GetHashCode();

    /// <summary>
    /// Determines whether two BootOptionIndex instances are equal.
    /// </summary>
    /// <param name="left">The first BootOptionIndex to compare.</param>
    /// <param name="right">The second BootOptionIndex to compare.</param>
    /// <returns>true if the specified BootOptionIndex instances are equal; otherwise, false.</returns>
    public static bool operator ==(BootOptionIndex left, BootOptionIndex right) => left.Equals(right);

    /// <summary>
    /// Determines whether two BootOptionIndex instances are not equal.
    /// </summary>
    /// <param name="left">The first BootOptionIndex to compare.</param>
    /// <param name="right">The second BootOptionIndex to compare.</param>
    /// <returns>true if the specified BootOptionIndex instances are not equal; otherwise, false.</returns>
    public static bool operator !=(BootOptionIndex left, BootOptionIndex right) => !left.Equals(right);

    /// <summary>
    /// Returns a string that represents boot option index in format "BootXXXX", where XXXX is the hexadecimal representation of the index index.
    /// </summary>
    public override readonly string ToString() => $"Boot{_index:X4}";

    /// <summary>
    /// Impictly converts <see cref="BootOptionIndex"/> structure to <see langword="ushort"/>
    /// </summary>
    /// <param name="index"></param>
    public static implicit operator ushort(BootOptionIndex index) => index._index;

    /// <summary>
    /// Implicitly converts <see langword="ushort"/> index to <see cref="BootOptionIndex"/> structure.
    /// </summary>
    /// <param name="index"></param>
    public static implicit operator BootOptionIndex(ushort index) => new BootOptionIndex(index);

    /// <summary>
    /// Converts a BootOptionIndex instance to its string representation.
    /// </summary>
    /// <param name="index"></param>
    public static implicit operator string(BootOptionIndex index) => index.ToString();
}
