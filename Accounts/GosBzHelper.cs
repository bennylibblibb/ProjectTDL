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
using System.Windows.Shapes;
using GOSTS;
using GOSTS.Common;
using System.Data;
using System.Threading;
using WPF.MDI;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace GOSTS
{
    public class GosBzTool
    {
        #region Order
        public static ProductBoundInfo getProdBoundInfo(string _ProdCode)
        {
            ProductBoundInfo info = new ProductBoundInfo();
            info.Product = _ProdCode;
            DataRow dr = MarketPriceData.GetProductInfo(_ProdCode);
            if (dr != null)
            {
                info.DecLen = dr.getColIntValue("decInPrice", 0);
                if (info.DecLen < 0)
                {
                    info.DecLen = 0;
                }
                info.strPriceLower = dr.getColValue("priceLowerLimit");
                decimal? d = info.DecPriceLower;
                if (d.HasValue && info.DecLen > 0)
                {
                    int divisor = (int)(Math.Pow(10, info.DecLen));
                    d = d / divisor;
                    info.strPriceLower = d.Value.ToString();
                }
                info.strPriceUp = dr.getColValue("priceUpperLimit");
                d = info.DecPriceUp;
                if (d.HasValue && info.DecLen > 0)
                {
                    int divisor = (int)(Math.Pow(10, info.DecLen));
                    d = d / divisor;
                    info.strPriceUp = d.Value.ToString();
                }
                info.strQtyLower = dr.getColValue("priceLowerQty");
                info.strQtyUp = dr.getColValue("priceUpperQty");
            }
            return info;
        }
        public static string InvalidSvValue = "-2147483648";
        public static bool CheckDeviationPrice(decimal Price, MarketPriceItem MktItem)
        {

            if (MktItem == null) return true;
            decimal decComparePrice = 0M;
            string str = MktItem.Last;
            if (str != InvalidSvValue && str != "0")
            {
                try
                {
                    decComparePrice = Convert.ToDecimal(str);
                    return OrderConfigManager.CheckPriceDeviation(Price, decComparePrice);
                }
                catch { }
            }
            str = MktItem.EP;
            if (str != InvalidSvValue && str != "0")
            {
                try
                {
                    decComparePrice = Convert.ToDecimal(str);
                    return OrderConfigManager.CheckPriceDeviation(Price, decComparePrice);
                }
                catch { }
            }
            str = MktItem.PreClose;
            if (str != InvalidSvValue && str != "0")
            {
                try
                {
                    decComparePrice = Convert.ToDecimal(str);
                    return OrderConfigManager.CheckPriceDeviation(Price, decComparePrice);
                }
                catch { }
            }
            return true;
        }

        public static bool CheckDeviationQty(int Qty)
        {
            return OrderConfigManager.CheckQtyDeviation(Qty);
        }

        public static int Verify_Order_Qty_ByBound(int Qty, string ProdCode)//--- to re consider
        {
            int? intQtyUpBound = null, intDownBound = 0;
            OrderDeviationModel Model = new OrderDeviationModel();
            try
            {
                Model = OrderConfigManager.LoadConfig(GOSTradeStation.UserID);
            }
            catch { }
            if (Model != null)
            {
                //if (Model.Qty.HasValue)
                {
                    intQtyUpBound = Model.Qty;
                }
            }
            if (ProdCode != null)
                if (ProdCode.Trim() != "")
                {
                    ProductBoundInfo boundInfo = getProdBoundInfo(ProdCode);
                    if (boundInfo != null)
                    {
                        if (boundInfo.IntQtyLower.HasValue)
                        {
                            if (boundInfo.IntQtyLower.Value > intDownBound.Value)
                            {
                                intDownBound = boundInfo.IntQtyLower.Value;
                            }
                        }
                        if (boundInfo.IntQtyUp.HasValue)
                        {
                            if (intQtyUpBound.HasValue == false)
                            {
                                intQtyUpBound = boundInfo.IntQtyUp.Value;
                            }
                            else if (boundInfo.IntQtyUp.Value < intQtyUpBound.Value)
                            {
                                intQtyUpBound = boundInfo.IntQtyUp.Value;
                            }
                        }
                    }
                }

            if (intQtyUpBound == intDownBound)
            {
                if (intQtyUpBound.HasValue)
                {
                    return intQtyUpBound.Value;
                }
            }
            if (intQtyUpBound.HasValue && intDownBound.HasValue)
                if (intQtyUpBound < intDownBound)
                {
                    return Qty;
                }
            if (intQtyUpBound.HasValue)
            {
                if (Qty > intQtyUpBound.Value)
                {
                    return intQtyUpBound.Value;
                }
            }
            if (intDownBound.HasValue)
            {
                if (Qty < intDownBound.Value)
                {
                    return intDownBound.Value;
                }
            }
            return Qty;
        }

        /// <summary>
        /// 是否不是trigger order
        /// </summary>
        /// <param name="Cond"></param>
        /// <returns></returns>
        public static bool CanChangeOrder(string Cond)
        {
            if (Cond == null) return true;
            if (Cond.IndexOf(">=") >= 0 || Cond.IndexOf("<=") >= 0)
            {
                return false;
            }
            //if (Cond.Trim() == "") return true;
            //if (Cond.Trim() == "AO") return true;
            return true;
        }

        public static TradeStationComm.Attribute.StopType getStopTypeByOrderBookStatus(string strStop)
        {
            TradeStationComm.Attribute.StopType StopNomal = TradeStationComm.Attribute.StopType.normalOrder;
            if (strStop == null || strStop == "")
            {
                return StopNomal;
            }
            strStop = strStop.ToUpper();
            if (strStop.IndexOf("SL") > -1)
            {
                StopNomal = TradeStationComm.Attribute.StopType.stopLoss;//sell
            }
            else if (strStop.IndexOf("UP") > -1)
            {
                StopNomal = TradeStationComm.Attribute.StopType.upTrigger;//sell
            }
            else if (strStop.IndexOf("DN") > -1)
            {
                StopNomal = TradeStationComm.Attribute.StopType.downTrigger;//sell
            }
            //switch (strStop)
            //{
            //    case "SL<=":
            //        StopNomal = TradeStationComm.Attribute.StopType.stopLoss;//sell
            //        break;
            //    case "SL>=":
            //        StopNomal = TradeStationComm.Attribute.StopType.stopLoss;//Buy
            //        break;
            //    case "UP>=":
            //        StopNomal = TradeStationComm.Attribute.StopType.upTrigger; 
            //        break;
            //    case "DN<=":
            //        StopNomal = TradeStationComm.Attribute.StopType.downTrigger;
            //        break;
            //}
            return StopNomal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="B64Str"></param>
        /// <param name="i">1,status no,2 or others,status name</param>
        /// <returns></returns>
        public static string getBase64StatusInfo(string B64Str, int i)
        {
            if (B64Str == null) return "";
            int pos = B64Str.IndexOf(";");
            if (pos < 0)
            {
                return B64Str;
            }
            switch (i)
            {
                case 1:
                    string no = B64Str.Substring(0, pos);
                    return no;
                    break;
                default:
                    string str = System.Text.RegularExpressions.Regex.Replace(B64Str, @"^\d+;", "");
                    str = TradeStationTools.Base64StringToString(str);
                    return str;
            }
        }
        #endregion

        public static int getDecLen(string prodCode)
        {
            if (prodCode == null) return 0;
            if (prodCode.Trim() == "") return 0;           
            return MarketPriceData.GetDecInPrice(prodCode);
        }

        public static double getExChange(string currency)
        {
            return MarketPriceData.GetRate(currency);         
        }

        public static string getCurrency(string prodCode)
        {
            return MarketPriceData.GetCcy(prodCode);        
        }

        /// <summary>
        /// calculate indeed decimal through getting product' decInPrice length
        /// </summary>
        /// <param name="prodCode"></param>
        /// <param name="Price"></param>
        /// <returns></returns>
        public static decimal getDecimalPrice(string prodCode, string Price)
        {
            int len = getDecLen(prodCode);
            try
            {
                decimal d = Convert.ToDecimal(Price);
                if (len <= 0) return d;
                checked
                {
                    int divisor = (int)(Math.Pow(10, len));
                    d = d / divisor;
                }
                return d;
            }
            catch
            {
                return 0.0M;
            }
        }


        public static string CalAndAdjustDecimalPrice(string prodCode, string Price)
        {
            int len = getDecLen(prodCode);           
            try
            {
                decimal d = Convert.ToDecimal(Price);
                if (len < 1)
                {
                    return Convert.ToInt32(Price).ToString();
                }

                checked
                {
                    int divisor = (int)(Math.Pow(10, len));
                    d = d / divisor;
                }

                string r = "";
                r = r.PadLeft(len, '0');
                string str = d.ToString("#0." + r);//位数超出时会截断并四舍五入

                return str;
            }
            catch
            {
                return "0";
            }
        }

        public static string getAndAdjustAveragePrice(string prodCode, string Price)
        {
            if (Price == null)
            {
                return "0";
            }

            int len = getDecLen(prodCode);
            try
            {
                decimal d = Convert.ToDecimal(Price);
                if (len < 1)
                {
                    return Price;
                }

                checked
                {
                    int divisor = (int)(Math.Pow(10, len));
                    d = d / divisor;
                }
                Price = Price.Trim();
                len += 2; //均价要求比decinprice多两位,取消下面的如果数的小数位数多于len就以实际数位数为准。

                //int pos = Price.IndexOf(".");
                //if (pos > -1)
                //{
                //    int len1 = Price.Length - 1 - pos;
                //    if (len < len1)
                //    {
                //        len = len1;
                //    }
                //}
                string r = "";
                r = r.PadLeft(len, '0');
                string str = d.ToString("#0." + r);//位数超出时会截断并四舍五入

                return str;
            }
            catch
            {
                return "0";
            }
        }



        public static int ChangeDecToIn(string prodCode, decimal decPrice)
        {
            int len = getDecLen(prodCode);
            int power = (int)Math.Pow(10, len);
            decimal d = decPrice * power;
            return (int)d;
        }


        #region 调整小数位数以便小数为零时，仍显示小位的位数

        public static decimal adjustDecLength(string prodCode, decimal decPrice)
        {
            int len = getDecLen(prodCode);
            if (len < 1) return decPrice;
            string r = "";
            r = r.PadLeft(len, '0');
            string str = decPrice.ToString("#0." + r);
            return Utility.ConvertToDecimal(str);

            //System.Globalization.NumberFormatInfo provider=new System.Globalization.NumberFormatInfo();
            //provider.NumberDecimalDigits =len;

        }

        public static decimal adjustDecLength(string prodCode, string strPrice)
        {
            int len = getDecLen(prodCode);
            decimal decPrice = Utility.ConvertToDecimal(strPrice);
            if (len < 1) return decPrice;
            string str = string.Format("{0:N" + len.ToString() + "}", decPrice).Replace(",", "");
            return Utility.ConvertToDecimal(str);

        }

        public static string adjustDecLengthToString(string prodCode, string strPrice)
        {
            int len = getDecLen(prodCode);
            decimal decPrice = Utility.ConvertToDecimal(strPrice);
            if (len < 1) return decPrice.ToString();
            string str = string.Format("{0:N" + len.ToString() + "}", decPrice).Replace(",", "");
            return str;
        }

        public static string adjustDecLengthToString(string prodCode, decimal decPrice)
        {
            if (prodCode == "CHTU3")
            {

            }
            int len = getDecLen(prodCode);
            if (len < 1) return decPrice.ToString();
            string str = string.Format("{0:N" + len.ToString() + "}", decPrice).Replace(",", "");
            return str;

        }

        public static string NFormatString(string strDecimal, int len)
        {
            decimal decPrice = Utility.ConvertToDecimal(strDecimal);
            if (decPrice == 0 || len < 1)
            {
                return "0";
            }
            string str = string.Format("{0:N" + len.ToString() + "}", decPrice);
            return str;
        }
        #endregion


        public static decimal CheckPriceBound(decimal d)
        {
            //if (d > AppFlag.PriceUpBound)
            //{
            //    d = AppFlag.PriceUpBound;
            //}
            //if (d < AppFlag.PriceDownBound)
            //{
            //    d = AppFlag.PriceDownBound;
            //}
            return d;
        }

        public static bool CheckPriceOverBound(decimal d)
        {
            //if (d > AppFlag.PriceUpBound)
            //{
            //    return true;
            //}
            //if (d < AppFlag.PriceDownBound)
            //{
            //    return true;
            //}
            return false;
        }

        public static bool CheckQtyOverBound(int qty)
        {
            //if (qty > AppFlag.QtyUpBound)
            //{
            //    return true;
            //}
            if (qty <= AppFlag.QtyDownBound)
            {
                return true;
            }
            return false;
        }


        public static bool CheckDecLength(string prodCode, decimal dec)
        {
            decimal decInt = Math.Truncate(dec);
            if ((dec - decInt) != 0)
            {
                int l = GosBzTool.getDecLen(prodCode);
                string strTicks = dec.ToString();
                int pos = strTicks.IndexOf('.');
                if (pos > 0)
                {
                    if (l == 0)
                    {
                        MessageBox.Show("no decimal digit allowed for product '" + prodCode + "' ");
                        return false;
                    }
                    int decLen = strTicks.Length - pos - 1;
                    if (decLen > l)
                    {
                        MessageBox.Show("only " + l.ToString() + " number of decimal digit allowed for product '" + prodCode + "'");
                        return false;
                    }
                }
            }
            return true;
        }



        public static void setTitle(MdiChild mchild, MessageDistribute odistributeMsg, string NewTitle)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                if (mchild == null)
                    return;
                if (odistributeMsg == null) return;
                try
                {
                    if (mchild.Title != null)
                    {
                        string oldTitle = mchild.Title;
                        if (oldTitle == NewTitle)
                        {
                            return;
                        }
                    }
                    string defaultTitle = mchild.Title;
                    mchild.Title = NewTitle;
                    odistributeMsg.DistributeControlChangeTitle(defaultTitle, mchild);
                }
                catch (Exception EX)
                {
                    MessageBox.Show(EX.Message);
                }
            }), null);
        }



        public static GosCulture.LabelTextExtension getTitleLabel(Type _O)
        {
            try
            {
                GosCulture.LabelTextExtension ltTitleExtension = null;

                string sType = _O.Name.ToString().ToLower();
                string key = sType + "Title";
                if (ltTitleExtension == null)
                {
                    ltTitleExtension = new GosCulture.LabelTextExtension(key);
                }
                return ltTitleExtension;
            }
            catch (Exception ex) { }
            return null;
        }

        public static void OpenCHPWD(MessageDistribute _MsgDist)
        {
            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
            MdiChild _MdiChild = new MdiChild();
            bool bCreatedNew = false;
            ChangePWD cp = ChangePWD.GetChPWD(_MsgDist, _MdiChild, ref bCreatedNew);
            if (bCreatedNew)
            {

                _MdiChild.Content = cp;
                _MdiChild.Width = 300;
                _MdiChild.Height = 180;
                Container.Children.Add(_MdiChild);
                double left = 350, top = 150;

                double ParentWidth = Container.Width;
                if (Double.IsNaN(ParentWidth))
                {
                    ParentWidth = Container.ActualWidth;
                }
                double ParentHeight = Container.Height;
                if (Double.IsNaN(ParentHeight))
                {
                    ParentHeight = Container.ActualHeight;
                }
                if (!Double.IsNaN(ParentWidth))
                {
                    left = (ParentWidth - _MdiChild.Width) / 2;
                }
                //if (!Double.IsNaN(ParentHeight))
                //{
                //    top = (ParentWidth - _MdiChild.Height) / 2; 
                //}
                _MdiChild.Position = new System.Windows.Point(left, top);
            }
        }

        #region changeOrder Sizes
        public static double ChOrderWidth = 320;
        public static double ChOrderHeight = 250;
        #endregion


        public static void CloseCHPWD()
        {
            //for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
            //{
            //    if (App.Current.Windows[intCounter] is ChangePWD)
            //    {
            //        App.Current.Windows[intCounter].Close();
            //    }
            //}
            ChangePWD.Clear();
        }



        #region check if AO value
        public static bool IsAOValue(string str)
        {
            if (str == null) return false;
            if (AppFlag.intAOConst.ToString().Trim() == str || AppFlag.strAOConst == str.Trim().ToUpper())
            {
                return true;
            }
            return false;
        }

        public static int SetIntAoValue()
        {
            return AppFlag.intAOConst;
        }
        #endregion

        //未测试，未使用 for validation situation from server: 1;wxas....
        public static TradeStationComm.Attribute.ValidType getValidTypeFromSVData(string strSVData)
        {
            bool bHasLeftFlag = false;
            TradeStationComm.Attribute.ValidType enumValid = TradeStationComm.Attribute.ValidType.today;
            int posSemicolon=strSVData.IndexOf(";");
            if(posSemicolon>-1)
            {
                string strLeft = strSVData.Substring(0, posSemicolon);
                if (strLeft.Trim() != "")
                {
                    int intValid = Utility.ConvertToInt(strLeft, -1);
                    if (intValid != -1)
                    {
                       enumValid= getValidType(intValid);
                       bHasLeftFlag = true;
                    }
                }
            }
            if (!bHasLeftFlag)
            {
                if(posSemicolon + 1 <=strSVData.Length)
                {
                    string strRight = strSVData.Substring(posSemicolon + 1).Trim();                
                    try
                    {
                        enumValid = (TradeStationComm.Attribute.ValidType)Enum.Parse(typeof(TradeStationComm.Attribute.ValidType), strRight, true);
                    }
                    catch
                    {
                        try
                        {
                            enumValid = CommonRsText.getEnumValidType(strRight);
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
            return enumValid;
        }

        public static TradeStationComm.Attribute.ValidType getValidType(int intValid)
        {
            if (intValid > -1 && intValid < 5)
            {
                return (TradeStationComm.Attribute.ValidType)intValid;
            }
            return TradeStationComm.Attribute.ValidType.today;
        }
    }
}
