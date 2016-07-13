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
                string exportType = "", addressTo = "", fullName = "", telephone = "", email = "", cellPhone = "", position = "", fileName = "";//"Excel|Pdf
                string sysViewId = "13";// "26|27"
                string languageId = "129";//"Excel|Pdf
                if (Context.Request.Form["exportType"] != null)
                    exportType = Context.Request.Form["exportType"];
                if (Context.Request.Form["sysViewID"] != null)
                    sysViewId = Context.Request.Form["sysViewId"];
                if (Context.Request.Form["languageId"] != null)
                    languageId = Context.Request.Form["languageId"];
                if (Context.Request.Form["exportType"] != null)
                    exportType = Context.Request.Form["exportType"];
                if (Context.Request.Form["sysViewID"] != null)
                    sysViewId = Context.Request.Form["sysViewId"];
                if (Context.Request.Form["languageId"] != null)
                    languageId = Context.Request.Form["languageId"];

                if (Context.Request.Form["addressTo"] != null)
                    addressTo = Context.Request.Form["addressTo"];

                if (Context.Request.Form["fullName"] != null)
                    fullName = Context.Request.Form["fullName"];
                if (Context.Request.Form["telephone"] != null)
                    telephone = Context.Request.Form["telephone"];
                if (Context.Request.Form["cellPhone"] != null)
                    cellPhone = Context.Request.Form["cellPhone"];
                if (Context.Request.Form["position"] != null)
                    position = Context.Request.Form["position"];
                if (Context.Request.Form["email"] != null)
                    email = Context.Request.Form["email"];

                ///SUA CHU FILE NAM HOA NHE NAM
                if (Context.Request.Form["fileName"] != null)
                    fileName = Context.Request.Form["fileName"];

                if (string.IsNullOrEmpty(fileName)) fileName = "NONAME";

               // CLogManager.WritePL("email", string.Format("email:{0},cellPhone:{1},telephone:{2},addressTo:{3},fullname:{4},position:{5},exportType:{6},sysViewId:{7}", email, cellPhone, telephone, addressTo,fullName, position, exportType, sysViewId));
               

                try
                {
                    string contentType = "application/Excel";
                    //string filename = ".xlsx";
                    if(exportType.Equals("pdf",StringComparison.OrdinalIgnoreCase))
                    {
                        contentType = "application/pdf";
                        if(!fileName.EndsWith(".pdf",StringComparison.OrdinalIgnoreCase))
                        {
                            fileName += ".pdf";
                        }
                        
                    }
                    else
                    {
                        if (!fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                        {
                            fileName += ".xlsx";
                        }
                    }
                    string input = string.Format("<RequestParams ListID=\"{0}\" ListProductId=\"{0}\" LanguageID=\"{1}\" ExportType=\"{2}\" start=\"0\"  length=\"1\"  Sys_ViewID=\"{3}\" ", listId, languageId, exportType, sysViewId);
                    input += string.Format(" FullName=\"{0}\" AddressTo=\"{1}\"",HtmlEncode( fullName), HtmlEncode(addressTo));
                    input += string.Format(" TelePhone=\"{0}\" CellPhone=\"{1}\" Email=\"{2}\" Position=\"{3}\"  FileName=\"{4}\"", HtmlEncode(telephone), HtmlEncode(cellPhone), HtmlEncode(email), HtmlEncode(position), HtmlEncode(fileName));
                    input += "/>";
                    if (sysViewId.Equals("26"))
                    {
                        string inputValue = input;// string.Format("<InputValue UserID=\"2\" /><RequestParams ListID=\"{0}\" ListProductId=\"{0}\" LanguageID=\"{1}\" ExportType=\"{2}\" start=\"0\"  length=\"1\"  Sys_ViewID=\"{3}\" />", listId, languageId, exportType, sysViewId);
                        string template = AppDomain.CurrentDomain.BaseDirectory + "\\_Template\\Excel\\Template_BaoGiaCoBan.xlsx";
                        string result = new CExcelReport().ExportReport("", inputValue, template);
                        //filename = "BaogiaCoban" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + filename;
                        WriteResponse(contentType, fileName, result);
                    }
                    else if (sysViewId.Equals("27"))
                    {
                        string inputValue = input;// string.Format("<InputValue UserID=\"2\" /><RequestParams ListID=\"{0}\" ListProductId=\"{0}\" LanguageID=\"{1}\" ExportType=\"{2}\" start=\"0\"  length=\"1\"  Sys_ViewID=\"{3}\" />", listId, languageId, exportType, sysViewId);
                        string template = AppDomain.CurrentDomain.BaseDirectory + "\\_Template\\Excel\\Template_BaoGiaChiTiet.xlsx";
                        string result = new CExcelReport().ExportReport("", inputValue, template);
                        //filename = "BaogiaChitiet" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + filename;
                        WriteResponse(contentType, fileName, result);
                    }
                    //Export(sData);

                }
                catch (Exception ex)
                {
                    CLogManager.WriteSL("ExportTemplate", ex.ToString());
                    Console.WriteLine(ex.ToString());
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

        private void WriteResponse(string contentType,string filename, string dataInBase64)
        {
            //CLogManager.WriteSL("WriteResponse", dataInBase64.Substring(0,100));
            if (dataInBase64.StartsWith("00-"))
            {
                dataInBase64 = dataInBase64.Remove(0, 3);
            }
            if (!string.IsNullOrEmpty(dataInBase64))
            {
                byte[] buffer = PMSA.Framework.Utils.CBinaryUtils.Base64ToBinary(dataInBase64);
                string fileName = filename;// string.Format("ExportData_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                Response.AddHeader("Content-Type", contentType);
                Response.ContentType = contentType;// "application/vnd.xls";
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

        private string HtmlEncode(string str)
        { 

            str = str.Replace("&", "&amp;");
            str = str.Replace("\"", "&qout;");
            str = str.Replace("'", "&#39;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            return str;
        }
    }
}