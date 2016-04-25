using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Datalayer.DataObject.Core;
using Service.Data.Base;

namespace Service.Data.Core.Class
{
    public class CExcelReport
    {
        string uploadPath = @"D:\Working\Projects\Nippon\FWS.VnAccounting.Presentation.WebApp";
        private DataTable CreateSummaryDataTable()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("Name", typeof(string));
            tbl.Columns.Add("MapName", typeof(string));
            tbl.Columns.Add("Extend", typeof(string));

            DataRow row = tbl.NewRow();
            row["Name"] = "Ton";
            row["MapName"] = "Ton";
            row["Extend"] = "";
            tbl.Rows.Add(row);

            row = tbl.NewRow();
            row["Name"] = "Da gui yeu cau";
            row["MapName"] = "Request";
            row["Extend"] = "{CStyle_Name:\"color_#fff,background-color_#26C6DA\"}";
            tbl.Rows.Add(row);

            row = tbl.NewRow();
            row["Name"] = "Tong chua hoan thanh";
            row["MapName"] = "Pending";
            row["Extend"] = "";
            tbl.Rows.Add(row);

            row = tbl.NewRow();
            row["Name"] = "Khao sat";
            row["MapName"] = "Survey";
            row["Extend"] = "{CStyle_Name:\"color_#fff,background-color_#66BB6A\"}";
            tbl.Rows.Add(row);

            row = tbl.NewRow();
            row["Name"] = "Duyet mau";
            row["MapName"] = "Approve";
            row["Extend"] = "{CStyle_Name:\"color_#fff,background-color_#00B0FF\"}";
            tbl.Rows.Add(row);

            row = tbl.NewRow();
            row["Name"] = "Lap dat";
            row["MapName"] = "Install";
            row["Extend"] = "{CStyle_Name:\"color_#fff,background-color_#66BB6A\"}";
            tbl.Rows.Add(row);
            return tbl;
        }

       
        private DataTable GetSummaryResult(string ClientKey, string InputValue)
        {
            string MarketColumnName = "MarketName";
            string StatusColumnName = "Status";
            DataTable tblResult = CreateSummaryDataTable();

           // string fileName = CXML.GetXmlNodeValue(InputValue, "RequestParams/@FilePath");
            //fileName = HttpUtility.HtmlDecode(fileName);
            //fileName = uploadPath + "\\" + fileName;
            //DataTable data = ReadExcelFile(fileName);
            InputValue = "<InputValue /><RequestParams Sys_ViewID=\"14\" />";
            DataSet ds = new CCoreDao().GetContextData(ClientKey, InputValue);
            //Du lieu tra ve dang 
            // Market;Color;Ton,...,Install
            
            DataTable data = ds.Tables[0];
            Hashtable columns = new Hashtable();
            if (data != null)
            {
                if (data.Columns.Contains(MarketColumnName))
                {
                    foreach (DataRow row in data.Rows)
                    {
                        if (row[MarketColumnName] != null)
                        {
                            string market = row[MarketColumnName].ToString();
                            if (!string.IsNullOrEmpty(market) && !columns.ContainsKey(market))
                            {
                                columns.Add(market, true);

                                tblResult.Columns.Add(market, typeof(int));
                            }
                        }
                    }
                }
                //Summary data
                foreach (DataRow row in data.Rows)
                {
                    string market = row[MarketColumnName].ToString();
                    //string status = row[StatusColumnName].ToString();
                    foreach (DataRow resultRow in tblResult.Rows)
                    {
                        string mappName = resultRow["MapName"].ToString();
                        if (data.Columns.Contains(mappName))
                        {
                            resultRow[market] = row[mappName];
                        }
                        //if (resultRow["MapName"].ToString().Contains(market))
                        //{
                        //    if (tblResult.Columns.Contains(market))
                        //    {
                        //        resultRow[market] = (int)(row[resultRow["MapName"]] == DBNull.Value ? 0 : resultRow[market]) + 1;
                        //    }
                        //}
                    }
                }

            }
            return tblResult;
            //return "";
        }
        


        public string Export4SSDataToExcel(string ClientKey, string InputValue)
        {
            DataTable dt = GetSummaryResult(ClientKey, InputValue);
            return new CExcelTemplateUtils().Export4SSTemplate(dt);
            //return "";
        }

    }
}