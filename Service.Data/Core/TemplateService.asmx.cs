using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Service.Data.Core.Class;
using Service.Data.Base;
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
    public class TemplateService : CServiceBase
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

        /// <summary>
        /// xuat bao gia co ban
        /// </summary>
        /// <param name="ClientKey"></param>
        /// <param name="InputValue"></param>
        /// <returns></returns>
        [WebMethod]
        public string OfferLetterBase(string ClientKey, string InputValue)
        {
            string template = AppDomain.CurrentDomain.BaseDirectory + "\\_Template\\Excel\\Template_BaoGiaCoBan.xlsx";
            return new CExcelReport().ExportReport(ClientKey, InputValue, template);
        }

        /// <summary>
        /// xuat bao gia chi tiet
        /// </summary>
        /// <param name="ClientKey"></param>
        /// <param name="InputValue"></param>
        /// <returns></returns>
        [WebMethod]
        public string OfferLetterDetail(string ClientKey, string InputValue)
        {
            string template = AppDomain.CurrentDomain.BaseDirectory + "\\_Template\\Excel\\Template_BaoGiaChiTiet.xlsx";
            return new CExcelReport().ExportReport(ClientKey, InputValue, template);
        }

        [WebMethod]
        public string ImportProductExcel(string ClientKey,string InputValue)
        {
            InputValue = HtmlDecode(InputValue);
            string uploadPath = System.Configuration.ConfigurationManager.AppSettings["CKFilderRootDir"];
            string fileName = CXmlUtils.GetXmlNodeValue(InputValue, "InputValue/@Filename");
            fileName = System.IO.Path.Combine(uploadPath, fileName);

            return  new CExcelImporter().ImportExcel(ClientKey, fileName);
            
        }

    }
}
