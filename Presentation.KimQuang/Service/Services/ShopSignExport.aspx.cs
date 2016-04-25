using FWS.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Presentation.WebApp2.Services
{
    public partial class ShopSignExport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string expdata = "";
            if (Page.Request.Form["data"] != null)
            {
                expdata = Page.Request.Form["data"].ToString();
                if (expdata != "")
                {
                    string filetype = "xls";
                    if (Page.Request.Form["filetype"] != null && Page.Request.Form["filetype"].ToString() != "")
                        filetype = Page.Request.Form["filetype"].ToString();
                    string fileName = "excel_export_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "." + filetype;

                    Page.Response.ContentType = "text/plain";
                    Page.Response.ClearHeaders();
                    Page.Response.ClearContent();
                    Page.Response.Clear();
                    if (filetype == "csv")
                        Page.Response.ContentType = "application/ms-excel";// "text/csv";
                    else if (filetype == "xls")
                        Page.Response.ContentType = "application/vnd.ms-excel";
                    Page.Response.ContentEncoding = Encoding.UTF8;
                    Page.Response.Charset = "UTF-8";
                    Page.Response.AddHeader("Content-Disposition", String.Format("attachment;filename=\"{0}\";inline Filename=\"{0}\";", fileName));

                    //Them cac byte thieu trong du lieu de danh dau encoding la UTF-8
                    var bBOM = new byte[] { 0xEF, 0xBB, 0xBF };
                    //var bBOM = new byte[] { 254, 255 };
                    Page.Response.OutputStream.Write(bBOM, 0, bBOM.Length);

                    Page.Response.Write(expdata);
                    Page.Response.End();
                }
            }
            else if (Page.Request.QueryString["request"] != null)
            {
                string request = Page.Request.QueryString["request"].ToString();
                TemplateService service = new TemplateService();
                string result = service.ShopSignSummary(System.Configuration.ConfigurationManager.AppSettings["FWS.VnAccounting.ClientKey"].ToString(), request);
                if (result.StartsWith("00"))
                {
                    result = result.Substring(3);
                    byte[] buffer = CBinaryUtils.Base64ToBinary(result);
                    string exportFile = "excel_export_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + ".xlsx";
                    Page.Response.ContentType = "application/ms-excel";// "text/csv";
                    Page.Response.ContentEncoding = Encoding.UTF8;
                    Page.Response.Charset = "UTF-8";
                    Page.Response.AddHeader("Content-Disposition", String.Format("attachment;filename=\"{0}\";inline Filename=\"{0}\";", exportFile));
                    Page.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    Page.Response.Flush();
                    Page.Response.End();
                    return;
                }
                //string result = new CSaleSummary
            }



            //else
            //{
            //    Page.Response.ContentType = "text/plain";
            //    Page.Response.Write("page request is invalid!!!");
            //    Page.Response.End();
            //}
        }
    }
}