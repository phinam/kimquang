using System;
using System.Collections.Generic;
using System.Text;

namespace PMSA.iMarkets.Service.Core.Class
{
    [Serializable]
    public class CTradeInterface
    {
        public string CSVFields = "No;ID;TradeDate;AccountCode;SideString;OrderUnits;SecuritiesCode;ExchangeCode;SecuritiesTypeString;OrderPrice;OrderTypeString;Currency;StatusString;OrderModeString;FilledUnits;FilledValue;TradeBlotterID;OrderNo;CountryCode;SettleDate";
        public string CSVFieldDJDs = "ID;SecuritiesCode;SideString;FilledUnits;FilledPrice;OrderTypeString;Durration;SecuritiesName;ExchangeCode;Type;CountryCode;Name;Address;AvailableArea;PriceDescription;VATTax;ServiceFee;Description";

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
        public string Name { get; set; }
        public string Address { set; get; }
        public string AvailableArea { set; get; }
        public string PriceDescription { set; get; }
        public string VATTax { set; get; }
        public string ServiceFee { set; get; }
        public string Description { set; get; }
        
    }

    [Serializable]
    public class CProductList
    {
        public List<CTradeInterface> ListTrade;
    }
}
