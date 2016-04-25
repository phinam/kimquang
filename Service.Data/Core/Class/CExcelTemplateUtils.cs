using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using OfficeOpenXml;

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
    }
}