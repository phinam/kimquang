using Datalayer.DataObject.Core;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.IO;

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
        /// <summary>
        /// Doc file excel chi tiet dua vao database
        /// </summary>
        /// <param name="clientKey"></param>
        /// <param name="excelfile"></param>
        /// <returns></returns>
        public string ImportExcel(string clientKey,string excelfile)
        {
            try
            {
                int startDataRow = 11;
                CMixExcel mixExcel = new CMixExcel(excelfile, false);
                ExcelPackage pck = (ExcelPackage)mixExcel.ExcelMixCore;
                var worksheet = pck.Workbook.Worksheets[1];
                bool isAllOk = true;
                for (int row = startDataRow; row <= worksheet.Dimension.End.Row; row++)
                {
                    string inputValue = GetRowInXml(worksheet, row);
                    if (string.IsNullOrEmpty(inputValue))
                    {
                        continue;
                    }
                    inputValue = "<RequestParams Sys_ViewID=\"28\" Action=\"INSERT\" " + inputValue + "/>";
                    if (ExecuteImport(clientKey, inputValue))
                    {
                        //Delete imported row
                        worksheet.DeleteRow(row);
                        row--;
                    }
                    else
                    {
                        isAllOk = false;
                    }
                }

                string downloadDir = System.Configuration.ConfigurationManager.AppSettings["DownloadDirectory"];
                string tempFile = downloadDir + "\\Product\\Error";// + Guid.NewGuid().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                if(!Directory.Exists(tempFile))
                {
                    Directory.CreateDirectory(tempFile);
                }
                tempFile = tempFile+"/" + Guid.NewGuid().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                //Khi xong thi luu file con lai vao temp
                pck.SaveAs(new System.IO.FileInfo(tempFile));
                mixExcel.CloseStream();

                if (isAllOk)
                {
                    return "00-OK";
                }
                else
                {
                    return "01-" + tempFile;
                }
            }
            catch(Exception ex)
            {
                return "02-" + ex.Message;
            }
        }

        /// <summary>
        /// chuyen du lieu 1 row thanh xml
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="row"></param>
        /// <returns></returns>
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
                    //neu la so thi lay 3 so le
                    if(value is float) value = ((float)value).ToString("#.###");
                    if(value is double ) value = ((double)value).ToString("#.###");
                    if(value is decimal) value = ((decimal)value).ToString("#.###");

                    s += string.Format("{0}=\"{1}\" ", col, XmlEscape(value.ToString()));
                }
                
            }
            return s;
        }
        /// <summary>
        /// Escape file special char in xml: &,",',<,>,
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
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
        private bool ExecuteImport(string ClientKey, string inputValue)
        {
            try
            {
                System.Data.DataSet ds = new CCoreDao().ExecuteAction(ClientKey, inputValue);
                if (ds == null || ds.Tables.Count < 2) return false;
                if (ds.Tables[1].Rows.Count == 0) return false;
                if (!ds.Tables[1].Columns.Contains("Result")) return false;
                var result = ds.Tables[1].Rows[0]["Result"];
                if (result == null || result.ToString() == "0") return false;
                return true;
            }
            catch
            {

            }
            return false;
        }
    }
}