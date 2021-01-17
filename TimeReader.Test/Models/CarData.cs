using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeReader.Test.Models
{
    public class CarData
    {
        public bool run { get; set; }
        public List<String> serialMessages { get; set; }
        public List<CarTime> data { get; set; }
        public SerialPort usb { get; set; }
    }
}
