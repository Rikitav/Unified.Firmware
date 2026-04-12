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
using System.Runtime.InteropServices;
using Unified.Firmware.PlatformBackend.LinuxPlatform;

namespace Unified.Firmware.PlatformBackend;

internal class LinuxPlatfromBackend : IFirmwareBackend
{
    private const string EfiVarsDirectory = "/sys/firmware/efi/efivars";

    /// <inheritdoc/>
    public bool CheckFirmwareAvailablity()
    {
        // Если директория существует, система загружена в UEFI-режиме
        return Directory.Exists(EfiVarsDirectory);
    }

    /// <inheritdoc/>
    public VolumePath FindEfiSystemPartition()
    {
        foreach (GptPartitionModel espEntry in IoctlPartitionReader.FindAllEfiPartitionsInSystem())
            return new VolumePath(espEntry.Entry.UniquePartitionGuid, espEntry.VolumePath);

        throw new DriveNotFoundException("Efi partition was not found");
    }

    /// <inheritdoc/>
    public IntPtr ReadEnvironmentVariable(string varName, Guid environmentIdentificator, out VariableAttributes attributes, int bufferSize, out uint dataSize)
    {
        if (!CheckFirmwareAvailablity())
            throw new PlatformNotSupportedException("This system does not support UEFI, or is loaded in LEGACY mode");

        string filePath = GetVariableFilePath(varName, environmentIdentificator);
        IntPtr varDataBuffer = IntPtr.Zero;

        try
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"UEFI variable {varName} not found.");

            byte[] fileData = File.ReadAllBytes(filePath);
            if (fileData.Length < 4)
                throw new InvalidDataException("EFI variable file is corrupted (missing attributes header).");

            attributes = (VariableAttributes)BitConverter.ToUInt32(fileData, 0);
            dataSize = (uint)(fileData.Length - 4);

            if (dataSize > bufferSize)
                throw new InvalidOperationException($"Buffer size ({bufferSize}) is insufficient for variable data ({dataSize}).");

            varDataBuffer = Marshal.AllocHGlobal(bufferSize);
            if (dataSize > 0)
                Marshal.Copy(fileData, 4, varDataBuffer, (int)dataSize);

            return varDataBuffer;
        }
        catch (Exception ex)
        {
            if (varDataBuffer != IntPtr.Zero)
                Marshal.FreeHGlobal(varDataBuffer);

            throw new FirmwareEnvironmentException("Failed to read environment variable", ex);
        }
    }

    /// <inheritdoc/>
    public void WriteEnvironmentVariable(string varName, Guid environmentIdentificator, VariableAttributes attributes, IntPtr valueBuffer, int bufferSize)
    {
        if (!CheckFirmwareAvailablity())
            throw new PlatformNotSupportedException("This system does not support UEFI, or is loaded in LEGACY mode");

        string filePath = GetVariableFilePath(varName, environmentIdentificator);

        try
        {
            byte[] payload = new byte[4 + bufferSize];
            BitConverter.GetBytes((uint)attributes).CopyTo(payload, 0);

            if (bufferSize > 0 && valueBuffer != IntPtr.Zero)
                Marshal.Copy(valueBuffer, payload, 4, bufferSize);

            if (File.Exists(filePath))
                VariableAttributesHelper.EnsureMutable(filePath);

            File.WriteAllBytes(filePath, payload);
            return;
        }
        catch (Exception ex)
        {
            throw new FirmwareEnvironmentException("Failed to write environment variable", ex);
        }
    }

    private static string GetVariableFilePath(string varName, Guid guid)
    {
        // should always be in lower case
        return Path.Combine(EfiVarsDirectory, $"{varName}-{guid.ToString("D").ToLowerInvariant()}");
    }
}
