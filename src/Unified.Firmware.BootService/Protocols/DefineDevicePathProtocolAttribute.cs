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

namespace Unified.Firmware.BootService.Protocols;

/// <summary>
/// Attribute describing the wrapper class for the DevicePath protocol
/// </summary>
/// <param name="type"></param>
/// <param name="subType"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class DefineDevicePathProtocolAttribute(DeviceProtocolType type, byte subType) : Attribute
{
    /// <summary>
    /// Type of protocol to be wrapped
    /// </summary>
    public DeviceProtocolType Type { get; } = type;

    /// <summary>
    /// SubType of protocol to be wrapped
    /// </summary>
    public byte SubType { get; } = subType;
}
