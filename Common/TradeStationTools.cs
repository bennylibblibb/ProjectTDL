using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Markup;
using System.Windows.Controls;

namespace GOSTS.Common
{
    public class TradeStationTools
    {
        private static DateTime unixTimeStart = new DateTime(1970, 1, 1, 0, 0, 0);

        public static DateTime getDateTimeFromUnixTime(String unixTimeStr)
        {
            try
            {
                return unixTimeStart.AddSeconds(Convert.ToDouble(unixTimeStr)).ToLocalTime();
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        // convert UTF8 response msg 
        public static string Base64StringToString(string strbase64)
        {
           return Base64StringToString(strbase64, "utf-8");
            //if (strbase64 != "")
            //{
            //    try
            //    {
            //        char[] charBuffer = strbase64.ToCharArray();
            //        byte[] bytes = Convert.FromBase64CharArray(charBuffer, 0, charBuffer.Length);
            //        return Encoding.GetEncoding("utf-8").GetString(bytes);
            //    }
            //    catch (Exception exp)
            //    {
            //        StackTrace st = new StackTrace(true);
            //        string strtMethod = st.GetFrame(1).GetMethod().Name.ToString();
            //        TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " TradeStationTools.Base64StringToString(string strbase64), Method:" + strtMethod + "\r\n" + " Content=" + strbase64 + "\r\n error: " + exp.ToString());

            //        return strbase64;
            //    }
            //}
            //else
            //{
            //    return "";
            //}
        }

        public static string Base64StringToString(string strbase64, string code)
        {
            if (strbase64 != "")
            {
                try
                {
                    char[] charBuffer = strbase64.ToCharArray();
                    byte[] bytes = Convert.FromBase64CharArray(charBuffer, 0, charBuffer.Length);
                    return Encoding.GetEncoding(code).GetString(bytes);
                }
                catch (Exception exp)
                {
                    StackTrace st = new StackTrace(true);
                    string strtMethod = st.GetFrame(1).GetMethod().Name.ToString();
                    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " TradeStationTools.Base64StringToString(string strbase64), Method:" + strtMethod + "\r\n" + " Content=" + strbase64 + "\r\n error: " + exp.ToString());

                    return strbase64;
                }
            }
            else
            {
                return "";
            }
        }

        //convert UTF16 response msg 
        public static string Base64Utf16StringToString(string strbase64)
        {
            if (strbase64 != "")
            {
                try
                {
                    //handle multiple b64 string with ';' separated  kenlo131003
                    if (strbase64.IndexOf(";") > -1)
                    {
                        String[] strArr = strbase64.Split(';');
                        String str = "";
                        for (int index = 0; index < strArr.Length; index++)
                        {
                            byte[] tempBytes = Convert.FromBase64String(strArr[index]);
                            if (str.Length > 0)
                                str += " ";
                            str += Encoding.Unicode.GetString(tempBytes);
                        }
                        return str;
                    }

                    //char[] charBuffer = strbase64.ToCharArray();
                    //byte[] bytes = Convert.FromBase64CharArray(charBuffer, 0, charBuffer.Length);
                    byte[] bytes = Convert.FromBase64String(strbase64);
                    return Encoding.Unicode.GetString(bytes);
                }
                catch (Exception exp)
                {
                    StackTrace st = new StackTrace(true);
                    string strtMethod = st.GetFrame(1).GetMethod().Name.ToString();
                    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " TradeStationTools.Base64Utf16StringToString(string strbase64), Method:" + strtMethod + "\r\n" + " Content=" + strbase64 + "\r\n error: " + exp.ToString());

                    return strbase64;
                }
            }
            else
            {
                return "";
            }
        }

        private class Header
        {
            public const int size = 11;
            public const int offsetSOH = 0;
            public const int offsetDST = 2;
            public const int offsetCmdH = 3;
            public const int offsetCmdL = 4;

            public const byte SOH = 0x01;

            public enum DST
            {
                priceManager = 'P',
                heartBeat = 'H',
                accountManager = 'A',
                orderManager = 'O',
                tradeManager = 'T',
                unknow = 'U'
            }
        }

