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

using Unified.Firmware.BootService.Protocols;

namespace Unified.Firmware.BootService.LoadOption;

/// <summary>
/// <see href="https://uefi.org/specs/UEFI/2.9_A/03_Boot_Manager.html#load-options">Load option</see> used to determine the boot parameters of a specific object, be it a driver or an OS
/// </summary>
public abstract class LoadOptionBase()
{
    /// <summary>
    /// The attributes for this load option entry
    /// </summary>
    public LoadOptionAttributes Attributes { get; set; } = LoadOptionAttributes.CATEGORY_BOOT;

    /// <summary>
    /// The user readable description for the load option
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// <para>FilePathList</para>
    /// A packed array of UEFI device paths.The first element of the array is a device path that describes the device and location of the Image for this load option.The FilePathList[0] is specific to the device type.Other device paths may optionally exist in the FilePathList, but their usage is OSV specific. Each element in the array is variable length, and ends at the device path end structure. Because the size of Description is arbitrary, this data structure is not guaranteed to be aligned on a natural boundary.This data structure may have to be copied to an aligned natural boundary before it is used
    /// </summary>
    public DevicePathProtocolBase[] Protocols { get; set; } = [];

    /// <summary>
    /// The remaining bytes in the load option descriptor are a binary data buffer that is passed to the loaded image. If the field is zero bytes long, a NULL pointer is passed to the loaded image. The number of bytes in OptionalData can be computed by subtracting the starting offset of OptionalData from total size in bytes of the EFI_LOAD_OPTION
    /// </summary>
    public byte[] OptionalData { get; set; } = [];

    /*
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadOptionBase"/> class with default boot category attributes and an empty name.
    /// </summary>
    protected LoadOptionBase()
        : this(LoadOptionAttributes.CATEGORY_BOOT, string.Empty) { }
    */

    /*
    /// <summary>
    /// Create new <see cref="LoadOptionBase"/> load option instance from raw structure
    /// </summary>
    /// <param name="loadOption"></param>
    protected LoadOptionBase(EFI_LOAD_OPTION loadOption)
    {
        Attributes = loadOption.Attributes;
        Description = loadOption.Description;
        OptionalData = loadOption.OptionalData;

        Protocols = new DevicePathProtocolBase[loadOption.FilePathList.Length];
        for (int i = 0; i < loadOption.FilePathList.Length; i++)
        {
            if (loadOption.FilePathList[i].Type == DeviceProtocolType.End && loadOption.FilePathList[i].SubType == 0xFF)
                break; // End protocol

            Type? protocolWrapperType = DevicePathProtocolWrapperSelector.GetRegisteredType(loadOption.FilePathList[i].Type, loadOption.FilePathList[i].SubType);
            Protocols[i] = protocolWrapperType == null
                ? new RawMediaDevicePath(loadOption.FilePathList[i].Type, loadOption.FilePathList[i].SubType)
                : DevicePathProtocolBase.CreateProtocol(protocolWrapperType, loadOption.FilePathList[i]);
        }
    }
    */
}
