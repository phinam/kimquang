using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Datalayer.DataObject.SQL;

namespace Datalayer.DataObject.Base
{
    public class CDaoBase
    {
        protected static string sConnectionString =
            "Data Source={0};Initial Catalog={1};MultipleActiveResultSets=True;Max Pool Size=500;User ID={2};Password={3};Connection Timeout=30";

        protected SqlConnection conn = null;
        protected SqlCommand command;
        protected SqlDataAdapter adapter;
        private const string CALLFUNCTIONSTORENAME = "core.usp_SYS_VRF_CallFunction";

        public CDaoBase()
        {
            string server = ConfigurationManager.AppSettings["Server"];
            string database = ConfigurationManager.AppSettings["Database"];
            string user = ConfigurationManager.AppSettings["User"];
            string pass = ConfigurationManager.AppSettings["Password"];
            
            sConnectionString = string.Format(sConnectionString, server, database, user, pass);
            conn = new SqlConnection(sConnectionString);
        }

        /// <summary>
        /// Open Sql Connection
        /// </summary>
        private void OpenConnection()
        {

            if (conn == null) conn = new SqlConnection(sConnectionString);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

        }

        /// <summary>
        /// Close Sql Connection
        /// </summary>
        private void CloseConnection()
        {
            if (conn == null) conn = new SqlConnection(sConnectionString);
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Excecute store by FunctionID
        /// Ham tra ve cau truc gom 1 hoac nhieu datatable
        /// DataTable cuoi luon la mot ApplicationMessage
        /// Khi tra ve tren giao dien thi dua table ApplicationMessage len dau tien
        /// 
        /// Data1                           ApplicationMessage
        /// Data2                   ===>    Data1
        /// ApplicationMessage              Data2
        /// 
        /// Nguyen nhan: Tren giao dien yeu can table ApplicationMessage la table dau tien
        /// Con duoi database thi phai tra ve o cuoi 
        /// do khong handle duoc ket qua tra ve cua cac store
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="inputValue"></param>
        /// <returns></returns>
        protected DataSet CallFunction(int functionID, string clientKey, string inputValue)
        {
            try
            {
                //inputValue = CCrypto.DeCodeInputValue(inputValue);
                command = new SqlCommand(CALLFUNCTIONSTORENAME, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@InputValue", inputValue);
                command.Parameters.AddWithValue("@FunctionID", functionID);
                command.Parameters.AddWithValue("@ClientKey", clientKey);
                adapter = new SqlDataAdapter(command);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                //Neu ket qua chi co 1 table hoac khong co thi khong xu ly
                if (ds == null || ds.Tables.Count < 2) return ds;

                //Neu ket qua co tu 2 table tro len thi dua table cuoi len thanh table dau tien
                //DataTable tbl = ds.Tables[ds.Tables.Count - 1];
                //ds.Tables.Remove(tbl);

                return ds;
            }
            catch (Exception ex)
            {
                string stack = GetStackTrace(new StackTrace());
                //FWS.Framework.Log.CLogManager.WriteDAL("CallFunction", stack + "::::" + ex.Message);
                throw ex;
            }
        }

       

        protected string GetStackTrace(StackTrace trace)
        {
            string stack = "";
            for (int i = trace.FrameCount - 1; i >= 0; i--)
            {
                string methodName = trace.GetFrame(i).GetMethod().Name;
                string moduleName = "";
                var reflectedType = trace.GetFrame(i).GetMethod().ReflectedType;
                if (reflectedType != null)
                    moduleName = reflectedType.Name;
                stack = stack + "/" + moduleName + "." + methodName;
            }
            return stack;
        }

        protected DataTable CreateMessageTable()
        {
            DataTable tbl = new DataTable("ApplicationMessage");
            tbl.Columns.Add("ID");
            tbl.Columns.Add("Code");
            tbl.Columns.Add("Name");
            tbl.Columns.Add("Description");
            tbl.Columns.Add("Result");
            tbl.Columns.Add("Type");
            return tbl;
        }
    }
}
