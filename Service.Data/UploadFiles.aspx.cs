using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Net;

namespace Service.Data
{
    public partial class UploadFiles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder list = new StringBuilder();
            foreach (string f in Request.Files.AllKeys)
            {
                HttpPostedFile file = Request.Files[f];
                string uploadPath = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
                var fileName = DateTime.Now.ToString("yyyyMMdd-HHmmss-") + file.FileName;
                file.SaveAs(uploadPath + "\\" + fileName);
                list.Append(fileName);
               
            }
            Response.Write(list.ToString());
            Response.End();

        }
    }
}