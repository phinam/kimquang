using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.IO;
using System.Data;

namespace PMSA.iMarkets.Service.Core.Class
{
    public  class CExcelUtil
    {
        string pathTemp = string.Empty;

        public static string ReadExcelOnline(string xmlInput)
        {
            string result = string.Empty;
            CExcelCSVOutput output = new CExcelCSVOutput();
            try
            {
                if (string.IsNullOrEmpty(xmlInput) || xmlInput.IndexOf(" FileName=\"") <= 0)
                {
                    result = output.CreateOutput("0", "1", "Error", "Input not have tag <FileName>", null);
                    return result;
                }
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(xmlInput);
                if (xDoc.FirstChild != null && xDoc.FirstChild.Name == "InputValue")
                {
                    XmlNode xNode = xDoc.FirstChild;
                    string fileName = xNode.Attributes["FileName"].Value;
                    string content64 = xNode.Attributes["FileContent"].Value;
                    string isReplaceSpecialString = "1";
                    if(xNode.Attributes["isReplaceSpecialString"] != null)
                        isReplaceSpecialString = xNode.Attributes["isReplaceSpecialString"].Value;
                    string tempPath = System.Configuration.ConfigurationManager.AppSettings["iMarkets.Service.Core.ExcelService.TempPath"];
                    if (!Directory.Exists(tempPath))
                        Directory.CreateDirectory(tempPath);
                    byte[] content = Convert.FromBase64String(content64);
                    string extand = fileName.Substring(fileName.LastIndexOf('.'));
                    fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
                    string fullFilePath = String.Format("{0}\\{1}{2:ddMMyyyy_hhmmssfff}{3}", tempPath, fileName, DateTime.Now, extand);
                    FileStream fstream = File.Open(fullFilePath, FileMode.OpenOrCreate);
                    fstream.Write(content, 0, content.Length);
                    fstream.Flush();
                    fstream.Close();
                    //File.WriteAllBytes(fullFilePath, content);
                    if (isReplaceSpecialString == "0")
                        result = BussinessWork(fullFilePath, false);
                    else
                        result = BussinessWork(fullFilePath);
                }
                else
                    result = output.CreateOutput("0", "1", "Error", "Error.Invalid Input!!!", null);
            }
            catch (Exception ex)
            {
                result = output.CreateOutput("0", "1", "ReadExcelOnlineError", ex.Message + ex.StackTrace, null);
            }

            return result;
        }

        public static string MakeExcelOnline(string xmlInput)
        {
            string result = string.Empty;
            CExcelCSVOutput output = new CExcelCSVOutput();
            try
            {
                if (string.IsNullOrEmpty(xmlInput) || xmlInput.IndexOf(" FileName=\"") <= 0)
                {
                    result = output.CreateOutput("0", "1", "Error", "Input not have tag <FileName>", null);
                    return result;
                }
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(xmlInput);
                if (xDoc.FirstChild != null && xDoc.FirstChild.Name == "InputValue")
                {
                    XmlNode xNode = xDoc.FirstChild;
                    string fileName = xNode.Attributes["FileName"].Value;
                    string content64 = xNode.Attributes["FileStructContent"].Value;
                    string contentCSV = xNode.Attributes["FileCSVContent"].Value;
                    try
                    {
                        contentCSV = System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(contentCSV));
                    }
                    catch { }
                    string tempPath = System.Configuration.ConfigurationManager.AppSettings["iMarkets.Service.Core.ExcelService.TempPath"];
                    byte[] content = Convert.FromBase64String(content64);
                    File.WriteAllBytes(tempPath + "\\" + fileName, content);
                    DataSet ds = ConvertCVSToDataSet(contentCSV);
                    result = BussinessWork2(tempPath + "\\" + fileName, ds);
                }
                else
                    result = output.CreateOutput("0", "1", "MakeExcelOnlineError", "Error.Invalid Input!!!", null);
            }
            catch (Exception ex)
            {
                result = output.CreateOutput("0", "1", "MakeExcelOnlineError", ex.Message, null);
            }

            return result;
        }

        public static string MakeExcelOnlineEx(string xmlInput)
        {
            string result = string.Empty;
            CExcelCSVOutput output = new CExcelCSVOutput();
            try
            {
                if (string.IsNullOrEmpty(xmlInput) || xmlInput.IndexOf(" FileName=\"") <= 0)
                {
                    result = output.CreateOutput("0", "1", "Error", "Input not have tag <FileName>", null);
                    return result;
                }
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(xmlInput);
                if (xDoc.FirstChild != null && xDoc.FirstChild.Name == "InputValue")
                {
                    XmlNode xNode = xDoc.FirstChild;
                    string fileName = xNode.Attributes["FileName"].Value;
                    string content64 = xNode.Attributes["FileStructContent"].Value;
                    string contentCSV = xNode.Attributes["FileCSVContent"].Value;
                    try
                    {
                        contentCSV = System.Text.ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(contentCSV));
                    }
                    catch { }
                    string tempPath = HttpRuntime.AppDomainAppPath + "_Template\\Export";//System.Configuration.ConfigurationManager.AppSettings["iMarkets.Service.Core.ExcelService.TempPath"];
                    byte[] content = Convert.FromBase64String(content64);
                    File.WriteAllBytes(tempPath + "\\" + fileName, content);
                    DataSet ds = ConvertCVSToDataSet(contentCSV);
                    result = BussinessWork2Ex(tempPath + "\\" + fileName, ds);
                }
                else
                    result = output.CreateOutput("0", "1", "MakeExcelOnlineExError", "Error.Invalid Input!!!", null);
            }
            catch (Exception ex)
            {
                result = output.CreateOutput("0", "1", "MakeExcelOnlineExError", ex.Message + ex.StackTrace, null);
            }

