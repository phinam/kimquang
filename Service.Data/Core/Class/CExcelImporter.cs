﻿using Datalayer.DataObject.Core;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.IO;
using PMSA.Framework.Log;

namespace Service.Data.Core.Class
{
    
    public class CExcelImporter
    {
        private HashSet<String> importedKey = new HashSet<String>();
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
        int[] keyCols = new int[] { 3, 6 };// "Name", "AvailableFloor" 
        int[] mergedCols = new int[] { 9 };
        const string MERGE_COL_SEPARATOR = " ";
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
                CLogManager.WriteSL("IMPORT", "File:" + excelfile);
                int startDataRow = 11;
                CMixExcel mixExcel = new CMixExcel(excelfile, false);
                ExcelPackage pck = (ExcelPackage)mixExcel.ExcelMixCore;
                var worksheet = pck.Workbook.Worksheets[1];
                bool isAllOk = true;
                CLogManager.WriteSL("IMPORT", "Begin For to :" + worksheet.Dimension.End.Row.ToString());
                for (int row = startDataRow; row <= worksheet.Dimension.End.Row; row++)
                {

                    string inputValue = GetRowInXml(worksheet, row);
                    CLogManager.WriteSL("IMPORT", "Input :" + inputValue);
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
                string fileName = Guid.NewGuid().ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                tempFile = tempFile+"/" + fileName;
                //Khi xong thi luu file con lai vao temp
                pck.SaveAs(new System.IO.FileInfo(tempFile));
                mixExcel.CloseStream();

                if (isAllOk)
                {
                    return "Code\n00-OK";
                }
                else
                {
                    return "Code\n01-//Product//Error//" + fileName;
                }
            }
            catch(Exception ex)
            {
                CLogManager.WriteSL("IMPORT", ex.ToString());
                return "Code\n02-" + ex.Message;
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
            string key = GetImportKey(ws, row);
            //if imported
            if (string.IsNullOrEmpty(key) || importedKey.Contains(key)) return "";
            importedKey.Add(key);

            string s = "";
            for(int i=1;i<=ws.Dimension.End.Column;i++)
            {
                object value = ws.Cells[row, i].Value;
                string col = "C" + i;
                if (colNames.Length >= i) col = colNames[i-1];
                if (mergedCols.Contains(i))
                {
                    value = GetMergeColValue(ws, i, row);
                }
                if (value !=null)
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

        private string GetMergeColValue(ExcelWorksheet ws,int col, int row)
        {
            string result = "";
            string key = GetImportKey(ws, row);

            for (int i = row; i <= ws.Dimension.End.Row; i++)
            {
                string keyi = GetImportKey(ws, i);
                if (key == keyi)
                {
                    object value = ws.Cells[i, col].Value;
                    if (value == null) continue;
                    if (string.IsNullOrEmpty(value.ToString())) continue;
                    if (!string.IsNullOrEmpty(result)) result += MERGE_COL_SEPARATOR;
                    result += value.ToString();
                }
            }
            return result;
        }
        private string GetImportKey(ExcelWorksheet ws, int row)
        {
            string key = "";
            for(int i=0;i<keyCols.Length;i++)
            {
                object value = ws.Cells[row, keyCols[i]].Value;
                if (value == null) return "";
                if(!string.IsNullOrEmpty(key))
                {
                    key += "##";
                }
                key += value.ToString();
            }

            return key;
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
                if (!ds.Tables[0].Columns.Contains("Result")) return false;
                var result = ds.Tables[0].Rows[0]["Result"];
                if (result == null || result.ToString() == "0") return false;

                return true;
            }
            catch(Exception ex)
            {
                CLogManager.WritePL("ExecuteImport", ex.ToString());
            }
            return false;
        }
    }
}