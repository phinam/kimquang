using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Service.Data.Core.Class;

namespace Service.Data.Core
{
    /// <summary>
    /// Summary description for TemplateService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class TemplateService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public string ShopSignSummary(string ClientKey, string InputValue)
        {
            return new CExcelReport().Export4SSDataToExcel(ClientKey, InputValue);
        }
    }
}
