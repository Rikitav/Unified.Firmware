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
using System.ComponentModel;

namespace Unified.Firmware.BootService
{
    /// <summary>
    /// Invalid descriptor value
    /// </summary>
    public class InvalidHandleValueException : Win32Exception
    {
        /// <inheritdoc/>
        public InvalidHandleValueException(int lastError)
            : base(lastError) { }
    }

    /// <summary>
    /// Failed to find free loadOption index
    /// </summary>
    public class FreeLoadOptionIndexNotFound : Exception
    {
        /// <inheritdoc/>
        public FreeLoadOptionIndexNotFound(string Message)
            : base(Message) { }
    }

    /// <summary>
    /// The registering type has the wrong parent
    /// </summary>
    public class InvalidInheritedClassException : Exception
    {
        /// <inheritdoc/>
        public InvalidInheritedClassException(string message)
            : base(message) { }
    }

    /// <summary>
    /// Failed to cast DevicePathProtocol
    /// </summary>
    public class DeviceProtocolCastingException : Exception
    {
        /// <inheritdoc/>
        public DeviceProtocolCastingException(string message)
            : base(message) { }
    }

    /// <summary>
    /// Load option has incorrect structure
    /// </summary>
    public class InvalidLoadOptionStrcutreException : Exception
    {
        /// <inheritdoc/>
        public InvalidLoadOptionStrcutreException(string message)
            : base(message) { }
    }

    /// <summary>
    /// The wrapper class does not contain a defining attribute
    /// </summary>
    public class MissingDevicePathProtocolWrapperAttributeException : Exception
    {
        /// <inheritdoc/>
        public MissingDevicePathProtocolWrapperAttributeException(string message)
            : base(message) { }
    }

    /// <summary>
    /// The wrapper class has the wrong structure
    /// </summary>
    public class InvalidDevicePathProtocolStructureException : Exception
    {
        /// <inheritdoc/>
        public InvalidDevicePathProtocolStructureException(string message)
            : base(message) { }
    }
}
