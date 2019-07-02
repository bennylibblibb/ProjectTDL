using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Drawing;
using GOSTS.Common;

namespace GOSTS
{
    public class TradeStationComm
    {
        //private static DateTime unixTimeStart = new DateTime(1970, 1, 1, 0, 0, 0);
        private static DateTime unixTimeStart = new DateTime(1970, 1, 1, 0, 0, 0);
        //private static DateTime unixTimeStartHK = new DateTime(1970, 1, 1, 8, 0, 0);

        private static String xInfoCh = "";
        private static String xInfoLang = "";
        //private static String changeLang = "";
        private static int refNo = 0;
        private static UInt16 seqNo = 0;
        private static object SeqNoLock = new object();
        private static String sessionhash = "10000000";
        public static String SessionHash
        {
            //get
            //{
            //    return sessionhash;
            //}
            set
            {
                sessionhash = value;
            }
        }

        //  private static TextWriterTraceListener tr1, tr2;
        private static void DebugLog(String remarkStr, String logString)
        {
            //if (tr1 == null)
            //{
            //    tr1 = new TextWriterTraceListener(System.Console.Out);
            //    Trace.Listeners.Add(tr1);
            //}

            //if (tr2 == null)
            //{
            //    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder))
            //    {
            //        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder);
            //    }
            //    //tr2 = new TextWriterTraceListener(System.IO.File.CreateText("ComDebug.txt"));
            //    tr2 = new TextWriterTraceListener(System.IO.File.CreateText(AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder + "ComDebug-" + DateTime.Now.ToString("yyMMdd-HHmmss") + ".txt"));
            //    Trace.Listeners.Add(tr2);
            //}

            //tr2.WriteLine(DateTime.Now.ToString("yyMMdd-HHmmss ") + remarkStr + "  " + logString);
            //tr2.Flush();
            TradeStationLog.WriteComDebug(DateTime.Now.ToString("yyMMdd-HHmmss ") + remarkStr + "  " + logString);
        }

        private static void QCLog(String remarkStr, String logString)
        {
            TradeStationLog.WriteQCLog(DateTime.Now.ToString("yyMMdd-HHmmss ") + remarkStr + "  " + logString);
        }

        #region Tools
        private class Tools
        {
            public static String byte2Str(byte theByte)
            {
                return "\\x" + theByte.ToString("X2");
            }

            public static String uint16ToStr(UInt16 theVal)
            {
                return "\\x" + ((byte)theVal).ToString("X2") + "\\x" + (theVal >> 8).ToString("X2");
            }

            public static String int32ToStr(Int32 theVal)
            {
                return "\\x" + ((byte)theVal).ToString("X2") + "\\x" + ((byte)(theVal >> 8)).ToString("X2") + "\\x" + ((byte)(theVal >> 16)).ToString("X2") + "\\x" + ((byte)(theVal >> 24)).ToString("X2");
            }

            public static String strArrToStr(String[] strArr)
            {
                String retString = "";
                if (strArr != null)
                {
                    int arrSize = strArr.Length;
                    retString = arrSize.ToString();
                    if (arrSize > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int index = 0; index < arrSize; index++)
                        {
                            sb.Append(",");
                            sb.Append(strArr[index]);
                        }
                        retString += sb.ToString();
                    }
                }
                return retString;
            }

