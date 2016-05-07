﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.Data;
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
        public string ExportTemplate(string templatePath, System.Data.DataTable dt, int sheetNumber =1)
        {
            FileInfo info = new FileInfo(templatePath);
            string newFile = Guid.NewGuid().ToString()+ info.Name;
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
            CExcelTemplateDefinition def = GetTemplateDefinition(worksheet);
            if (def == null) return "";

            if(def.loopDataRowIndex>0)
            {
                for(int i=0;i<dt.Rows.Count;i++)
                {
                    ApplyDataRow(worksheet, def, dt.Rows[i]);
                }
            }


            pck.Save();
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
            return "";
        }
        private void ApplyDataRow(ExcelWorksheet ws,CExcelTemplateDefinition def,DataRow row)
        {
            string templateRange = "";
            templateRange = "A{0}:" + def.definedColumnField[def.definedColumnField.Count - 1].address +"{0}";
            templateRange = string.Format(templateRange, def.loopDataRowIndex);
            ExcelRange rowTemplate = ws.Cells[templateRange];

            
            int pasteRow = row.Table.Rows.IndexOf(row)+ def.loopDataRowIndex;
            string pasteAddress = "A"+ pasteRow;
            rowTemplate.Copy(ws.Cells[pasteAddress]);
            for(int i = 0;i < def.definedColumnField.Count;i++)
            {
                ws.Cells[def.definedColumnField[i].address + pasteRow].Value = row[def.definedColumnField[i].value];
            }

        }
        private CExcelTemplateDefinition GetTemplateDefinition(ExcelWorksheet worksheet)
        {
            CExcelTemplateDefinition def = new CExcelTemplateDefinition();
            //Kiem tra o A1 co gia tri [Column] ko
            ExcelRange a1Cell = worksheet.Cells["A1"];
            if(a1Cell.Value !=null)
            {
                if(a1Cell.Value.ToString().Equals("[Column]",StringComparison.OrdinalIgnoreCase))
                {
                    def.isTemplate = true;
                }
            }
            //Neu khong phai template
            if (!def.isTemplate) return def;
            //Tim kiem row Index
            for(int i=2;i<100;i++)
            {
                ExcelRange aiCell = worksheet.Cells["A"+i];
                if(aiCell.Value !=null && aiCell.Value.ToString().Equals("[Row]",StringComparison.OrdinalIgnoreCase))
                {
                    def.definedRowIndex = i;
                    break;
                }
            }
            //Neu khong tim thay [Row] thi khong phai template
            if(def.definedRowIndex <=0)
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
                    CExcelCellValue c = new CExcelCellValue("" + (char)i + def.definedRowIndex, cell.Value.ToString());
                    def.definedColumnField.Add(c);
                }
            }
            def.definedDataRowIndex = new List<CExcelCellValue>();
            //Tim i
            for (int i = 2; i < 100; i++)
            {
                ExcelRange aiCell = worksheet.Cells["A" + i];
                if (aiCell.Value != null && aiCell.Value.ToString().Equals("i", StringComparison.OrdinalIgnoreCase))
                {
                    def.loopDataRowIndex = i;
                    break;
                }
                int dataIndex = 0;
                if(aiCell.Value !=null && int.TryParse(aiCell.Value.ToString(),out dataIndex))
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
    }
    public class CExcelTemplateDefinition
    {
        public bool isTemplate = false;
        public int definedRowIndex=0;
        public List<CExcelCellValue> definedColumnField;
        public int loopDataRowIndex;
        public List<CExcelCellValue> definedDataRowIndex;
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