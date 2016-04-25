using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Service.Data.Core.Class;
using System.Data;

namespace Service.Data.Core
{
    /// <summary>
    /// Summary description for CoreService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class CoreService : System.Web.Services.WebService
    {

        [WebMethod]
        public string GetContextData(string clientKey, string inputValue)
        {
            return new CCoreService().GetContextData(clientKey, inputValue);
        }

        [WebMethod]
        public string ExecuteAction(string clientKey, string inputValue)
        {
            return new CCoreService().ExecuteAction(clientKey, inputValue);
        }
        [WebMethod]
        public string Login(string clientKey, string inputValue)
        {
            return new CCoreService().Login(clientKey, inputValue);
        }

        [WebMethod]
        public DataSet GetContextDataSet(string clientKey, string inputValue)
        {
            return new CCoreService().GetContextDataSet(clientKey, inputValue);
        }
    }
}
