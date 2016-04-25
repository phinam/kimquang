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
            string sData = Context.Request.Form["data"];
            if (String.IsNullOrEmpty(sData)) { return; }
            try
            {
                string inputValue =string.Format( "<InputValue UserID=\"2\" /><RequestParams ListProductId=\"{0}\"  start=\"0\"  length=\"1\"  Sys_ViewID=\"23\" />",sData);
                DataSet data = new CCoreService().GetContextDataSet("", inputValue);

                ExcelPlusExport(data.Tables[0], Context);
                Response.Cookies["iscompleteexport"].Value = "true";
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
    }
}