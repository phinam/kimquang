using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace Service.Data.Base
{
    public class CDataParser
    {
        public static string DataSetToCSV(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0) return "";

            StringBuilder sb = new StringBuilder();
            //To CSV table cuoi 
            sb.Append(DataTableToCSV(ds.Tables[ds.Tables.Count - 1]));
            //duyet qua cac table tru table cuoi
            for (int i = 0; i < ds.Tables.Count - 1; i++)
            {
                sb.Append("\n###\n");
                sb.Append(DataTableToCSV(ds.Tables[i]));
            }

            return sb.ToString();
        }

        public static string DataTableToCSV(DataTable tbl)
        {
            string Separator = ";";

            //********* Header **********
            string csvHeader = "";
            for (int i = 0; i < tbl.Columns.Count; i++)
            {
                csvHeader += tbl.Columns[i].ColumnName + Separator;
            }

            if (csvHeader.EndsWith(Separator))
            {
                csvHeader = csvHeader.Remove(csvHeader.Length - 1);
            }
            //******** Data **********

            string template = "{0}" + Separator;
            string stringTemplate = "\"{0}\"" + Separator;
            StringBuilder sbData = new StringBuilder();
            StringBuilder sbLine = new StringBuilder();

            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                sbLine = new StringBuilder();
                for (int j = 0; j < tbl.Columns.Count; j++)
                {
                    object obj = tbl.Rows[i][j];
                    if (obj == DBNull.Value)
                    {
                        sbLine.Append(Separator);
                    }
                    else if (obj is DateTime)
                    {
                        sbLine.AppendFormat(stringTemplate, ((DateTime)obj).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    }
                    else if (obj is string)
                    {
                        sbLine.AppendFormat(template, Newtonsoft.Json.JsonConvert.SerializeObject(obj.ToString()));
                    }
                    else if (obj is byte[])
                    {
                        sbLine.AppendFormat(template, System.Convert.ToBase64String((byte[])obj, 0, ((byte[])obj).Length));
                    }
                    else
                    {
                        sbLine.AppendFormat(template, obj);
                    }
                }

                sbLine.Append("\n");
                sbLine.Replace(";\n", "\n");
                sbData.Append(sbLine);
            }
            string result = csvHeader + "\n" + sbData.ToString();

            if (result.EndsWith("\n"))
                result = result.Remove(result.Length - 1);
            return result;
        }


    }
}