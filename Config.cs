

using System.Collections.Generic;

namespace PersonFlashSync
{
    struct Config
    {
        public List<Device> Devices { get; set; }
    }

    struct Device
    {
        public string VolumeLabel { get; set; }
        public DirectoryPair[] DirectoryPairs { get; set; }
    }

    class DirectoryPair
    {
        public int Direction { get; set; } = 2;
        public string DirOnPC { get; set; }
        public string DirOnUsb { get; set; }
    }

    enum Directions 
    {
        PcToUsb = 0,
        UsbToPc = 1,
        bidirectional = 2
    }
}