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
            if (!d.IsReady) continue;
            Console.WriteLine("Name: {0}", d.Name);
            Console.WriteLine("VolumeLabel: {0}", d.VolumeLabel);
            Console.WriteLine();
            if (d.VolumeLabel == "UN") usbFlash = d;
        }

        DirectoryInfo dirUsbFlash = new DirectoryInfo($@"{usbFlash.Name}test");
        DirectoryInfo dirPC = new DirectoryInfo($@"C:\test");

        ScanDirectoryAndCopyFiles(dirPC, dirUsbFlash);
        

        Console.ReadLine();
    }

    static void ScanDirectoryAndCopyFiles(DirectoryInfo source, DirectoryInfo purpose)
    {
        string path = null;

        // Для начала перебираем все файлы
        FileInfo[] filesPC = source.GetFiles();
        foreach (FileInfo file in filesPC)
        {
            if (File.Exists(@$"{purpose.FullName}{Path.DirectorySeparatorChar}{file.Name}"))
            {

            }
        }
    }
}