        public static string getMsg(MemoryStream msg)
        {
            string body = "";
            byte[] header;
            if (msg != null)
            {
                try
                {
                    header = new byte[Header.size];
                    msg.Read(header, 0, Header.size);

                    byte[] tmpBodyArr = new byte[msg.Length - msg.Position];
                    msg.Read(tmpBodyArr, 0, tmpBodyArr.Length);
                    return body = Encoding.ASCII.GetString(tmpBodyArr, 0, tmpBodyArr.Length);
                }
                catch
                {
                }
            }
            return body;
        }

        public static int ConvertToInt(object str)
        {
            if (str == null || str.ToString().Trim().IndexOf(AppFlag.InvalidNum.ToString()) > -1)
                return 0;

            int intValue;
            if (!int.TryParse(str.ToString().Trim(), out intValue))
            {
                return 0;
            }
            else
            {
                return intValue;
            }
        }

        public static int ConvertToInt(object str, int Type)
        {
            if (str == null || str.ToString().Trim().IndexOf(AppFlag.InvalidNum.ToString()) > -1)
                return -1;

            int intValue;
            switch (Type)
            {
                case -1:
                    if (!int.TryParse(str.ToString().Trim(), out intValue))
                    {
                        return -1;
                    }
                    return intValue;
                default:
                    if (!int.TryParse(str.ToString().Trim(), out intValue))
                    {
                        return -1;
                    }
                    return intValue;
            }
        }

        //public static string ConvertToString(object obj)
        //{
        //    if (obj == null || obj.ToString().Trim().IndexOf(AppFlag.InvalidNum) > -1)
        //        return "";
        //    return obj.ToString();
        //}

        //public static string ConvertToFormatString(object str, int DepInPrice)
        //{
        //    if (str == null || str.ToString().Trim().IndexOf(AppFlag.InvalidNum) > -1)
        //        return "AO";
        //    switch (str.ToString().Trim())
        //    {
        //        case "":
        //            return "";
        //        case "AO":
        //            return "AO";
        //        case "-1":
        //            return "AO";
        //        case "0":
        //            return "0";
        //        default:
        //            float i = ConvertToFloat(str, DepInPrice);
        //            switch (DepInPrice)
        //            {
        //                case 1:
        //                    return (i == -99999) ? "" : Convert.ToDouble(i).ToString("0.0");
        //                case 2:
        //                    return (i == -99999) ? "" : Convert.ToDouble(i).ToString("0.00");
        //                case 3:
        //                    return (i == -99999) ? "" : Convert.ToDouble(i).ToString("0.000");
        //                case 4:
        //                    return (i == -99999) ? "" : Convert.ToDouble(i).ToString("0.0000");
        //                case 5:
        //                    return (i == -99999) ? "" : Convert.ToDouble(i).ToString("0.00000");
        //                default:
        //                    return (i == -99999) ? "" : Convert.ToDouble(i).ToString();
        //            }
        //    }
        //}

        //public static string ConvertToFormatStringNull(object str, int DepInPrice)
        //{
        //    if (str == null || str.ToString().Trim().IndexOf(AppFlag.InvalidNum) > -1)
        //        return "";
        //    switch (str.ToString().Trim())
        //    {
        //        case "":
        //            return "";
        //        case "0":
        //            return "0";
        //        default:
        //            float i = ConvertToFloat(str, DepInPrice);
        //            switch (DepInPrice)
        //            {
        //                case 1:
        //                    return (i == -99999) ? "" : Convert.ToDouble(i).ToString("0.0");
        //                case 2:
        //                    return (i == -99999) ? "" : Convert.ToDouble(i).ToString("0.00");
        //                case 3:
        //                    return (i == -99999) ? "" : Convert.ToDouble(i).ToString("0.000");
        //                case 4:
        //                    return (i == -99999) ? "" : Convert.ToDouble(i).ToString("0.0000");
        //                case 5:
        //                    return (i == -99999) ? "" : Convert.ToDouble(i).ToString("0.00000");
        //                default:
        //                    return (i == -99999) ? "" : Convert.ToDouble(i).ToString();
        //            }
        //    }
        //}

        //public static float ConvertToFloat(object str, int Dep)
        //{
        //    if (str == null || str.ToString().Trim().IndexOf(AppFlag.InvalidNum) > -1)
        //        return -99999;
        //    int intValue;
        //    if (!int.TryParse(str.ToString().Trim(), out intValue))
        //    {
        //        return -99999;
        //    }
        //    return (Dep == 0) ? (float)intValue : (float)(intValue / Math.Pow(10, Dep));
        //}

