using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Service.Data.Core.Class
{
    /// <summary>
    /// Export Excel to PDF using Microsoft.Office.Interop.Excel 12.0.0.0
    /// Have No free library export excel to pdf other
    /// Use Office 2007 or 2010
    /// Office 2007: 
    ///   Install The 2007 Microsoft Office Suite Service Pack 3 (SP3)
    ///     https://www.microsoft.com/en-in/download/details.aspx?id=27838
    ///     
    ///   Install Plugin Save PDF and XPS from Microsoft office site
    ///     https://www.microsoft.com/en-us/download/details.aspx?id=7
    /// </summary>
    public class CExcelToPDF
    {
        public static bool ExportWorkbookToPdf(string workbookPath, string outputPath)
        {
            // If either required string is null or empty, stop and bail out
            if (string.IsNullOrEmpty(workbookPath) || string.IsNullOrEmpty(outputPath))
            {
                return false;
            }

            // Create COM Objects
            Microsoft.Office.Interop.Excel.Application excelApplication;
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook;

            // Create new instance of Excel
            excelApplication = new Microsoft.Office.Interop.Excel.Application();

            // Make the process invisible to the user
            excelApplication.ScreenUpdating = false;

            // Make the process silent
            excelApplication.DisplayAlerts = false;

            // Open the workbook that you wish to export to PDF
            excelWorkbook = excelApplication.Workbooks.Open(workbookPath);

            // If the workbook failed to open, stop, clean up, and bail out
            if (excelWorkbook == null)
            {
                excelApplication.Quit();

                excelApplication = null;
                excelWorkbook = null;

                return false;
            }

            var exportSuccessful = true;
            try
            {
               
                // Call Excel's native export function (valid in Office 2007 and Office 2010, AFAIK)
                excelWorkbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, outputPath);
            }
            catch (System.Exception ex)
            {
                // Mark the export as failed for the return value...
                exportSuccessful = false;

                // Do something with any exceptions here, if you wish...
                // MessageBox.Show...        
            }
            finally
            {
                // Close the workbook, quit the Excel, and clean up regardless of the results...
                excelWorkbook.Close();
                excelApplication.Quit();

                excelApplication = null;
                excelWorkbook = null;
            }

           
            return exportSuccessful;
        }
    }
}