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

namespace Unified.Firmware.EnvironmentVendor;

/// <summary>
/// GLobally defined UEFI environment variables. Documentation : https://uefi.org/specs/UEFI/2.10/03_Boot_Manager.html#globally-defined-variables
/// </summary>
/// <param name="backend"></param>
public class GlobalFirmwareEnvironment(IFirmwareBackend backend) : FirmwareEnvironment(backend, new Guid("8BE4DF61-93CA-11D2-AA0D-00E098032B8C"))
{
    /// <summary>
    /// Whether the system is operating in Audit Mode (1) or not (0). All other values are reserved. Should be treated as read-only except when DeployedMode is 0. Always becomes read-only after ExitBootServices() is called.
    /// Variable attributes : BS, RT
    /// </summary>
    public bool AuditMode
    {
        get => ReadVariable<bool>(nameof(AuditMode), out _);
    }

    /// <summary>
    /// The boot option that was selected for the current boot.
    /// Variable attributes : BS, RT
    /// </summary>
    public ushort BootCurrent
    {
        get => ReadVariable<ushort>(nameof(BootCurrent), out _);
    }

    /// <summary>
    /// The boot option for the next boot only.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public ushort BootNext
    {
        get => ReadVariable<ushort>(nameof(BootNext), out _);
        set => WriteVariable(nameof(BootNext), value, VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /// <summary>
    /// The ordered boot option load list.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public ushort[] BootOrder
    {
        get => ReadArrayVariable<ushort>(nameof(BootOrder), out _);
        set => WriteArrayVariable<ushort>(nameof(BootOrder), value, VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /// <summary>
    /// The types of boot options supported by the boot manager. Should be treated as read-only.
    /// Variable attributes : BS, RT
    /// </summary>
    public uint BootOptionSupport
    {
        get => ReadVariable<uint>(nameof(BootOptionSupport), out _);
        set => WriteVariable(nameof(BootOptionSupport), value, VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /*
    /// <summary>
    /// The device path of the default input console.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public EFI_DEVICE_PATH_PROTOCOL ConIn
    {
        get => ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConIn));
        //set => WriteVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConIn), value, VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /// <summary>
    /// The device path of all possible console input devices.
    /// Variable attributes : BS, RT
    /// </summary>
    public EFI_DEVICE_PATH_PROTOCOL ConInDev
    {
        get => ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConInDev));
        //set => WriteVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConInDev), value, VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /// <summary>
    /// The device path of the default output console.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public EFI_DEVICE_PATH_PROTOCOL ConOut
    {
        get => ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConOut));
        //set => WriteVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConOut), value, VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /// <summary>
    /// The device path of all possible console output devices.
    /// Variable attributes : BS, RT
    /// </summary>
    public EFI_DEVICE_PATH_PROTOCOL ConOutDev
    {
        get => ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConOutDev));
        //set => WriteVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ConOutDev), value, VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }
    */

    /// <summary>
    /// Whether the system is operating in Deployed Mode (1) or not (0). All other values are reserved. Should be treated as read-only when its value is 1. Always becomes read-only after ExitBootServices() is called.
    /// Variable attributes : BS, RT
    /// </summary>
    public bool DeployedMode
    {
        get => ReadVariable<bool>(nameof(DeployedMode), out _);
    }

    /// <summary>
    /// Whether the platform firmware is operating in device authentication boot mode (1) or not (0). All other values are reserved. Should be treated as read-only.
    /// Variable attributes : BS, RT
    /// </summary>
    public bool DevAuthBoot
    {
        get => ReadVariable<bool>("devAuthBoot", out _);
    }

    /// <summary>
    /// The ordered driver load option list.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public ushort[] DriverOrder
    {
        get => ReadArrayVariable<ushort>(nameof(DriverOrder), out _);
        set => WriteArrayVariable(nameof(DriverOrder), value, VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /*
    /// <summary>
    /// The device path of the default error output device.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public EFI_DEVICE_PATH_PROTOCOL ErrOut
    {
        get => ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ErrOut));
    }

    /// <summary>
    /// The device path of all possible error output devices.
    /// Variable attributes : BS, RT
    /// </summary>
    public EFI_DEVICE_PATH_PROTOCOL ErrOutDev
    {
        get => ReadVariable<EFI_DEVICE_PATH_PROTOCOL>(nameof(ErrOutDev));
    }
    */

    /// <summary>
    /// Identifies the level of hardware error record persistence support implemented by the platform. This variable is only modified by firmware and is read-only to the OS.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public ushort HwErrRecSupport
    {
        get => ReadVariable<ushort>(nameof(HwErrRecSupport), out _);
    }

    /// <summary>
    /// The language code that the system is configured for. This value is deprecated.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public string Lang
    {
        get => ReadStringVariable(nameof(Lang), out _);
        set => WriteStringVariable(nameof(Lang), value, VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /// <summary>
    /// The language codes that the firmware supports. This value is deprecated.
    /// Variable attributes : BS, RT
    /// </summary>
    public string LangCodes
    {
        get => ReadStringVariable(nameof(LangCodes), out _);
        set => WriteStringVariable(nameof(LangCodes), value, VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /// <summary>
    /// Allows the OS to request the firmware to enable certain features and to take certain actions.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public OsIndications OsIndications
    {
        get => (OsIndications)ReadVariable<long>(nameof(OsIndications), out _);
        set => WriteVariable(nameof(OsIndications), (long)value, VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /// <summary>
    /// Allows the firmware to indicate supported features and actions to the OS.
    /// Variable attributes : BS, RT
    /// </summary>
    public OsIndications OsIndicationsSupported
    {
        get => (OsIndications)ReadVariable<long>(nameof(OsIndicationsSupported), out _);
    }

    /// <summary>
    /// OS-specified recovery options.
    /// Variable attributes : BS, RT, NV, AT
    /// </summary>
    public ushort[] OsRecoveryOrder
    {
        get => ReadArrayVariable<ushort>(nameof(OsRecoveryOrder), out _);
        set => WriteArrayVariable<ushort>(nameof(OsRecoveryOrder), value, VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS | VariableAttributes.NON_VOLATILE | VariableAttributes.AUTHENTICATED_WRITE_ACCESS);
    }

    /// <summary>
    /// The language codes that the firmware supports.
    /// Variable attributes : BS, RT
    /// </summary>
    public string PlatformLangCodes
    {
        get => ReadStringVariable(nameof(PlatformLangCodes), out _);
        set => WriteStringVariable(nameof(PlatformLangCodes), value, VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /// <summary>
    /// The language code that the system is configured for.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public string PlatformLang
    {
        get => ReadStringVariable(nameof(PlatformLang), out _);
        set => WriteStringVariable(nameof(PlatformLang), value, VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /// <summary>
    /// Array of GUIDs representing the type of signatures supported by the platform firmware. Should be treated as read-only.
    /// Variable attributes : BS, RT
    /// </summary>
    public Guid[] SignatureSupport
    {
        get => ReadArrayVariable<Guid>(nameof(PlatformLang), out _);
    }

    /// <summary>
    /// Whether the platform firmware is operating in Secure boot mode (1) or not (0). All other values are reserved. Should be treated as read-only.
    /// Variable attributes : BS, RT
    /// </summary>
    public bool SecureBoot
    {
        get => ReadVariable<bool>(nameof(SecureBoot), out _);
    }

    /// <summary>
    /// Whether the system should require authentication on SetVariable() requests to Secure Boot policy variables (0) or not (1). Should be treated as read-only. The system is in "Setup Mode" when SetupMode==1, AuditMode==0, and DeployedMode==0.
    /// Variable attributes : BS, RT
    /// </summary>
    public bool SetupMode
    {
        get => ReadVariable<bool>(nameof(SetupMode), out _);
    }

    /// <summary>
    /// The firmware's boot managers timeout, in seconds, before initiating the default boot selection.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public ushort Timeout
    {
        get => ReadVariable<ushort>(nameof(Timeout), out _);
        set => WriteVariable(nameof(Timeout), value, VariableAttributes.NON_VOLATILE | VariableAttributes.BOOTSERVICE_ACCESS | VariableAttributes.RUNTIME_ACCESS);
    }

    /// <summary>
    /// Whether the system is configured to use only vendor-provided keys or not. Should be treated as read-only.
    /// Variable attributes : BS, RT
    /// </summary>
    public bool VendorKeys
    {
        get => ReadVariable<bool>(nameof(VendorKeys), out _);
    }

    /*
    /// <summary>
    /// A boot load option. #### is a printed hex value. No 0x or h is included in the hex value.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public string BootOption(short OptIndex) => "Boot" + OptIndex.ToString("X").PadLeft(4, '0');

    /// <summary>
    /// A driver load option. #### is a printed hex value.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public string DriverOption(short OptIndex) => "Driver" + OptIndex.ToString("X").PadLeft(4, '0');

    /// <summary>
    /// Describes hot key relationship with a Boot#### load option.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public string KeyOption(short OptIndex) => "Key" + OptIndex.ToString("X").PadLeft(4, '0');

    /// <summary>
    /// Platform-specified recovery options. These variables are only modified by firmware and are read-only to the OS.
    /// Variable attributes : BS, RT
    /// </summary>
    public string PlatformRecoveryOption(short OptIndex) => "PlatformRecovery" + OptIndex.ToString("X").PadLeft(4, '0');

    /// <summary>
    /// A System Prep application load option containing an EFI_LOAD_OPTION descriptor. #### is a printed hex value.
    /// Variable attributes : NV, BS, RT
    /// </summary>
    public string SysPrepOption(short OptIndex) => "SysPrep" + OptIndex.ToString("X").PadLeft(4, '0');
    */
}
