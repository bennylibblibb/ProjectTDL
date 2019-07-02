using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Xml;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using GOSTS;

namespace GOSTS.Preference
{
    /// <summary>
    /// OrderLimit.xaml 的交互逻辑
    /// </summary>
    public partial class OrderLimit : UserControl
    {
        OrderDeviationModel Model = new OrderDeviationModel();
        public OrderLimit()
        {
            InitializeComponent();
            btnSave.Click += btnSave_Click;
            Load();           
        }

        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }
        void Load()
        {
            Model = OrderConfigManager.LoadConfig(GOSTradeStation.UserID);
            if (Model == null) return;
            this.txtQty.Text = Model.Qty.ToString().Trim();
            this.txtPriceDeviation.Text = Model.Price.HasValue ? Model.Price.ToString().Trim() : "";
        }
        public void Save()
        {
            string strQty = this.txtQty.Text.Trim();
            string strPrice = this.txtPriceDeviation.Text.Trim();
            if (strQty != "")
            {
                //MessageBox.Show("Please input qty deviation");
                //return;
                string strErr = GOSTS.GosCulture.CultureHelper.GetString("OLR_Invalid_Qty");
                try
                {
                    int Qty = Convert.ToInt32(strQty);
                    if (Qty < 0)
                    {
                        MessageBox.Show(strErr);
                        return;
                    }
                    Model.Qty = Qty;
                }
                catch
                {
                    MessageBox.Show(strErr);
                    return;
                }
            }
            else
            {
                Model.Qty = null;
            }

            if (strPrice != "")
            {               
                string strErrPrice = GOSTS.GosCulture.CultureHelper.GetString("OLR_Invalid_Price");
                try
                {
                    decimal decPrice = Convert.ToDecimal(strPrice);
                    if (decPrice < 0)
                    {

                        MessageBox.Show(strErrPrice);
                        return;
                    }
                    Model.Price = decPrice;
                }
                catch
                {
                    MessageBox.Show(strErrPrice);
                    return;
                }
            }
            else
            {
                Model.Price = null;
            }

            if (OrderConfigManager.SaveConfig(Model) == false)
            {
                string _err = GOSTS.GosCulture.CultureHelper.GetString("input_err_saveFailed");
                MessageBox.Show(_err);
            }
        }

        public bool bCheckChange()
        {
            string strQty = this.txtQty.Text.Trim();
            string strPrice = this.txtPriceDeviation.Text.Trim();
            if (Model == null)
            {
                if (strQty != "" || strPrice != "")
                    return true;              
            }
            else
            {
                if (Model.Qty.HasValue == false)
                {
                    if (strQty != "")
                    {
                        return true;
                    }
                }
                else if (Model.Qty.ToString() != strQty)
                {
                    return true;
                }

                if (Model.Price.HasValue == false)
                {
                    if (strPrice != "")
                    {
                        return true;
                    }
                }
                else if (Model.Price.ToString() != strPrice)
                {
                   return true;                   
                }
                            
            }
            return false;
        }
    }
}

namespace GOSTS
{
    public class OrderDeviationModel
    {
        private decimal? _price =null;
        public decimal? Price { get { return _price; } set { _price = value; } }

        private int? _qty = null;
        public int? Qty { get { return _qty; } set { _qty = value; } }
    }
    public class OrderConfigManager
    {
        static string FileName = @"UserData\accpref.xml";
        static string xmlpath = System.AppDomain.CurrentDomain.BaseDirectory + FileName;

        public static OrderDeviationModel LoadConfig(string UID)
        {
            OrderDeviationModel Model = new OrderDeviationModel();
            XDocument XD;
            try
            {
                XD = XDocument.Load(xmlpath);  //;  ("Account")
            }
            catch { return Model; }

            XElement cs = XD.Root.Element("Customizes");
            if (cs == null) return Model;
            bool bGetDefaultConfig = false;
            var result = from acc in cs.Elements("Customize")
                         where (string)acc.Attribute("ID") == UID
                         select acc;
            if (result.Count() > 0)
            {
                var custom = result.FirstOrDefault();
                if (custom != null)
                {
                    var ele = custom.Element("OrderDeviation");
                    if (ele != null)
                    {
                        string strPrice = ele.Element("Price").Value;
                        string strQty = ele.Element("Qty").Value;
                        try
                        {
                            Model.Price = Convert.ToDecimal(strPrice);
                        }
                        catch { Model.Price = null; }
                        try
                        {
                            Model.Qty = Convert.ToInt32(strQty);
                        }
                        catch { Model.Qty = null; }
                    }
                    else
                    {
                        bGetDefaultConfig = true;
                    }
                }
                else
                {
                    bGetDefaultConfig = true;
                }
            }
            else
            {
                bGetDefaultConfig = true;
            }
            if (bGetDefaultConfig)
            {
                result = from acc in cs.Elements("Customize")
                         where (string)acc.Attribute("ID") == "DEFAULT"
                         select acc;
                if (result.Count() > 0)
                {
                    var custom = result.FirstOrDefault();
                    if (custom != null)
                    {
                         var ele = custom.Element("OrderDeviation");
                         if (ele != null)
                         {
                             string strPrice = ele.Element("Price").Value;
                             string strQty = ele.Element("Qty").Value;
                             try
                             {
                                 Model.Price = Convert.ToDecimal(strPrice);
                             }
                             catch { Model.Price = null; }
                             try
                             {
                                 Model.Qty = Convert.ToInt32(strQty);
                             }
                             catch { Model.Qty = null; }
                         }
                    }
                }
            }
            
            return Model;
        }

