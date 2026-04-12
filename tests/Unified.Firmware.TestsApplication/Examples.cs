// The MIT License (MIT)
// 
// Copyright 2026 © Rikitav Tim4ik
// 
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

using Unified.Firmware.BootService;
using Unified.Firmware.BootService.LoadOption;
using Unified.Firmware.BootService.Protocols;
using Unified.Firmware.SystemPartition;

namespace Unified.Firmware.TestsApplication;

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
