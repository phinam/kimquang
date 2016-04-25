using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.Text;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.HSSF.UserModel;

namespace PMSA.iMarkets.Service.Core
{
    public class CExcelCSVOutput
    {
        //const int RowMaximumSize = 65536;
        //const int ColumnMaximumSize = 256;
        public static string REPLACEDELEMETER = "~!@#$%^&*()";//(char)00;
        public static string CSVCELLDELEMETER = ";";
        private string Table1(string ID, string Code, string Name, string Description)
        {
            string result = "ID;Code;Name;Description\n";
            result += String.Format("{0};{1};{2};{3}", ID, Code.Replace(CSVCELLDELEMETER, REPLACEDELEMETER), Name.Replace(CSVCELLDELEMETER, REPLACEDELEMETER), Description.Replace(CSVCELLDELEMETER, REPLACEDELEMETER));
            return result;
        }

        private string Table2(CMixExcel mixExcel)
        {
            string result = string.Empty;
            result = "Name;Description\n";
            if (mixExcel != null && mixExcel.ExcelMixCore is ExcelPackage)
            {
                ExcelPackage epk = (ExcelPackage)mixExcel.ExcelMixCore;

                for (int i = 1; i <= epk.Workbook.Worksheets.Count; i++)
                    result += String.Format("{0};{1}\n", epk.Workbook.Worksheets[i].Name.Replace(CSVCELLDELEMETER, REPLACEDELEMETER), epk.Workbook.Worksheets[i].Name.Replace(CSVCELLDELEMETER, REPLACEDELEMETER));
            }
            else if (mixExcel != null && mixExcel.ExcelMixCore is HSSFWorkbook)
            {
                HSSFWorkbook hssWorkBook = (HSSFWorkbook)mixExcel.ExcelMixCore;
                for (int i = 0; i < hssWorkBook.NumberOfSheets; i++)
                {
                    ISheet excelSheet = hssWorkBook.GetSheetAt(i);
                    result += String.Format("{0};{1}\n", excelSheet.SheetName.Replace(CSVCELLDELEMETER, REPLACEDELEMETER), excelSheet.SheetName.Replace(CSVCELLDELEMETER, REPLACEDELEMETER));
                }
            }
            else
            {
                result = string.Empty;
            }
            return result;
        }

