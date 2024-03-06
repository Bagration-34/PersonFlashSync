using System;
using System.IO;

namespace PersonFlashSync;

class Program
{
    static DriveInfo usbFlash;

    static void Main(string[] args)
    {
        Console.WriteLine("***** PERSON FLASH SYNC *****\n");

        DriveInfo[] myDrives = DriveInfo.GetDrives();

        

        // Вывести сведения об устройствах
        foreach (DriveInfo d in myDrives)
        {
            Console.WriteLine("Name: {0}", d.Name);
            Console.WriteLine("HashCode: {0}", d.GetHashCode());
            Console.WriteLine();
            if (d.GetHashCode() == 59941933) usbFlash = d;
        }

        DirectoryInfo dirUsbFlash = new DirectoryInfo($@"{usbFlash.Name}test");
        DirectoryInfo dirPC = new DirectoryInfo($@"C:\test");

        FileInfo[] filesPC = dirPC.GetFiles();
        foreach (FileInfo file in filesPC)
        {
            //if (dirUsbFlash)
        }

        Console.ReadLine();
    }
}