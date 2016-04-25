using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Text;
using System.Reflection;
using System.IO;
using OfficeOpenXml;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Xml;
using System.Data;

namespace PMSA.iMarkets.Service.Core.Class
{
    public class CExcelUtilEx : CExcelUtil
    {
        public static string ReadExcelOnlineSub(string xmlInput)
        {
            string resultFromCore = ReadExcelOnline(xmlInput);
            if (!string.IsNullOrEmpty(resultFromCore))
            {
                CFormatForExcelUtilEx formatForExcelUtilEx = new CFormatForExcelUtilEx(resultFromCore);
                if (formatForExcelUtilEx.LPartData.Count >= 3)
                    resultFromCore = String.Format("{{\"isSuccess\":true,\"data\":\"{0}\"}}", formatForExcelUtilEx.ToCSVOutput());
                else
                    resultFromCore = string.Format("{{\"isSuccess\":false,\"error\":\"{0}\"}}", formatForExcelUtilEx.Message);
            }
            return resultFromCore;
        }

        public static string MakeExcelOnlineSub(string xmlInput, DataTable dt)
        {
            string result = string.Empty;
            CExcelCSVOutput output = new CExcelCSVOutput();
            try
            {
                //if (string.IsNullOrEmpty(xmlInput) || xmlInput.IndexOf(" FileName=\"") <= 0)
                //{
                //    result = output.CreateOutput("0", "1", "Error", "Input not have tag <FileName>", null);
                //    return result;
                //}
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(xmlInput);
                if (xDoc.FirstChild != null && xDoc.FirstChild.Name == "InputValue")
                {
                    XmlNode xNode = xDoc.FirstChild;
                    //string fileName = xNode.Attributes["FileName"].Value;
                    //string content64 = xNode.Attributes["FileStructContent"].Value;
                    string fileName = String.Format("Template_InterfaceOrder_{0:ddMMyyyy_hhmmss}.xlsx", DateTime.Now);
                    string samplePath = HttpRuntime.AppDomainAppPath + "_Template\\Excel";
                    byte[] bSampleFileContent = File.ReadAllBytes(samplePath + "\\Template_InterfaceOrder.xlsx");
                    string content64 = Convert.ToBase64String(bSampleFileContent);
                    string contentJSon = "";//xNode.Attributes["FileJSonData"].Value;
                    result = MakeInputFromJSonAndExcelFile(fileName, content64, contentJSon, dt);
                }
                else
                    result = output.CreateOutput("0", "1", "Error", "Error.Invalid Input!!!", null);
            }
            catch (Exception ex)
            {
                result = output.CreateOutput("0", "1", "Error MakeExcelOnlineSub", ex.Message + ex.StackTrace, null);
            }

            return result;
        }


        private static string MakeInputFromJSonAndExcelFile(string fileName, string content64, string sJSonDatabase64, DataTable dt)
        {
            StringBuilder sBuilder = new StringBuilder();
            var jsonSerializer = new JavaScriptSerializer();
            byte[] bJSonData = Convert.FromBase64String(sJSonDatabase64);
            string RawJsonData = ASCIIEncoding.ASCII.GetString(bJSonData);
            PMSA.Framework.Log.CLogManager.WriteSL("JsonRawInput", RawJsonData);
            var objtest = jsonSerializer.Deserialize<CTradeInterfaceList>(RawJsonData);
            //   if(objtest is CTradeInterfaceList)
            //    {
            CTradeInterfaceList oCTradeInterfaceList = (CTradeInterfaceList)objtest;
            //string CSVContent = "ID;Code;Name;Phone;BirthDate\n1;'KH01';'Tuong';'0989876666';'2013-12-01'\n2;'KH02';'Nam';'0102001111';'2013-12-20'" +
            //                              "\n3;'KH03';'Thien';'0102001111';'2013-12-24'" +
            //                              "\n###\nID;Code;Name;Phone;BirthDate\n1;'KH01';'Tuong';'0989876666';'2013-12-01'\n2;'KH02';'Nam';'0102001111';'2013-12-20'";
            int sheetsNumber = 1;
            string fileContent = CreateExcelSampleContent(fileName, content64, ref sheetsNumber);
            List<CTradeInterface>[] lTemp = new List<CTradeInterface>[1];
            lTemp[0] = new List<CTradeInterface>();
            //lTemp[1] = new List<CTradeInterface>();
            //lTemp[2] = new List<CTradeInterface>();
            /*   foreach (var item in oCTradeInterfaceList.ListTrade)
               {
                   if (item.IsFixedIncome == "1")
                       lTemp[2].Add(item);
               }

               foreach (var item in oCTradeInterfaceList.ListTrade)
               {
                   if (item.IsPreferredShare == "1")
                       lTemp[1].Add(item);
               }

               foreach (var item in oCTradeInterfaceList.ListTrade)
               {
                   if (item.IsFixedIncome != "1" && item.IsPreferredShare != "1")
                       lTemp[0].Add(item);
               }
                */
            int i = 1;
            foreach (DataRow r in dt.Rows)
            {
                CTradeInterface item = new CTradeInterface();
                item.ID = i++;
                item.Address = r["Address"] != null ? r["Address"].ToString() : "";
                item.Name = r["Name"] != null ? r["Name"].ToString() : "";
                item.AvailableArea = r["AvailableArea"] != null ? r["AvailableArea"].ToString() : "";
                item.PriceDescription = r["PriceDescription"] != null ? r["PriceDescription"].ToString() : "";
                item.VATTax = r["VATTax"] != null ? r["VATTax"].ToString() : "";
                item.ServiceFee = r["ServiceFee"] != null ? r["ServiceFee"].ToString() : "";
                item.Description = r["Description"] != null ? r["Description"].ToString() : "";

                lTemp[0].Add(item);
            }
            //ltemp[1].add(item);
            // ltemp[2].add(item);
            string JSONContent = FormatCSV(lTemp, sheetsNumber);
            JSONContent = Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(JSONContent));
            string inputXML = string.Format("<InputValue FileName=\"{0}\" FileStructContent=\"{1}\" FileCSVContent=\"{2}\"/>", fileName, fileContent, JSONContent);

