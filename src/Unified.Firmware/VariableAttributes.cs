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
/// Attributes bitmask to set for the variable
/// </summary>
[Flags]
public enum VariableAttributes : uint
{
    /// <summary>
    /// No attributes
    /// </summary>
    None = 0,

    /// <summary>
    /// The variable is accessible from a non volatile environment
    /// </summary>
    NON_VOLATILE = 0x00000001,

    /// <summary>
    /// The variable is available while the Boot service is running
    /// </summary>
    BOOTSERVICE_ACCESS = 0x00000002,

    /// <summary>
    /// The variable is available at runtime
    /// </summary>
    RUNTIME_ACCESS = 0x00000004,

    /// <summary>
    /// NoDescription
    /// </summary>
    HARDWARE_ERROR_RECORD = 0x00000008,

    /// <summary>
    /// The variable is available only to authorized sources
    /// </summary>
    AUTHENTICATED_WRITE_ACCESS = 0x00000010,

    /// <summary>
    /// NoDescription
    /// </summary>
    TIME_BASED_AUTHENTICATED_WRITE_ACCESS = 0x00000020,

    /// <summary>
    /// NoDescription
    /// </summary>
    APPEND_WRITE = 0x00000040,

    /// <summary>
    /// NoDescription
    /// </summary>
    ENHANCED_AUTHENTICATED_ACCESS = 0x00000080
}