        public static double ConvertToDouble(object str)
        {
            double doubleValue = 0;

            if (!double.TryParse(str.ToString().Trim(), out doubleValue))
            {
                return 0;
            }

            return doubleValue;
        }

        public static long ConvertToLong(object str)
        {
            long longValue = 0;

            if (!long.TryParse(str.ToString().Trim(), out longValue))
            {
                return 0;
            }

            return longValue;
        }

        public static uint ConvertToUInt(object str)
        {
            uint uintValue = 0;

            if (!uint.TryParse(str.ToString().Trim(), out uintValue))
            {
                return 0;
            }

            return uintValue;
        }
        public static int ConvertToInt32(object obj)
        {
            if (obj == null || obj.ToString().Trim() == "")
                return AppFlag.InvalidNum;

            int intValue;
            if (!int.TryParse(obj.ToString().Trim(), out intValue))
            {
                return AppFlag.InvalidNum;
            }
            else
            {
                return intValue;
            }
        }

        public static float ConvertToFloat(object obj, int Dep)
        {
            if (obj == null || obj.ToString().Trim() == "" || obj.ToString().Trim().IndexOf(AppFlag.InvalidNum.ToString()) > -1)
            {
                return AppFlag.InvalidNum;
            }
            else if (obj.ToString() == "AO")
            {
                return AppFlag.AONum;
            }
            else
            {
                int intValue;
                if (!int.TryParse(obj.ToString().Trim(), out intValue))
                {
                    return AppFlag.InvalidNum;
                }
                return (Dep == 0) ? (float)intValue : (float)(intValue / Math.Pow(10, Dep));
            }
        }

        public static string ConvertToStringForBidAsk(object str, int depInPrice)
        {
            if (str == null || str.ToString () == "")
            {
                return "";
            }
            else if (str.ToString().Trim() == "AO" || str.ToString().Trim().IndexOf(AppFlag.AONum.ToString()) > -1)
            {
                return "AO";
            }
            else
            {
                float i = ConvertToFloat(str, depInPrice);
                if (i == AppFlag.InvalidNum)
                {
                    return "";
                }
                else
                {
                    switch (depInPrice)
                    {
                        case 1:
                            return System.Convert.ToDouble(i).ToString("0.0");
                        case 2:
                            return System.Convert.ToDouble(i).ToString("0.00");
                        case 3:
                            return System.Convert.ToDouble(i).ToString("0.000");
                        case 4:
                            return System.Convert.ToDouble(i).ToString("0.0000");
                        case 5:
                            return System.Convert.ToDouble(i).ToString("0.00000");
                        default:
                            return System.Convert.ToDouble(i).ToString();
                    }
                }
            }
        }

        public static string ConvertToFormatStringNull(object str, int depInPrice)
        {
            float i = ConvertToFloat(str, depInPrice);

            if (i == AppFlag.AONum)
            {
                return "AO";
            }
            else if (i == (float)AppFlag.InvalidNum)
            {
                return "";
            }
            else
            {
                switch (depInPrice)
                {
                    case 1:
                        return System.Convert.ToDouble(i).ToString("0.0");
                    case 2:
                        return System.Convert.ToDouble(i).ToString("0.00");
                    case 3:
                        return System.Convert.ToDouble(i).ToString("0.000");
                    case 4:
                        return System.Convert.ToDouble(i).ToString("0.0000");
                    case 5:
                        return System.Convert.ToDouble(i).ToString("0.00000");
                    default:
                        return System.Convert.ToDouble(i).ToString();
                }
            }
        }

        public static string ConvertToString(object obj)
        {
            if (obj == null || obj.ToString().Trim().IndexOf(AppFlag.InvalidNum.ToString ()) > -1 || obj.ToString().Trim() == "")
            { return ""; }
            else
            {
                int intValue;
                if (!int.TryParse(obj.ToString().Trim(), out intValue))
                {
                    return "";
                }
                return obj.ToString();
            }
        }
    }

    public class MultiValueStringFormatConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var formatterArgsEnum = values.Select((a, i) => new { Value = a, Index = i })
                   .Where(b => b.Index > 0)
                   .Select(c => c.Value);

