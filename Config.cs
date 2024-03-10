

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
        public Directions Direction { get; set; }
        public DirectoryPair[] DirectoryPairs { get; set; }
    }

    struct DirectoryPair
    {
        public string dirOnPC { get; set; }
        public string dirOnUsb { get; set; }
    }

    enum Directions 
    {
        PcToUsb = 0,
        UsbToPc = 1,
        bidirectional = 2
    }
}