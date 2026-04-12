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

public interface IFirmwareBackend
{
    /// <summary>
    /// Checks whether the UEFI platform is available on this system
    /// </summary>
    /// <returns>If available, return <see langword="true"/>, else <see langword="false"/></returns>
    bool CheckFirmwareAvailablity();

    /// <summary>
    /// Seraches fro GUID identifier of the system EFI partition.
    /// This is a unique identifier assigned to the partition when it is created,
    /// and can be used to identify the partition among other partitions on the same disk.
    /// It is not related to the partition type, and is not a constant value.
    /// It can be used to access the partition's files and directories or passed to Win32 API functions that require a volume path.
    /// </summary>
    VolumePath FindEfiSystemPartition();

    /// <summary>
    /// Gets the value of an environment variable with the specified name and attributes from the specified environment.
    /// </summary>
    /// <param name="varName"></param>
    /// <param name="environmentIdentificator"></param>
    /// <param name="attributes"></param>
    /// <param name="bufferSize"></param>
    /// <param name="dataSize"></param>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException"></exception>
    /// <exception cref="FirmwareEnvironmentException"></exception>
    IntPtr ReadEnvironmentVariable(string varName, Guid environmentIdentificator, out VariableAttributes attributes, int bufferSize, out uint dataSize);

    /// <summary>
    /// Sets the value of an environment variable with the specified name and attributes in the specified environment.
    /// The variable is stored in non-volatile memory and will persist across system reboots.
    /// The variable is associated with the specified environment, which is identified by a GUID.
    /// The attributes parameter specifies the attributes of the variable, such as whether it is read-only or can be accessed from user mode.
    /// The value parameter is a pointer to a buffer that contains the data to be stored in the variable, and ptrSize specifies the size of the buffer in bytes.
    /// </summary>
    /// <param name="varName"></param>
    /// <param name="environmentIdentificator"></param>
    /// <param name="attributes"></param>
    /// <param name="valueBuffer"></param>
    /// <param name="bufferSize"></param>
    /// <exception cref="PlatformNotSupportedException"></exception>
    /// <exception cref="FirmwareEnvironmentException"></exception>
    void WriteEnvironmentVariable(string varName, Guid environmentIdentificator, VariableAttributes attributes, IntPtr valueBuffer, int bufferSize);
}