            return result;
        }
        #region private method
        
        private static string BussinessWork(string pathFile,bool isReplaceSpecialString = true)
        {
            CExcelCSVOutput output = new CExcelCSVOutput();
            string result = string.Empty;
            try
            {
                //FileInfo newFile = new FileInfo(pathFile);
                CMixExcel mixExcel = new CMixExcel(pathFile,false);
                result = output.CreateOutput("0", "0", "Success", "Success", mixExcel, isReplaceSpecialString);
                mixExcel.CloseStream();
                File.Delete(pathFile);
            }
            catch (Exception ex)
            {
                result = output.CreateOutput("0", "1", " BussinessWork Error", ex.Message + ex.StackTrace, null);
            }
            return result;
        }

        //FileCSVContent="1;2;3\na;b;c\n###\n4;5;6\nd;e;f"

        /// <summary>
        /// ID;Code;Name;Phone;BirthDate\n1;"KH01";"Tuong";"0989876666";"2013-12-01\n2;"KH02";"Nam";"0102001111";"2013-12-20"
        /// \n###\nID;Code;Name;Phone;BirthDate\n1;"KH01";"Tuong";"0989876666";"2013-12-01\n2;"KH02";"Nam";"0102001111";"2013-12-20"
        /// </summary>
        /// <param name="csvContent"></param>
        /// <returns></returns>
        public static DataSet ConvertCVSToDataSet(string csvContent)
        {
            DataSet ds = new DataSet();
            string[] detailsSheetsData = csvContent.Split(new string[] { "\n###\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < detailsSheetsData.Length; i++)
            {
                DataTable dt = new DataTable("Sheet " + (i + 1));
                if (!string.IsNullOrEmpty(detailsSheetsData[i]))
                {
                    string[] detailsRowData = detailsSheetsData[i].Split(new string[] { "\n","\r" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int r = 0; r < detailsRowData.Length; r++)
                    {
                        if (!string.IsNullOrEmpty(detailsRowData[r]))
                        {
                            string[] detailsCellsInRow = detailsRowData[r].Split(new string[] { ";" }, StringSplitOptions.None);
                            if (dt.Columns.Count < detailsCellsInRow.Length)
                            {
                                dt.Columns.Clear();
                                for (int c = 0; c < detailsCellsInRow.Length; c++)
                                {
                                    DataColumn dc = new DataColumn("Column " + (c + 1));
                                    dt.Columns.Add(dc);
                                }
                            }
                            DataRow dr = dt.NewRow();
                            for (int c = 0; c < detailsCellsInRow.Length; c++)
                            {
                                object value = detailsCellsInRow[c];
                                if (value != null)
                                    dr[c] = value.ToString().Replace(CExcelCSVOutput.REPLACEDELEMETER, CExcelCSVOutput.CSVCELLDELEMETER);
                                else
                                    dr[c] = value;
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
                ds.Tables.Add(dt);
            }
            return ds;
        }

        private static string BussinessWork2(string pathFile, DataSet contentExcel)
        {
            CExcelCSVOutput output = new CExcelCSVOutput();
            string result = string.Empty;
            try
            {
                CMixExcel mixExcel = new CMixExcel(pathFile);
                result = output.CreateOutput2(mixExcel, contentExcel);
                mixExcel.CloseStream();
                File.Delete(pathFile);
            }
            catch (Exception ex)
            {
                result = output.CreateOutput("0", "1", "Error", ex.Message, null);
            }
            return result;
        }


        private static string BussinessWork2Ex(string pathFile, DataSet contentExcel)
        {
            CExcelCSVOutput output = new CExcelCSVOutput();
            string result = string.Empty;
            try
            {
                CMixExcel mixExcel = new CMixExcel(pathFile, true);
                result = output.CreateOutput2Ex(mixExcel, contentExcel);
                mixExcel.CloseStream();
                //File.Delete(pathFile);
            }
            catch (Exception ex)
            {
                result = output.CreateOutput("0", "1", "Error BussinessWork2Ex", ex.Message + ex.StackTrace, null);
            }
            return result;
        }
        #endregion
    }
}