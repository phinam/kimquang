using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Service.Data.Base
{
    public class CServiceBase
    {
        public delegate DataSet dlg_CallFunction(string x, string y);
        protected string CallCSVService(string ClientKey, string inputValue, dlg_CallFunction callFunction)
        {
            inputValue = HtmlDecode(inputValue);
            
            DataSet ds = null;
            ds = callFunction(ClientKey, inputValue);
            //new CCoreDao().Login(ClientKey, inputValue);
            return CDataParser.DataSetToCSV(ds);
           
        }

        private static string HtmlDecode(string xmlString)
        {
            return System.Web.HttpUtility.HtmlDecode(xmlString);
        }

        protected DataSet CallDataSetService(string ClientKey, string inputValue, dlg_CallFunction callFunction)
        {
            inputValue = HtmlDecode(inputValue);
            DataSet ds = null;
            ds = callFunction(ClientKey, inputValue);
            return ds;

        }
    }
}