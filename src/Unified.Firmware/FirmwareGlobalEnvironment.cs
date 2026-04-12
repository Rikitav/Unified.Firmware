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

#pragma warning disable IDE1006
namespace Unified.Firmware;

/// <summary>
/// Interface for reading and editing UEFI environment variables
/// </summary>
public static class FirmwareGlobalEnvironment
{
    /// <summary>
    /// Whether the system is operating in Audit Mode (1) or not (0). All other values are reserved. Should be treated as read-only except when DeployedMode is 0. Always becomes read-only after ExitBootServices() is called.
    /// Variable attributes : BS, RT
    /// </summary>
    public static bool AuditMode
    {
        get => FirmwareUtilities.ReadVariable<bool>(nameof(AuditMode));
    }

    /// <summary>
    /// The boot option that was selected for the current boot.
    /// Variable attributes : BS, RT
    /// </summary>
    public static ushort BootCurrent
    {
        get => FirmwareUtilities.ReadVariable<ushort>(nameof(BootCurrent));
    }

    /// <summary>
    /// The boot option for the next boot only.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public static ushort BootNext
    {
        get => FirmwareUtilities.ReadVariable<ushort>(nameof(BootNext));
        set => FirmwareUtilities.WriteVariable(nameof(BootNext), value); // VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
    }

    /// <summary>
    /// The ordered boot option load list.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public static ushort[] BootOrder
    {
        get => FirmwareUtilities.ReadArrayVariable<ushort>(nameof(BootOrder));
        set => FirmwareUtilities.WriteArrayVariable<ushort>(nameof(BootOrder), value); // VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
    }

    /// <summary>
    /// The types of boot options supported by the boot manager. Should be treated as read-only.
    /// Variable attributes : BS, RT
    /// </summary>
    public static uint BootOptionSupport
    {
        get => FirmwareUtilities.ReadVariable<uint>(nameof(BootOptionSupport));
        set => FirmwareUtilities.WriteVariable(nameof(BootOptionSupport), value); // VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
    }

    /*
    /// <summary>
    /// The device path of the default input console.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public static EFI_DEVICE_PATH_PROTOCOL ConIn
    {
        get => FirmwareUtilities.ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConIn));
        //set => FirmwareUtilities.WriteVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConIn), value, VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /// <summary>
    /// The device path of all possible console input devices.
    /// Variable attributes : BS, RT
    /// </summary>
    public static EFI_DEVICE_PATH_PROTOCOL ConInDev
    {
        get => FirmwareUtilities.ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConInDev));
        //set => FirmwareUtilities.WriteVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConInDev), value, VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /// <summary>
    /// The device path of the default output console.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public static EFI_DEVICE_PATH_PROTOCOL ConOut
    {
        get => FirmwareUtilities.ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConOut));
        //set => FirmwareUtilities.WriteVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConOut), value, VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /// <summary>
    /// The device path of all possible console output devices.
    /// Variable attributes : BS, RT
    /// </summary>
    public static EFI_DEVICE_PATH_PROTOCOL ConOutDev
    {
        get => FirmwareUtilities.ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConOutDev));
        //set => FirmwareUtilities.WriteVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConOutDev), value, VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }
    */

    /// <summary>
    /// Whether the system is operating in Deployed Mode (1) or not (0). All other values are reserved. Should be treated as read-only when its value is 1. Always becomes read-only after ExitBootServices() is called.
    /// Variable attributes : BS, RT
    /// </summary>
    public static bool DeployedMode
    {
        get => FirmwareUtilities.ReadVariable<bool>(nameof(DeployedMode));
    }

    /// <summary>
    /// Whether the platform firmware is operating in device authentication boot mode (1) or not (0). All other values are reserved. Should be treated as read-only.
    /// Variable attributes : BS, RT
    /// </summary>
    public static bool devAuthBoot
    {
        get => FirmwareUtilities.ReadVariable<bool>(nameof(devAuthBoot));
    }

    /// <summary>
    /// The ordered driver load option list.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public static ushort[] DriverOrder
    {
        get => FirmwareUtilities.ReadArrayVariable<ushort>(nameof(DriverOrder));
        set => FirmwareUtilities.WriteArrayVariable(nameof(DriverOrder), value); // VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
    }

    /*
    /// <summary>
    /// The device path of the default error output device.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public static EFI_DEVICE_PATH_PROTOCOL ErrOut
    {
        get => FirmwareUtilities.ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ErrOut));
    }

    /// <summary>
    /// The device path of all possible error output devices.
    /// Variable attributes : BS, RT
    /// </summary>
    public static EFI_DEVICE_PATH_PROTOCOL ErrOutDev
    {
        get => FirmwareUtilities.ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ErrOutDev));
    }
    */

    /// <summary>
    /// Identifies the level of hardware error record persistence support implemented by the platform. This variable is only modified by firmware and is read-only to the OS.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public static ushort HwErrRecSupport
    {
        get => FirmwareUtilities.ReadVariable<ushort>(nameof(HwErrRecSupport));
    }

    /// <summary>
    /// The language code that the system is configured for. This value is deprecated.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public static string Lang
    {
        get => FirmwareUtilities.ReadStringVariable(nameof(Lang));
        set => FirmwareUtilities.WriteStringVariable(nameof(Lang), value); // VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
    }

