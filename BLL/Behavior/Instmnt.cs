using System.Collections.Generic;
using System.Linq;
using System;

namespace GOSTS
{
    public class Instmnt
    {
        public Instmnt(string instmntCode, string marketCode)
        {
            this.InstmntCode = instmntCode;
            this.MarketCode = marketCode;
        }

        public string InstmntCode { get; private set; }
        public string MarketCode { get; private set; }

        readonly List<ProdType> _prodTypes = new List<ProdType>();
        public List<ProdType> ProdTypes
        {
            get { return _prodTypes; }
        }
    }

    public class ProdType
    {
        public ProdType(string TypeCode, string instmntCode, string marketCode)
        {
            this.ProdTypeCode = TypeCode;
            this.InstmntCode = instmntCode;
            this.MarketCode = marketCode;
        }
        public ProdType(string TypeCode, Instmnt instmnt)
        {
            this.ProdTypeCode = TypeCode;
            this.InstmntCode = instmnt.InstmntCode;
            this.MarketCode = instmnt.MarketCode;
        }

        readonly List<Prod> _Prods = new List<Prod>();
        public List<Prod> Prods
        {
            get { return _Prods; }
        }

        private string prodTypeCode;
        public string ProdTypeCode
        {
            get { return prodTypeCode; }
            private set
            {
                prodTypeCode = value;
                switch (value)
                {
                    case "0":
                        ProdTypeName = GOSTS.GosCulture.CultureHelper.GetString("TreeUnderlying");// "Underlying";
                        break;
                    case "1":
                        ProdTypeName = GOSTS.GosCulture.CultureHelper.GetString("TreeFutures");//  "Futures";
                        break;
                    case "2":
                        ProdTypeName = GOSTS.GosCulture.CultureHelper.GetString("TreeOptions");//  "Options";
                        break;
                    case "3":
                        ProdTypeName = GOSTS.GosCulture.CultureHelper.GetString("TreeSpreads");//  "Spreads";
                        break;
                }
            }
        }
        public string ProdTypeName { get; private set; }
        public string InstmntCode { get; private set; }
        public string MarketCode { get; private set; }
    }

    public class Prod
    {
        public Prod(string prodCode)
        {
            this.ProdCode = prodCode;
        }

        public Prod(string prodCode,string prodName, ProdType prodType)
        {
            this.ProdCode = prodCode;
            this.ProdName = prodName;
            this.ProdTypeCode = prodType.ProdTypeCode;
            this.InstmntCode = prodType.InstmntCode;
            this.MarketCode = prodType.MarketCode;
        }

        public Prod(string prodCode,string prodName, ProdOptionType prodOptionType)
        {
            this.ProdCode = prodCode;
            this.ProdName = prodName;
            this.ProdTypeCode = prodOptionType.ProdOptionTypeName ;
            this.InstmntCode = prodOptionType.InstmntCode;
            this.MarketCode = prodOptionType.MarketCode;
        }

        public string ProdName { get; private set; }
        public string ProdCode { get; private set; }
        public string ProdOptionType { get; private set; }
        public string ProdTypeCode { get; private set; }
        public string InstmntCode { get; private set; }
        public string MarketCode { get; private set; }
    }

    public class ProdOption
    {
        public ProdOption(DateTime prodOptionName, ProdType prodType)
        {
            this.ProdOptionName = prodOptionName;
            this.InstmntCode = prodType.InstmntCode;
            this.MarketCode = prodType.MarketCode;
        } 

        public DateTime  ProdOptionName { get; private set; } 
        public string InstmntCode { get; private set; }
        public string MarketCode { get; private set; }
    }

    public class ProdOptionType
    {
        public ProdOptionType(string prodOptionTypeName, ProdOption prodOption)
        {
            this.ProdOptionTypeName = prodOptionTypeName;
            this.ProdOptionName = prodOption.ProdOptionName;
            this.InstmntCode = prodOption.InstmntCode;
            this.MarketCode = prodOption.MarketCode;
        }

        public string ProdOptionTypeName { get; private set; }
        public DateTime ProdOptionName { get; private set; } 
        public string InstmntCode { get; private set; }
        public string MarketCode { get; private set; }
    } 
}