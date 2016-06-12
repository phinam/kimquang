using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.Data;
using System.Drawing;
using System.Configuration;
using PMSA.Framework.Log;

namespace Service.Data.Core.Class
{
    public class CExcelTemplateUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string Export4SSTemplate(System.Data.DataTable dt)
        {
            string templateFile = AppDomain.CurrentDomain.BaseDirectory + "\\_Template\\NIPPON_4SSTemplate.xlsx";
            string newFile = AppDomain.CurrentDomain.BaseDirectory + "\\_Template\\NIPPON_4SS" + Guid.NewGuid().ToString() + ".xlsx";
            //Copy template to file
            if (File.Exists(templateFile))
            {
                File.Copy(templateFile, newFile);
            }
            else
            {
                return "01-Template not found";
            }
            CMixExcel mixExcel = new CMixExcel(newFile, true);
            int colIndex = 3;
            ExcelPackage pck = (ExcelPackage)mixExcel.ExcelMixCore;
            var worksheet = pck.Workbook.Worksheets[1];
            ExcelRange columnTemplate = worksheet.Cells["C1:C7"];
            ExcelRange pastedColumn = worksheet.Cells["D1"];//1, colIndex, 7, colIndex];
            //columnTemplate.Copy(pastedColumn);
            for (int i = 3; i < dt.Columns.Count; i++)
            {
                if (colIndex > 3)
                {
                    pastedColumn = worksheet.Cells[1, colIndex];//1, colIndex, 7, colIndex];
                    columnTemplate.Copy(pastedColumn);
                }

                pastedColumn[1, colIndex].Value = dt.Columns[i].ColumnName;
                pastedColumn[2, colIndex].Value = dt.Rows[0][i];
                pastedColumn[3, colIndex].Value = dt.Rows[1][i];
                pastedColumn[4, colIndex].Value = dt.Rows[2][i];
                pastedColumn[5, colIndex].Value = dt.Rows[3][i];
                pastedColumn[6, colIndex].Value = dt.Rows[4][i];
                pastedColumn[7, colIndex].Value = dt.Rows[5][i];

                colIndex++;

            }
            //Xoa cot template

            //if (!string.IsNullOrEmpty(mixExcel.PathFile))
            newFile = AppDomain.CurrentDomain.BaseDirectory + "\\_Template\\NIPPON_4SS" + Guid.NewGuid().ToString() + ".xlsx";
            //pck.Save();
            pck.SaveAs(new FileInfo(newFile));
            mixExcel.CloseStream();

            //pck.Stream.Flush();
            //pck.Stream.Close();

            FileStream fs = new FileStream(newFile, FileMode.OpenOrCreate);
            if (fs != null)
            {
                byte[] binaryData = new byte[fs.Length];
                long bytesRead = fs.Read(binaryData, 0, (int)fs.Length);
                fs.Close();
                string base64Data = Convert.ToBase64String(binaryData);
                string result = String.Format("00-{0}", base64Data);
                return result;
            }
            return "01-Unknow Error";
        }

