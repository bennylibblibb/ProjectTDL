using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPF.MDI;
using GOSTS.WPFControls;
using System.Collections;
using System.Xml.Linq;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace GOSTS.Common
{
    public class PriceAlertData : INotifyPropertyChanged
    {
        private string _No;
        public string No
        {
            get { return _No; }
            set { _No = value; OnPropertyChanged(new PropertyChangedEventArgs("No")); }
        }

        private string _ProductCode;
        public string ProductCode
        {
            get { return _ProductCode; }
            set { _ProductCode = value; }
        }

        private string _ProductName;
        public string ProductName
        {
            get { return _ProductName; }
            set { _ProductName = value; OnPropertyChanged(new PropertyChangedEventArgs("ProductName")); }
        }

        private string _AlertType;
        public string AlertType
        {
            get { return _AlertType; }
            set { _AlertType = value; OnPropertyChanged(new PropertyChangedEventArgs("AlertType")); }
        }

        private string _Market;
        public string Market
        {
            get { return _Market; }
            set { _Market = value; OnPropertyChanged(new PropertyChangedEventArgs("Market")); }
        }
        private string _Above;
        public string Above
        {
            get { return _Above; }
            set { _Above = value; OnPropertyChanged(new PropertyChangedEventArgs("Above")); }
        }

        private string _Below;
        public string Below
        {
            get { return _Below; }
            set { _Below = value; OnPropertyChanged(new PropertyChangedEventArgs("Below")); }
        }

        private string _Action;
        public string Action
        {
            get { return _Action; }
            set { _Action = value; OnPropertyChanged(new PropertyChangedEventArgs("Action")); }
        }
        private string _Status;
        public string Status
        {
            get { return _Status; }
            set { _Status = value; OnPropertyChanged(new PropertyChangedEventArgs("Status")); }
        }

        private string _AlertedTime;
        public string AlertedTime
        {
            get { return _AlertedTime; }
            set { _AlertedTime = value; OnPropertyChanged(new PropertyChangedEventArgs("AlertedTime")); }
        }

        private string _ArrivedType;
        public string ArrivedType
        {
            get { return _ArrivedType; }
            set { _ArrivedType = value; OnPropertyChanged(new PropertyChangedEventArgs("ArrivedType")); }
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

    public class DesktopData
    {
        public int Type { get; set; }
        public bool Locked { get; set; }
        public Double Height { get; set; }
        public Double Width { get; set; }
        public Double X { get; set; }
        public Double Y { get; set; }
        public string Code { get; set; }
        public List<string> Codes { get; set; }
        public bool ArrayMode { get; set; }
        public String chartSettings { get; set; }
    }

    class TradeStationDesktop
    {
        private static XDocument Load(bool Default)
        {
            try
            {
                if (GOSTradeStation.m_ChangedUser == true)
                {
                    if (!File.Exists(AppFlag.UserData + GOSTradeStation.UserID + "-" + "GOSTS.XML"))
                    {
                        File.Copy( AppFlag.Url,  AppFlag.UserData + GOSTradeStation.UserID + "-" + "GOSTS.XML");
                    }
                    TradeStationSetting.UpdateConfig("ConfigFile", GOSTradeStation.UserID + "-" + "GOSTS.XML");
                    AppFlag.ConfigFile = GOSTradeStation.UserID + "-" + "GOSTS.XML";
                    return XDocument.Load(AppFlag.UserData + GOSTradeStation.UserID + "-" + "GOSTS.XML");
                }
                else
                {
                    if (Default)
                    {
                        if (!File.Exists(  AppFlag.UserData + GOSTradeStation.UserID + "-" + "GOSTS.XML"))
                        {
                            File.Copy( AppFlag.Url,  AppFlag.UserData + GOSTradeStation.UserID + "-" + "GOSTS.XML");
                        }
                        if (AppFlag.DefaultUser != "")
                        {
                            TradeStationSetting.UpdateConfig("ConfigFile", GOSTradeStation.UserID + "-" + "GOSTS.XML");
                        }
                        AppFlag.ConfigFile = GOSTradeStation.UserID + "-" + "GOSTS.XML";
                        return XDocument.Load( AppFlag.UserData + GOSTradeStation.UserID + "-" + "GOSTS.XML");
                    }
                    else
                    {
                        if (File.Exists( AppFlag.UserData + AppFlag.ConfigFile) && AppFlag.DefaultUser != "")
                        {
                            return XDocument.Load( AppFlag.UserData + AppFlag.ConfigFile);
                        }
                        else
                        {
                            if (!File.Exists( AppFlag.UserData + GOSTradeStation.UserID + "-" + "GOSTS.XML"))
                            {
                                File.Copy(AppFlag.Url,  AppFlag.UserData + GOSTradeStation.UserID + "-" + "GOSTS.XML");
                            }
                            if (AppFlag.DefaultUser != "")
                            {
                                TradeStationSetting.UpdateConfig("ConfigFile", GOSTradeStation.UserID + "-" + "GOSTS.XML");
                            }
                            AppFlag.ConfigFile = GOSTradeStation.UserID + "-" + "GOSTS.XML";
                            return XDocument.Load(  AppFlag.UserData + GOSTradeStation.UserID + "-" + "GOSTS.XML");
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TradeStationDesktop.Load(bool Default),Error:" + exp.ToString());
                return null;
            }
        }
        private static XDocument Load(string fileName)
        {
            try
            {
                if (!File.Exists(  AppFlag.UserData + AppFlag.ConfigFile))
                {
                    File.Copy(AppFlag.Url,  AppFlag.UserData + AppFlag.ConfigFile);
                }
                return XDocument.Load( AppFlag.UserData + AppFlag.ConfigFile);
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TradeStationDesktop.Load(string  fileName),Error:" + exp.ToString());
                return null;
            }
        }
        private static void Save(XDocument xDocument, string url)
        {
            try
            {
                xDocument.Save(url);
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TradeStationDesktop.Save(),Error:" + exp.ToString());
            }
        }

        public static List<DesktopData> ParserXmlWindow(string url)
        {
            List<DesktopData> lsDesktopData = new List<DesktopData>();
            DesktopData data;
            try
            {
                var dataXml = XDocument.Load(url);
                IEnumerable<XNode> atr = dataXml.Root.Element("Windows").Nodes();
                foreach (XElement item in atr)
                {
                    try
                    {
                        data = new DesktopData();
                        data.Type = Convert.ToInt32(item.Attribute("Type").Value);
                        data.Locked = (item.Element("Locked").Value == "0") ? false : true;
                        data.Height = (item.Element("Height").Value == "" || item.Element("Height").Value == "NaN") ? 0 : Convert.ToInt32(item.Element("Height").Value);
                        data.Width = (item.Element("Width").Value == "" || item.Element("Width").Value == "NaN") ? 0 : Convert.ToInt32(item.Element("Width").Value);
                        string[] Position = item.Element("Position").Value.Split(new Char[] { ',' });
                        data.X = Convert.ToInt32(Position[0]);
                        data.Y = Convert.ToInt32(Position[1]);
                        data.Code = item.Attribute("Code").Value;
                        data.ArrayMode = (item.Element("ArrayMode") == null) ? false : (item.Element("ArrayMode").Value == "Vertical") ? true : false;
                        data.Codes = new List<string>();
                        if (data.Type != 0)
                        {
                            data.Codes = null;
                        }
                        else
                        {
                            XElement xe = item.Element("Items");
                            if (xe != null)
                            {
                                // var result = from window in dataXml.Descendants("Item")
                                var result = from window in xe.Descendants("Item")
                                             select new
                                             {
                                                 Codes = window.Value,
                                                 Name = window.Attribute("Name")
                                             };
                                foreach (var s in result)
                                {
                                    data.Codes.Add(s.Name.Value + "," + s.Codes);
                                }
                            }
                        }

                        if (data.Type == (int)WindowTypes.SCIChartAnalysis)
                        {
                            data.chartSettings = (item.Element("chartSettings") == null) ? "" : item.Element("chartSettings").Value;
                        }
                        else
                        {
                            data.chartSettings = null;
                        }

                        lsDesktopData.Add(data);
                    }
                    catch
                    {
                        TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TradeStationDesktop.ParserXmlWindow(string url),Paser Error,Type:" + item.Attribute("Type").Value);
                        continue;
                    }
                }
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " ParserXmlWindow(string url)" + exp.Message);
                return null;
            }
            return lsDesktopData;
        }

        public static List<DesktopData> ParserXmlWindow(bool Default)
        {
            List<DesktopData> lsDesktopData = new List<DesktopData>();
            DesktopData data;
            var dataXml = Load(Default);
            IEnumerable<XNode> atr = dataXml.Root.Element("Windows").Nodes();
            foreach (XElement item in atr)
            {
                try
                {
                    if (item.Attribute("Type").Value == ((int)WindowTypes.PriceAlert ).ToString ())
                    {
                        continue;
                    }

                    data = new DesktopData();
                    data.Type = Convert.ToInt32(item.Attribute("Type").Value);
                    data.Locked = (item.Element("Locked").Value == "0") ? false : true;
                    data.Height = (item.Element("Height").Value == "" || item.Element("Height").Value == "NaN") ? 0 : Convert.ToInt32(item.Element("Height").Value);
                    data.Width = (item.Element("Width").Value == "" || item.Element("Width").Value == "NaN") ? 0 : Convert.ToInt32(item.Element("Width").Value);
                    string[] Position = item.Element("Position").Value.Split(new Char[] { ',' });
                    data.X = Convert.ToInt32(Position[0]);
                    data.Y = Convert.ToInt32(Position[1]);
                    data.Code = item.Attribute("Code").Value;
                    data.ArrayMode = (item.Element("ArrayMode") == null) ? false : (item.Element("ArrayMode").Value == "Vertical") ? true : false;
                    data.Codes = new List<string>();
                    if (data.Type != (int)WindowTypes.MarketPriceControl )
                    {
                        data.Codes = null;
                    }
                    else
                    {
                        XElement xe = item.Element("Items");
                        if (xe != null)
                        {
                            var result = from window in xe.Descendants("Item")
                                         select new
                                         {
                                             Codes = window.Value,
                                             Name = window.Attribute("Name")
                                         };
                            foreach (var s in result)
                            {
                                data.Codes.Add(s.Name.Value + "," + s.Codes);
                            }
                        }
                    }

                    if (data.Type == (int)WindowTypes.SCIChartAnalysis)
                    {
                        data.chartSettings = (item.Element("chartSettings") == null) ? "" : item.Element("chartSettings").Value;
                    }
                    else
                    {
                        data.chartSettings = null;
                    }

                    lsDesktopData.Add(data);
                }
                catch
                {
                    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TradeStationDesktop.ParserXmlWindow(bool Default),Paser Error,Type:" + item.Attribute("Type").Value);
                    continue;
                }
            }
            return lsDesktopData;
        }

        public static void WriteCDATACodes(String mode, String strFormName, String strHeader, String strRename, List<string> lsCodes)
        {
            var dataXml = Load(AppFlag.ConfigFile);
            if (dataXml == null) return;

            IEnumerable<XNode> atr = dataXml.Root.Element("Windows").Nodes();
            foreach (XElement item in atr)
            {
                try
                {
                    int type = Convert.ToInt32(item.Attribute("Type").Value);
                    string code = item.Attribute("Code").Value;
                    if (type == 0)
                    {
                        if (code == strFormName)
                        {
                            XElement xe = item.Element("Items");
                            if (xe != null)
                            {
                                var result = from s in xe.Descendants("Item")
                                             select s;
                                //List<string> lsGridProd = new List<string>();
                                //lsGridProd=result.AsEnumerable().Select(d => d.Attribute("Name").Value).ToList<string>();
                                if (result.AsEnumerable().Select(d => d.Attribute("Name").Value).ToList<string>().Contains(strHeader))
                                {
                                    foreach (var s in result)
                                    {
                                        if (mode == "ADD")
                                        {
                                            if (s.Attribute("Name").Value == strHeader)
                                            {
                                                XElement dataNode = s.Descendants("Codes").First();
                                                ((XCData)dataNode.FirstNode).Value = s.Value + string.Concat("", string.Join(" ", lsCodes), " ");
                                                //  dataXml.Save(AppFlag.Url);
                                            }
                                        }
                                        else if (mode == "MOVE")
                                        {
                                            if (s.Attribute("Name").Value == strHeader)
                                            {
                                                s.Remove();
                                                // dataXml.Save(AppFlag.Url);
                                                break;
                                            }
                                        }
                                        else if (mode == "MOVECODE")
                                        {
                                            if (s.Attribute("Name").Value == strHeader)
                                            {
                                                XElement dataNode = s.Descendants("Codes").First();
                                                string newCodes = s.Value;
                                                foreach (string str in lsCodes)
                                                {
                                                    newCodes = newCodes.Replace(str + " ", "");
                                                }
                                                ((XCData)dataNode.FirstNode).Value = newCodes;
                                                // dataXml.Save(AppFlag.Url);
                                                break;
                                            }
                                        }
                                        else if (mode == "RENAME")
                                        {
                                            if (s.Attribute("Name").Value == strHeader)
                                            {
                                                s.Attribute("Name").Value = strRename;
                                                // dataXml.Save(AppFlag.Url);
                                                break;
                                            }
                                        }
                                        else if (mode == "SORT")
                                        {
                                            if (s.Attribute("Name").Value == strHeader)
                                            {
                                                XElement dataNode = s.Descendants("Codes").First();
                                                ((XCData)dataNode.FirstNode).Value = string.Concat("", string.Join(" ", lsCodes), " ");
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //have no tab 
                                    XElement xEle = new XElement("Item");
                                    xe.Add(xEle);
                                    XAttribute xAttr;
                                    if (strRename == null)
                                    {
                                        xAttr = new XAttribute("Name", strHeader);
                                    }
                                    else
                                    {
                                        xAttr = new XAttribute("Name", strRename);
                                    }
                                    xEle.Add(xAttr);
                                    XElement cEle = new XElement("Codes");
                                    xEle.Add(cEle);
                                    XCData xcdata = (lsCodes != null) ? new XCData(string.Concat("", string.Join(" ", lsCodes), " ")) : new XCData("");
                                    cEle.Add(xcdata);
                                    // dataXml.Save(AppFlag.Url);
                                }
                            }
                        }
                        else
                        {
                            //have no marketprice  
                        }
                    }
                }
                catch
                {
                    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TradeStationDesktop.WriteCDATACodes(),Write Error,Type:" + item.Attribute("Type").Value);
                    continue;
                }
            }

            string url =   AppFlag.UserData + AppFlag.ConfigFile;
            if (File.Exists(url))
            {
                // dataXml.Save(url);
                Save(dataXml, url);
            }
            else
            {
                // dataXml.Save(AppFlag.Url);
                Save(dataXml, AppFlag.Url);
            }
        }

        public static void UpdateXmlData(WindowTypes windowType, string Code, string AppKey, string AppValue)
        {
            var dataXml = Load(false);
            IEnumerable<XNode> atr = dataXml.Root.Element("Windows").Nodes();
            foreach (XElement item in atr)
            {
                if (item.Attribute("Type").Value == ((int)WindowTypes.PriceAlert).ToString())
                {
                    continue;
                }

                int type = Convert.ToInt32(item.Attribute("Type").Value);
                string code = item.Attribute("Code").Value;
                if (type == (int)windowType)
                {
                    if (code == Code)
                    {
                        item.Element(AppKey).Value = AppValue;
                        break;
                    }
                }
            }

            string url =   AppFlag.UserData + AppFlag.ConfigFile;
            if (File.Exists(url))
            {
                //dataXml.Save(url);
                Save(dataXml, url);
            }
            else
            {
                //dataXml.Save(AppFlag.Url);
                Save(dataXml, AppFlag.Url);
            }
        }

        public static void UpdateXmlAttribute(WindowTypes windowType, string Code, string AppValue)
        {
            // var dataXml = Load(false);
            var dataXml = Load(AppFlag.ConfigFile);
            IEnumerable<XNode> atr = dataXml.Root.Element("Windows").Nodes();
            foreach (XElement item in atr)
            {
                int type = Convert.ToInt32(item.Attribute("Type").Value);
                string code = item.Attribute("Code").Value;
                if (type == (int)windowType)
                {
                    if (code == Code)
                    {
                        item.Attribute("Code").Value = AppValue;
                        break;
                    }
                }
            }

            string url =   AppFlag.UserData + AppFlag.ConfigFile;
            if (File.Exists(url))
            {
                Save(dataXml, url);
            }
            else
            {
                Save(dataXml, AppFlag.Url);
            }
        }

        public static bool SaveDesktop(string fileName, List<DesktopData> lsDesktopData)
        {
            try
            {
                XDocument xdoc = new XDocument(
                    new XElement("Gosts", new XAttribute("UserName", GOSTradeStation.UserID),
                                          new XAttribute("DateTime", DateTime.Now.ToString("yyyy/MM/dd HH:mm")),
                    new XElement("Windows",
                        from window in lsDesktopData
                        where window.Type != -1 && window.Type != 1 && window.Type != 0 && (window.Type != (int)WindowTypes.SCIChartAnalysis)
                        select new XElement("Window",
                                new XAttribute("Type", window.Type),
                                new XAttribute("Code", (window.Code == null) ? "" : window.Code),
                                new XElement("Locked", (window.Locked == true) ? 1 : 0),
                                new XElement("Height", window.Height.Equals(double.NaN) ? double.NaN : Convert.ToInt32(window.Height)),
                                new XElement("Width", window.Width.Equals(double.NaN) ? double.NaN : Convert.ToInt32(window.Width)),
                                new XElement("Position", Convert.ToInt32(window.X) + "," + Convert.ToInt32(window.Y))
                                              ),
                        from window in lsDesktopData
                        where window.Type == 1
                        select new XElement("Window",
                               new XAttribute("Type", window.Type),
                               new XAttribute("Code", (window.Code == null) ? "" : window.Code),
                               new XElement("Locked", (window.Locked == true) ? 1 : 0),
                               new XElement("Height", window.Height.Equals(double.NaN) ? double.NaN : Convert.ToInt32(window.Height)),
                               new XElement("Width", window.Width.Equals(double.NaN) ? double.NaN : Convert.ToInt32(window.Width)),
                               new XElement("Position", Convert.ToInt32(window.X) + "," + Convert.ToInt32(window.Y)),
                               new XElement("ArrayMode", (window.ArrayMode == true) ? "Vertical" : "Horizontal")
                                             ),

                        from window in lsDesktopData
                        where window.Type == 0
                        select new XElement("Window",
                                new XAttribute("Type", window.Type),
                                new XAttribute("Code", (window.Code == null) ? "" : window.Code),
                                new XElement("Locked", (window.Locked == true) ? 1 : 0),
                                new XElement("Height", window.Height.Equals(double.NaN) ? double.NaN : Convert.ToInt32(window.Height)),
                                new XElement("Width", window.Width.Equals(double.NaN) ? double.NaN : Convert.ToInt32(window.Width)),
                                new XElement("Position", Convert.ToInt32(window.X) + "," + Convert.ToInt32(window.Y)),
                                new XElement("Items",
                                    from str in window.Codes
                                    where window.Codes != null
                                    select new XElement("Item", new XAttribute("Name", (str == null) ? "" : str.Substring(0, str.IndexOf(","))),
                                        new XElement("Codes", new XCData((str == null) ? "" : (str.Substring(str.IndexOf(",") + 1, str.Length - str.IndexOf(",") - 1) == " ") ? "" : str.Substring(str.IndexOf(",") + 1, str.Length - str.IndexOf(",") - 1)))))
                                        ),

                        from window in lsDesktopData
                        where window.Type == (int)WindowTypes.SCIChartAnalysis
                        select new XElement("Window",
                               new XAttribute("Type", window.Type),
                               new XAttribute("Code", (window.Code == null) ? "" : window.Code),
                               new XElement("Locked", (window.Locked == true) ? 1 : 0),
                               new XElement("Height", window.Height.Equals(double.NaN) ? double.NaN : Convert.ToInt32(window.Height)),
                               new XElement("Width", window.Width.Equals(double.NaN) ? double.NaN : Convert.ToInt32(window.Width)),
                               new XElement("Position", Convert.ToInt32(window.X) + "," + Convert.ToInt32(window.Y)),
                               new XElement("chartSettings", (window.chartSettings == null) ? "" : window.chartSettings)
                                        )
                                        )));

                Save(xdoc, fileName);
                return true;
                //if (TradeStationSetting.UpdateConfig("ConfigFile", fileName))
                //{
                //    Save(xdoc, AppFlag.UserData + fileName);
                //} 
            }
            catch
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TradeStationDesktop.SaveDesktop()");
                return false;
            }
        }

        /// <summary>
        /// Return price alerts and history
        /// </summary>
        /// <param name="bAlert">true - history/ false alerts</param>
        /// <returns></returns>
        public static ObservableCollection<PriceAlertData> ReturnPriceAlerts(bool bAlert)
        {
            ObservableCollection<PriceAlertData> data = new ObservableCollection<PriceAlertData>();
            PriceAlertData priceAlertData;

            var dataXml = Load(AppFlag.ConfigFile);
            if (dataXml == null) return null;

            IEnumerable<XNode> atr = dataXml.Root.Element("Windows").Nodes();
            if (atr == null) return null;
            try
            {
                foreach (XElement item in atr)
                {
                    int type = Convert.ToInt32(item.Attribute("Type").Value);
                    if (type == (int)WindowTypes.PriceAlert)
                    {
                        XElement xe = item.Element("Items");
                        if (xe != null)
                        {
                            var result = from s in xe.Descendants("Item")
                                         select s;
                            foreach (var s in result)
                            {
                                priceAlertData = new PriceAlertData();
                                priceAlertData.No = s.Attribute("No").Value;
                                priceAlertData.ProductCode = s.Attribute("Code").Value;
                                priceAlertData.ProductName = MarketPriceData.GetProductName(priceAlertData.ProductCode);
                                priceAlertData.AlertType = s.Attribute("AlertType").Value;
                                priceAlertData.Market = s.Attribute("ArrivedPrice").Value;
                                priceAlertData.Above = s.Attribute("Above").Value;
                                priceAlertData.Below = s.Attribute("Below").Value;
                                priceAlertData.Action = s.Attribute("Action").Value;
                                priceAlertData.Status = s.Attribute("Status").Value;
                                priceAlertData.AlertedTime = s.Attribute("AlertedTime").Value;
                                priceAlertData.ArrivedType = s.Attribute("ArrivedType").Value;
                                if (bAlert && priceAlertData.Status == "")
                                {
                                    data.Add(priceAlertData);
                                }
                                else if (!bAlert && priceAlertData.Status != "" && priceAlertData.Status != "3")
                                {
                                    data.Add(priceAlertData);
                                }
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TradeStationDesktop.ReturnPriceAlerts(),error:" + exp.ToString());
                return null;
            }

            return data;
        }

        /// <summary>
        /// Triger alert
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateTrigerAlerts(PriceAlertData data)
        {
            var dataXml = Load(AppFlag.ConfigFile);
            if (dataXml == null) return false;

            IEnumerable<XNode> atr = dataXml.Root.Element("Windows").Nodes();
            if (atr == null) return false;
            try
            {
                foreach (XElement item in atr)
                {
                    int type = Convert.ToInt32(item.Attribute("Type").Value);
                    if (type == (int)WindowTypes.PriceAlert)
                    {
                        XElement xe = item.Element("Items");
                        if (xe != null)
                        {
                            var result = from s in xe.Descendants("Item")
                                         select s;
                            foreach (var s in result)
                            {
                                if (s.Attribute("No").Value == data.No && s.Attribute("Status").Value != "")
                                {
                                    s.Attribute("AlertType").Value = data.AlertType;
                                    //s.Attribute("ArrivedPrice").Value = "";
                                    s.Attribute("Above").Value = data.Above;
                                    s.Attribute("Below").Value = data.Below;
                                    s.Attribute("Action").Value = data.Action;
                                    s.Attribute("Status").Value = data.Status;
                                    //s.Attribute("AlertedTime").Value = "";
                                    s.Attribute("ArrivedType").Value = (data.Status == "2") ? "Both" : (data.ArrivedType == "Both") ? "" : data.ArrivedType;
                                    break;
                                }
                            }

                            if (data.ArrivedType == "Both" || data.ArrivedType == "") break;

                            int m_count = 1;
                            foreach (var s in result)
                            {
                                if (s.Attribute("Status").Value == "")
                                {
                                    m_count++;
                                }
                            }
                            var xeItem = new XElement("Item",
                           new XAttribute("No", m_count),
                           new XAttribute("Code", data.ProductCode),
                           new XAttribute("AlertType", data.AlertType),
                           new XAttribute("ArrivedPrice", data.Market),
                           new XAttribute("Above", data.Above),
                           new XAttribute("Below", data.Below),
                           new XAttribute("Action", data.Action),
                           new XAttribute("Status", ""),
                           new XAttribute("AlertedTime", DateTime.Now.ToString("yyyy/MM/dd HH:mm fff")),
                           new XAttribute("ArrivedType", data.ArrivedType));
                            xe.Add(xeItem);

                            break;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TradeStationDesktop.UpdatePriceAlerts(PriceAlertData data),error:" + exp.ToString());
                return false;
            }

            Save(dataXml,   AppFlag.UserData + AppFlag.ConfigFile);
            return true;
        }

        /// <summary>
        /// Add new alert
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddPriceAlerts(PriceAlertData data)
        {
            var dataXml = Load(AppFlag.ConfigFile);
            if (dataXml == null) return -1;

            try
            {
                bool isExisted = false;
                IEnumerable<XNode> atr = dataXml.Root.Element("Windows").Nodes();
                foreach (XElement item in atr)
                {
                    int type = Convert.ToInt32(item.Attribute("Type").Value);
                    if (type == (int)WindowTypes.PriceAlert)
                    {
                        isExisted = true;
                        break;
                    }
                }
                XElement xeWindows = dataXml.Element("Gosts").Element("Windows");
                XElement xelWindow = null; ;
                if (!isExisted)
                {
                    xelWindow = new XElement("Window", new XAttribute("Type", (int)WindowTypes.PriceAlert), new XElement("Items"));
                    xeWindows.Add(xelWindow);
                }
                else
                {
                    foreach (XElement item in xeWindows.Nodes())
                    {
                        if (item.Attribute("Type").Value == ((int)WindowTypes.PriceAlert).ToString())
                        {
                            xelWindow = item;
                            break;
                        }
                    }
                }
                XElement xeItems = xelWindow.Element("Items");
                if (xeItems == null)
                {
                    xeItems = new XElement("Items");
                    xelWindow.Add(xeItems);

                }

                int m_count = 1;
                foreach (XElement item in xeItems.Nodes())
                {
                    if (item.Attribute("Status").Value != "")
                    {
                        m_count++;
                    }
                }

                var xeItem = new XElement("Item",
                                new XAttribute("No", m_count),
                                new XAttribute("Code", data.ProductCode),
                                new XAttribute("AlertType", data.AlertType),
                                new XAttribute("ArrivedPrice", data.Market),
                                new XAttribute("Above", data.Above),
                                new XAttribute("Below", data.Below),
                                new XAttribute("Action", data.Action),
                                new XAttribute("Status", data.Status),
                                new XAttribute("AlertedTime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                                new XAttribute("ArrivedType", data.ArrivedType));
                xeItems.Add(xeItem);

                Save(dataXml,   AppFlag.UserData + AppFlag.ConfigFile);
                return m_count;
            }
            catch
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TradeStationDesktop.SavePriceAlerts(),Write Error,Code:" + data.ProductCode);
                return -1;
            }
        }

        /// <summary>
        /// Update alert
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdatePriceAlert(PriceAlertData data)
        {
            var dataXml = Load(AppFlag.ConfigFile);
            if (dataXml == null) return false;

            IEnumerable<XNode> atr = dataXml.Root.Element("Windows").Nodes();
            if (atr == null) return false;
            try
            {
                foreach (XElement item in atr)
                {
                    int type = Convert.ToInt32(item.Attribute("Type").Value);
                    if (type == (int)WindowTypes.PriceAlert)
                    {
                        XElement xe = item.Element("Items");
                        if (xe != null)
                        {
                            var result = from s in xe.Descendants("Item")
                                         select s;
                            foreach (var s in result)
                            {
                                if (s.Attribute("No").Value == data.No && s.Attribute("Status").Value != "")
                                {
                                    s.Attribute("AlertType").Value = data.AlertType;
                                    //s.Attribute("ArrivedPrice").Value = "";
                                    s.Attribute("Above").Value = data.Above;
                                    s.Attribute("Below").Value = data.Below;
                                    s.Attribute("Action").Value = data.Action;
                                    s.Attribute("Status").Value = data.Status;
                                    s.Attribute("AlertedTime").Value = data.AlertedTime;
                                    s.Attribute("ArrivedType").Value = (data.Status == "2") ? "Both" : (data.ArrivedType == "Both") ? "" : data.ArrivedType;
                                    break;
                                }
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TradeStationDesktop.UpdatePriceAlerts(PriceAlertData data),error:" + exp.ToString());
                return false;
            }

            Save(dataXml,   AppFlag.UserData + AppFlag.ConfigFile);
            return true;
        }

         /// <summary>
        /// Delete PriceAlert or history
        /// </summary>
        /// <param name="strType">"History" or alert</param>
        /// <param name="strNo">No</param>
        /// <returns></returns>
        public static bool DeletePriceAlert(string strType,string strNo)
        {
            var dataXml = Load(AppFlag.ConfigFile);
            if (dataXml == null) return false;

            IEnumerable<XNode> atr = dataXml.Root.Element("Windows").Nodes();
            if (atr == null) return false;
            try
            {
                foreach (XElement item in atr)
                {
                    int type = Convert.ToInt32(item.Attribute("Type").Value);
                    if (type == (int)WindowTypes.PriceAlert)
                    {
                        XElement xe = item.Element("Items");
                        if (xe != null)
                        {
                            var result = from s in xe.Descendants("Item")
                                         select s;
                            foreach (var s in result)
                            {
                                 
                                    if (strNo == s.Attribute("No").Value && s.Attribute("Status").Value == "" && strType == "History")
                                    {
                                        s.Remove();
                                        break;
                                    }
                                    else if (strNo == s.Attribute("No").Value && s.Attribute("Status").Value != "" && strType != "History")
                                    {
                                        s.Remove();
                                        break;
                                    }   
                            }

                            var result2 = from s in xe.Descendants("Item")
                                         select s;
                            int no = 0;
                            foreach (var s in result2)
                            { 
                                if ( s.Attribute("Status").Value == "" && strType == "History")
                                {
                                    no++;
                                    s.Attribute("No").Value = no.ToString();
                                }
                                else if (  s.Attribute("Status").Value != "" && strType != "History")
                                {
                                    no++;
                                    s.Attribute("No").Value = no.ToString();
                                } 
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "TradeStationDesktop.DeletePriceAlert(List<string> lsNo),error:" + exp.ToString());
                return false;
            }

            Save(dataXml,   AppFlag.UserData + AppFlag.ConfigFile);
            return true;
        }

   }
}