        private string Table3(CMixExcel mixExcel)
        {
            StringBuilder result = new StringBuilder();
            if (mixExcel != null && mixExcel.ExcelMixCore is ExcelPackage)
            {
                ExcelPackage excelPackage = (ExcelPackage)mixExcel.ExcelMixCore;
                ExcelWorksheets workSheets = excelPackage.Workbook.Worksheets;
                for (int i = 1; i <= workSheets.Count; i++)
                {
                    var worksheet = workSheets[i];
                    System.Xml.XmlNodeList test = worksheet.WorksheetXml.GetElementsByTagName("dimension");
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
                                for (int c = MinColumn; c <= MaxColumn; c++)
                                {
                                    ExcelColumn excelColumn = worksheet.Column(c);
                                    result.Append(String.Format("column({0}:{1})", excelColumn.ColumnMin, excelColumn.ColumnMax));
                                    if (c != MaxColumn)
                                        result.Append(";");
                                }

                                result.Append("\n");

                                int intMinRow = int.Parse(MinRow);
                                int intMaxRow = int.Parse(MaxRow);
                                for (int r = intMinRow; r <= intMaxRow; r++)
                                {
                                    for (int c = MinColumn; c <= MaxColumn; c++)
                                    {
                                        object valueCell = worksheet.Cells[r, c].Value;
                                        if (valueCell != null)
                                            result.Append(valueCell.ToString().Replace(CSVCELLDELEMETER, REPLACEDELEMETER));
                                        else
                                            result.Append(valueCell);
                                        if (c != MaxColumn)
                                            result.Append(";");
                                    }
                                    if(r < intMaxRow)
                                        result.Append("\n");
                                }
                                result.Append("\n###\n");
                            }
                        }//(rangesize != "A1")
                        else
                        {
                            object valueCell = worksheet.Cells[1, 1].Value;

                            if (valueCell != null)
                            {
                                result.Append("column(1:1)" + "\n");
                                result.Append(valueCell.ToString().Replace(CSVCELLDELEMETER, REPLACEDELEMETER));
                            }
                            else
                                result.Append("[NULL]");
                            result.Append("\n###\n");
                        }
                    }
                }
            }
            else if (mixExcel != null && mixExcel.ExcelMixCore is HSSFWorkbook)
            {
                HSSFWorkbook hssWorkbook = (HSSFWorkbook)mixExcel.ExcelMixCore;
                for (int i = 0; i < hssWorkbook.NumberOfSheets; i++)
                {
                    NPOI.HSSF.UserModel.HSSFSheet excelSheet = (NPOI.HSSF.UserModel.HSSFSheet)hssWorkbook.GetSheetAt(i);
                    NPOI.HSSF.Record.DimensionsRecord sheetDemention = excelSheet.Sheet.Dimensions;
                    for (int r = sheetDemention.FirstRow; r < sheetDemention.LastRow; r++)
                    {
                        for (int c = sheetDemention.FirstCol; c < sheetDemention.LastCol; c++)
                        {
                            try
                            {
                                IRow row = excelSheet.GetRow(r);
                                if (row != null)
                                {
                                    ICell excelSheetGetRowGetCell = row.GetCell(c);
                                    if (excelSheetGetRowGetCell != null)
                                    {
                                        switch (excelSheetGetRowGetCell.CellType)
                                        {
                                            case CellType.String:
                                                result.Append(excelSheetGetRowGetCell.StringCellValue.Replace(CSVCELLDELEMETER,REPLACEDELEMETER) + ";"); break;
                                            case CellType.Numeric:
                                                result.Append(excelSheetGetRowGetCell.NumericCellValue + ";"); break;
                                            case CellType.Boolean:
                                                result.Append(excelSheetGetRowGetCell.BooleanCellValue + ";"); break;
                                            case CellType.Blank:
                                                result.Append("[Blank];"); break;
                                            case CellType.Error:
                                                result.Append(excelSheetGetRowGetCell.ErrorCellValue + ";"); break;
                                            case CellType.Formula:
                                                {
                                                    switch (excelSheetGetRowGetCell.CachedFormulaResultType)
                                                    {
                                                        case CellType.Numeric:
                                                            result.Append(excelSheetGetRowGetCell.NumericCellValue + ";"); break;
                                                        case CellType.String:
                                                            result.Append(excelSheetGetRowGetCell.StringCellValue.Replace(CSVCELLDELEMETER,REPLACEDELEMETER) + ";"); break;
                                                        case CellType.Boolean:
                                                            result.Append(excelSheetGetRowGetCell.BooleanCellValue + ";"); break;
                                                        case CellType.Error:
                                                            result.Append(excelSheetGetRowGetCell.ErrorCellValue + ";"); break;
                                                    }
                                                }
                                                break;
                                            case CellType.Unknown:
                                                result.Append("[Unknown];"); break;
                                        }
                                    }
                                    else //excelSheetGetRowGetCell == null
                                    {
                                        result.Append("[NULL];");
                                    }
                                }
                            }
                            catch { }

                        }
                        result.Append("\n");
                    }
                    result.Append("\n###\n");
                }
            }

           // if (mixExcel != null)
            //    mixExcel.CloseStream();
            return result.ToString();
        }

        public string CreateOutput(string ID, string Code, string Name, string Description, CMixExcel mixExcel,bool isReplaceSpectialString = true)
        {
            string result = string.Empty;
            if (mixExcel != null)
                result = String.Format("{0}\n###\n{1}\n###\n{2}", Table1(ID, Code, Name, Description), Table2(mixExcel), Table3(mixExcel));
            else
                result = String.Format("{0}", Table1(ID, Code, Name, Description));
            if (isReplaceSpectialString)
                result = result.Replace(REPLACEDELEMETER, CSVCELLDELEMETER);
            return result;
        }

        public string CreateOutput2(CMixExcel mixExcel, System.Data.DataSet ds)
        {
            string result = string.Empty;
            try
            {
                if (mixExcel.ExcelMixCore != null && mixExcel.ExcelMixCore is ExcelPackage)
                {
                    ExcelPackage pck = (ExcelPackage)mixExcel.ExcelMixCore;
                    if (pck.Workbook.Worksheets.Count == ds.Tables.Count && ds != null)
                    {
                        for (int i = 0; i < pck.Workbook.Worksheets.Count; i++)
                        {
                            var worksheet = pck.Workbook.Worksheets[i + 1];//do worksheets bat dau tu index 1
                            System.Xml.XmlNodeList test = worksheet.WorksheetXml.GetElementsByTagName("dimension");
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
                                        System.Data.DataTable dt = ds.Tables[i];
                                        if (dt != null)
                                        {
                                            int indexSchema = 1;
                                            for (int r = 1; r <= intMaxRow; r++)
                                                if (worksheet.Cells[r, 1].Value.ToString().ToLower() == "schema")
                                                {
                                                    indexSchema = r; break;
                                                }

                                            List<int> lMappingIndex = new List<int>();
                                            for (int c = MinColumn; c <= MaxColumn; c++)
                                            {
                                                lMappingIndex.Add(-1);
                                                for (int j = 0; j < dt.Rows[0].Table.Columns.Count; j++)
                                                    if (worksheet.Cells[indexSchema, c].Value != null && dt.Rows[0][j] != null)
                                                    {
                                                        if (worksheet.Cells[indexSchema, c].Value.ToString() == dt.Rows[0][j].ToString())
                                                        {
                                                            lMappingIndex[c - MinColumn] = j;
                                                            break;
                                                        }
                                                    }
                                            }

                                            int indexStartInsert = 1;
                                            ExcelRange excelRangeTemplate = null;
                                            for (int r = 1; r <= intMaxRow; r++)
                                                if (worksheet.Cells[r, 1].Value.ToString().ToLower() == "rowtemplate")
                                                {
                                                    excelRangeTemplate = worksheet.Cells[r, MinColumn + 1, r, MaxColumn];
                                                    indexStartInsert = r; break;
                                                }

                                            ExcelRange excelRangeSummary = worksheet.Cells[indexStartInsert + 1, MinColumn, indexStartInsert + 1, MaxColumn];

                                            ExcelRange excelRangeSummaryNew = worksheet.Cells[indexStartInsert + dt.Rows.Count, MinColumn, indexStartInsert + dt.Rows.Count, MaxColumn]; ;
                                            excelRangeSummary.Copy(excelRangeSummaryNew);

                                            excelRangeSummary.Clear();
                                            int startValueIndentity = 1;
                                            for (int r = indexStartInsert + 1; r <= indexStartInsert + dt.Rows.Count - 1; r++)
                                            {
                                                ExcelRange excelRangeNew = worksheet.Cells[r, MinColumn + 1, r, MaxColumn];
                                                excelRangeTemplate.Copy(excelRangeNew);
                                                for (int c = MinColumn; c <= MaxColumn; c++)
                                                {
                                                    ExcelRange worksheetCells = worksheet.Cells[r, c];
                                                    int indexDT = lMappingIndex[c - MinColumn];
                                                    if (indexDT != -1)
                                                    {
                                                        string insertValue = dt.Rows[r - indexStartInsert][indexDT].ToString();
                                                        if (insertValue.StartsWith("'") && insertValue.EndsWith("'"))
                                                            worksheetCells.Value = insertValue.Replace("'", string.Empty);
                                                        else
                                                            worksheetCells.Value = Convert.ToInt64(insertValue);
                                                        worksheetCells.AutoFitColumns();
                                                    }
                                                    else if (worksheetCells.Value != null && !string.IsNullOrEmpty(worksheetCells.Value.ToString()) && worksheetCells.Value.ToString().ToUpper() == "IDENTITY")
                                                    {
                                                        worksheetCells.Value = startValueIndentity;
                                                        startValueIndentity++;
                                                    }
                                                }
                                            }
                                            worksheet.DeleteRow(indexSchema, 1);
                                            worksheet.DeleteRow(indexStartInsert - 1, 1);
                                        }
                                    }
                                }//(rangesize != "A1")
                                else
                                {
                                    worksheet.Cells[1, 1].Value = ds.Tables[i].Rows[0][0];
                                }
                            }
                            //else
                            //{
                            //    result = "<OutputValue Result=\"0\" ErrorMessage=\"Excel file have error page!!!!\"/>";
                            //    break;
                            //}
                        }
                        if (!string.IsNullOrEmpty(mixExcel.PathFile))
                            pck.SaveAs(new FileInfo(mixExcel.PathFile));


                        FileStream fs = new FileStream(mixExcel.PathFile, FileMode.OpenOrCreate);

                        if (fs != null)
                        {
                            byte[] binaryData = new byte[fs.Length];
                            long bytesRead = fs.Read(binaryData, 0, (int)fs.Length);
                            fs.Close();
                            string base64Data = Convert.ToBase64String(binaryData);
                            result = String.Format("<OutputValue  Result=\"1\" FileContent=\"{0}\" />", base64Data);
                        }
                    }
                    else //(pck.Workbook.Worksheets.Count == ds.Tables.Count && ds != null)
                    {
                        result = "<OutputValue Result=\"0\" ErrorMessage=\"Excel file have not suit with data CSV tables!!!!\"/>";
                    }
                }
                else if (mixExcel.ExcelMixCore != null && mixExcel.ExcelMixCore is HSSFWorkbook)
                {
                    HSSFWorkbook hssWorkbook = (HSSFWorkbook)mixExcel.ExcelMixCore;
                    if (hssWorkbook.NumberOfSheets == ds.Tables.Count && ds != null)
                    {
                        for (int i = 0; i < hssWorkbook.NumberOfSheets; i++)
                        {
                            NPOI.HSSF.UserModel.HSSFSheet excelSheet = (NPOI.HSSF.UserModel.HSSFSheet)hssWorkbook.GetSheetAt(i);
                            NPOI.HSSF.Record.DimensionsRecord sheetDemention = excelSheet.Sheet.Dimensions;
                            System.Data.DataTable dt = ds.Tables[i];
                            int indexSchema = 0;
                            for (int r = 1; r <= sheetDemention.LastRow; r++)
                            {
                                try
                                {
                                    IRow row = excelSheet.GetRow(r - 1);
                                    ICell excelSheetGetRowGetCell = row.GetCell(0);
                                    string cellValues = GetCell2003Value(excelSheetGetRowGetCell);
                                    if (!string.IsNullOrEmpty(cellValues) && cellValues.ToLower() == "schema")
                                    {
                                        indexSchema = r - 1; break;
                                    }
                                }
                                catch { }
                            }

                            List<int> lMappingIndex = new List<int>();
                            for (int c = sheetDemention.FirstCol; c <= sheetDemention.LastCol; c++)
                            {
                                lMappingIndex.Add(-1);
                                for (int j = 0; j < dt.Rows[0].Table.Columns.Count; j++)
                                {
                                    try
                                    {
                                        IRow row = excelSheet.GetRow(indexSchema);
                                        ICell excelSheetGetRowGetCell = row.GetCell(c);
                                        string cellValue = GetCell2003Value(excelSheetGetRowGetCell);
                                        if (!string.IsNullOrEmpty(cellValue) && dt.Rows[0][j] != null)
                                        {
                                            if (cellValue == dt.Rows[0][j].ToString())
                                            {
                                                lMappingIndex[c - sheetDemention.FirstRow] = j;
                                                break;
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }

                            int indexStartInsert = 0;
                            IRow excelRowTemplate = null;
                            for (int r = 1; r <= sheetDemention.LastRow; r++)
                            {
                                try
                                {
                                    IRow row = excelSheet.GetRow(r - 1);
                                    ICell excelSheetGetRowGetCell = row.GetCell(0);
                                    string cellValues = GetCell2003Value(excelSheetGetRowGetCell);
                                    if (!string.IsNullOrEmpty(cellValues) && cellValues.ToLower() == "rowtemplate")
                                    {
                                        excelRowTemplate = excelSheet.GetRow(r - 1);
                                        indexStartInsert = r - 1; break;
                                    }
                                }
                                catch { }
                            }

                            ///phan code ben excel  207 tro len
                            IRow excelRowSummary = excelSheet.GetRow(indexStartInsert + 1);
                            //ExcelRange excelRangeSummary = worksheet.Cells[indexStartInsert + 1, MinColumn, indexStartInsert + 1, MaxColumn];

                            IRow excelRowSummaryNew = excelSheet.GetRow(indexStartInsert + dt.Rows.Count);
                            excelRowSummaryNew = excelRowSummary.CopyRowTo(indexStartInsert + dt.Rows.Count);
                            excelSheet.RemoveRow(excelSheet.GetRow(indexStartInsert + 1));
                            int startValueIndentity = 1;
                            for (int r = indexStartInsert + 1; r <= indexStartInsert + dt.Rows.Count - 1; r++)
                            {
                                try
                                {
                                    IRow excelRowNew = null;
                                    excelRowNew = excelRowTemplate.CopyRowTo(r);
                                    for (int c = sheetDemention.FirstCol; c <= sheetDemention.LastCol; c++)
                                    {
                                        ICell worksheetCells = excelRowNew.GetCell(c);
                                        //ExcelRange worksheetCells = worksheet.Cells[r, c];
                                        string cellValue = GetCell2003Value(worksheetCells);
                                        int indexDT = lMappingIndex[c - sheetDemention.FirstCol];
                                        if (indexDT != -1)
                                        {
                                            string insertValue = dt.Rows[r - indexStartInsert][indexDT].ToString();
                                            if (insertValue.StartsWith("'") && insertValue.EndsWith("'"))
                                            {
                                                worksheetCells.SetCellValue(insertValue.Replace("'", string.Empty));
                                                worksheetCells.SetCellType(CellType.String);
                                            }
                                            else
                                            {
                                                worksheetCells.SetCellValue(Convert.ToInt64(insertValue));
                                                worksheetCells.SetCellType(CellType.Numeric);
                                            }
                                            worksheetCells.Sheet.AutoSizeColumn(c);
                                        }
                                        else if (!string.IsNullOrEmpty(cellValue) && cellValue.ToUpper() == "IDENTITY")
                                        {
                                            worksheetCells.SetCellValue(startValueIndentity);
                                            worksheetCells.SetCellType(CellType.Numeric);
                                            startValueIndentity++;
                                        }

                                        if (c == sheetDemention.FirstCol && cellValue == "RowTemplate")
                                        {
                                            worksheetCells.SetCellValue(string.Empty);
                                            worksheetCells.SetCellType(CellType.String);
                                        }
                                    }
                                }
                                catch { }
                            }
                            excelSheet.RemoveRow(excelSheet.GetRow(indexSchema));
                            excelSheet.RemoveRow(excelSheet.GetRow(indexStartInsert));

                            for (int t = indexStartInsert; t < indexStartInsert + dt.Rows.Count; t++)
                            {
                                IRow tempRow = excelSheet.GetRow(t + 1);
                                if (tempRow != null)
                                {
                                    tempRow.CopyRowTo(t);
                                    excelSheet.RemoveRow(excelSheet.GetRow(t + 1));
                                }
                            }

                            for (int t = indexSchema; t < indexSchema + dt.Rows.Count; t++)
                            {
                                IRow tempRow = excelSheet.GetRow(t + 1);
                                if (tempRow != null)
                                {
                                    tempRow.CopyRowTo(t);
                                    excelSheet.RemoveRow(excelSheet.GetRow(t + 1));
                                }
                            }
                            ///phan code ben excel  207 tro len
                        }
                        if (!string.IsNullOrEmpty(mixExcel.PathFile))
                        {
                            FileStream fs = new FileStream(mixExcel.PathFile, FileMode.OpenOrCreate);
                            hssWorkbook.Write(fs);

                            if (fs != null)
                            {
                                byte[] binaryData = new byte[fs.Length];
                                long bytesRead = fs.Read(binaryData, 0, (int)fs.Length);
                                fs.Close();
                                string base64Data = Convert.ToBase64String(binaryData);
                                result = String.Format("<OutputValue  Result=\"1\" FileContent=\"{0}\" />", base64Data);
                            }
                        }
                    }
                    else //(hssWorkbook.NumberOfSheets == ds.Tables.Count && ds != null)
                    {
                        result = "<OutputValue Result=\"0\" ErrorMessage=\"Excel file have not suit with data CSV tables!!!!\"/>";
                    }
                }
            }
            catch (Exception ex)
            {
                result = String.Format("<OutputValue Result=\"1\" ErrorMessage=\"{0}\"/>", ex.Message);
            }
            return result;
        }

        public string CreateOutput2Ex(CMixExcel mixExcel, System.Data.DataSet ds)
        {
            string result = string.Empty;
            try
            {
                if (mixExcel.ExcelMixCore != null && mixExcel.ExcelMixCore is ExcelPackage)
                {
                    #region Excel 2007++
                    ExcelPackage pck = (ExcelPackage)mixExcel.ExcelMixCore;
                    if (pck.Workbook.Worksheets.Count == ds.Tables.Count && ds != null)
                    {
                        for (int i = 0; i < pck.Workbook.Worksheets.Count; i++)
                        {
                            var worksheet = pck.Workbook.Worksheets[i + 1];//do worksheets bat dau tu index 1
                            System.Xml.XmlNodeList test = worksheet.WorksheetXml.GetElementsByTagName("dimension");
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
                                        System.Data.DataTable dt = ds.Tables[i];
                                        if (dt != null)
                                        {
                                            int indexSchema = 1;
                                            for (int r = 1; r <= intMaxRow; r++)
                                                if (worksheet.Cells[r, 1].Value != null && worksheet.Cells[r, 1].Value.ToString().ToLower() == "schema")
                                                {
                                                    indexSchema = r; break;
                                                }

                                            List<int> lMappingIndex = new List<int>();
                                            for (int c = MinColumn; c <= MaxColumn; c++)
                                            {
                                                lMappingIndex.Add(-1);
                                                for (int j = 0; j < dt.Rows[0].Table.Columns.Count; j++)
                                                    if (worksheet.Cells[indexSchema, c].Value != null && dt.Rows[0][j] != null)
                                                    {
                                                        if (worksheet.Cells[indexSchema, c].Value.ToString() == dt.Rows[0][j].ToString())
                                                        {
                                                            lMappingIndex[c - MinColumn] = j;
                                                            break;
                                                        }
                                                    }
                                            }

                                            int indexStartInsert = 1;
                                            ExcelRange excelRangeTemplate = null;
                                            for (int r = 1; r <= intMaxRow; r++)
                                            {
                                                if (worksheet.Cells[r, 1].Value == null)
                                                    worksheet.Cells[r, 1].Value = string.Empty;
                                                if (worksheet.Cells[r, 1].Value.ToString().ToLower() == "rowtemplate")
                                                {
                                                    excelRangeTemplate = worksheet.Cells[r, MinColumn + 1, r, MaxColumn];
                                                    indexStartInsert = r; break;
                                                }
                                            }

                                            ExcelRange excelRangeSummary = worksheet.Cells[indexStartInsert + 1, MinColumn, indexStartInsert + 1, MaxColumn];

                                            ExcelRange excelRangeSummaryNew = worksheet.Cells[indexStartInsert + dt.Rows.Count, MinColumn, indexStartInsert + dt.Rows.Count, MaxColumn];
                                            excelRangeSummary.Copy(excelRangeSummaryNew);

                                            excelRangeSummary.Clear();
                                            int startValueIndentity = 1;
                                            for (int r = indexStartInsert + 1; r <= indexStartInsert + dt.Rows.Count - 1; r++)
                                            {
                                                ExcelRange excelRangeNew = worksheet.Cells[r, MinColumn + 1, r, MaxColumn];
                                                excelRangeTemplate.Copy(excelRangeNew);
                                                for (int c = MinColumn; c <= MaxColumn; c++)
                                                {
                                                    ExcelRange worksheetCells = worksheet.Cells[r, c];
                                                    int indexDT = lMappingIndex[c - MinColumn];
                                                    if (indexDT != -1)
                                                    {
                                                        string insertValue = dt.Rows[r - indexStartInsert][indexDT].ToString();
                                                        //if (insertValue.StartsWith("'") && insertValue.EndsWith("'"))
                                                            worksheetCells.Value = insertValue.Replace("'", string.Empty);
                                                        //else
                                                        //    worksheetCells.Value = Convert.ToInt64(insertValue);
                                                        //worksheetCells.AutoFitColumns();
                                                        ExcelColumn excelColumn = worksheet.Column(r);
                                                        if (excelColumn != null && !excelColumn.Hidden)
                                                            excelColumn.AutoFit();
                                                    }
                                                    else if (worksheetCells.Value != null && !string.IsNullOrEmpty(worksheetCells.Value.ToString()) && worksheetCells.Value.ToString().ToUpper() == "IDENTITY")
                                                    {
                                                        worksheetCells.Value = startValueIndentity;
                                                        startValueIndentity++;
                                                    }
                                                }
                                            }
                                            worksheet.DeleteRow(indexSchema, 1);
                                            worksheet.DeleteRow(indexStartInsert - 1, 1);
                                            
                                        }
                                    }
                                }//(rangesize != "A1")
                                else
                                {
                                    worksheet.Cells[1, 1].Value = ds.Tables[i].Rows[0][0];
                                }
                            }
                            //else
                            //{
                            //    result = "<OutputValue Result=\"0\" ErrorMessage=\"Excel file have error page!!!!\"/>";
                            //    break;
                            //}
                            worksheet.Column(1).Hidden = true;
                        }
                        FileStream fs = new FileStream(mixExcel.PathFile + "_Temp.xlsx", FileMode.OpenOrCreate);

                        if (fs != null)
                        {
                            pck.SaveAs(fs);
                            fs.Close();
                            fs = new FileStream(mixExcel.PathFile + "_Temp.xlsx", FileMode.OpenOrCreate);
                            byte[] binaryData = new byte[fs.Length];
                            long bytesRead = fs.Read(binaryData, 0, (int)fs.Length);
                            fs.Close();
                            string base64Data = Convert.ToBase64String(binaryData);
                            result = String.Format("<OutputValue  Result=\"1\" FileContent=\"{0}\" />", base64Data);
                        }
                    }
                    else //(pck.Workbook.Worksheets.Count == ds.Tables.Count && ds != null)
                    {
                        result = "<OutputValue Result=\"0\" ErrorMessage=\"Excel file have not suit with data CSV tables!!!!\"/>";
                    }
                    #endregion
                }
                else if (mixExcel.ExcelMixCore != null && mixExcel.ExcelMixCore is HSSFWorkbook)
                {
                    #region Excel 2003--
                    HSSFWorkbook hssWorkbook = (HSSFWorkbook)mixExcel.ExcelMixCore;
                    if (hssWorkbook.NumberOfSheets == ds.Tables.Count && ds != null)
                    {
                        for (int i = 0; i < hssWorkbook.NumberOfSheets; i++)
                        {
                            NPOI.HSSF.UserModel.HSSFSheet excelSheet = (NPOI.HSSF.UserModel.HSSFSheet)hssWorkbook.GetSheetAt(i);
                            NPOI.HSSF.Record.DimensionsRecord sheetDemention = excelSheet.Sheet.Dimensions;
                            System.Data.DataTable dt = ds.Tables[i];
                            int indexSchema = 0;
                            for (int r = 0; r < sheetDemention.LastRow; r++)
                            {
                                try
                                {
                                    IRow row = excelSheet.GetRow(r);
                                    if (row != null)
                                    {
                                        ICell excelSheetGetRowGetCell = row.GetCell(0);
                                        string cellValues = GetCell2003Value(excelSheetGetRowGetCell);
                                        if (!string.IsNullOrEmpty(cellValues) && cellValues.ToLower() == "schema")
                                        {
                                            indexSchema = r; 
                                            break;
                                        }
                                    }
                                }
                                catch { }
                            }

                            List<int> lMappingIndex = new List<int>();
                            for (int c = sheetDemention.FirstCol; c <= sheetDemention.LastCol; c++)
                            {
                                lMappingIndex.Add(-1);
                                for (int j = 0; j < dt.Rows[0].Table.Columns.Count; j++)
                                {
                                    try
                                    {
                                        IRow row = excelSheet.GetRow(indexSchema);
                                        if (row != null)
                                        {
                                            ICell excelSheetGetRowGetCell = row.GetCell(c);
                                            string cellValue = GetCell2003Value(excelSheetGetRowGetCell);
                                            if (!string.IsNullOrEmpty(cellValue) && dt.Rows[0][j] != null)
                                            {
                                                if (cellValue == dt.Rows[0][j].ToString())
                                                {
                                                    lMappingIndex[c - sheetDemention.FirstRow] = j;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }

                            int indexStartInsert = 0;
                            IRow excelRowTemplate = null;
                            for (int r = 0; r < sheetDemention.LastRow; r++)
                            {
                                try
                                {
                                    IRow row = excelSheet.GetRow(r);
                                    if (row != null)
                                    {
                                        ICell excelSheetGetRowGetCell = row.GetCell(0);
                                        string cellValues = GetCell2003Value(excelSheetGetRowGetCell);
                                        if (!string.IsNullOrEmpty(cellValues) && cellValues.ToLower() == "rowtemplate")
                                        {
                                            excelRowTemplate = excelSheet.GetRow(r);
                                            indexStartInsert = r; break;
                                        }
                                    }
                                }
                                catch { }
                            }

                            ///phan code ben excel  207 tro len
                            IRow excelRowSummary = excelSheet.GetRow(indexStartInsert + 1);
                            //ExcelRange excelRangeSummary = worksheet.Cells[indexStartInsert + 1, MinColumn, indexStartInsert + 1, MaxColumn];

                            IRow excelRowSummaryNew = excelSheet.GetRow(indexStartInsert + dt.Rows.Count);
                            excelRowSummaryNew = excelRowSummary.CopyRowTo(indexStartInsert + dt.Rows.Count);
                            excelSheet.RemoveRow(excelSheet.GetRow(indexStartInsert + 1));
                            int startValueIndentity = 1;
                            for (int r = indexStartInsert + 1; r <= indexStartInsert + dt.Rows.Count - 1; r++)
                            {
                                try
                                {
                                    IRow excelRowNew = null;
                                    excelRowNew = excelRowTemplate.CopyRowTo(r);
                                    for (int c = sheetDemention.FirstCol; c < sheetDemention.LastCol; c++)
                                    {
                                        //ICell worksheetCells = excelRowNew.GetCell(c, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                                        ICell worksheetCells = excelRowNew.CreateCell(c);
                                        //ExcelRange worksheetCells = worksheet.Cells[r, c];
                                        string cellValue = GetCell2003Value(worksheetCells);
                                        int indexMap = c - sheetDemention.FirstCol;
                                        if (indexMap >= 0 && indexMap < lMappingIndex.Count)
                                        {
                                            int indexDT = lMappingIndex[c - sheetDemention.FirstCol];
                                            if (indexDT != -1)
                                            {
                                                string insertValue = dt.Rows[r - indexStartInsert][indexDT].ToString();
                                                if (insertValue.StartsWith("'") && insertValue.EndsWith("'"))
                                                {
                                                    worksheetCells.SetCellValue(insertValue.Replace("'", string.Empty));
                                                    worksheetCells.SetCellType(CellType.String);
                                                }
                                                else
                                                {
                                                    worksheetCells.SetCellValue(Convert.ToInt64(insertValue));
                                                    worksheetCells.SetCellType(CellType.Numeric);
                                                }
                                                worksheetCells.Sheet.AutoSizeColumn(c);
                                            }
                                            else if (!string.IsNullOrEmpty(cellValue) && cellValue.ToUpper() == "IDENTITY")
                                            {
                                                worksheetCells.SetCellValue(startValueIndentity);
                                                worksheetCells.SetCellType(CellType.Numeric);
                                                startValueIndentity++;
                                            }
                                        }
                                        if (c == sheetDemention.FirstCol && cellValue == "RowTemplate")
                                        {
                                            worksheetCells.SetCellValue(string.Empty);
                                            worksheetCells.SetCellType(CellType.String);
                                        }
                                    }
                                }
                                catch { }
                            }
                            excelSheet.RemoveRow(excelSheet.GetRow(indexSchema));
                            excelSheet.RemoveRow(excelSheet.GetRow(indexStartInsert));

                            for (int t = indexStartInsert; t < indexStartInsert + dt.Rows.Count; t++)
                            {
                                IRow tempRow = excelSheet.GetRow(t + 1);
                                if (tempRow != null)
                                {
                                    tempRow.CopyRowTo(t);
                                    excelSheet.RemoveRow(excelSheet.GetRow(t + 1));
                                }
                            }

                            for (int t = indexSchema; t < indexSchema + dt.Rows.Count; t++)
                            {
                                IRow tempRow = excelSheet.GetRow(t + 1);
                                if (tempRow != null)
                                {
                                    tempRow.CopyRowTo(t);
                                    excelSheet.RemoveRow(excelSheet.GetRow(t + 1));
                                }
                            }
                            ///phan code ben excel  207 tro len
                            excelSheet.SetColumnHidden(0, true);
                            //excelSheet.RemoveColumn(0);
                        }
                        if (!string.IsNullOrEmpty(mixExcel.PathFile))
                        {
                            FileStream fs = new FileStream(mixExcel.PathFile, FileMode.OpenOrCreate);
                            hssWorkbook.Write(fs);
                            fs.Close();
                            if (fs != null)
                            {
                                fs = new FileStream(mixExcel.PathFile, FileMode.OpenOrCreate);
                                byte[] binaryData = new byte[fs.Length];
                                long bytesRead = fs.Read(binaryData, 0, (int)fs.Length);
                                fs.Close();
                                string base64Data = Convert.ToBase64String(binaryData);
                                result = String.Format("<OutputValue  Result=\"1\" FileContent=\"{0}\" />", base64Data);
                            }
                        }
                    }
                    else //(hssWorkbook.NumberOfSheets == ds.Tables.Count && ds != null)
                    {
                        result = "<OutputValue Result=\"0\" ErrorMessage=\"Excel file have not suit with data CSV tables!!!!\"/>";
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result = String.Format("<OutputValue Result=\"0\" ErrorMessage=\"{0}\"/>", ex.Message + ex.StackTrace);
            }
            return result;
        }

        private string GetCell2003Value(ICell excelSheetGetRowGetCell)
        {
            if (excelSheetGetRowGetCell != null)
            try
            {
                switch (excelSheetGetRowGetCell.CellType)
                {
                    case CellType.String:
                        return excelSheetGetRowGetCell.StringCellValue;
                    case CellType.Numeric:
                        return excelSheetGetRowGetCell.NumericCellValue.ToString();
                    case CellType.Boolean:
                        return excelSheetGetRowGetCell.BooleanCellValue.ToString();
                    case CellType.Blank:
                        return "[Blank]";
                    case CellType.Error:
                        return excelSheetGetRowGetCell.ErrorCellValue.ToString();
                    case CellType.Formula:
                        return excelSheetGetRowGetCell.CellFormula.ToString();
                    case CellType.Unknown:
                        return "[Unknown]";
                    default:
                        return excelSheetGetRowGetCell.StringCellValue;
                }
            }
            catch { return string.Empty; }
            else
                return string.Empty;
        }
    }
}