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
/// The firmware and an Operating System may exchange information through the OsIndicationsSupported and the OSIndications variables as follows
/// </summary>
[Flags]
public enum OsIndications : ulong
{
    /// <summary>
    /// Nothing
    /// </summary>
    None = 0,

    /// <summary>
    /// Once the firmware consumes this bit in the OsIndications variable and stops at the firmware user interface
    /// </summary>
    BOOT_TO_FW_UI = 0x0000000000000001,

    /// <summary>
    /// If the firmware supports timestamp based revocation and the � dbt � uthorized timestamp database variable.
    /// </summary>
    TIMESTAMP_REVOCATION = 0x0000000000000002,

    /// <summary>
    /// When submitting capsule via the Mass Storage Device method of Delivery of Capsules via file on Mass Storage Device, the bit EFI_OS_INDICATIONS_FILE_CAPSULE_DELIVERY_SUPPORTED in OsIndications variable must be set by submitter to trigger processing of submitted capsule on next reboot
    /// </summary>
    FILE_CAPSULE_DELIVERY_SUPPORTED = 0x0000000000000004,

    /// <summary>
    /// If platform supports processing of Firmware Management Protocol update capsule as defined in <see href="https://uefi.org/specs/UEFI/2.9_A/23_Firmware_Update_and_Reporting.html#dependency-expression-instruction-set">Dependency Expression Instruction Set</see>
    /// </summary>
    FMP_CAPSULE_SUPPORTED = 0x0000000000000008,

    /// <summary>
    /// If platform supports reporting of deferred capsule processing by creation of result variable as defined in <see href="https://uefi.org/specs/UEFI/2.9_A/08_Services_Runtime_Services.html#uefi-variable-reporting-on-the-success-or-any-errors-encountered-in-processing-of-capsules-after-restart">UEFI variable reporting on the Success or any Errors encountered in processing of capsules after restart</see>
    /// </summary>
    CAPSULE_RESULT_VAR_SUPPORTED = 0x0000000000000010,

    /// <summary>
    /// Indicate that OS-defined recovery should commence upon reboot
    /// </summary>
    START_OS_RECOVERY = 0x0000000000000020,

    /// <summary>
    /// Indicate that Platform-defined recovery should commence upon reboot
    /// </summary>
    START_PLATFORM_RECOVERY = 0x0000000000000040,

    /// <summary>
    /// Bit is set in the OsIndications variable by submitter to trigger collecting current configuration and reporting the refreshed data to EFI System Configuration Table on next boot
    /// </summary>
    JSON_CONFIG_DATA_REFRESH = 0x0000000000000080
}
