using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using System.Net;

namespace Service.Data
{
    public partial class UploadFiles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (string f in Request.Files.AllKeys)
            {
                HttpPostedFile file = Request.Files[f];
                string uploadPath = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
                file.SaveAs(uploadPath+"\\" + file.FileName);
                Response.End();
            }	

        }
    }
}