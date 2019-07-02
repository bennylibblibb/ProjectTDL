


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GOSTS.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace GOSTS
{ 
    public class MarketPriceData
    {
        private static DataTable instListTable = null;
        public DataTable InstListTable
        {
            get
            {
                return instListTable;
            }
            set
            {
                instListTable = value;
            }
        }

        private static List<String> marketCodeList = null;
        public List<String> MarketCodeList
        {
            get
            {
                return marketCodeList;
            }
            set
            {
                marketCodeList = value;
            }
        }

        private static DataTable prodListTable = null;
        public DataTable ProdListTable
        {
            get
            {
                return prodListTable;
            }
            set
            {
                prodListTable = value;
            }
        }

        private DataTable marketListTable = null;
        public DataTable MarketListTable
        {
            get
            {
                return marketListTable;
            }
            set
            {
                marketListTable = value;
                marketListTable.TableName = "marketListTable";
            }
        }

        private List<string> marketPriceList = null;
        public List<string> MarketPriceList
        {
            get
            {
                if (marketListTable != null)
                {
                    marketPriceList = marketListTable.AsEnumerable().Select(d => d.Field<string>("productCode")).ToList<string>();
                    GOSTradeStation.marketPriceData.MarketPriceList = marketPriceList;
                }
                return marketPriceList;
            }
            set
            {
                marketPriceList = value;
            }
        }

        private static DataTable marketTreeTable = null;
        public DataTable MarketTreeTable
        {
            get
            {
                if (instListTable != null && prodListTable != null && marketTreeTable == null)
                {
                    //Treeview
                    marketTreeTable = new DataTable();
                    marketTreeTable.Columns.Add("marketCode");
                    marketTreeTable.Columns.Add("instmntCode");
                    marketTreeTable.Columns.Add("productCode");
                    marketTreeTable.Columns.Add("prodType");
                    marketTreeTable.Columns.Add("contraceSize");
                    marketTreeTable.Columns.Add("decInPrice");
                    marketTreeTable.Columns.Add("ccy");
                    marketTreeTable.Columns.Add("callPut");
                    marketTreeTable.Columns.Add("productName");
                    marketTreeTable.Columns.Add("expiryDate");
                    var query = from a in prodListTable.AsEnumerable()
                                join b in instListTable.AsEnumerable()
                                on a.Field<string>("instmntCode") equals b.Field<string>("instmntCode") into g
                                from b in g.DefaultIfEmpty()
                                where (b != null)
                                select new
                                {
                                    marketCode = b.Field<string>("marketCode"),
                                    instmntCode = a.Field<string>("instmntCode"),
                                    productCode = a.Field<string>("productCode"),
                                    prodType = a.Field<string>("prodType"),
                                    contraceSize = b.Field<string>("contraceSize"),
                                    //decInPrice = b.Field<string>("decInPrice"),
                                    decInPrice = a.Field<string>("decInPrice"), // moved to productlist, kenlo131003
                                    ccy = b.Field<string>("ccy"),
                                    callPut = a.Field<string>("callPut"),
                                    productName = a.Field<string>("prodName"),
                                    expiryDate = a.Field<string>("expiryDate")
                                };
                    query.ToList().ForEach(q => marketTreeTable.Rows.Add(q.marketCode, q.instmntCode, q.productCode, q.prodType, q.contraceSize, q.decInPrice, q.ccy, q.callPut, q.productName, q.expiryDate));

                    if (GOSTradeStation.marketPriceData == null) return null;
                    GOSTradeStation.marketPriceData.MarketTreeTable = marketTreeTable;
                }
                return marketTreeTable;
            }
            set
            {
                marketTreeTable = value;
            }
        }

        //private ObservableCollection<MarketPriceItem> marktePriceItems = null;
        //public ObservableCollection<MarketPriceItem> MarktePriceItems
        //{
        //    get
        //    {
        //        return marktePriceItems;
        //    }
        //    set
        //    {
        //        marktePriceItems = value;
        //    }
        //}


        //private DataTable priceDepthTable = null;
        //public DataTable PriceDepthTable
        //{
        //    get
        //    {
        //        return priceDepthTable;
        //    }
        //    set
        //    {
        //        priceDepthTable = value;
        //        priceDepthTable.TableName = "priceDepthTable";
        //    }
        //}

        //private ObservableCollection<LongPriceDepthData> priceDepthDataes = null;
        //public ObservableCollection<LongPriceDepthData> PriceDepthDataes
        //{
        //    get
        //    {
        //        return priceDepthDataes;
        //    }
        //    set
        //    {
        //        priceDepthDataes = value;

        //    }
        //}

        //private List<string> longpriceDepthList = null;
        //public List<string> LongPriceDepthList
        //{
        //    get
        //    {
        //        return longpriceDepthList;
        //    }
        //    set
        //    {
        //        longpriceDepthList = value;

        //    }

        //}

        private DataTable tickerTable = null;
        public DataTable TickerTable
        {
            get
            {
                return tickerTable;
            }
            set
            {

                tickerTable = value;
                if (value != null)
                {
                    tickerTable.TableName = "tickerTable";
                }
            }
        }

        private ObservableCollection<TickerData> _TickerDataes = null;
        public ObservableCollection<TickerData> TickerDataes
        {
            get
            {
                return _TickerDataes;
            }
            set
            {
                _TickerDataes = value;
            }
        }


        //private DataTable accoutList = null;
        //public DataTable AccoutList
        //{
        //    get
        //    {
        //        return accoutList;
        //    }
        //    set
        //    {
        //        accoutList = value;

        //    }

        //} 

        public static int contraceSize(string productCode)
        {
            try
            {
                string contraceSize;
                if (marketTreeTable != null)
                {
                    DataRow[] drs = marketTreeTable.Select("productCode='" + productCode + "'");
                    if (drs.Length > 0)
                    {
                        contraceSize = drs[0]["contraceSize"].ToString();
                        return Convert.ToInt32(contraceSize);
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            catch { }
            return 1;
        }

        public static int GetDecInPrice(string productCode)
        {
            try
            {
                if (prodListTable != null)
                {
                    DataRow[] drs = prodListTable.Select("productCode='" + productCode + "'");
                    if (drs.Length > 0)
                    {
                        return (drs[0]["decInPrice"] != null && drs[0]["decInPrice"].ToString() != "") ? Convert.ToInt32(drs[0]["decInPrice"]) : 0;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
            }
            return 0;
        }

        public static bool ExistInProduct(string productCode)
        {
            try
            {
                if (prodListTable != null)
                {
                    DataRow[] drs = prodListTable.Select("productCode='" + productCode + "'");
                    if (drs.Length > 0)
                    {
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public static bool ProductListExists()
        {
            return (prodListTable != null);
        }

        public static string GetProductName(string productCode)
        {
            try
            {
                if (prodListTable != null)
                {
                    DataRow[] drs = prodListTable.Select("productCode='" + productCode + "'");
                    if (drs.Length > 0)
                    {
                        //return drs[0]["prodName"].ToString();
                        string strName = (AppFlag.DefaultLanguage == "en-US") ? drs[0]["prodName"].ToString() : (AppFlag.DefaultLanguage == "zh-CN") ? drs[0]["prodName1"].ToString() : drs[0]["prodName2"].ToString();
                        return (strName.Trim() == "") ? drs[0]["prodName"].ToString() : strName.Trim(); 
                    }
                }
            }
            catch { }
            return "";
        }

        public static string GetProductStrike(string productCode)
        {
            try
            {
                if (prodListTable != null)
                {
                    DataRow[] drs = prodListTable.Select("productCode='" + productCode + "'");
                    if (drs.Length > 0)
                    {
                        return drs[0]["Strike"].ToString();
                    }
                }
            }
            catch { }
            return "";
        }

        public static DataRow GetProductInfo(string productCode)
        {
            try
            {
                if (prodListTable != null)
                {
                    DataRow[] drs = prodListTable.Select("productCode='" + productCode + "'");
                    if (drs.Length > 0)
                    {
                        return drs[0];
                    }
                }
            }
            catch { }
            return null;
        }

        public static DataRow GetMoreProductInfo(string productCode)
        {
            try
            {
                if (instListTable != null && prodListTable != null && marketTreeTable == null)
                {
                    marketTreeTable = new DataTable();
                    marketTreeTable.Columns.Add("marketCode");
                    marketTreeTable.Columns.Add("instmntCode");
                    marketTreeTable.Columns.Add("productCode");
                    marketTreeTable.Columns.Add("prodType");
                    marketTreeTable.Columns.Add("contraceSize");
                    marketTreeTable.Columns.Add("decInPrice");
                    marketTreeTable.Columns.Add("ccy");
                    marketTreeTable.Columns.Add("callPut");
                    marketTreeTable.Columns.Add("productName");
                    marketTreeTable.Columns.Add("expiryDate");
                    var query = from a in prodListTable.AsEnumerable()
                                join b in instListTable.AsEnumerable()
                                on a.Field<string>("instmntCode") equals b.Field<string>("instmntCode") into g
                                from b in g.DefaultIfEmpty()
                                where (b != null)
                                select new
                                {
                                    marketCode = b.Field<string>("marketCode"),
                                    instmntCode = a.Field<string>("instmntCode"),
                                    productCode = a.Field<string>("productCode"),
                                    prodType = a.Field<string>("prodType"),
                                    contraceSize = b.Field<string>("contraceSize"),
                                    //decInPrice = b.Field<string>("decInPrice"),
                                    decInPrice = a.Field<string>("decInPrice"), // moved to productlist, kenlo131003
                                    ccy = b.Field<string>("ccy"),
                                    callPut = a.Field<string>("callPut"),
                                    productName = a.Field<string>("prodName"),
                                    expiryDate = a.Field<string>("expiryDate")
                                };
                    query.ToList().ForEach(q => marketTreeTable.Rows.Add(q.marketCode, q.instmntCode, q.productCode, q.prodType, q.contraceSize, q.decInPrice, q.ccy, q.callPut, q.productName, q.expiryDate));
                }

                if (marketTreeTable != null)
                {
                    DataRow[] drs = marketTreeTable.Select("productCode='" + productCode + "'");
                    if (drs.Length > 0)
                    {
                        return drs[0];
                    }
                }
            }
            catch { }
            return null;
        }

        public static DataRow GetProductInfo(string[] strColumn, string[] strValue)
        {
            try
            {
                if (marketTreeTable != null)
                {
                    string strSQL = "";
                    for  (int i=0;i< strColumn.Length ;i++)
                    {
                        strSQL += strColumn[i] + "='" + strValue[i] + "' and ";
                    }
                    string str = strSQL.Trim().Substring(0, strSQL.Length - 4);
                    DataRow[] drs = marketTreeTable.Select(str);
                    if (drs.Length > 0)
                    {
                        return drs[0];
                    }
                }
            }
            catch { }
            return null;
        }

        public static DataRow[] GetProductInfos(string strSQL)
        {
            try
            {
                if (prodListTable != null)
                {
                    DataRow[] drs = prodListTable.Select(strSQL,"Strike asc");
                    if (drs.Length > 0)
                    {
                        return drs;
                    }
                }
            }
            catch { }
            return null;
        }

        public static List<string> GetMarketCodes()
        {
            if (marketCodeList != null)
            {
                return marketCodeList;
            }
            return null;
        }

        public static List<string> GetInstrCoedes(string marketCode)
        {
            try
            {
                if (instListTable != null)
                {
                    DataRow[] drs = instListTable.Select("marketCode='" + marketCode + "'");
                    if (drs == null) return null;
                    return drs.Select(d => d.Field<string>("instmntCode")).ToList<string>();
                }
            }
            catch { }
            return null;
        }

        public static List<string> GetOptionCoedes(string productCode)
        {
            try
            {
                if (prodListTable != null)
                {
                    DataRow dr = GetProductInfo(productCode);
                    string strInstrmntCode = "";
                    string strEpiryDate = "";
                    if (dr != null)
                    {
                        strInstrmntCode = dr["instmntCode"].ToString();
                        strEpiryDate = Convert.ToDateTime(dr["expiryDate"]).ToString("yyyy/MM");
                    }
                    else
                    {
                        string strSymbol = "";
                        switch (productCode.Substring(3, 1))
                        {
                            case "F":
                                strSymbol = "01";
                                break;
                            case "G":
                                strSymbol = "02";
                                break;
                            case "H":
                                strSymbol = "03";
                                break;
                            case  "J":
                                strSymbol = "04";
                                break;
                            case "K":
                                strSymbol = "05";
                                break;
                            case  "M":
                                strSymbol = "06";
                                break;
                            case "N":
                                strSymbol = "07";
                                break;
                            case "Q":
                                strSymbol = "08";
                                break;
                            case  "U":
                                strSymbol = "09";
                                break;
                            case "V":
                                strSymbol = "10";
                                break;
                            case "X":
                                strSymbol = "11";
                                break;
                            case "Z":
                                strSymbol = "12";
                                break;
                            default:
                                break;
                        }

                        strInstrmntCode = productCode.Substring(0, 3);
                        strEpiryDate = DateTime.Now.Year.ToString().Substring(0, 3) + productCode.Substring(4, 1) + "/" + strSymbol;
                    }
                    DataRow[] drs = GetProductInfos("instmntCode='" + strInstrmntCode + "' and prodType = '2' and expiryDate like '" + strEpiryDate + "%'");
                    if (drs == null) return null;
                    return drs.Select(d => d.Field<string>("productCode")).ToList<string>();
                }
            }
            catch { }
            return null;
        }

        public static string GetProdCodebyCode(string optionCode)
        {
            //FGHJKMNQUVXZ
            DataRow dr = GetProductInfo(optionCode);
            if (dr == null) return "";
            string symbol = "";
            switch (Convert.ToDateTime(dr["expiryDate"]).Month)
            {
                case 1:
                    symbol = "F";
                    break;
                case 2:
                    symbol = "G";
                    break;
                case 3:
                    symbol = "H";
                    break;
                case 4:
                    symbol = "J";
                    break;
                case 5:
                    symbol = "K";
                    break;
                case 6:
                    symbol = "M";
                    break;
                case 7:
                    symbol = "N";
                    break;
                case 8:
                    symbol = "Q";
                    break;
                case 9:
                    symbol = "U";
                    break;
                case 10:
                    symbol = "V";
                    break;
                case 11:
                    symbol = "X";
                    break;
                case 12:
                    symbol = "Z";
                    break;
                default:
                    break;
            }

            string tempCode = dr["instmntCode"].ToString() + symbol + Convert.ToDateTime(dr["expiryDate"]).Year.ToString().Substring(3, 1);
            DataRow dr2 = GetProductInfo(tempCode);
            if (dr2 == null) return "";

            return tempCode;
        }

        public static string GetCcy(string productCode)
        {
            try
            {
                if (marketTreeTable != null)
                {
                    DataRow[] drs = marketTreeTable.Select("productCode='" + productCode + "'");
                    if (drs.Length > 0)
                    {
                        return drs[0]["ccy"].ToString();
                    }
                    else
                    {
                        return "HKD";
                    }
                }
            }
            catch { }
            return "HKD";
        }
        public static double GetRate(string Type)
        {
            switch (Type.ToUpper())
            {
                case "HKD":
                    return 1;
                case "DOLLOR":
                    return 7.1;
                default:
                    return 1;
            }
        }
         
    }

    public class PriceDepthData : INotifyPropertyChanged
    { 
        public string ProdCode { get; set; }
        public int DepInPrice { get; set; }

        private string _Item;
        public string Item
        {
            get { return _Item; }
            set { _Item = value; OnPropertyChanged(new PropertyChangedEventArgs("Item")); }
        }

        private float _B5;
        public float B5
        {
            get { return _B5; }
            set { _B5 = value; OnPropertyChanged(new PropertyChangedEventArgs("B5")); }
        }

        private float _B4;
        public float B4
        {
            get { return _B4; }
            set { _B4 = value; OnPropertyChanged(new PropertyChangedEventArgs("B4")); }
        }
        private float _B3;
        public float B3
        {
            get { return _B3; }
            set { _B3 = value; OnPropertyChanged(new PropertyChangedEventArgs("B3")); }
        }

        private float _B2;
        public float B2
        {
            get { return _B2; }
            set { _B2 = value; OnPropertyChanged(new PropertyChangedEventArgs("B2")); }
        }
        private float _B1;
        public float B1
        {
            get { return _B1; }
            set { _B1 = value; OnPropertyChanged(new PropertyChangedEventArgs("B1")); }
        }

        private float _A5;
        public float A5
        {
            get { return _A5; }
            set { _A5 = value; OnPropertyChanged(new PropertyChangedEventArgs("A5")); }
        }

        private float _A4;
        public float A4
        {
            get { return _A4; }
            set { _A4 = value; OnPropertyChanged(new PropertyChangedEventArgs("A4")); }
        }
        private float _A3;
        public float A3
        {
            get { return _A3; }
            set { _A3 = value; OnPropertyChanged(new PropertyChangedEventArgs("A3")); }
        }

        private float _A2;
        public float A2
        {
            get { return _A2; }
            set { _A2 = value; OnPropertyChanged(new PropertyChangedEventArgs("A2")); }
        }
        private float _A1;
        public float A1
        {
            get { return _A1; }
            set { _A1 = value; OnPropertyChanged(new PropertyChangedEventArgs("A1")); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, (propertyName));
            }
        }
    }

    public class PriceDataViewModel
    {
        public string productCode { get; set; }
        public string productName { get; set; }
        private ObservableCollection<PriceDepthData> _DataList = new ObservableCollection<PriceDepthData>();
        public ObservableCollection<PriceDepthData> DataList
        {
            get { return _DataList; }
            set { _DataList = value; }
        }
        private ObservableCollection<LongPriceDepthData> _DataList2 = new ObservableCollection<LongPriceDepthData>();
        public ObservableCollection<LongPriceDepthData> DataList2
        {
            get { return _DataList2; }
            set { _DataList2 = value; }
        } 

        public int GetPriceDepthData(string prodCode, DataTable dataTable)
        {
            if (dataTable.Rows.Count < 1)
                return -1;
            productCode = prodCode;
            DataRow[] rows = dataTable.Select("productCode='" + prodCode + "'");

            if (rows.Length > 0)
            {
                DataList.Add(new PriceDepthData()
                {
                    Item = "Prc",
                    B5 = Convert.ToInt32(rows[0]["bidPrice5"]),
                    B4 = Convert.ToInt32(rows[0]["bidPrice4"]),
                    B3 = Convert.ToInt32(rows[0]["bidPrice3"]),
                    B2 = Convert.ToInt32(rows[0]["bidPrice2"]),
                    B1 = Convert.ToInt32(rows[0]["bidPrice1"]),
                    A1 = Convert.ToInt32(rows[0]["askPrice1"]),
                    A2 = Convert.ToInt32(rows[0]["askPrice2"]),
                    A3 = Convert.ToInt32(rows[0]["askPrice3"]),
                    A4 = Convert.ToInt32(rows[0]["askPrice4"]),
                    A5 = Convert.ToInt32(rows[0]["askPrice5"]),
                });
                DataList.Add(new PriceDepthData()
                {
                    Item = "Qty",
                    B5 = Convert.ToInt32(rows[0]["bidQty5"]),
                    B4 = Convert.ToInt32(rows[0]["bidQty4"]),
                    B3 = Convert.ToInt32(rows[0]["bidQty3"]),
                    B2 = Convert.ToInt32(rows[0]["bidQty2"]),
                    B1 = Convert.ToInt32(rows[0]["bidQty1"]),
                    A1 = Convert.ToInt32(rows[0]["askQty1"]),
                    A2 = Convert.ToInt32(rows[0]["askQty2"]),
                    A3 = Convert.ToInt32(rows[0]["askQty3"]),
                    A4 = Convert.ToInt32(rows[0]["askQty4"]),
                    A5 = Convert.ToInt32(rows[0]["askQty5"]),
                });


                int i = 0;
                do
                {
                    DataList2.Add(new LongPriceDepthData()
                    {
                        CurrentTime = rows[0][0].ToString(),
                        ProdCode = prodCode,
                        Reserve = "",
                        Bid = Convert.ToInt32(rows[0][2 * i + 2]),
                        BQty = Convert.ToInt32(rows[0][2 * i + 3]),
                        Ask = Convert.ToInt32(rows[0][2 * i + 12]),
                        AQty = Convert.ToInt32(rows[0][2 * i + 13])
                    });
                    i++;
                }
                while (i < 5);
            }

            if (DataList.Count > 0)
            {
                return DataList.Count;
            }
            else
            {
                return -1;
            }
        }

        public void GetPriceDepthData(string prodCode, DataTable dataTable, out ObservableCollection<PriceDepthData> lvData, out ObservableCollection<LongPriceDepthData> gdData)
        {
            lvData = new ObservableCollection<PriceDepthData>();
            gdData = new ObservableCollection<LongPriceDepthData>();

            if (dataTable.Rows.Count < 0) return;

            DataRow[] rows = dataTable.Select("productCode='" + prodCode + "'");
            int depInPrice = MarketPriceData.GetDecInPrice(prodCode);
            if (rows.Length > 0)
            {
                lvData.Add(new PriceDepthData()
                {
                    Item = "Prc",
                    DepInPrice = depInPrice,
                    ProdCode = rows[0]["productCode"].ToString(),
                    B5 = TradeStationTools.ConvertToFloat(rows[0]["bidPrice5"], depInPrice),
                    B4 = TradeStationTools.ConvertToFloat(rows[0]["bidPrice4"], depInPrice),
                    B3 = TradeStationTools.ConvertToFloat(rows[0]["bidPrice3"], depInPrice),
                    B2 = TradeStationTools.ConvertToFloat(rows[0]["bidPrice2"], depInPrice),
                    B1 = (rows[0]["bidPrice1"].ToString() == "AO" || rows[0]["bidPrice1"].ToString() == AppFlag .InvalidNum .ToString ()) ? Convert.ToInt32(AppFlag.AONum) : TradeStationTools.ConvertToFloat(rows[0]["bidPrice1"], depInPrice),
                    A1 = (rows[0]["askPrice1"].ToString() == "AO" || rows[0]["askPrice1"].ToString() == AppFlag.InvalidNum.ToString()) ? Convert.ToInt32(AppFlag.AONum) : TradeStationTools.ConvertToFloat(rows[0]["askPrice1"], depInPrice),
                    A2 = TradeStationTools.ConvertToFloat(rows[0]["askPrice2"], depInPrice),
                    A3 = TradeStationTools.ConvertToFloat(rows[0]["askPrice3"], depInPrice),
                    A4 = TradeStationTools.ConvertToFloat(rows[0]["askPrice4"], depInPrice),
                    A5 = TradeStationTools.ConvertToFloat(rows[0]["askPrice5"], depInPrice),
                });
                lvData.Add(new PriceDepthData()
                {
                    Item = "Qty",
                    ProdCode = rows[0]["productCode"].ToString(),
                    //B5 = TradeStationTools.ConvertToInt(rows[0]["bidQty5"], -1),
                    //B4 = TradeStationTools.ConvertToInt(rows[0]["bidQty4"], -1),
                    //B3 = TradeStationTools.ConvertToInt(rows[0]["bidQty3"], -1),
                    //B2 = TradeStationTools.ConvertToInt(rows[0]["bidQty2"], -1),
                    //B1 = TradeStationTools.ConvertToInt(rows[0]["bidQty1"], -1),
                    //A1 = TradeStationTools.ConvertToInt(rows[0]["askQty1"], -1),
                    //A2 = TradeStationTools.ConvertToInt(rows[0]["askQty2"], -1),
                    //A3 = TradeStationTools.ConvertToInt(rows[0]["askQty3"], -1),
                    //A4 = TradeStationTools.ConvertToInt(rows[0]["askQty4"], -1),
                    //A5 = TradeStationTools.ConvertToInt(rows[0]["askQty5"], -1),
                    B5 = TradeStationTools.ConvertToInt32(rows[0]["bidQty5"]),
                    B4 = TradeStationTools.ConvertToInt32(rows[0]["bidQty4"]),
                    B3 = TradeStationTools.ConvertToInt32(rows[0]["bidQty3"]),
                    B2 = TradeStationTools.ConvertToInt32(rows[0]["bidQty2"]),
                    B1 = TradeStationTools.ConvertToInt32(rows[0]["bidQty1"]),
                    A1 = TradeStationTools.ConvertToInt32(rows[0]["askQty1"]),
                    A2 = TradeStationTools.ConvertToInt32(rows[0]["askQty2"]),
                    A3 = TradeStationTools.ConvertToInt32(rows[0]["askQty3"]),
                    A4 = TradeStationTools.ConvertToInt32(rows[0]["askQty4"]),
                    A5 = TradeStationTools.ConvertToInt32(rows[0]["askQty5"]),
                }); 

                int i = 0;
                do
                {
                    gdData.Add(new LongPriceDepthData()
                    {
                        CurrentTime = rows[0][0].ToString(),
                        ProdCode = rows[0]["productCode"].ToString(),
                        DepInPrice = depInPrice,
                        Reserve = "",
                        ID = i,
                        Bid = ((rows[0][2 * i + 2].ToString() == "AO" || rows[0][2 * i + 2].ToString() == AppFlag .InvalidNum .ToString()) && i == 0) ? AppFlag.AONum : TradeStationTools.ConvertToFloat(rows[0][2 * i + 2], depInPrice),
                        BQty = TradeStationTools.ConvertToInt32(rows[0][2 * i + 3]),
                        Ask = ((rows[0][2 * i + 12].ToString() == "AO" || rows[0][2 * i + 12].ToString() == AppFlag.InvalidNum.ToString()) && i == 0) ? AppFlag.AONum : TradeStationTools.ConvertToFloat(rows[0][2 * i + 12], depInPrice),
                        AQty = TradeStationTools.ConvertToInt32(rows[0][2 * i + 13])
                    });
                    i++;
                }
                while (i < 5);
            }
        }
    }

    public class LongPriceDepthData : INotifyPropertyChanged
    {
       public int DepInPrice { get; set; }

        private string _ProdCode;
        public string ProdCode
        {
            get { return _ProdCode; }
            set { _ProdCode = value; OnPropertyChanged(new PropertyChangedEventArgs("ProdCode")); }
        }

        private string _CurrentTime;
        public string CurrentTime
        {
            get { return _CurrentTime; }
            set { _CurrentTime = value; OnPropertyChanged(new PropertyChangedEventArgs("CurrentTime")); }
        }
        private string _Reserve;
        public string Reserve
        {
            get { return _Reserve; }
            set { _Reserve = value; OnPropertyChanged(new PropertyChangedEventArgs("Reserve")); }
        }

        private int _ID;
        public int ID
        {
            get { return _ID; }
            set { _ID = value; OnPropertyChanged(new PropertyChangedEventArgs("ID")); }
        }

        private float _Bid;
        public float Bid
        {
            get { return _Bid; }
            set { _Bid = value; OnPropertyChanged(new PropertyChangedEventArgs("Bid")); }
        }

        private int _BQty;
        public int BQty
        {
            get { return _BQty; }
            set { _BQty = value; OnPropertyChanged(new PropertyChangedEventArgs("BQty")); }
        }

        private float _Ask;
        public float Ask
        {
            get { return _Ask; }
            set { _Ask = value; OnPropertyChanged(new PropertyChangedEventArgs("Ask")); }
        }
        private int _AQty;
        public int AQty
        {
            get { return _AQty; }
            set { _AQty = value; OnPropertyChanged(new PropertyChangedEventArgs("AQty")); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, (propertyName));
            }
        }

    }
    public class LongPriceDepthViewModel
    {
        public List<LongPriceDepthData> longPriceDepthData { get; set; }

        public List<LongPriceDepthData> GetLongPriceDepthData(string prodCode, TradeStationComm.infoClass.LongPriceDepth.LongPriceDepthItem item)
        {
            longPriceDepthData = new List<LongPriceDepthData>();
            int count = Convert.ToInt32(item.AllItem[2]);
            int i = 0;
            do
            {
                longPriceDepthData.Add(new LongPriceDepthData()
                {
                    CurrentTime = item.AllItem[0].ToString(),
                    ProdCode = prodCode,
                    Reserve = item.AllItem[5 * i + 3].ToString(),
                    Bid = Convert.ToInt32(item.AllItem[5 * i + 4]),
                    BQty = Convert.ToInt32(item.AllItem[5 * i + 5]),
                    Ask = Convert.ToInt32(item.AllItem[5 * i + 6]),
                    AQty = Convert.ToInt32(item.AllItem[5 * i + 7])
                });
                i++;
            }
            while (i < 5);

            if (longPriceDepthData.Count > 0)
            {
                return longPriceDepthData;
            }
            else
            {
                return longPriceDepthData;
            }

        }
    }

    public class TickerData
    {
        public int DepInPrice { get; set; }
        public string productCode { get; set; }
        public string qty { get; set; }
        public string price { get; set; }
        public DateTime logTime { get; set; }
        public string dealSrc { get; set; }

        //public int  DepInPrice { get; set; }
        //private string _productCode;
        //public string productCode
        //{
        //    get { return _productCode; }
        //    set { _productCode = value; OnPropertyChanged(new PropertyChangedEventArgs("productCode")); }
        //}

        //private string _qty;
        //public string qty
        //{
        //    get { return _qty; }
        //    set { _qty = value; OnPropertyChanged(new PropertyChangedEventArgs("qty")); }
        //}
        //private string _price;
        //public string price
        //{
        //    get { return _price; }
        //    set { _price = value; OnPropertyChanged(new PropertyChangedEventArgs("price")); }
        //}

        //private string _logTime;
        //public string logTime
        //{
        //    get { return _logTime; }
        //    set { _logTime = value; OnPropertyChanged(new PropertyChangedEventArgs("logTime")); }
        //}
        //private string _dealSrc;
        //public string dealSrc
        //{
        //    get { return _dealSrc; }
        //    set { _dealSrc = value; OnPropertyChanged(new PropertyChangedEventArgs("dealSrc")); }
        //}

        //public event PropertyChangedEventHandler PropertyChanged;
        //protected void OnPropertyChanged(PropertyChangedEventArgs propertyName)
        //{
        //    PropertyChangedEventHandler handler = this.PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, (propertyName));
        //    }
        //}

    }
    public class TickerViewModel
    {
        public string productCode { get; set; }
        public string productName { get; set; }
        //public ObservableCollection <TickerData> TickerList { get; set; }
        //public ObservableCollection<TickerData> TickerList5 { get; set; }

        public ObservableCollection<TickerData> TickerList = new ObservableCollection<TickerData>();
        public ObservableCollection<TickerData> TickerList5 = new ObservableCollection<TickerData>();

        public int GetTickerData(string prodCode, DataTable dataTable)
        {

            if (dataTable.Rows.Count < 1)
                return -1;
            // TickerList = new ObservableCollection<TickerData>();
            productCode = prodCode;// "HSIU2";
            productName = MarketPriceData.GetProductName(prodCode);
            int depInPrice = MarketPriceData.GetDecInPrice(prodCode);
            try
            {
                DataRow[] rows = dataTable.Select("productCode='" + productCode + "'");

                foreach (DataRow dr in rows)
                {
                    TickerList.Add(new TickerData()
                    {
                        DepInPrice = depInPrice,
                        productCode = dr["productCode"].ToString(),
                        qty = dr["qty"].ToString(),
                        price = TradeStationTools.ConvertToFormatStringNull(dr["price"], depInPrice),
                        logTime = TradeStationTools.getDateTimeFromUnixTime(dr["logTime"].ToString()),
                        dealSrc = dr["dealSrc"].ToString()
                    });
                }

                TickerList.Reverse();
                int i = 0;
                foreach (TickerData item in TickerList)
                {
                    TickerList5.Add(item);
                    i++;
                    if (i > 5)
                    {
                        break;
                    }
                }
                //  TickerList5 = TickerList.Take(5);
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + exp.ToString());
            }

            if (TickerList.Count > 0)
            {
                return TickerList.Count;
            }
            else
            {
                return -1;
            }
        }

    }

    public class MarginCheckAEData : INotifyPropertyChanged
    {
        private String _AE;
        public String AE
        {
            get { return _AE; }
            set { _AE = value; OnPropertyChanged(new PropertyChangedEventArgs("AE")); }
        }

        private String _ccy;
        public String Ccy
        {
            get { return _ccy; }
            set { _ccy = value; OnPropertyChanged(new PropertyChangedEventArgs("Ccy")); }
        }
        private double _call;
        public double Call
        {
            get { return _call; }
            set { _call = value; OnPropertyChanged(new PropertyChangedEventArgs("Call")); }
        }
        private String creadit;
        public String Creadit
        {
            get { return creadit; }
            set { creadit = value; OnPropertyChanged(new PropertyChangedEventArgs("Creadit")); }
        }
        private double imargin;
        public double IMargin
        {
            get { return imargin; }
            set { imargin = value; OnPropertyChanged(new PropertyChangedEventArgs("IMargin")); }
        }
        private double mmargin;
        public double MMargin
        {
            get { return mmargin; }
            set { mmargin = value; OnPropertyChanged(new PropertyChangedEventArgs("MMargin")); }
        }
        private double cash;
        public double Cash
        {
            get { return cash; }
            set { cash = value; OnPropertyChanged(new PropertyChangedEventArgs("Cash")); }
        }
        private String loanLimit;
        public String LoanLimit
        {
            get { return loanLimit; }
            set { loanLimit = value; OnPropertyChanged(new PropertyChangedEventArgs("LoanLimit")); }
        }
        private String netEq;
        public String NetEq
        {
            get { return netEq; }
            set { netEq = value; OnPropertyChanged(new PropertyChangedEventArgs("NetEq")); }
        }
        private double pl;
        public double PL
        {
            get { return pl; }
            set { pl = value; OnPropertyChanged(new PropertyChangedEventArgs("PL")); }
        }
        private String fee;
        public String Fee
        {
            get { return fee; }
            set { fee = value; OnPropertyChanged(new PropertyChangedEventArgs("Fee")); }
        }
        private String totalEq;
        public String TotalEq
        {
            get { return totalEq; }
            set { totalEq = value; OnPropertyChanged(new PropertyChangedEventArgs("TotalEq")); }
        }
        private String marketValue;
        public String MarketValue
        {
            get { return marketValue; }
            set { marketValue = value; OnPropertyChanged(new PropertyChangedEventArgs("MarketValue")); }
        }
        private String maxMgn;
        public String MaxMgn
        {
            get { return maxMgn; }
            set { maxMgn = value; OnPropertyChanged(new PropertyChangedEventArgs("MaxMgn")); }
        }
        private String pos;
        public String Pos
        {
            get { return pos; }
            set { pos = value; OnPropertyChanged(new PropertyChangedEventArgs("Pos")); }
        }

        private String orders;
        public String Orders
        {
            get { return orders; }
            set { orders = value; OnPropertyChanged(new PropertyChangedEventArgs("Orders")); }
        }

        private DateTime updateTime;
        public DateTime UpdateTime
        {
            get { return updateTime; }
            set { updateTime = value; OnPropertyChanged(new PropertyChangedEventArgs("UpdateTime")); }
        }

        private double mLevel;
        public double MLevel
        {
            get { return mLevel; }
            set { mLevel = value; OnPropertyChanged(new PropertyChangedEventArgs("MLevel")); }
        }
        private String rawMarginLevel;
        public String RawMarginLevel
        {
            get { return rawMarginLevel; }
            set { rawMarginLevel = value; OnPropertyChanged(new PropertyChangedEventArgs("RawMarginLevel")); }
        }
        private double buyPower;
        public double BuyPower
        {
            get { return buyPower; }
            set { buyPower = value; OnPropertyChanged(new PropertyChangedEventArgs("BuyPower")); }
        }

        private double tradeLimit;
        public double TradeLimit
        {
            get { return tradeLimit; }
            set { tradeLimit = value; OnPropertyChanged(new PropertyChangedEventArgs("TradeLimit")); }
        }
        private String marginClass;
        public String MarginClass
        {
            get { return marginClass; }
            set { marginClass = value; OnPropertyChanged(new PropertyChangedEventArgs("MarginClass")); }
        }
        private String tradeClass;
        public String TradeClass
        {
            get { return tradeClass; }
            set { tradeClass = value; OnPropertyChanged(new PropertyChangedEventArgs("TradeClass")); }
        }
        private double nav;
        public double Nav
        {
            get { return nav; }
            set { nav = value; OnPropertyChanged(new PropertyChangedEventArgs("Nav")); }
        }

        private double unpresented;
        public double Unpresented
        {
            get { return unpresented; }
            set { unpresented = value; OnPropertyChanged(new PropertyChangedEventArgs("Unpresented")); }
        }
        private double avFund;
        public double AvFund
        {
            get { return avFund; }
            set { avFund = value; OnPropertyChanged(new PropertyChangedEventArgs("AvFund")); }
        }
        private String ctrlLevel;
        public String CtrlLevel
        {
            get { return ctrlLevel; }
            set { ctrlLevel = value; OnPropertyChanged(new PropertyChangedEventArgs("CtrlLevel")); }
        }

        private String accName;
        public String AccName
        {
            get { return accName; }
            set { accName = value; OnPropertyChanged(new PropertyChangedEventArgs("AccName")); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, (propertyName));
            }
        }

    }
    public class MarginCheckAccData : MarginCheckAEData
    {
        private String accNo;
        public String AccNo
        {
            get { return accNo; }
            set { accNo = value; OnPropertyChanged(new PropertyChangedEventArgs("AccNo")); }
        }

    }
    public class MarginCheckViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<MarginCheckAEData> _MarginCheckAEDataes;
        public ObservableCollection<MarginCheckAEData> MarginCheckAEDataes
        {
            get { return _MarginCheckAEDataes; }
            set { _MarginCheckAEDataes = value; OnPropertyChanged(new PropertyChangedEventArgs("MarginCheckAEDataes")); }
        }

        private ObservableCollection<MarginCheckAccData> _MarginCheckAccDataes;
        public ObservableCollection<MarginCheckAccData> MarginCheckAccDataes
        {
            get { return _MarginCheckAccDataes; }
            set { _MarginCheckAccDataes = value; OnPropertyChanged(new PropertyChangedEventArgs("MarginCheckAccDataes")); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, (propertyName));
            }
        }

        public void MarginCheckViewModels(ObservableCollection<MarginCheckAccData> data)
        {
            MarginCheckAEDataes = new ObservableCollection<MarginCheckAEData>();
            MarginCheckAEData drAE;

            foreach (MarginCheckAEData dr in data)
            {
                drAE = new MarginCheckAEData();
                var list = (from filter in MarginCheckAEDataes
                            where filter.AE.Equals(dr.AE)
                            select filter).ToList();

                if (list.Count() == 0)
                {
                    drAE = new MarginCheckAEData();
                    drAE.AE = dr.AE;
                    drAE.Ccy = dr.Ccy;
                    drAE.Call = dr.Call;
                    drAE.Creadit = dr.Creadit;
                    drAE.IMargin = dr.IMargin;
                    drAE.MMargin = dr.MMargin;
                    drAE.Cash = dr.Cash;
                    drAE.LoanLimit = dr.LoanLimit;
                    drAE.NetEq = dr.NetEq;
                    drAE.PL = dr.PL;
                    drAE.Fee = dr.Fee;
                    drAE.TotalEq = dr.TotalEq;
                    drAE.MarketValue = dr.MarketValue;
                    drAE.MaxMgn = dr.MaxMgn;
                    drAE.Pos = dr.Pos;
                    drAE.Orders = dr.Orders;
                    drAE.UpdateTime = dr.UpdateTime;

                    drAE.MLevel = dr.MLevel;
                    drAE.RawMarginLevel = dr.RawMarginLevel;
                    drAE.BuyPower = dr.BuyPower;
                    drAE.TradeLimit = dr.TradeLimit;
                    drAE.MarginClass = dr.MarginClass;
                    drAE.TradeClass = dr.TradeClass;
                    drAE.Nav = dr.Nav;
                    drAE.Unpresented = dr.Unpresented;
                    drAE.AvFund = dr.AvFund;
                    drAE.CtrlLevel = dr.CtrlLevel;

                    _MarginCheckAEDataes.Add(drAE);
                }
                else
                {
                    var list2 = (from filter in MarginCheckAEDataes
                                 where filter.AE.Equals(dr.AE)
                                 select filter).ToList();

                    list2[0].Call = list2[0].Call + dr.Call;
                    list2[0].IMargin = list2[0].IMargin + dr.IMargin;
                    list2[0].MMargin = list2[0].MMargin + dr.MMargin;
                    list2[0].Cash = list2[0].Cash + dr.Cash;
                    list2[0].PL = list2[0].PL + dr.PL;
                    list2[0].BuyPower = list2[0].BuyPower + dr.BuyPower;
                    list2[0].Nav = list2[0].Nav + dr.Nav;
                    list2[0].UpdateTime = (list2[0].UpdateTime > dr.UpdateTime) ? list2[0].UpdateTime : dr.UpdateTime;
                }
            }

            MarginCheckAccDataes = new ObservableCollection<MarginCheckAccData>();
            if (MarginCheckAEDataes.Count() > 0)
            {
                foreach (MarginCheckAccData dr3 in data)
                {
                    if (dr3.AE == MarginCheckAEDataes[0].AE)
                    {
                        MarginCheckAccDataes.Add(dr3);
                    }
                }
            }
            else
            {

            }

        }
    }
     
    public class TradeSimply
    {
        public String accNo { get; set; }

        public String report { get; set; }

        public String initiator { get; set; }

        public String productCode { get; set; }

        public String osBQty { get; set; }

        public String osSQty { get; set; }

        public int DepInPrice { get; set; }

        public String tradedPrice { get; set; }

        public DateTime UpdateTime { get; set; }

        public String recNo { get; set; }

        public String refNo { get; set; }

        public String tradeNo { get; set; }

        public String orderNo { get; set; }

        public String extOrderNo { get; set; }

        public String status { get; set; }

        //public String _report { get; set; }
        //public string report
        //{
        //    get { return _report; }
        //    set { _report = value; OnPropertyChanged(new PropertyChangedEventArgs("report")); }
        //} 
        //public String _initiator { get; set; }
        //public string initiator
        //{
        //    get { return _initiator; }
        //    set { _initiator = value; OnPropertyChanged(new PropertyChangedEventArgs("initiator")); }
        //}
        //public String _productCode { get; set; }
        //public string productCode
        //{
        //    get { return _productCode; }
        //    set { _productCode = value; OnPropertyChanged(new PropertyChangedEventArgs("productCode")); }
        //}
        //public String _osBQty { get; set; }
        //public string osBQty
        //{
        //    get { return _osBQty; }
        //    set { _osBQty = value; OnPropertyChanged(new PropertyChangedEventArgs("osBQty")); }
        //}
        //public String _osSQty { get; set; }
        //public string osSQty
        //{
        //    get { return _osSQty; }
        //    set { _osSQty = value; OnPropertyChanged(new PropertyChangedEventArgs("osSQty")); }
        //}
        //public String _tradedPrice { get; set; }
        //public string tradedPrice
        //{
        //    get { return _tradedPrice; }
        //    set { _tradedPrice = value; OnPropertyChanged(new PropertyChangedEventArgs("tradedPrice")); }
        //}
        //public String _UpdateTime { get; set; }
        //public string UpdateTime
        //{
        //    get { return _UpdateTime; }
        //    set { _UpdateTime = value; OnPropertyChanged(new PropertyChangedEventArgs("UpdateTime")); }
        //}
        //public String _recNo { get; set; }
        //public string recNo
        //{
        //    get { return _recNo; }
        //    set { _recNo = value; OnPropertyChanged(new PropertyChangedEventArgs("recNo")); }
        //}
        //public String _refNo { get; set; }
        //public string refNo
        //{
        //    get { return _refNo; }
        //    set { _refNo = value; OnPropertyChanged(new PropertyChangedEventArgs("refNo")); }
        //}
        //public String _tradeNo { get; set; }
        //public string tradeNo
        //{
        //    get { return _tradeNo; }
        //    set { _tradeNo = value; OnPropertyChanged(new PropertyChangedEventArgs("tradeNo")); }
        //}
        //public String _orderNo { get; set; }
        //public string orderNo
        //{
        //    get { return _orderNo; }
        //    set { _orderNo = value; OnPropertyChanged(new PropertyChangedEventArgs("orderNo")); }
        //}
        //public String _extOrderNo { get; set; }
        //public string extOrderNo
        //{
        //    get { return _extOrderNo; }
        //    set { _extOrderNo = value; OnPropertyChanged(new PropertyChangedEventArgs("extOrderNo")); }
        //} 
        //public event PropertyChangedEventHandler PropertyChanged;
        //protected void OnPropertyChanged(PropertyChangedEventArgs propertyName)
        //{
        //    PropertyChangedEventHandler handler = this.PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, (propertyName));
        //    }
        //}
    }
    public class TradeOrderGroup : INotifyPropertyChanged
    {
        //public String report { get; set; }
        private String _report;
        public String report
        {
            get { return _report; }
            set { _report = value; OnPropertyChanged(new PropertyChangedEventArgs("report")); }
        }

        public String accNo { get; set; }
        public String productCode { get; set; }
        public int DepInPrice { get; set; }
        public String osBQty { get; set; }
        public String osSQty { get; set; }
        public String bs { get; set; }
        public String avgTrdPrc { get; set; }
        public int tradeQty { get; set; }
        public String orderNo { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, (propertyName));
            }
        }
    }

    public class MarketPriceItem : INotifyPropertyChanged
    {
        private DateTime? _Datetime;
        public DateTime? Datetime
        {
            get { return _Datetime; }
            set { _Datetime = value; OnPropertyChanged(new PropertyChangedEventArgs("Datetime")); }
        }

        private String _ProductCode;
        public String ProductCode
        {
            get { return _ProductCode; }
            set
            {
                _ProductCode = value;
                _ProductName = MarketPriceData.GetProductName(_ProductCode);

                OnPropertyChanged(new PropertyChangedEventArgs("productCode"));
            }
        }
        private String _ProductName;
        public String ProductName
        {
            get { return _ProductName; }
            set { _ProductName = value; OnPropertyChanged(new PropertyChangedEventArgs("ProductName")); }
        }
        private DateTime? _Expiry;
        public DateTime? Expiry
        {
            get { return _Expiry; }
            set { _Expiry = value; OnPropertyChanged(new PropertyChangedEventArgs("Expiry")); }
        }
        private String _ProductStatus;
        public String ProductStatus
        {
            get { return _ProductStatus; }
            set { _ProductStatus = value; OnPropertyChanged(new PropertyChangedEventArgs("ProductStatus")); }
        }
        private String _BQty;
        public String BQty
        {
            get { return _BQty; }
            set { _BQty = value; OnPropertyChanged(new PropertyChangedEventArgs("BQty")); }
        }
        private String _Bid;
        public String Bid
        {
            get { return _Bid; }
            set { _Bid = value; OnPropertyChanged(new PropertyChangedEventArgs("Bid")); }
        }
        private String _Ask;
        public String Ask
        {
            get { return _Ask; }
            set { _Ask = value; OnPropertyChanged(new PropertyChangedEventArgs("Ask")); }
        }
        private String _AQty;
        public String AQty
        {
            get { return _AQty; }
            set { _AQty = value; OnPropertyChanged(new PropertyChangedEventArgs("AQty")); }
        }
        private String _Last;
        public String Last
        {
            get { return _Last; }
            set { _Last = value; OnPropertyChanged(new PropertyChangedEventArgs("Last")); }
        }
        private String _EP;
        public String EP
        {
            get { return _EP; }
            set { _EP = value; OnPropertyChanged(new PropertyChangedEventArgs("EP")); }
        }
        private String _LQty;
        public String LQty
        {
            get { return _LQty; }
            set { _LQty = value; OnPropertyChanged(new PropertyChangedEventArgs("LQty")); }
        }
        private String _Change;
        public String Change
        {
            get { return _Change; }
            set { _Change = value; OnPropertyChanged(new PropertyChangedEventArgs("Change")); }
        }
        private String _ChangePer;
        public String ChangePer
        {
            get { return _ChangePer; }
            set { _ChangePer = value; OnPropertyChanged(new PropertyChangedEventArgs("ChangePer")); }
        }
        private String _Volume;
        public String Volume
        {
            get { return _Volume; }
            set { _Volume = value; OnPropertyChanged(new PropertyChangedEventArgs("Volume")); }
        }
        private String _High;
        public String High
        {
            get { return _High; }
            set { _High = value; OnPropertyChanged(new PropertyChangedEventArgs("High")); }
        }
        private String _Low;
        public String Low
        {
            get { return _Low; }
            set { _Low = value; OnPropertyChanged(new PropertyChangedEventArgs("Low")); }
        }
        private String _Open;
        public String Open
        {
            get { return _Open; }
            set { _Open = value; OnPropertyChanged(new PropertyChangedEventArgs("Open")); }
        }
        private String _PreClose;
        public String PreClose
        {
            get { return _PreClose; }
            set { _PreClose = value; OnPropertyChanged(new PropertyChangedEventArgs("PreClose")); }
        }
        private DateTime? _CloseDate;
        public DateTime? CloseDate
        {
            get { return _CloseDate; }
            set { _CloseDate = value; OnPropertyChanged(new PropertyChangedEventArgs("CloseDate")); }
        }
        private String _Strike;
        public String Strike
        {
            get { return _Strike; }
            set { _Strike = value; OnPropertyChanged(new PropertyChangedEventArgs("Strike")); }
        }

        private String _Pos;
        public String Pos
        {
            get { return _Pos; }
            set { _Pos = value; OnPropertyChanged(new PropertyChangedEventArgs("Pos")); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, (propertyName));
            }
        }
    }

    public class MasterOptionItem : MarketPriceItem
    {
        private String _ProductCode;
        public String ProductCodeP
        {
            get { return _ProductCode; }
            set
            {
                _ProductCode = value;
                _ProductName = MarketPriceData.GetProductName(_ProductCode);

                OnPropertyChanged(new PropertyChangedEventArgs("productCodeP"));
            }
        }
        private String _ProductName;
        public String ProductNameP
        {
            get { return _ProductName; }
            set { _ProductName = value; OnPropertyChanged(new PropertyChangedEventArgs("ProductNameP")); }
        }
        private String _BQty;
        public String BQtyP
        {
            get { return _BQty; }
            set { _BQty = value; OnPropertyChanged(new PropertyChangedEventArgs("BQtyP")); }
        }
        private String _Bid;
        public String BidP
        {
            get { return _Bid; }
            set { _Bid = value; OnPropertyChanged(new PropertyChangedEventArgs("BidP")); }
        }
        private String _Ask;
        public String AskP
        {
            get { return _Ask; }
            set { _Ask = value; OnPropertyChanged(new PropertyChangedEventArgs("AskP")); }
        }
        private String _AQty;
        public String AQtyP
        {
            get { return _AQty; }
            set { _AQty = value; OnPropertyChanged(new PropertyChangedEventArgs("AQtyP")); }
        }
        private String _Last;
        public String LastP
        {
            get { return _Last; }
            set { _Last = value; OnPropertyChanged(new PropertyChangedEventArgs("LastP")); }
        }
        private String _Volume;
        public String VolumeP
        {
            get { return _Volume; }
            set { _Volume = value; OnPropertyChanged(new PropertyChangedEventArgs("VolumeP")); }
        }
        private String _High;
        public String HighP
        {
            get { return _High; }
            set { _High = value; OnPropertyChanged(new PropertyChangedEventArgs("HighP")); }
        }
        private String _Low;
        public String LowP
        {
            get { return _Low; }
            set { _Low = value; OnPropertyChanged(new PropertyChangedEventArgs("LowP")); }
        }
        private String _PreClose;
        public String PreCloseP
        {
            get { return _PreClose; }
            set { _PreClose = value; OnPropertyChanged(new PropertyChangedEventArgs("PreCloseP")); }
        }
        private String _Strike;
        public String StrikeP
        {
            get { return _Strike; }
            set { _Strike = value; OnPropertyChanged(new PropertyChangedEventArgs("StrikeP")); }
        }

        private String _Pos;
        public String PosP
        {
            get { return _Pos; }
            set { _Pos = value; OnPropertyChanged(new PropertyChangedEventArgs("PosP")); }
        }
    }

    public class OptionMasterViewModel : INotifyPropertyChanged
    {
        public string productCode { get; private set; }
        public List<string> lsProdCode { get; private set; }

        //        private string _productCode;
        //        public string productCode
        //        {
        //            get { return _productCode; }
        //            set { _productCode = value; OnPropertyChanged(new PropertyChangedEventArgs("productCode")); }
        //        }
        //
        //        private string _productName;
        //        public string productName
        //        {
        //            get { return _productCode; }
        //            set { _productName = value; OnPropertyChanged(new PropertyChangedEventArgs("productName")); }
        //        }
        //        public ObservableCollection<MarketPriceItem> MarketPriceItems = new ObservableCollection<MarketPriceItem>();
        //        public ObservableCollection<MarketPriceItem> MarketPriceItemList = new ObservableCollection<MarketPriceItem>();

        private MarketPriceItem _marketPriceItem;
        public MarketPriceItem marketPriceItem
        {
            get { return _marketPriceItem; }
            set { _marketPriceItem = value; OnPropertyChanged(new PropertyChangedEventArgs("marketPriceItem")); }
        }

        private ObservableCollection<MarketPriceItem> _marketPriceItems;
        public ObservableCollection<MarketPriceItem> marketPriceItems
        {
            get { return _marketPriceItems; }
            set { _marketPriceItems = value; OnPropertyChanged(new PropertyChangedEventArgs("marketPriceItems")); }
        }

        private ObservableCollection<MasterOptionItem> _masterOptionItems;
        public ObservableCollection<MasterOptionItem> masterOptionItems
        {
            get { return _masterOptionItems; }
            set { _masterOptionItems = value; OnPropertyChanged(new PropertyChangedEventArgs("masterOptionItems")); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, (propertyName));
            }
        }

        public bool GetOptionMasterData(string prodCode)
        {
           //  int depInPrice = MarketPriceData.GetDecInPrice(prodCode);
            try
            {
                lsProdCode = MarketPriceData.GetOptionCoedes(prodCode);

                marketPriceItem = new MarketPriceItem();
                marketPriceItem.ProductCode = MarketPriceData.GetProdCodebyCode(prodCode);
                marketPriceItem.ProductName = MarketPriceData.GetProductName(marketPriceItem.ProductCode);
                marketPriceItem.Strike = marketPriceItem.ProductCode;

                productCode = marketPriceItem.ProductCode;

                if (marketPriceItems == null)
                {
                    marketPriceItems = new ObservableCollection<MarketPriceItem>();
                }
                else
                {
                    marketPriceItems.Clear();
                }
                marketPriceItems.Add(_marketPriceItem);

                if (masterOptionItems == null)
                {
                    masterOptionItems = new ObservableCollection<MasterOptionItem>();
                }
                else
                {
                    masterOptionItems.Clear();
                }
                MasterOptionItem drMasterOptionItem;
                string[] arrCALL = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };
                string[] arrPUT = { "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X" };
                if (lsProdCode == null) return false ;
                foreach (string code in lsProdCode)
                {
                    if (code == this.productCode) continue;

                    string symbol = code.Substring(code.Length - 2, 1).ToUpper();
                    if (arrCALL.Contains(symbol))
                    {
                        drMasterOptionItem = new MasterOptionItem();
                        drMasterOptionItem.ProductCode = code;
                        drMasterOptionItem.Strike = MarketPriceData.GetProductStrike(drMasterOptionItem.ProductCode);
                        masterOptionItems.Add(drMasterOptionItem);
                    }
                    else if (arrPUT.Contains(symbol))
                    {
                        string symbol2 = "";
                        switch (symbol)
                        {
                            case "M":
                                symbol2 = "A";
                                break;
                            case "N":
                                symbol2 = "B";
                                break;
                            case "O":
                                symbol2 = "C";
                                break;
                            case "P":
                                symbol2 = "D";
                                break;
                            case "Q":
                                symbol2 = "E";
                                break;
                            case "R":
                                symbol2 = "F";
                                break;
                            case "S":
                                symbol2 = "G";
                                break;
                            case "T":
                                symbol2 = "H";
                                break;
                            case "U":
                                symbol2 = "I";
                                break;
                            case "V":
                                symbol2 = "J";
                                break;
                            case "W":
                                symbol2 = "K";
                                break;
                            case "X":
                                symbol2 = "L";
                                break;
                            default:
                                break;
                        }
                        string code2 = code.Substring(0, code.Length - 2) + symbol2 + code.Substring(code.Length - 1, 1).ToUpper();
                        drMasterOptionItem = masterOptionItems.FirstOrDefault(d => (d.ProductCode == code2)) as MasterOptionItem;
                        if (drMasterOptionItem == null) continue;
                        drMasterOptionItem.ProductCodeP = code;
                    }
                }
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "OptionMasterViewModel(string prodCode, List<string> prodCodeList),error: " + exp.ToString());
                return false ;
            }
            return true;
        }
    }
}