            sBuilder.Append(MakeExcelOnlineEx(inputXML));
            // }

            return sBuilder.ToString();
        }

        private static string FormatCSV(List<CTradeInterface>[] alCTradeInterface, int sheetsNumber = 1)
        {
            StringBuilder sBuilder = new StringBuilder();
            if (alCTradeInterface != null && alCTradeInterface.Length > 0)
            {
                for (int i = 0; i < sheetsNumber; i++)
                {
                    string strProtertiesName = string.Empty;
                    List<string> lpInfo = new List<string>();

                    //properties
                    foreach (PropertyInfo pInfo in typeof(CTradeInterface).GetProperties())
                    {
                        strProtertiesName += pInfo.Name.Replace(CExcelCSVOutput.CSVCELLDELEMETER, CExcelCSVOutput.REPLACEDELEMETER) + ";";
                        lpInfo.Add(pInfo.Name);
                    }
                    if (strProtertiesName.EndsWith(";"))
                        sBuilder.Append(strProtertiesName);
                    sBuilder.Append("\n");

                    //content

                    foreach (var item in alCTradeInterface[i])
                    {
                        string aRow = string.Empty;
                        foreach (var propertyName in lpInfo)
                        {
                            object value = item.GetType().GetProperty(propertyName).GetValue(item, null);
                            if (value is string)
                                aRow += String.Format("'{0}'", item.GetType().GetProperty(propertyName).GetValue(item, null)).Replace(CExcelCSVOutput.CSVCELLDELEMETER, CExcelCSVOutput.REPLACEDELEMETER);
                            else
                                aRow += item.GetType().GetProperty(propertyName).GetValue(item, null);
                            aRow += ";";
                        }
                        if (aRow != string.Empty)
                            sBuilder.Append(aRow + "\n");
                    }
                    //xong 1 sheet
                    if (i < sheetsNumber - 1)
                        sBuilder.Append("\n###\n");
                }
            }
            return sBuilder.ToString();
        }

        private static string CreateExcelSampleContent(string fileName, string content64, ref int sheetsNumber)
        {
            string tempPath = HttpRuntime.AppDomainAppPath + "_Template\\Export";// System.Configuration.ConfigurationManager.AppSettings["iMarkets.Service.Core.ExcelService.TempPath"];
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            byte[] content = Convert.FromBase64String(content64);
            File.WriteAllBytes(String.Format("{0}\\{1}", tempPath, fileName), content);
            CMixExcel mixExcel = new CMixExcel(String.Format("{0}\\{1}", tempPath, fileName));
            sheetsNumber = GetSheetNumber(mixExcel);
            mixExcel.CloseStream();
            return content64;
        }

