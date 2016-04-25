using System;
using System.Collections.Generic;
using System.Text;

namespace PMSA.iMarkets.Service.Core.Class
{
    [Serializable]
    public class CProduct
    {
        public string CSVFields = "No;ID;TradeDate;AccountCode;SideString;OrderUnits;SecuritiesCode;ExchangeCode;SecuritiesTypeString;OrderPrice;OrderTypeString;Currency;StatusString;OrderModeString;FilledUnits;FilledValue;TradeBlotterID;OrderNo;CountryCode;SettleDate";

        public int No { set; get; }
        public int ID { set; get; }
       
        public string TradeDate { set; get; }
        public string AccountCode { set; get; }
        public string SideString { set; get; }
        public string OrderUnits { set; get; }
        public string SecuritiesCode { set; get; }
        
        public string ExchangeCode { set; get; }
        public string SecuritiesTypeString { set; get; }

        public string OrderPrice { set; get; }
        public string OrderTypeString { set; get; }
        public string Currency { set; get; }
        public string StatusString { set; get; }
        public string OrderModeString { set; get; }
        public string FilledUnits { set; get; }
        public string FilledPrice { set; get; }
        public string FilledValue { set; get; }
        public string TradeBlotterID { set; get; }
        public string OrderNo { set; get; }

        public string Durration { set; get; }
        public string SecuritiesName { set; get; }

        public string IsFixedIncome { set; get; }
        public string IsPreferredShare { set; get; }
        public string CountryCode { get; set; }
        public string SettleDate { set; get; }
    }

    [Serializable]
    public class CTradeInterfaceList
    {
        public List<CTradeInterface> ListTrade;
    }
}
