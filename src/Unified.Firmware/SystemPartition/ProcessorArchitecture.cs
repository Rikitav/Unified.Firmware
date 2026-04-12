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

namespace Unified.Firmware.SystemPartition;
#pragma warning disable RCS1154 // Sort enum members

/// <summary>
/// Enumeration of supported processor architectures for firmware applications
/// </summary>
public enum FirmwareApplicationArchitecture : ushort
{
    /// <summary>
    /// Unknown architecture
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// 32-bit
    /// </summary>
    Ia32 = 0x14c,

    /// <summary>
    /// AMD64
    /// </summary>
    x64 = 0x8664,

    /// <summary>
    /// Intel Itanium 64
    /// </summary>
    Ia64 = 0x200,

    /// <summary>
    /// AArch32 architecture
    /// </summary>
    Arm = 0x1c2,

    /// <summary>
    /// AArch64 architecture
    /// </summary>
    AArch64 = 0xaa64,

    /// <summary>
    /// RISC-V 32-bit architecture
    /// </summary>
    RISC_V32 = 0x5032,

    /// <summary>
    /// RISC-V 64-bit architecture
    /// </summary>
    RISC_V64 = 0x5064,

    /// <summary>
    /// RISC-V 128-bit architecture
    /// </summary>
    RISC_V128 = 0x5128
}
