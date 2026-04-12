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
/// Basic implementation of <see cref="LoadOptionBase"/>
/// </summary>
public class FirmwareBootOption : LoadOptionBase
{
    /// <summary>
    /// Initializes a new instance of the FirmwareBootOption class with default attribute and description values.
    /// </summary>
    /// <remarks>
    /// The Attributes property is set to LoadOptionAttributes.CATEGORY_BOOT, and the Description property is initialized as an empty string.
    /// This constructor is typically used when creating a boot option with default settings before customizing its properties.
    /// </remarks>
    public FirmwareBootOption()
    {
        Attributes = LoadOptionAttributes.CATEGORY_BOOT;
        Description = string.Empty;
    }

    /// <summary>
    /// Create new <see cref="FirmwareBootOption"/> load option instance from <see cref="LoadOptionAttributes"/> and Option description
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="description"></param>
    public FirmwareBootOption(LoadOptionAttributes attributes, string description)
    {
        Attributes = attributes;
        Description = description;
    }

    /// <summary>
    /// Create new <see cref="FirmwareBootOption"/> load option instance from <see cref="LoadOptionAttributes"/>, Option description, <see cref="LoadOptionBase.OptionalData"/> and <see cref="DevicePathProtocolBase"/>s
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="description"></param>
    /// <param name="optionalData"></param>
    /// <param name="protocols"></param>
    public FirmwareBootOption(LoadOptionAttributes attributes, string description, DevicePathProtocolBase[] protocols, byte[] optionalData) : this(attributes, description)
    {
        Protocols = protocols;
        OptionalData = optionalData;
    }
}
