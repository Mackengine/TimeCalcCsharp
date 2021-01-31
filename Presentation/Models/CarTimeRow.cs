using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Models
{
    public class CarTimeRow
    {
        public String CarNumber { get; set; }
        public String StartTime { get; set; }
        public String EndTime { get; set; }
        public String CalculatedTime { get; set; }
        public String Status { get; set; }
    }
}
