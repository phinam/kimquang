using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Datalayer.DataObject.Core;
using Service.Data.Base;
using System.Data;

namespace Service.Data.Core.Class
{
    public class CCoreService:CServiceBase
    {
        public string GetContextData(string clientKey, string inputValue)
        {
            return CallCSVService(clientKey, inputValue,
                new dlg_CallFunction((c, i) => new CCoreDao().GetContextData(c, i)));

        }

        public DataSet GetContextDataSet(string clientKey, string inputValue)
        {
            return CallDataSetService(clientKey, inputValue,
                new dlg_CallFunction((c, i) => new CCoreDao().GetContextData(c, i)));

        }

        public string ExecuteAction(string clientKey, string inputValue)
        {
            return CallCSVService(clientKey, inputValue,
                new dlg_CallFunction((c, i) => new CCoreDao().ExecuteAction(c, i)));

        }

        public string Login(string clientKey, string inputValue)
        {
            return CallCSVService(clientKey, inputValue,
                new dlg_CallFunction((c, i) => new CCoreDao().Login(c, i)));

        }
    }
}