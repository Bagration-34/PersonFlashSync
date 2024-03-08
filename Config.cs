

using System.Collections.Generic;

namespace PersonFlashSync
{
    class Config
    {
        public List<Device> Devices { get; set; }
    }

    class Device
    {
        public string VolumeLabel { get; set; }
        public string dirOnPC { get; set; }
        public string dirOnUsb { get; set; }
    }
}