        private static string MakeFileExcelSample(CMixExcel mixExcel, ref int sheetsNumber)
        {
            string result = string.Empty;
            Stream _stream = null;
            string fileNameTail = mixExcel.PathFile.Substring(mixExcel.PathFile.LastIndexOf('.'));
            string samplefileName = mixExcel.PathFile.Substring(0, mixExcel.PathFile.LastIndexOf('.'));
            string pathsamplefileName = String.Format("{0}_Sample{1}", samplefileName, fileNameTail);
            FileStream fstream = File.Open(pathsamplefileName, FileMode.OpenOrCreate);
            fstream.Close();
            File.Copy(mixExcel.PathFile, pathsamplefileName, true);
            CMixExcel mixExcelSample = new CMixExcel(pathsamplefileName);
            if (mixExcel != null && mixExcel.ExcelMixCore is ExcelPackage)
            {
                #region Excel 2007++
                ExcelPackage excelSamplePackage = (ExcelPackage)mixExcelSample.ExcelMixCore;
                ExcelPackage excelPackage = (ExcelPackage)mixExcel.ExcelMixCore;

                ExcelWorksheets workSheetsSample = excelSamplePackage.Workbook.Worksheets;
                ExcelWorksheets workSheets = excelPackage.Workbook.Worksheets;
                sheetsNumber = workSheetsSample.Count;
                for (int i = 1; i <= workSheetsSample.Count; i++)
                {
                    var worksheetSample = workSheetsSample[i];
                    var worksheet = workSheets[i];
                    System.Xml.XmlNodeList test = worksheetSample.WorksheetXml.GetElementsByTagName("dimension");
                    if (test != null && test.Count > 0)
                    {
                        System.Xml.XmlNode value = test[0];
                        //<dimension ref="A1:J17" />
                        string outerXml = value.OuterXml.Substring("<dimension ref=\"".Length);
                        string rangesize = outerXml.Substring(0, outerXml.IndexOf('"'));
                        if (rangesize != "A1")
                        {
                            string[] arrayRange = rangesize.Split(':');
                            int indextemp = 0; int temp;
                            string MinRow = string.Empty, MaxRow = string.Empty;
                            int MinColumn = 0, MaxColumn = 0;
                            string range1 = arrayRange[0];
                            while (!int.TryParse(range1[indextemp].ToString(), out temp) && indextemp < range1.Length)
                            {
                                MinColumn += (System.Text.Encoding.ASCII.GetBytes(range1[indextemp].ToString())[0] - 64);
                                indextemp++;
                            }
                            MinRow = range1.Substring(indextemp);

                            indextemp = 0;
                            string range2 = arrayRange[1];
                            while (!int.TryParse(range2[indextemp].ToString(), out temp) && indextemp < range2.Length)
                            {
                                MaxColumn += (System.Text.Encoding.ASCII.GetBytes(range2[indextemp].ToString())[0] - 64);
                                indextemp++;
                            }
                            MaxRow = range2.Substring(indextemp);
                            if (!string.IsNullOrEmpty(MinRow) && MinColumn > 0 && !string.IsNullOrEmpty(MaxRow) && MaxColumn > 0)
                            {
                                int intMinRow = int.Parse(MinRow);
                                int intMaxRow = int.Parse(MaxRow);
                                for (int r = intMinRow; r <= intMaxRow; r++)
                                {
                                    for (int c = MinColumn; c <= MaxColumn + 1; c++)
                                    {
                                        if (c > MinColumn)
                                            worksheetSample.Cells[r, c].Value = worksheet.Cells[r, c - 1].Value;
                                        else if (r == intMinRow)
                                            worksheetSample.Cells[r, c].Value = "Heading";
                                        else if (r == intMinRow + 1)
                                            worksheetSample.Cells[r, c].Value = "Schema";
                                        else
                                            worksheetSample.Cells[r, c].Value = string.Empty;
                                    }
                                }
                            }
                        }//(rangesize != "A1")
                    }
                }

                excelSamplePackage.Save();
                _stream = excelSamplePackage.Stream;
                #endregion
            }
            else if (mixExcel != null && mixExcel.ExcelMixCore is HSSFWorkbook)
            {
                #region Excel 2003--
                HSSFWorkbook hssWorkbookSample = (HSSFWorkbook)mixExcelSample.ExcelMixCore;
                HSSFWorkbook hssWorkbook = (HSSFWorkbook)mixExcel.ExcelMixCore;
                sheetsNumber = hssWorkbookSample.NumberOfSheets;
                for (int i = 0; i < hssWorkbookSample.NumberOfSheets; i++)
                {
                    NPOI.HSSF.UserModel.HSSFSheet excelSheetSample = (NPOI.HSSF.UserModel.HSSFSheet)hssWorkbookSample.GetSheetAt(i);
                    NPOI.HSSF.UserModel.HSSFSheet excelSheet = (NPOI.HSSF.UserModel.HSSFSheet)hssWorkbook.GetSheetAt(i);
                    NPOI.HSSF.Record.DimensionsRecord sheetDementionSample = excelSheetSample.Sheet.Dimensions;
                    NPOI.HSSF.Record.DimensionsRecord sheetDemention = excelSheet.Sheet.Dimensions;
                    for (int r = sheetDementionSample.FirstRow; r <= sheetDementionSample.LastRow; r++)
                    {
                        IRow rowSample = excelSheetSample.GetRow(r);
                        if (rowSample != null)
                            excelSheetSample.RemoveRow(rowSample);
                    }

                    for (int r = sheetDemention.FirstRow; r <= sheetDemention.LastRow; r++)
                    {
                        int maxCol = sheetDementionSample.LastCol;
                        int minCol = sheetDementionSample.FirstCol;
                        for (int c = minCol; c <= maxCol + 1; c++)
                        {
                            try
                            {
                                IRow rowSample = excelSheetSample.GetRow(r);
                                IRow row = excelSheet.GetRow(r);
                                if (row != null)
                                {
                                    ICell excelSheetGetRowGetCellSample = rowSample.GetCell(c);
                                    if (excelSheetGetRowGetCellSample == null)
                                        excelSheetGetRowGetCellSample = rowSample.CreateCell(c);
                                    if (c > sheetDementionSample.FirstCol)
                                    {
                                        ICell excelSheetGetRowGetCell = row.GetCell(c - 1);
                                        if (excelSheetGetRowGetCell != null)
                                        {
                                            switch (excelSheetGetRowGetCell.CellType)
                                            {
                                                case CellType.String:
                                                    excelSheetGetRowGetCellSample.SetCellValue(excelSheetGetRowGetCell.StringCellValue);
                                                    break;
                                                case CellType.Numeric:
                                                    excelSheetGetRowGetCellSample.SetCellValue(excelSheetGetRowGetCell.NumericCellValue);
                                                    break;
                                                case CellType.Boolean:
                                                    excelSheetGetRowGetCellSample.SetCellValue(excelSheetGetRowGetCell.BooleanCellValue);
                                                    break;
                                                case CellType.Blank:
                                                    excelSheetGetRowGetCellSample.SetCellValue(string.Empty);
                                                    break;
                                                case CellType.Error:
                                                    excelSheetGetRowGetCellSample.SetCellValue(excelSheetGetRowGetCell.ErrorCellValue);
                                                    break;
                                                case CellType.Formula:
                                                    {
                                                        switch (excelSheetGetRowGetCellSample.CachedFormulaResultType)
                                                        {
                                                            case CellType.Numeric:
                                                                excelSheetGetRowGetCellSample.SetCellValue(excelSheetGetRowGetCell.NumericCellValue);
                                                                break;
                                                            case CellType.String:
                                                                excelSheetGetRowGetCellSample.SetCellValue(excelSheetGetRowGetCell.StringCellValue);
                                                                break;
                                                            case CellType.Boolean:
                                                                excelSheetGetRowGetCellSample.SetCellValue(excelSheetGetRowGetCell.BooleanCellValue);
                                                                break;
                                                            case CellType.Error:
                                                                excelSheetGetRowGetCellSample.SetCellValue(excelSheetGetRowGetCell.ErrorCellValue);
                                                                break;
                                                        }
                                                    }
                                                    break;
                                                case CellType.Unknown:
                                                    excelSheetGetRowGetCellSample.SetCellValue("Unknown");
                                                    break;
                                            }
                                        }
                                    }
                                    else //if (c > sheetDementionSample.FirstCol)
                                    {
                                        if (r == sheetDementionSample.FirstRow)
                                        {
                                            excelSheetGetRowGetCellSample.SetCellValue("Heading");
                                        }
                                        else if (r == sheetDementionSample.FirstRow + 1)
                                        {
                                            excelSheetGetRowGetCellSample.SetCellValue("Schema");
                                        }
                                        else
                                            excelSheetGetRowGetCellSample.SetCellValue(string.Empty);
                                    }
                                }
                            }
                            catch { }

                        }
                    }
                }

                FileStream fs = new FileStream(mixExcelSample.PathFile, FileMode.OpenOrCreate);
                hssWorkbookSample.Write(fs);
                _stream = fs;
                #endregion
            }

            if (_stream != null)
            {
                byte[] binaryData = new byte[_stream.Length];
                long bytesRead = _stream.Read(binaryData, 0, (int)_stream.Length);
                _stream.Close();
                result = Convert.ToBase64String(binaryData);
            }
            mixExcelSample.CloseStream();
            return result;
        }

