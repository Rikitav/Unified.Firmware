using System;

namespace Unified.Firmware.EnvironmentVendor;

/// <summary>
/// Contains known GUIDs of UEFI variable namespaces.
/// </summary>
public static class FirmwareVendors
{
    // =========================================================================
    // 1. GLOBAL STANDARDS (UEFI Specification)
    // =========================================================================

    /// <summary>
    /// Global UEFI namespace (EFI_GLOBAL_VARIABLE).
    /// Stores base variables: BootOrder, BootNext, Lang, Timeout, SetupMode, SecureBoot, etc.
    /// </summary>
    public static readonly Guid GlobalVariable = new Guid("8BE4DF61-93CA-11D2-AA0D-00E098032B8C");

    /// <summary>
    /// Image security database namespace (EFI_IMAGE_SECURITY_DATABASE_GUID).
    /// Stores Secure Boot keys: db (allowed certificates), dbx (revoked certificates), dbt (timestamping).
    /// </summary>
    public static readonly Guid ImageSecurityDatabase = new Guid("D719B2CB-3D3A-4596-A3BC-DAD00E67656F");

    /// <summary>
    /// Hardware error namespace (EFI_HARDWARE_ERROR_VARIABLE).
    /// Used by the WHEA (Windows Hardware Error Architecture) mechanism to record fatal hardware errors in NVRAM (HwErrRec).
    /// </summary>
    public static readonly Guid HardwareErrorVariable = new Guid("414E6BDD-E47B-47CC-B244-BB610208F4EF");

    /// <summary>
    /// Capsule update result reports (EFI_CAPSULE_REPORT_GUID).
    /// </summary>
    public static readonly Guid CapsuleReport = new Guid("39B68C46-F7FB-441B-B6D1-E15C1B773062");

    // =========================================================================
    // 2. OPERATING SYSTEMS AND PLATFORMS
    // =========================================================================

    /// <summary>
    /// Microsoft namespace.
    /// Often used for variables related to BitLocker, kernel signatures, and early boot telemetry (e.g., CurrentPolicy).
    /// </summary>
    public static readonly Guid MicrosoftVendor = new Guid("77FA9ABD-0359-4D32-BD60-28F4E78F784B");

    // =========================================================================
    // 3. OEM VENDORS (Hardware specific)
    // =========================================================================

    /// <summary>
    /// Lenovo (ThinkPad) specific variables.
    /// Used to manage BIOS settings directly from the OS (e.g., WMI-to-UEFI).
    /// </summary>
    public static readonly Guid LenovoVendor = new Guid("C020489E-6DB2-4EF2-9AA5-CA06FC11D36A");

    /// <summary>
    /// Dell specific variables.
    /// Often used to control keyboard backlighting, power profiles, and component inventory (CBOM).
    /// </summary>
    public static readonly Guid DellVendor = new Guid("4BCFDDBD-65F0-4FC7-BF96-981FBB0EBFF4");

    /// <summary>
    /// HP (Hewlett-Packard) specific variables.
    /// Used for BIOS SureStart, security settings, and hardware tokens.
    /// </summary>
    public static readonly Guid HpVendor = new Guid("577FA4AD-1A3E-4BCE-A268-3F173B00DBEC");

    /// <summary>
    /// ASUS specific variables.
    /// Often found on ROG/Prime motherboards to control memory timings, LED lighting (Aura), and overclocking profiles.
    /// </summary>
    public static readonly Guid AsusVendor = new Guid("20C731A8-79C0-4E80-AFDE-0C3DEB22CDAE");

    /// <summary>
    /// Apple (Mac EFI) specific variables.
    /// Store parameters like StartupMute, Bluetooth pairings for peripherals prior to OS boot, and the selected boot volume (efi-boot-device).
    /// </summary>
    public static readonly Guid AppleVendor = new Guid("7C436110-AB2A-4BBB-A880-FE41995C9F82");
}
