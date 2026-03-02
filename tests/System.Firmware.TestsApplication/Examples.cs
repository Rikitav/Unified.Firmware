using System.Firmware.BootService;
using System.Firmware.BootService.LoadOption;
using System.Firmware.BootService.Protocols;
using System.Firmware.SystemPartition;

namespace System.Firmware.TestsApplication;

public static class Examples
{
    public static void EnumerateOptions()
    {
        // Enumerating all boot options in boot order
        int index = 0;
        foreach (FirmwareBootOption bootOption in FirmwareBootService.EnumerateBootOptions())
        {
            // Showing basic information
            Console.WriteLine("\n===[ Boot option #" + index++ + " ]" + new string('=', 60));
            Console.WriteLine("Option name : \"{0}\"", bootOption.Description);
            Console.WriteLine("Attributes  : {0}", bootOption.Attributes);

            // Enumerating all protocols
            foreach (DevicePathProtocolBase protocol in bootOption.Protocols)
                Console.WriteLine(protocol.ToString()); // Getting string representation of protocol
        }
    }

    public static void CreatingOption()
    {
        // Setting device path protocols
        // This protocols instructions boot service how to load your option
        DevicePathProtocolBase[] protocols = [
            
            // The partition on which the bootloader is located
            new HardDriveProtocol(new Guid("XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX")),
            
            // Path to the EFI application to be loaded
            new FilePathProtocol("EFI\\MyApplication\\bootx64.efi"),
        ];

        // Creating simple load option
        FirmwareBootOption bootOption = new FirmwareBootOption(LoadOptionAttributes.ACTIVE, "MyLoader", protocols, []);

        // Creating new load option
        BootOptionIndex newOption = FirmwareBootService.CreateLoadOption(bootOption, true);

        // Logging new boot option index
        Console.WriteLine("Boot option sucessfully created, new option index : {0}", newOption);
    }

    public static void ReadingOption()
    {
        // Reading boot option
        FirmwareBootOption bootOption = FirmwareBootService.ReadLoadOption(0x0003); // <-- Set here your variable index

        // Showing basic information
        Console.WriteLine("Option name : \"{0}\"", bootOption.Description);
        Console.WriteLine("Attributes  : {0}", bootOption.Attributes);

        // Enumerating all protocols
        foreach (DevicePathProtocolBase protocol in bootOption.Protocols)
            Console.WriteLine(protocol.ToString()); // Getting string representation of protocol
    }

    public static void GettingEsp()
    {
        // Getting 'EFI system partition' volume path
        string EspVolumePath = EfiPartition.VolumePath;

        // Example reading config file from ESP using volume path, instead of using MountVol
        string configText = File.ReadAllText(Path.Combine(EspVolumePath, "EFI", "Ubuntu", "grub.cfg"));

        // Dumping result
        Console.WriteLine(configText);
    }
}