    /// <summary>
    /// The language codes that the firmware supports. This value is deprecated.
    /// Variable attributes : BS, RT
    /// </summary>
    public static string LangCodes
    {
        get => FirmwareUtilities.ReadStringVariable(nameof(LangCodes));
        set => FirmwareUtilities.WriteStringVariable(nameof(LangCodes), value); // VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
    }

    /// <summary>
    /// Allows the OS to request the firmware to enable certain features and to take certain actions.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public static EfiOsIindications OsIndications
    {
        get => (EfiOsIindications)FirmwareUtilities.ReadVariable<long>(nameof(OsIndications));
        set => FirmwareUtilities.WriteVariable(nameof(OsIndications), (long)value); // VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
    }

    /// <summary>
    /// Allows the firmware to indicate supported features and actions to the OS.
    /// Variable attributes : BS, RT
    /// </summary>
    public static EfiOsIindications OsIndicationsSupported
    {
        get => (EfiOsIindications)FirmwareUtilities.ReadVariable<long>(nameof(OsIndicationsSupported));
    }

    /// <summary>
    /// OS-specified recovery options.
    /// Variable attributes : BS, RT, NV, AT
    /// </summary>
    public static ushort[] OsRecoveryOrder
    {
        get => FirmwareUtilities.ReadArrayVariable<ushort>(nameof(OsRecoveryOrder));
        set => FirmwareUtilities.WriteArrayVariable<ushort>(nameof(OsRecoveryOrder), value); // VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS | VariableAttributes.NON_VOLATILE | VariableAttributes.AUTHENTICATED_WRITE_ACCESS
    }

    /// <summary>
    /// The language codes that the firmware supports.
    /// Variable attributes : BS, RT
    /// </summary>
    public static string PlatformLangCodes
    {
        get => FirmwareUtilities.ReadStringVariable(nameof(PlatformLangCodes));
        set => FirmwareUtilities.WriteStringVariable(nameof(PlatformLangCodes), value); // VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
    }

    /// <summary>
    /// The language code that the system is configured for.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public static string PlatformLang
    {
        get => FirmwareUtilities.ReadStringVariable(nameof(PlatformLang));
        set => FirmwareUtilities.WriteStringVariable(nameof(PlatformLang), value); // VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
    }

    /// <summary>
    /// Array of GUIDs representing the type of signatures supported by the platform firmware. Should be treated as read-only.
    /// Variable attributes : BS, RT
    /// </summary>
    public static Guid[] SignatureSupport
    {
        get => FirmwareUtilities.ReadArrayVariable<Guid>(nameof(PlatformLang));
    }

    /// <summary>
    /// Whether the platform firmware is operating in Secure boot mode (1) or not (0). All other values are reserved. Should be treated as read-only.
    /// Variable attributes : BS, RT
    /// </summary>
    public static bool SecureBoot
    {
        get => FirmwareUtilities.ReadVariable<bool>(nameof(SecureBoot));
    }

    /// <summary>
    /// Whether the system should require authentication on SetVariable() requests to Secure Boot policy variables (0) or not (1). Should be treated as read-only. The system is in "Setup Mode" when SetupMode==1, AuditMode==0, and DeployedMode==0.
    /// Variable attributes : BS, RT
    /// </summary>
    public static bool SetupMode
    {
        get => FirmwareUtilities.ReadVariable<bool>(nameof(SetupMode));
    }

    /// <summary>
    /// The firmware's boot managers timeout, in seconds, before initiating the default boot selection.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public static ushort Timeout
    {
        get => FirmwareUtilities.ReadVariable<ushort>(nameof(Timeout));
        set => FirmwareUtilities.WriteVariable(nameof(Timeout), value); // VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS
    }

    /// <summary>
    /// Whether the system is configured to use only vendor-provided keys or not. Should be treated as read-only.
    /// Variable attributes : BS, RT
    /// </summary>
    public static bool VendorKeys
    {
        get => FirmwareUtilities.ReadVariable<bool>(nameof(VendorKeys));
    }

    /*
    /// <summary>
    /// A boot load option. #### is a printed hex value. No 0x or h is included in the hex value.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public static string BootOption(short OptIndex) => "Boot" + OptIndex.ToString("X").PadLeft(4, '0');

    /// <summary>
    /// A driver load option. #### is a printed hex value.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public static string DriverOption(short OptIndex) => "Driver" + OptIndex.ToString("X").PadLeft(4, '0');

    /// <summary>
    /// Describes hot key relationship with a Boot#### load option.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public static string KeyOption(short OptIndex) => "Key" + OptIndex.ToString("X").PadLeft(4, '0');

    /// <summary>
    /// Platform-specified recovery options. These variables are only modified by firmware and are read-only to the OS.
    /// Variable attributes : BS, RT
    /// </summary>
    public static string PlatformRecoveryOption(short OptIndex) => "PlatformRecovery" + OptIndex.ToString("X").PadLeft(4, '0');

    /// <summary>
    /// A System Prep application load option containing an EFI_LOAD_OPTION descriptor. #### is a printed hex value.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public static string SysPrepOption(short OptIndex) => "SysPrep" + OptIndex.ToString("X").PadLeft(4, '0');
    */
}
