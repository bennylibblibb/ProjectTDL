 using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using GOSTS.Common;
using GOSTS.ViewModel;
using GOSTS; 
using System.ComponentModel;
using System.Data;
using System.Drawing; 
using System.Windows.Forms; 
using System.Net.Sockets;
using System.Collections;
using System.Linq;
using System.Threading;

using System.Globalization;
using System.Resources;

namespace GOSTS
{
    /// <summary>
    /// A data source that provides raw data objects.  In a real
    /// application this class would make calls to a database.
    /// </summary>
    public static class Database
    {
        static string Underlying
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("TreeUnderlying");
            }
        }

        static string Futures
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("TreeFutures");
            }
        }

        static string Options
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("TreeOptions");
            }
        }

        static string Spreads
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("TreeSpreads");
            }
        }

        static string Call
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("TreeCall");
            }
        }

        static string Put
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("TreePut");
            }
        }

        #region GetInstmnts 
        public static Instmnt[] GetInstmnts(string marketCode, DataTable dataTable)
        {
            try
            {
                //List<string> code = new List<string>();
                //List<string> codeParent = new List<string>();
                List<Instmnt> list = new List<Instmnt>();
                if (dataTable != null)
                {
                    foreach (DataRow dr in dataTable.Select("marketCode='" + marketCode + "'"))
                    {
                        if (!list.Select(d => d.InstmntCode ).ToArray<string>().Contains(dr[1].ToString()))
                        {
                            list.Add(new Instmnt(dr[1].ToString(), dr[0].ToString()));
                            //code.Add(dr[1].ToString());
                            //codeParent.Add(dr[0].ToString());
                        }
                    }
                    //for (int i = 0; i < code.Count; i++)
                    //{
                    //    list.Add(new Instmnt(code[i].ToString(), codeParent[i].ToString()));
                    //}
                }
                return list.ToArray();
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "Error: GetInstmnts() " + exp.ToString());
                return null;
            }
        } 

        #endregion // GetInstmnts

        #region GetProdTypes

        public static ProdType[] GetProdTypes(Instmnt instmnt)
        {
            //List<string> codeParent = new List<string>();
            List<string> code = new List<string>();
            List<ProdType> list = new List<ProdType>();
            if (GOSTradeStation.marketPriceData.MarketTreeTable != null)
            {
                //foreach (DataRow dr in FilterDrs.Where(x => x.Field<string>("instmntCode") != instmnt.InstmntCode && x.Field<string>("marketCode") != instmnt.MarketCode))//("instmntCode='" + instmnt.InstmntCode + "'and marketCode='" + instmnt.MarketCode + "'"))
                 foreach (DataRow dr in GOSTradeStation.marketPriceData.MarketTreeTable.Select("instmntCode='" + instmnt.InstmntCode  + "'and marketCode='" + instmnt.MarketCode  + "'"))
                {
                    if (!code.Contains(dr[3].ToString()))
                    {
                        code.Add(dr[3].ToString());
                       // codeParent.Add(dr[1].ToString());
                       // list.Add(new ProdType((dr[3].ToString() == "0") ? "Underlying" : (dr[3].ToString() == "1") ? "Futures" : (dr[3].ToString() == "2") ? "Options" : (dr[3].ToString() == "3") ? "Spreads" : "no", dr[1].ToString(), dr[0].ToString()));
                      //  list.Add(new ProdType(dr[3].ToString(), dr[1].ToString(), dr[0].ToString()));
                        list.Add(new ProdType(dr[3].ToString(), instmnt));
                    }
                }
                //for (int i = 0; i < code.Count; i++)
                //{
                //    string type = code[i].ToString();
                //    list.Add(new ProdType((type == "0") ? "Underlying" : (type == "1") ? "Futures" : (type == "2") ? "Options" : (type == "3") ? "Spreads" : "no", codeParent[i].ToString()));
                //}
                return list.ToArray();
            }
            return null;
        }

        #endregion // GetProdTypes  
        

        #region GetProdOptions 
        public static ProdOption[] GetProdOptions(ProdType prodType)
        {
            List<DateTime> code = new List<DateTime>();
            List<ProdOption> list = new List<ProdOption>();
            if (GOSTradeStation.marketPriceData.MarketTreeTable != null)
            {
                foreach (DataRow dr in GOSTradeStation.marketPriceData.MarketTreeTable.Select("instmntCode='" + prodType.InstmntCode + "'and marketCode='" + prodType.MarketCode + "' and prodType='" + prodType.ProdTypeCode + "'"))
                {
                    DateTime optionDate = GetOptionDate(dr["productCode"].ToString(), dr["callPut"].ToString());
                    if (!code.Contains(optionDate))
                    {
                        code.Add(optionDate);
                        list.Add(new ProdOption(optionDate, prodType));
                    }
                }
                //.Sort((x, y) => y.ProdOptionName.CompareTo(x.ProdOptionName))
                return list.OrderBy(o => o.ProdOptionName ).ToArray();
            }
            return null;
        }

        private static DateTime GetOptionDate(string productCode, string optionType)
        {
            try
            {
                string strPartYear = DateTime.Now.Year.ToString().Substring(0, 3);

                string symbol = productCode.Substring(productCode.Length - 2, 2).ToUpper();
                string strOptionDate = strPartYear + symbol.Substring(1, 1) + "/";

                if (optionType.ToUpper() == "C")
                {
                    switch (symbol.Substring(0, 1))
                    {
                        case "A":
                            strOptionDate += "01";
                            break;
                        case "B":
                            strOptionDate += "02";
                            break;
                        case "C":
                            strOptionDate += "03";
                            break;
                        case "D":
                            strOptionDate += "04";
                            break;
                        case "E":
                            strOptionDate += "05";
                            break;
                        case "F":
                            strOptionDate += "06";
                            break;
                        case "G":
                            strOptionDate += "07";
                            break;
                        case "H":
                            strOptionDate += "08";
                            break;
                        case "I":
                            strOptionDate += "09";
                            break;
                        case "J":
                            strOptionDate += "10";
                            break;
                        case "K":
                            strOptionDate += "11";
                            break;
                        case "L":
                            strOptionDate += "12";
                            break;
                        default:
                            break;
                    }
                }
                else if (optionType.ToUpper() == "P")
                {
                    switch (symbol.Substring(0, 1))
                    {
                        case "M":
                            strOptionDate += "01";
                            break;
                        case "N":
                            strOptionDate += "02";
                            break;
                        case "O":
                            strOptionDate += "03";
                            break;
                        case "P":
                            strOptionDate += "04";
                            break;
                        case "Q":
                            strOptionDate += "05";
                            break;
                        case "R":
                            strOptionDate += "06";
                            break;
                        case "S":
                            strOptionDate += "07";
                            break;
                        case "T":
                            strOptionDate += "08";
                            break;
                        case "U":
                            strOptionDate += "09";
                            break;
                        case "V":
                            strOptionDate += "10";
                            break;
                        case "W":
                            strOptionDate += "11";
                            break;
                        case "X":
                            strOptionDate += "12";
                            break;
                        default:
                            break;
                    }
                }
                return Convert.ToDateTime(strOptionDate);
            }
            catch { }
            {
                return new DateTime ();
            }
        }

        #endregion 

        #region GetProdTypeOptions 
        public static ProdOptionType[] GetProdOptionTypes(ProdOption prodOption)
        {
            List<ProdOptionType> list = new List<ProdOptionType>();
            list.Add(new ProdOptionType(Call, prodOption));
            list.Add(new ProdOptionType(Put, prodOption));

            return list.ToArray(); ;
        }

        #endregion  

        public static Prod[] GetProds(ProdOptionType prodOptionType)
        {
            List<string> code = new List<string>();
            List<Prod> list = new List<Prod>();
            string prodOptionTypeSymbol = "";
            if (GOSTradeStation.marketPriceData.MarketTreeTable != null)
            {
                prodOptionTypeSymbol = (prodOptionType.ProdOptionTypeName == Call) ? "C" : "P";
                //foreach (DataRow dr in GOSTradeStation.marketPriceData.MarketTreeTable.Select("instmntCode='" + prodOptionType.InstmntCode + "'and marketCode='" + prodOptionType.MarketCode + "' and prodType='2' and callPut='" + prodOptionTypeSymbol + "'", "productCode asc"))
                foreach (DataRow dr in GOSTradeStation.marketPriceData.MarketTreeTable.Select("instmntCode='" + prodOptionType.InstmntCode + "'and marketCode='" + prodOptionType.MarketCode + "' and prodType='2' and callPut='" + prodOptionTypeSymbol + "'"))
                {
                    DateTime  optionDate = GetOptionDate(dr["productCode"].ToString(), dr["callPut"].ToString());
                    if (prodOptionType.ProdOptionName != optionDate) continue;
                    if (code.Contains(dr["productCode"].ToString())) continue;
                    code.Add(dr["productCode"].ToString());
                    list.Add(new Prod(dr["productCode"].ToString(),dr["productName"].ToString(), prodOptionType));

                } return list.ToArray();
            }
            return null;
        }

        #region GetProds 
        public static Prod[] GetProds(ProdType prodType)
        {
            string type = "-1";
            type = (prodType.ProdTypeName ==Underlying) ? "0" : (prodType.ProdTypeName == Futures) ? "1" : (prodType.ProdTypeName == Options) ? "2" : (prodType.ProdTypeName == Spreads) ? "3" : "0";
            List<Prod> list = new List<Prod>();
            if (GOSTradeStation.marketPriceData.MarketTreeTable != null)
            {
                //foreach (DataRow dr in GOSTradeStation.marketPriceData.MarketTreeTable.Select("prodType='" + type + "'and instmntCode='" + prodType.InstmntCode + "' and marketCode='" + prodType.MarketCode + "'", "productCode asc"))
                foreach (DataRow dr in GOSTradeStation.marketPriceData.MarketTreeTable.Select("prodType='" + type + "'and instmntCode='" + prodType.InstmntCode + "' and marketCode='" + prodType.MarketCode + "'"))
                {
                    if (!list.Select(d => d.ProdCode).ToArray<string>().Contains(dr[2].ToString()))
                    {
                        list.Add(new Prod(dr[2].ToString(), dr["productName"].ToString(), prodType));
                    }
                } return list.ToArray();
            }
            return null;
        }

        #endregion 
      
    }
}