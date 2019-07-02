using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Configuration;
using GOSTS.WPFControls.Chart.SCI;
using System.Threading;
using System.Net;
using GOSTS.WPFControls;
using GOSTS.Dealer;
using WPF.MDI;
using System.Text.RegularExpressions ;
 
namespace GOSTS.Common
{

    public class TradeStationSetting
    {
        #region private method for handle XML
        private static XDocument Load(xmlType xmlType, string Url)
        {
            try
            {
                if (!File.Exists(Url))
                {
                    switch (xmlType)
                    {
                        case xmlType.Desktop:

                            break;
                        case xmlType.Customize:

                            break;
                        case xmlType.Server:
                            CreateXml(xmlType, Url);
                            break;
                    }
                }
                return XDocument.Load(Url);
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + " TradeStationSetting.Load(xmlType xmlType, string Url),error:" + exp.ToString());
                return null;
            }
        }
        private static bool CreateXml(xmlType xmlType, string Url)
        {
            XmlText xmltext;
            XmlDocument xmldoc = new XmlDocument();
            switch (xmlType)
            {
                case xmlType.Desktop:
                    return false;
                case xmlType.Customize:
                    return false;
                case xmlType.Server:
                    try
                    {
                        XmlNode xmlnode = xmldoc.CreateXmlDeclaration("1.0", "utf-8", null);
                        xmldoc.AppendChild(xmlnode);
                        XmlElement xmlelem = xmldoc.CreateElement("", "Servers", "");
                        xmltext = xmldoc.CreateTextNode("");
                        xmlelem.AppendChild(xmltext);
                        xmldoc.AppendChild(xmlelem);
                        xmldoc.Save(Url);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }
        private static void Save(XDocument xDocument, string Url)
        {
            try
            {
                xDocument.Save(Url);
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TradeStationSetting.Save(),Error:" + exp.ToString());
            }
        }
        #endregion

        #region config Servers

        public static string ReturnServerIp(string domain)
        { 
            string strIp = (domain.IndexOf(":") > 0) ? domain.Substring(domain.IndexOf(":") + 1, domain.Length - domain.IndexOf(":") - 1) : domain.Trim();
            if (Regex.IsMatch(strIp, @"(^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$)"))
            {
                return strIp;
            }
            else if (Regex.IsMatch(strIp, @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$"))
            {
                try
                {
                    IPAddress[] addresslist = Dns.GetHostAddresses(strIp);
                    foreach (IPAddress theaddress in addresslist)
                    {
                        return theaddress.ToString();
                    }
                }
                catch
                {
                    return "";
                }
            }
            return "";
        }

        public static bool SaveServerSettings(string Name, string Ip)
        {
            var dataXml = Load(xmlType.Server, AppFlag.UserData + AppFlag.Servers);
            if (dataXml == null) return false;

            try
            {
                bool isExisted = false;
                IEnumerable<XNode> atr = dataXml.Element("Servers").Nodes();
                if (atr != null)
                {
                    //set true for selected server
                    foreach (XElement item in atr)
                    {
                        if (item.Element("Ip").Value == Ip)
                        {
                            item.Attribute("Selected").Value = "true";
                            item.Element("Name").Value = Name;
                            isExisted = true;
                        }
                        else
                        {
                            item.Attribute("Selected").Value = "false";
                        }
                    }
                }
                if (!isExisted)
                {
                    //add server
                    XElement xe = dataXml.Element("Servers");
                    var xel = new XElement("Server", new XAttribute("Selected", "true"),
                                new XElement("Name", Name), new XElement("Ip", Ip));
                    xe.Add(xel);
                }

                Save(dataXml, AppFlag.UserData + AppFlag.Servers);
                return true;
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + " TradeStationSetting.SaveServerSettings(string Name,string Ip),error:" + exp.ToString());
                return false;
            }
        }

        public static List<Server> ReturnServers()
        {
            var dataXml = Load(xmlType.Server, AppFlag.UserData + AppFlag.Servers);
            if (dataXml == null) return null;
            List<Server> servers = new List<Server>();

            try
            {
                IEnumerable<XNode> atr = dataXml.Element("Servers").Nodes();
                foreach (XElement item in atr)
                {
                    servers.Add(new Server
                    {
                        Name = item.Element("Name").Value,
                        Ip = item.Element("Ip").Value,
                        DisplayName = item.Element("Name").Value + ":" + item.Element("Ip").Value,
                        Selected = (item.Attribute("Selected").Value.ToUpper() == "TRUE") ? true : false
                    });
                }

                return servers;
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + " TradeStationSetting.ReturnServers(),error:" + exp.ToString());
                return null;
            }
        }
        #endregion

        private static XDocument Load()
        {
            try
            {
                if (!File.Exists(AppFlag.UserData + AppFlag.CustomizeFile))
                {
                    return null;
                }
                return XDocument.Load(AppFlag.UserData + AppFlag.CustomizeFile);
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + " TradeStationSetting.Load(),error:" + exp.ToString());
                return null;
            }
        }

        public static bool UpdateConfig(string AppKey, string AppValue)
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(Application.ExecutablePath + ".config");

                XmlNode xNode;
                XmlElement xElemKey;
                XmlElement xElemValue;

                xNode = xDoc.SelectSingleNode("//appSettings");

                xElemKey = (XmlElement)xNode.SelectSingleNode("//add[@key=\"" + AppKey + "\"]");
                if (xElemKey != null) xElemKey.SetAttribute("value", AppValue);
                else
                {
                    xElemValue = xDoc.CreateElement("add");
                    xElemValue.SetAttribute("key", AppKey);
                    xElemValue.SetAttribute("value", AppValue);
                    xNode.AppendChild(xElemValue);
                }
                xDoc.Save(Application.ExecutablePath + ".config");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ReturnWindowName(WindowTypes windowType, String Code)
        {
            string strTitle = "";
            switch (windowType)
            {
                case WindowTypes.Ticker:
                    strTitle = GOSTS.GosCulture.CultureHelper.GetString("WinTitleTicker") + " - " + Code + "(" + MarketPriceData.GetProductName(Code) + ")";
                    break;
                case WindowTypes.PriceDepth:
                    strTitle = GOSTS.GosCulture.CultureHelper.GetString("WinTitlePriceDepth") + " - " + Code + "(" + MarketPriceData.GetProductName(Code) + ")";
                    break;
                //case WindowTypes.LongPriceDepth:
                //    strTitle = "Long Price Depth – " + Code + "(" + MarketPriceData.GetProductName(Code) + ")";
                //    break;
                case WindowTypes.MarginCallList:
                    strTitle = GOSTS.GosCulture.CultureHelper.GetString("WinTitleMCL");
                    break;
                case WindowTypes.MarginCheck:
                    strTitle = GOSTS.GosCulture.CultureHelper.GetString("WinTitleMarginCheck") + " - " + Code;
                    break;
                case WindowTypes.MarketPriceControl:
                    strTitle = GOSTS.GosCulture.CultureHelper.GetString("WinTitleMarketPrice") + " - " + Code;
                    break;
                case WindowTypes.AppPreference:
                    strTitle = GOSTS.GosCulture.CultureHelper.GetString("WinTitleCustomizePref") + " - " + "[" + Code + "]";
                    break;
                case WindowTypes.TradeConfirmation:
                    strTitle = GOSTS.GosCulture.CultureHelper.GetString("WinTitleTradeConf");
                    break;
                case WindowTypes.TradeConfirmDetails :
                    strTitle = GOSTS.GosCulture.CultureHelper.GetString("WinTitleTradeConfDetails");//+ " - " + "[" + Code + "]";
                    break;
                case WindowTypes.SCIChartAnalysis:
                    strTitle = GOSTS.GosCulture.CultureHelper.GetString("WinTitleSCIChart") + " - " + Code + "(" + MarketPriceData.GetProductName(Code) + ")";
                    break;
                case WindowTypes.OptionMaster:
                    strTitle = GOSTS.GosCulture.CultureHelper.GetString("WinTitleOptionMatrix");
                    break;
                case WindowTypes.Clock:
                    strTitle = GOSTS.GosCulture.CultureHelper.GetString("WinTitleClock");
                    break;
                case WindowTypes.PriceAlert:
                    strTitle = GOSTS.GosCulture.CultureHelper.GetString("WinTitlePriceAlert");
                    break;
                case WindowTypes.AddPriceAlert:
                    strTitle =Code +" "+ GOSTS.GosCulture.CultureHelper.GetString("WinTitleAddPriceAlert")+ " - " + Code + "(" + MarketPriceData.GetProductName(Code) + ")";
                    break;
                case WindowTypes.Login:
                    strTitle = "SSL " + GOSTS.GosCulture.CultureHelper.GetString("WinTitleLogin");
                    break;
                case WindowTypes.OrderConfirm:
                    if (Code == "Disclaimer")
                    {
                        strTitle = GOSTradeStation.UserID + " - Disclaimer";
                    }
                    else
                    {
                    }
                    break;
            }

            return strTitle;
        }

        public static string ChangeWindowLanguage(MdiChild  mdiChild)
        {
            string title = "";
            Type WindowType = mdiChild.Content.GetType();
            if (WindowType == typeof(PriceDepth))
            {
                title = TradeStationSetting.ReturnWindowName(WindowTypes.PriceDepth, ((PriceDepth)mdiChild.Content).ProdCode) + ((((PriceDepth)mdiChild.Content).Locked == true) ? " * " : "");
            }
            else if (WindowType == typeof(Ticker))
            {
                title = TradeStationSetting.ReturnWindowName(WindowTypes.Ticker, ((Ticker)mdiChild.Content).ProdCode) + ((((Ticker )mdiChild.Content).Locked == true) ? " * " : "");
            }
            else if (WindowType == typeof(MarketPriceControl))
            {
                title = TradeStationSetting.ReturnWindowName(WindowTypes.MarketPriceControl, ((MarketPriceControl)mdiChild.Content).countSymbol);
            }
            else if (WindowType == typeof(MarginCallList))
            {
                title = TradeStationSetting.ReturnWindowName(WindowTypes.MarginCallList, "");
            }
            else if (WindowType == typeof(MarginCheck))
            {
                title = TradeStationSetting.ReturnWindowName(WindowTypes.MarginCheck, "");
            }
            else if (WindowType == typeof(TradeConfirmation))
            {
                title = TradeStationSetting.ReturnWindowName(WindowTypes.TradeConfirmation, "");
            }
            else if (WindowType == typeof(TradeConfirmDetails))
            {
                title = TradeStationSetting.ReturnWindowName(WindowTypes.TradeConfirmDetails,((TradeConfirmDetails)mdiChild.Content).AccNo);
            }
            else if (WindowType == typeof(Clock))
            {
                title = TradeStationSetting.ReturnWindowName(WindowTypes.Clock, "");
            }
            else if (WindowType == typeof(SCIChartAnalysis))
            {
                title = TradeStationSetting.ReturnWindowName(WindowTypes.SCIChartAnalysis, "");
            }
            else if (WindowType == typeof(OptionMaster))
            {
                title = TradeStationSetting.ReturnWindowName(WindowTypes.OptionMaster, "");
            }
            else if (WindowType == typeof(PriceAlert ))
            {
                title = TradeStationSetting.ReturnWindowName(WindowTypes.PriceAlert, "");
            }
            return title;
        }


        public static string GetMarketPriceCode(string title)
        {
            string str = GOSTS.GosCulture.CultureHelper.GetString("WinTitleMarketPrice");
            if (title.Length < str.Length)
                return "";
            try
            {
            string strcodes=title.Remove (0,str.Length );
            return (strcodes).Substring(3, strcodes.Length  - 3);
            }
            catch 
            {
            return""; 
            }
        }

        public static MarginColorData ReturnCustomizeData(String UserID)
        {
            MarginColorData data = new MarginColorData();
            var dataXml = Load();
            if (dataXml == null) return data;
            IEnumerable<XNode> atr = dataXml.Root.Element("Customizes").Nodes();
            if (atr == null) return data;
            bool isExist = false;

            foreach (XElement item in atr)
            {
                if (item.Attribute("ID").Value == UserID)
                {
                    isExist = true;
                    data.UserID = UserID;
                    data.Type = "MarginColor";
                    XElement xe = item.Element("MarginColors");
                    if (xe != null)
                    {
                        data.isEnabled = (xe.Attribute("NoColor").Value.ToUpper() == "TRUE") ? true : false;
                        var result = from window in xe.Descendants("Per")
                                     select new
                                     {
                                         Per = window.Value,
                                         Color = window.Attribute("Color")
                                     };
                        foreach (var s in result)
                        {
                            data.MarginColors.Add(new MarginColor
                            {
                                Percent = Convert.ToInt32(s.Per),
                                Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString((string)s.Color))
                            });
                        }
                        break;
                    }
                    else
                    {
                        var customize = from c in dataXml.Root.Element("Customizes").Elements("Customize")
                                        where (string)c.Attribute("ID") == "DEFAULT"
                                        select c;
                        XElement xeMC = customize.FirstOrDefault().Element("MarginColors");
                        if (xeMC != null)
                        {
                            data.isEnabled = (xeMC.Attribute("NoColor").Value.ToUpper() == "TRUE") ? true : false;
                            var result = from window in xeMC.Descendants("Per")
                                         select new
                                         {
                                             Per = window.Value,
                                             Color = window.Attribute("Color")
                                         };
                            foreach (var s in result)
                            {
                                data.MarginColors.Add(new MarginColor
                                {
                                    Percent = Convert.ToInt32(s.Per),
                                    Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString((string)s.Color))
                                });
                            }
                            break;
                        }
                    }
                }
            }
            if (!isExist)
            {
                foreach (XElement item in atr)
                {
                    if (item.Attribute("ID").Value == "DEFAULT")
                    {
                        data.UserID = UserID;
                        data.Type = "MarginColor";
                        XElement xe = item.Element("MarginColors");
                        if (xe != null)
                        {
                            data.isEnabled = (xe.Attribute("NoColor").Value.ToUpper() == "TRUE") ? true : false;
                            var result = from window in xe.Descendants("Per")
                                         select new
                                         {
                                             Per = window.Value,
                                             Color = window.Attribute("Color")
                                         };
                            foreach (var s in result)
                            {
                                data.MarginColors.Add(new MarginColor
                                {
                                    Percent = Convert.ToInt32(s.Per),
                                    Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString((string)s.Color))
                                });
                            }
                            break;
                        }
                    }
                }
            }
            return data;
        }

        public static CustomizeData GetCustomizeData(String UserID, string Type)
        {
            CustomizeData data = new CustomizeData();

            var dataXml = Load();
            if (dataXml == null) return data;
            var customizeDefault = from c in dataXml.Root.Element("Customizes").Elements("Customize")
                                   where (string)c.Attribute("ID") == UserID
                                   select c;
            XElement xe;
            switch (Type)
            {
                case "General":
                    AlertData alertData = new AlertData();
                    alertData.UserID = UserID;
                    alertData.Type = Type;

                    xe = customizeDefault.FirstOrDefault().Element("Alert");
                    if (xe != null)
                    {
                        alertData.isOrderConfirmS = (xe.Element("OrderConfirm").Attribute("Sound").Value.ToUpper() == "TRUE") ? true : false;
                        alertData.isReqAccAlertS = (xe.Element("ReqAccAlert").Attribute("Sound").Value.ToUpper() == "TRUE") ? true : false;
                        alertData.isReqErrAlertS = (xe.Element("ReqErrAlert").Attribute("Sound").Value.ToUpper() == "TRUE") ? true : false;
                        alertData.isTradeAlertS = (xe.Element("TradeAlert").Attribute("Sound").Value.ToUpper() == "TRUE") ? true : false;
                        alertData.isNewAppAlertS = (xe.Element("NewAppAlert").Attribute("Sound").Value.ToUpper() == "TRUE") ? true : false;

                        alertData.isOrderConfirmT = (xe.Element("OrderConfirm").Attribute("Text").Value.ToUpper() == "TRUE") ? true : false;
                        alertData.isReqAccAlertT = (xe.Element("ReqAccAlert").Attribute("Text").Value.ToUpper() == "TRUE") ? true : false;
                        alertData.isReqErrAlertT = (xe.Element("ReqErrAlert").Attribute("Text").Value.ToUpper() == "TRUE") ? true : false;
                        alertData.isTradeAlertT = (xe.Element("TradeAlert").Attribute("Text").Value.ToUpper() == "TRUE") ? true : false;
                        alertData.isTimeCorrAlertT = (xe.Element("TimeCorrAlert").Attribute("Text").Value.ToUpper() == "TRUE") ? true : false;
                        alertData.NotifyLevel = (xe.Element("NotifyLevel").Value.Trim() == "") ? -1 : TradeStationTools.ConvertToInt(xe.Element("NotifyLevel").Value, -1);
                        alertData.isEnableWheel = (xe.Element("EnableWheel").Value.ToUpper() == "TRUE") ? true : false;
                        alertData.isSingleClick = (xe.Element("ClickMethod").Value.ToUpper() == "SINGLE") ? true : false;
                        alertData.isDoubleClick = (xe.Element("ClickMethod").Value.ToUpper() == "SINGLE") ? false : true;
                    }
                    data.AlertData = alertData;
                    break;
                case "Margin Color":
                    MarginColorData marginColorData = new MarginColorData();
                    marginColorData.UserID = UserID;
                    marginColorData.Type = Type;

                    xe = customizeDefault.FirstOrDefault().Element("MarginColors");
                    if (xe != null)
                    {
                        marginColorData.isEnabled = (xe.Attribute("NoColor").Value.ToUpper() == "TRUE") ? true : false;
                        var result = from window in xe.Descendants("Per")
                                     select new
                                     {
                                         Per = window.Value,
                                         Color = window.Attribute("Color")
                                     };
                        foreach (var s in result)
                        {
                            marginColorData.MarginColors.Add(new MarginColor
                            {
                                Percent = Convert.ToInt32(s.Per),
                                Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString((string)s.Color))
                            });
                        }
                        data.MarginColorData = marginColorData;
                    }
                    break;
                case "ChartData":
                    ChartData chartData = new ChartData();
                    chartData.UserID = UserID;
                    chartData.Type = Type;

                    xe = customizeDefault.FirstOrDefault().Element("ChartData");
                    if (xe != null)
                    {
                        chartData.isPan = (xe.Element("isPan").Value.ToUpper().Equals("TRUE")) ? true : false;
                        chartData.showTickerLine = (xe.Element("showTickerLine").Value.ToUpper().Equals("TRUE")) ? true : false;
                        chartData.showVolume = (xe.Element("showVolume").Value.ToUpper().Equals("TRUE")) ? true : false;
                        chartData.showOverview = (xe.Element("showOverview").Value.ToUpper().Equals("TRUE")) ? true : false;
                        chartData.showBollingerBands = (xe.Element("BollingerBands").Attribute("Enable").Value.ToUpper().Equals("TRUE")) ? true : false;
                        uint bbPeriod = TradeStationTools.ConvertToUInt(xe.Element("BollingerBands").Attribute("Period").Value);
                        chartData.bollingerBandsPeriod = (bbPeriod < SCIConstants.SCI_MINIMUM_BOLLINGER_BANDS_PERIOD) ? SCIConstants.SCI_DEFAULT_BOLLINGER_BANDS_PERIOD : bbPeriod;
                        double bbDevi = TradeStationTools.ConvertToDouble(xe.Element("BollingerBands").Attribute("Deviation").Value);
                        chartData.bollingerBandsDeviation = ((bbDevi < SCIConstants.SCI_MINIMUM_BOLLINGER_BANDS_DEVIATION) || (bbDevi > SCIConstants.SCI_MAXIMUM_BOLLINGER_BANDS_DEVIATION)) ? SCIConstants.SCI_DEFAULT_BOLLINGER_BANDS_DEVIATION : bbDevi;
                        chartData.bollingerBandsOuterColour = SCICommon.ValidateColourValue(xe.Element("BollingerBands").Attribute("OuterColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_BOLLINGER_BANDS_OUTER_COLOUR);
                        chartData.bollingerBandsSMAColour = SCICommon.ValidateColourValue(xe.Element("BollingerBands").Attribute("SMAColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_BOLLINGER_BANDS_SMA_COLOUR);
                        chartData.showSMA1 = (xe.Element("SMA1").Attribute("Enable").Value.ToUpper().Equals("TRUE")) ? true : false;
                        uint sma1Period = TradeStationTools.ConvertToUInt(xe.Element("SMA1").Attribute("Period").Value);
                        chartData.sma1Period = (sma1Period == 0) ? SCIConstants.SCI_DEFAULT_SMA1_PERIOD : sma1Period;
                        chartData.sma1Colour = SCICommon.ValidateColourValue(xe.Element("SMA1").Attribute("SMAColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_SMA1_COLOUR);
                        chartData.showSMA2 = (xe.Element("SMA2").Attribute("Enable").Value.ToUpper().Equals("TRUE")) ? true : false;
                        uint sma2Period = TradeStationTools.ConvertToUInt(xe.Element("SMA2").Attribute("Period").Value);
                        chartData.sma2Period = (sma2Period == 0) ? SCIConstants.SCI_DEFAULT_SMA2_PERIOD : sma2Period;
                        chartData.sma2Colour = SCICommon.ValidateColourValue(xe.Element("SMA2").Attribute("SMAColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_SMA2_COLOUR);
                        chartData.showSMA3 = (xe.Element("SMA3").Attribute("Enable").Value.ToUpper().Equals("TRUE")) ? true : false;
                        uint sma3Period = TradeStationTools.ConvertToUInt(xe.Element("SMA3").Attribute("Period").Value);
                        chartData.sma3Period = (sma3Period == 0) ? SCIConstants.SCI_DEFAULT_SMA3_PERIOD : sma3Period;
                        chartData.sma3Colour = SCICommon.ValidateColourValue(xe.Element("SMA3").Attribute("SMAColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_SMA3_COLOUR);
                        chartData.theme = SCICommon.ValidateChartTheme(xe.Element("Theme").Value.ToUpper());
                        uint chartPeriod = TradeStationTools.ConvertToUInt(xe.Element("ChartPeriod").Value);
                        chartData.chartPeriod = (chartPeriod == 0) ? SCIConstants.SCI_DEFAULT_CHART_PERIOD : chartPeriod;
                        chartData.showMACDIndicator = (xe.Element("MACD").Attribute("showMACDIndicator").Value.ToUpper().Equals("TRUE")) ? true : false;
                        uint macdFastPeriod = TradeStationTools.ConvertToUInt(xe.Element("MACD").Attribute("macdFastPeriod").Value);
                        chartData.macdFastPeriod = (macdFastPeriod == 0) ? SCIConstants.SCI_DEFAULT_MACD_FAST_PERIOD : macdFastPeriod;
                        uint macdSlowPeriod = TradeStationTools.ConvertToUInt(xe.Element("MACD").Attribute("macdSlowPeriod").Value);
                        chartData.macdSlowPeriod = (macdSlowPeriod == 0) ? SCIConstants.SCI_DEFAULT_MACD_SLOW_PERIOD : macdSlowPeriod;
                        uint macdSignalPeriod = TradeStationTools.ConvertToUInt(xe.Element("MACD").Attribute("macdSignalPeriod").Value);
                        chartData.macdSignalPeriod = (macdSignalPeriod == 0) ? SCIConstants.SCI_DEFAULT_MACD_SIGNAL_PERIOD : macdSignalPeriod;
                        chartData.macdMACDColour = SCICommon.ValidateColourValue(xe.Element("MACD").Attribute("macdMACDColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_MACD_MACD_COLOUR);
                        chartData.macdSignalColour = SCICommon.ValidateColourValue(xe.Element("MACD").Attribute("macdSignalColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_MACD_SIGNAL_COLOUR);
                        chartData.macdHistogramColour = SCICommon.ValidateColourValue(xe.Element("MACD").Attribute("macdHistogramColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_MACD_HISTOGRAM_COLOUR);
                        chartData.showRSIIndicator = (xe.Element("RSI").Attribute("showRSIIndicator").Value.ToUpper().Equals("TRUE")) ? true : false;
                        uint rsiPeriod = TradeStationTools.ConvertToUInt(xe.Element("RSI").Attribute("rsiPeriod").Value);
                        chartData.rsiPeriod = (rsiPeriod == 0) ? SCIConstants.SCI_DEFAULT_RSI_PERIOD : rsiPeriod;
                        uint rsiOverBoughtLine = TradeStationTools.ConvertToUInt(xe.Element("RSI").Attribute("rsiOverBoughtLine").Value);
                        chartData.rsiOverBoughtLine = (rsiOverBoughtLine > 100) ? SCIConstants.SCI_DEFAULT_RSI_OVERBOUGHT_LINE : rsiOverBoughtLine;
                        uint rsiOverSoldLine = TradeStationTools.ConvertToUInt(xe.Element("RSI").Attribute("rsiOverSoldLine").Value);
                        chartData.rsiOverSoldLine = (rsiOverSoldLine > 100) ? SCIConstants.SCI_DEFAULT_RSI_OVERSOLD_LINE : rsiOverSoldLine;
                        uint rsiMidLine = TradeStationTools.ConvertToUInt(xe.Element("RSI").Attribute("rsiMidLine").Value);
                        chartData.rsiMidLine = (rsiMidLine > 100) ? SCIConstants.SCI_DEFAULT_RSI_MID_LINE : rsiMidLine;
                        chartData.rsiColour = SCICommon.ValidateColourValue(xe.Element("RSI").Attribute("rsiColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_RSI_COLOUR);
                        chartData.rsiOverBoughtLineColour = SCICommon.ValidateColourValue(xe.Element("RSI").Attribute("rsiOverBoughtLineColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_RSI_OVERBOUGHT_LINE_COLOUR);
                        chartData.rsiOverSoldLineColour = SCICommon.ValidateColourValue(xe.Element("RSI").Attribute("rsiOverSoldLineColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_RSI_OVERSOLD_LINE_COLOUR);
                        chartData.rsiMidLineColour = SCICommon.ValidateColourValue(xe.Element("RSI").Attribute("rsiMidLineColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_RSI_MID_LINE_COLOUR);
                        chartData.showWilliamsRIndicator = (xe.Element("WilliamsR").Attribute("showWilliamsRIndicator").Value.ToUpper().Equals("TRUE")) ? true : false;
                        uint williamsRPeriod = TradeStationTools.ConvertToUInt(xe.Element("WilliamsR").Attribute("williamsRPeriod").Value);
                        chartData.williamsRPeriod = (williamsRPeriod == 0) ? SCIConstants.SCI_DEFAULT_WILLIAMSR_PERIOD : williamsRPeriod;
                        int williamsROverBoughtLine = TradeStationTools.ConvertToInt(xe.Element("WilliamsR").Attribute("williamsROverBoughtLine").Value);
                        chartData.williamsROverBoughtLine = ((williamsROverBoughtLine > 0) || (williamsROverBoughtLine < -100)) ? SCIConstants.SCI_DEFAULT_WILLIAMSR_OVERBOUGHT_LINE : williamsROverBoughtLine;
                        int williamsROverSoldLine = TradeStationTools.ConvertToInt(xe.Element("WilliamsR").Attribute("williamsROverSoldLine").Value);
                        chartData.williamsROverSoldLine = ((williamsROverSoldLine > 0) || (williamsROverSoldLine < -100)) ? SCIConstants.SCI_DEFAULT_WILLIAMSR_OVERSOLD_LINE : williamsROverSoldLine;
                        int williamsRMidLine = TradeStationTools.ConvertToInt(xe.Element("WilliamsR").Attribute("williamsRMidLine").Value);
                        chartData.williamsRMidLine = ((williamsRMidLine > 0) || (williamsRMidLine < -100)) ? SCIConstants.SCI_DEFAULT_WILLIAMSR_MID_LINE : williamsRMidLine;
                        chartData.williamsRColour = SCICommon.ValidateColourValue(xe.Element("WilliamsR").Attribute("williamsRColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_WILLIAMSR_COLOUR);
                        chartData.williamsROverBoughtLineColour = SCICommon.ValidateColourValue(xe.Element("WilliamsR").Attribute("williamsROverBoughtLineColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_WILLIAMSR_OVERBOUGHT_LINE_COLOUR);
                        chartData.williamsROverSoldLineColour = SCICommon.ValidateColourValue(xe.Element("WilliamsR").Attribute("williamsROverSoldLineColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_WILLIAMSR_OVERSOLD_LINE_COLOUR);
                        chartData.williamsRMidLineColour = SCICommon.ValidateColourValue(xe.Element("WilliamsR").Attribute("williamsRMidLineColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_WILLIAMSR_MID_LINE_COLOUR);
                        chartData.candlestickUpColour = SCICommon.ValidateColourValue(xe.Element("Candlestick").Attribute("candlestickUpColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_CANDLESTICK_UP_COLOUR);
                        chartData.candlestickDownColour = SCICommon.ValidateColourValue(xe.Element("Candlestick").Attribute("candlestickDownColour").Value.ToUpper(), SCIConstants.SCI_DEFAULT_CANDLESTICK_DOWN_COLOUR);
                    }

                    data.ChartData = chartData;
                    break;
            }
            return data;
        }

        public static CustomizeData ReturnCustomizeData(String UserID, string Type)
        {
            CustomizeData data = new CustomizeData();
            data.UserID = UserID;

            var dataXml = Load();
            if (dataXml == null)
            {
                if (Type.Equals("ChartData") == true)
                {
                    //Always return an object
                    data.ChartData = new ChartData();
                }

                return data;
            }

            var customize = from s in dataXml.Root.Element("Customizes").Elements("Customize")
                            where (string)s.Attribute("ID") == UserID
                            select s;
            if (customize.Count() <= 0)
            {
                //userID not existed in,load DEFAULT 
                switch (Type)
                {
                    case "General":
                        data.Type = "AlertData";
                        data.AlertData = GetCustomizeData("DEFAULT", Type).AlertData; ;
                        break;
                    case "Margin Color":
                        data.Type = "MarginColorData";
                        data.MarginColorData = GetCustomizeData("DEFAULT", Type).MarginColorData;
                        break;
                    case "ChartData":
                        data.Type = "ChartData";
                        data.ChartData = new ChartData();
                        break;
                }
            }
            else
            {
                //userID existed in 
                XElement xe;
                switch (Type)
                {
                    case "General":
                        data.Type = "AlertData";
                        xe = customize.FirstOrDefault().Element("Alert");
                        if (xe == null)
                        {
                            //Alert not existed in  
                            data.AlertData = GetCustomizeData("DEFAULT", Type).AlertData;
                        }
                        else
                        {
                            //Alert  existed in   
                            data.AlertData = GetCustomizeData(UserID, Type).AlertData;
                        }
                        break;
                    case "Margin Color":
                        data.Type = "MarginColorData";
                        xe = customize.FirstOrDefault().Element("MarginColors");
                        if (xe == null)
                        {
                            data.MarginColorData = GetCustomizeData("DEFAULT", Type).MarginColorData;
                        }
                        else
                        {
                            //MarginColors  existed in   
                            data.MarginColorData = GetCustomizeData(UserID, Type).MarginColorData;
                        }
                        break;

                    case "ChartData":
                        data.Type = "ChartData";
                        xe = customize.FirstOrDefault().Element("ChartData");
                        if (xe == null)
                        {
                            data.ChartData = new ChartData();
                        }
                        else
                        {
                            data.ChartData = GetCustomizeData(UserID, Type).ChartData;
                        }

                        break;
                }
            }
            return data;
        }

        public static bool SaveCustomizeData(CustomizeData customizeData)
        {
            try
            {
                var dataXml = Load();
                if (dataXml == null) return false;
                XElement xe = dataXml.Root.Element("Customizes");
                var customize = from c in xe.Elements("Customize")
                                where (string)c.Attribute("ID") == customizeData.UserID
                                select c;

                switch (customizeData.Type)
                {
                    case "MarginColorData":
                        MarginColorData data = customizeData.MarginColorData;
                        if (customize.Count() <= 0)
                        {
                            var xel = new XElement("Customize", new XAttribute("ID", data.UserID),
                               new XElement("MarginColors", new XAttribute("NoColor", (data.isEnabled == true) ? "true" : "false"),
                                    from mc in data.MarginColors
                                    select new XElement("Per", mc.Percent.ToString(), new XAttribute("Color", mc.Color.Color.ToString()))
                                   ));
                            xe.Add(xel);
                        }
                        else
                        {
                            XElement xeMC = customize.FirstOrDefault().Element("MarginColors");
                            if (xeMC == null)
                            {
                                var xel = new XElement("MarginColors", new XAttribute("NoColor", (data.isEnabled == true) ? "true" : "false"),
                                        from mc in data.MarginColors
                                        select new XElement("Per", mc.Percent.ToString(), new XAttribute("Color", mc.Color.Color.ToString()))
                                       );
                                customize.FirstOrDefault().Add(xel);
                            }
                            else
                            {
                                xeMC.Attribute("NoColor").Value = (data.isEnabled == true) ? "true" : "false";
                                var result = from s in xeMC.Descendants("Per")
                                             select s;
                                int i = 0;
                                foreach (var per in result)
                                {
                                    per.Value = data.MarginColors[i].Percent.ToString();
                                    per.Attribute("Color").Value = data.MarginColors[i].Color.Color.ToString();
                                    i++;
                                }
                            }
                        }
                        Save(dataXml, AppFlag.UserData + AppFlag.CustomizeFile);
                        MessageBox.Show("Saved!");
                        return true;
                    case "AlertData":
                        AlertData alertData = customizeData.AlertData;
                        if (customize.Count() <= 0)
                        {
                            var xel = new XElement("Customize", new XAttribute("ID", customizeData.UserID),
                                new XElement("Alert",
                                    new XElement("OrderConfirm", new XAttribute("Text", alertData.isOrderConfirmT.ToString()), new XAttribute("Sound", alertData.isOrderConfirmS.ToString())),
                                    new XElement("ReqAccAlert", new XAttribute("Text", alertData.isReqAccAlertT.ToString()), new XAttribute("Sound", alertData.isReqAccAlertS.ToString())),
                                    new XElement("ReqErrAlert", new XAttribute("Text", alertData.isReqErrAlertT.ToString()), new XAttribute("Sound", alertData.isReqErrAlertS.ToString())),
                                    new XElement("TradeAlert", new XAttribute("Text", alertData.isTradeAlertT.ToString()), new XAttribute("Sound", alertData.isTradeAlertS.ToString())),
                                    new XElement("NewAppAlert", new XAttribute("Sound", alertData.isNewAppAlertS.ToString())),
                                    new XElement("TimeCorrAlert", new XAttribute("Text", alertData.isTimeCorrAlertT.ToString())),
                                    new XElement("NotifyLevel", alertData.NotifyLevel.ToString()),
                                    new XElement("EnableWheel", alertData.isEnableWheel.ToString()),
                                    new XElement("ClickMethod", (alertData.isSingleClick == true) ? "Single" : "Double")
                                    ));

                            xe.Add(xel);
                        }
                        else
                        {
                            XElement xeMC = customize.FirstOrDefault().Element("Alert");
                            if (xeMC == null)
                            {
                                //var xel = new XElement("Alert",
                                //  new XElement("OrderConfirm", new XAttribute("Text", (alertData.isOrderConfirmT == true) ? "true" : "false"), new XAttribute("Sound", (alertData.isOrderConfirmS == true) ? "true" : "false")),
                                //  new XElement("ReqAccAlert", new XAttribute("Text", (alertData.isReqAccAlertT == true) ? "true" : "false"), new XAttribute("Sound", (alertData.isReqAccAlertS == true) ? "true" : "false")),
                                //  new XElement("ReqErrAlert", new XAttribute("Text", (alertData.isReqErrAlertT == true) ? "true" : "false"), new XAttribute("Sound", (alertData.isReqErrAlertS == true) ? "true" : "false")),
                                //  new XElement("TradeAlert", new XAttribute("Text", (alertData.isTradeAlertT == true) ? "true" : "false"), new XAttribute("Sound", (alertData.isTradeAlertS == true) ? "true" : "false")),
                                //  new XElement("NewAppAlert",  new XAttribute("Sound",(alertData.isNewAppAlertS == true) ? "true" : "false")),
                                //  new XElement("TimeCorrAlert",  new XAttribute("Text",alertData.isTimeCorrAlertT.ToString())),
                                //  new XElement("NotifyLevel", alertData.NotifyLevel.ToString()));
                                var xel = new XElement("Alert",
                                    new XElement("OrderConfirm", new XAttribute("Text", alertData.isOrderConfirmT.ToString()), new XAttribute("Sound", alertData.isOrderConfirmS.ToString())),
                                    new XElement("ReqAccAlert", new XAttribute("Text", alertData.isReqAccAlertT.ToString()), new XAttribute("Sound", alertData.isReqAccAlertS.ToString())),
                                    new XElement("ReqErrAlert", new XAttribute("Text", alertData.isReqErrAlertT.ToString()), new XAttribute("Sound", alertData.isReqErrAlertS.ToString())),
                                    new XElement("TradeAlert", new XAttribute("Text", alertData.isTradeAlertT.ToString()), new XAttribute("Sound", alertData.isTradeAlertS.ToString())),
                                    new XElement("NewAppAlert", new XAttribute("Sound", alertData.isNewAppAlertS.ToString())),
                                    new XElement("TimeCorrAlert", new XAttribute("Text", alertData.isTimeCorrAlertT.ToString())),
                                    new XElement("NotifyLevel", alertData.NotifyLevel.ToString()),
                                    new XElement("EnableWheel", alertData.isEnableWheel.ToString()),
                                     new XElement("ClickMethod", (alertData.isSingleClick == true) ? "Single" : "Double")
                                         );

                                customize.FirstOrDefault().Add(xel);
                            }
                            else
                            {
                                xeMC.Element("OrderConfirm").Attribute("Text").Value = alertData.isOrderConfirmT.ToString();
                                xeMC.Element("OrderConfirm").Attribute("Sound").Value = alertData.isOrderConfirmS.ToString();
                                xeMC.Element("ReqAccAlert").Attribute("Text").Value = alertData.isReqAccAlertT.ToString();
                                xeMC.Element("ReqAccAlert").Attribute("Sound").Value = alertData.isReqAccAlertS.ToString();
                                xeMC.Element("ReqErrAlert").Attribute("Text").Value = alertData.isReqErrAlertT.ToString();
                                xeMC.Element("ReqErrAlert").Attribute("Sound").Value = alertData.isReqErrAlertS.ToString();
                                xeMC.Element("TradeAlert").Attribute("Text").Value = alertData.isTradeAlertT.ToString();
                                xeMC.Element("TradeAlert").Attribute("Sound").Value = alertData.isTradeAlertS.ToString();
                                xeMC.Element("NewAppAlert").Attribute("Sound").Value = alertData.isNewAppAlertS.ToString();
                                xeMC.Element("TimeCorrAlert").Attribute("Text").Value = alertData.isTimeCorrAlertT.ToString();
                                xeMC.Element("NotifyLevel").Value = alertData.NotifyLevel.ToString();
                                xeMC.Element("EnableWheel").Value = alertData.isEnableWheel.ToString();
                                xeMC.Element("ClickMethod").Value = (alertData.isSingleClick == true) ? "Single" : "Double";
                            }
                        }
                        Save(dataXml, AppFlag.UserData + AppFlag.CustomizeFile);
                        return true;

                    case "ChartData":
                        ChartData chartData = customizeData.ChartData;
                        if (customize.Count() <= 0)
                        {
                            var xel = new XElement("Customize", new XAttribute("ID", chartData.UserID),
                                      new XElement("ChartData",
                                      new XElement("isPan", chartData.isPan.ToString().ToUpper()),
                                      new XElement("showTickerLine", chartData.showTickerLine.ToString().ToUpper()),
                                      new XElement("showVolume", chartData.showVolume.ToString().ToUpper()),
                                      new XElement("showOverview", chartData.showOverview.ToString().ToUpper()),
                                      new XElement("BollingerBands", new XAttribute("Enable", chartData.showBollingerBands.ToString().ToUpper()),
                                                                     new XAttribute("Period", chartData.bollingerBandsPeriod.ToString()),
                                                                     new XAttribute("Deviation", chartData.bollingerBandsDeviation.ToString()),
                                                                     new XAttribute("OuterColour", chartData.bollingerBandsOuterColour.ToUpper()),
                                                                     new XAttribute("SMAColour", chartData.bollingerBandsSMAColour.ToUpper())),
                                      new XElement("SMA1", new XAttribute("Enable", chartData.showSMA1.ToString().ToUpper()),
                                                           new XAttribute("Period", chartData.sma1Period.ToString()),
                                                           new XAttribute("SMAColour", chartData.sma1Colour.ToUpper())),
                                      new XElement("SMA2", new XAttribute("Enable", chartData.showSMA2.ToString().ToUpper()),
                                                           new XAttribute("Period", chartData.sma2Period.ToString()),
                                                           new XAttribute("SMAColour", chartData.sma2Colour.ToUpper())),
                                      new XElement("SMA3", new XAttribute("Enable", chartData.showSMA3.ToString().ToUpper()),
                                                           new XAttribute("Period", chartData.sma3Period.ToString()),
                                                           new XAttribute("SMAColour", chartData.sma3Colour.ToUpper())),
                                      new XElement("Theme", chartData.theme.ToUpper()),
                                      new XElement("ChartPeriod", chartData.chartPeriod.ToString()),
                                      new XElement("MACD", new XAttribute("showMACDIndicator", chartData.showMACDIndicator.ToString().ToUpper()),
                                                           new XAttribute("macdFastPeriod", chartData.macdFastPeriod.ToString().ToUpper()),
                                                           new XAttribute("macdSlowPeriod", chartData.macdSlowPeriod.ToString().ToUpper()),
                                                           new XAttribute("macdSignalPeriod", chartData.macdSignalPeriod.ToString().ToUpper()),
                                                           new XAttribute("macdMACDColour", chartData.macdMACDColour.ToUpper()),
                                                           new XAttribute("macdSignalColour", chartData.macdSignalColour.ToUpper()),
                                                           new XAttribute("macdHistogramColour", chartData.macdHistogramColour.ToUpper())),
                                      new XElement("RSI", new XAttribute("showRSIIndicator", chartData.showRSIIndicator.ToString().ToUpper()),
                                                          new XAttribute("rsiPeriod", chartData.rsiPeriod.ToString().ToUpper()),
                                                          new XAttribute("rsiOverBoughtLine", chartData.rsiOverBoughtLine.ToString().ToUpper()),
                                                          new XAttribute("rsiOverSoldLine", chartData.rsiOverSoldLine.ToString().ToUpper()),
                                                          new XAttribute("rsiMidLine", chartData.rsiMidLine.ToString().ToUpper()),
                                                          new XAttribute("rsiColour", chartData.rsiColour.ToUpper()),
                                                          new XAttribute("rsiOverBoughtLineColour", chartData.rsiOverBoughtLineColour.ToUpper()),
                                                          new XAttribute("rsiOverSoldLineColour", chartData.rsiOverSoldLineColour.ToUpper()),
                                                          new XAttribute("rsiMidLineColour", chartData.rsiMidLineColour.ToUpper())),
                                      new XElement("WilliamsR", new XAttribute("showWilliamsRIndicator", chartData.showWilliamsRIndicator.ToString().ToUpper()),
                                                          new XAttribute("williamsRPeriod", chartData.williamsRPeriod.ToString().ToUpper()),
                                                          new XAttribute("williamsROverBoughtLine", chartData.williamsROverBoughtLine.ToString().ToUpper()),
                                                          new XAttribute("williamsROverSoldLine", chartData.williamsROverSoldLine.ToString().ToUpper()),
                                                          new XAttribute("williamsRMidLine", chartData.williamsRMidLine.ToString().ToUpper()),
                                                          new XAttribute("williamsRColour", chartData.williamsRColour.ToUpper()),
                                                          new XAttribute("williamsROverBoughtLineColour", chartData.williamsROverBoughtLineColour.ToUpper()),
                                                          new XAttribute("williamsROverSoldLineColour", chartData.williamsROverSoldLineColour.ToUpper()),
                                                          new XAttribute("williamsRMidLineColour", chartData.williamsRMidLineColour.ToUpper())),
                                      new XElement("Candlestick", new XAttribute("candlestickUpColour", chartData.candlestickUpColour.ToUpper()),
                                                          new XAttribute("candlestickDownColour", chartData.candlestickDownColour.ToUpper()))
                                      ));
                            xe.Add(xel);
                        }
                        else
                        {
                            XElement xeMC = customize.FirstOrDefault().Element("ChartData");
                            if (xeMC == null)
                            {
                                var xel = new XElement("ChartData",
                                          new XElement("isPan", chartData.isPan.ToString().ToUpper()),
                                          new XElement("showTickerLine", chartData.showTickerLine.ToString().ToUpper()),
                                          new XElement("showVolume", chartData.showVolume.ToString().ToUpper()),
                                          new XElement("showOverview", chartData.showOverview.ToString().ToUpper()),
                                          new XElement("BollingerBands", new XAttribute("Enable", chartData.showBollingerBands.ToString().ToUpper()),
                                                                         new XAttribute("Period", chartData.bollingerBandsPeriod.ToString()),
                                                                         new XAttribute("Deviation", chartData.bollingerBandsDeviation.ToString()),
                                                                         new XAttribute("OuterColour", chartData.bollingerBandsOuterColour.ToUpper()),
                                                                         new XAttribute("SMAColour", chartData.bollingerBandsSMAColour.ToUpper())),
                                          new XElement("SMA1", new XAttribute("Enable", chartData.showSMA1.ToString().ToUpper()),
                                                               new XAttribute("Period", chartData.sma1Period.ToString()),
                                                               new XAttribute("SMAColour", chartData.sma1Colour.ToUpper())),
                                          new XElement("SMA2", new XAttribute("Enable", chartData.showSMA2.ToString().ToUpper()),
                                                               new XAttribute("Period", chartData.sma2Period.ToString()),
                                                               new XAttribute("SMAColour", chartData.sma2Colour.ToUpper())),
                                          new XElement("SMA3", new XAttribute("Enable", chartData.showSMA3.ToString().ToUpper()),
                                                               new XAttribute("Period", chartData.sma3Period.ToString()),
                                                               new XAttribute("SMAColour", chartData.sma3Colour.ToUpper())),
                                          new XElement("Theme", chartData.theme.ToUpper()),
                                          new XElement("ChartPeriod", chartData.chartPeriod.ToString()),
                                          new XElement("MACD", new XAttribute("showMACDIndicator", chartData.showMACDIndicator.ToString().ToUpper()),
                                                               new XAttribute("macdFastPeriod", chartData.macdFastPeriod.ToString().ToUpper()),
                                                               new XAttribute("macdSlowPeriod", chartData.macdSlowPeriod.ToString().ToUpper()),
                                                               new XAttribute("macdSignalPeriod", chartData.macdSignalPeriod.ToString().ToUpper()),
                                                               new XAttribute("macdMACDColour", chartData.macdMACDColour.ToUpper()),
                                                               new XAttribute("macdSignalColour", chartData.macdSignalColour.ToUpper()),
                                                               new XAttribute("macdHistogramColour", chartData.macdHistogramColour.ToUpper())),
                                          new XElement("RSI", new XAttribute("showRSIIndicator", chartData.showRSIIndicator.ToString().ToUpper()),
                                                              new XAttribute("rsiPeriod", chartData.rsiPeriod.ToString().ToUpper()),
                                                              new XAttribute("rsiOverBoughtLine", chartData.rsiOverBoughtLine.ToString().ToUpper()),
                                                              new XAttribute("rsiOverSoldLine", chartData.rsiOverSoldLine.ToString().ToUpper()),
                                                              new XAttribute("rsiMidLine", chartData.rsiMidLine.ToString().ToUpper()),
                                                              new XAttribute("rsiColour", chartData.rsiColour.ToUpper()),
                                                              new XAttribute("rsiOverBoughtLineColour", chartData.rsiOverBoughtLineColour.ToUpper()),
                                                              new XAttribute("rsiOverSoldLineColour", chartData.rsiOverSoldLineColour.ToUpper()),
                                                              new XAttribute("rsiMidLineColour", chartData.rsiMidLineColour.ToUpper())),
                                          new XElement("WilliamsR", new XAttribute("showWilliamsRIndicator", chartData.showWilliamsRIndicator.ToString().ToUpper()),
                                                              new XAttribute("williamsRPeriod", chartData.williamsRPeriod.ToString().ToUpper()),
                                                              new XAttribute("williamsROverBoughtLine", chartData.williamsROverBoughtLine.ToString().ToUpper()),
                                                              new XAttribute("williamsROverSoldLine", chartData.williamsROverSoldLine.ToString().ToUpper()),
                                                              new XAttribute("williamsRMidLine", chartData.williamsRMidLine.ToString().ToUpper()),
                                                              new XAttribute("williamsRColour", chartData.williamsRColour.ToUpper()),
                                                              new XAttribute("williamsROverBoughtLineColour", chartData.williamsROverBoughtLineColour.ToUpper()),
                                                              new XAttribute("williamsROverSoldLineColour", chartData.williamsROverSoldLineColour.ToUpper()),
                                                              new XAttribute("williamsRMidLineColour", chartData.williamsRMidLineColour.ToUpper())),
                                          new XElement("Candlestick", new XAttribute("candlestickUpColour", chartData.candlestickUpColour.ToUpper()),
                                                          new XAttribute("candlestickDownColour", chartData.candlestickDownColour.ToUpper()))
                                      );
                                customize.FirstOrDefault().Add(xel);
                            }
                            else
                            {
                                xeMC.Element("isPan").Value = chartData.isPan.ToString().ToUpper();
                                xeMC.Element("showTickerLine").Value = chartData.showTickerLine.ToString().ToUpper();
                                xeMC.Element("showVolume").Value = chartData.showVolume.ToString().ToUpper();
                                xeMC.Element("showOverview").Value = chartData.showOverview.ToString().ToUpper();
                                xeMC.Element("BollingerBands").Attribute("Enable").Value = chartData.showBollingerBands.ToString().ToUpper();
                                xeMC.Element("BollingerBands").Attribute("Period").Value = chartData.bollingerBandsPeriod.ToString();
                                xeMC.Element("BollingerBands").Attribute("Deviation").Value = chartData.bollingerBandsDeviation.ToString();
                                xeMC.Element("BollingerBands").Attribute("OuterColour").Value = chartData.bollingerBandsOuterColour.ToUpper();
                                xeMC.Element("BollingerBands").Attribute("SMAColour").Value = chartData.bollingerBandsSMAColour.ToUpper();
                                xeMC.Element("SMA1").Attribute("Enable").Value = chartData.showSMA1.ToString().ToUpper();
                                xeMC.Element("SMA1").Attribute("Period").Value = chartData.sma1Period.ToString();
                                xeMC.Element("SMA1").Attribute("SMAColour").Value = chartData.sma1Colour.ToUpper();
                                xeMC.Element("SMA2").Attribute("Enable").Value = chartData.showSMA2.ToString().ToUpper();
                                xeMC.Element("SMA2").Attribute("Period").Value = chartData.sma2Period.ToString();
                                xeMC.Element("SMA2").Attribute("SMAColour").Value = chartData.sma2Colour.ToUpper();
                                xeMC.Element("SMA3").Attribute("Enable").Value = chartData.showSMA3.ToString().ToUpper();
                                xeMC.Element("SMA3").Attribute("Period").Value = chartData.sma3Period.ToString();
                                xeMC.Element("SMA3").Attribute("SMAColour").Value = chartData.sma3Colour.ToUpper();
                                xeMC.Element("Theme").Value = chartData.theme.ToUpper();
                                xeMC.Element("ChartPeriod").Value = chartData.chartPeriod.ToString();
                                xeMC.Element("MACD").Attribute("showMACDIndicator").Value = chartData.showMACDIndicator.ToString().ToUpper();
                                xeMC.Element("MACD").Attribute("macdFastPeriod").Value = chartData.macdFastPeriod.ToString().ToUpper();
                                xeMC.Element("MACD").Attribute("macdSlowPeriod").Value = chartData.macdSlowPeriod.ToString().ToUpper();
                                xeMC.Element("MACD").Attribute("macdSignalPeriod").Value = chartData.macdSignalPeriod.ToString().ToUpper();
                                xeMC.Element("MACD").Attribute("macdMACDColour").Value = chartData.macdMACDColour.ToUpper();
                                xeMC.Element("MACD").Attribute("macdSignalColour").Value = chartData.macdSignalColour.ToUpper();
                                xeMC.Element("MACD").Attribute("macdHistogramColour").Value = chartData.macdHistogramColour.ToUpper();
                                xeMC.Element("RSI").Attribute("showRSIIndicator").Value = chartData.showRSIIndicator.ToString().ToUpper();
                                xeMC.Element("RSI").Attribute("rsiPeriod").Value = chartData.rsiPeriod.ToString().ToUpper();
                                xeMC.Element("RSI").Attribute("rsiOverBoughtLine").Value = chartData.rsiOverBoughtLine.ToString().ToUpper();
                                xeMC.Element("RSI").Attribute("rsiOverSoldLine").Value = chartData.rsiOverSoldLine.ToString().ToUpper();
                                xeMC.Element("RSI").Attribute("rsiMidLine").Value = chartData.rsiMidLine.ToString().ToUpper();
                                xeMC.Element("RSI").Attribute("rsiColour").Value = chartData.rsiColour.ToUpper();
                                xeMC.Element("RSI").Attribute("rsiOverBoughtLineColour").Value = chartData.rsiOverBoughtLineColour.ToUpper();
                                xeMC.Element("RSI").Attribute("rsiOverSoldLineColour").Value = chartData.rsiOverSoldLineColour.ToUpper();
                                xeMC.Element("RSI").Attribute("rsiMidLineColour").Value = chartData.rsiMidLineColour.ToUpper();
                                xeMC.Element("WilliamsR").Attribute("showWilliamsRIndicator").Value = chartData.showWilliamsRIndicator.ToString().ToUpper();
                                xeMC.Element("WilliamsR").Attribute("williamsRPeriod").Value = chartData.williamsRPeriod.ToString().ToUpper();
                                xeMC.Element("WilliamsR").Attribute("williamsROverBoughtLine").Value = chartData.williamsROverBoughtLine.ToString().ToUpper();
                                xeMC.Element("WilliamsR").Attribute("williamsROverSoldLine").Value = chartData.williamsROverSoldLine.ToString().ToUpper();
                                xeMC.Element("WilliamsR").Attribute("williamsRMidLine").Value = chartData.williamsRMidLine.ToString().ToUpper();
                                xeMC.Element("WilliamsR").Attribute("williamsRColour").Value = chartData.williamsRColour.ToUpper();
                                xeMC.Element("WilliamsR").Attribute("williamsROverBoughtLineColour").Value = chartData.williamsROverBoughtLineColour.ToUpper();
                                xeMC.Element("WilliamsR").Attribute("williamsROverSoldLineColour").Value = chartData.williamsROverSoldLineColour.ToUpper();
                                xeMC.Element("WilliamsR").Attribute("williamsRMidLineColour").Value = chartData.williamsRMidLineColour.ToUpper();
                                xeMC.Element("Candlestick").Attribute("candlestickUpColour").Value = chartData.candlestickUpColour.ToUpper();
                                xeMC.Element("Candlestick").Attribute("candlestickDownColour").Value = chartData.candlestickDownColour.ToUpper();
                            }
                        }

                        Save(dataXml, AppFlag.UserData + AppFlag.CustomizeFile);
                        return true;

                    default:
                        return false;
                }
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "SaveCustomizeData(CustomizeData customizeData),error:" + exp.ToString());

                MessageBox.Show("Please try again later!");
                return false;
            }
        }
    }

    public class Server
    {
        private string _Name = "";
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
            }
        }
        private string _Ip = "";
        public string Ip
        {
            get { return _Ip; }
            set
            {
                _Ip = value;
            }
        }
        private string _Port = "";
        public string Port
        {
            get { return _Port; }
            set
            {
                _Port = value;
            }
        }
        private string _DisplayName = "";
        public string DisplayName
        {
            get { return _DisplayName; }
            set
            {
                _DisplayName = value;
            }
        }
        private bool _Selected = false;
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;
            }
        }
    }

    public class MarginColor
    {
        public int Percent { get; set; }
        public SolidColorBrush Color { get; set; }
    }

    public class BaseCustomizeData
    {
        public string UserID { get; set; }
        public string Type { get; set; }
    }

    public class MarginColorData : BaseCustomizeData
    {
        public bool isEnabled { get; set; }

        private List<MarginColor> _MarginColors = new List<MarginColor>();
        public List<MarginColor> MarginColors
        {
            get { return _MarginColors; }
            set { _MarginColors = value; }
        }
    }

    public class AlertData : BaseCustomizeData
    {
        public bool isOrderConfirmS { get; set; }
        public bool isReqAccAlertS { get; set; }
        public bool isReqErrAlertS { get; set; }
        public bool isTradeAlertS { get; set; }
        public bool isNewAppAlertS { get; set; }

        public bool isOrderConfirmT { get; set; }
        public bool isReqAccAlertT { get; set; }
        public bool isReqErrAlertT { get; set; }
        public bool isTradeAlertT { get; set; }
        public bool isTimeCorrAlertT { get; set; }

        public int NotifyLevel { get; set; }
        public bool isEnableWheel { get; set; }
        public bool isSingleClick { get; set; }
        public bool isDoubleClick { get; set; }
    }

    public class ChartData : BaseCustomizeData
    {
        public bool isPan { get; set; }
        public bool showTickerLine { get; set; }
        public bool showVolume { get; set; }
        public bool showOverview { get; set; }
        public bool showBollingerBands { get; set; }
        public uint bollingerBandsPeriod { get; set; }
        public double bollingerBandsDeviation { get; set; }
        public String bollingerBandsOuterColour { get; set; }
        public String bollingerBandsSMAColour { get; set; }
        public bool showSMA1 { get; set; }
        public uint sma1Period { get; set; }
        public String sma1Colour { get; set; }
        public bool showSMA2 { get; set; }
        public uint sma2Period { get; set; }
        public String sma2Colour { get; set; }
        public bool showSMA3 { get; set; }
        public uint sma3Period { get; set; }
        public String sma3Colour { get; set; }
        public String theme { get; set; }
        public uint chartPeriod { get; set; }
        public bool showMACDIndicator { get; set; }
        public uint macdFastPeriod { get; set; }
        public uint macdSlowPeriod { get; set; }
        public uint macdSignalPeriod { get; set; }
        public String macdMACDColour { get; set; }
        public String macdSignalColour { get; set; }
        public String macdHistogramColour { get; set; }
        public bool showRSIIndicator { get; set; }
        public uint rsiPeriod { get; set; }
        public uint rsiOverBoughtLine { get; set; }
        public uint rsiOverSoldLine { get; set; }
        public uint rsiMidLine { get; set; }
        public String rsiColour { get; set; }
        public String rsiOverBoughtLineColour { get; set; }
        public String rsiOverSoldLineColour { get; set; }
        public String rsiMidLineColour { get; set; }
        public bool showWilliamsRIndicator { get; set; }
        public uint williamsRPeriod { get; set; }
        public int williamsROverBoughtLine { get; set; }
        public int williamsROverSoldLine { get; set; }
        public int williamsRMidLine { get; set; }
        public String williamsRColour { get; set; }
        public String williamsROverBoughtLineColour { get; set; }
        public String williamsROverSoldLineColour { get; set; }
        public String williamsRMidLineColour { get; set; }
        public String candlestickUpColour { get; set; }
        public String candlestickDownColour { get; set; }

        public ChartData()
        {
            isPan = SCIConstants.SCI_DEFAULT_ISPAN;
            showTickerLine = SCIConstants.SCI_DEFAULT_SHOW_TICKER_LINE;
            showVolume = SCIConstants.SCI_DEFAULT_SHOW_VOLUME;
            showOverview = SCIConstants.SCI_DEFAULT_SHOW_OVERVIEW;
            showBollingerBands = SCIConstants.SCI_DEFAULT_SHOW_BOLLINGER_BANDS;
            bollingerBandsPeriod = SCIConstants.SCI_DEFAULT_BOLLINGER_BANDS_PERIOD;
            bollingerBandsDeviation = SCIConstants.SCI_DEFAULT_BOLLINGER_BANDS_DEVIATION;
            bollingerBandsOuterColour = SCIConstants.SCI_DEFAULT_BOLLINGER_BANDS_OUTER_COLOUR;
            bollingerBandsSMAColour = SCIConstants.SCI_DEFAULT_BOLLINGER_BANDS_SMA_COLOUR;
            showSMA1 = SCIConstants.SCI_DEFAULT_SHOW_SMA1;
            sma1Period = SCIConstants.SCI_DEFAULT_SMA1_PERIOD;
            sma1Colour = SCIConstants.SCI_DEFAULT_SMA1_COLOUR;
            showSMA2 = SCIConstants.SCI_DEFAULT_SHOW_SMA2;
            sma2Period = SCIConstants.SCI_DEFAULT_SMA2_PERIOD;
            sma2Colour = SCIConstants.SCI_DEFAULT_SMA2_COLOUR;
            showSMA3 = SCIConstants.SCI_DEFAULT_SHOW_SMA3;
            sma3Period = SCIConstants.SCI_DEFAULT_SMA3_PERIOD;
            sma3Colour = SCIConstants.SCI_DEFAULT_SMA3_COLOUR;
            theme = SCIConstants.SCI_DEFAULT_THEME;
            chartPeriod = SCIConstants.SCI_DEFAULT_CHART_PERIOD;
            showMACDIndicator = SCIConstants.SCI_DEFAULT_MACD_SHOW_MACD;
            macdFastPeriod = SCIConstants.SCI_DEFAULT_MACD_FAST_PERIOD;
            macdSlowPeriod = SCIConstants.SCI_DEFAULT_MACD_SLOW_PERIOD;
            macdSignalPeriod = SCIConstants.SCI_DEFAULT_MACD_SIGNAL_PERIOD;
            macdMACDColour = SCIConstants.SCI_DEFAULT_MACD_MACD_COLOUR;
            macdSignalColour = SCIConstants.SCI_DEFAULT_MACD_SIGNAL_COLOUR;
            macdHistogramColour = SCIConstants.SCI_DEFAULT_MACD_HISTOGRAM_COLOUR;
            showRSIIndicator = SCIConstants.SCI_DEFAULT_RSI_SHOW_RSI;
            rsiPeriod = SCIConstants.SCI_DEFAULT_RSI_PERIOD;
            rsiOverBoughtLine = SCIConstants.SCI_DEFAULT_RSI_OVERBOUGHT_LINE;
            rsiOverSoldLine = SCIConstants.SCI_DEFAULT_RSI_OVERSOLD_LINE;
            rsiMidLine = SCIConstants.SCI_DEFAULT_RSI_MID_LINE;
            rsiColour = SCIConstants.SCI_DEFAULT_RSI_COLOUR;
            rsiOverBoughtLineColour = SCIConstants.SCI_DEFAULT_RSI_OVERBOUGHT_LINE_COLOUR;
            rsiOverSoldLineColour = SCIConstants.SCI_DEFAULT_RSI_OVERSOLD_LINE_COLOUR;
            rsiMidLineColour = SCIConstants.SCI_DEFAULT_RSI_MID_LINE_COLOUR;
            showWilliamsRIndicator = SCIConstants.SCI_DEFAULT_WILLIAMSR_SHOW_WILLIAMSR;
            williamsRPeriod = SCIConstants.SCI_DEFAULT_WILLIAMSR_PERIOD;
            williamsROverBoughtLine = SCIConstants.SCI_DEFAULT_WILLIAMSR_OVERBOUGHT_LINE;
            williamsROverSoldLine = SCIConstants.SCI_DEFAULT_WILLIAMSR_OVERSOLD_LINE;
            williamsRMidLine = SCIConstants.SCI_DEFAULT_WILLIAMSR_MID_LINE;
            williamsRColour = SCIConstants.SCI_DEFAULT_WILLIAMSR_COLOUR;
            williamsROverBoughtLineColour = SCIConstants.SCI_DEFAULT_WILLIAMSR_OVERBOUGHT_LINE_COLOUR;
            williamsROverSoldLineColour = SCIConstants.SCI_DEFAULT_WILLIAMSR_OVERSOLD_LINE_COLOUR;
            williamsRMidLineColour = SCIConstants.SCI_DEFAULT_WILLIAMSR_MID_LINE_COLOUR;
            candlestickUpColour = SCIConstants.SCI_DEFAULT_CANDLESTICK_UP_COLOUR;
            candlestickDownColour = SCIConstants.SCI_DEFAULT_CANDLESTICK_DOWN_COLOUR;
        }
    }

    public class CustomizeData : BaseCustomizeData
    {
        //private MarginColorData _MarginColorData;
        //public MarginColorData MarginColorData
        //{
        //    get { return _MarginColorData; }
        //    set { _MarginColorData = value; OnPropertyChanged(new PropertyChangedEventArgs("MarginColorData")); }
        //}

        //private AlertData _AlertData;
        //public AlertData AlertData
        //{
        //    get { return _AlertData; }
        //    set { _AlertData = value; OnPropertyChanged(new PropertyChangedEventArgs("AlertData")); }
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

        private MarginColorData _MarginColorData;// =new MarginColorData ();
        public MarginColorData MarginColorData
        {
            get { return _MarginColorData; }
            set { _MarginColorData = value; }
        }

        private AlertData _AlertData;//=new AlertData ();
        public AlertData AlertData
        {
            get { return _AlertData; }
            set { _AlertData = value; }
        }

        private ChartData _ChartData;
        public ChartData ChartData
        {
            get { return _ChartData; }
            set { _ChartData = value; }
        }
    }

    public class CustomizeDataModel2
    {
        private CustomizeData _CustomizeDataes;

        public CustomizeData CustomizeDataes
        {
            get { return _CustomizeDataes; }
            set { _CustomizeDataes = value; }
        }

        public CustomizeDataModel2()
        {
            _CustomizeDataes = new CustomizeData();
        }

        public static CustomizeData Create2(string UserID, string Type)
        {
            CustomizeData data = new CustomizeData();
            data.Type = Type;
            data.UserID = UserID;

            switch (Type)
            {
                case "General":
                    data.AlertData = TradeStationSetting.ReturnCustomizeData(UserID, Type).AlertData;
                    break;
                case "Margin Color":
                    data.MarginColorData = TradeStationSetting.ReturnCustomizeData(UserID, Type).MarginColorData;
                    break;
                case "ChartData":
                    data.ChartData = TradeStationSetting.ReturnCustomizeData(UserID, Type).ChartData;
                    break;
            }
            return data;
        }

        //public static CustomizeDataModel2 Create(string UserID, string Type)
        //{
        //    CustomizeDataModel2 model = new CustomizeDataModel2();
        //    model._CustomizeDataes.Type = Type;
        //    model._CustomizeDataes.UserID = UserID;
        //    switch (Type)
        //    {
        //        case "General":
        //            model._CustomizeDataes.AlertData = TradeStationSetting.ReturnCustomizeData(UserID, Type).AlertData;
        //            break;
        //        case "Margin Color":
        //            model._CustomizeDataes.MarginColorData = TradeStationSetting.ReturnCustomizeData(UserID, Type).MarginColorData;
        //            break;
        //    }
        //    return model;
        //}
    }

    public class CustomizeDataModel// : INotifyPropertyChanged
    {
        private string _UserID;
        public string UserID
        {
            get { return _UserID; }
            //  set { _UserID = value; OnPropertyChanged(new PropertyChangedEventArgs("UserID")); }
            set { _UserID = value; }
        }

        private string _Type;
        public string Type
        {
            get { return _Type; }
            // set { _Type = value; OnPropertyChanged(new PropertyChangedEventArgs("Type")); }
            set { _Type = value; }
        }

        private MarginColorData _MarginColorData;
        public MarginColorData MarginColorData
        {
            get { return _MarginColorData; }
            //  set { _MarginColorData = value; OnPropertyChanged(new PropertyChangedEventArgs("MarginColorData")); }
            set { _MarginColorData = value; }
        }

        private AlertData _AlertData;
        public AlertData AlertData
        {
            get { return _AlertData; }
            //   set { _AlertData = value; OnPropertyChanged(new PropertyChangedEventArgs("AlertData")); }
            set { _AlertData = value; }
        }

        private ChartData _ChartData;
        public ChartData Chartdata
        {
            get { return _ChartData; }
            set { _ChartData = value; }
        }

        //public event PropertyChangedEventHandler PropertyChanged;
        //protected void OnPropertyChanged(PropertyChangedEventArgs propertyName)
        //{
        //    PropertyChangedEventHandler handler = this.PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, (propertyName));
        //    }
        //}


        //public CustomizeDataModel()
        //{

        //}

        //public CustomizeDataModel(string UserID, string Type)
        //{
        //    switch (Type)
        //    {
        //        case "General":
        //            _AlertData = TradeStationSetting.ReturnCustomizeData(UserID, Type).AlertData;
        //            break;
        //        case "Margin Color":
        //            _MarginColorData = TradeStationSetting.ReturnCustomizeData(UserID, Type).MarginColorData;
        //            break;
        //    }
        //}

        public static CustomizeDataModel Create(string UserID, string Type)
        {
            CustomizeDataModel model = new CustomizeDataModel();

            switch (Type)
            {
                case "General":
                    model.AlertData = TradeStationSetting.ReturnCustomizeData(UserID, Type).AlertData;
                    break;
                case "Margin Color":
                    model.MarginColorData = TradeStationSetting.ReturnCustomizeData(UserID, Type).MarginColorData;
                    break;
                case "ChartData":
                    model.Chartdata = TradeStationSetting.ReturnCustomizeData(UserID, Type).ChartData;
                    break;
            }
            return model;
        }
    }

    /// <summary>
    /// For update new version
    /// </summary> 
    ///  // the currently application info
    public struct AppInfo
    {
        public Version version;
        public string appName;
        public string updateUrl;
        public string date;
        public string tempFile;
    }

    // this struct will contain the info from the xml file
    public struct DownloadedVersionInfo
    {
        public bool error;
        public Version latestVersion;
        public string description;
        public string updateFileUrl;
        public List<string> updateFileList;
        public Version MinimumRequiredVersion;
          
    }

    // this will contain info about the downloaded installer
    public struct DownloadInstallerInfo
    {
        public bool error;
        public string path;
    }

    public class CheckForUpdate
    {
        public delegate bool DelegateCheckForUpdateFinished(DownloadedVersionInfo versionInfo, AppInfo appInfo);
        public event DelegateCheckForUpdateFinished CheckForUpdateFinished;
        public delegate void DelegateDownloadInstallerFinished(DownloadInstallerInfo info);
        public event DelegateDownloadInstallerFinished DownloadInstallerFinished;
        public delegate void DelegateDownloadedFinished(int num);
        public event DelegateDownloadedFinished DownloadedFinished;

        public delegate void DelegateReplacedFinished();
        public event DelegateReplacedFinished ReplacedFinished;

        private static string xmlFileUrl = AppFlag.VersionInfoFile;
        private static List<string> newFiles = null;

        Thread m_WorkerThread;
        // events used to stop worker thread
        readonly ManualResetEvent m_EventStopThread;
        readonly ManualResetEvent m_EventThreadStopped;


        public CheckForUpdate()
        {
            m_EventStopThread = new ManualResetEvent(false);
            m_EventThreadStopped = new ManualResetEvent(false);
        }

        // start the check for update process (if it is not already running)
        public void OnCheckForUpdate()
        {
            if ((this.m_WorkerThread != null) && (this.m_WorkerThread.IsAlive)) return;
            m_WorkerThread = new Thread(this.CheckForUpdateFunction);
            m_EventStopThread.Reset();
            m_EventThreadStopped.Reset();
            m_WorkerThread.Start();
        }

        // when the worker thread is running - let it know it should stop
        public void StopThread()
        {
            if ((this.m_WorkerThread != null) && this.m_WorkerThread.IsAlive)
            {
                StopWorkerThread();
                //m_EventStopThread.Set();
                //while (m_WorkerThread.IsAlive)
                //{
                //    if (WaitHandle.WaitAll(
                //                            (new ManualResetEvent[] { m_EventThreadStopped }),
                //                            100,
                //                            true))
                //    {
                //        break;
                //    }
                //}
            }
        }

        // internal method - return true when the thread is supposed to stop
        private bool StopWorkerThread()
        {
            if (m_EventStopThread.WaitOne(0, true))
            {
                m_EventThreadStopped.Set();
                return true;
            }
            return false;
        }

        AppInfo appInfo;
        DownloadedVersionInfo downloadedInfo;

        public void CheckForUpdateFunction()
        {
            appInfo = GetCurrentAppInfo();
            downloadedInfo = new DownloadedVersionInfo();

            downloadedInfo.error = true;
            downloadedInfo.updateFileUrl = "";
            downloadedInfo.updateFileList = new List<string>();

            try
            {
                XmlTextReader reader = new XmlTextReader(appInfo.updateUrl);

                reader.MoveToContent();
                string elementName = "";

                if (StopWorkerThread()) return;

                if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Production"))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            elementName = reader.Name;
                            if (elementName == "FileList")
                            {
                                downloadedInfo.updateFileUrl = reader.GetAttribute("SourcePath");
                            }
                            else if (elementName == "Item")
                            {
                                downloadedInfo.updateFileList.Add(reader.GetAttribute("Name"));
                            }
                        }
                        else
                        {
                            if ((reader.NodeType == XmlNodeType.Text) && (reader.HasValue))
                            {
                                switch (elementName)
                                {
                                    case "Version":
                                        downloadedInfo.latestVersion = new Version(reader.Value);
                                        break;
                                    case "Description":
                                        downloadedInfo.description = reader.Value;
                                        break;
                                    case "MinimumRequiredVersion":
                                        downloadedInfo.MinimumRequiredVersion =new Version( reader.Value);
                                        break;
                                }
                            }
                        }
                    }
                }
                reader.Close();

                downloadedInfo.error = false;
            }
            catch (Exception)
            {
                TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "Can not found " + appInfo.updateUrl + "! ");
            }
            if (StopWorkerThread()) return;

            bool download = this.CheckForUpdateFinished(downloadedInfo, appInfo);
            if (!download) return;

            if (DoenloadUpdateFile(downloadedInfo, appInfo))
            {
                //if (ReplaceFile())
                //{
                if (ReplaceUpgradeFile(appInfo))
                {
                    this.ReplacedFinished();
                }
                // this.SaveLocalXml();
                //}
            }
            else
            {
            }

        }

        public static bool ReplaceUpgradeFile(AppInfo appInfo)
        {
            try
            {
                string strPath = AppDomain.CurrentDomain.BaseDirectory + appInfo.tempFile;
                DirectoryInfo p_Source = new DirectoryInfo(strPath);
                if (Directory.Exists(p_Source.FullName))
                {
                    foreach (FileInfo _File in p_Source.GetFiles())
                    {
                        if (_File.Name.ToUpper() == "GOSTSUPGRADE.EXE")
                        {
                            string moveNewPath = AppDomain.CurrentDomain.BaseDirectory + _File.Name;
                            if (File.Exists(moveNewPath)) File.Delete(moveNewPath);
                            File.Move(AppDomain.CurrentDomain.BaseDirectory + appInfo.tempFile + "\\" + _File.Name, moveNewPath);
                            return true;
                        }
                    } 

                    foreach (DirectoryInfo _Folder in p_Source.GetDirectories())
                    {
                        foreach (FileInfo _File in _Folder.GetFiles())
                        {
                            if (_File.Name.ToUpper() == "GOSTSUPGRADE.EXE")
                            {
                                string moveNewPath = AppDomain.CurrentDomain.BaseDirectory + _File.Name;
                                if (File.Exists(moveNewPath)) File.Delete(moveNewPath);
                                File.Move(_Folder.FullName + "\\" + _File.Name, moveNewPath);
                                return true;
                            }
                        }
                    } 
                }
            }
            catch (System.Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " ReplaceUpgradeFile,error: " + exp.Message);
                return false;
            }
            return false;
        }

        private bool DoenloadUpdateFile(DownloadedVersionInfo downloadInfo, AppInfo appInfo)
        {
            try
            {
                if (!Directory.Exists(appInfo.tempFile))
                {
                    Directory.CreateDirectory(appInfo.tempFile);
                }
                int updateFileCount = downloadInfo.updateFileList.Count;
                for (int i = 0; i < updateFileCount; i++)
                {
                    using (WebClient wcDown = new WebClient())
                    {
                        try
                        {
                            string str = downloadInfo.updateFileUrl + downloadInfo.updateFileList[i];

                            TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " URL: " + str);

                            string strDir = appInfo.tempFile;
                            if (downloadInfo.updateFileList[i].IndexOf("/") > 0)
                            {
                                strDir = appInfo.tempFile + "\\" + downloadInfo.updateFileList[i].Substring(0, downloadInfo.updateFileList[i].IndexOf("/"));
                                if (!Directory.Exists(strDir))
                                {
                                    Directory.CreateDirectory(strDir);
                                }
                            }
                            wcDown.DownloadFile(str, AppDomain.CurrentDomain.BaseDirectory + appInfo.tempFile + "\\" + downloadInfo.updateFileList[i]);
                            Thread.Sleep(1000);

                            this.DownloadedFinished(i + 1);
                        }
                        catch (Exception ex)
                        {
                            TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " DoenloadUpdateFile,error: " + ex.Message);
                            if (i == updateFileCount - 1)
                            {
                                return false;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    Thread.Sleep(100);
                }

                return true;
            }
            catch (System.Exception ex)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "DoenloadUpdateFile()," + ex.Message);

                return false;
            }
        }

        /*
        public bool ReplaceFile()
        {

            try
            {
                //OpreateMainForm(true);
                foreach (string fileName in downloadedInfo.updateFileList)
                {
                    string moveNewPath = AppDomain.CurrentDomain.BaseDirectory + fileName;
                    if (File.Exists(moveNewPath)) File.Delete(moveNewPath);
                    File.Move(AppDomain.CurrentDomain.BaseDirectory + appInfo.tempFile + "\\" + fileName, moveNewPath);
                }

                SaveLocalXml();
                TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " Replaced file! ");

                return true;
            }
            catch (System.Exception ex)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "ERROR:" + ex.Message);

                return false;
            }
        }

        private void SaveLocalXml()
        {
            XmlDocument _xmlDoc = new XmlDocument();
            try
            {
                _xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + AppFlag.VersionInfoFile);
                XmlElement xmlContent = _xmlDoc.DocumentElement;

                xmlContent.SelectSingleNode("Version").InnerText = downloadedInfo.latestVersion.ToString();
                _xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory + AppFlag.VersionInfoFile);

            }
            catch (System.Exception ex)
            {

            }
        }

        private void OpreateMainForm(bool IsClose)
        {
            if (IsClose)
            {
                System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcesses();
                foreach (System.Diagnostics.Process p in ps)
                {
                    if (p.ProcessName.ToUpper() == "GOSTS".ToUpper())
                    {
                        TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "GOSTS.EXE IS KILLED! ");

                        p.Kill();
                        break;
                    }
                }
            }
            else
            {
                System.Diagnostics.Process.Start("GOSTS" + ".exe");
            }
        }
        */

        public static AppInfo GetCurrentAppInfo()
        {
            AppInfo info = new AppInfo();
            try
            {
                XmlTextReader reader = new XmlTextReader(xmlFileUrl);

                reader.MoveToContent();
                string elementName = "";
                Version version = null;
                string appName = "";
                string updataUrl = "";
                string updataDate = "";
                string tempFile = "";

                if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "AppInfo"))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element) elementName = reader.Name;
                        else
                        {
                            if ((reader.NodeType == XmlNodeType.Text) && (reader.HasValue))
                            {

                                switch (elementName)
                                {
                                    case "Version":
                                        version = new Version(reader.Value);
                                        break;
                                    case "AppName":
                                        appName = reader.Value;
                                        break;
                                    case "UpdateURL":
                                        if (!GOSTradeStation.isDealer)
                                        {
                                            updataUrl = reader.Value;
                                        }
                                        break;
                                    case "DealerUpdateURL":
                                        if (GOSTradeStation.isDealer)
                                        {
                                            updataUrl = reader.Value;
                                        }
                                        break;
                                    case "Date":
                                        updataDate = reader.Value;
                                        break;
                                    case "TempFile":
                                        tempFile = reader.Value; ;
                                        break;
                                }
                            }
                        }
                    }
                }
                reader.Close();

                info.version = version;
                info.appName = appName;
                info.updateUrl = updataUrl;
                info.date = updataDate;
                info.tempFile = tempFile;

            }
            catch (Exception)
            {
                TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "Can not found " + AppFlag.VersionInfoFile + "! ");
            }
            return info;
        }
    }
}