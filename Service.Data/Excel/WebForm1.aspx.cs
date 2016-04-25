using PMSA.Framework.Utils;
using PMSA.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Data;
using Service.Data.Core.Class;


namespace Service.Data.Excel
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string inputValue = "<InputValue UserID=\"2\" /><RequestParams draw=\"1\"  start=\"0\"  length=\"1\"  Sys_ViewID=\"20\" />";
            DataSet data = new CCoreService().GetContextDataSet("", inputValue);

            ExcelPlusExport(data.Tables[0], Context);
        }
        private void ExcelPlusExport( DataTable sData, HttpContext context)
        {
            try
            {
                //CLogManager.WritePL("ExcelPlusExport", "Input Data::" + sData);
                //build Input <InputValue FileJSonData="StrinInBase64"/>
                string inputXml = "<InputValue />";
                string dataInBase64 = "";// PMSA.Framework.Utils.CBinaryUtils.BinaryToBase64(System.Text.Encoding.UTF8.GetBytes(sData));
             //   inputXml = PMSA.Framework.Utils.CXmlUtils.AddXmlAttribute(inputXml, "InputValue", "FileJSonData", dataInBase64);

                CLogManager.WritePL("ExcelPlusExport", "Input Value::" + inputXml);
                string outputXml =  PMSA.iMarkets.Service.Core.Class.CExcelUtilEx.MakeExcelOnlineSub(inputXml,sData);

                CLogManager.WritePL("ExcelPlusExport", "Output Value::" + outputXml);
                // Xy ly output
                string result = PMSA.Framework.Utils.CXmlUtils.GetXmlNodeValue(outputXml, "OutputValue/@Result");
                if (result == "1")
                {
                    dataInBase64 = PMSA.Framework.Utils.CXmlUtils.GetXmlNodeValue(outputXml, "OutputValue/@FileContent");
                    if (!string.IsNullOrEmpty(dataInBase64))
                    {
                        byte[] buffer = PMSA.Framework.Utils.CBinaryUtils.Base64ToBinary(dataInBase64);
                        string fileName = string.Format("ExportData_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
                        Response.Clear();
                        Response.ClearHeaders();
                        Response.ClearContent();
                        Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                        Response.AddHeader("Content-Type", "application/Excel");
                        Response.ContentType = "application/vnd.xls";
                        Response.AddHeader("Content-Length", buffer.Length.ToString());
                        byte[] bufferwriter = new byte[1024];
                        int srcOffset = 0;
                        int block = 1024;
                        while (srcOffset < buffer.Length)
                        {
                            if (srcOffset + 1024 > buffer.Length) block = buffer.Length - srcOffset;
                            System.Buffer.BlockCopy(buffer, srcOffset, bufferwriter, 0, block);
                            srcOffset += 1024;
                            Response.BinaryWrite(bufferwriter);
                        }
                        //Response.End();
                        //File.Delete(filePath);
                    }
                }
                else
                {

                    string message = PMSA.Framework.Utils.CXmlUtils.GetXmlNodeValue(outputXml, "OutputValue/@ErrorMessage");
                    //Response.Write(message);
                    throw new Exception(message);
                }
                Context.ApplicationInstance.CompleteRequest();

            }
            catch (Exception ex)
            {
                CLogManager.WritePL("ExcelPlusExport", ex.ToString());
                throw ex;
            }
        }
    }
}