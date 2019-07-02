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
    public class CommonHelper
    {
    }

    public class CommonRsText
    {
        #region Deviation
        public static string StrRs_Qty_Deviation_MoreThan
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("Qty_Deviation_MoreThan"); }
        }

        public static string StrRs_Price_Deviation_Low
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("Price_Deviation_Low"); }
        }

        public static string StrRs_Price_Deviation_Exceed
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("Price_Deviation_Exceed"); }
        }

        #endregion

        public static string strRs_Price_Shlnot_ULimit
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("Price_Shlnot_ULimit"); }
        }

        public static string strRs_Price_Shlnot_LowLimit
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("Price_Shlnot_LowLimit");
            }
        }

        public static string strRs_Qty_UP_Limit
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("Qty_UP_Limit");
            }            
        }

        public static string strRs_Qty_ShdNot_LessThan_LowLimit
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("Qty_ShdNot_LessThan_LowLimit");
            }            
        }

        

        public static string strRs_Order_Qty_Pls
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("Order_Qty_Pls");
            }
        }

        public static string strRs_Order_Qty_Invalid
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("Order_Qty_Invalid");
            }
        }

      

        public static string StrRs_Order_Pls_Price
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("Order_Pls_Price"); }
        }

        public static string StrRs_Order_Invalid_Price
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("Order_Invalid_Price"); }
        }

        public static string StrRs_input_Acc_pls
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("input_Acc_pls"); }
        }

        public static string StrRs_input_prod_pls
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("input_prod_pls"); }
        }

        public static string StrRs_input_ticks_invalid
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("input_ticks_invalid"); }
        }        

        public static string strRs_Order_AccNotEqual
        {
            get {
                return GOSTS.GosCulture.CultureHelper.GetString("Order_AccNotEqual"); 
            }
        }

        public static string strRs_input_price_no_allow_dec
        {
            get {
                return GOSTS.GosCulture.CultureHelper.GetString("input_price_no_allow_dec"); 
            }
        }
        public static string strRs_input_priceDec_only_sLen_allowed
        {
            get {
                return GOSTS.GosCulture.CultureHelper.GetString("input_priceDec_only_sLen_allowed"); 
            }
        }
        

        public static string strRs_Input_order_specDate_pls
        {
            get {
                return GOSTS.GosCulture.CultureHelper.GetString("Input_order_specDate_pls"); 
            }
        }

        public static string strRs_input_stoptype_error
        {
            get {
                return GOSTS.GosCulture.CultureHelper.GetString("input_stoptype_error"); 
            }
        }

        public static string strRs_input_trgStop_Pls
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("input_trgStop_Pls"); }
        }

        public static string strRs_input_SLPrice_invalid
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("input_trgStopPrice_invalid"); }
        }

        public static string strRs_TrStopPrice_Must_High_MktPrice
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_TrPrice_Must_High_MktPrice"); }
        }

        public static string strRs_TrStopPrice_Must_Low_MktPrice
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_TrPrice_Must_Low_MktPrice"); }
        }

        public static string strRs_SL_BuyPrice_Must_EqOrMore_MktPrice
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_StopLoss_BuyPrice_Must_EqOrMore_MktPrice"); }            
        }

        public static string strRs_SL_SellPrice_Must_EqOrLow_TrPrice
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_SL_Sell_Price_Must_EqOrLow_TrPrice"); }            
        }

        

        #region buy sell result
        public static string strRs_BuyOrder_Fail
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("BuyOrder_Fail"); }
        }
        public static string strRs_SellOrder_Fail
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("SellOrder_Fail"); }
        }
        public static string strRs_Del_Failed
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("input_del_failed"); }
        }
        public static string strRs_Save_Successfully
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("input_save_success"); }
        }
         public static string strRs_Save_Failed
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("Input_Save_failed"); }
        }

        
        public static string strRs_Add_Failed
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("input_add_failed"); }
        }

        public static string strRs_Acc_Existed
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("input_acc_exists"); }
        }  
        #endregion


        #region buy sell confirm
        public static string strRs_confirm_BuyOrder_Title
        {
            get {
                return GOSTS.GosCulture.CultureHelper.GetString("confirm_BuyOrder_Title"); 
            }
        }

        public static string strRs_confirm_SellOrder_Title
        {
            get {
                return GOSTS.GosCulture.CultureHelper.GetString("confirm_SellOrder_Title"); 
            }
        }        

        public static string strRs_confirm_SellOrder 
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_SellOrder"); }
        }

        public static string strRs_confirm_BuyOrder 
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_BuyOrder"); }
        }

        public static string strRs_confirm_StopTrgOrder 
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_StopTrgOrder"); }
        }

        public static string strRs_confirmTitle 
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirmTitle"); }
        }
        

        //prompt
        public static string strRs_confirm_accMask_SaveAsk 
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_accMask_SaveAsk"); }
        }
        public static string strRs_confirm_accMask_Ask_StaySave
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_accMask_Ask_StaySave"); }
        }

        
        public static string strRs_confirm_OrderDeviation_change_AskSave 
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_OrderDeviation_change_AskSave"); }
        }

        public static string strRs_confirm_OrderDeviation_change_Ask_StaySave
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_OrderDeviation_change_Ask_StaySave"); }
        }

        public static string strRs_ask_Save_Changes
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("ask_Save_Changes"); }
        }

        public static string strRs_Confirm_Order_Del
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_Order_Del"); }
        }
        public static string strRs_Confirm_OrderDel_Title
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("CHOD_DelConfTitle"); }
        }
        public static string strRs_Confirm_Order_Active
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_Order_Active"); }
        }
        public static string strRs_Confirm_OrderActive_title
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_OrderActive_title"); }
        }
        public static string strRs_Confirm_Order_Inactive
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_Order_Inactive"); }
        }

        public static string strRs_Confirm_OrderInActive_title
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_OrderInActive_title"); }
        }
        public static string strRs_Confirm_Sure_DelAll
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_Sure_DelAll"); }
        }

        public static string strRs_Confirm_sure_ActiveAllOrder
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_sure_ActiveAllOrder"); }
        }

        public static string strRs_Confirm_sure_InActiveAllOrder
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("confirm_sure_InActiveAllOrder"); }
        }
        
        #endregion


        #region validType

        public static string strRs_ValidType_today
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("ValidType_today"); }
        }
        public static string strRs_ValidType_fak
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("ValidType_fak"); }
        }

        public static string strRs_ValidType_fok
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("ValidType_fok"); }
        }

        public static string strRs_ValidType_gtc
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("ValidType_gtc"); }
        }

        public static string strRs_ValidType_specDate
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("ValidType_specDate"); }
        }
        
        public static string getValidTypeText( TradeStationComm.Attribute.ValidType  valid)
        {
            switch (valid)
            {
                case TradeStationComm.Attribute.ValidType.today:
                    return strRs_ValidType_today;
                    break;
                case TradeStationComm.Attribute.ValidType.FAK:
                    return strRs_ValidType_fak;
                    break;
                case TradeStationComm.Attribute.ValidType.FOK :
                    return strRs_ValidType_fok;
                    break;
                case TradeStationComm.Attribute.ValidType.GTC:
                    return strRs_ValidType_gtc;
                    break;
                case TradeStationComm.Attribute.ValidType.specTime:
                    return strRs_ValidType_specDate;
                    break;

            }
            return "";
        }


        public static TradeStationComm.Attribute.ValidType getEnumValidType(string validText)
        {
            validText = validText.ToLower().Trim();
            //string strToday = strRs_ValidType_today.ToLower();
            //string strFAK=strRs_ValidType_fak.ToLower();
            //string strFOK=strRs_ValidType_fok.ToLower();
            //string strGTC=strRs_ValidType_gtc.ToLower();
            //string strSpecDate=strRs_ValidType_specDate.ToLower();
           
            if (validText == strRs_ValidType_today.ToLower())
            {
                return TradeStationComm.Attribute.ValidType.today;
            }
            if (validText == strRs_ValidType_fak.ToLower())
            {
                return TradeStationComm.Attribute.ValidType.FAK;
            }
            if (validText == strRs_ValidType_fok.ToLower())
            {
                return TradeStationComm.Attribute.ValidType.FOK;
            }
            if (validText == strRs_ValidType_gtc.ToLower())
            {
                return TradeStationComm.Attribute.ValidType.GTC;
            }
            if (validText == strRs_ValidType_specDate.ToLower())
            {
                return TradeStationComm.Attribute.ValidType.specTime;
            }
            return TradeStationComm.Attribute.ValidType.today;
        }
        #endregion

        #region stop Type

        public static string strRs_StopType_SL
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("cbItemStopLoss"); }
        }

        public static string strRs_StopType_UPStrigger
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("cbItemUpTriggler"); }
        }

        public static string strRs_StopType_DownStrigger
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("cbItemDownTr"); }
        }

        public static string strRs_StopType_normal
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("input_stoptypeItem_normal"); }
        }

        public static string getStopTypeText(TradeStationComm.Attribute.StopType stopType)
        {
            switch (stopType)
            {
                case TradeStationComm.Attribute.StopType.normalOrder:
                    return strRs_StopType_normal;
                    break;
                case  TradeStationComm.Attribute.StopType.stopLoss:
                    return strRs_StopType_SL;
                    break;
                case TradeStationComm.Attribute.StopType.upTrigger:
                    return strRs_StopType_UPStrigger;
                    break;
                case TradeStationComm.Attribute.StopType.downTrigger:
                    return strRs_StopType_DownStrigger;
                    break;
            }
            return "";
        }

        #endregion
               
        public static string strRs_TOne
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("comm_TOne"); }
        }

        public static string strRs_AO
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("comm_AO"); }
        }

        public static string strRs_AccText
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("AccCmbText"); }
        }
        #region Action
        public static string strRs_ActiveItem_active
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("activeItem_active"); }
        }

        public static string strRs_activeITem_inactive
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("activeITem_inactive"); }
        }

        public static string getActiveText(TradeStationComm.Attribute.Active status)
        {
            switch (status)
            {
                case TradeStationComm.Attribute.Active.active:
                    return strRs_ActiveItem_active;
                    break;
                case TradeStationComm.Attribute.Active.inactive:
                    return strRs_activeITem_inactive;
                    break;
            }
            return "";
        }

        public static string strRs_Buy
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("buy"); }
        }

        public static string strRs_Sell
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("Sell"); }
        }

        public static string GetBSText(string bs)
        {
            bs = bs.ToLower().Trim();
            switch (bs)
            {
                case "buy":
                case "b":
                    return strRs_Buy;
                    break;
                case "sell":
                case "s":
                    return strRs_Sell;
                    break;
            }
            return null;
        }
        #endregion
    }

    public class Utility
    {
        public static int ConvertToUnixTime(DateTime dt)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime dtNow = DateTime.Parse(dt.ToString());
            TimeSpan toNow = dtNow.Subtract(dtStart);
            string timeStamp = toNow.Ticks.ToString();
            timeStamp = timeStamp.Substring(0, timeStamp.Length - 7);
            return Convert.ToInt32(timeStamp);
        }

        public static DateTime UnixToWinTime(long timeStamp)
        {

            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);

            return dtResult;
        }


        public static string getColumnValue(DataRowView drv, string colName)
        {
            if (drv.Row.Table.Columns.Contains(colName))
            {
                if (drv[colName] == System.DBNull.Value) return "";
                string str = drv[colName].ToString().Trim();               
                return str;
            }
            return "";
        }

        public static string getColumnValue(DataRow dr, string colName)
        {
            if (dr.Table.Columns.Contains(colName))
            {
                string str = dr[colName].ToString().Trim();              
                return str;
            }
            return "";
        }

        public static int ConvertToInt(string str)
        {
            if (str == null) return 0;
            if (str.Trim() == "")
            {
                return 0;
            }
            try
            {
                int i = Convert.ToInt32(str);
                return i;
            }
            catch (Exception ex) { }
            return 0;
        }

        public static int ConvertToInt(string str,int intdefault)
        {
            if (str == null) return intdefault;
            if (str.Trim() == "")
            {
                return intdefault;
            }
            try
            {
                int i = Convert.ToInt32(str);
                return i;
            }
            catch (Exception ex) { }
            return intdefault;
        }

        public static bool isInt(string str)
        {
            if (str == null) return false;
            if (str.Trim() == "") { return false; }
            if (System.Text.RegularExpressions.Regex.IsMatch(str, @"^\-{0,1}\d+$"))
            {
                return true; ;
            }
            return false;
        }
        public static bool isUInt(string str)
        {
            if (str == null) return false;
            if (str.Trim() == "") { return false; }
            if (System.Text.RegularExpressions.Regex.IsMatch(str, @"^{0,1}\d+$"))
            {
                return true; ;
            }
            return false;
        }

        public static string ConvertToIntString(string str)
        {
            if (str == "") return "";
            str = str.Trim();
            if (str == "") return "";
            try
            {
                int i = Convert.ToInt32(str);
                return i.ToString();
            }
            catch { }
            return "";
        }

        public static bool IsDecimal(string str)
        {
            if (str == null) return false;
            if (str.Trim() == "") return false;
            string strReg = @"^\-{0,1}\d{0,}(\.\d{0,})?$";
            if (System.Text.RegularExpressions.Regex.IsMatch(str.Trim(), strReg))
            {
                return true;
            }
            return false;
        }

        public static Decimal ConvertToDecimal(string str)
        {
            if (str == null) return 0.00M;
            if (str.Trim() == "") return 0.00M;
            str = str.Trim();
            if (!IsDecimal(str))
            {
                return 0.00M;
            }
            //if (AppFlag.InvalidNum == str.Trim())
            //{
            //    return 0.00M;
            //}
            try
            {
                decimal d = Convert.ToDecimal(str);
                return d;
            }
            catch
            {
                return 0.00M;
            }

        }

        public static Decimal ConvertToDecimal(string str, decimal dvalue)
        {
            if (str == null) return dvalue;
            if (str == "") return dvalue;
            if (!IsDecimal(str))
            {
                return dvalue;
            }
            try
            {
                decimal d = Convert.ToDecimal(str);
                return d;
            }
            catch
            {
                return dvalue;
            }

        }

        public static double? ConvertToDouble(string str)
        {
            if (str == null) return 0;
            try
            {
                double d = Convert.ToDouble(str);
                return d;
            }
            catch
            {
                return null;
            }

        }

        public static string getArrayItemValue(string[] arr, int i)
        {
            if (arr.Length <= i)
            {
                return "";
            }
            if (i < 0)
            {
                return "";
            }
            string str = arr[i];
            return str;
        }

        public static List<T> GetLinealChildObjects<T>(DependencyObject obj) where T : FrameworkElement//, string name
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child is T)
                {
                    childList.Add((T)child);
                }               
            }
            return childList;
        }

    }

   
}
