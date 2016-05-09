using PMSA.Framework.Log;
using Service.Data.Core.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Service.Data.Excel
{
    public partial class export : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.Request.Form["listId"] != null)
            {
                string listId = Context.Request.Form["listId"];
                if (String.IsNullOrEmpty(listId)) { return; }
                string exportType = "";//"Excel|Pdf
                string sysViewId = "13";// "26|27"
                string languageId = "129";//"Excel|Pdf
                if (Context.Request.Form["exportType"] != null)
                    exportType = Context.Request.Form["exportType"];
                if (Context.Request.Form["sysViewID"] != null)
                    sysViewId = Context.Request.Form["sysViewId"];
                if (Context.Request.Form["languageId"] != null)
                    languageId = Context.Request.Form["languageId"];
               
                try
                {
                    if (sysViewId.Equals( "13"))
                    {
                        string inputValue = string.Format("<InputValue UserID=\"2\" /><RequestParams ListID=\"{0}\" ListProductId=\"{0}\" LanguageID=\"{1}\" ExportType=\"{2}\" start=\"0\"  length=\"1\"  Sys_ViewID=\"{3}\" />", listId, languageId, exportType, sysViewId);
                        DataSet data = new CCoreService().GetContextDataSet("", inputValue);

                        ExcelPlusExport(data.Tables[0], Context);
                        Response.Cookies["iscompleteexport"].Value = "true";
                    }
                    else if(sysViewId.Equals("26"))
                    {
                        string inputValue = string.Format("<InputValue UserID=\"2\" /><RequestParams ListID=\"{0}\" ListProductId=\"{0}\" LanguageID=\"{1}\" ExportType=\"{2}\" start=\"0\"  length=\"1\"  Sys_ViewID=\"{3}\" />", listId, languageId, exportType, sysViewId);
                        string template = AppDomain.CurrentDomain.BaseDirectory + "\\_Template\\Excel\\Template_BaoGiaCoBan.xlsx";
                        string result = new CExcelReport().ExportReport("", inputValue, template);
                        WriteResponse(result);
                    }
                    else if (sysViewId.Equals("27"))
                    {
                        string inputValue = string.Format("<InputValue UserID=\"2\" /><RequestParams ListID=\"{0}\" ListProductId=\"{0}\" LanguageID=\"{1}\" ExportType=\"{2}\" start=\"0\"  length=\"1\"  Sys_ViewID=\"{3}\" />", listId, languageId, exportType, sysViewId);
                        string template = AppDomain.CurrentDomain.BaseDirectory + "\\_Template\\Excel\\Template_BaoGiaChiTiet.xlsx";
                        string result = new CExcelReport().ExportReport("", inputValue, template);
                        WriteResponse(result);
                    }
                    //Export(sData);

                }
                catch (Exception ex)
                {
                    Response.Cookies["iscompleteexport"].Value = "false";
                    Response.Write(ex.ToString());
                    Response.End();
                    // Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "Exception", string.Format("alert('{0}')", ex.ToString()), true);
                }
            }
        }
        private void ExcelPlusExport(DataTable sData, HttpContext context)
        {
            try
            {
                CLogManager.WritePL("ExcelPlusExport", "Input Data::" + sData);
                //build Input <InputValue FileJSonData="StrinInBase64"/>
                string inputXml = "<InputValue />";
                string dataInBase64 = "";// PMSA.Framework.Utils.CBinaryUtils.BinaryToBase64(System.Text.Encoding.UTF8.GetBytes(sData));
                //   inputXml = PMSA.Framework.Utils.CXmlUtils.AddXmlAttribute(inputXml, "InputValue", "FileJSonData", dataInBase64);

                //   CLogManager.WritePL("ExcelPlusExport", "Input Value::" + inputXml);
                string outputXml = PMSA.iMarkets.Service.Core.Class.CExcelUtilEx.MakeExcelOnlineSub(inputXml, sData);

                //    CLogManager.WritePL("ExcelPlusExport", "Output Value::" + outputXml);
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
                //    CLogManager.WritePL("ExcelPlusExport", ex.ToString());
                throw ex;
            }
        }

        private void WriteResponse(string dataInBase64)
        {
            if(dataInBase64.StartsWith("00-"))
            {
                dataInBase64 = dataInBase64.Remove(0, 3);
            }
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
    }
}