            object[] formatterArgs = formatterArgsEnum.ToArray();
            int depInPrice = int.Parse(String.Format(values[1].ToString(), formatterArgs));
            float price = (float)values[0];

            if (price == AppFlag.AONum)
            {
                return "AO";
            }
            else if (price == (float)AppFlag.InvalidNum)
            {
                return "";
            }
            else
            {
                switch (depInPrice)
                {
                    case 1:
                        return System.Convert.ToDouble(price).ToString("0.0");
                    case 2:
                        return System.Convert.ToDouble(price).ToString("0.00");
                    case 3:
                        return System.Convert.ToDouble(price).ToString("0.000");
                    case 4:
                        return System.Convert.ToDouble(price).ToString("0.0000");
                    case 5:
                        return System.Convert.ToDouble(price).ToString("0.00000");
                    default:
                        return System.Convert.ToDouble(price).ToString();
                }
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw (new NotImplementedException());
        }
    }

    public class GOSConverterStringFormatConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int price = (int)value;

            if (price == AppFlag.InvalidNum)
            {
                return "";
            }
            return price;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                int intValue;
                if (!int.TryParse((string)value, out intValue))
                {
                    intValue = 0;
                }
                return intValue;
            }
            return 0;
        }

    }

    public class MultiValueStringFormatByDepInPriceConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var formatterArgsEnum = values.Select((a, i) => new { Value = a, Index = i })
                   .Where(b => b.Index > 0)
                   .Select(c => c.Value);

            object[] formatterArgs = formatterArgsEnum.ToArray();
            int depInPrice = int.Parse(String.Format(values[1].ToString(), formatterArgs));

            float price = TradeStationTools.ConvertToFloat(values[0], depInPrice);
            if (price == AppFlag.AONum)
            {
                return "AO";
            }
            else if (price == AppFlag.InvalidNum)
            {
                return "";
            }
            switch (depInPrice)
            {
                case 1:
                    return System.Convert.ToDouble(price).ToString("0.0");
                case 2:
                    return System.Convert.ToDouble(price).ToString("0.00");
                case 3:
                    return System.Convert.ToDouble(price).ToString("0.000");
                case 4:
                    return System.Convert.ToDouble(price).ToString("0.0000");
                case 5:
                    return System.Convert.ToDouble(price).ToString("0.00000");
                default:
                    return System.Convert.ToDouble(price).ToString();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw (new NotImplementedException());
        }
    }

    public class GOSConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            else if (value.ToString() == "-1")
            {
                return "AO";
            }
            else if (value.GetType().ToString() == "System.DateTime" && ((DateTime)value) == DateTime.MinValue)
            {
                return "";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                int intValue;
                if (!int.TryParse((string)value, out intValue))
                {
                    intValue = 0;
                }
                return intValue;
            }
            return 0;
        }

    }

    public class GOSDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            else if (value.ToString() == "0" || value.ToString() == "")
            {
                return "";
            }
            else if (value.GetType().ToString() == "System.DateTime" && ((DateTime)value) == DateTime.MinValue)
            {
                return "";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class GOSPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            return value.ToString() + "%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                int intValue;
                if (!int.TryParse((string)value, out intValue))
                {
                    intValue = 0;
                }
                return intValue;
            }
            return 0;
        }

    }

    public class GOSRowHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            return value.ToString().Replace("_", "__");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw (new NotImplementedException());
        }
    }

    public class Base64Utf16Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == "")
            {
                return "";
            }
            else
            {
                return TradeStationTools.Base64Utf16StringToString((string)value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw (new NotImplementedException());
        }
    }

    public class Base64Utf8Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == "")
            {
                return "";
            }
            else
            {
                if (value.ToString().IndexOf(";") > 0)
                {
                    value = value.ToString().Substring(value.ToString().IndexOf(";") + 1, value.ToString().Length - value.ToString().IndexOf(";") - 1);
                }
                return TradeStationTools.Base64StringToString((string)value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw (new NotImplementedException());
        }
    }

    public class BoolToVisibleOrHidden : IValueConverter
    {
        #region Constructors
        /// <summary>
        /// The default constructor
        /// </summary>
        public BoolToVisibleOrHidden() { }
        #endregion

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int i;
            if (int.TryParse((string)value, out i))
            {
                // Status 0= traded , 20 = rejected ,  21 = reallocated
                //100 = traded and reported ,120 = rejected and reported  , 121 = reallocated and reported 
                switch (i)
                {
                    case 100:
                    case 120:
                    case 121:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;

            if (visibility == Visibility.Visible)
                return true;
            else
                return false;
        }
        #endregion
    }

    public class DefaultConverterForReport : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == "")
            {
                return "";
            }
            else
            {
                if (value.ToString().IndexOf(";") > 0)
                {
                    value = value.ToString().Substring(0, value.ToString().IndexOf(";"));
                }
            }

            int i;
            if (int.TryParse((string)value, out i))
            {
                // Status 0= traded , 20 = rejected ,  21 = reallocated
                //100 = traded and reported ,120 = rejected and reported  , 121 = reallocated and reported 
                switch (i)
                {
                    case 100:
                    case 120:
                    case 121:
                        return "Reported";
                    default:
                        return "UnReported";
                }
            }
            else
            {
                return (string)value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw (new NotImplementedException());
        }
    }

    //public class MultiValueStringFormatConvertor : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        var formatterArgsEnum = values.Select((a, i) => new { Value = a, Index = i })
    //               .Where(b => b.Index > 0)
    //               .Select(c => c.Value);

    //        object[] formatterArgs = formatterArgsEnum.ToArray();
    //        int depInPrice = int.Parse(String.Format(values[1].ToString(), formatterArgs));
    //        string price = String.Format(values[0].ToString(), formatterArgs);
    //        if (price.IndexOf(AppFlag.InvalidNum) > -1)
    //            return "AO";

    //        switch (price)
    //        {
    //            case "":
    //                return "";
    //            case "AO":
    //                return "AO";
    //            case "-99999":
    //                return "";
    //            case "-1":
    //                return "AO";
    //            case "0":
    //                return "0";
    //            default:
    //                switch (depInPrice)
    //                {
    //                    case 1:
    //                        return System.Convert.ToDouble(price).ToString("0.0");
    //                    case 2:
    //                        return System.Convert.ToDouble(price).ToString("0.00");
    //                    case 3:
    //                        return System.Convert.ToDouble(price).ToString("0.000");
    //                    case 4:
    //                        return System.Convert.ToDouble(price).ToString("0.0000");
    //                    case 5:
    //                        return System.Convert.ToDouble(price).ToString("0.00000");
    //                    default:
    //                        return System.Convert.ToDouble(price).ToString();
    //                }
    //        }
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    //    {
    //        throw (new NotImplementedException());
    //    }
    //}

    //public class MultiValueStringFormatByDepInPriceConvertor : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        var formatterArgsEnum = values.Select((a, i) => new { Value = a, Index = i })
    //               .Where(b => b.Index > 0)
    //               .Select(c => c.Value);

    //        object[] formatterArgs = formatterArgsEnum.ToArray();
    //        int depInPrice = int.Parse(String.Format(values[1].ToString(), formatterArgs));
    //        string price = String.Format(values[0].ToString(), formatterArgs);

    //        if (price.IndexOf(AppFlag.InvalidNum) > -1)
    //            return "";

    //        switch (price)
    //        {
    //            case "AO":
    //                return "AO";
    //            case "-99999":
    //                return "";
    //            case "-1":
    //                return "AO";
    //            case "0":
    //                return "0";
    //            default:
    //                float i = TradeStationTools.ConvertToFloat(price, depInPrice);
    //                switch (depInPrice)
    //                {
    //                    case 1:
    //                        return System.Convert.ToDouble(i).ToString("0.0");
    //                    case 2:
    //                        return System.Convert.ToDouble(i).ToString("0.00");
    //                    case 3:
    //                        return System.Convert.ToDouble(i).ToString("0.000");
    //                    case 4:
    //                        return System.Convert.ToDouble(i).ToString("0.0000");
    //                    case 5:
    //                        return System.Convert.ToDouble(i).ToString("0.00000");
    //                    default:
    //                        return System.Convert.ToDouble(i).ToString();
    //                }
    //        }
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    //    {
    //        throw (new NotImplementedException());
    //    }
    //}

    public class ReportedVisibleOrHiddens : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var formatterArgsEnum = values.Select((a, i) => new { Value = a, Index = i })
                   .Where(b => b.Index > 0)
                   .Select(c => c.Value);

            object[] formatterArgs = formatterArgsEnum.ToArray();
            string status = String.Format(values[0].ToString(), formatterArgs);
            if (status.ToString().IndexOf(";") > 0)
            {
                status = status.ToString().Substring(0, status.ToString().IndexOf(";"));
            }

            Visibility visibility = (Visibility)values[1];
            int j;
            if (int.TryParse((string)status, out j))
            {
                // Status 0= traded , 20 = rejected ,  21 = reallocated
                //100 = traded and reported ,120 = rejected and reported  , 121 = reallocated and reported 
                switch (j)
                {
                    case 100:
                    case 120:
                    case 121:
                        switch (visibility)
                        {
                            case Visibility.Collapsed:
                                return Visibility.Collapsed;
                            case Visibility.Visible:
                                return Visibility.Visible;
                            case Visibility.Hidden:
                                return Visibility.Hidden;
                            default:
                                return Visibility.Visible;
                        }
                    default:
                        return Visibility.Visible;
                }
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw (new NotImplementedException());
        }

    }

    public class MultiValueBSConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var formatterArgsEnum = values.Select((a, i) => new { Value = a, Index = i })
                   .Where(b => b.Index > 0)
                   .Select(c => c.Value);

            object[] formatterArgs = formatterArgsEnum.ToArray();
            int osBQty;
            int osSQty;
            osBQty = TradeStationTools.ConvertToInt(values[0], -1);
            osSQty = TradeStationTools.ConvertToInt(values[1], -1);

            return (osBQty > 0) ? "BUY" : "SELL";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw (new NotImplementedException());
        }
    }

    public class MultiValueRemainQtyConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var formatterArgsEnum = values.Select((a, i) => new { Value = a, Index = i })
                   .Where(b => b.Index > 0)
                   .Select(c => c.Value);

            object[] formatterArgs = formatterArgsEnum.ToArray();
            int osBQty;
            int osSQty;
            int tradedQty;
            osBQty = TradeStationTools.ConvertToInt(values[0], -1);
            osSQty = TradeStationTools.ConvertToInt(values[1], -1);
            tradedQty = TradeStationTools.ConvertToInt(values[2], -1);
            int qty = (osBQty > 0) ? osBQty - tradedQty : osSQty - tradedQty;
            return qty.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw (new NotImplementedException());
        }
    }
    public class MultiValueQtyConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var formatterArgsEnum = values.Select((a, i) => new { Value = a, Index = i })
                   .Where(b => b.Index > 0)
                   .Select(c => c.Value);

            object[] formatterArgs = formatterArgsEnum.ToArray();
            int osBQty;
            int osSQty;
            osBQty = TradeStationTools.ConvertToInt(values[0], -1);
            osSQty = TradeStationTools.ConvertToInt(values[1], -1);

            return (osBQty > 0) ? osBQty.ToString() : osSQty.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw (new NotImplementedException());
        }
    }
    public class MultiReportedSelectedConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var formatterArgsEnum = values.Select((a, i) => new { Value = a, Index = i })
                   .Where(b => b.Index > 0)
                   .Select(c => c.Value);
            object[] formatterArgs = formatterArgsEnum.ToArray();
            string recNo = String.Format(values[0].ToString(), formatterArgs);
            ObservableCollection<TradeSimply> data = values[1] as ObservableCollection<TradeSimply>;
            if (data != null)
            {
                foreach (TradeSimply dr in data)
                {
                    if (dr.recNo == recNo)
                    {
                        LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
                        myLinearGradientBrush.StartPoint = new Point(0.504, 0.03);
                        myLinearGradientBrush.EndPoint = new Point(0.504, 1.5);
                        myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("LightGreen"), 0.042));
                        myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("LightGreen"), 0.925));
                        return myLinearGradientBrush;

                    }
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // throw (new NotImplementedException());
            return null;
        }
    }
    public class MultiReportedSelectedConvertor2 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var formatterArgsEnum = values.Select((a, i) => new { Value = a, Index = i })
                   .Where(b => b.Index > 0)
                   .Select(c => c.Value);
            object[] formatterArgs = formatterArgsEnum.ToArray();
            string accNo = String.Format(values[0].ToString(), formatterArgs);
            string orderNo = String.Format(values[1].ToString(), formatterArgs);
            ObservableCollection<TradeSimply> data = values[2] as ObservableCollection<TradeSimply>;
            if (data != null)
            {
                foreach (TradeSimply dr in data)
                {
                    if (dr.accNo == accNo && dr.orderNo == orderNo)
                    {
                        LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
                        myLinearGradientBrush.StartPoint = new Point(0.504, 0.03);
                        myLinearGradientBrush.EndPoint = new Point(0.504, 1.5);
                        myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("LightGreen"), 0.042));
                        myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("LightGreen"), 0.925));
                        return myLinearGradientBrush;

                    }
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // throw (new NotImplementedException());
            return null;
        }
    }

    public class GOSLoginServerNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            if (value.ToString().IndexOf(":") > 0)
            {
                value = value.ToString().Substring(0, value.ToString().IndexOf(":"));
            }
            //  value.ToString ().Substring(cbServer.Text.IndexOf(":") + 1, cbServer.Text.Length - cbServer.Text.IndexOf(":") - 1) : cbServer.Text.Trim();

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class GOSLoginServerDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }

            value = value.ToString() + "abc";

            //  value.ToString ().Substring(cbServer.Text.IndexOf(":") + 1, cbServer.Text.Length - cbServer.Text.IndexOf(":") - 1) : cbServer.Text.Trim();

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class MultiValuePriceAlertValueConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var formatterArgsEnum = values.Select((a, i) => new { Value = a, Index = i })
                   .Where(b => b.Index > 0)
                   .Select(c => c.Value);
            object[] formatterArgs = formatterArgsEnum.ToArray();
            string arrivedType = String.Format(values[0].ToString(), formatterArgs);
            string above = String.Format(values[1].ToString(), formatterArgs);
            string below = String.Format(values[2].ToString(), formatterArgs);
            if (arrivedType == "Above")
            {
                return above;
            }
            else if (arrivedType == "Below")
            {
                return below;
            }
            return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // throw (new NotImplementedException());
            return null;
        }
    }

    public class MutiPriceAlertAboveColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var formatterArgsEnum = values.Select((a, i) => new { Value = a, Index = i })
                   .Where(b => b.Index > 0)
                   .Select(c => c.Value);
            object[] formatterArgs = formatterArgsEnum.ToArray();
            string arrivedType = String.Format(values[0].ToString(), formatterArgs);
            string status = String.Format(values[1].ToString(), formatterArgs);
            if (arrivedType == "Above" || status == "2")
            {
                return Brushes.Blue; ;
            }
            else
            {
                return Brushes.Black;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class MutiPriceAlertBelowColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var formatterArgsEnum = values.Select((a, i) => new { Value = a, Index = i })
                   .Where(b => b.Index > 0)
                   .Select(c => c.Value);
            object[] formatterArgs = formatterArgsEnum.ToArray();
            string arrivedType = String.Format(values[0].ToString(), formatterArgs);
            string status = String.Format(values[1].ToString(), formatterArgs);
            if (arrivedType == "Below" || status == "2")
            {
                return Brushes.Blue; ;
            }
            else
            {
                return Brushes.Black;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class PriceAlertTypeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Brushes.Transparent;
            }

            if (value.ToString() == "Above")
            {
                return Brushes.Green;
            }
            else if (value.ToString() == "Below")
            {
                return Brushes.Red;
            }
            else
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class RowToIndexConverter : MarkupExtension, IValueConverter
    {
        static RowToIndexConverter converter;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DataGridRow row = value as DataGridRow;
            if (row != null)
                return row.GetIndex();
            else
                return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null) converter = new RowToIndexConverter();
            return converter;
        }

        public RowToIndexConverter()
        {
        }
    }

    public class GOSCheckBoxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            if (value.ToString().Trim() == "0") return false;
            if (value.ToString().Trim() == "1") return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw (new NotImplementedException());
        }
    }

    public class GOSProdNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            return MarketPriceData.GetProductName(value.ToString ());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw (new NotImplementedException());
        }
    }

    public class GOSConverterItemLanguage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            return (value.ToString() == "Prc") ? GOSTS.GosCulture.CultureHelper.GetString("PDPrice") : (value.ToString() == "Qty") ? GOSTS.GosCulture.CultureHelper.GetString("PDQty") : "";
            //return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw (new NotImplementedException());
        }
    }
}