        public static bool SaveConfig(OrderDeviationModel Model)
        {
             XDocument XD;
             try
             {
                 XD = XDocument.Load(xmlpath);
                 XElement cs = XD.Root.Element("Customizes");
                 if (cs == null)
                 {
                     cs = new XElement("Customizes");
                     XD.Add(cs);
                 }
                 // cs;

                 XElement ele;
                 var result = from acc in cs.Elements("Customize")
                              where (string)acc.Attribute("ID") == GOSTradeStation.UserID
                              select acc;
                 if (result.Count() <= 0)
                 {
                     ele = new XElement("Customize", new XAttribute("ID", GOSTradeStation.UserID));
                     cs.Add(ele);
                 }
                 else
                 {
                     ele = result.FirstOrDefault();
                 }
                 result = from acc in ele.Elements("OrderDeviation")
                          select acc;
                 //where (string)acc.Attribute("ID") == GOSTradeStation.UserID
                 if (result.Count() <= 0)
                 {
                     var ele1 = new XElement("OrderDeviation",
                         new XElement("Price", Model.Price.HasValue ? Model.Price.ToString().Trim() : "")
                         , new XElement("Qty", Model.Qty.HasValue?Model.Qty.Value.ToString():""));
                     ele.Add(ele1);
                     ele = ele1;
                 }
                 else
                 {
                     ele = result.FirstOrDefault();
                     ele.Element("Price").Value = Model.Price.HasValue ? Model.Price.ToString().Trim() : "";
                     ele.Element("Qty").Value = Model.Qty.HasValue?Model.Qty.ToString():"";
                 }

                 XD.Save(xmlpath);
                 MessageBox.Show("saved successfullly");
                 return true;
             }
             catch { return false; }
        }

        public static bool CheckQtyDeviation(int Qty)
        {          
            OrderDeviationModel Model = OrderConfigManager.LoadConfig(GOSTradeStation.UserID);
            if (Model == null)
            {
                //Model = new OrderDeviationModel();
                return true;
            }
            if (Model.Qty.HasValue == false)
            {
                return true;
            }
            string Msg = "";
            if (Model.Qty.Value < Qty)
            {
                string _msgFormat = GOSTS.GosCulture.CultureHelper.GetString("Qty_Deviation_MoreThan");
                Msg = string.Format(_msgFormat, Model.Qty.ToString());// "qty is more than deviation qty " + Model.Qty.ToString() + ",continue to trade?";
            }
            if (Msg != "")
            {
                string _strMsgTitle = GOSTS.GosCulture.CultureHelper.GetString("ORL_Limit_Warnning");
                if (MessageBoxResult.Yes == MessageBox.Show(Msg, _strMsgTitle, MessageBoxButton.YesNo))
                {
                    return true; ;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckPriceDeviation(decimal Price,decimal ComparePrice)
        {          
            OrderDeviationModel Model = OrderConfigManager.LoadConfig(GOSTradeStation.UserID);
            if (Model == null)
            {
                //Model = new OrderDeviationModel();
                return true;
            }
            if (Model.Price.HasValue == false)
            {
                return true;
            }
            string Msg = "";
            if (Model.Price.HasValue)
            {
                decimal DeviationPrice=ComparePrice * (Model.Price.Value /100);
                if (ComparePrice >= 0)
                {
                    decimal BoundPrice = ComparePrice - DeviationPrice;
                    BoundPrice = Math.Floor(BoundPrice);
                    if (BoundPrice > Price)
                    {
                        string _msgFormat = GOSTS.GosCulture.CultureHelper.GetString("Price_Deviation_Low");
                        string _Msg = string.Format(_msgFormat, BoundPrice.ToString());
                        Msg +=_Msg;
                       // Msg += "\n price is over lower deviation " + BoundPrice.ToString() + " ,continue to trade?";
                    }
                    else
                    {
                        BoundPrice = ComparePrice + DeviationPrice;
                        BoundPrice = Math.Ceiling(BoundPrice);
                        if (BoundPrice < Price)
                        {
                            string _msgFormat = GOSTS.GosCulture.CultureHelper.GetString("Price_Deviation_Exceed");
                            string _Msg = string.Format(_msgFormat, BoundPrice.ToString());
                            Msg +=_Msg;                            
                           // Msg += "\n price is over upper deviation " + BoundPrice.ToString() + " ,continue to trade?";
                        }
                    }
                }
                else
                {
                    decimal BoundPrice = ComparePrice + DeviationPrice;
                    BoundPrice = Math.Floor(BoundPrice);
                    if (BoundPrice > Price)
                    {
                        string _msgFormat = GOSTS.GosCulture.CultureHelper.GetString("Price_Deviation_Low");
                        string _Msg = string.Format(_msgFormat, BoundPrice.ToString());
                        Msg += _Msg;
                        //Msg += "\n price is over lower deviation " + BoundPrice.ToString() + " ,continue to trade?";     
                    }
                    else
                    {
                        BoundPrice = ComparePrice - DeviationPrice;
                        BoundPrice = Math.Ceiling(BoundPrice);
                        if (BoundPrice < Price)
                        {
                            string _msgFormat = GOSTS.GosCulture.CultureHelper.GetString("Price_Deviation_Exceed");
                            string _Msg = string.Format(_msgFormat, BoundPrice.ToString());
                            Msg += _Msg; 
                            //Msg += "\n price is over upper deviation " + BoundPrice.ToString() + " ,continue to trade?";         
                        }
                    }
                }
            }
            if (Msg != "")
            {
                string _strMsgTitle = GOSTS.GosCulture.CultureHelper.GetString("ORL_Limit_Warnning");

                if (MessageBoxResult.Yes == MessageBox.Show(Msg, _strMsgTitle, MessageBoxButton.YesNo))
                {
                    return true; ;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
