using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace CsvExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            Application application = new Application();
            Workbook workBook = application.Workbooks.Open(Filename: @"D:\etg2_product.xlsx");   
            Worksheet workSheet = workBook.Worksheets.Item[1];

            Range range = workSheet.Range["A3", Type.Missing];
            range.EntireRow.Delete();
            //workBook.SaveAs(@"D:\etg2_product.csv", XlFileFormat.xlCSVWindows);


            workBook.SaveAs(@"D:\etg2_product.csv", FileFormat:Microsoft.Office.Interop.Excel.XlFileFormat.xlCSV);

            //workBook.SaveAs(Filename: @"D:\etg2_product.csv", FileFormat: Microsoft.Office.Interop.Excel.XlFileFormat.xlCSVWindows);
            workBook.Close(false, Type.Missing, Type.Missing);
            application.Quit();
            Console.WriteLine("Save");
            Console.ReadLine();
        }
    }
}
