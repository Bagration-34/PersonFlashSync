using System;
using System.IO;
using System.Threading;
using System.Text.Json;
using System.Reflection;
using System.Diagnostics;

// Создавать папку Backup для сохранения старых версий

namespace PersonFlashSync;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("***** PERSON FLASH SYNC *****\n");

        Device[] devices = GetConfig().Devices.ToArray();
        if (devices.Length == 0) return;
        foreach (Device d in devices)
        {
            DeviceProcess(d);
        }
    }

    static void DeviceProcess(Device device)
    {
        DriveInfo[] myDrives = DriveInfo.GetDrives();
        DriveInfo usbDrive = null;

        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine(@$"Search {device.VolumeLabel}...");
            foreach (DriveInfo d in myDrives)
            {
                if (!d.IsReady) continue;
                if (d.VolumeLabel == device.VolumeLabel) usbDrive = d;
            }
            if (usbDrive != null) break;
            Thread.Sleep(500);
        }
        if (usbDrive == null) return;

        Console.WriteLine("Drive is detected!\n");

        DirectoryInfo dirPC = new DirectoryInfo(device.dirOnPC);
        if (!dirPC.Exists) dirPC.Create();
        DirectoryInfo dirUsbFlash = new DirectoryInfo($@"{usbDrive.Name}{device.dirOnUsb}");
        if (!dirUsbFlash.Exists) dirUsbFlash.Create();

        Console.WriteLine("PC -> USB");
        ScanDirectoryAndCopyFiles(dirPC, dirUsbFlash);
        Console.WriteLine("USB -> PC");
        ScanDirectoryAndCopyFiles(dirUsbFlash, dirPC);
        Console.WriteLine("\nDone!");
    }

    static Config GetConfig()
    {
        using (FileStream fs = new FileStream("config.json", FileMode.Open))
        {
            Config? config = JsonSerializer.Deserialize<Config>(fs);
            return config;
        }
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