        private static int GetSheetNumber(CMixExcel mixExcel)
        {
            int sheetsNumber = 0;
            if (mixExcel != null && mixExcel.ExcelMixCore is ExcelPackage)
            {
                ExcelPackage excelPackage = (ExcelPackage)mixExcel.ExcelMixCore;
                ExcelWorksheets workSheets = excelPackage.Workbook.Worksheets;
                sheetsNumber = workSheets.Count;
            }
            else if (mixExcel != null && mixExcel.ExcelMixCore is HSSFWorkbook)
            {
                HSSFWorkbook hssWorkbook = (HSSFWorkbook)mixExcel.ExcelMixCore;
                sheetsNumber = hssWorkbook.NumberOfSheets;
            }

            return sheetsNumber;
        }
    }

    public class CFormatForExcelUtilEx
    {
        public List<string> LPartData = new List<string>();
        public DataSet DSSheets = new DataSet();
        public string Result = "1";
        public string Message = string.Empty;
        public CFormatForExcelUtilEx(string csvContent)
        {
            string[] strResultFromCore = csvContent.Split(new string[] { "\n###\n" }, StringSplitOptions.RemoveEmptyEntries);
            string csvContentSub = string.Empty;
            for (int i = 0; i < strResultFromCore.Length; i++)
            {
                LPartData.Add(strResultFromCore[i]);
                if (i >= 2)
                {
                    csvContentSub += strResultFromCore[i];
                    if (i < strResultFromCore.Length - 1)
                        csvContentSub += "\n###\n";
                }
            }

            if (LPartData.Count >= 3)
            {
                DSSheets = CExcelUtil.ConvertCVSToDataSet(csvContentSub);
            }
            else
            {
                string resultFirst = strResultFromCore[0];
                string[] detailResultFirst = resultFirst.Split(';');
                Result = detailResultFirst[5];
                Message = detailResultFirst[6];
            }
        }

        public string ToCSVOutput()
        {
            StringBuilder sBuilder = new StringBuilder();
            if (DSSheets != null && DSSheets.Tables.Count > 0)
            {
                for (int i = 0; i < DSSheets.Tables.Count; i++)
                {
                    if (DSSheets.Tables[i] != null)
                    {
                        DataTable dt = DSSheets.Tables[i];
                        if (dt.Rows.Count > 3)
                        {
                            for (int r = 1; r < dt.Rows.Count; r++)
                            {
                                if (r != 2 && r != 3)
                                {
                                    for (int c = 1; c < dt.Columns.Count; c++)
                                    {
                                        //format theo yeu cau cua Mr.Nam
                                        if (r == 1)
                                        {
                                            string dataCell = string.Empty;
                                            if (dt.Rows[r][c] != null)
                                            {
                                                dataCell = dt.Rows[r][c].ToString();
                                                switch (dataCell)
                                                {
                                                    case "OrderID":
                                                        sBuilder.Append("ID"); break;
                                                    case "Trs. Code":
                                                    case "Side":
                                                        sBuilder.Append("SideString"); break;
                                                    case "Order":
                                                    case "OrderType":
                                                        sBuilder.Append("OrderTypeString"); break;
                                                    case "Duration":
                                                    case "OrderDuration":
                                                        sBuilder.Append("Durration"); break;
                                                    case "Sec. Code":
                                                        sBuilder.Append("SecuritiesCode"); break;
                                                    case "N. Units":
                                                        sBuilder.Append("FilledUnits"); break;
                                                    case "Price":
                                                        sBuilder.Append("FilledPrice"); break;
                                                    case "Security Name":
                                                        sBuilder.Append("SecuritiesName"); break;
                                                    case "Exchange":
                                                        sBuilder.Append("ExchangeCode"); break;
                                                    case "Trade Date":
                                                        sBuilder.Append("TradeDate"); break;
                                                    case "Settlement Date":
                                                        sBuilder.Append("SettleDate"); break;

                                                    default:
                                                        if (dataCell != "Heading")
                                                            sBuilder.Append(dataCell);
                                                        else
                                                            goto X;
                                                        break;
                                                }
                                            }
                                        }
                                        else
                                            sBuilder.Append(dt.Rows[r][c]);
                                        sBuilder.Append(";");
                                    X:
                                        int nothingToDo = 1;
                                    }

                                    if (r == 1)
                                        sBuilder.Append("Type;");
                                    else
                                        sBuilder.Append((i + 1).ToString());
                                    sBuilder.Append("\\r\\n");
                                }
                            }
                        }
                    }
                }
            }
            return sBuilder.ToString();
        }
    }
}