            public static StringBuilder getHeaderBeforeLen(byte SRC, Header.DST DST, byte CMDL)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Tools.byte2Str(Header.SOH));
                sb.Append(Tools.byte2Str(SRC));
                sb.Append(Tools.byte2Str((byte)DST));
                sb.Append(Tools.byte2Str((byte)DST));
                sb.Append(Tools.byte2Str(CMDL));
                //sb.Append(Tools.uint16ToStr(seqNo++));
                lock (TradeStationComm.SeqNoLock)
                {
                    sb.Append(Tools.uint16ToStr(seqNo++));
                }
                return sb;
            }

            public static String getUnixTime()
            {
                //return "1345012233";
                //TimeSpan _TimeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
                //return ((long)_TimeSpan.TotalSeconds).ToString();
                //TimeSpan _TimeSpan = (DateTime.UtcNow - unixTimeStart);
                return ((long)((DateTime.UtcNow - unixTimeStart).TotalSeconds)).ToString();
            }

            public static DateTime getDateTimeFromUnixTime(String unixTimeStr)
            {
                if (unixTimeStr == null || unixTimeStr.Length == 0)
                    return DateTime.MinValue;

                try
                {
                    //return unixTimeStartHK.AddSeconds(Convert.ToDouble(unixTimeStr));
                    return unixTimeStart.AddSeconds(Convert.ToDouble(unixTimeStr)).ToLocalTime();
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }


            public static String getStringFromUnixTime(String unixTimeStr)
            {
                if (unixTimeStr == null || unixTimeStr.Length == 0)
                    return "";

                try
                {
                    //return unixTimeStartHK.AddSeconds(Convert.ToDouble(unixTimeStr));
                    return (unixTimeStart.AddSeconds(Convert.ToDouble(unixTimeStr)).ToLocalTime()).ToString();
                }
                catch
                {
                    return "";
                }
            }

            public static String getDateTimeUserSessionhash(String userID)
            {
                //StringBuilder dusSB = new StringBuilder(Tools.getUnixTime());
                //dusSB.Append(",");
                //dusSB.Append(userID);
                //dusSB.Append(",");
                //dusSB.Append(sessionhash);
                //return dusSB.ToString();
                //return Tools.getUnixTime() + "," + userID + "," + sessionhash;
                //return Tools.getUnixTime() + "," + userID + ",pv=1.1&sh=" + sessionhash;
                return getDateTimeUserSessionhash(userID, null);
            }

            public static String getDateTimeUserSessionhash(String userID, String ac)
            {
                String retStr = Tools.getUnixTime() + "," + userID + ",pv=1.1&sh=" + sessionhash;
                if (ac != null && ac.Length > 0)
                    retStr += "&ac=" + ac;
                if (xInfoCh != null && xInfoCh.Length > 0)
                    retStr += "&ch=" + xInfoCh;
                if (xInfoLang != null && xInfoLang.Length > 0)
                    retStr += "&lang=" + xInfoLang;
                return retStr;
            }

            public static String getLanguageString(Attribute.Language theLanguage)
            {
                switch (theLanguage)
                {
                    case Attribute.Language.English:
                        return "en";

                    case Attribute.Language.ChinesTraditional:
                        return "zh_tw";

                    case Attribute.Language.ChineseSimplified:
                        return "zh_cn";

                    default:
                        return "";
                }
            }

            /// <summary>
            /// before 1.1
            /// </summary>
            //class simpleResponse
            //{
            //    public const int minLength = 3;
            //    public const int offsetResult = 1;
            //    public const int offsetMessage = 2;
            //    public const String success = "0";
            //}
            public class simpleResponse
            {
                public const int minLength = 6;
                public const int offsetDateTime = 0;
                //public const int offsetUserID = 1;
                //public const int offsetAccNo = 2;
                //public const int offsetRInfo = 3;
                public const int offsetResult = 4;
                public const int offsetMessage = 5;
                public const String success = "0";
            }

            public static infoClass.infoBasic getSimpleResponseInfo(String bodyStr)
            {
                //String outString;
                //int outErrorCode;
                //bool isOK = getSimpleResponse(bodyStr, out outString);
                //return new infoClass.infoBasic(isOK, outString);
                //bool isOK = getSimpleResponse(bodyStr, out outString, out outErrorCode);
                //return new infoClass.infoBasic(isOK, outErrorCode, outString);
                return new infoClass.infoBasic(bodyStr);
            }

            public static infoClass.infoBasic getErrorResponseInfo(String bodyStr)
            {
                //String outString;
                //int errorCode = getErrorResponse(bodyStr, out outString);
                //return new infoClass.infoBasic(errorCode, outString);
                return new infoClass.infoBasic(bodyStr);
            }

            //public static bool getSimpleResponse(String bodyStr, out String outString)
            //{
            //    //outString = null;
            //    //bool isOK = false;
            //    //String[] strArr = bodyStr.Split(',');
            //    //if (strArr != null && strArr.Length >= simpleResponse.minLength)
            //    //{
            //    //    if (strArr[simpleResponse.offsetResult].Equals(simpleResponse.success))
            //    //    {
            //    //        isOK = true;
            //    //    }
            //    //    outString = strArr[simpleResponse.offsetMessage];
            //    //}
            //    //return isOK;
            //    int errorCode;
            //    return getSimpleResponse(bodyStr, out outString, out errorCode);
            //}

            //public static bool getSimpleResponse(String bodyStr, out String outString, out int outErrorCode)
            //{
            //    outString = null;
            //    outErrorCode = (int)MsgResponse.errCodeDef.failure;
            //    bool isOK = false;
            //    String[] strArr = bodyStr.Split(',');
            //    if (strArr != null && strArr.Length >= simpleResponse.minLength)
            //    {
            //        if (strArr[simpleResponse.offsetResult].Equals(simpleResponse.success))
            //        {
            //            isOK = true;
            //        }

            //        int number;
            //        if (Int32.TryParse(strArr[simpleResponse.offsetResult], out number))
            //        {
            //            outErrorCode = number;
            //        }

            //        outString = strArr[simpleResponse.offsetMessage];
            //    }
            //    return isOK;
            //}

            //public static int getErrorResponse(String bodyStr, out String outString)
            //{
            //    outString = null;
            //    int errorCode = -1;
            //    String[] strArr = bodyStr.Split(',');
            //    if (strArr != null && strArr.Length >= simpleResponse.minLength)
            //    {
            //        try
            //        {
            //            errorCode = Convert.ToInt32(strArr[simpleResponse.offsetResult]);
            //        }
            //        catch
            //        {
            //        }
            //        outString = strArr[simpleResponse.offsetMessage];
            //    }
            //    return errorCode;
            //}

            private static int getRefNo()
            {
                return ++refNo;
            }

            public static String getRefNoStr(String refNoStr)
            {
                if (refNoStr == null || refNoStr.Length == 0)
                    return getRefNo().ToString();
                else
                    return refNoStr;
            }

            //public static List <String> getElementList(infoItem[] infoItems, int elementIndex)
            //{
            //    List<String> retStrList = null;
            //    if (infoItems != null && infoItems.Length > 0)
            //    {
            //        int size = infoItems.Length;
            //        retStrList = new List<String>(size);
            //        for (int index = 0; index < size; index++)
            //        {
            //            String retStr = infoItems[index].GetElementAt(elementIndex);
            //            if (retStr != null)
            //                retStrList.Add(retStr);
            //        }
            //    }
            //    return retStrList;
            //}

        }
        #endregion


        //class UserInfo
        //{
        //}

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

        public class Attribute
        {
            public enum Language
            {
                English,
                ChinesTraditional,
                ChineseSimplified
            }

            public enum BuySell
            {
                buy = 'B',
                sell = 'S'
            }

            public enum ValidType
            {
                today,
                FAK,
                FOK,
                GTC,
                specTime
            }

            //public enum CondType
            //{
            //    normal,
            //    bullAndBear
            //}

            public enum CondType
            {
                normal = 1,
                limitOrder = 1,
                AO = 3
            }

            public enum StopType
            {
                normalOrder = ' ',
                stopLoss = 'L',
                upTrigger = 'U',
                downTrigger = 'D'
            }

            public enum Active
            {
                inactive,
                active
            }

            public enum AE
            {
                normalUser,
                AE
            }

            public enum TOne
            {
                TOnly,
                TPluseOne
            }

        }

        #region PriceManager
        private class PriceManager
        {
            const Header.DST DST = Header.DST.priceManager;

            public enum cmdClient
            {
                getMarketPrice,
                registerMarketPrice,
                getPriceDepth,
                registerPriceDepth,
                getLongPriceDepth,
                registerLongPriceDepth,
                getProductList,
                getInstrumentList,
                getTicker = 10,
                registerTicker,
                getChart,
            }

            public enum cmdServer
            {
                getMarketPrice,
                registerMarketPrice,
                getPriceDepth,
                registerPriceDepth,
                getLongPriceDepth,
                registerLongPriceDepth,
                getProductList,
                getInstrumentList,
                getTicker = 10,
                registerTicker,
                getChart,
                marketPricePush = 100,
                priceDepthPush = 102,
                longPriceDepthPush = 104,
                tickerPush = 110,
                errorMsg = 255
            }



            public static String genMessage(cmdClient cmd, String userID)
            {
                return genMessage(cmd, userID, null, null, 1);
            }
            public static String genMessage(cmdClient cmd, String userID, String extraStr)
            {
                return genMessage(cmd, userID, null, extraStr, 1);
            }
            public static String genMessage(cmdClient cmd, String userID, String[] productCodeArray)
            {
                return genMessage(cmd, userID, productCodeArray, null, 1);
            }
            public static String genMessage(cmdClient cmd, String userID, String[] productCodeArray, String extraStr, byte SRC)
            {
                String retString = null;
                String bodyStr = Tools.getDateTimeUserSessionhash(userID);
                if (productCodeArray != null)
                    bodyStr += "," + Tools.strArrToStr(productCodeArray);
                if (extraStr != null)
                    bodyStr += "," + extraStr;

                StringBuilder sb = Tools.getHeaderBeforeLen(SRC, DST, (byte)cmd);
                sb.Append(Tools.int32ToStr(bodyStr.Length));
                sb.Append(bodyStr);
                retString = sb.ToString();

                //Debug.WriteLine("PriceManager: " + retString);
                DebugLog("PriceManager:", retString);

                return retString;
            }

            #region get message
            //public static String getMsgGetMarketPrice(String userID, String[] productCodeArray)
            //{
            //    return genMessage(cmdClient.getMarketPrice, userID, productCodeArray);
            //}
            //public static String getMsgRegisterMarketPrice(String userID, String[] productCodeArray)
            //{
            //    return genMessage(cmdClient.registerMarketPrice, userID, productCodeArray);
            //}
            //public static String getMsgGetPriceDepth(String userID, String[] productCodeArray)
            //{
            //    return genMessage(cmdClient.getPriceDepth, userID, productCodeArray);
            //}
            //public static String getMsgRegisterPriceDepth(String userID, String[] productCodeArray)
            //{
            //    return genMessage(cmdClient.registerPriceDepth, userID, productCodeArray);
            //}
            //public static String getMsgGetLongPriceDepth(String userID, String[] productCodeArray)
            //{
            //    return genMessage(cmdClient.getLongPriceDepth, userID, productCodeArray);
            //}
            //public static String getMsgRegisterLongPriceDepth(String userID, String[] productCodeArray)
            //{
            //    return genMessage(cmdClient.registerLongPriceDepth, userID, productCodeArray);
            //}
            //public static String getMsgGetProductList(String userID)
            //{
            //    return genMessage(cmdClient.getProductList, userID, null);
            //}
            //public static String getMsgGetInstrumentList(String userID)
            //{
            //    return genMessage(cmdClient.getInstrumentList, userID, null);
            //}
            #endregion

            #region Server respose
            public static bool getResponseMarketPrice(String bodyStr, out infoClass.MarketPrice mktPriceInfo)
            {
                bool isOK = false;
                mktPriceInfo = new infoClass.MarketPrice(bodyStr);
                if (mktPriceInfo != null)
                    isOK = true;
                return isOK;
            }

            //public static bool getResponseRegisterMarketPrice(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            public static bool getResponsePriceDepth(String bodyStr, out infoClass.PriceDepth priceDepthInfo)
            {
                bool isOK = false;
                priceDepthInfo = new infoClass.PriceDepth(bodyStr);
                if (priceDepthInfo != null)
                    isOK = true;
                return isOK;
            }

            //public static bool getResponseRegisterPriceDepth(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            public static bool getResponseLongPriceDepth(String bodyStr, out infoClass.LongPriceDepth longPriceDepthInfo)
            {
                bool isOK = false;
                longPriceDepthInfo = new infoClass.LongPriceDepth(bodyStr);
                if (longPriceDepthInfo != null)
                    isOK = true;
                return isOK;
            }

            //public static bool getResponseRegisterLongPriceDepth(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}

            public static bool getResponseProductList(String bodyStr, out infoClass.ProductList prodListInfo)
            {
                bool isOK = false;
                prodListInfo = new infoClass.ProductList(bodyStr);
                if (prodListInfo != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseInstrumentList(String bodyStr, out infoClass.InstrumentList instListInfo)
            {
                bool isOK = false;
                instListInfo = new infoClass.InstrumentList(bodyStr);
                if (instListInfo != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseTicker(String bodyStr, out infoClass.Ticker tickerInfo)
            {
                bool isOK = false;
                tickerInfo = new infoClass.Ticker(bodyStr);
                if (tickerInfo != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseChart(String bodyStr, out infoClass.Chart chartInfo)
            {
                bool isOK = false;
                chartInfo = new infoClass.Chart(bodyStr);
                if (chartInfo != null)
                    isOK = true;
                return isOK;
            }
            //public static int getResponseErrorMessage(String bodyStr, out String outString)
            //{
            //    return Tools.getErrorResponse(bodyStr, out outString);
            //}
            #endregion
        }
        #endregion

        #region AccountManager
        private class AccountManager
        {
            const Header.DST DST = Header.DST.accountManager;
            public const String clientTypeTradeStion = "T";    // 'T' trade station; 'M' mobile; 'D' Dealing
            public const String clientTypeDealer = "D";    // 'T' trade station; 'M' mobile; 'D' Dealing

            public enum cmdClient
            {
                login,
                logout,
                changePassword,
                ChangeLanguage,
                getAccountInfo,
                getOrderBookInfo,
                getPositionInfo,
                getPositionSummary,
                getCashInfo,
                getDoneTradeInfo,
                getClearTradeInfo,
                getAccList,
                getMarginCheck,
                getMarginCallList,
                reqCashApproval,
                getCashApprovalInfo,
                getTradeConfOrders,
                getTradeConfTrades,
                reportTradeConf,
                getAccountMaster,
                getSysParam,
                getTradeConfOrderDetail,
                tableNotificationAck = 253,
                notificationAck = 254,
            }

            public enum cmdServer
            {
                login,
                logout,
                changePassword,
                ChangeLanguage,
                getAccountInfo,
                getOrderBookInfo,
                getPositionInfo,
                getPositionSummary,
                getCashInfo,
                getDoneTradeInfo,
                getClearTradeInfo,
                getAccList,
                getMarginCheck,
                getMarginCallList,
                reqCashApproval,
                getCashApprovalInfo,
                getTradeConfOrders,
                getTradeConfTrades,
                reportTradeConf,
                getAccountMaster,
                getSysParam,
                getTradeConfOrderDetail,
                doneTradeInfoPush = 109,
                tableNotification = 253,
                notificationMsg = 254,
                errorMsg = 255
            }

            //class LoginRespose
            //{
            //    public const int minLength = 3;
            //    public const int offsetLoginResult = 1;
            //    public const int offsetLoginMessage = 2;
            //    public const String loginSuccess = "0";
            //}


            public static String genMessage(cmdClient cmd, String userID, String string1, String string2)
            {
                return genMessage(cmd, userID, null, string1, string2, 1);
            }
            public static String genMessage(cmdClient cmd, String userID)
            {
                return genMessage(cmd, userID, null, null, null, 1);
            }
            public static String genMessage(cmdClient cmd, String userID, String ac)
            {
                return genMessage(cmd, userID, ac, null, null, 1);
            }
            public static String genMessage(cmdClient cmd, String userID, String ac, String string1, String string2)
            {
                return genMessage(cmd, userID, ac, string1, string2, 1);
            }
            public static String genMessage(cmdClient cmd, String userID, String ac, String string1, String string2, byte SRC)
            {
                String retString = null;
                String bodyStr = Tools.getDateTimeUserSessionhash(userID, ac);
                if (string1 != null)
                    bodyStr += "," + string1;
                if (string2 != null)
                    bodyStr += "," + string2;

                StringBuilder sb = Tools.getHeaderBeforeLen(SRC, DST, (byte)cmd);
                sb.Append(Tools.int32ToStr(bodyStr.Length));
                sb.Append(bodyStr);
                retString = sb.ToString();

                //Debug.WriteLine("AccountManager: " + retString);
                if (cmd == cmdClient.login)
                {
                    string[] arr = Regex.Split(retString, ",");
                    arr[arr.Length - 2] = "********";
                    DebugLog("AccountManager:", string.Join(",", arr));
                }
                else if (cmd == cmdClient.changePassword)
                {
                    string[] arr = Regex.Split(retString, ",");
                    arr[arr.Length - 2] = "********";
                    arr[arr.Length - 1] = "********";
                    DebugLog("AccountManager:", string.Join(",", arr));
                }
                else
                {
                    DebugLog("AccountManager:", retString);
                }

                return retString;
            }

            #region get message
            //public static String getMsgLogin(String userID, String password)
            //{
            //    return genMessage(cmdClient.login, userID, password, clientType.ToString());
            //}
            //public static String getMsgLogout(String userID)
            //{
            //    return genMessage(cmdClient.logout, userID, null, null);
            //}
            //public static String getMsgChangePassword(String userID, String password, String newPassword)
            //{
            //    return genMessage(cmdClient.changePassword, userID, password, newPassword);
            //}
            //public static String getMsgChangeLanguage(String userID, language theLanguage)
            //{
            //    return genMessage(cmdClient.ChangeLanguage, userID, Tools.getLanguageString(theLanguage), null);
            //}
            //public static String getMsgGetAccountInfo(String userID)
            //{
            //    return genMessage(cmdClient.getAccountInfo, userID, null, null);
            //}
            //public static String getMsgGetOrderBookInfo(String userID)
            //{
            //    return genMessage(cmdClient.getOrderBookInfo, userID, null, null);
            //}
            //public static String getMsgGetPositionInfo(String userID)
            //{
            //    return genMessage(cmdClient.getPositionInfo, userID, null, null);
            //}
            //public static String getMsgGetPositionSummary(String userID)
            //{
            //    return genMessage(cmdClient.getPositionSummary, userID, null, null);
            //}
            //public static String getMsgGetCashInfo(String userID)
            //{
            //    return genMessage(cmdClient.getCashInfo, userID, null, null);
            //}
            //public static String getMsgDoneTradeInfo(String userID)
            //{
            //    return genMessage(cmdClient.getDoneTradeInfo, userID, null, null);
            //}
            //public static String getMsgClearTradeInfo(String userID)
            //{
            //    return genMessage(cmdClient.getClearTradeInfo, userID, null, null);
            //}
            #endregion

            #region Server respose
            public static bool getResponseLogin(String bodyStr, out infoClass.LoginResponse loginResponse)
            {
                bool isOK = false;
                loginResponse = new infoClass.LoginResponse(bodyStr);
                if (loginResponse != null)
                    isOK = true;
                return isOK;
            }
            //public static bool getResponseLogout(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static bool getResponseChangePassword(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static bool getResponseChangeLanguage(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            public static bool getResponseAccountInfo(String bodyStr, out infoClass.AccountInfo acInfo)
            {
                bool isOK = false;
                acInfo = new infoClass.AccountInfo(bodyStr);
                if (acInfo != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseOrderBook(String bodyStr, out infoClass.OrderBook obInfo)
            {
                bool isOK = false;
                obInfo = new infoClass.OrderBook(bodyStr);
                if (obInfo != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponsePosition(String bodyStr, out infoClass.Position posInfo)
            {
                bool isOK = false;
                posInfo = new infoClass.Position(bodyStr);
                if (posInfo != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponsePositionSummary(String bodyStr, out infoClass.PositionSummary posSumInfo)
            {
                bool isOK = false;
                posSumInfo = new infoClass.PositionSummary(bodyStr);
                if (posSumInfo != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseCashInfo(String bodyStr, out infoClass.CashInfo cashInfo)
            {
                bool isOK = false;
                cashInfo = new infoClass.CashInfo(bodyStr);
                if (cashInfo != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseDoneTradeInfo(String bodyStr, out infoClass.DoneTrade doneTradeInfo)
            {
                bool isOK = false;
                doneTradeInfo = new infoClass.DoneTrade(bodyStr);
                if (doneTradeInfo != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseClearTradeInfo(String bodyStr, out infoClass.ClearTrade clrTradeInfo)
            {
                bool isOK = false;
                clrTradeInfo = new infoClass.ClearTrade(bodyStr);
                if (clrTradeInfo != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseGetAccList(String bodyStr, out infoClass.AccList accList)
            {
                bool isOK = false;
                accList = new infoClass.AccList(bodyStr);
                if (accList != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseGetMarginCheck(String bodyStr, out infoClass.MarginCheck marginCheck)
            {
                bool isOK = false;
                marginCheck = new infoClass.MarginCheck(bodyStr);
                if (marginCheck != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseGetMarginCallList(String bodyStr, out infoClass.MarginCheck marginCallList)
            {
                return getResponseGetMarginCheck(bodyStr, out marginCallList);
            }

            public static bool getResponseGetMarginCallList(String bodyStr, out infoClass.MarginCallList marginCallList)
            {
                bool isOK = false;
                marginCallList = new infoClass.MarginCallList(bodyStr);
                if (marginCallList != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseGetTradeConfOrders(String bodyStr, out infoClass.TradeConfOrder tradeConfOrder)
            {
                bool isOK = false;
                tradeConfOrder = new infoClass.TradeConfOrder(bodyStr);
                if (tradeConfOrder != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseGetTradeConfTrades(String bodyStr, out infoClass.TradeConfTrade tradeConfTrade)
            {
                bool isOK = false;
                tradeConfTrade = new infoClass.TradeConfTrade(bodyStr);
                if (tradeConfTrade != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseReportTradeConf(String bodyStr, out infoClass.ReportTradeConfAck reportTradeConfAck)
            {
                bool isOK = false;
                reportTradeConfAck = new infoClass.ReportTradeConfAck(bodyStr);
                if (reportTradeConfAck != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseGetAccountMaster(String bodyStr, out infoClass.AccountMaster accountMaster)
            {
                bool isOK = false;
                accountMaster = new infoClass.AccountMaster(bodyStr);
                if (accountMaster != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseGetTradeConfOrderdetail(String bodyStr, out infoClass.TradeConfOrderDetail tradeConfOrderDetail)
            {
                bool isOK = false;
                tradeConfOrderDetail = new infoClass.TradeConfOrderDetail(bodyStr);
                if (tradeConfOrderDetail != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseNotificationMsg(String bodyStr, out infoClass.NotificationInfo notificationInfo)
            {
                bool isOK = false;
                notificationInfo = new infoClass.NotificationInfo(bodyStr);
                if (notificationInfo != null)
                    isOK = true;
                return isOK;
            }

            public static bool getResponseTableNotificationMsg(String bodyStr, out infoClass.TableNotificationInfo notificationInfo)
            {
                bool isOK = false;
                notificationInfo = new infoClass.TableNotificationInfo(bodyStr);
                if (notificationInfo != null)
                    isOK = true;
                return isOK;
            }

            //public static int getResponseErrorMessage(String bodyStr, out String outString)
            //{
            //    return Tools.getErrorResponse(bodyStr, out outString);
            //}
            #endregion
        }
        #endregion

        #region OrderManager
        private class OrderManager
        {
            const Header.DST DST = Header.DST.orderManager;

            public enum cmdClient
            {
                addOrder,
                changeOrder,
                deleteOrder,
                activateOrder,
                inactivateOrder
            }

            public enum cmdServer
            {
                addOrder,
                changeOrder,
                deleteOrder,
                activateOrder,
                inactivateOrder,
                errorMsg = 255
            }

            //public class BuySell
            //{
            //    private bool isBuy;
            //    public BuySell(bool isBuy)
            //    {
            //        this.isBuy = isBuy;
            //    }
            //    //public void setBuy()
            //    //{
            //    //    isBuy = true;
            //    //}
            //    //public void setSell()
            //    //{
            //    //    isBuy = false;
            //    //}

            //    public override string  ToString()
            //    {
            //        if (isBuy)
            //            return "B";
            //        else
            //            return "S";
            //    }
            //}

            //public enum BuySell
            //{
            //    buy = 'B',
            //    sell = 'S'
            //}

            //public enum ValidType
            //{
            //    today,
            //    FAK,
            //    FOK,
            //    GTC,
            //    specTime
            //}

            //public enum CondType
            //{
            //    normal,
            //    bullAndBear
            //}

            //public enum StopType
            //{
            //    stopLoss = 'L',
            //    upTrigger = 'U',
            //    downTrigger = 'D'
            //}

            //public enum Active
            //{
            //    inactive,
            //    active
            //}

            //public enum AE
            //{
            //    normalUser,
            //    AE
            //}

            //public abstract class OrderClass
            //{
            //    protected String productCode;
            //    protected AE ae;
            //    protected String accNo;

            //    public abstract override string ToString();
            //}

            //public abstract class OrderClass
            //{
            //    protected String productCode;
            //    protected AE ae;
            //    protected String accNo;

            //    public abstract String elementsToString();
            //    //public virtual String elementsToString()
            //    //{
            //    //    StringBuilder sb = new StringBuilder(productCode);
            //    //    sb.Append(',');
            //    //    sb.Append(ae);
            //    //    sb.Append(',');
            //    //    sb.Append(accNo);
            //    //    return sb.ToString();
            //    //}
            //}

            //public class AddOrderClass : OrderClass
            //{
            //    //String productCode;
            //    BuySell buySell;
            //    uint qty;
            //    int price;
            //    ValidType validType;
            //    int refNo;
            //    CondType condType;
            //    StopType stopType;
            //    int stopPrice;
            //    Active active;
            //    long specTime;
            //    //AE ae;
            //    //String accNo;

            //    public AddOrderClass(String productCode, BuySell buySell, uint qty, int price, ValidType validType, CondType condType, StopType stopType, int stopPrice, Active active, long specTime, AE ae, String accNo)
            //    {
            //        this.productCode = productCode;
            //        this.buySell = buySell;
            //        this.qty = qty;
            //        this.price = price;
            //        this.validType = validType;
            //        this.refNo = Tools.getRefNo();
            //        this.condType = condType;
            //        this.stopType = stopType;
            //        this.stopPrice = stopPrice;
            //        this.active = active;
            //        this.specTime = specTime;
            //        this.ae = ae;
            //        this.accNo = accNo;
            //    }

            //    public override String elementsToString()
            //    {
            //        StringBuilder sb = new StringBuilder(productCode);
            //        sb.Append(',');
            //        //sb.Append(buySell.ToString());
            //        sb.Append((char)buySell);
            //        sb.Append(',');
            //        sb.Append(qty);
            //        sb.Append(',');
            //        sb.Append(price);
            //        sb.Append(',');
            //        sb.Append((int)validType);
            //        sb.Append(',');
            //        sb.Append(refNo);
            //        sb.Append(',');
            //        sb.Append((int)condType);
            //        sb.Append(',');
            //        sb.Append((char)stopType);
            //        sb.Append(',');
            //        sb.Append(stopPrice);
            //        sb.Append(',');
            //        sb.Append((int)active);
            //        sb.Append(',');
            //        sb.Append(specTime);
            //        sb.Append(',');
            //        sb.Append((int)ae);
            //        sb.Append(',');
            //        sb.Append(accNo);
            //        return sb.ToString();
            //    }
            //}

            //public class ChangeOrderClass : OrderClass
            //{
            //    String internalOrderNo;
            //    //String productCode;
            //    BuySell buySell;
            //    uint qty;
            //    int price;
            //    int refNo;
            //    //AE ae;
            //    //String accNo;

            //    public ChangeOrderClass(String internalOrderNo, String productCode, BuySell buySell, uint qty, int price, AE ae, String accNo)
            //    {
            //        this.internalOrderNo = internalOrderNo;
            //        this.productCode = productCode;
            //        this.buySell = buySell;
            //        this.qty = qty;
            //        this.price = price;
            //        this.refNo = Tools.getRefNo();
            //        this.ae = ae;
            //        this.accNo = accNo;
            //    }

            //    public override String elementsToString()
            //    {
            //        StringBuilder sb = new StringBuilder(internalOrderNo);
            //        sb.Append(',');
            //        sb.Append(productCode);
            //        sb.Append(',');
            //        //sb.Append(buySell.ToString());
            //        sb.Append((char)buySell);
            //        sb.Append(',');
            //        sb.Append(qty);
            //        sb.Append(',');
            //        sb.Append(price);
            //        sb.Append(',');
            //        sb.Append(refNo);
            //        sb.Append(',');
            //        sb.Append((int)ae);
            //        sb.Append(',');
            //        sb.Append(accNo);
            //        return sb.ToString();
            //    }
            //}

            //public class DeleteOrderClass : OrderClass
            //{
            //    String internalOrderNo;
            //    //String productCode;
            //    int refNo;
            //    //AE ae;
            //    //String accNo;

            //    public DeleteOrderClass(String internalOrderNo, String productCode, AE ae, String accNo)
            //    {
            //        this.internalOrderNo = internalOrderNo;
            //        this.productCode = productCode;
            //        this.refNo = Tools.getRefNo();
            //        this.ae = ae;
            //        this.accNo = accNo;
            //    }

            //    public override String elementsToString()
            //    {
            //        StringBuilder sb = new StringBuilder(internalOrderNo);
            //        sb.Append(',');
            //        sb.Append(productCode);
            //        sb.Append(',');
            //        //sb.Append(buySell.ToString());
            //        sb.Append(refNo);
            //        sb.Append(',');
            //        sb.Append((int)ae);
            //        sb.Append(',');
            //        sb.Append(accNo);
            //        return sb.ToString();
            //    }
            //}

            //public class ActivateOrderClass : OrderClass
            //{
            //    String internalOrderNo;
            //    //String productCode;
            //    BuySell buySell;
            //    uint qty;
            //    int price;
            //    //AE ae;
            //    //String accNo;

            //    public ActivateOrderClass(String internalOrderNo, String productCode, BuySell buySell, uint qty, int price, AE ae, String accNo)
            //    {
            //        this.internalOrderNo = internalOrderNo;
            //        this.productCode = productCode;
            //        this.buySell = buySell;
            //        this.qty = qty;
            //        this.price = price;
            //        this.ae = ae;
            //        this.accNo = accNo;
            //    }

            //    public override String elementsToString()
            //    {
            //        StringBuilder sb = new StringBuilder(internalOrderNo);
            //        sb.Append(',');
            //        sb.Append(productCode);
            //        sb.Append(',');
            //        //sb.Append(buySell.ToString());
            //        sb.Append((char)buySell);
            //        sb.Append(',');
            //        sb.Append(qty);
            //        sb.Append(',');
            //        sb.Append(price);
            //        sb.Append(',');
            //        sb.Append((int)ae);
            //        sb.Append(',');
            //        sb.Append(accNo);
            //        return sb.ToString();
            //    }
            //}

            public static String genMessage(cmdClient cmd, String userID, String string1, MsgRequest.OrderClass.OrderClassBasic orderClass)
            {
                return genMessage(cmd, userID, string1, null, orderClass, 1);
            }
            public static String genMessage(cmdClient cmd, String userID, String string1, String ac, MsgRequest.OrderClass.OrderClassBasic orderClass)
            {
                return genMessage(cmd, userID, string1, ac, orderClass, 1);
            }
            public static String genMessage(cmdClient cmd, String userID, String string1, String ac, MsgRequest.OrderClass.OrderClassBasic orderClass, byte SRC)
            {
                String retString = null;
                String bodyStr = Tools.getDateTimeUserSessionhash(userID, ac);
                if (string1 != null)
                    bodyStr += "," + string1;
                if (orderClass != null)
                    bodyStr += "," + orderClass.elementsToString();

                StringBuilder sb = Tools.getHeaderBeforeLen(SRC, DST, (byte)cmd);
                sb.Append(Tools.int32ToStr(bodyStr.Length));
                sb.Append(bodyStr);
                retString = sb.ToString();

                //Debug.WriteLine("OrderManager: " + retString);
                DebugLog("OrderManager:", retString);

                return retString;
            }
            public static String genMessage(cmdClient cmd, String userID, String string1, String ac, String orderClassString, String moString)
            {
                String retString = null;
                String bodyStr = Tools.getDateTimeUserSessionhash(userID, ac);
                if (string1 != null)
                    bodyStr += "," + string1;
                if (orderClassString != null)
                    bodyStr += "," + orderClassString;
                if (moString != null)
                    bodyStr += moString;

                StringBuilder sb = Tools.getHeaderBeforeLen(1, DST, (byte)cmd);
                sb.Append(Tools.int32ToStr(bodyStr.Length));
                sb.Append(bodyStr);
                retString = sb.ToString();

                //Debug.WriteLine("OrderManager: " + retString);
                DebugLog("OrderManager: ", retString);

                return retString;
            }

            #region get message
            //public static String getMsgAddOrder(String userID, String password, AddOrderClass addOrderClass)
            //{
            //    return genMessage(cmdClient.addOrder, userID, password, addOrderClass);
            //}
            //public static String getMsgChangeOrder(String userID, String password, ChangeOrderClass changeOrderClass)
            //{
            //    return genMessage(cmdClient.changeOrder, userID, password, changeOrderClass);
            //}
            //public static String getMsgDeleteOrder(String userID, String password, DeleteOrderClass deleteOrderClass)
            //{
            //    return genMessage(cmdClient.deleteOrder, userID, password, deleteOrderClass);
            //}
            //public static String getMsgActivateOrder(String userID, String password, ActivateOrderClass activateOrderClass)
            //{
            //    return genMessage(cmdClient.activateOrder, userID, password, activateOrderClass);
            //}
            //public static String getMsgInactivateOrder(String userID, String password, ActivateOrderClass inactivateOrderClass)
            //{
            //    return genMessage(cmdClient.inactivateOrder, userID, password, inactivateOrderClass);
            //}
            #endregion

            #region Server respose
            //public static bool getResponseAddOrder(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static bool getResponseChangeOrder(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static bool getResponseDeleteOrder(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static bool getResponseActivateOrder(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static bool getResponseInactivateOrder(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}

            //public static int getResponseErrorMessage(String bodyStr, out String outString)
            //{
            //    return Tools.getErrorResponse(bodyStr, out outString);
            //}
            #endregion
        }
        #endregion

        #region TradeManager
        private class TradeManager
        {
            const Header.DST DST = Header.DST.tradeManager;

            public enum cmdClient
            {
                //getTicker,
                //registerTicker,
                getTradeStatistics = 2,
                //getChart
            }

            public enum cmdServer
            {
                //getTicker,
                //registerTicker,
                getTradeStatistics = 2,
                //getChart,
                //tickerPush = 100,
                errorMsg = 255
            }

            //public class Ticker : infoTypeNoUser
            //{
            //    public class TickerItem : infoItem
            //    {
            //        public enum tickerItemEnum
            //        {
            //            productCode,
            //            qty,
            //            price,
            //            logTime,
            //            dealSrc,
            //            TOTAL
            //        }

            //        public TickerItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)tickerItemEnum.TOTAL) { }

            //        public TickerItem(infoItem srcInfoItem) : base(srcInfoItem) { }
            //    }

            //    public Ticker(String scrStr) : base(scrStr, (int)TickerItem.tickerItemEnum.TOTAL) { }

            //}

            //public class TradeStatistics : infoTypeNoUser
            //{
            //    public class TradeStatisticsItem : infoItem
            //    {
            //        public enum tradeStatisticsItemEnum
            //        {
            //            productCode,
            //            prev,
            //            inOut,
            //            dayLong,
            //            dayShort,
            //            dayNet,
            //            net,
            //            mktPrice,
            //            pl,
            //            refFxRate,
            //            plBaseCcy,
            //            contract,
            //            TOTAL
            //        }

            //        public TradeStatisticsItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)tradeStatisticsItemEnum.TOTAL) { }

            //        public TradeStatisticsItem(infoItem srcInfoItem) : base(srcInfoItem) { }
            //    }

            //    public TradeStatistics(String scrStr) : base(scrStr, (int)TradeStatisticsItem.tradeStatisticsItemEnum.TOTAL) { }

            //}

            //public class Chart : infoTypeNoUser
            //{
            //    public class ChartItem : infoItem
            //    {
            //        public enum chartItemEnum
            //        {
            //            TOTAL
            //        }

            //        public ChartItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)chartItemEnum.TOTAL) { }

            //        public ChartItem(infoItem srcInfoItem) : base(srcInfoItem) { }
            //    }

            //    public Chart(String scrStr) : base(scrStr, (int)ChartItem.chartItemEnum.TOTAL) { }

            //}

            public static String genMessage(cmdClient cmd, String userID, String[] productCodeArray, String string1, String string2)
            {
                return genMessage(cmd, userID, productCodeArray, string1, string2, 1);
            }
            public static String genMessage(cmdClient cmd, String userID, String[] productCodeArray, String string1, String string2, byte SRC)
            {
                String retString = null;
                String bodyStr = Tools.getDateTimeUserSessionhash(userID);
                if (productCodeArray != null)
                    bodyStr += "," + Tools.strArrToStr(productCodeArray);
                if (string1 != null)
                    bodyStr += "," + string1;
                if (string2 != null)
                    bodyStr += "," + string2;

                StringBuilder sb = Tools.getHeaderBeforeLen(SRC, DST, (byte)cmd);
                sb.Append(Tools.int32ToStr(bodyStr.Length));
                sb.Append(bodyStr);
                retString = sb.ToString();

                //Debug.WriteLine("TradeManager: " + retString);
                DebugLog("TradeManager:", retString);

                return retString;
            }

            #region get message
            //public static String getMsgGetTicker(String userID, String[] productCodeArray)
            //{
            //    return genMessage(cmdClient.getTicker, userID, productCodeArray, null, null);
            //}
            //public static String getMsgRegisterTicker(String userID, String[] productCodeArray)
            //{
            //    return genMessage(cmdClient.registerTicker, userID, productCodeArray, null, null);
            //}
            //public static String getMsgGetTradeStatistics(String userID)
            //{
            //    return genMessage(cmdClient.getTradeStatistics, userID, null, null, null);
            //}
            //public static String getMsgGetChart(String userID, String Productcode, String Period)
            //{
            //    return genMessage(cmdClient.getChart, userID, null, Productcode, Period);
            //}
            #endregion

            #region Server respose
            //public static bool getResponseTicker(String bodyStr, out infoClass.Ticker tickerInfo)
            //{
            //    bool isOK = false;
            //    tickerInfo = new infoClass.Ticker(bodyStr);
            //    if (tickerInfo != null)
            //        isOK = true;
            //    return isOK;
            //}

            //public static bool getResponseRegisterTicker(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}

            public static bool getResponseTradeStatistics(String bodyStr, out infoClass.TradeStatistics tradeStatInfo)
            {
                bool isOK = false;
                tradeStatInfo = new infoClass.TradeStatistics(bodyStr);
                if (tradeStatInfo != null)
                    isOK = true;
                return isOK;
            }

            //public static bool getResponseChart(String bodyStr, out infoClass.Chart chartInfo)
            //{
            //    bool isOK = false;
            //    chartInfo = new infoClass.Chart(bodyStr);
            //    if (chartInfo != null)
            //        isOK = true;
            //    return isOK;
            //}

            //public static int getResponseErrorMessage(String bodyStr, out String outString)
            //{
            //    return Tools.getErrorResponse(bodyStr, out outString);
            //}
            #endregion
        }
        #endregion

        #region HeartBeatManager
        private class HeartBeatManager
        {
            const Header.DST DST = Header.DST.heartBeat;

            private static String genMessage()
            {
                String retString = null;
                StringBuilder sb = Tools.getHeaderBeforeLen(0, DST, 0);
                sb.Append(Tools.int32ToStr(0));
                retString = sb.ToString();

                DebugLog("HeartBeatManager:", retString);

                return retString;
            }

            #region get message
            public static String getMsgHeartBeat()
            {
                return genMessage();
            }
            #endregion
        }
        #endregion

        private static string HexToStringMatchEvaluator(Match match)
        {
            int intValue = int.Parse(match.Groups[1].Value, NumberStyles.AllowHexSpecifier);
            return Convert.ToChar(intValue).ToString();
        }

        //private static bool getMsg(String msg, out byte[] header, out String body)
        //{
        //    header = null;
        //    body = null;
        //    bool isValid = false;
        //    if (msg != null)
        //    {
        //        try
        //        {
        //            //String[] strArr = msg.Split(',');
        //            //if (strArr != null && strArr.Length > 0)
        //            //{
        //            //    string source = Regex.Replace(strArr[0], @"\\x([\dA-F][\dA-F])", HexToStringMatchEvaluator);
        //            //    byte[] bytes = Encoding.ASCII.GetBytes(source);

        //            //    if (bytes != null)
        //            //    {
        //            //        header = new byte[Header.size];
        //            //        Array.Copy(bytes, header, Header.size);
        //            //        int indexStart = strArr[0].Length - Header.size;
        //            //        body = msg.Substring(indexStart + 1);
        //            //        isValid = true;
        //            //    }
        //            //}
        //            int posSeparator = msg.IndexOf(',');
        //            if (posSeparator > 0)
        //            {
        //                String headerStr = msg.Substring(0, posSeparator);
        //                string source = Regex.Replace(headerStr, @"\\x([\dA-F][\dA-F])", HexToStringMatchEvaluator);
        //                byte[] bytes = Encoding.ASCII.GetBytes(source);

        //                if (bytes != null)
        //                {
        //                    header = new byte[Header.size];
        //                    Array.Copy(bytes, header, Header.size);
        //                    int indexStart = headerStr.Length - Header.size;
        //                    body = msg.Substring(indexStart + 1);
        //                    isValid = true;
        //                }
        //            }
        //        }
        //        catch
        //        {
        //        }
        //    }

        //    return isValid;
        //}

        private static bool getMsg(MemoryStream msg, out byte[] header, out String body)
        {
            header = null;
            body = null;
            bool isValid = false;
            if (msg != null)
            {
                try
                {
                    header = new byte[Header.size];
                    msg.Read(header, 0, Header.size);

                    byte[] tmpBodyArr = new byte[msg.Length - msg.Position];
                    msg.Read(tmpBodyArr, 0, tmpBodyArr.Length);
                    body = Encoding.ASCII.GetString(tmpBodyArr, 0, tmpBodyArr.Length);
                    isValid = true;
                }
                catch
                {
                }
            }

            return isValid;
        }

        private static Header.DST getMsgManager(byte[] headerArr, out byte cmd)
        {
            cmd = 0xff;
            Header.DST manager = Header.DST.unknow;
            if (headerArr.Length >= Header.size)
            {
                manager = (Header.DST)headerArr[Header.offsetDST];
                cmd = headerArr[Header.offsetCmdL];
            }
            return manager;
        }

        public static DateTime getDateTimeFromUnixTime(String unixTimeStr)
        {
            return Tools.getDateTimeFromUnixTime(unixTimeStr);
        }

        #region MsgRequest
        public class MsgRequest
        {

            #region OrderClass
            public class OrderClass
            {
                public abstract class OrderClassBasic
                {
                    protected String productCode;
                    protected Attribute.AE ae;
                    protected String accNo;
                    protected Attribute.TOne tplus1;

                    public abstract String elementsToString();
                    //public virtual String elementsToString()
                    //{
                    //    StringBuilder sb = new StringBuilder(productCode);
                    //    sb.Append(',');
                    //    sb.Append(ae);
                    //    sb.Append(',');
                    //    sb.Append(accNo);
                    //    return sb.ToString();
                    //}
                }

                public class AddOrderClass : OrderClassBasic
                {
                    //String productCode;
                    Attribute.BuySell buySell;
                    uint qty;
                    int price;
                    Attribute.ValidType validType;
                    //int refNo;
                    String refNo;
                    Attribute.CondType condType;
                    Attribute.StopType stopType;
                    int stopPrice;
                    Attribute.Active active;
                    long specTime;
                    //AE ae;
                    //String accNo;
                    char chkMarginOpt = '\0';

                    public AddOrderClass(String productCode, Attribute.BuySell buySell, uint qty, int price, Attribute.ValidType validType, Attribute.CondType condType, Attribute.StopType stopType, int stopPrice, Attribute.Active active, long specTime, Attribute.AE ae, String accNo)
                        : this(productCode, buySell, qty, price, validType, condType, stopType, stopPrice, active, specTime, ae, accNo, null, Attribute.TOne.TOnly) { }
                    //: this(productCode, buySell, qty, price, validType, condType, stopType, stopPrice, active, specTime, ae, accNo, Tools.getRefNo().ToString()) { }

                    public AddOrderClass(String productCode, Attribute.BuySell buySell, uint qty, int price, Attribute.ValidType validType, Attribute.CondType condType, Attribute.StopType stopType, int stopPrice, Attribute.Active active, long specTime, Attribute.AE ae, String accNo, String theRef, Attribute.TOne tplus1)
                        : this(productCode, buySell, qty, price, validType, condType, stopType, stopPrice, active, specTime, ae, accNo, theRef, tplus1, '\0') { }

                    public AddOrderClass(String productCode, Attribute.BuySell buySell, uint qty, int price, Attribute.ValidType validType, Attribute.CondType condType, Attribute.StopType stopType, int stopPrice, Attribute.Active active, long specTime, Attribute.AE ae, String accNo, String theRef, Attribute.TOne tplus1, char chkMarginOpt)
                    {
                        this.productCode = productCode;
                        this.buySell = buySell;
                        this.qty = qty;
                        this.price = price;
                        this.validType = validType;
                        //this.refNo = Tools.getRefNo();
                        //this.refNo = Tools.getRefNo().ToString();
                        //if (theRef == null || theRef.Length == 0)
                        //    this.refNo = Tools.getRefNo().ToString();
                        //else
                        //    this.refNo = theRef;
                        this.refNo = Tools.getRefNoStr(theRef);
                        this.condType = condType;
                        this.stopType = stopType;
                        this.stopPrice = stopPrice;
                        this.active = active;
                        this.specTime = specTime;
                        this.ae = ae;
                        this.accNo = accNo;
                        this.tplus1 = tplus1;
                        this.chkMarginOpt = chkMarginOpt;
                    }

                    public override String elementsToString()
                    {
                        StringBuilder sb = new StringBuilder(productCode);
                        sb.Append(',');
                        //sb.Append(buySell.ToString());
                        sb.Append((char)buySell);
                        sb.Append(',');
                        sb.Append(qty);
                        sb.Append(',');
                        sb.Append(price);
                        sb.Append(',');
                        sb.Append((int)validType);
                        sb.Append(',');
                        sb.Append(refNo);
                        sb.Append(',');
                        sb.Append((int)condType);
                        sb.Append(',');
                        sb.Append((char)stopType);
                        sb.Append(',');
                        sb.Append(stopPrice);
                        sb.Append(',');
                        sb.Append((int)active);
                        sb.Append(',');
                        sb.Append(specTime);
                        sb.Append(',');
                        sb.Append((int)ae);
                        sb.Append(',');
                        sb.Append(accNo);
                        sb.Append(',');
                        sb.Append((int)tplus1);
                        sb.Append(',');
                        sb.Append(chkMarginOpt);
                        return sb.ToString();
                    }
                }

                public class ChangeOrderClass : OrderClassBasic
                {
                    String internalOrderNo;
                    //String productCode;
                    Attribute.BuySell buySell;
                    uint qty;
                    int price;
                    // int price;
                    //int refNo;
                    String refNo;
                    //AE ae;
                    //String accNo;
                    int stopPrice;

                    //public ChangeOrderClass(String internalOrderNo, String productCode, Attribute.BuySell buySell, uint qty, int price, Attribute.AE ae, String accNo)
                    //    : this(internalOrderNo, productCode, buySell, qty, price, ae, accNo, Tools.getRefNo().ToString()) { }

                    //public ChangeOrderClass(String internalOrderNo, String productCode, Attribute.BuySell buySell, uint qty, int price, Attribute.AE ae, String accNo, String theRef)
                    //    : this(internalOrderNo, productCode, buySell, qty, price, ae, accNo, theRef, Attribute.TOne.TOnly) { }

                    public ChangeOrderClass(String internalOrderNo, String productCode, Attribute.BuySell buySell, uint qty, int price, Attribute.AE ae, String accNo, String theRef, Attribute.TOne tplus1)
                        : this(internalOrderNo, productCode, buySell, qty, price, ae, accNo, theRef, tplus1, 0) { }
                    public ChangeOrderClass(String internalOrderNo, String productCode, Attribute.BuySell buySell, uint qty, int price, Attribute.AE ae, String accNo, String theRef, Attribute.TOne tplus1, int stopPrice)
                    {
                        this.internalOrderNo = internalOrderNo;
                        this.productCode = productCode;
                        this.buySell = buySell;
                        this.qty = qty;
                        this.price = price;
                        //this.refNo = Tools.getRefNo();
                        //this.refNo = Tools.getRefNo().ToString();
                        //this.refNo = theRef;
                        this.refNo = Tools.getRefNoStr(theRef);
                        this.ae = ae;
                        this.accNo = accNo;
                        this.tplus1 = tplus1;
                        this.stopPrice = stopPrice;
                    }

                    public override String elementsToString()
                    {
                        StringBuilder sb = new StringBuilder(internalOrderNo);
                        sb.Append(',');
                        sb.Append(productCode);
                        sb.Append(',');
                        //sb.Append(buySell.ToString());
                        sb.Append((char)buySell);
                        sb.Append(',');
                        sb.Append(qty);
                        sb.Append(',');
                        sb.Append(price);
                        sb.Append(',');
                        sb.Append(refNo);
                        sb.Append(',');
                        sb.Append((int)ae);
                        sb.Append(',');
                        sb.Append(accNo);
                        sb.Append(',');
                        sb.Append((int)tplus1);
                        sb.Append(',');
                        sb.Append((int)stopPrice);
                        return sb.ToString();
                    }
                }

                public class DeleteOrderClass : OrderClassBasic
                {
                    String internalOrderNo;
                    //String productCode;
                    //int refNo;
                    String refNo;
                    //AE ae;
                    //String accNo;

                    public DeleteOrderClass(String internalOrderNo, String productCode, Attribute.AE ae, String accNo)
                        : this(internalOrderNo, productCode, ae, accNo, null) { }
                    //: this(internalOrderNo, productCode, ae, accNo, Tools.getRefNo().ToString()) { }

                    public DeleteOrderClass(String internalOrderNo, String productCode, Attribute.AE ae, String accNo, String theRef)
                        : this(internalOrderNo, productCode, ae, accNo, theRef, Attribute.TOne.TOnly) { }
                    public DeleteOrderClass(String internalOrderNo, String productCode, Attribute.AE ae, String accNo, String theRef, Attribute.TOne tplus1)
                    {
                        this.internalOrderNo = internalOrderNo;
                        this.productCode = productCode;
                        //this.refNo = Tools.getRefNo();
                        //this.refNo = Tools.getRefNo().ToString();
                        this.refNo = Tools.getRefNoStr(theRef);
                        this.ae = ae;
                        this.accNo = accNo;
                        this.tplus1 = tplus1;
                    }

                    public override String elementsToString()
                    {
                        StringBuilder sb = new StringBuilder(internalOrderNo);
                        sb.Append(',');
                        sb.Append(productCode);
                        sb.Append(',');
                        //sb.Append(buySell.ToString());
                        sb.Append(refNo);
                        sb.Append(',');
                        sb.Append((int)ae);
                        sb.Append(',');
                        sb.Append(accNo);
                        sb.Append(',');
                        sb.Append((int)tplus1);
                        return sb.ToString();
                    }
                }

                public class ActivateOrderClass : OrderClassBasic
                {
                    String internalOrderNo;
                    //String productCode;
                    Attribute.BuySell buySell;
                    uint qty;
                    int price;
                    //AE ae;
                    //String accNo;

                    public ActivateOrderClass(String internalOrderNo, String productCode, Attribute.BuySell buySell, uint qty, int price, Attribute.AE ae, String accNo)
                        : this(internalOrderNo, productCode, buySell, qty, price, ae, accNo, Attribute.TOne.TOnly) { }

                    public ActivateOrderClass(String internalOrderNo, String productCode, Attribute.BuySell buySell, uint qty, int price, Attribute.AE ae, String accNo, Attribute.TOne tplus1)
                    {
                        this.internalOrderNo = internalOrderNo;
                        this.productCode = productCode;
                        this.buySell = buySell;
                        this.qty = qty;
                        this.price = price;
                        this.ae = ae;
                        this.accNo = accNo;
                        this.tplus1 = tplus1;
                    }

                    public override String elementsToString()
                    {
                        StringBuilder sb = new StringBuilder(internalOrderNo);
                        sb.Append(',');
                        sb.Append(productCode);
                        sb.Append(',');
                        //sb.Append(buySell.ToString());
                        sb.Append((char)buySell);
                        sb.Append(',');
                        sb.Append(qty);
                        sb.Append(',');
                        sb.Append(price);
                        sb.Append(',');
                        sb.Append((int)ae);
                        sb.Append(',');
                        sb.Append(accNo);
                        sb.Append(',');
                        sb.Append((int)tplus1);
                        return sb.ToString();
                    }
                }
            }
            #endregion

            #region HeartBeat
            public static String getMsgHeartBeat()
            {
                return HeartBeatManager.getMsgHeartBeat();
            }
            #endregion

            #region PriceManager
            public static String getMsgGetMarketPrice(String userID, String[] productCodeArray)
            {
                QCLog("TX --P-0-GET_MARKET_PRICE:", "userID:" + userID + " product:" + Tools.strArrToStr(productCodeArray));
                return PriceManager.genMessage(PriceManager.cmdClient.getMarketPrice, userID, productCodeArray);
            }
            public static String getMsgRegisterMarketPrice(String userID, String[] productCodeArray)
            {
                QCLog("TX --P-1-REG_MARKET_PRICE:", "userID:" + userID + " product:" + Tools.strArrToStr(productCodeArray));
                return PriceManager.genMessage(PriceManager.cmdClient.registerMarketPrice, userID, productCodeArray);
            }
            public static String getMsgGetPriceDepth(String userID, String[] productCodeArray)
            {
                QCLog("TX --P-2-GET_PRICE_DEPTH:", "userID:" + userID + " product:" + Tools.strArrToStr(productCodeArray));
                return PriceManager.genMessage(PriceManager.cmdClient.getPriceDepth, userID, productCodeArray);
            }
            public static String getMsgRegisterPriceDepth(String userID, String[] productCodeArray)
            {
                QCLog("TX --P-3-REG_PRICE_DEPTH:", "userID:" + userID + " product:" + Tools.strArrToStr(productCodeArray));
                return PriceManager.genMessage(PriceManager.cmdClient.registerPriceDepth, userID, productCodeArray);
            }
            public static String getMsgGetLongPriceDepth(String userID, String[] productCodeArray)
            {
                QCLog("TX --P-4-GET_LONG_PRICE_DEPTH:", "userID:" + userID + " product:" + Tools.strArrToStr(productCodeArray));
                return PriceManager.genMessage(PriceManager.cmdClient.getLongPriceDepth, userID, productCodeArray);
            }
            public static String getMsgRegisterLongPriceDepth(String userID, String[] productCodeArray)
            {
                QCLog("TX --P-5-REG_LONG_PRICE_DEPTH:", "userID:" + userID + " product:" + Tools.strArrToStr(productCodeArray));
                return PriceManager.genMessage(PriceManager.cmdClient.registerLongPriceDepth, userID, productCodeArray);
            }
            public static String getMsgGetProductList(String userID)
            {
                QCLog("TX --P-6-GET_PRODUCT_LIST:", "userID:" + userID);
                return PriceManager.genMessage(PriceManager.cmdClient.getProductList, userID);
            }
            public static String getMsgGetInstrumentList(String userID)
            {
                QCLog("TX --P-7-GET_INSTRUMENT_LIST:", "userID:" + userID);
                return PriceManager.genMessage(PriceManager.cmdClient.getInstrumentList, userID);
            }
            public static String getMsgGetTicker(String userID, String[] productCodeArray)
            {
                QCLog("TX --P-10-GET_TICKER:", "userID:" + userID + " product:" + Tools.strArrToStr(productCodeArray));
                return PriceManager.genMessage(PriceManager.cmdClient.getTicker, userID, productCodeArray);
            }
            public static String getMsgRegisterTicker(String userID, String[] productCodeArray)
            {
                QCLog("TX --P-11-REG_TICKER:", "userID:" + userID + " product:" + Tools.strArrToStr(productCodeArray));
                return PriceManager.genMessage(PriceManager.cmdClient.registerTicker, userID, productCodeArray);
            }
            public static String getMsgGetChart(String userID, String productCode)
            {
                return getMsgGetChart(userID, productCode, "0", "0");
            }
            public static String getMsgGetChart(String userID, String productCode, String period)
            {
                return getMsgGetChart(userID, productCode, period, "0");
            }
            public static String getMsgGetChart(String userID, String productCode, String period, String startTime)
            {
                QCLog("TX --P-12-GET_CHART:", "userID:" + userID + " product:" + productCode + " period:" + period + " startTime:" + startTime);
                return PriceManager.genMessage(PriceManager.cmdClient.getChart, userID, productCode + "," + period + "," + startTime);
            }
            #endregion

            #region AccountManager
            //public static String getMsgLogin(String userID, String password)
            //{
            //    //return AccountManager.genMessage(AccountManager.cmdClient.login, userID, password, AccountManager.clientType.ToString());
            //    return getMsgLogin(userID, password, false);
            //}
            public static String getMsgLogin(String userID, String password, Boolean isDealer)
            {
                //QCLog("TX A---0-LOGIN:", (isDealer ? "Dealer:" : "User:") + userID + " password:" + "********");
                //xInfoCh = isDealer ? AccountManager.clientTypeDealer : AccountManager.clientTypeTradeStion;
                ////return AccountManager.genMessage(AccountManager.cmdClient.login, userID, password, isDealer ? AccountManager.clientTypeDealer : AccountManager.clientTypeTradeStion);
                //return AccountManager.genMessage(AccountManager.cmdClient.login, userID, password, xInfoCh);
                return getMsgLogin(userID, password, isDealer, Attribute.Language.ChinesTraditional);
            }
            public static String getMsgLogin(String userID, String password, Boolean isDealer, Attribute.Language theLanguage)
            {
                QCLog("TX A---0-LOGIN:", (isDealer ? "Dealer:" : "User:") + userID + " password:" + "********");
                xInfoCh = isDealer ? AccountManager.clientTypeDealer : AccountManager.clientTypeTradeStion;
                xInfoLang = Tools.getLanguageString(theLanguage);
                //return AccountManager.genMessage(AccountManager.cmdClient.login, userID, password, isDealer ? AccountManager.clientTypeDealer : AccountManager.clientTypeTradeStion);
                return AccountManager.genMessage(AccountManager.cmdClient.login, userID, password, xInfoCh);
            }
            public static String getMsgLogout(String userID)
            {
                QCLog("TX A---1-LOGOUT:", userID);
                return AccountManager.genMessage(AccountManager.cmdClient.logout, userID, null, null);
            }
            public static String getMsgChangePassword(String userID, String password, String newPassword)
            {
                QCLog("TX A---2-CHANGE_PWD:", "userID:" + userID + " OLD password:" + "********" + " NEW password:" + "********");
                return AccountManager.genMessage(AccountManager.cmdClient.changePassword, userID, password, newPassword);
            }
            public static String getMsgChangeLanguage(String userID, Attribute.Language theLanguage)
            {
                QCLog("TX A---3-CHANGE_LAN:", "userID:" + userID + " Language:" + Enum.GetName(typeof(Attribute.Language), theLanguage));
                //changeLang = Tools.getLanguageString(theLanguage);
                xInfoLang = Tools.getLanguageString(theLanguage);
                return AccountManager.genMessage(AccountManager.cmdClient.ChangeLanguage, userID, Tools.getLanguageString(theLanguage), null);
            }
            public static String getMsgGetAccountInfo(String userID)
            {
                QCLog("TX A---4-GET_ACC_INFO:", "userID:" + userID);
                //return AccountManager.genMessage(AccountManager.cmdClient.getAccountInfo, userID, null, null);
                return getMsgGetAccountInfo(userID, null);
            }
            public static String getMsgGetAccountInfo(String userID, String ac)
            {
                QCLog("TX A---4-GET_ACC_INFO:", "userID:" + userID + " ac:" + ac);
                return AccountManager.genMessage(AccountManager.cmdClient.getAccountInfo, userID, ac);
            }
            public static String getMsgGetOrderBookInfo(String userID)
            {
                QCLog("TX A---5-GET_ORDER_BOOK:", "userID:" + userID);
                //return AccountManager.genMessage(AccountManager.cmdClient.getOrderBookInfo, userID, null, null);
                return getMsgGetOrderBookInfo(userID, null);
            }
            public static String getMsgGetOrderBookInfo(String userID, String ac)
            {
                QCLog("TX A---5-GET_ORDER_BOOK:", "userID:" + userID + " ac:" + ac);
                return AccountManager.genMessage(AccountManager.cmdClient.getOrderBookInfo, userID, ac);
            }
            public static String getMsgGetPositionInfo(String userID)
            {
                QCLog("TX A---6-GET_POSITION:", "userID:" + userID);
                //return AccountManager.genMessage(AccountManager.cmdClient.getPositionInfo, userID, null, null);
                return getMsgGetPositionInfo(userID, null);
            }
            public static String getMsgGetPositionInfo(String userID, String ac)
            {
                QCLog("TX A---6-GET_POSITION:", "userID:" + userID + " ac:" + ac);
                return AccountManager.genMessage(AccountManager.cmdClient.getPositionInfo, userID, ac);
            }
            //public static String getMsgGetPositionSummary(String userID)
            //{
            //    //return AccountManager.genMessage(AccountManager.cmdClient.getPositionSummary, userID, null, null);
            //    return getMsgGetPositionSummary(userID, null);
            //}
            //public static String getMsgGetPositionSummary(String userID, String ac)
            //{
            //    return AccountManager.genMessage(AccountManager.cmdClient.getPositionSummary, userID, ac);
            //}
            public static String getMsgGetCashInfo(String userID)
            {
                QCLog("TX A---8-GET_CASH:", "userID:" + userID);
                //return AccountManager.genMessage(AccountManager.cmdClient.getCashInfo, userID, null, null);
                return getMsgGetCashInfo(userID, null);
            }
            public static String getMsgGetCashInfo(String userID, String ac)
            {
                QCLog("TX A---8-GET_CASH:", "userID:" + userID + " ac:" + ac);
                return AccountManager.genMessage(AccountManager.cmdClient.getCashInfo, userID, ac);
            }
            public static String getMsgDoneTradeInfo(String userID)
            {
                QCLog("TX A---9-GET_DONE_TRADE:", "userID:" + userID);
                //return AccountManager.genMessage(AccountManager.cmdClient.getDoneTradeInfo, userID, null, null);
                return getMsgDoneTradeInfo(userID, null);
            }
            public static String getMsgDoneTradeInfo(String userID, String ac)
            {
                QCLog("TX A---9-GET_DONE_TRADE:", "userID:" + userID + " ac:" + ac);
                return AccountManager.genMessage(AccountManager.cmdClient.getDoneTradeInfo, userID, ac);
            }
            public static String getMsgClearTradeInfo(String userID)
            {
                QCLog("TX A---10-GET_CLEAR_TRADE:", "userID:" + userID);
                //return AccountManager.genMessage(AccountManager.cmdClient.getClearTradeInfo, userID, null, null);
                return getMsgClearTradeInfo(userID, null);
            }
            public static String getMsgClearTradeInfo(String userID, String ac)
            {
                QCLog("TX A---10-GET_CLEAR_TRADE:", "userID:" + userID + " ac:" + ac);
                return AccountManager.genMessage(AccountManager.cmdClient.getClearTradeInfo, userID, ac);
            }
            public static String getMsgAccList(String userID)
            {
                QCLog("TX A---11-GET_ACC_LIST:", "userID:" + userID);
                return AccountManager.genMessage(AccountManager.cmdClient.getAccList, userID);
            }
            public static String getMsMarginCheck(String userID)
            {
                QCLog("TX A---12-GET_MARGIN_CHECK:", "userID:" + userID);
                return AccountManager.genMessage(AccountManager.cmdClient.getMarginCheck, userID);
            }
            public static String getMsgMarginCallList(String userID)
            {
                QCLog("TX A---12-GET_MARGIN_CHECK MarginCallList:", "userID:" + userID);
                //return AccountManager.genMessage(AccountManager.cmdClient.getMarginCallList, userID);
                return AccountManager.genMessage(AccountManager.cmdClient.getMarginCheck, userID, null, "C", null);
            }
            //public static String getMsgReqCashApproval(String userID)
            //{
            //    return AccountManager.genMessage(AccountManager.cmdClient.reqCashApproval, userID);
            //}
            //public static String getMsgGetCashApprovalInfo(String userID)
            //{
            //    return AccountManager.genMessage(AccountManager.cmdClient.getCashApprovalInfo, userID);
            //}
            public static String getMsgTradeConfOrders(String userID)
            {
                QCLog("TX A---16-GET_TRADE_CONF_ORDERS:", "userID:" + userID);
                return AccountManager.genMessage(AccountManager.cmdClient.getTradeConfOrders, userID);
            }
            public static String getMsgTradeConfTrades(String userID)
            {
                QCLog("TX A---17-GET_TRADE_CONF_TRADES:", "userID:" + userID);
                return AccountManager.genMessage(AccountManager.cmdClient.getTradeConfTrades, userID);
            }
            //public static String getMsgReportTradeConf(String userID, String ac, String orderNo, String tradeNo, String recNo)
            //{
            //    QCLog("TX A---18-REPORT_TRADE_CONF:", "userID:" + userID + " ac:" + ac + " orderNo:" + orderNo + " tradeNo:" + tradeNo + " recNo:" + recNo);
            //    return AccountManager.genMessage(AccountManager.cmdClient.reportTradeConf, userID, ac, orderNo + "," + tradeNo, recNo);
            //}
            public static String getMsgReportTradeConf(String userID, String recNo, int action)
            {
                QCLog("TX A---18-REPORT_TRADE_CONF:", "userID:" + userID + " recNo:" + recNo + " action:" + action);
                return AccountManager.genMessage(AccountManager.cmdClient.reportTradeConf, userID, null, ",,,,1," + recNo, action.ToString());
            }
            public static String getMsgReportTradeConf(String userID, String[] recNo, int action)
            {
                String str = "";
                if (recNo != null)
                {
                    int strLen = recNo.Length;
                    if (strLen > 0)
                    {
                        str = strLen.ToString();
                        for (int index = 0; index < strLen; index++)
                        {
                            str += "," + recNo[index] + "," + action.ToString();
                        }
                        QCLog("TX A---18-REPORT_TRADE_CONF:", "userID:" + userID + " str:" + str);
                        return AccountManager.genMessage(AccountManager.cmdClient.reportTradeConf, userID, null, ",,,", str);
                    }
                }
                return str;
            }
            public static String getMsgAccountMaster(String userID, String accNoSearch, String accNameSearch, String idNoSearch)
            {
                QCLog("TX A---19-GET_ACCOUNT_MASTER:", "userID:" + userID + " accNoSearch:" + accNoSearch + " accNameSearch:" + accNameSearch + " idNoSearch:" + idNoSearch);
                String stringSearch = "";
                if (accNoSearch != null)
                    stringSearch += accNoSearch + ",";
                else
                    stringSearch += ",";
                if (accNameSearch != null)
                    stringSearch += accNameSearch + ",";
                else
                    stringSearch += ",";
                if (idNoSearch != null)
                    stringSearch += idNoSearch;
                return AccountManager.genMessage(AccountManager.cmdClient.getAccountMaster, userID, null, stringSearch, null);
            }
            public static String getMsgTradeConfOrderDetail(String userID, String ac, String orderNo)
            {
                QCLog("TX A---21-GET_TRADE_CONF_ORDER_DETAIL:", "userID:" + userID + " ac:" + ac + " orderNo:" + orderNo);
                return AccountManager.genMessage(AccountManager.cmdClient.getTradeConfOrderDetail, userID, ac, orderNo, null);
            }
            public static String getMsgTableNotificationAck(String userID, String ac, String tableCode, String versionNo)
            {
                QCLog("TX A---253-TABLE_NOTIFY_ACK:", "userID:" + userID + " ac:" + ac + " tableCode:" + tableCode + " versionNo:" + versionNo);
                return AccountManager.genMessage(AccountManager.cmdClient.tableNotificationAck, userID, ac, tableCode, versionNo);
            }
            public static String getMsgNotificationAck(String userID, String ac, String notifyCode, String seqNo)
            {
                QCLog("TX A---254-MSG_NOTIFY_ACK:", "userID:" + userID + " ac:" + ac + " notifyCode:" + notifyCode + " seqNo:" + seqNo);
                return AccountManager.genMessage(AccountManager.cmdClient.notificationAck, userID, ac, notifyCode, seqNo);
            }
            #endregion

            #region OrderManager
            //public static String getMsgAddOrder(String userID, String password, MsgRequest.OrderClass.AddOrderClass addOrderClass)
            //{
            //    //return OrderManager.genMessage(OrderManager.cmdClient.addOrder, userID, password, addOrderClass);
            //    return getMsgAddOrder(userID, password, null, addOrderClass);
            //}
            public static String getMsgAddOrder(String userID, String password, String ac, String addOrderString, String moString)
            {
                QCLog("TX -O--0-ADD_ORDER request:", "userID:" + userID + " ac:" + ac + " addOrderString:" + addOrderString + " moString:" + moString);
                return OrderManager.genMessage(OrderManager.cmdClient.addOrder, userID, password, ac, addOrderString, moString);
            }
            public static String getMsgAddOrder(String userID, String password, String ac, MsgRequest.OrderClass.AddOrderClass addOrderClass)
            {
                QCLog("TX -O--0-ADD_ORDER:", "userID:" + userID + " ac:" + ac + " AddOrderClass:" + addOrderClass.elementsToString());
                return OrderManager.genMessage(OrderManager.cmdClient.addOrder, userID, password, ac, addOrderClass);
            }
            public static String getMsgChangeOrder(String userID, String password, MsgRequest.OrderClass.ChangeOrderClass changeOrderClass)
            {
                QCLog("TX -O--1-CHANGE_ORDER:", "userID:" + userID + " ChangeOrderClass:" + changeOrderClass.elementsToString());
                //return OrderManager.genMessage(OrderManager.cmdClient.changeOrder, userID, password, changeOrderClass);
                return getMsgChangeOrder(userID, password, null, changeOrderClass);
            }
            public static String getMsgChangeOrder(String userID, String password, String ac, MsgRequest.OrderClass.ChangeOrderClass changeOrderClass)
            {
                QCLog("TX -O--1-CHANGE_ORDER:", "userID:" + userID + " ac:" + ac + " ChangeOrderClass:" + changeOrderClass.elementsToString());
                return OrderManager.genMessage(OrderManager.cmdClient.changeOrder, userID, password, ac, changeOrderClass);
            }
            public static String getMsgDeleteOrder(String userID, String password, MsgRequest.OrderClass.DeleteOrderClass deleteOrderClass)
            {
                QCLog("TX -O--2-DELETE_ORDER:", "userID:" + userID + " DeleteOrderClass:" + deleteOrderClass.elementsToString());
                //return OrderManager.genMessage(OrderManager.cmdClient.deleteOrder, userID, password, deleteOrderClass);
                return getMsgDeleteOrder(userID, password, null, deleteOrderClass);
            }
            public static String getMsgDeleteOrder(String userID, String password, String ac, MsgRequest.OrderClass.DeleteOrderClass deleteOrderClass)
            {
                QCLog("TX -O--2-DELETE_ORDER:", "userID:" + userID + " ac:" + ac + " DeleteOrderClass:" + deleteOrderClass.elementsToString());
                return OrderManager.genMessage(OrderManager.cmdClient.deleteOrder, userID, password, ac, deleteOrderClass);
            }
            public static String getMsgActivateOrder(String userID, String password, MsgRequest.OrderClass.ActivateOrderClass activateOrderClass)
            {
                QCLog("TX -O--3-ACTIVE_ORDER:", "userID:" + userID + " ActivateOrderClass:" + activateOrderClass.elementsToString());
                //return OrderManager.genMessage(OrderManager.cmdClient.activateOrder, userID, password, activateOrderClass);
                return getMsgActivateOrder(userID, password, activateOrderClass);
            }
            public static String getMsgActivateOrder(String userID, String password, String ac, MsgRequest.OrderClass.ActivateOrderClass activateOrderClass)
            {
                QCLog("TX -O--3-ACTIVE_ORDER:", "userID:" + userID + " ac:" + ac + " ActivateOrderClass:" + activateOrderClass.elementsToString());
                return OrderManager.genMessage(OrderManager.cmdClient.activateOrder, userID, password, ac, activateOrderClass);
            }
            public static String getMsgInactivateOrder(String userID, String password, MsgRequest.OrderClass.ActivateOrderClass inactivateOrderClass)
            {
                QCLog("TX -O--4-INACTIVE_ORDER:", "userID:" + userID + " ActivateOrderClass:" + inactivateOrderClass.elementsToString());
                //return OrderManager.genMessage(OrderManager.cmdClient.inactivateOrder, userID, password, inactivateOrderClass);
                return getMsgInactivateOrder(userID, password, null, inactivateOrderClass);
            }
            public static String getMsgInactivateOrder(String userID, String password, String ac, MsgRequest.OrderClass.ActivateOrderClass inactivateOrderClass)
            {
                QCLog("TX -O--4-INACTIVE_ORDER:", "userID:" + userID + " ac:" + ac + " ActivateOrderClass:" + inactivateOrderClass.elementsToString());
                return OrderManager.genMessage(OrderManager.cmdClient.inactivateOrder, userID, password, ac, inactivateOrderClass);
            }
            #endregion

            #region TradeManager
            //public static String getMsgGetTicker(String userID, String[] productCodeArray)
            //{
            //    return TradeManager.genMessage(TradeManager.cmdClient.getTicker, userID, productCodeArray, null, null);
            //}
            //public static String getMsgRegisterTicker(String userID, String[] productCodeArray)
            //{
            //    return TradeManager.genMessage(TradeManager.cmdClient.registerTicker, userID, productCodeArray, null, null);
            //}
            public static String getMsgGetTradeStatistics(String userID)
            {
                return TradeManager.genMessage(TradeManager.cmdClient.getTradeStatistics, userID, null, null, null);
            }
            //public static String getMsgGetChart(String userID, String Productcode, String Period)
            //{
            //    return TradeManager.genMessage(TradeManager.cmdClient.getChart, userID, null, Productcode, Period);
            //}
            #endregion
        }
        #endregion

        #region infoClass
        public class infoClass
        {
            /// <summary>
            /// !retStrList.Contains(retStr) to avoid duplicate items
            /// </summary>
            /// <param name="infoItems"></param>
            /// <param name="elementIndex"></param>
            /// <returns></returns>
            private static List<String> getElementList(infoItem[] infoItems, int elementIndex)
            {
                List<String> retStrList = null;
                if (infoItems != null && infoItems.Length > 0)
                {
                    int size = infoItems.Length;
                    retStrList = new List<String>(size);
                    for (int index = 0; index < size; index++)
                    {
                        String retStr = infoItems[index].GetElementAt(elementIndex);
                        if (retStr != null && !retStrList.Contains(retStr))
                            retStrList.Add(retStr);
                    }
                }
                return retStrList;
            }

            private static infoItem getInfoItem(infoItem srcInfoItem, int elementIndex, String findStr)
            {
                infoItem theInfoItem = null;
                if (srcInfoItem != null)
                {
                    if (srcInfoItem.GetElementAt(elementIndex) == findStr)
                    {
                        theInfoItem = srcInfoItem;
                    }
                }
                return theInfoItem;
            }

            private static infoItem getInfoItem(infoItem[] infoItems, int elementIndex, String findStr)
            {
                infoItem theInfoItem = null;
                if (infoItems != null && infoItems.Length > 0)
                {
                    foreach (infoItem loopInfoItem in infoItems)
                    {
                        if (loopInfoItem.GetElementAt(elementIndex) == findStr)
                        {
                            theInfoItem = loopInfoItem;
                            break;
                        }
                    }
                }
                return theInfoItem;
            }

            public class infoItem
            {
                protected String[] allItem;
                public String[] AllItem
                {
                    get { return allItem; }
                }
                private int itemLen;
                public int ItemLen
                {
                    get { return itemLen; }
                }
                public String GetElementAt(int elementIndex)
                {
                    if (allItem != null && allItem.Length > elementIndex)
                    {
                        return allItem[elementIndex];
                    }
                    return null;
                }

                public infoItem(String[] scrStrArr, int startIndex, int itemLen)
                {
                    this.allItem = null;
                    this.itemLen = itemLen;
                    if (scrStrArr != null && scrStrArr.Length >= startIndex + itemLen)
                    {
                        allItem = new String[itemLen];
                        Array.Copy(scrStrArr, startIndex, allItem, 0, itemLen);
                    }
                }

                public infoItem(infoItem srcInfoItem)
                {
                    if (srcInfoItem != null)
                    {
                        this.allItem = srcInfoItem.allItem;
                        this.itemLen = srcInfoItem.itemLen;
                    }
                }

            }

            //public class infoBasic
            //{
            //    DateTime datetime;
            //    String userID;
            //    String accNo;   //p1.1
            //    String rInfo;   //p1.1

            //    public DateTime Datetime
            //    {
            //        get { return datetime; }
            //    }
            //    public String UserID
            //    {
            //        get { return userID; }
            //    }
            //    public String AccNo
            //    {
            //        get { return accNo; }
            //    }
            //    public String RInfo
            //    {
            //        get { return rInfo; }
            //    }

            //    bool isSuccess;
            //    public bool IsSuccess
            //    {
            //        get { return isSuccess; }
            //    }
            //    String responseString;
            //    public String ResponseString
            //    {
            //        get { return responseString; }
            //    }
            //    int errorCode;
            //    public int ErrorCode
            //    {
            //        get { return errorCode; }
            //    }

            //    //public infoBasic(bool isSuccess, String responseString) : this(isSuccess, -1, responseString) { }

            //    public infoBasic(int errorCode, String responseString) : this(true, errorCode, responseString) { }

            //    public infoBasic(bool isSuccess, int errorCode, String responseString) : this(isSuccess, errorCode, responseString, DateTime.Now, "", "", "") {}

            //    public infoBasic(bool isSuccess, int errorCode, String responseString, DateTime datetime, String userID, String accNo, String rInfo)
            //    {
            //        this.isSuccess = isSuccess;
            //        this.errorCode = errorCode;
            //        this.responseString = responseString;
            //        this.datetime = datetime;
            //        this.userID = userID;
            //        this.accNo = accNo;
            //        this.rInfo = rInfo;
            //    }
            //}

            public class infoBasic : infoTypeOneLayer
            {
                bool isSuccess;
                public bool IsSuccess
                {
                    get { return isSuccess; }
                }
                String responseString;
                public String ResponseString
                {
                    get { return responseString; }
                }
                int errorCode;
                public int ErrorCode
                {
                    get { return errorCode; }
                }


                public class infoBasicItem : infoItem
                {
                    public enum infoBasictemEnum
                    {
                        datetime,
                        userID,
                        accNo,  //p1.1
                        rInfo,  //p1.1
                        result,
                        message,
                        TOTAL
                    }

                    public infoBasicItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)infoBasictemEnum.TOTAL) { }

                    public infoBasicItem(infoItem srcInfoItem) : base(srcInfoItem) { }

                    public DateTime getTime()
                    {
                        DateTime retTime = DateTime.MinValue;
                        if (allItem != null && allItem.Length == (int)infoBasictemEnum.TOTAL)
                        {
                            retTime = Tools.getDateTimeFromUnixTime(allItem[(int)infoBasictemEnum.datetime]);
                        }
                        return retTime;
                    }

                    //public override MsgResponse.responseInfo GetResponseInfo()
                    //{
                    //    return new MsgResponse.responseInfo(allItem[(int)infoBasictemEnum.datetime], allItem[(int)infoBasictemEnum.userID], allItem[(int)infoBasictemEnum.accNo], allItem[(int)infoBasictemEnum.rInfo]);
                    //}
                }

                public infoBasic(String scrStr)
                    : base(scrStr, (int)infoBasicItem.infoBasictemEnum.TOTAL)
                {
                    this.isSuccess = false;
                    this.errorCode = (int)MsgResponse.errCodeDef.failure;
                    this.responseString = null;
                    if (this.InfoItem != null)
                    {
                        if (this.InfoItem.AllItem[(int)infoBasicItem.infoBasictemEnum.result].Equals(Tools.simpleResponse.success))
                        {
                            this.isSuccess = true;
                        }

                        int number;
                        if (Int32.TryParse(this.InfoItem.AllItem[(int)infoBasicItem.infoBasictemEnum.result], out number))
                        {
                            this.errorCode = number;
                        }
                        this.responseString = this.InfoItem.AllItem[(int)infoBasicItem.infoBasictemEnum.message];
                    }
                    else
                    {
                        this.errorCode = (int)MsgResponse.errCodeDef.ok;
                    }
                }


                //public infoBasic(bool isSuccess, String responseString) : this(isSuccess, -1, responseString) { }

                //public infoBasic(int errorCode, String responseString) : this(true, errorCode, responseString) { }

                //public infoBasic(bool isSuccess, int errorCode, String responseString)
                //{
                //    this.isSuccess = isSuccess;
                //    this.errorCode = errorCode;
                //    this.responseString = responseString;
                //}

                public OrderResponse GetOrderResponse(string[] items)
                {
                    OrderResponse orderResponse = new OrderResponse();

                    if (items != null && items.Count() > 0)
                    {
                        string dateTime = TradeStationTools.getDateTimeFromUnixTime(items[0]).ToString();
                        orderResponse.Datetime = dateTime;
                        orderResponse.Type = "Order";
                        orderResponse.UserID = items[1];
                        orderResponse.Acc_no = items[2];
                        orderResponse.Rinfo = items[3];
                        orderResponse.Result = Convert.ToInt32(items[4]);
                        //orderResponse.Message = items[5]; 
                        orderResponse.B64Message = items[5];
                    }
                    return orderResponse;
                }

            }

            #region Basic Info Type
            public abstract class infoType : infoObjectAbs
            {
                DateTime datetime;
                String userID;
                String accNo;   //p1.1
                String rInfo;   //p1.1
                infoItem[] infoItems;

                public DateTime Datetime
                {
                    get { return datetime; }
                }
                public String UserID
                {
                    get { return userID; }
                }
                public String AccNo
                {
                    get { return accNo; }
                }
                public String RInfo
                {
                    get { return rInfo; }
                }

                // add by ben for generating table
                public infoItem[] InfoItems
                {
                    get { return infoItems; }
                }

                enum offsetEnum
                {
                    datetime,
                    userID,
                    accNo,  //p1.1
                    rInfo,  //p1.1
                    numOfItem,
                    itemStart
                }

                public infoType(String scrStr, int itemLen) : this(scrStr, itemLen, 0) { }


                public infoType(String scrStr, int itemLen, int extraLenBeforeStart)
                {
                    datetime = DateTime.MinValue;
                    userID = null;
                    accNo = null; //p1.1
                    rInfo = null; //p1.1
                    infoItems = null;
                    if (scrStr != null && scrStr.Length > 0)
                    {
                        String[] allItem = scrStr.Split(',');
                        int adjustedNumOfItem = (int)offsetEnum.numOfItem + extraLenBeforeStart;
                        if (allItem != null && allItem.Length > adjustedNumOfItem)
                        {
                            datetime = Tools.getDateTimeFromUnixTime(allItem[(int)offsetEnum.datetime]);
                            userID = allItem[(int)offsetEnum.userID];
                            accNo = allItem[(int)offsetEnum.accNo]; //p1.1
                            rInfo = allItem[(int)offsetEnum.rInfo]; //p1.1
                            int numOfItem = Convert.ToInt32(allItem[adjustedNumOfItem]);
                            int adjustedItemStart = (int)offsetEnum.itemStart + extraLenBeforeStart;
                            if (allItem.Length == numOfItem * itemLen + adjustedItemStart)
                            {
                                int startIndex = adjustedItemStart;
                                infoItems = new infoItem[numOfItem];
                                for (int index = 0; startIndex < allItem.Length; index++)
                                {
                                    infoItems[index] = new infoItem(allItem, startIndex, itemLen);
                                    startIndex += itemLen;
                                }
                            }
                        }
                    }
                }

                protected List<String> GetElementList(int index)
                {
                    //return Tools.getElementList(infoItems, index);
                    return getElementList(infoItems, index);
                }

                protected infoItem GetInfoItem(int index, String productCode)
                {
                    //return Tools.getInfoItem(infoItems, index, productCode);
                    return getInfoItem(infoItems, index, productCode);
                }

                protected infoItem GetInfoItem(int itemIndex)
                {
                    if (infoItems != null && infoItems.Length > itemIndex)
                        return infoItems[itemIndex];
                    else
                        return null;
                }

                public override MsgResponse.responseInfo GetResponseInfo()
                {
                    return new MsgResponse.responseInfo(datetime, userID, accNo, rInfo);
                }

                //public OrderResponse GetOrderResponse()
                //{
                //    OrderResponse data = new OrderResponse();

                //    return data;
                //}
            }

            ///// <summary>
            ///// For items which don't have UserID
            ///// </summary>
            //public abstract class infoTypeNoUser
            //{
            //    DateTime datetime;
            //    infoItem[] infoItems;
            //    public infoItem[] InfoItems
            //    {
            //        get { return infoItems; }
            //    }

            //    enum offsetEnum
            //    {
            //        datetime,
            //        numOfItem,
            //        itemStart
            //    }

            //    public infoTypeNoUser(String scrStr, int itemLen)
            //    {
            //        datetime = DateTime.MinValue;
            //        infoItems = null;
            //        if (scrStr != null && scrStr.Length > 0)
            //        {
            //            String[] allItem = scrStr.Split(',');
            //            if (allItem != null && allItem.Length > (int)offsetEnum.numOfItem)
            //            {
            //                datetime = Tools.getDateTimeFromUnixTime(allItem[(int)offsetEnum.datetime]);
            //                int numOfItem = Convert.ToInt32(allItem[(int)offsetEnum.numOfItem]);
            //                if (allItem.Length == numOfItem * itemLen + (int)offsetEnum.itemStart)
            //                {
            //                    int startIndex = (int)offsetEnum.itemStart;
            //                    infoItems = new infoItem[numOfItem];
            //                    for (int index = 0; startIndex < allItem.Length; index++)
            //                    {
            //                        infoItems[index] = new infoItem(allItem, startIndex, itemLen);
            //                        startIndex += itemLen;
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    protected List<String> GetElementList(int index)
            //    {
            //        //return Tools.getElementList(infoItems, index);
            //        return getElementList(infoItems, index);
            //    }

            //    protected infoItem GetInfoItem(int index, String productCode)
            //    {
            //        //return Tools.getInfoItem(infoItems, index, productCode);
            //        return getInfoItem(infoItems, index, productCode);
            //    }
            //    protected infoItem GetInfoItem(int itemIndex)
            //    {
            //        if (infoItems != null && infoItems.Length > itemIndex)
            //            return infoItems[itemIndex];
            //        else
            //            return null;
            //    }
            //}

            public abstract class infoObjectAbs
            {
                public virtual MsgResponse.responseInfo GetResponseInfo()
                {
                    return null;
                }
            }

            /// <summary>
            /// For items which don't have Number of Item
            /// </summary>
            public abstract class infoTypeOneLayer : infoObjectAbs
            {
                infoItem theInfoItem;
                public infoItem InfoItem
                {
                    get { return theInfoItem; }
                }

                public infoTypeOneLayer(String scrStr, int itemLen)
                {
                    theInfoItem = null;
                    if (scrStr != null && scrStr.Length > 0)
                    {
                        String[] allItem = scrStr.Split(',');
                        if (allItem != null && allItem.Length == itemLen)
                        {
                            theInfoItem = new infoItem(allItem, 0, itemLen);
                        }
                    }
                }

                enum resInfoEnum
                {
                    datetime,
                    userID,
                    accNo,
                    rInfo,
                    Total
                }

                public override MsgResponse.responseInfo GetResponseInfo()
                {
                    if (theInfoItem != null && theInfoItem.AllItem != null && theInfoItem.AllItem.Length >= (int)resInfoEnum.Total)
                        return new MsgResponse.responseInfo(theInfoItem.AllItem[(int)resInfoEnum.datetime], theInfoItem.AllItem[(int)resInfoEnum.userID], theInfoItem.AllItem[(int)resInfoEnum.accNo], theInfoItem.AllItem[(int)resInfoEnum.rInfo]);
                    return base.GetResponseInfo();
                }
            }

            public abstract class infoTypeSubItem : infoObjectAbs
            {
                DateTime datetime;
                String userID;
                String accNo;   //p1.1
                String rInfo;   //p1.1
                infoItem[] infoItems;

                public DateTime Datetime
                {
                    get { return datetime; }
                }
                public String UserID
                {
                    get { return userID; }
                }
                public String AccNo
                {
                    get { return accNo; }
                }
                public String RInfo
                {
                    get { return rInfo; }
                }

                // add by ben
                public infoItem[] InfoItems
                {
                    get { return infoItems; }
                }

                enum offsetEnum
                {
                    datetime,
                    userID,
                    accNo,  //p1.1
                    rInfo,  //p1.1
                    numOfItem,
                    itemStart
                }

                public infoTypeSubItem(String scrStr, int offsetNumOfSubItem, int subItemLen)
                {
                    datetime = DateTime.MinValue;
                    userID = null;
                    accNo = null; //p1.1
                    rInfo = null; //p1.1
                    infoItems = null;
                    try
                    {
                        if (scrStr != null && scrStr.Length > 0)
                        {
                            String[] allItem = scrStr.Split(',');
                            if (allItem != null && allItem.Length > (int)offsetEnum.numOfItem)
                            {
                                datetime = Tools.getDateTimeFromUnixTime(allItem[(int)offsetEnum.datetime]);
                                userID = allItem[(int)offsetEnum.userID];
                                accNo = allItem[(int)offsetEnum.accNo]; //p1.1
                                rInfo = allItem[(int)offsetEnum.rInfo]; //p1.1
                                int numOfItem = Convert.ToInt32(allItem[(int)offsetEnum.numOfItem]);
                                //if (allItem.Length == numOfItem * itemLen + (int)offsetEnum.itemStart)
                                {
                                    int startIndex = (int)offsetEnum.itemStart;
                                    infoItems = new infoItem[numOfItem];
                                    for (int index = 0; startIndex < allItem.Length; index++)
                                    {
                                        int numOfSubItem = Convert.ToInt32(allItem[startIndex + offsetNumOfSubItem]);
                                        int itemLen = numOfSubItem * subItemLen + offsetNumOfSubItem + 1;
                                        infoItems[index] = new infoItem(allItem, startIndex, itemLen);
                                        startIndex += itemLen;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        infoItems = null;
                    }
                }

                protected List<String> GetElementList(int index)
                {
                    //return Tools.getElementList(infoItems, index);
                    return getElementList(infoItems, index);
                }

                protected infoItem GetInfoItem(int index, String productCode)
                {
                    //return Tools.getInfoItem(infoItems, index, productCode);
                    return getInfoItem(infoItems, index, productCode);
                }

                protected infoItem GetInfoItem(int itemIndex)
                {
                    if (infoItems != null && infoItems.Length > itemIndex)
                        return infoItems[itemIndex];
                    else
                        return null;
                }

                public override MsgResponse.responseInfo GetResponseInfo()
                {
                    return new MsgResponse.responseInfo(datetime, userID, accNo, rInfo);
                }
            }

            #endregion

            #region AccountManager
            public class LoginResponse : infoTypeOneLayer
            {

                public class LoginResponseItem : infoItem
                {
                    public enum loginResponseItemEnum
                    {
                        datetime,
                        userID,
                        accNo,  //p1.1
                        rInfo,  //p1.1
                        result,
                        errorMessage,
                        disclaimerText,
                        TOTAL
                    }

                    public LoginResponseItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)loginResponseItemEnum.TOTAL) { }

                    public LoginResponseItem(infoItem srcInfoItem) : base(srcInfoItem) { }

                }

                public LoginResponse(String scrStr) : base(scrStr, (int)LoginResponseItem.loginResponseItemEnum.TOTAL) { }

                //add by ben06252013
                public LoginResp GetLoginResponse(infoItem list)
                {
                    if (list == null) return null;
                    LoginResp data = new LoginResp();
                    data.DateTime = getDateTimeFromUnixTime(list.AllItem[0].ToString());
                    data.UserID = list.AllItem[1];
                    data.AccNo = list.AllItem[2];
                    data.RInfo = list.AllItem[3];
                    data.Result = (list.AllItem[4] == "0") ? true : false;
                    if (data.Result)
                    {
                        data.SessionHash = list.AllItem[5];
                    }
                    else
                    {
                        data.ErrorMsg = (list.AllItem[3].IndexOf("mg=b64") > -1) ? TradeStationTools.Base64StringToString(list.AllItem[5]) : list.AllItem[5];  //list.AllItem[5];
                    }
                    data.DisText = (list.AllItem[3].IndexOf("mg=b64") > -1) ? TradeStationTools.Base64StringToString(list.AllItem[6], "big5").TrimEnd(' ') : list.AllItem[6]; //list.AllItem[6];
                    return data;
                }
            }

            public class AccountInfo : infoTypeOneLayer
            {
                public class AccountInfoItem : infoItem
                {
                    public enum accountInfoItemEnum
                    {
                        datetime,
                        userID,
                        accNo,  //p1.1
                        rInfo,  //p1.1
                        buyingPower,
                        nav,
                        marginCall,
                        commodityPL,
                        iMargin,
                        mMargin,
                        mLevel,
                        period,
                        cashBalance,
                        creditLimit,
                        maxMargin,
                        ctrlLevel,
                        mgnClass,
                        ae,
                        TOTAL
                    }

                    public AccountInfoItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)accountInfoItemEnum.TOTAL) { }

                    public AccountInfoItem(infoItem srcInfoItem) : base(srcInfoItem) { }

                    public DateTime getTime()
                    {
                        DateTime retTime = DateTime.MinValue;
                        if (allItem != null && allItem.Length == (int)accountInfoItemEnum.TOTAL)
                        {
                            retTime = Tools.getDateTimeFromUnixTime(allItem[(int)accountInfoItemEnum.datetime]);
                        }
                        return retTime;
                    }
                }

                public AccountInfo(String scrStr) : base(scrStr, (int)AccountInfoItem.accountInfoItemEnum.TOTAL) { }

                //GetAccountInfoItemTable
                /// <summary>
                /// coco
                /// </summary>
                /// <param name="list"></param>
                /// <returns></returns>
                public DataTable GetAccountInfoItemTable(infoItem list)
                {
                    String[] accountdata = list.AllItem;
                    DataTable dt_accountInfo = new DataTable();

                    if (accountdata.Length >= 18)
                    {
                        for (int i = 0; i < accountdata.Length; i++)
                        {
                            if (accountdata[i] == "")
                            {
                                accountdata[i] = "-";
                            }
                        }


                        dt_accountInfo.Columns.Add(new DataColumn("name", typeof(string)));
                        dt_accountInfo.Columns.Add(new DataColumn("value", typeof(string)));

                        DataRow bp = dt_accountInfo.NewRow();
                        bp["name"] = "Buying Power";
                        bp["value"] = accountdata[4].ToString() + " HKD";
                        dt_accountInfo.Rows.Add(bp);

                        //NAV;
                        DataRow nav = dt_accountInfo.NewRow();
                        nav["name"] = "NAV";
                        nav["value"] = accountdata[5].ToString() + " HKD";
                        dt_accountInfo.Rows.Add(nav);

                        //Margin Call;
                        DataRow mc = dt_accountInfo.NewRow();
                        mc["name"] = "Margin Call";
                        mc["value"] = accountdata[6].ToString() + " HKD";
                        dt_accountInfo.Rows.Add(mc);

                        //Commodity P/L;
                        DataRow cpl = dt_accountInfo.NewRow();
                        cpl["name"] = "Commodity P/L";
                        cpl["value"] = accountdata[7].ToString() + " HKD";
                        dt_accountInfo.Rows.Add(cpl);

                        //I.Margin;
                        DataRow im = dt_accountInfo.NewRow();
                        im["name"] = "I.Margin";
                        im["value"] = accountdata[8].ToString() + " HKD";
                        dt_accountInfo.Rows.Add(im);

                        //M.Margin;
                        DataRow mm = dt_accountInfo.NewRow();
                        mm["name"] = "M.Margin";
                        mm["value"] = accountdata[9].ToString() + " HKD";
                        dt_accountInfo.Rows.Add(mm);

                        //M.Level;
                        DataRow ml = dt_accountInfo.NewRow();
                        ml["name"] = "M.Level";
                        ml["value"] = accountdata[10].ToString();
                        dt_accountInfo.Rows.Add(ml);

                        //period;
                        DataRow period = dt_accountInfo.NewRow();
                        period["name"] = "Period";
                        period["value"] = accountdata[11].ToString();
                        dt_accountInfo.Rows.Add(period);

                        //Cash Balance;
                        DataRow cb = dt_accountInfo.NewRow();
                        cb["name"] = "Cash Balance";
                        cb["value"] = accountdata[12].ToString() + " HKD";
                        dt_accountInfo.Rows.Add(cb);


                        //credit Limit;
                        DataRow cl = dt_accountInfo.NewRow();
                        cl["name"] = "Credit Limit";
                        cl["value"] = accountdata[13].ToString() + " HKD";
                        dt_accountInfo.Rows.Add(cl);

                        //max margin;
                        DataRow max = dt_accountInfo.NewRow();
                        max["name"] = "Max Margin";
                        max["value"] = accountdata[14].ToString();
                        dt_accountInfo.Rows.Add(max);

                        //ctrl level;
                        DataRow ctrl = dt_accountInfo.NewRow();
                        ctrl["name"] = "Ctrl Level";
                        ctrl["value"] = accountdata[15].ToString();
                        dt_accountInfo.Rows.Add(ctrl);

                        //mgn class;
                        DataRow mgn = dt_accountInfo.NewRow();
                        mgn["name"] = "Mgn Class";
                        mgn["value"] = accountdata[16].ToString();
                        dt_accountInfo.Rows.Add(mgn);

                        //AE;
                        DataRow AE = dt_accountInfo.NewRow();
                        AE["name"] = "AE";
                        AE["value"] = accountdata[17].ToString();
                        dt_accountInfo.Rows.Add(AE);


                    }
                    return dt_accountInfo;
                }



            }

            public class OrderBook : infoType
            {
                public class OrderBookItem : infoItem
                {
                    public enum orderbookItemEnum
                    {
                        internalOrderNo,
                        productCode,
                        productName,
                        osBQty,
                        osSQty,
                        price,
                        valid,
                        cond,
                        status,
                        traded,
                        initiator,
                        refNo,
                        tStamp,
                        externalOrderNo,
                        tOne,
                        specTime,
                        TOTAL
                    }

                    public OrderBookItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)orderbookItemEnum.TOTAL) { }

                    public OrderBookItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public OrderBook(String scrStr) : base(scrStr, (int)OrderBookItem.orderbookItemEnum.TOTAL) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public OrderBookItem GetOrderBookItem(int elementIndex)
                {
                    return new OrderBookItem(GetInfoItem(elementIndex));
                }

                /// <summary>
                /// get the first found item with given productCode
                /// </summary>
                /// <param name="productCode"></param>
                /// <returns></returns>
                public OrderBookItem GetOrderBookItem(String productCode)
                {
                    return new OrderBookItem(GetInfoItem((int)OrderBookItem.orderbookItemEnum.productCode, productCode));
                }

                ///coco
                ///
                public DataTable GetOrderBookItemTable(infoItem[] list)
                {
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(OrderBookItem.orderbookItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        //harry 2014-10-27
                        if (s == "tStamp")
                        {                          
                            dataColumn.DataType = typeof(DateTime);                          
                        }
                        else
                        {
                            dataColumn.DataType = typeof(string);
                        }
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            if (i == (int)OrderBookItem.orderbookItemEnum.tStamp)
                                dr[i] = Tools.getDateTimeFromUnixTime(r.AllItem[i]);
                            else
                                dr[i] = r.AllItem[i];
                        }
                        tempTable.Rows.Add(dr);
                    }
                    return tempTable;

                }
            }

            public class Position : infoType
            {
                public class PositionItem : infoItem
                {
                    public enum positionItemEnum
                    {
                        productCode,
                        productName,
                        prev,
                        inOut,
                        dayLong,
                        dayShort,
                        dayNet,
                        net,
                        mktPrice,
                        pl,
                        refFxRate,
                        plBaseCcy,
                        contract,
                        netAverage,
                        Prevaverage,
                        InOutaverage,
                        Daylongaverage,
                        Dayshortaverage,
                        Daynetaverage,
                        TOTAL
                    }

                    public PositionItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)positionItemEnum.TOTAL) { }

                    public PositionItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public Position(String scrStr) : base(scrStr, (int)PositionItem.positionItemEnum.TOTAL) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public PositionItem GetPositionItem(int elementIndex)
                {
                    return new PositionItem(GetInfoItem(elementIndex));
                }

                /// <summary>
                /// get the first found item with given productCode
                /// </summary>
                /// <param name="productCode"></param>
                /// <returns></returns>
                public PositionItem GetPositionItem(String productCode)
                {
                    return new PositionItem(GetInfoItem((int)PositionItem.positionItemEnum.productCode, productCode));
                }

                ///coco
                ///
                public DataTable GetPositionItemTable(infoItem[] list)
                {
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(PositionItem.positionItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataColumn.DataType = typeof(string);
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            dr[i] = r.AllItem[i];
                        }
                        tempTable.Rows.Add(dr);
                    }
                    return tempTable;
                }

            }

            public class PositionSummary : infoType
            {
                public class PositionSummaryItem : infoItem
                {
                    public enum positionSummaryItemEnum
                    {
                        productCode,
                        prev,
                        dayLong,
                        dayShort,
                        net,
                        TOTAL
                    }

                    public PositionSummaryItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)positionSummaryItemEnum.TOTAL) { }

                    public PositionSummaryItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public PositionSummary(String scrStr) : base(scrStr, (int)PositionSummaryItem.positionSummaryItemEnum.TOTAL) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public PositionSummaryItem GetPositionSummaryItem(int elementIndex)
                {
                    return new PositionSummaryItem(GetInfoItem(elementIndex));
                }

                /// <summary>
                /// get the first found item with given productCode
                /// </summary>
                /// <param name="productCode"></param>
                /// <returns></returns>
                public PositionSummaryItem GetPositionSummaryItem(String productCode)
                {
                    return new PositionSummaryItem(GetInfoItem((int)PositionSummaryItem.positionSummaryItemEnum.productCode, productCode));
                }

                /// <summary>
                /// coco
                /// </summary>
                /// <param name="list"></param>
                /// <returns></returns>

                public DataTable GetPositionSummaryItemTable(infoItem[] list)
                {
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(PositionSummaryItem.positionSummaryItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataColumn.DataType = typeof(string);
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            dr[i] = r.AllItem[i];
                        }
                        tempTable.Rows.Add(dr);
                    }
                    return tempTable;

                }
            }

            public class CashInfo : infoType
            {
                public class CashInfoItem : infoItem
                {
                    public enum cashInfoItemEnum
                    {
                        ccy,
                        cashBf,
                        unsettle,
                        todayInOut,
                        withdrawlReq,
                        cash,
                        unpresented,
                        refFxRate,
                        cashBaseCcy,
                        TOTAL
                    }

                    public CashInfoItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)cashInfoItemEnum.TOTAL) { }

                    public CashInfoItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public CashInfo(String scrStr) : base(scrStr, (int)CashInfoItem.cashInfoItemEnum.TOTAL) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public CashInfoItem GetCashInfoItemItem(int elementIndex)
                {
                    return new CashInfoItem(GetInfoItem(elementIndex));
                }

                //GetCashInfoItemTable
                /// <summary>
                /// coco
                /// </summary>
                /// <param name="list"></param>
                /// <returns></returns>
                public DataTable GetCashInfoItemTable(infoItem[] list)
                {
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(CashInfoItem.cashInfoItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataColumn.DataType = typeof(string);
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            dr[i] = r.AllItem[i];
                        }
                        tempTable.Rows.Add(dr);
                    }
                    DataTable dtCashInfo = new DataTable();
                    dtCashInfo.Columns.Add(new DataColumn("name", typeof(string)));
                    dtCashInfo.Columns.Add(new DataColumn("value", typeof(string)));

                    for (int i = 0; i < tempTable.Rows.Count; i++)
                    {
                        DataRow ccy = dtCashInfo.NewRow();
                        ccy["name"] = "Ccy";
                        ccy["value"] = tempTable.Rows[i][0].ToString();
                        dtCashInfo.Rows.Add(ccy);

                        DataRow CashBf = dtCashInfo.NewRow();
                        CashBf["name"] = "Cash Bf.";
                        CashBf["value"] = tempTable.Rows[i][1].ToString();
                        dtCashInfo.Rows.Add(CashBf);

                        DataRow Unsettle = dtCashInfo.NewRow();
                        Unsettle["name"] = "Unsettle";
                        Unsettle["value"] = tempTable.Rows[i][2].ToString();
                        dtCashInfo.Rows.Add(Unsettle);

                        DataRow todayInOut = dtCashInfo.NewRow();
                        todayInOut["name"] = "Today In/Out";
                        todayInOut["value"] = tempTable.Rows[i][3].ToString();
                        dtCashInfo.Rows.Add(todayInOut);

                        DataRow WithdrawalReq = dtCashInfo.NewRow();
                        WithdrawalReq["name"] = "Withdrawal Req";
                        WithdrawalReq["value"] = tempTable.Rows[i][4].ToString();
                        dtCashInfo.Rows.Add(WithdrawalReq);

                        DataRow Cash = dtCashInfo.NewRow();
                        Cash["name"] = "Cash";
                        Cash["value"] = tempTable.Rows[i][5].ToString();
                        dtCashInfo.Rows.Add(Cash);

                        DataRow Unpresented = dtCashInfo.NewRow();
                        Unpresented["name"] = "Unpresented";
                        Unpresented["value"] = tempTable.Rows[i][6].ToString();
                        dtCashInfo.Rows.Add(Unpresented);

                        DataRow reffxrate = dtCashInfo.NewRow();
                        reffxrate["name"] = "Ref.Fx Rate ";
                        reffxrate["value"] = tempTable.Rows[i][7].ToString();
                        dtCashInfo.Rows.Add(reffxrate);

                        DataRow CashBaseCcy = dtCashInfo.NewRow();
                        CashBaseCcy["name"] = "Cash(Base Ccy)";
                        CashBaseCcy["value"] = tempTable.Rows[i][8].ToString();
                        dtCashInfo.Rows.Add(CashBaseCcy);

                    }

                    return dtCashInfo;

                }

            }

            public class DoneTrade : infoType
            {
                public class DoneTradeItem : infoItem
                {
                    public enum doneTradeItemEnum
                    {
                        productCode,
                        bQty,
                        sQty,
                        price,
                        time,
                        internalOrderNo,
                        externalOrderNo,
                        TOTAL
                    }

                    public DoneTradeItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)doneTradeItemEnum.TOTAL) { }

                    public DoneTradeItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public DoneTrade(String scrStr) : base(scrStr, (int)DoneTradeItem.doneTradeItemEnum.TOTAL) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public DoneTradeItem GetDoneTradeItem(int elementIndex)
                {
                    return new DoneTradeItem(GetInfoItem(elementIndex));
                }

                /// <summary>
                /// get the first found item with given productCode
                /// </summary>
                /// <param name="productCode"></param>
                /// <returns></returns>
                public DoneTradeItem GetDoneTradeItem(String productCode)
                {
                    return new DoneTradeItem(GetInfoItem((int)DoneTradeItem.doneTradeItemEnum.productCode, productCode));
                }

                public DataTable GetDoneTradeInfoItemTable(infoItem[] list)
                {
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(DoneTradeItem.doneTradeItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataColumn.DataType = typeof(string);
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            if (i == (int)DoneTradeItem.doneTradeItemEnum.time)
                                dr[i] = Tools.getDateTimeFromUnixTime(r.AllItem[i]);
                            else
                                dr[i] = r.AllItem[i];
                        }
                        tempTable.Rows.Add(dr);
                    }
                    return tempTable;
                }
            }

            public class ClearTrade : infoType
            {
                public class ClearTradeItem : infoItem
                {
                    public enum clearTradeItemEnum
                    {
                        productCode,
                        productName,
                        bQty,
                        sQty,
                        tradePrice,
                        internalTradeNo,
                        status,
                        initiator,
                        refNo,
                        tradeTime,
                        orderPrice,
                        internalOrderNo,
                        externalOrderNo,
                        TOTAL
                    }

                    public ClearTradeItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)clearTradeItemEnum.TOTAL) { }

                    public ClearTradeItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public ClearTrade(String scrStr) : base(scrStr, (int)ClearTradeItem.clearTradeItemEnum.TOTAL) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public ClearTradeItem GetClearTradeItem(int elementIndex)
                {
                    return new ClearTradeItem(GetInfoItem(elementIndex));
                }

                /// <summary>
                /// get the first found item with given productCode
                /// </summary>
                /// <param name="productCode"></param>
                /// <returns></returns>
                public ClearTradeItem GetClearTradeItem(String productCode)
                {
                    return new ClearTradeItem(GetInfoItem((int)ClearTradeItem.clearTradeItemEnum.productCode, productCode));
                }

                //GetClearTradeInfoItemTable
                /// <summary>
                /// coco
                /// </summary>
                /// <param name="list"></param>
                /// <returns></returns>
                public DataTable GetClearTradeInfoItemTable(infoItem[] list)
                {
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(ClearTradeItem.clearTradeItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        //harry 2014-10-27
                        if (s == "tradeTime")
                        {
                            dataColumn.DataType = typeof(DateTime);
                        }
                        else
                        {
                            dataColumn.DataType = typeof(string);
                        }
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            if (i == (int)ClearTradeItem.clearTradeItemEnum.tradeTime)
                                dr[i] = Tools.getStringFromUnixTime(r.AllItem[i]);
                            else
                                dr[i] = r.AllItem[i];
                        }
                        tempTable.Rows.Add(dr);
                    }
                    return tempTable;
                }
            }

            public class AccList : infoType
            {
                public class AccListItem : infoItem
                {
                    public enum accListItemEnum
                    {
                        userID,
                        acNo,
                        aeID,
                        reserved,
                        TOTAL
                    }

                    public AccListItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)accListItemEnum.TOTAL) { }

                    public AccListItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public AccList(String scrStr) : base(scrStr, (int)AccListItem.accListItemEnum.TOTAL) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public AccListItem GetAccListItem(int elementIndex)
                {
                    return new AccListItem(GetInfoItem(elementIndex));
                }

                /// <summary>
                /// get the first found item with given userID
                /// </summary>
                /// <param name="userID"></param>
                /// <returns></returns>
                public AccListItem GetAccListItem(String userID)
                {
                    return new AccListItem(GetInfoItem((int)AccListItem.accListItemEnum.userID, userID));
                }


                /// <summary>
                /// Kenlo
                /// </summary>
                /// <param name="list"></param>
                /// <returns></returns>
                public DataTable GetAccListItemTable(infoItem[] list)
                {
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(AccListItem.accListItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataColumn.DataType = typeof(string);
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            dr[i] = r.AllItem[i];
                        }
                        tempTable.Rows.Add(dr);
                    }
                    return tempTable;
                }
            }

            public class MarginCheck : infoType
            {
                public class MarginCheckItem : infoItem
                {
                    //public enum marginCheckItemEnum
                    //{
                    //    AE,
                    //    ccy,
                    //    call,
                    //    creadit,
                    //    IMargin,
                    //    MMargin,
                    //    Cash,
                    //    LoanLimit,
                    //    NetEq,
                    //    PL,
                    //    Fee,
                    //    TotalEq,
                    //    MarketValue,
                    //    MaxMgn,
                    //    Pos,
                    //    Orders,
                    //    updateTime,
                    //    accNo,
                    //    TOTAL
                    //}
                    public enum marginCheckItemEnum
                    {
                        AE,
                        ccy,
                        call,
                        creadit,
                        IMargin,
                        MMargin,
                        Cash,
                        LoanLimit,
                        NetEq,
                        PL,
                        Fee,
                        TotalEq,
                        MarketValue,
                        MaxMgn,
                        Pos,
                        Orders,
                        updateTime,
                        accNo,
                        mLevel,
                        rawMarginLevel,
                        buyPower,
                        tradeLimit,
                        marginClass,
                        tradeClass,
                        nav,
                        unpresented,
                        avFund,
                        ctrlLevel,
                        accName,
                        TOTAL
                    }

                    public MarginCheckItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)marginCheckItemEnum.TOTAL) { }

                    public MarginCheckItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public MarginCheck(String scrStr) : base(scrStr, (int)MarginCheckItem.marginCheckItemEnum.TOTAL) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public MarginCheckItem GetMarginCheckItem(int elementIndex)
                {
                    return new MarginCheckItem(GetInfoItem(elementIndex));
                }

                /// <summary>
                /// get the first found item with given userID
                /// </summary>
                /// <param name="userID"></param>
                /// <returns></returns>
                public MarginCheckItem GetMarginCheckItem(String AE)
                {
                    return new MarginCheckItem(GetInfoItem((int)MarginCheckItem.marginCheckItemEnum.AE, AE));
                }


                /// <summary>
                /// Kenlo
                /// </summary>
                /// <param name="list"></param>
                /// <returns></returns>
                public DataTable GetMarginCheckItemItemTable(infoItem[] list)
                {
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(MarginCheckItem.marginCheckItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataColumn.DataType = typeof(string);
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            dr[i] = r.AllItem[i];
                        }
                        tempTable.Rows.Add(dr);
                    }
                    return tempTable;
                }

                /// <summary>
                /// Ben
                /// </summary>
                /// <param name="list"></param>
                /// <returns>ObservableCollection></returns>
                public ObservableCollection<MarginCheckAccData> GetMarginCheckItemData(infoItem[] list)
                {
                    if (list == null) return null;
                    ObservableCollection<MarginCheckAccData> data = new ObservableCollection<MarginCheckAccData>();

                    MarginCheckAccData item;
                    foreach (infoItem ls in list)
                    {
                        item = new MarginCheckAccData();
                        item.AE = ls.AllItem[0];
                        item.Ccy = ls.AllItem[1];
                        item.Call = (ls.AllItem[2] == "") ? 0 : Convert.ToDouble(ls.AllItem[2]);
                        item.Creadit = ls.AllItem[3];
                        item.IMargin = (ls.AllItem[4] == "") ? 0 : Convert.ToDouble(ls.AllItem[4]);
                        item.MMargin = (ls.AllItem[5] == "") ? 0 : Convert.ToDouble(ls.AllItem[5]);
                        item.Cash = (ls.AllItem[6] == "") ? 0 : Convert.ToDouble(ls.AllItem[6]);
                        item.LoanLimit = ls.AllItem[7];
                        item.NetEq = ls.AllItem[8];
                        item.PL = (ls.AllItem[9] == "") ? 0 : Convert.ToDouble(ls.AllItem[9]);
                        item.Fee = ls.AllItem[10];
                        item.TotalEq = ls.AllItem[11];
                        item.MarketValue = ls.AllItem[12];
                        item.MaxMgn = ls.AllItem[13];
                        item.Pos = ls.AllItem[14];
                        item.Orders = ls.AllItem[15];
                        item.UpdateTime = Convert.ToDateTime(Tools.getDateTimeFromUnixTime(ls.AllItem[16]));
                        item.AccNo = ls.AllItem[17];
                        item.MLevel = (ls.AllItem[18] == "") ? 0 : Convert.ToDouble(ls.AllItem[18]);
                        item.RawMarginLevel = ls.AllItem[19];
                        item.BuyPower = (ls.AllItem[20] == "") ? 0 : Convert.ToDouble(ls.AllItem[20]);
                        item.TradeLimit = (ls.AllItem[21] == "") ? 0 : Convert.ToDouble(ls.AllItem[21]);
                        item.MarginClass = ls.AllItem[22];
                        item.TradeClass = ls.AllItem[23];
                        item.Nav = (ls.AllItem[24] == "") ? 0 : Convert.ToDouble(ls.AllItem[24]);
                        item.Unpresented = (ls.AllItem[25] == "") ? 0 : Convert.ToDouble(ls.AllItem[25]);
                        item.AvFund = (ls.AllItem[26] == "") ? 0 : Convert.ToDouble(ls.AllItem[26]);
                        item.CtrlLevel = ls.AllItem[27];
                        item.AccName = TradeStationTools.Base64Utf16StringToString(ls.AllItem[28]);
                        data.Add(item);
                    }
                    return data;
                }
            }

            public class MarginCallList : infoType
            {
                public class MarginCallListItem : infoItem
                {
                    public enum marginCallListItemEnum
                    {
                        No,
                        acc,
                        MarginLevel,
                        RawMarginLevel,
                        TOTAL
                    }

                    public MarginCallListItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)marginCallListItemEnum.TOTAL) { }

                    public MarginCallListItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public MarginCallList(String scrStr) : base(scrStr, (int)MarginCallListItem.marginCallListItemEnum.TOTAL, 1) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public MarginCallListItem GetMarginCallListItem(int elementIndex)
                {
                    return new MarginCallListItem(GetInfoItem(elementIndex));
                }

                /// <summary>
                /// get the first found item with given userID
                /// </summary>
                /// <param name="userID"></param>
                /// <returns></returns>
                public MarginCallListItem GetMarginCallListItem(String acc)
                {
                    return new MarginCallListItem(GetInfoItem((int)MarginCallListItem.marginCallListItemEnum.acc, acc));
                }

                /// <summary>
                /// Kenlo
                /// </summary>
                /// <param name="list"></param>
                /// <returns></returns>
                public DataTable GetMarginCallListItemItemTable(infoItem[] list)
                {
                    if (list == null) return null;
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(MarginCallListItem.marginCallListItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataColumn.DataType = typeof(string);
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            dr[i] = r.AllItem[i];
                        }
                        tempTable.Rows.Add(dr);
                    }
                    return tempTable;
                }

                /// <summary>
                /// Ben
                /// </summary>
                /// <param name="list"></param>
                /// <returns>ObservableCollection</returns>
                //public ObservableCollection<MarginCallData> GetMarginCallListItemData(infoItem[] list)
                //{
                //    ObservableCollection<MarginCallData> data = new ObservableCollection<MarginCallData>();

                //    MarginCallData item;
                //    foreach (infoItem ls in list)
                //    {
                //        item = new MarginCallData();
                //        item.No = ls.AllItem[0];
                //        item.Acc = ls.AllItem[1];
                //        item.MarginLevel = ls.AllItem[2];
                //        item.RawMarginLevel = ls.AllItem[3];

                //        data.Add(item);
                //    }
                //    return data;
                //}

            }

            public class TradeConfOrder : infoType
            {
                public class TradeConfOrderItem : infoItem
                {
                    public enum tradeConfOrderItemEnum
                    {
                        accNo,
                        internalOrderNo,
                        productCode,
                        osBQty,
                        osSQty,
                        price,
                        valid,
                        cond,
                        status,
                        traded,
                        initiator,
                        refNo,
                        tStamp,
                        extOrderNo,
                        initiatorChannel,
                        tOne,
                        TOTAL
                    }

                    public TradeConfOrderItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)tradeConfOrderItemEnum.TOTAL) { }

                    public TradeConfOrderItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public TradeConfOrder(String scrStr) : base(scrStr, (int)TradeConfOrderItem.tradeConfOrderItemEnum.TOTAL) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public TradeConfOrderItem GetTradeConfOrderItem(int elementIndex)
                {
                    return new TradeConfOrderItem(GetInfoItem(elementIndex));
                }


                /// <summary>
                /// Kenlo
                /// </summary>
                /// <param name="list"></param>
                /// <returns></returns>
                public DataTable GetTradeConfOrderItemTable(infoItem[] list)
                {
                    if (list == null) return null;
                    DataTable tempTable = new DataTable();
                    tempTable.TableName = "TradeConfOrder";
                    foreach (string s in Enum.GetNames(typeof(TradeConfOrderItem.tradeConfOrderItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        if (s == "tStamp")
                        {
                            dataColumn.DataType = typeof(DateTime);
                        }
                        else
                        {
                            dataColumn.DataType = typeof(string);
                        }
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    tempTable.Columns.Add(new DataColumn("DepInPrice", typeof(int)));
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            if (i == (int)TradeConfOrderItem.tradeConfOrderItemEnum.tStamp)
                            {
                                dr[i] = Tools.getDateTimeFromUnixTime(r.AllItem[i]);
                            }
                            //else if (i == (int)TradeConfOrderItem.tradeConfOrderItemEnum.price)
                            //{
                            //    int depInPrice = TradeStationTools.ConvertToInt(MarketPriceData.GetDecInPrice(r.AllItem[2]), -1);
                            //    dr[i] = TradeStationTools.ConvertToFormatString(r.AllItem[i], depInPrice);
                            //}
                            else
                            {
                                dr[i] = r.AllItem[i];
                            }
                        }
                        dr[tempTable.Columns.Count - 1] = MarketPriceData.GetDecInPrice(dr["productCode"].ToString()); ;
                        tempTable.Rows.Add(dr);
                    }
                    return tempTable;
                }
            }

            public class TradeConfTrade : infoType
            {
                public class TradeConfTradeItem : infoItem
                {
                    public enum tradeConfTradeItemEnum
                    {
                        accNo,
                        orderNo,
                        productCode,
                        osBQty,
                        osSQty,
                        tradedPrice,
                        status,
                        initiator,
                        refNo,
                        tStamp,
                        extOrderNo,
                        tradeNo,
                        recNo,
                        channel,
                        TOTAL
                    }

                    public TradeConfTradeItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)tradeConfTradeItemEnum.TOTAL) { }

                    public TradeConfTradeItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public TradeConfTrade(String scrStr) : base(scrStr, (int)TradeConfTradeItem.tradeConfTradeItemEnum.TOTAL) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public TradeConfTradeItem GetTradeConfTradeItem(int elementIndex)
                {
                    return new TradeConfTradeItem(GetInfoItem(elementIndex));
                }


                /// <summary>
                /// Kenlo
                /// </summary>
                /// <param name="list"></param>
                /// <returns></returns>
                public DataTable GetTradeConfTradeItemTable(infoItem[] list)
                {
                    if (list == null) return null;
                    DataTable tempTable = new DataTable();
                    tempTable.TableName = "TradeConfTrade";
                    foreach (string s in Enum.GetNames(typeof(TradeConfTradeItem.tradeConfTradeItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        if (s == "tStamp")
                        {
                            dataColumn.DataType = typeof(DateTime);
                        }
                        else
                        {
                            dataColumn.DataType = typeof(string);
                        }
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    tempTable.Columns.Add(new DataColumn("DepInPrice", typeof(int)));
                    // tempTable.PrimaryKey = new DataColumn[] { tempTable.Columns["accNo"], tempTable.Columns["orderNo"]}; 
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            if (i == (int)TradeConfTradeItem.tradeConfTradeItemEnum.tStamp)
                            {
                                dr[i] = Tools.getDateTimeFromUnixTime(r.AllItem[i]);

                            }
                            //else if (i == (int)TradeConfTradeItem.tradeConfTradeItemEnum.tradedPrice)
                            //{
                            //    int depInPrice = TradeStationTools.ConvertToInt(MarketPriceData.GetDecInPrice(r.AllItem[2]), -1);
                            //    dr[i] = TradeStationTools.ConvertToFormatString(r.AllItem[i], depInPrice);
                            //}
                            else
                            {
                                dr[i] = r.AllItem[i];
                            }
                        }
                        dr[tempTable.Columns.Count - 1] = MarketPriceData.GetDecInPrice(dr["productCode"].ToString()); ;
                        tempTable.Rows.Add(dr);
                    }
                    return tempTable;
                }
            }

            public class ReportTradeConfAck : infoTypeOneLayer
            {
                public class ReportTradeConfAckItem : infoItem
                {
                    public enum reportTradeConfAckItemEnum
                    {
                        datetime,
                        userID,
                        accNo,  //p1.1
                        rInfo,  //p1.1
                        orderNo,
                        tradeNo,
                        recNo,
                        TOTAL
                    }

                    public ReportTradeConfAckItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)reportTradeConfAckItemEnum.TOTAL) { }

                    public ReportTradeConfAckItem(infoItem srcInfoItem) : base(srcInfoItem) { }

                }

                public ReportTradeConfAck(String scrStr) : base(scrStr, (int)ReportTradeConfAckItem.reportTradeConfAckItemEnum.TOTAL) { }

                public String GetReportTradeConfAckItem(infoItem list)
                {
                    String[] TradeConfAckInfo = list.AllItem;
                    string str = null;
                    for (int i = 0; i < TradeConfAckInfo.Length; i++)
                    {
                        str += TradeConfAckInfo[i] + ",";
                    }
                    return str;
                }

            }

            public class AccountMaster : infoType
            {
                public class AccountMasterItem : infoItem
                {
                    public enum accountMasterItemEnum
                    {
                        accNo,
                        accName,
                        idBrNo,
                        aeID,
                        sex,
                        home,
                        office,
                        mobile,
                        marginClass,
                        tradeClass,
                        contactInfo,
                        address,
                        emailAddress,
                        baseCurrency,
                        maxMargin,
                        creditLimit,
                        active,
                        TOTAL
                    }

                    public AccountMasterItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)accountMasterItemEnum.TOTAL) { }

                    public AccountMasterItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public AccountMaster(String scrStr) : base(scrStr, (int)AccountMasterItem.accountMasterItemEnum.TOTAL) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public AccountMasterItem GetAccountMasterItem(int elementIndex)
                {
                    return new AccountMasterItem(GetInfoItem(elementIndex));
                }


                /// <summary>
                /// Kenlo
                /// </summary>
                /// <param name="list"></param>
                /// <returns></returns>
                public DataTable GetAccountMasterItemTable(infoItem[] list)
                {
                    if (list == null) return null;
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(AccountMasterItem.accountMasterItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataColumn.DataType = typeof(string);
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            dr[i] = r.AllItem[i];
                        }
                        tempTable.Rows.Add(dr);
                    }
                    return tempTable;
                }
            }

            public class TradeConfOrderDetail : infoType
            {
                public class TradeConfOrderDetailItem : infoItem
                {
                    public enum tradeConfOrderDetailEnum
                    {
                        accNo,
                        orderNo,
                        productCode,
                        osBQty,
                        osSQty,
                        price,
                        traded,
                        advTradedPrice,
                        TOTAL
                    }

                    public TradeConfOrderDetailItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)tradeConfOrderDetailEnum.TOTAL) { }

                    public TradeConfOrderDetailItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public TradeConfOrderDetail(String scrStr) : base(scrStr, (int)TradeConfOrderDetailItem.tradeConfOrderDetailEnum.TOTAL) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public TradeConfOrderDetailItem GetTradeConfOrderDetailItem(int elementIndex)
                {
                    return new TradeConfOrderDetailItem(GetInfoItem(elementIndex));
                }


                /// <summary>
                /// Ben
                /// </summary>
                /// <param name="list"></param>
                /// <returns></returns>
                public DataTable GetTradeConfOrderDetailTable(infoItem[] list)
                {
                    if (list == null) return null;
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(TradeConfOrderDetailItem.tradeConfOrderDetailEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataColumn.DataType = typeof(string);
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    tempTable.Columns.Add(new DataColumn("DepInPrice", typeof(int)));
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            dr[i] = r.AllItem[i];
                        }
                        dr[tempTable.Columns.Count - 1] = MarketPriceData.GetDecInPrice(dr["productCode"].ToString()); ;
                        tempTable.Rows.Add(dr);
                    }
                    return tempTable;
                }
            }

            public class NotificationInfo : infoTypeOneLayer
            {
                public class NotificationInfoItem : infoItem
                {
                    public enum notificationInfoItemEnum
                    {
                        datetime,
                        userID,
                        accNo,  //p1.1
                        rInfo,  //p1.1
                        notifyCode,
                        notificationMsg,
                        TOTAL
                    }

                    public NotificationInfoItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)notificationInfoItemEnum.TOTAL) { }

                    public NotificationInfoItem(infoItem srcInfoItem) : base(srcInfoItem) { }

                    public DateTime getTime()
                    {
                        DateTime retTime = DateTime.MinValue;
                        if (allItem != null && allItem.Length == (int)notificationInfoItemEnum.TOTAL)
                        {
                            retTime = Tools.getDateTimeFromUnixTime(allItem[(int)notificationInfoItemEnum.datetime]);
                        }
                        return retTime;
                    }
                }

                public NotificationInfo(String scrStr) : base(scrStr, (int)NotificationInfoItem.notificationInfoItemEnum.TOTAL) { }

                public String GetNotificationInfoItem(infoItem list)
                {
                    String[] NotificationInfo = list.AllItem;
                    string str = null;
                    for (int i = 0; i < NotificationInfo.Length; i++)
                    {
                        str += NotificationInfo[i] + ",";
                    }
                    return str;
                }

            }

            public class TableNotificationInfo : infoTypeOneLayer
            {
                public class TableNotificationInfoItem : infoItem
                {
                    public enum tableNotificationInfoItemEnum
                    {
                        datetime,
                        userID,
                        accNo,  //p1.1
                        rInfo,  //p1.1
                        tableCode,
                        versionNo,
                        notificationMsg,
                        TOTAL
                    }

                    public TableNotificationInfoItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)tableNotificationInfoItemEnum.TOTAL) { }

                    public TableNotificationInfoItem(infoItem srcInfoItem) : base(srcInfoItem) { }

                    public DateTime getTime()
                    {
                        DateTime retTime = DateTime.MinValue;
                        if (allItem != null && allItem.Length == (int)tableNotificationInfoItemEnum.TOTAL)
                        {
                            retTime = Tools.getDateTimeFromUnixTime(allItem[(int)tableNotificationInfoItemEnum.datetime]);
                        }
                        return retTime;
                    }
                }

                public TableNotificationInfo(String scrStr) : base(scrStr, (int)TableNotificationInfoItem.tableNotificationInfoItemEnum.TOTAL) { }

                public String GetTableNotificationInfoItem(infoItem list)
                {
                    String[] NotificationInfo = list.AllItem;
                    string str = null;
                    for (int i = 0; i < NotificationInfo.Length; i++)
                    {
                        str += NotificationInfo[i] + ",";
                    }
                    return str;
                }

            }

            #endregion

            #region PriceManager
            public class MarketPrice : infoType
            {
                public class MarketPriceItem : infoItem
                {
                    public enum marketPriceItemEnum
                    {
                        datetime,
                        productCode,
                        productName,
                        expiry,
                        productStatus,
                        bQty,
                        bid,
                        ask,
                        aQty,
                        last,
                        ep,
                        lQty,
                        change,
                        changePercent,
                        volume,
                        high,
                        low,
                        open,
                        preClose,
                        closeDate,
                        strike,
                        TOTAL
                    }

                    public MarketPriceItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)marketPriceItemEnum.TOTAL) { }

                    public MarketPriceItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public MarketPrice(String scrStr) : base(scrStr, (int)MarketPriceItem.marketPriceItemEnum.TOTAL) { }

                public List<String> GetProductCodeList()
                {
                    return GetElementList((int)MarketPriceItem.marketPriceItemEnum.productCode);
                }

                public MarketPriceItem GetMarketPriceItem(String productCode)
                {
                    return new MarketPriceItem(GetInfoItem((int)MarketPriceItem.marketPriceItemEnum.productCode, productCode));
                }

                /*  /// <summary>
          /// add by ben
          /// </summary>
          /// <param name="list"></param>
          /// <returns>MarketPrice table </returns>
          public DataTable GetMarketPriceTable(infoItem[] list)
          {
              DataTable tempTable = new DataTable();
              foreach (string s in Enum.GetNames(typeof(MarketPriceItem.marketPriceItemEnum)))
              {
                  DataColumn dataColumn = new DataColumn(); 





















                  switch (s)
                  {
                      case "productCode":
                      case "productName":
                      case "productStatus":
                          dataColumn.DataType = typeof(string);
                          break;
                      case "closeDate":
                      case "expiry":
                      case "datetime":
                          dataColumn.DataType = typeof(DateTime);
                          break;
                      case "changePercent":
                          dataColumn.DataType = typeof(double);
                          break;
                      case "bid":
                      case "ask":
                      case "high":
                      case "last":
                      case "low":
                      case "open":
                      case "preClose":
                      case "ep":
                      case "change":
                      case "strike":
                          dataColumn.DataType = typeof(string);
                          break;
                      default:
                          dataColumn.DataType = typeof(int);
                          break;
                  }

                  dataColumn.ColumnName = s;
                  tempTable.Columns.Add(dataColumn);
              }
              tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
              tempTable.PrimaryKey = new DataColumn[] { tempTable.Columns["productCode"] };
              foreach (infoItem r in list)
              {
                  DataRow dr = tempTable.NewRow();
                  for (int i = 0; i < r.AllItem.Length; i++)
                  {
                      if (i == (int)MarketPriceItem.marketPriceItemEnum.datetime || i == (int)MarketPriceItem.marketPriceItemEnum.expiry || i == (int)MarketPriceItem.marketPriceItemEnum.closeDate)
                          dr[i] = Tools.getDateTimeFromUnixTime(r.AllItem[i]);
                      else if (i == (int)MarketPriceItem.marketPriceItemEnum.productStatus)
                      {
                          //trading status for futures:
                          // 0-Undefined, 1-pre open, 2-pre open alloc, 3-open alloc,
                          //4-pause, 5-open, 6-close, 7-pre market activity,
                          //8: expire, 9: delist, 
                          if (r.AllItem[i] == "0")
                          {
                              dr[i] = "";
                          }
                          else if (r.AllItem[i] == "1")
                          {
                              dr[i] = "Pre Open";
                          }
                          else if (r.AllItem[i] == "2")
                          {
                              dr[i] = "Pre Open Alloc";
                          }
                          else if (r.AllItem[i] == "3")
                          {
                              dr[i] = "Open Alloc";
                          }
                          else if (r.AllItem[i] == "4")
                          {
                              dr[i] = "Pause";
                          }
                          else if (r.AllItem[i] == "5")
                          {
                              dr[i] = "Open";
                          }
                          else if (r.AllItem[i] == "6")
                          {
                              dr[i] = "Close";
                          }
                          else if (r.AllItem[i] == "7")
                          {
                              dr[i] = "Pre Market Activity";
                          }
                          else if (r.AllItem[i] == "8")
                          {
                              dr[i] = "Expire";
                          }
                          else if (r.AllItem[i] == "9")
                          {
                              dr[i] = "Delist";
                          }
                      }
                      else if (i == (int)MarketPriceItem.marketPriceItemEnum.productCode ||
                          i == (int)MarketPriceItem.marketPriceItemEnum.productName ||
                          i == (int)MarketPriceItem.marketPriceItemEnum.strike ||
                          i == (int)MarketPriceItem.marketPriceItemEnum.closeDate ||
                          i == (int)MarketPriceItem.marketPriceItemEnum.expiry ||
                          i == (int)MarketPriceItem.marketPriceItemEnum.datetime ||
                          i == (int)MarketPriceItem.marketPriceItemEnum.changePercent)
                      {
                          dr[i] = r.AllItem[i];
                      }
                      else if (i == (int)MarketPriceItem.marketPriceItemEnum.ask ||
                          i == (int)MarketPriceItem.marketPriceItemEnum.bid
                            )









                      {
                          int depInPrice = TradeStationTools.ConvertToInt(MarketPriceData.GetDecInPrice(r.AllItem[1]), -1);
                          dr[i] = TradeStationTools.ConvertToFormatString(r.AllItem[i], depInPrice);
                      }
                      else if (i == (int)MarketPriceItem.marketPriceItemEnum.high ||
                          i == (int)MarketPriceItem.marketPriceItemEnum.last ||
                          i == (int)MarketPriceItem.marketPriceItemEnum.low ||
                          i == (int)MarketPriceItem.marketPriceItemEnum.preClose ||
                          i == (int)MarketPriceItem.marketPriceItemEnum.open ||
                          i == (int)MarketPriceItem.marketPriceItemEnum.change ||
                          i == (int)MarketPriceItem.marketPriceItemEnum.strike ||
                          i == (int)MarketPriceItem.marketPriceItemEnum.ep)
                      {
                          int depInPrice = TradeStationTools.ConvertToInt(MarketPriceData.GetDecInPrice(r.AllItem[1]), -1);
                          dr[i] = TradeStationTools.ConvertToFormatStringNull(r.AllItem[i], depInPrice);
                      }
                      else
                      {
                          switch (r.AllItem[i].ToString())
                          {
                              case "":
                                  dr[i] = DBNull.Value;
                                  break;
                              default:
                                  dr[i] = Convert.ToInt32(r.AllItem[i]);
                                  break;
                          }
                      }
                  }
                  tempTable.Rows.Add(dr);
              }
              return tempTable;
          }
*/

                public ObservableCollection<GOSTS.MarketPriceItem> GetMarketPriceItems(infoItem[] list)
                {
                    if (list == null) return null;
                    ObservableCollection<GOSTS.MarketPriceItem> marketPriceItems = new ObservableCollection<GOSTS.MarketPriceItem>();

                    GOSTS.MarketPriceItem item;
                    int depInPrice;
                    foreach (infoItem ls in list)
                    {
                        depInPrice = MarketPriceData.GetDecInPrice(ls.AllItem[1]);

                        item = new GOSTS.MarketPriceItem();
                        item.Datetime = Tools.getDateTimeFromUnixTime(ls.AllItem[0]);
                        item.ProductCode = ls.AllItem[1];
                        item.ProductName = ls.AllItem[2];
                        item.Expiry = Tools.getDateTimeFromUnixTime(ls.AllItem[3]);
                        switch (ls.AllItem[4])
                        {
                            //trading status for futures:
                            // 0-Undefined, 1-pre open, 2-pre open alloc, 3-open alloc,
                            //4-pause, 5-open, 6-close, 7-pre market activity,
                            //8: expire, 9: delist, 
                            //10: vcm,
                            case "0":
                                item.ProductStatus = "";
                                break;
                            case "1":
                                item.ProductStatus = "Pre Open";
                                break;
                            case "2":
                                item.ProductStatus = "Pre Open Alloc";
                                break;
                            case "3":
                                item.ProductStatus = "Open Alloc";
                                break;
                            case "4":
                                item.ProductStatus = "Pause";
                                break;
                            case "5":
                                item.ProductStatus = "Open";
                                break;
                            case "6":
                                item.ProductStatus = "Close";
                                break;
                            case "7":
                                item.ProductStatus = "Pre Market Activity";
                                break;
                            case "8":
                                item.ProductStatus = "Expire";
                                break;
                            case "9":
                                item.ProductStatus = "Delist";
                                break;
                            case "10":
                                item.ProductStatus = "VCM";
                                break;
                            default:
                                item.ProductStatus = "";
                                break;

                        }
                        item.BQty = TradeStationTools.ConvertToString(ls.AllItem[5]);
                        item.Bid = TradeStationTools.ConvertToStringForBidAsk(ls.AllItem[6], depInPrice);
                        item.Ask = TradeStationTools.ConvertToStringForBidAsk(ls.AllItem[7], depInPrice);
                        item.AQty = TradeStationTools.ConvertToString(ls.AllItem[8]);
                        item.Last = TradeStationTools.ConvertToFormatStringNull(ls.AllItem[9], depInPrice);
                        item.EP = TradeStationTools.ConvertToFormatStringNull(ls.AllItem[10], depInPrice);
                        item.LQty = TradeStationTools.ConvertToString(ls.AllItem[11]);
                        item.Change = TradeStationTools.ConvertToFormatStringNull(ls.AllItem[12], depInPrice);
                        item.ChangePer = ls.AllItem[13];
                        item.Volume = TradeStationTools.ConvertToString(ls.AllItem[14]);
                        item.High = TradeStationTools.ConvertToFormatStringNull(ls.AllItem[15], depInPrice);
                        item.Low = TradeStationTools.ConvertToFormatStringNull(ls.AllItem[16], depInPrice);
                        item.Open = TradeStationTools.ConvertToFormatStringNull(ls.AllItem[17], depInPrice);
                        item.PreClose = TradeStationTools.ConvertToFormatStringNull(ls.AllItem[18], depInPrice);
                        item.CloseDate = Tools.getDateTimeFromUnixTime(ls.AllItem[19]);
                        item.Strike = TradeStationTools.ConvertToFormatStringNull(ls.AllItem[20], depInPrice);

                        marketPriceItems.Add(item);
                    }
                    return marketPriceItems;
                }
            }

            public class PriceDepth : infoType
            {
                public class PriceDepthItem : infoItem
                {
                    public enum priceDepthItemEnum
                    {
                        datetime,
                        productCode,
                        bidPrice1,
                        bidQty1,
                        bidPrice2,
                        bidQty2,
                        bidPrice3,
                        bidQty3,
                        bidPrice4,
                        bidQty4,
                        bidPrice5,
                        bidQty5,
                        askPrice1,
                        askQty1,
                        askPrice2,
                        askQty2,
                        askPrice3,
                        askQty3,
                        askPrice4,
                        askQty4,
                        askPrice5,
                        askQty5,
                        TOTAL
                    }

                    public PriceDepthItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)priceDepthItemEnum.TOTAL) { }

                    public PriceDepthItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public PriceDepth(String scrStr) : base(scrStr, (int)PriceDepthItem.priceDepthItemEnum.TOTAL) { }

                public List<String> GetProductCodeList()
                {
                    return GetElementList((int)PriceDepthItem.priceDepthItemEnum.productCode);
                }

                public PriceDepthItem GetPriceDepthItem(String productCode)
                {
                    return new PriceDepthItem(GetInfoItem((int)PriceDepthItem.priceDepthItemEnum.productCode, productCode));
                }

                /// <summary>
                /// add by ben
                /// </summary>
                /// <param name="list"></param>
                /// <returns>PriceDepth table </returns>
                public DataTable GetPriceDepthTable(infoItem[] list)
                {
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(PriceDepthItem.priceDepthItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataColumn.DataType = typeof(string);
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    tempTable.PrimaryKey = new DataColumn[] { tempTable.Columns["productCode"] };
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            dr[i] = r.AllItem[i];
                        }
                        tempTable.Rows.Add(dr);
                    }
                    return tempTable;
                }

                /// <summary>
                /// Ben
                /// </summary>
                /// <param name="list"></param>
                /// <returns>ObservableCollection></returns>
                public ObservableCollection<LongPriceDepthData> GetPriceDepthItemData(infoItem[] list)
                {
                    if (list == null)
                        return null;
                    ObservableCollection<LongPriceDepthData> priceDepthDatas = new ObservableCollection<LongPriceDepthData>();
                    foreach (infoItem ls in list)
                    {
                        int i = 0;
                        int depInPrice = MarketPriceData.GetDecInPrice(ls.AllItem[1].ToString());
                        do
                        {
                            priceDepthDatas.Add(new LongPriceDepthData()
                            {
                                CurrentTime = ls.AllItem[0].ToString(),
                                ProdCode = ls.AllItem[1].ToString(),
                                Reserve = "",
                                ID = i,
                                Bid = (ls.AllItem[2 * i + 2] == "AO") ? AppFlag.AONum : TradeStationTools.ConvertToFloat(ls.AllItem[2 * i + 2], depInPrice),
                                BQty = TradeStationTools.ConvertToInt32(ls.AllItem[2 * i + 3]),
                                Ask = (ls.AllItem[2 * i + 12] == "AO") ? AppFlag.AONum : TradeStationTools.ConvertToFloat(ls.AllItem[2 * i + 12], depInPrice),
                                AQty = TradeStationTools.ConvertToInt32(ls.AllItem[2 * i + 13])
                            });
                            i++;
                        }
                        while (i < 5);
                    }
                    return priceDepthDatas;
                }

            }

            public class LongPriceDepth : infoTypeSubItem
            {
                public class LongPriceDepthItem : infoItem
                {
                    //public enum longPriceDepthItemEnum
                    //{
                    //    datetime,
                    //    productCode,
                    //    bidPrice,
                    //    bidQty,
                    //    askPrice,
                    //    askQty,
                    //    TOTAL
                    //}
                    public enum longPriceDepthItemEnum
                    {
                        datetime,
                        productCode,
                        numOfSubItems
                    }
                    public enum longPriceDepthSubItemEnum
                    {
                        reserved,
                        bidPrice,
                        bidQty,
                        askPrice,
                        askQty,
                        TOTAL
                    }

                    //public LongPriceDepthItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)longPriceDepthItemEnum.TOTAL) { }

                    public LongPriceDepthItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public LongPriceDepth(String scrStr) : base(scrStr, (int)LongPriceDepthItem.longPriceDepthItemEnum.numOfSubItems, (int)LongPriceDepthItem.longPriceDepthSubItemEnum.TOTAL) { }

                public List<String> GetProductCodeList()
                {
                    return GetElementList((int)LongPriceDepthItem.longPriceDepthItemEnum.productCode);
                }

                public LongPriceDepthItem GetLongPriceDepthItem(String productCode)
                {
                    return new LongPriceDepthItem(GetInfoItem((int)LongPriceDepthItem.longPriceDepthItemEnum.productCode, productCode));
                }

                ///// <summary>
                ///// add by ben
                ///// </summary>
                ///// <param name="list"></param>
                ///// <returns>LongPriceDepth table </returns>
                //public DataTable GetLongPriceDepthTable(infoItem[] list)
                //{
                //    DataTable tempTable = new DataTable();
                //    foreach (string s in Enum.GetNames(typeof(LongPriceDepthItem.longPriceDepthItemEnum)))
                //    {
                //        DataColumn dataColumn = new DataColumn();
                //        dataColumn.DataType = typeof(string);
                //        dataColumn.ColumnName = s;
                //        tempTable.Columns.Add(dataColumn);
                //    }
                //    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                //    foreach (infoItem r in list)
                //    {
                //        DataRow dr = tempTable.NewRow();
                //        for (int i = 0; i < r.AllItem.Length; i++)
                //        {
                //            dr[i] = r.AllItem[i];
                //        }
                //        tempTable.Rows.Add(dr);
                //    }
                //    return tempTable;
                //}
            }

            public class ProductList : infoType
            {
                public class ProductListItem : infoItem
                {
                    public enum productListItemEnum
                    {
                        datetime,
                        productCode,
                        prodType,
                        prodName,
                        prodName1,
                        prodName2,
                        prodName3,
                        underlying,
                        instmntCode,
                        expiryDate,
                        active,
                        callPut,
                        strike,
                        lotSize,
                        optStyle,
                        convRatio,
                        marginRatio,
                        gatewayCode,
                        // added some fields, kenlo131003
                        tickSize,
                        decInPrice,
                        priceLowerLimit,
                        priceUpperLimit,
                        priceLowerQty,
                        priceUpperQty,
                        tPlusOne,
                        TOTAL
                    }

                    public String InstrumentCode
                    {
                        get
                        {
                            return allItem[(int)productListItemEnum.instmntCode];
                        }
                    }

                    public ProductListItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)productListItemEnum.TOTAL) { }

                    public ProductListItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public ProductList(String scrStr) : base(scrStr, (int)ProductListItem.productListItemEnum.TOTAL) { }

                public List<String> GetProductCodeList()
                {
                    return GetElementList((int)ProductListItem.productListItemEnum.productCode);
                }

                public ProductListItem GetProductListItem(String productCode)
                {
                    return new ProductListItem(GetInfoItem((int)ProductListItem.productListItemEnum.productCode, productCode));
                }
                /// <summary>
                /// add by Ben
                /// </summary>
                /// <param name="list"></param>
                /// <returns>ProductList table</returns>
                public DataTable GetProductListTable(infoItem[] list)
                {
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(ProductListItem.productListItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataColumn.DataType = typeof(string);
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            if (i == (int)ProductListItem.productListItemEnum.expiryDate)
                                dr[i] = Tools.getDateTimeFromUnixTime(r.AllItem[i]).ToString("yyyy/MM/dd HH:mm:ss");
                            else
                            {
                                dr[i] = r.AllItem[i];
                            }
                        }
                        tempTable.Rows.Add(dr);
                    }
                    DataView dv = tempTable.DefaultView;
                    dv.Sort = "instmntCode";
                    tempTable = dv.ToTable();
                    return tempTable;
                }
            }

            public class InstrumentList : infoType
            {
                public class InstrumentListItem : infoItem
                {
                    public enum instrumentListItemEnum
                    {
                        datetime,
                        marketCode,
                        instmntCode,
                        instmntName,
                        instmntType,
                        ccy,
                        lotSize,
                        contraceSize,
                        decInPrice,
                        openMkt1,
                        dayCutoff1,
                        closeMkt1,
                        marginMethod,
                        spanCommCode,
                        openMkt2,
                        dayCutoff2,
                        closeMkt2,
                        spanExchAcro,
                        decInUlPrice,
                        unitName,
                        hodidayCode,
                        span4ExchAcro,
                        span4CommCode,
                        gateway,
                        marginCode,
                        margin,
                        spread,
                        spanWeight,
                        mmarginWeight,
                        spanDecInPrice,
                        spanDecInStrike,
                        instmntName1,
                        instmntName2,
                        fixedRate,
                        brokerID,
                        tickSize,
                        boInstmntCode,
                        openExpdel1,
                        closeExpdel1,
                        openExpdel2,
                        closeExpdel2,
                        gwCode,
                        busNo,
                        boDecInPrice,
                        maxMonth,
                        maxOptMonth,
                        tradeMonth,
                        TOTAL
                    }

                    public String MarketCode
                    {
                        get
                        {
                            return allItem[(int)instrumentListItemEnum.marketCode];
                        }
                    }
                    public int ContractSize
                    {
                        get
                        {
                            int number;
                            if (Int32.TryParse(allItem[(int)instrumentListItemEnum.contraceSize], out number))
                            {
                                return number;
                            }
                            return -1;
                        }
                    }
                    public InstrumentListItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)instrumentListItemEnum.TOTAL) { }

                    public InstrumentListItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public InstrumentList(String scrStr) : base(scrStr, (int)InstrumentListItem.instrumentListItemEnum.TOTAL) { }

                public List<String> GetMarketCodeList()
                {
                    return GetElementList((int)InstrumentListItem.instrumentListItemEnum.marketCode);
                }
                public List<String> GetInstrumentCodeList()
                {
                    return GetElementList((int)InstrumentListItem.instrumentListItemEnum.instmntCode);
                }

                public InstrumentListItem GetInstrumentListItem(String instmntCode)
                {
                    return new InstrumentListItem(GetInfoItem((int)InstrumentListItem.instrumentListItemEnum.instmntCode, instmntCode));
                }

                /// <summary>
                /// add by ben
                /// </summary>
                /// <param name="list"></param>
                /// <returns>InstrumentList table </returns>
                public DataTable GetInstrumentListTable(infoItem[] list)
                {
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(InstrumentListItem.instrumentListItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataColumn.DataType = typeof(string);
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            dr[i] = r.AllItem[i];
                        }
                        tempTable.Rows.Add(dr);
                    }
                    //DataView dv = tempTable.DefaultView;
                    //dv.Sort = "marketCode";
                    //tempTable = dv.ToTable();
                    return tempTable;
                }
            }
            #endregion

            #region TradeManager
            public class Ticker : infoType
            {
                public class TickerItem : infoItem
                {
                    public enum tickerItemEnum
                    {
                        productCode,
                        qty,
                        price,
                        logTime,
                        dealSrc,
                        TOTAL
                    }

                    public TickerItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)tickerItemEnum.TOTAL) { }

                    public TickerItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public Ticker(String scrStr) : base(scrStr, (int)TickerItem.tickerItemEnum.TOTAL) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public TickerItem GetTickerItem(int elementIndex)
                {
                    return new TickerItem(GetInfoItem(elementIndex));
                }

                /// <summary>
                /// get the first found item with given productCode
                /// </summary>
                /// <param name="productCode"></param>
                /// <returns></returns>
                public TickerItem GetTickerItem(String productCode)
                {
                    return new TickerItem(GetInfoItem((int)TickerItem.tickerItemEnum.productCode, productCode));
                }

                /// <summary>
                /// add by ben
                /// </summary>
                /// <param name="list"></param>
                /// <returns>Ticker table </returns>
                public DataTable GetTickerTable(infoItem[] list)
                {
                    if (list == null) return null;
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(TickerItem.tickerItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataColumn.DataType = typeof(string);
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    //tempTable.PrimaryKey = new DataColumn[] { tempTable.Columns["productCode"] };
                    foreach (infoItem r in list)
                    {

                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            if (i == (int)TickerItem.tickerItemEnum.logTime)
                                dr[i] = Tools.getDateTimeFromUnixTime(r.AllItem[i]);
                            else
                                dr[i] = r.AllItem[i];
                        }
                        tempTable.Rows.Add(dr);
                    }
                    //tempTable.DefaultView.Sort = " logTime asc";
                    return tempTable;
                }
                /// <summary>
                /// Ben
                /// </summary>
                /// <param name="list"></param>
                /// <returns>ObservableCollection></returns>
                public ObservableCollection<TickerData> GetTickerItemData(infoItem[] list)
                {
                    if (list == null)
                        return null;
                    ObservableCollection<TickerData> tickerDatas = new ObservableCollection<TickerData>();
                    foreach (infoItem ls in list)
                    {
                        int depInPrice = MarketPriceData.GetDecInPrice(ls.AllItem[0]);
                        tickerDatas.Add(new TickerData()
                        {
                            productCode = ls.AllItem[0].ToString(),
                            qty = ls.AllItem[1].ToString(),
                            price = TradeStationTools.ConvertToFloat(ls.AllItem[2], depInPrice).ToString(),
                            logTime = Tools.getDateTimeFromUnixTime(ls.AllItem[3]),
                            dealSrc = ls.AllItem[4].ToString(),

                        });
                    }
                    return tickerDatas;
                }
            }

            public class TradeStatistics : infoType
            {
                public class TradeStatisticsItem : infoItem
                {
                    public enum tradeStatisticsItemEnum
                    {
                        productCode,
                        prev,
                        inOut,
                        dayLong,
                        dayShort,
                        dayNet,
                        net,
                        mktPrice,
                        pl,
                        refFxRate,
                        plBaseCcy,
                        contract,
                        TOTAL
                    }

                    public TradeStatisticsItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)tradeStatisticsItemEnum.TOTAL) { }

                    public TradeStatisticsItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public TradeStatistics(String scrStr) : base(scrStr, (int)TradeStatisticsItem.tradeStatisticsItemEnum.TOTAL) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public TradeStatisticsItem GetTradeStatisticsItem(int elementIndex)
                {
                    return new TradeStatisticsItem(GetInfoItem(elementIndex));
                }

                /// <summary>
                /// get the first found item with given productCode
                /// </summary>
                /// <param name="productCode"></param>
                /// <returns></returns>
                public TradeStatisticsItem GetTradeStatisticsItem(String productCode)
                {
                    return new TradeStatisticsItem(GetInfoItem((int)TradeStatisticsItem.tradeStatisticsItemEnum.productCode, productCode));
                }

                /// <summary>
                /// add by ben
                /// </summary>
                /// <param name="list"></param>
                /// <returns> Trade Statistic Table </returns>
                public DataTable GetStatisticTable(infoItem[] list)
                {
                    DataTable tempTable = new DataTable();
                    foreach (string s in Enum.GetNames(typeof(TradeStatisticsItem.tradeStatisticsItemEnum)))
                    {
                        DataColumn dataColumn = new DataColumn();
                        dataColumn.DataType = typeof(string);
                        dataColumn.ColumnName = s;
                        tempTable.Columns.Add(dataColumn);
                    }
                    tempTable.Columns.Remove(tempTable.Columns[tempTable.Columns.Count - 1]);
                    tempTable.PrimaryKey = new DataColumn[] { tempTable.Columns["productCode"] };
                    foreach (infoItem r in list)
                    {
                        DataRow dr = tempTable.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            dr[i] = r.AllItem[i];
                        }
                        tempTable.Rows.Add(dr);
                    }
                    return tempTable;
                }
            }

            public class Chart : infoType
            {
                public class ChartItem : infoItem
                {
                    public enum chartItemEnum
                    {
                        date,
                        time,
                        close,
                        volume,
                        high,
                        low,
                        open,
                        TOTAL
                    }

                    public ChartItem(String[] scrStrArr, int startIndex) : base(scrStrArr, startIndex, (int)chartItemEnum.TOTAL) { }

                    public ChartItem(infoItem srcInfoItem) : base(srcInfoItem) { }
                }

                public Chart(String scrStr) : base(scrStr, (int)ChartItem.chartItemEnum.TOTAL) { }

                /// <summary>
                /// get the nth item
                /// </summary>
                /// <param name="elementIndex"></param>
                /// <returns></returns>
                public ChartItem GetChartItem(int elementIndex)
                {
                    return new ChartItem(GetInfoItem(elementIndex));
                }

                ///// <summary>
                ///// get the first found item with given productCode
                ///// </summary>
                ///// <param name="productCode"></param>
                ///// <returns></returns>
                //public ChartItem GetChartItem(String productCode)
                //{
                //    return new ChartItem(GetInfoItem((int)ChartItem.chartItemEnum.productCode, productCode));
                //}

                public class ChartHistoryData
                {
                    public String productCode { get; private set; }
                    public DataTable points { get; private set; }

                    public ChartHistoryData(String prod)
                    {
                        productCode = prod;
                        points = new DataTable();
                        foreach (string s in Enum.GetNames(typeof(ChartItem.chartItemEnum)))
                        {
                            DataColumn dataColumn = new DataColumn();
                            dataColumn.DataType = typeof(string);
                            dataColumn.ColumnName = s;
                            points.Columns.Add(dataColumn);
                        }

                        points.Columns.Remove(points.Columns[points.Columns.Count - 1]);
                    }
                }

                public ChartHistoryData GetChartInfoTable(TradeStationComm.infoClass.Chart chart)
                {
                    String productCode = chart.RInfo;
                    int Start = productCode.IndexOf("id=", 0) + "id=".Length;
                    int End = productCode.IndexOf("&ps=", Start);
                    productCode = productCode.Substring(Start, End - Start);
                    if (productCode.Length == 0) productCode = "";
                    ChartHistoryData chartHistory = new ChartHistoryData(productCode);
                    foreach (infoItem r in chart.InfoItems)
                    {
                        DataRow dr = chartHistory.points.NewRow();
                        for (int i = 0; i < r.AllItem.Length; i++)
                        {
                            dr[i] = r.AllItem[i];
                        }
                        chartHistory.points.Rows.Add(dr);
                    }

                    return chartHistory;
                }

            }
            #endregion
        }
        #endregion

        #region MsgResponse
        public class MsgResponse
        {
            //private static String  getMsgBody(String respMsg,  TradeStationComm.Header.DST manager, int cmd)
            //{
            //    String retBody = null;
            //    byte[] header;
            //    String body;
            //    if (TradeStationComm.getMsg(respMsg, out header, out body))
            //    {
            //        byte cmdGot;
            //        TradeStationComm.Header.DST managerGot = TradeStationComm.getMsgManager(header, out cmdGot);
            //        if (managerGot == manager && cmdGot == cmd)
            //        {
            //            retBody = body;
            //        }
            //    }
            //    return retBody;
            //}

            //#region PriceManager
            //public static infoClass.MarketPrice getMarketPrice(String respMsg)
            //{
            //    infoClass.MarketPrice retInfoClass = null;
            //    String body = getMsgBody(respMsg, TradeStationComm.Header.DST.priceManager, (int)TradeStationComm.PriceManager.cmdServer.getMarketPrice);
            //    if (body != null)
            //    {
            //        infoClass.MarketPrice infoClassGot;
            //        if (TradeStationComm.PriceManager.getResponseMarketPrice(body, out infoClassGot))
            //        {
            //            retInfoClass = infoClassGot;
            //        }
            //    }
            //    return retInfoClass;
            //}
            //public static bool getResponseRegisterMarketPrice(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static infoClass.PriceDepth getPriceDepth(String respMsg)
            //{
            //    infoClass.PriceDepth retInfoClass = null;
            //    String body = getMsgBody(respMsg, TradeStationComm.Header.DST.priceManager, (int)TradeStationComm.PriceManager.cmdServer.getPriceDepth);
            //    if (body != null)
            //    {
            //        infoClass.PriceDepth infoClassGot;
            //        if (TradeStationComm.PriceManager.getResponsePriceDepth(body, out infoClassGot))
            //        {
            //            retInfoClass = infoClassGot;
            //        }
            //    }
            //    return retInfoClass;
            //}
            //public static bool getResponseRegisterPriceDepth(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static infoClass.LongPriceDepth getLongPriceDepth(String respMsg)
            //{
            //    infoClass.LongPriceDepth retInfoClass = null;
            //    String body = getMsgBody(respMsg, TradeStationComm.Header.DST.priceManager, (int)TradeStationComm.PriceManager.cmdServer.getLongPriceDepth);
            //    if (body != null)
            //    {
            //        infoClass.LongPriceDepth infoClassGot;
            //        if (TradeStationComm.PriceManager.getResponseLongPriceDepth(body, out infoClassGot))
            //        {
            //            retInfoClass = infoClassGot;
            //        }
            //    }
            //    return retInfoClass;
            //}
            //public static bool getResponseRegisterLongPriceDepth(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}

            //public static infoClass.ProductList getProductList(String respMsg)
            //{
            //    infoClass.ProductList retInfoClass = null;
            //    String body = getMsgBody(respMsg, TradeStationComm.Header.DST.priceManager, (int)TradeStationComm.PriceManager.cmdServer.getProductList);
            //    if (body != null)
            //    {
            //        infoClass.ProductList infoClassGot;
            //        if (TradeStationComm.PriceManager.getResponseProductList(body, out infoClassGot))
            //        {
            //            retInfoClass = infoClassGot;
            //        }
            //    }
            //    return retInfoClass;
            //}
            //public static infoClass.InstrumentList getInstrumentList(String respMsg)
            //{
            //    infoClass.InstrumentList retInfoClass = null;
            //    String body = getMsgBody(respMsg, TradeStationComm.Header.DST.priceManager, (int)TradeStationComm.PriceManager.cmdServer.getInstrumentList);
            //    if (body != null)
            //    {
            //        infoClass.InstrumentList infoClassGot;
            //        if (TradeStationComm.PriceManager.getResponseInstrumentList(body, out infoClassGot))
            //        {
            //            retInfoClass = infoClassGot;
            //        }
            //    }
            //    return retInfoClass;
            //}
            //#endregion

            //#region AccountManager
            //public static bool getResponseLogin(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static bool getResponseLogout(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static bool getResponseChangePassword(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static bool getResponseChangeLanguage(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static infoClass.AccountInfo getAccountInfo(String respMsg)
            //{
            //    infoClass.AccountInfo retInfoClass = null;
            //    String body = getMsgBody(respMsg, TradeStationComm.Header.DST.accountManager, (int)TradeStationComm.AccountManager.cmdServer.getAccountInfo);
            //    if (body != null)
            //    {
            //        infoClass.AccountInfo infoClassGot;
            //        if (TradeStationComm.AccountManager.getResponseAccountInfo(body, out infoClassGot))
            //        {
            //            retInfoClass = infoClassGot;
            //        }
            //    }
            //    return retInfoClass;
            //}
            //public static infoClass.OrderBook getOrderBook(String respMsg)
            //{
            //    infoClass.OrderBook retInfoClass = null;
            //    String body = getMsgBody(respMsg, TradeStationComm.Header.DST.accountManager, (int)TradeStationComm.AccountManager.cmdServer.getOrderBookInfo);
            //    if (body != null)
            //    {
            //        infoClass.OrderBook infoClassGot;
            //        if (TradeStationComm.AccountManager.getResponseOrderBook(body, out infoClassGot))
            //        {
            //            retInfoClass = infoClassGot;
            //        }
            //    }
            //    return retInfoClass;
            //}
            //public static infoClass.Position getPosition(String respMsg)
            //{
            //    infoClass.Position retInfoClass = null;
            //    String body = getMsgBody(respMsg, TradeStationComm.Header.DST.accountManager, (int)TradeStationComm.AccountManager.cmdServer.getPositionInfo);
            //    if (body != null)
            //    {
            //        infoClass.Position infoClassGot;
            //        if (TradeStationComm.AccountManager.getResponsePosition(body, out infoClassGot))
            //        {
            //            retInfoClass = infoClassGot;
            //        }
            //    }
            //    return retInfoClass;
            //}
            //public static infoClass.PositionSummary getPositionSummary(String respMsg)
            //{
            //    infoClass.PositionSummary retInfoClass = null;
            //    String body = getMsgBody(respMsg, TradeStationComm.Header.DST.accountManager, (int)TradeStationComm.AccountManager.cmdServer.getPositionSummary);
            //    if (body != null)
            //    {
            //        infoClass.PositionSummary infoClassGot;
            //        if (TradeStationComm.AccountManager.getResponsePositionSummary(body, out infoClassGot))
            //        {
            //            retInfoClass = infoClassGot;
            //        }
            //    }
            //    return retInfoClass;
            //}
            //public static infoClass.CashInfo getCashInfo(String respMsg)
            //{
            //    infoClass.CashInfo retInfoClass = null;
            //    String body = getMsgBody(respMsg, TradeStationComm.Header.DST.accountManager, (int)TradeStationComm.AccountManager.cmdServer.getCashInfo);
            //    if (body != null)
            //    {
            //        infoClass.CashInfo infoClassGot;
            //        if (TradeStationComm.AccountManager.getResponseCashInfo(body, out infoClassGot))
            //        {
            //            retInfoClass = infoClassGot;
            //        }
            //    }
            //    return retInfoClass;
            //}
            //public static infoClass.DoneTrade getDoneTrade(String respMsg)
            //{
            //    infoClass.DoneTrade retInfoClass = null;
            //    String body = getMsgBody(respMsg, TradeStationComm.Header.DST.accountManager, (int)TradeStationComm.AccountManager.cmdServer.getDoneTradeInfo);
            //    if (body != null)
            //    {
            //        infoClass.DoneTrade infoClassGot;
            //        if (TradeStationComm.AccountManager.getResponseDoneTradeInfo(body, out infoClassGot))
            //        {
            //            retInfoClass = infoClassGot;
            //        }
            //    }
            //    return retInfoClass;
            //}
            //public static infoClass.ClearTrade getClearTrade(String respMsg)
            //{
            //    infoClass.ClearTrade retInfoClass = null;
            //    String body = getMsgBody(respMsg, TradeStationComm.Header.DST.accountManager, (int)TradeStationComm.AccountManager.cmdServer.getClearTradeInfo);
            //    if (body != null)
            //    {
            //        infoClass.ClearTrade infoClassGot;
            //        if (TradeStationComm.AccountManager.getResponseClearTradeInfo(body, out infoClassGot))
            //        {
            //            retInfoClass = infoClassGot;
            //        }
            //    }
            //    return retInfoClass;
            //}
            //#endregion

            //#region OrderManager
            //public static bool getResponseAddOrder(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static bool getResponseChangeOrder(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static bool getResponseDeleteOrder(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static bool getResponseActivateOrder(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //public static bool getResponseInactivateOrder(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}
            //#endregion

            //#region TradeManager
            //public static infoClass.Ticker getTicker(String respMsg)
            //{
            //    infoClass.Ticker retInfoClass = null;
            //    String body = getMsgBody(respMsg, TradeStationComm.Header.DST.tradeManager, (int)TradeStationComm.TradeManager.cmdServer.getTicker);
            //    if (body != null)
            //    {
            //        infoClass.Ticker infoClassGot;
            //        if (TradeStationComm.TradeManager.getResponseTicker(body, out infoClassGot))
            //        {
            //            retInfoClass = infoClassGot;
            //        }
            //    }
            //    return retInfoClass;
            //}

            //public static bool getResponseRegisterTicker(String bodyStr, out String outString)
            //{
            //    return Tools.getSimpleResponse(bodyStr, out outString);
            //}

            //public static infoClass.TradeStatistics getTradeStatistics(String respMsg)
            //{
            //    infoClass.TradeStatistics retInfoClass = null;
            //    String body = getMsgBody(respMsg, TradeStationComm.Header.DST.tradeManager, (int)TradeStationComm.TradeManager.cmdServer.getTradeStatistics);
            //    if (body != null)
            //    {
            //        infoClass.TradeStatistics infoClassGot;
            //        if (TradeStationComm.TradeManager.getResponseTradeStatistics(body, out infoClassGot))
            //        {
            //            retInfoClass = infoClassGot;
            //        }
            //    }
            //    return retInfoClass;
            //}
            //public static infoClass.Chart getChart(String respMsg)
            //{
            //    infoClass.Chart retInfoClass = null;
            //    String body = getMsgBody(respMsg, TradeStationComm.Header.DST.tradeManager, (int)TradeStationComm.TradeManager.cmdServer.getChart);
            //    if (body != null)
            //    {
            //        infoClass.Chart infoClassGot;
            //        if (TradeStationComm.TradeManager.getResponseChart(body, out infoClassGot))
            //        {
            //            retInfoClass = infoClassGot;
            //        }
            //    }
            //    return retInfoClass;
            //}
            //#endregion

            //public static int getResponseErrorMessage(String bodyStr, out String outString)
            //{
            //    return Tools.getErrorResponse(bodyStr, out outString);
            //}

            public enum responseType
            {
                NIL,
                getMarketPrice,
                registerMarketPrice,
                getPriceDepth,
                registerPriceDepth,
                getLongPriceDepth,
                registerLongPriceDepth,
                getProductList,
                getInstrumentList,
                //marketPricePush,
                //priceDepthPush,
                //longPriceDepthPush,
                login,
                logout,
                changePassword,
                ChangeLanguage,
                getAccountInfo,
                getOrderBookInfo,
                getPositionInfo,
                getPositionSummary,
                getCashInfo,
                getDoneTradeInfo,
                getClearTradeInfo,
                getAccList,
                getMarginCheck,
                getMarginCallList,
                reqCashApproval,
                getCashApprovalInfo,
                getTradeConfOrders,
                getTradeConfTrades,
                reportTradeConf,
                getAccountMaster,
                getSysParam,
                getTradeConfOrderDetail,
                //doneTradeInfoPush,
                tableNotification,
                notificationMsg,
                addOrder,
                changeOrder,
                deleteOrder,
                activateOrder,
                inactivateOrder,
                getTicker,
                registerTicker,
                getTradeStatistics,
                getChart,
                //tickerPush,
                heartbeat, //add by ben 14/01/2014

                packetErrNotSOH = 200,

                errorMsg = 255
            }

            public enum errCodeDef
            {
                inactivateOrderFail = -180,
                activateOrderFail = -160,
                deleteOrderFail = -140,
                changeOrderFail = -120,
                addOrderFail = -100,
                invalidPassword = -9,
                userExpired = -8,
                userNotAcctive = -7,
                protocolVersionNotSupported = -6,
                noAccNo = -5,
                notLoggedIn = -4,
                invalidUser_userNotFound = -3,
                invalidSessionHash = -2,
                failure = -1,
                ok = 0,
                //-19,AE not match with acc  add by Ben
            }

            public class responseInfo
            {
                public DateTime Datetime { get; set; }
                public String UserID { get; set; }
                public String AccNo { get; set; }
                public String RInfo { get; set; }

                public responseInfo(DateTime Datetime, String UserID, String AccNo, String RInfo)
                {
                    this.Datetime = Datetime;
                    this.UserID = UserID;
                    this.AccNo = AccNo;
                    this.RInfo = RInfo;
                }

                public responseInfo(String Datetime, String UserID, String AccNo, String RInfo)
                    : this(Tools.getDateTimeFromUnixTime(Datetime), UserID, AccNo, RInfo)
                {
                }
            }

            public class ResponseObject
            {
                responseType theResponseType;
                public responseType ResponseType
                {
                    get { return theResponseType; }
                }

                responseInfo theResponseInfo;
                public responseInfo ResponseInfo
                {
                    get { return theResponseInfo; }
                }

                infoClass.infoObjectAbs theObject;
                public infoClass.infoObjectAbs InfoObject
                {
                    get { return theObject; }
                }

                //public ResponseObject() : this(responseType.NIL, null) { }

                public ResponseObject(responseType theResponseType, infoClass.infoObjectAbs theObject, responseInfo theResponseInfo)
                {
                    this.theResponseType = theResponseType;
                    this.theObject = theObject;
                    this.theResponseInfo = theResponseInfo;
                }
            }

            /// <summary>
            /// return ResponseObject, cast to appropriate infoClass
            /// </summary>
            /// <param name="respMsg"></param>
            /// <param name="infoClassObj"></param>
            /// <returns></returns>
            //public static ResponseObject getResponseAnyMsg(String respMsg)
            //{
            //    object infoClassObj = null;
            //    responseType resp = responseType.NIL;

            //    byte[] header;
            //    String body;
            //    if (TradeStationComm.getMsg(respMsg, out header, out body))
            //    {
            //        byte cmdGot;
            //        TradeStationComm.Header.DST managerGot = TradeStationComm.getMsgManager(header, out cmdGot);
            //        if (cmdGot == (int)PriceManager.cmdServer.errorMsg)
            //        {
            //            //resp = responseType.errorMsg;
            //            //String outString;
            //            //Tools.getErrorResponse(body, out outString);
            //            //infoClassObj = outString;
            //            resp = responseType.errorMsg;
            //            infoClassObj = Tools.getErrorResponseInfo(body);
            //        }
            //        else
            //        {
            //            switch (managerGot)
            //            {
            //                #region Price Manager Response
            //                case Header.DST.priceManager:
            //                    switch (cmdGot)
            //                    {
            //                        case (int)PriceManager.cmdServer.getMarketPrice:
            //                        case (int)PriceManager.cmdServer.marketPricePush:
            //                            resp = responseType.getMarketPrice;
            //                            {
            //                                infoClass.MarketPrice infoClassGot;
            //                                if (TradeStationComm.PriceManager.getResponseMarketPrice(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;

            //                        case (int)PriceManager.cmdServer.registerMarketPrice:
            //                            resp = responseType.registerMarketPrice;
            //                            infoClassObj = Tools.getSimpleResponseInfo(body);
            //                            break;

            //                        case (int)PriceManager.cmdServer.getPriceDepth:
            //                        case (int)PriceManager.cmdServer.priceDepthPush:
            //                            resp = responseType.getPriceDepth;
            //                            {
            //                                infoClass.PriceDepth infoClassGot;
            //                                if (TradeStationComm.PriceManager.getResponsePriceDepth(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;

            //                        case (int)PriceManager.cmdServer.registerPriceDepth:
            //                            resp = responseType.registerPriceDepth;
            //                            infoClassObj = Tools.getSimpleResponseInfo(body);
            //                            break;

            //                        case (int)PriceManager.cmdServer.getLongPriceDepth:
            //                        case (int)PriceManager.cmdServer.longPriceDepthPush:
            //                            resp = responseType.getLongPriceDepth;
            //                            {
            //                                infoClass.LongPriceDepth infoClassGot;
            //                                if (TradeStationComm.PriceManager.getResponseLongPriceDepth(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;

            //                        case (int)PriceManager.cmdServer.registerLongPriceDepth:
            //                            resp = responseType.registerLongPriceDepth;
            //                            infoClassObj = Tools.getSimpleResponseInfo(body);
            //                            break;

            //                        case (int)PriceManager.cmdServer.getProductList:
            //                            resp = responseType.getProductList;
            //                            {
            //                                infoClass.ProductList infoClassGot;
            //                                if (TradeStationComm.PriceManager.getResponseProductList(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;
            //                        case (int)PriceManager.cmdServer.getInstrumentList:
            //                            resp = responseType.getInstrumentList;
            //                            {
            //                                infoClass.InstrumentList infoClassGot;
            //                                if (TradeStationComm.PriceManager.getResponseInstrumentList(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;
            //                    }
            //                    break;
            //                #endregion

            //                #region Account Manager Response
            //                case Header.DST.accountManager:
            //                    switch (cmdGot)
            //                    {
            //                        case (int)AccountManager.cmdServer.login:
            //                            resp = responseType.login;
            //                            infoClassObj = Tools.getSimpleResponseInfo(body);
            //                            break;

            //                        case (int)AccountManager.cmdServer.logout:
            //                            resp = responseType.logout;
            //                            infoClassObj = Tools.getSimpleResponseInfo(body);
            //                            break;

            //                        case (int)AccountManager.cmdServer.changePassword:
            //                            resp = responseType.changePassword;
            //                            infoClassObj = Tools.getSimpleResponseInfo(body);
            //                            break;

            //                        case (int)AccountManager.cmdServer.ChangeLanguage:
            //                            resp = responseType.ChangeLanguage;
            //                            infoClassObj = Tools.getSimpleResponseInfo(body);
            //                            break;

            //                        case (int)AccountManager.cmdServer.getAccountInfo:
            //                            resp = responseType.getAccountInfo;
            //                            {
            //                                infoClass.AccountInfo infoClassGot;
            //                                if (TradeStationComm.AccountManager.getResponseAccountInfo(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;

            //                        case (int)AccountManager.cmdServer.getOrderBookInfo:
            //                            resp = responseType.getOrderBookInfo;
            //                            {
            //                                infoClass.OrderBook infoClassGot;
            //                                if (TradeStationComm.AccountManager.getResponseOrderBook(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;

            //                        case (int)AccountManager.cmdServer.getPositionInfo:
            //                            resp = responseType.getPositionInfo;
            //                            {
            //                                infoClass.Position infoClassGot;
            //                                if (TradeStationComm.AccountManager.getResponsePosition(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;

            //                        case (int)AccountManager.cmdServer.getPositionSummary:
            //                            resp = responseType.getPositionSummary;
            //                            {
            //                                infoClass.PositionSummary infoClassGot;
            //                                if (TradeStationComm.AccountManager.getResponsePositionSummary(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;

            //                        case (int)AccountManager.cmdServer.getCashInfo:
            //                            resp = responseType.getCashInfo;
            //                            {
            //                                infoClass.CashInfo infoClassGot;
            //                                if (TradeStationComm.AccountManager.getResponseCashInfo(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;

            //                        case (int)AccountManager.cmdServer.getDoneTradeInfo:
            //                        case (int)AccountManager.cmdServer.doneTradeInfoPush:
            //                            resp = responseType.getDoneTradeInfo;
            //                            {
            //                                infoClass.DoneTrade infoClassGot;
            //                                if (TradeStationComm.AccountManager.getResponseDoneTradeInfo(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;

            //                        case (int)AccountManager.cmdServer.getClearTradeInfo:
            //                            resp = responseType.getClearTradeInfo;
            //                            {
            //                                infoClass.ClearTrade infoClassGot;
            //                                if (TradeStationComm.AccountManager.getResponseClearTradeInfo(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;

            //                        case (int)AccountManager.cmdServer.notificationMsg:
            //                            resp = responseType.notificationMsg;
            //                            {
            //                                infoClass.NotificationInfo infoClassGot;
            //                                if (TradeStationComm.AccountManager.getResponseNotificationMsg(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;

            //                    }
            //                    break;
            //                #endregion

            //                #region Order Manager Response
            //                case Header.DST.orderManager:
            //                    switch (cmdGot)
            //                    {
            //                        case (int)OrderManager.cmdServer.addOrder:
            //                            resp = responseType.addOrder;
            //                            infoClassObj = Tools.getSimpleResponseInfo(body);
            //                            break;
            //                        case (int)OrderManager.cmdServer.changeOrder:
            //                            resp = responseType.changeOrder;
            //                            infoClassObj = Tools.getSimpleResponseInfo(body);
            //                            break;
            //                        case (int)OrderManager.cmdServer.deleteOrder:
            //                            resp = responseType.deleteOrder;
            //                            infoClassObj = Tools.getSimpleResponseInfo(body);
            //                            break;
            //                        case (int)OrderManager.cmdServer.activateOrder:
            //                            resp = responseType.activateOrder;
            //                            infoClassObj = Tools.getSimpleResponseInfo(body);
            //                            break;
            //                        case (int)OrderManager.cmdServer.inactivateOrder:
            //                            resp = responseType.inactivateOrder;
            //                            infoClassObj = Tools.getSimpleResponseInfo(body);
            //                            break;
            //                    }
            //                    break;
            //                #endregion

            //                #region Trade Manager Response
            //                case Header.DST.tradeManager:
            //                    if (cmdGot == (int)TradeManager.cmdServer.tickerPush)
            //                    {
            //                        resp = responseType.getTicker;
            //                        infoClass.Ticker infoClassGot;
            //                        if (TradeStationComm.TradeManager.getResponseTicker(body, out infoClassGot))
            //                        {
            //                            infoClassObj = infoClassGot;
            //                        }
            //                    }
            //                    //break;
            //                   switch (cmdGot)
            //                    {
            //                        case (int)TradeManager.cmdServer.getTicker:
            //                        case (int)TradeManager.cmdServer.tickerPush:
            //                            resp = responseType.getTicker;
            //                            {
            //                                infoClass.Ticker infoClassGot;
            //                                if (TradeStationComm.TradeManager.getResponseTicker(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;

            //                        case (int)TradeManager.cmdServer.registerTicker:
            //                            resp = responseType.registerTicker;
            //                            infoClassObj = Tools.getSimpleResponseInfo(body);
            //                            break;

            //                        case (int)TradeManager.cmdServer.getTradeStatistics:
            //                            resp = responseType.getTradeStatistics;
            //                            {
            //                                infoClass.TradeStatistics infoClassGot;
            //                                if (TradeStationComm.TradeManager.getResponseTradeStatistics(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;

            //                        case (int)TradeManager.cmdServer.getChart:
            //                            resp = responseType.getChart;
            //                            {
            //                                infoClass.Chart infoClassGot;
            //                                if (TradeStationComm.TradeManager.getResponseChart(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;
            //                    }
            //                    break;
            //                #endregion

            //            }

            //        }
            //    }

            //    return new ResponseObject(resp, infoClassObj);
            //}

            ///// <summary>
            ///// return ResponseObject, cast to appropriate infoClass
            ///// </summary>
            ///// <param name="respMsg"></param>
            ///// <param name="infoClassObj"></param>
            ///// <returns></returns>
            //public static ResponseObject getResponseAnyMsg(String respMsg)
            //{
            //    byte[] byteArray = Encoding.ASCII.GetBytes(respMsg);
            //    MemoryStream stream = new MemoryStream(byteArray);
            //    return getResponseAnyMsg(stream);
            //}

            /// <summary>
            /// return ResponseObject, cast to appropriate infoClass
            /// </summary>
            /// <param name="respMsg"></param>
            /// <param name="infoClassObj"></param>
            /// <returns></returns>
            public static ResponseObject getResponseAnyMsg(MemoryStream respMsg)
            {
                infoClass.infoObjectAbs infoClassObj = null;
                responseType resp = responseType.NIL;
                responseInfo theResponseInfo = null;

                byte[] header;
                String body;
                if (TradeStationComm.getMsg(respMsg, out header, out body))
                {
                    //Debug.WriteLine(body);
                    DebugLog("---GET:", body);

                    byte cmdGot;
                    TradeStationComm.Header.DST managerGot = TradeStationComm.getMsgManager(header, out cmdGot);
                    if (cmdGot == (int)PriceManager.cmdServer.errorMsg)
                    {
                        //resp = responseType.errorMsg;
                        //String outString;
                        //Tools.getErrorResponse(body, out outString);
                        //infoClassObj = outString;
                        resp = responseType.errorMsg;
                        infoClassObj = Tools.getErrorResponseInfo(body);
                    }
                    else
                    {
                        //Debug.WriteLine(managerGot.ToString() + " " + cmdGot.ToString());
                        DebugLog("     ", managerGot.ToString() + " " + cmdGot.ToString());

                        bool isInfoGot = false;

                        switch (managerGot)
                        {
                            #region Price Manager Response
                            case Header.DST.priceManager:
                                QCLog("   --p-" + cmdGot + "-", Enum.GetName(typeof(PriceManager.cmdServer), cmdGot));
                                switch (cmdGot)
                                {
                                    case (int)PriceManager.cmdServer.getMarketPrice:
                                    case (int)PriceManager.cmdServer.marketPricePush:
                                        resp = responseType.getMarketPrice;
                                        {
                                            infoClass.MarketPrice infoClassGot;
                                            if (TradeStationComm.PriceManager.getResponseMarketPrice(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        // TradeStationLog.WriteForCheckMarketPrice(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + "1---getMarketPrice: " + body);

                                        break;

                                    case (int)PriceManager.cmdServer.registerMarketPrice:
                                        resp = responseType.registerMarketPrice;
                                        infoClassObj = Tools.getSimpleResponseInfo(body);
                                        if (infoClassObj != null)
                                            isInfoGot = true;
                                        break;

                                    case (int)PriceManager.cmdServer.getPriceDepth:
                                    case (int)PriceManager.cmdServer.priceDepthPush:
                                        resp = responseType.getPriceDepth;
                                        {
                                            infoClass.PriceDepth infoClassGot;
                                            if (TradeStationComm.PriceManager.getResponsePriceDepth(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)PriceManager.cmdServer.registerPriceDepth:
                                        resp = responseType.registerPriceDepth;
                                        infoClassObj = Tools.getSimpleResponseInfo(body);
                                        if (infoClassObj != null)
                                            isInfoGot = true;
                                        break;

                                    case (int)PriceManager.cmdServer.getLongPriceDepth:
                                    case (int)PriceManager.cmdServer.longPriceDepthPush:
                                        resp = responseType.getLongPriceDepth;
                                        {
                                            infoClass.LongPriceDepth infoClassGot;
                                            if (TradeStationComm.PriceManager.getResponseLongPriceDepth(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)PriceManager.cmdServer.registerLongPriceDepth:
                                        resp = responseType.registerLongPriceDepth;
                                        infoClassObj = Tools.getSimpleResponseInfo(body);
                                        if (infoClassObj != null)
                                            isInfoGot = true;
                                        break;

                                    case (int)PriceManager.cmdServer.getProductList:
                                        resp = responseType.getProductList;
                                        {
                                            infoClass.ProductList infoClassGot;
                                            if (TradeStationComm.PriceManager.getResponseProductList(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;
                                    case (int)PriceManager.cmdServer.getInstrumentList:
                                        resp = responseType.getInstrumentList;
                                        {
                                            infoClass.InstrumentList infoClassGot;
                                            if (TradeStationComm.PriceManager.getResponseInstrumentList(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)PriceManager.cmdServer.getTicker:
                                    case (int)PriceManager.cmdServer.tickerPush:
                                        resp = responseType.getTicker;
                                        {
                                            infoClass.Ticker infoClassGot;
                                            if (TradeStationComm.PriceManager.getResponseTicker(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)PriceManager.cmdServer.registerTicker:
                                        resp = responseType.registerTicker;
                                        infoClassObj = Tools.getSimpleResponseInfo(body);
                                        if (infoClassObj != null)
                                            isInfoGot = true;
                                        break;

                                    case (int)PriceManager.cmdServer.getChart:
                                        resp = responseType.getChart;
                                        {
                                            infoClass.Chart infoClassGot;
                                            if (TradeStationComm.PriceManager.getResponseChart(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                }
                                break;
                            #endregion

                            #region Account Manager Response
                            case Header.DST.accountManager:
                                QCLog("   a---" + cmdGot + "-", Enum.GetName(typeof(AccountManager.cmdServer), cmdGot));
                                switch (cmdGot)
                                {
                                    case (int)AccountManager.cmdServer.login:
                                        resp = responseType.login;
                                        //infoClassObj = Tools.getSimpleResponseInfo(body);
                                        {
                                            infoClass.LoginResponse infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseLogin(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.logout:
                                        resp = responseType.logout;
                                        infoClassObj = Tools.getSimpleResponseInfo(body);
                                        if (infoClassObj != null)
                                            isInfoGot = true;
                                        break;

                                    case (int)AccountManager.cmdServer.changePassword:
                                        resp = responseType.changePassword;
                                        infoClassObj = Tools.getSimpleResponseInfo(body);
                                        if (infoClassObj != null)
                                            isInfoGot = true;
                                        break;

                                    case (int)AccountManager.cmdServer.ChangeLanguage:
                                        resp = responseType.ChangeLanguage;
                                        infoClassObj = Tools.getSimpleResponseInfo(body);
                                        if (infoClassObj != null)
                                        {
                                            isInfoGot = true;
                                            //xInfoLang = changeLang;
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.getAccountInfo:
                                        resp = responseType.getAccountInfo;
                                        {
                                            infoClass.AccountInfo infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseAccountInfo(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.getOrderBookInfo:
                                        resp = responseType.getOrderBookInfo;
                                        {
                                            infoClass.OrderBook infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseOrderBook(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.getPositionInfo:
                                        resp = responseType.getPositionInfo;
                                        {
                                            infoClass.Position infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponsePosition(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.getPositionSummary:
                                        resp = responseType.getPositionSummary;
                                        {
                                            infoClass.PositionSummary infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponsePositionSummary(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.getCashInfo:
                                        resp = responseType.getCashInfo;
                                        {
                                            infoClass.CashInfo infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseCashInfo(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.getDoneTradeInfo:
                                    case (int)AccountManager.cmdServer.doneTradeInfoPush:
                                        resp = responseType.getDoneTradeInfo;
                                        {
                                            infoClass.DoneTrade infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseDoneTradeInfo(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.getClearTradeInfo:
                                        resp = responseType.getClearTradeInfo;
                                        {
                                            infoClass.ClearTrade infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseClearTradeInfo(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.getAccList:
                                        resp = responseType.getAccList;
                                        {
                                            infoClass.AccList infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseGetAccList(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.getMarginCheck:
                                        resp = responseType.getMarginCheck;
                                        {
                                            infoClass.MarginCheck infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseGetMarginCheck(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.getMarginCallList:
                                        resp = responseType.getMarginCallList;
                                        {
                                            infoClass.MarginCallList infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseGetMarginCallList(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.reqCashApproval:
                                        resp = responseType.reqCashApproval;
                                        infoClassObj = Tools.getSimpleResponseInfo(body);
                                        if (infoClassObj != null)
                                            isInfoGot = true;
                                        break;

                                    case (int)AccountManager.cmdServer.getCashApprovalInfo:
                                        resp = responseType.getCashApprovalInfo;
                                        infoClassObj = Tools.getSimpleResponseInfo(body);
                                        if (infoClassObj != null)
                                            isInfoGot = true;
                                        break;


                                    case (int)AccountManager.cmdServer.getTradeConfOrders:
                                        resp = responseType.getTradeConfOrders;
                                        {
                                            infoClass.TradeConfOrder infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseGetTradeConfOrders(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.getTradeConfTrades:
                                        resp = responseType.getTradeConfTrades;
                                        {
                                            infoClass.TradeConfTrade infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseGetTradeConfTrades(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.reportTradeConf:
                                        resp = responseType.reportTradeConf;
                                        {
                                            infoClass.ReportTradeConfAck infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseReportTradeConf(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.getAccountMaster:
                                        resp = responseType.getAccountMaster;
                                        {
                                            infoClass.AccountMaster infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseGetAccountMaster(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.getTradeConfOrderDetail:
                                        resp = responseType.getTradeConfOrderDetail;
                                        {
                                            infoClass.TradeConfOrderDetail infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseGetTradeConfOrderdetail(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.notificationMsg:
                                        resp = responseType.notificationMsg;
                                        {
                                            infoClass.NotificationInfo infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseNotificationMsg(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;

                                    case (int)AccountManager.cmdServer.tableNotification:
                                        resp = responseType.tableNotification;
                                        {
                                            infoClass.TableNotificationInfo infoClassGot;
                                            if (TradeStationComm.AccountManager.getResponseTableNotificationMsg(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                                isInfoGot = true;
                                            }
                                        }
                                        break;


                                }
                                break;
                            #endregion

                            #region Order Manager Response
                            case Header.DST.orderManager:
                                QCLog("   -o--" + cmdGot + "-", Enum.GetName(typeof(OrderManager.cmdServer), cmdGot));
                                switch (cmdGot)
                                {
                                    case (int)OrderManager.cmdServer.addOrder:
                                        resp = responseType.addOrder;
                                        infoClassObj = Tools.getSimpleResponseInfo(body);
                                        if (infoClassObj != null)
                                            isInfoGot = true;
                                        break;
                                    case (int)OrderManager.cmdServer.changeOrder:
                                        resp = responseType.changeOrder;
                                        infoClassObj = Tools.getSimpleResponseInfo(body);
                                        if (infoClassObj != null)
                                            isInfoGot = true;
                                        break;
                                    case (int)OrderManager.cmdServer.deleteOrder:
                                        resp = responseType.deleteOrder;
                                        infoClassObj = Tools.getSimpleResponseInfo(body);
                                        if (infoClassObj != null)
                                            isInfoGot = true;
                                        break;
                                    case (int)OrderManager.cmdServer.activateOrder:
                                        resp = responseType.activateOrder;
                                        infoClassObj = Tools.getSimpleResponseInfo(body);
                                        if (infoClassObj != null)
                                            isInfoGot = true;
                                        break;
                                    case (int)OrderManager.cmdServer.inactivateOrder:
                                        resp = responseType.inactivateOrder;
                                        infoClassObj = Tools.getSimpleResponseInfo(body);
                                        if (infoClassObj != null)
                                            isInfoGot = true;
                                        break;
                                }
                                break;
                            #endregion

                            #region Trade Manager Response
                            case Header.DST.tradeManager:
                                //if (cmdGot == (int)TradeManager.cmdServer.tickerPush)
                                //{
                                //    resp = responseType.getTicker;
                                //    infoClass.Ticker infoClassGot;
                                //    if (TradeStationComm.TradeManager.getResponseTicker(body, out infoClassGot))
                                //    {
                                //        infoClassObj = infoClassGot;
                                //    }
                                //}
                                //break;
                                switch (cmdGot)
                                {
                                    //case (int)TradeManager.cmdServer.getTicker:
                                    //case (int)TradeManager.cmdServer.tickerPush:
                                    //    resp = responseType.getTicker;
                                    //    {
                                    //        infoClass.Ticker infoClassGot;
                                    //        if (TradeStationComm.TradeManager.getResponseTicker(body, out infoClassGot))
                                    //        {
                                    //            infoClassObj = infoClassGot;
                                    //        }
                                    //    }
                                    //    break;

                                    //case (int)TradeManager.cmdServer.registerTicker:
                                    //    resp = responseType.registerTicker;
                                    //    infoClassObj = Tools.getSimpleResponseInfo(body);
                                    //    break;

                                    case (int)TradeManager.cmdServer.getTradeStatistics:
                                        resp = responseType.getTradeStatistics;
                                        {
                                            infoClass.TradeStatistics infoClassGot;
                                            if (TradeStationComm.TradeManager.getResponseTradeStatistics(body, out infoClassGot))
                                            {
                                                infoClassObj = infoClassGot;
                                            }
                                        }
                                        break;

                                    //case (int)TradeManager.cmdServer.getChart:
                                    //    resp = responseType.getChart;
                                    //    {
                                    //        infoClass.Chart infoClassGot;
                                    //        if (TradeStationComm.TradeManager.getResponseChart(body, out infoClassGot))
                                    //        {
                                    //            infoClassObj = infoClassGot;
                                    //        }
                                    //    }
                                    //    break;
                                }
                                break;
                            #endregion

                            //add by ben 14012012
                            #region HeartBeat
                            case Header.DST.heartBeat:
                                resp = responseType.heartbeat;
                                infoClassObj = Tools.getSimpleResponseInfo(body);
                                if (infoClassObj != null)
                                    isInfoGot = true;
                                break;
                            #endregion

                        }

                        if (!isInfoGot)
                            QCLog("     xxxx:", body);
                    }
                }
                else
                {
                    if (header != null && header.Length > Header.offsetSOH)
                    {
                        if (header[Header.offsetSOH] != Header.SOH)
                            resp = responseType.packetErrNotSOH;
                    }
                }

                if (infoClassObj != null)
                    theResponseInfo = infoClassObj.GetResponseInfo();
                return new ResponseObject(resp, infoClassObj, theResponseInfo);
            }

            //public enum pushResponse
            //{
            //    NIL,
            //    marketPrice,
            //    priceDepth,
            //    longPriceDepth,
            //    doneTradeInfo,
            //    ticker,
            //    errorMsg,
            //}

            //public static pushResponse getResponsePushMessage(String respMsg, out object infoClassObj)
            //{
            //    infoClassObj = null;
            //    pushResponse resp = pushResponse.NIL;

            //    byte[] header;
            //    String body;
            //    if (TradeStationComm.getMsg(respMsg, out header, out body))
            //    {
            //        byte cmdGot;
            //        TradeStationComm.Header.DST managerGot = TradeStationComm.getMsgManager(header, out cmdGot);
            //        if (cmdGot == (int)PriceManager.cmdServer.errorMsg)
            //        {
            //            resp = pushResponse.errorMsg;
            //            String outString;
            //            Tools.getErrorResponse(respMsg, out outString);
            //            infoClassObj = outString;
            //        }
            //        else
            //        {
            //            switch (managerGot)
            //            {
            //                case Header.DST.priceManager:
            //                    switch (cmdGot)
            //                    {
            //                        case (int)PriceManager.cmdServer.marketPricePush:
            //                            resp = pushResponse.marketPrice;
            //                            {
            //                                infoClass.MarketPrice infoClassGot;
            //                                if (TradeStationComm.PriceManager.getResponseMarketPrice(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;
            //                        case (int)PriceManager.cmdServer.priceDepthPush:
            //                            resp = pushResponse.priceDepth;
            //                            {
            //                                infoClass.PriceDepth infoClassGot;
            //                                if (TradeStationComm.PriceManager.getResponsePriceDepth(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;
            //                        case (int)PriceManager.cmdServer.longPriceDepthPush:
            //                            resp = pushResponse.longPriceDepth;
            //                            {
            //                                infoClass.LongPriceDepth infoClassGot;
            //                                if (TradeStationComm.PriceManager.getResponseLongPriceDepth(body, out infoClassGot))
            //                                {
            //                                    infoClassObj = infoClassGot;
            //                                }
            //                            }
            //                            break;
            //                    }
            //                    break;

            //                case Header.DST.accountManager:
            //                    if (cmdGot == (int)AccountManager.cmdServer.doneTradeInfoPush)
            //                    {
            //                        resp = pushResponse.doneTradeInfo;
            //                        infoClass.DoneTrade infoClassGot;
            //                        if (TradeStationComm.AccountManager.getResponseDoneTradeInfo(body, out infoClassGot))
            //                        {
            //                            infoClassObj = infoClassGot;
            //                        }
            //                    }
            //                    break;

            //                case Header.DST.tradeManager:
            //                    if (cmdGot == (int)TradeManager.cmdServer.tickerPush)
            //                    {
            //                        resp = pushResponse.ticker;
            //                        infoClass.Ticker infoClassGot;
            //                        if (TradeStationComm.TradeManager.getResponseTicker(body, out infoClassGot))
            //                        {
            //                            infoClassObj = infoClassGot;
            //                        }
            //                    }
            //                     break;
            //            }

            //        }
            //    }

            //    return resp;
            //}
        }
        #endregion

        //public enum cmdServerToClient
        //{
        //    priceManagerGetMarketPrice,
        //    priceManagerRegisterMarketPrice,
        //    priceManagerGetPriceDepth,
        //    priceManagerRegisterPriceDepth,
        //    priceManagerGetLongPriceDepth,
        //    priceManagerRegisterLongPriceDepth,
        //    priceManagerGetProductList,
        //    priceManagerGetnstrumentList,
        //    priceManagerMarketPricePush,
        //    priceManagerPriceDepthPush,
        //    priceManagerLongPriceDepthPush,
        //    priceManagerErrorMsg,
        //} 
    }
}