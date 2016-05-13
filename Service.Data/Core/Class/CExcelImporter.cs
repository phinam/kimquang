using Datalayer.DataObject.Core;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Service.Data.Core.Class
{
    public class CExcelImporter
    {
        string[] colNames = new string[] 
        {
            "LastUpdatedDateTime",
            "PriorityLevel",
            "Name",
            "AvailableAreaNet",
            "AvailableAreaGross",
            "AvailableFloor",
            "AvailableFrom",
            "State",
            "FloorArea",
            "PriceDescription",
            "HirePrice",
            "HireManagermentFee",
            "TotalPrice",
            "HireTotalAmount",
            "HireFinalPrice",
            "HomeNumber",
            "StreetName",
            "District",
            "Ward",
            "ContactPhone",
            "ContactMgntPhone",
            "ContactMgntEmail",
            "ContactOwnerPhone",
            "ContactOwnerEmail",
            "PackingBikeCount",
            "PackingBikeFee",
            "PackingCarCount",
            "PackingCarFee",
            "ElectricityFee",
            "ServiceWaterFee",
            "OtherFee",
            "Struture",
            "CeilingHeight",
            "CompleteTime",
            "OwnerName",
            "OwnerTaxNo",
            "TransferStock",
            "BuildingDirection",
            "OfficeDirection",
            "PayByDeposit",
            "PayByCredit",
            "Bonus",
            "OfficeRank",
            "BuildingPosition",
            "Description"

        };
        public void ImportExcel(string clientKey,string excelfile)
        {
            int startDataRow = 11;
            CMixExcel mixExcel = new CMixExcel(excelfile, false);
            ExcelPackage pck = (ExcelPackage)mixExcel.ExcelMixCore;
            var worksheet = pck.Workbook.Worksheets[1];
            for(int row = startDataRow;row <= worksheet.Dimension.End.Row;row ++)
            {
                string inputValue = GetRowInXml(worksheet, row);
                if(string.IsNullOrEmpty(inputValue))
                {
                    continue;
                }
                inputValue = "<RequestParams Sys_ViewID=\"28\" Action=\"INSERT\" " + inputValue + "/>";
                ExecuteImport(clientKey, inputValue);
            }
          
        }

        private string GetRowInXml(ExcelWorksheet ws,int row)
        {
            string s = "";
            for(int i=1;i<=ws.Dimension.End.Column;i++)
            {
                object value = ws.Cells[row, i].Value;
                string col = "C" + i;
                if (colNames.Length >= i) col = colNames[i-1];
                if(value !=null)
                {
                    if(col.Equals("AvailableFrom"))
                    {
                        DateTime aFrom;
                        try
                        {
                            aFrom = DateTime.ParseExact(value.ToString(), "dd/MM/yyyy", new CultureInfo("vi-VN"));
                            value = aFrom.ToString("yyyy-MM-dd");

                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("Parse Date Error:" + e.Message);
                        }
                        
                    }
                    s += string.Format("{0}=\"{1}\" ", col, XmlEscape(value.ToString()));
                }
                
            }
            return s;
        }
        private string XmlEscape(string s)
        {
            /*
             * "   &quot;
            '   &apos;
            <   &lt;
            >   &gt;
            &   &amp;
             * */
            s = s.Replace("&", "&amp;");
            s = s.Replace("\"", "&quot;");
            s = s.Replace("'", "&apos;");
            s = s.Replace("<", "&lt;");
            s = s.Replace(">", "&gt;");
            return s;
        }
        private void ExecuteImport(string ClientKey,string inputValue)
        {
            new CCoreDao().ExecuteAction(ClientKey, inputValue);
        }
    }
}