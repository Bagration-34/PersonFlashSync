using System;
using System.IO;
using System.Threading;

namespace PersonFlashSync;

class Program
{
    const string RootDirectory = "test";

    static void Main(string[] args)
    {
        Console.WriteLine("***** PERSON FLASH SYNC *****\n");

        DriveInfo usbFlash;
        do
        {
            Console.WriteLine("Drive search...");
            usbFlash = IdentifyDrive();
            if (usbFlash == null) Thread.Sleep(1000);

        } while (usbFlash == null);

        Console.WriteLine("Drive is detected!\n");

        DirectoryInfo dirPC = new DirectoryInfo($@"C:\{RootDirectory}");
        DirectoryInfo dirUsbFlash = new DirectoryInfo($@"{usbFlash.Name}{RootDirectory}");
        if (!dirUsbFlash.Exists) dirUsbFlash.Create();

        Console.WriteLine("PC -> USB");
        ScanDirectoryAndCopyFiles(dirPC, dirUsbFlash);
        Console.WriteLine("USB -> PC");
        ScanDirectoryAndCopyFiles(dirUsbFlash, dirPC);
        Console.WriteLine("\nDone!");
    }

    static DriveInfo IdentifyDrive()
    {
        DriveInfo[] myDrives = DriveInfo.GetDrives();

        // Вывести сведения об устройствах
        foreach (DriveInfo d in myDrives)
        {
            if (!d.IsReady) continue;
            if (d.VolumeLabel == "UN") return d;
        }
        return null;
    }

    static void ScanDirectoryAndCopyFiles(DirectoryInfo source, DirectoryInfo destination)
    {
        if (!destination.Exists) destination.Create();

        // Для начала перебираем все файлы
        FileInfo[] sourceFiles = source.GetFiles();
        foreach (FileInfo file in sourceFiles)
        {
            string destPath = @$"{destination.FullName}{Path.DirectorySeparatorChar}{file.Name}";
            if (File.Exists(destPath))
            {
                DateTime dateOfSource = file.LastWriteTime;
                DateTime dateOfDest = File.GetLastWriteTime(destPath);
                if (DateTime.Compare(dateOfSource, dateOfDest) > 0)
                {
                    Console.WriteLine(@$"File: {file.FullName} copy with replace to drive");
                    file.CopyTo(destPath, true);
                }
                else
                {
                    Console.WriteLine(@$"File: {file.FullName} already on the device");
                }
            }
            else
            {
                Console.WriteLine(@$"File: {file.FullName} copy to drive");
                file.CopyTo(destPath);
            }
        }

        DirectoryInfo[] sourceDir = source.GetDirectories();
        if (sourceDir.Length == 0) return;
        foreach (DirectoryInfo dir in sourceDir)
        {
            DirectoryInfo subDir = new DirectoryInfo($@"{destination.FullName}{Path.DirectorySeparatorChar}{dir.Name}");
            ScanDirectoryAndCopyFiles(dir, subDir);
        }
    }
}