        /// <summary>
        /// Cot dau tien la cot define
        /// o A1 la chuoi [Column] dung de xac dinh la sheet chua template
        /// Trên cột đầu sẽ có ô chứa chuỗi [Row] để định nghĩa các cột dữ liệu mapping
        /// trên dòng có chứa ô [Row], cot nao can dien du lieu thi chua ten field du lieu
        /// duoi o [Row] la ô có chứa ký tự "i" hoặc một số không âm
        /// "i": là dòng lặp duyệt từ 0 đến hết. duoi o "i" thi khong duoc phep co gia tri so
        /// số: là index của dòng dữ liệu
        /// Sau khi đổ dữ liệu vào thì xóa cột define và row define
        /// </summary>
        /// <param name="templatePath"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string ExportTemplate(string templatePath, System.Data.DataTable title, System.Data.DataTable dt, int sheetNumber =1,bool isExportPdf=false)
        {
            FileInfo info = new FileInfo(templatePath);
            string newFile = info.DirectoryName +"\\" + Guid.NewGuid().ToString()+ info.Name;
            if(info.Exists)
            {
                File.Copy(templatePath, newFile);
            }
            else
            {
                return "01-Template not found";
            }
            CMixExcel mixExcel = new CMixExcel(newFile, true);
            ExcelPackage pck = (ExcelPackage)mixExcel.ExcelMixCore;
            var worksheet = pck.Workbook.Worksheets[sheetNumber];

            ApplyTitleData(worksheet, title);
            CExcelTemplateDefinition def = GetTemplateDefinition(worksheet);
            if (def == null|| !def.isTemplate) return "";

            if(def.loopDataRowIndex>0)
            {
                for(int i=0;i<dt.Rows.Count;i++)
                {
                    ApplyLoopDataRow(worksheet, def, dt.Rows[i]);
                }
            }
            else if(!def.isHorizontal && def.definedDataRowIndex.Count >0 )
            {
                for(int i=0;i<def.definedDataRowIndex.Count;i++)
                {
                    int rowindex = 0;
                    if(int.TryParse(def.definedDataRowIndex[i].value,out rowindex))
                    {
                        if (dt.Rows.Count > rowindex)
                        {
                            DataRow row = dt.Rows[rowindex];
                            ApplyDataIndexRow(worksheet, def, def.definedDataRowIndex[i], row);
                        }
                    }
                    
                }
            }
            else if(def.isHorizontal && def.definedColumnField.Count>0)
            {
                for (int i = 0; i < def.definedColumnField.Count; i++)
                {
                    int rowindex = 0;
                    if (int.TryParse(def.definedColumnField[i].value, out rowindex))
                    {
                        if (dt.Rows.Count > rowindex)
                        {
                            DataRow row = dt.Rows[rowindex];
                            ApplyDataIndexColumn(worksheet, def, def.definedColumnField[i], row);
                        }
                    }

                }
            }
            DeleteDefineRow(worksheet, def);
            string newFile2 = AppDomain.CurrentDomain.BaseDirectory + "\\_Template\\Excel\\SaveAs" + Guid.NewGuid().ToString() + ".xlsx";
            pck.SaveAs(new FileInfo(newFile2));
            pck.Stream.Flush();
            pck.Stream.Close();
            mixExcel.CloseStream();
            
            //delete tempfile
            File.Delete(newFile);

            //pck.Stream.Flush();
            //pck.Stream.Close();
            //isExportPdf = true;
            if (isExportPdf)
            {
                string pdfFile = AppDomain.CurrentDomain.BaseDirectory + "\\_Template\\Excel\\SaveAs" + Guid.NewGuid().ToString() + ".pdf";
                FileInfo f = new FileInfo(newFile2);
                newFile2 = f.FullName;
                CExcelToPDF.ExportWorkbookToPdf(newFile2, pdfFile);
                File.Delete(newFile2);
                newFile2 = pdfFile;
            }

            FileStream fs = new FileStream(newFile2, FileMode.OpenOrCreate);
            if (fs != null)
            {
                byte[] binaryData = new byte[fs.Length];
                long bytesRead = fs.Read(binaryData, 0, (int)fs.Length);
                fs.Close();
                File.Delete(newFile2);
                string base64Data = Convert.ToBase64String(binaryData);
                string result = String.Format("00-{0}", base64Data);

                return result;
            }

            return "";
        }
        /// <summary>
        /// Insert new row, copy format row i to new row, set value
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="def"></param>
        /// <param name="row"></param>
        private void ApplyLoopDataRow(ExcelWorksheet ws,CExcelTemplateDefinition def,DataRow row)
        {
            string templateRange = "";
            templateRange = "A{0}:" + def.definedColumnField[def.definedColumnField.Count - 1].address +"{0}";
            templateRange = string.Format(templateRange, def.loopDataRowIndex);
            ExcelRange rowTemplate = ws.Cells[templateRange];

            
            int pasteRow = row.Table.Rows.IndexOf(row)+ def.loopDataRowIndex+1;
            string pasteAddress = "A"+ pasteRow;
            ws.InsertRow(pasteRow, 1);
            rowTemplate.Copy(ws.Cells[pasteAddress]);
            
            for(int i = 0;i < def.definedColumnField.Count;i++)
            {
                if (row.Table.Columns.Contains(def.definedColumnField[i].value))
                {
                    ws.Cells[def.definedColumnField[i].address + pasteRow].Value = row[def.definedColumnField[i].value];
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="def"></param>
        /// <param name="row"></param>
        private void ApplyDataIndexRow(ExcelWorksheet ws, CExcelTemplateDefinition def, CExcelCellValue rowIndex, DataRow row)
        {

            for (int i = 0; i < def.definedColumnField.Count; i++)
            {
                if (def.definedDataRowIndex[i].value.StartsWith("Image:", StringComparison.OrdinalIgnoreCase))
                {
                    string fieldName = def.definedDataRowIndex[i].value.Split(':')[1];
                    ApplyImageCell(ws, def.definedColumnField[i].address + rowIndex.address, def.definedDataRowIndex[i].value, row[fieldName].ToString());
                }
                else if (row.Table.Columns.Contains(def.definedDataRowIndex[i].value))
                {
                    ws.Cells[def.definedColumnField[i].address + rowIndex.address].Value = row[def.definedColumnField[i].value];
                }
            }

        }
        private void ApplyDataIndexColumn(ExcelWorksheet ws, CExcelTemplateDefinition def,CExcelCellValue col, DataRow row)
        {
          
            for (int i = 0; i < def.definedDataRowIndex.Count; i++)
            {
                if(def.definedDataRowIndex[i].value.StartsWith("Image:",StringComparison.OrdinalIgnoreCase))
                {
                    string fieldName = def.definedDataRowIndex[i].value.Split(':')[1];
                    ApplyImageCell(ws, col.address + def.definedDataRowIndex[i].address, def.definedDataRowIndex[i].value, row[fieldName].ToString());
                }
                else if (row.Table.Columns.Contains(def.definedDataRowIndex[i].value))
                {
                    ws.Cells[col.address + def.definedDataRowIndex[i].address].Value = row[def.definedDataRowIndex[i].value];
                }

            }

        }
        /// <summary>
        /// Xoa các dòng dinh nghia template
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="def"></param>
        private void DeleteDefineRow(ExcelWorksheet ws, CExcelTemplateDefinition def)
        {
            
            //xoa row i
            if(def.loopDataRowIndex >0)
            {
                ws.DeleteRow(def.loopDataRowIndex, 1);
            }
            //xóa [Row]
            if (def.definedRowIndex > 0)
            {
                ws.DeleteRow(def.definedRowIndex, 1);
            }

            //xoa column
            ws.DeleteColumn(def.definedColumnIndex);
        }
        private CExcelTemplateDefinition GetTemplateDefinition(ExcelWorksheet worksheet)
        {
            CExcelTemplateDefinition def = new CExcelTemplateDefinition();
            //Kiem tra o A1 co gia tri [Column] ko
            //duyet qua cac cell tren row 1 de kiem cel co gia tri [Column]
            for (int i = 1; i <= worksheet.Dimension.End.Column; i++)
            {
                ExcelRange a1Cell = worksheet.Cells[1,i];
                if (a1Cell.Value != null)
                {
                    if (a1Cell.Value.ToString().Equals("[Column]", StringComparison.OrdinalIgnoreCase))
                    {
                        def.isTemplate = true;
                        def.definedColumnIndex = i;
                    }
                }
            }
            //Neu khong phai template
            if (!def.isTemplate) return def;
            //Tim kiem row Index
            for(int i=1;i<100;i++)
            {
                ExcelRange aiCell = worksheet.Cells[i,def.definedColumnIndex];
                if(aiCell.Value !=null && aiCell.Value.ToString().Equals("[Row]",StringComparison.OrdinalIgnoreCase))
                {
                    def.definedRowIndex = i;
                    break;
                }
            }
            if (def.definedRowIndex == 0)
            {
                //Tim kiem row Index
                for (int i = 2; i < 100; i++)
                {
                    ExcelRange aiCell = worksheet.Cells[i,def.definedColumnIndex];
                    if (aiCell.Value != null && aiCell.Value.ToString().Equals("[Column]", StringComparison.OrdinalIgnoreCase))
                    {
                        def.definedRowIndex = i;
                        def.isHorizontal = true;
                        break;
                    }
                }
            }
            //Neu khong tim thay [Row] thi khong phai template
            if (def.definedRowIndex <=0)
            {
                def.isTemplate = false;
                return def;
            }
            def.definedColumnField = new List<CExcelCellValue>();
            //duyet qua cac o tren dong [Row] de tim column
            //for tu A-Z
            for (int i = 65; i<=90; i ++)
            {
                ExcelRange cell = worksheet.Cells[""+(char)i + def.definedRowIndex];
                if(cell.Value != null && cell.Value.ToString().Length>0)
                {
                    CExcelCellValue c = new CExcelCellValue("" + (char)i, cell.Value.ToString());
                    def.definedColumnField.Add(c);
                }
            }
            def.definedDataRowIndex = new List<CExcelCellValue>();
            //Tim i
            for (int i = 2; i < 100; i++)
            {
                if (i == def.definedRowIndex) continue;

                ExcelRange aiCell = worksheet.Cells[i,def.definedColumnIndex];
                if (aiCell.Value != null && aiCell.Value.ToString().Equals("i", StringComparison.OrdinalIgnoreCase))
                {
                    def.loopDataRowIndex = i;
                    break;
                }
                //int dataIndex = 0;
                if(aiCell.Value !=null && aiCell.Value.ToString().Length>0)
                {
                    def.definedDataRowIndex.Add(new CExcelCellValue(""+i, aiCell.Value.ToString()));
                }
            }

            return def;
        }

        private string GetCellValue(ExcelWorksheet worksheet,string address)
        {
            ExcelRange cell = worksheet.Cells[address];
            if (cell != null) return null;
            return cell.Value.ToString();
        }

        private void ApplyTitleData(ExcelWorksheet ws,DataTable title)
        {
            //Duyet qua tat ca cell cua template
            //cel nao co dinh dang {colName} thi replay gia tri vao
            //table title co 1 dong
            if (title == null || title.Rows.Count == 0) return;
            int maxcol = ws.Dimension.End.Column;
            int maxRow = ws.Dimension.End.Row;
            for(int row=1;row<=maxRow;row++)
            {
                for (int col = 1; col <= maxcol; col++)
                {
                    if (ws.Cells[row, col].Value == null) continue;
                    string value = ws.Cells[row, col].Value.ToString();
                    int replaceNum = 0;
                    while (replaceNum<10 && value.Contains("{{") && value.Contains("}}"))
                    {
                        replaceNum++;
                        if (value.Contains("{{") && value.Contains("}}"))
                        {
                            string colname = value.Substring(value.IndexOf("{{") + 2, value.IndexOf("}}") - value.IndexOf("{{") - 2);
                            for (int i = 0; i < title.Columns.Count; i++)
                            {
                                if (title.Columns[i].ColumnName.Equals(colname, StringComparison.OrdinalIgnoreCase))
                                {
                                    //gan gia tri   
                                    value = value.Replace("{{" + colname + "}}", title.Rows[0][i].ToString());
                                    //ws.Cells[row, col].Value = value;
                                    ws.SetValue(row, col, value);
                                }
                            }
                        }
                    }
                    
                }
            }
        }

        private void ApplyImageCell(ExcelWorksheet ws,string cellAddress, string format,string filename)
        {
            CLogManager.WriteSL("ApplyImageCell", "Cell:" + cellAddress + " format:" + format + " file:" + filename);
            try
            {
                if (!File.Exists(filename))
                {
                    filename = Path.Combine(ConfigurationManager.AppSettings["UploadDirectory"], "NoImageAvailable.png");
                }
                if (File.Exists(filename))
                {

                    Image image = Image.FromFile(filename);
                    var picture = ws.Drawings.AddPicture("FrontImage_"+ cellAddress, image);
                    picture.From.Column = GetColIndex(cellAddress)-1;// colIndex;
                    picture.From.Row = GetRowIndex(cellAddress)-1;// rowIndex;
                    picture.SetSize(220, 220);
                    // 2x2 px space for better alignment
                    picture.From.ColumnOff = Pixel2MTU(5);
                    picture.From.RowOff = Pixel2MTU(5);
                }
                else
                {
                    CLogManager.WriteSL("ApplyImageCell", "File not found:" + filename);
                }
            }
            catch(Exception ex)
            {
                CLogManager.WriteSL("ApplyImageCell-Ex", ex.ToString());
            }
            
        }
        public int Pixel2MTU(int pixels)
        {
            int mtus = pixels * 9525;
            return mtus;
        }
        
        private int GetRowIndex(string address)
        {
            string rowName = address.Substring(1);
           
            return int.Parse(rowName);
        }
        private int GetColIndex(string address)
        {
            char colName = address[0];
            int colIndex = colName - 64;
            return colIndex;

        }
    }
    public class CExcelTemplateDefinition
    {
        public bool isTemplate = false;
        public bool isHorizontal = false;
        public int definedColumnIndex = 1;
        public int definedRowIndex=0;
        public List<CExcelCellValue> definedColumnField;
        public int loopDataRowIndex;
        public List<CExcelCellValue> definedDataRowIndex;

        public CExcelTemplateDefinition()
        {
            definedDataRowIndex = new List<CExcelCellValue>();
            definedColumnField = new List<CExcelCellValue>();
        }
    }

    public class CExcelCellValue
    {
        public string address;
        public string value;

        public CExcelCellValue() { }
        public CExcelCellValue(string address,string value)
        {
            this.address = address;
            this.value = value;
        }
    }
}