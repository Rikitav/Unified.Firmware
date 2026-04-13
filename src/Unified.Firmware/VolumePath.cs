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

namespace Unified.Firmware;

/// <summary>
/// Represents a strongly-typed volume path based on a unique volume identifier (GUID). Provides comparison, equality,
/// and conversion operations for working with volume paths in a type-safe manner.
/// </summary>
/// 
/// <remarks>
/// <para>
/// FullPath format is depends on platform you using this record.
/// For Windows it stores `\\?\Volume{GUID}\` path format, which coressponds to WinAPI standart and can be used to open handles to this volume.
/// For Linux it stores absolute virtual path `/dev/{diskName}/boot` to directory, where ESP volume is mounted.
/// </para>
/// 
/// <para>
/// Use this struct to encapsulate and manipulate volume paths identified by GUIDs,
/// enabling type-safe operations and conversions to and from related types such as Guid, string, and DirectoryInfo.
/// Instances are immutable and can be compared or used in collections that require equality or ordering.
/// Implicit conversions simplify interoperability with APIs that accept Guid or string representations of volume paths.
/// </para>
/// </remarks>
public readonly record struct VolumePath
{
    /// <summary>
    /// Gets the unique identifier associated with this instance.
    /// </summary>
    public readonly Guid Identificator { get; }

    /// <summary>
    /// Gets the absolute path to root of volume.
    /// </summary>
    public readonly string FullPath { get; }

    /// <summary>
    /// Initializes a new instance of the VolumePath class using the specified string representation of a GUID.
    /// </summary>
    /// <param name="volumeIdentificator">The unique identifier (GUID) that specifies the volume path associated with this instance.</param>
    /// <param name="fullPath">Platform specific, full path to volume root</param>
    public VolumePath(Guid volumeIdentificator, string fullPath)
    {
        Identificator = volumeIdentificator;
        FullPath = fullPath;
    }

    /*
    /// <inheritdoc/>
    public int CompareTo(VolumePath other) => _dentificator.CompareTo(other._dentificator);

    /// <inheritdoc/>
    public int CompareTo(Guid other) => _dentificator.CompareTo(other);

    /// <inheritdoc/>
    public bool Equals(VolumePath other) => _dentificator.Equals(other._dentificator);

    /// <inheritdoc/>
    public bool Equals(Guid other) => _dentificator.Equals(other);
    */

    /// <summary>
    /// Returns the string representation of the volume in the Windows device path format.
    /// </summary>
    /// <remarks>
    /// This format is commonly used for referencing volumes in Windows APIs and system utilities.
    /// The returned string can be used for operations that require the full device path of a volume.
    /// </remarks>
    /// <returns>
    /// A string containing the volume path in the format "\\?\Volume{GUID}\",
    /// where GUID is the value of the <c>Identificator</c> property.
    /// </returns>
    public override string ToString() => FullPath; //string.Concat(@"\\?\Volume{", Identificator.ToString(), @"}\");

    /// <summary>
    /// Converts a <see cref="VolumePath"/> instance to its associated <see cref="Guid"/> identifier.
    /// </summary>
    /// <param name="path">The <see cref="VolumePath"/> object to convert to a <see cref="Guid"/>.</param>
    public static implicit operator Guid(VolumePath path) => path.Identificator;

    /// <summary>
    /// Converts a <see cref="VolumePath"/> instance to its string representation.
    /// </summary>
    /// <param name="path">The <see cref="VolumePath"/> instance to convert.</param>
    public static implicit operator string(VolumePath path) => path.FullPath;

    /*
    /// <summary>
    /// Defines an implicit conversion from a <see cref="System.Guid"/> to a <c>VolumePath</c> instance.
    /// </summary>
    /// <param name="guid">The <see cref="System.Guid"/> value representing the unique identifier of the volume path to convert.</param>
    public static implicit operator VolumePath(Guid guid) => new VolumePath(guid);

    /// <summary>
    /// Converts a string containing a volume GUID path to a <see cref="VolumePath"/> instance.
    /// </summary>
    /// <param name="guidStr">A string representing the volume GUID path to convert. Must not be null or empty.</param>
    public static implicit operator VolumePath(string guidStr) => new VolumePath(guidStr);
    */

    /// <summary>
    /// Converts a <see cref="VolumePath"/> instance to a <see cref="DirectoryInfo"/> representing the same file system path.
    /// </summary>
    /// <param name="path">The <see cref="VolumePath"/> to convert to a <see cref="DirectoryInfo"/>.</param>
    public static implicit operator DirectoryInfo(VolumePath path) => new DirectoryInfo(path.FullPath);
}
