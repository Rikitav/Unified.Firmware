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

namespace Unified.Firmware.BootService.LoadOption;

/// <summary>
/// The attributes for this load option entry
/// </summary>
[Flags]
public enum LoadOptionAttributes : uint
{
    /// <summary>
    /// If a load option is marked as LOAD_OPTION_ACTIVE, the boot manager will attempt to boot automatically using the device path information in the load option
    /// </summary>
    ACTIVE = 0x00000001,

    /// <summary>
    /// If any Driver#### load option is marked as LOAD_OPTION_FORCE_RECONNECT , then all of the UEFI drivers in the system will be disconnected and reconnected after the last Driver#### load option is processed
    /// </summary>
    FORCE_RECONNECT = 0x00000002,

    /// <summary>
    /// If any Boot#### load option is marked as LOAD_OPTION_HIDDEN , then the load option will not appear in the menu (if any) provided by the boot manager for load option selection
    /// </summary>
    HIDDEN = 0x00000008,

    /// <summary>
    /// The LOAD_OPTION_CATEGORY is a sub-field of Attributes that provides details to the boot manager to describe how it should group the Boot#### load options
    /// </summary>
    CATEGORY = 0x00001F00,

    /// <summary>
    /// Boot#### load options with LOAD_OPTION_CATEGORY set to LOAD_OPTION_CATEGORY_BOOT are meant to be part of the normal boot processing
    /// </summary>
    CATEGORY_BOOT = 0x00000000,

    /// <summary>
    /// Boot#### load options with LOAD_OPTION_CATEGORY set to LOAD_OPTION_CATEGORY_APP are executables which are not part of the normal boot processing but can be optionally chosen for execution if boot menu is provided, or via Hot Keys
    /// </summary>
    CATEGORY_APP = 0x00000100
}
