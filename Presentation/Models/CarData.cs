using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Models
{
    public class CarData
    {
        public List<CarTimeRow> data { get; set; }
        public SerialPort usb { get; set; }
        public String current { get; set; }
    }
}
