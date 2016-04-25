using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Datalayer.DataObject.Base;
using Datalayer.DataObject.SQL;

namespace Datalayer.DataObject.Core
{
    public class CCoreDao:CDaoBase
    {
        public DataSet GetContextData(string clientKey, string inputValue)
        {
            return CallFunction(CStoreProcedure.GetContextData, clientKey, inputValue);
        }

        public DataSet ExecuteAction(string clientKey, string inputValue)
        {
            return CallFunction(CStoreProcedure.ExecuteAction, clientKey, inputValue);
        }
        public DataSet Login(string clientKey, string inputValue)
        {
            return CallFunction(CStoreProcedure.Login, clientKey, inputValue);
        }
    }
}
