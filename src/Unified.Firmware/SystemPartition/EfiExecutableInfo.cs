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

namespace Unified.Firmware.SystemPartition;

/// <summary>
/// Provides properties of Efi application files, such as architecture, application provider, and file information
/// </summary>
public class EfiExecutableInfo
{
    // private fields
    private readonly string _FilePath;
    private FileInfo? _FileInfo;

    /// <summary>
    /// Full path to the executable file, including the partition path
    /// </summary>
    public string FullName
    {
        get => _FilePath;
    }

    /// <summary>
    /// The name of the folder in which the EFI executable is stored is the name of the provider of the executable file
    /// </summary>
    public string Application
    {
        get => Path.GetFileNameWithoutExtension(Path.GetDirectoryName(FullName));
    }

    /// <summary>
    /// Executable's file information
    /// </summary>
    public FileInfo FileInfo
    {
        get => _FileInfo ??= new FileInfo(_FilePath);
    }

    /// <summary>
    /// Executable architecture based on <see href="https://uefi.org/specs/UEFI/2.9_A/03_Boot_Manager.html#removable-media-boot-behavior">Boot manager docs</see>
    /// </summary>
    public FirmwareApplicationArchitecture Architecture
    {
        get
        {
            // Getting arch bit
            byte[] data = File.ReadAllBytes(FullName);
            ushort archVal = BitConverter.ToUInt16(data, BitConverter.ToInt32(data, 0x3c) + 4);
            
            // Checking if value is defined in enum
            if (!Enum.IsDefined(typeof(FirmwareApplicationArchitecture), (FirmwareApplicationArchitecture)archVal))
                return FirmwareApplicationArchitecture.Unknown;

            // Result
            return (FirmwareApplicationArchitecture)archVal;
        }
    }

    /// <summary>
    /// Creates a new instance of <see cref="EfiExecutableInfo"/> from info of efi application file
    /// </summary>
    /// <param name="EfiExecutableFile"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public EfiExecutableInfo(FileInfo EfiExecutableFile)
    {
        if (EfiExecutableFile == null)
            throw new ArgumentNullException(nameof(EfiExecutableFile));

        _FileInfo = EfiExecutableFile;
        _FilePath = EfiExecutableFile.FullName;
    }

    /// <summary>
    /// Creates a new instance of <see cref="EfiExecutableInfo"/> from the full path to the efi application file
    /// </summary>
    /// <param name="FullPath"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public EfiExecutableInfo(string FullPath)
    {
        if (string.IsNullOrEmpty(FullPath))
            throw new ArgumentNullException(nameof(FullPath));

        string? ext = Path.GetExtension(FullPath);
        if (string.IsNullOrEmpty(ext))
            FullPath += FullPath.EndsWith(".") ? "efi" : ".efi";

        if (!ext.Equals(".efi", StringComparison.CurrentCultureIgnoreCase))
            throw new ArgumentException(string.Format("Applications provided in the path have an extension other than \".EFI\" ({0})", ext));

        _FilePath = FullPath;
    }

    /// <summary>
    /// Creates a new instance of <see cref="EfiExecutableInfo"/> located on the ESP from the name of the application provider and its architecture with the default application name "boot"
    /// </summary>
    /// <param name="architecture"></param>
    /// <exception cref="ArgumentException"></exception>
    public EfiExecutableInfo(FirmwareApplicationArchitecture architecture) : this(Path.Combine(EfiPartition.VolumePath, "EFI", "Boot", string.Format("boot{0}.efi", architecture)))
    {
        if (!Enum.IsDefined(typeof(FirmwareApplicationArchitecture), architecture))
            throw new ArgumentException("The provided architecture was not recognized", nameof(architecture));
    }

    /// <summary>
    /// Creates a new instance of <see cref="EfiExecutableInfo"/> located on the ESP from the name of the application provider and application file name
    /// </summary>
    /// <param name="ApplicationName"></param>
    /// <param name="ApplicationFile"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public EfiExecutableInfo(string ApplicationName, string ApplicationFile) : this(Path.Combine(EfiPartition.VolumePath, "EFI", ApplicationName, ApplicationFile))
    {
        if (string.IsNullOrEmpty(ApplicationName))
            throw new ArgumentNullException(nameof(ApplicationName));

        if (string.IsNullOrEmpty(ApplicationFile))
            throw new ArgumentNullException(nameof(ApplicationFile));
    }

    /// <summary>
    /// Creates a new instance of <see cref="EfiExecutableInfo"/> located on external drive from the and its architecture with the default application name "boot"
    /// </summary>
    /// <param name="ApplicationRoot"></param>
    /// <param name="architecture"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public EfiExecutableInfo(DriveInfo ApplicationRoot, FirmwareApplicationArchitecture architecture) : this(Path.Combine(ApplicationRoot.Name, "EFI", "Boot", string.Format("boot{0}.efi", architecture)))
    {
        if (ApplicationRoot == null)
            throw new ArgumentNullException(nameof(ApplicationRoot));

        if (!Enum.IsDefined(typeof(FirmwareApplicationArchitecture), architecture))
            throw new ArgumentException(nameof(architecture));
    }

    /// <summary>
    /// Creates a new instance of <see cref="EfiExecutableInfo"/> located on external drive from the name of the application provider and application file name
    /// </summary>
    /// <param name="ApplicationRoot"></param>
    /// <param name="ApplicationName"></param>
    /// <param name="ApplicationFile"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DriveNotFoundException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public EfiExecutableInfo(DriveInfo ApplicationRoot, string ApplicationName, string ApplicationFile) : this(Path.Combine(ApplicationRoot.Name, "EFI", ApplicationName, ApplicationFile))
    {
        if (ApplicationRoot == null)
            throw new ArgumentNullException(nameof(ApplicationRoot));

        if (!ApplicationRoot.IsReady)
            throw new DriveNotFoundException(string.Format("{0} drive is not ready", ApplicationRoot.Name));

        if (string.IsNullOrEmpty(ApplicationName))
            throw new ArgumentNullException(nameof(ApplicationName));

        if (string.IsNullOrEmpty(ApplicationFile))
            throw new ArgumentException(nameof(ApplicationFile));
    }
}
