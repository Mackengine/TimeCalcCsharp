using OfficeOpenXml;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Helpes
{
    public class ExcelAuditor
    {
        public ExcelAuditor() { }
        public bool WriteToDisk(IEnumerable<CarTimeRow> rows)
        {
            ExcelPackage package = new ExcelPackage();
            ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Data");
            int row = 1;
            sheet.Cells[row, 1].Value = "Car Number";
            sheet.Cells[row, 2].Value = "Start Time";
            sheet.Cells[row, 3].Value = "End Time";
            sheet.Cells[row, 4].Value = "Calculated Time";
            sheet.Cells[row, 5].Value = "Status";
            ++row;

            foreach (CarTimeRow t in rows)
            {
                sheet.Cells[row, 1].Value = t.CarNumber;
                sheet.Cells[row, 2].Value = t.StartTime;
                sheet.Cells[row, 3].Value = t.EndTime;
                sheet.Cells[row, 4].Value = t.CalculatedTime;
                sheet.Cells[row, 5].Value = t.Status;
                ++row;
            }

            package.SaveAs(new System.IO.FileInfo(System.IO.Directory.GetCurrentDirectory() + "\\Output\\dump.xlsx"));

            return true;
        }
    }
}
