using System;
using System.IO;
using System.Threading;
using System.Text.Json;

namespace PersonFlashSync;

class Program
{
    static void Main(string[] args)
    {
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
            foreach (DriveInfo d in myDrives) // Поиск нужного устройства
            {
                if (!d.IsReady) continue;
                if (d.VolumeLabel == device.VolumeLabel) usbDrive = d;
            }
            if (usbDrive != null) break;
            Thread.Sleep(500);
        }
        if (usbDrive == null) return;

        foreach (DirectoryPair dp in device.DirectoryPairs)
        {
            DirectoryProcess(dp);
        }

        void DirectoryProcess(DirectoryPair directoryPair)
        {
            DirectoryInfo dirPC = new DirectoryInfo(directoryPair.DirOnPC);
            DirectoryInfo dirUsbFlash = new DirectoryInfo($@"{usbDrive.Name}{directoryPair.DirOnUsb}");

            // PC -> USB
            if (directoryPair.Direction == Directions.bidirectional ||
                directoryPair.Direction == Directions.PcToUsb)
            {
                ScanDirectoryAndCopyFiles(dirPC, dirUsbFlash);
            }
            // USB -> PC
            if (directoryPair.Direction == Directions.bidirectional ||
                directoryPair.Direction == Directions.UsbToPc)
            {
                ScanDirectoryAndCopyFiles(dirUsbFlash, dirPC);
            }
        }
    }

    static Config GetConfig()
    {
        using (FileStream fs = new FileStream("config.json", FileMode.Open))
        {
            Config config = JsonSerializer.Deserialize<Config>(fs);
            Console.WriteLine(config.Devices[0].DirectoryPairs[0].DirOnPC);
            return config;
        }
    }

    static void ScanDirectoryAndCopyFiles(DirectoryInfo source, DirectoryInfo destination)
    {
        if (!source.Exists) 
        {
            source.Create();
            return;
        }
        if (!destination.Exists) destination.Create();

        // Для начала перебираем все файлы
        FileInfo[] sourceFiles = source.GetFiles();
        foreach (FileInfo sourcefile in sourceFiles)
        {
            string destPath = @$"{destination.FullName}{Path.DirectorySeparatorChar}{sourcefile.Name}";
            if (File.Exists(destPath))
            {
                // Из-за того, что при копировании с PC на USB устройство (FAT32) к времени изменения файла прибовляется примерно +2 секунды
                if (sourcefile.LastWriteTime > File.GetLastWriteTime(destPath).AddSeconds(3)) // Если у источника более новый 
                {
                    // Сохраняем старый файл и копируем с заменой
                    DirectoryInfo dirBackup = new DirectoryInfo(@$"{destination.FullName}{Path.DirectorySeparatorChar}backup");
                    if (!dirBackup.Exists) dirBackup.Create();
                    File.Copy(destPath,  @$"{dirBackup.FullName}{Path.DirectorySeparatorChar}{sourcefile.Name}", true);  // Копируем старый файл в бэкап
                    sourcefile.CopyTo(destPath, true);            // Перезаписываем
                    File.SetLastWriteTime(destPath, sourcefile.LastWriteTime);
                }
                else
                {
                    // Если у источника более старый файл - Ничего не делаем
                }
            }
            else
            {
                sourcefile.CopyTo(destPath); // Если ничего нет, копируем на устройство
            }
        }

        // Перебираем все директории
        DirectoryInfo[] sourceDir = source.GetDirectories();
        if (sourceDir.Length == 0) return;
        foreach (DirectoryInfo dir in sourceDir)
        {
            if (dir.Name == "backup") continue; // Не синхронизируем папку бэкап
            DirectoryInfo subDir = new DirectoryInfo($@"{destination.FullName}{Path.DirectorySeparatorChar}{dir.Name}");
            ScanDirectoryAndCopyFiles(dir, subDir);
        }
    }
}