#define IsDelearVersion
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
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using WPF.MDI;

namespace GOSTS.Accounts
{
    /// <summary>
    /// OrderHistory.xaml 的交互逻辑
    /// </summary>
    public partial class OrderHistory : UserControl,GOSTS.IChangeLang 
    {
        public MdiChild GosMdiChild { get; set; }
        GOSTS.Common.MessageDistribute msgDistribute;
        public string AccNo;
        public string OrderNo;

        public OrderHistory(MdiChild _GosMdiChild, string _AccNo, string _OrderNo, GOSTS.Common.MessageDistribute _msgDistribute)
        {
            InitializeComponent();
            GosMdiChild = _GosMdiChild;
            GosMdiChild.Title = strRs_OHistTitle;
            AccNo = _AccNo;
            OrderNo = _OrderNo;
            this.tbAccNo.Text = AccNo;
            this.tbOrderNo.Text = OrderNo;

            msgDistribute = _msgDistribute;

            OHistParm parm=new OHistParm();
            parm.AccNo=AccNo;
            parm.OrderNo=OrderNo;
            parm.RecDataCallBack = BindOHist;
            OrderHistManager.AddReqTask(parm);
        }

        void BindOHist(DataSet ds,string AccNo,string OrderNo,dasResult dasRult)
        {
            if (dasRult == dasResult.nostr)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    MaskMsg.ShowMsgOnDataGrid(this.gdContainer, "can not connect to db,please check and restart app");
                }), null);               
                return;
            }
            if (ds == null) return;
            if (ds.Tables.Count < 1) return;

            Dispatcher.Invoke((Action)(() =>
            {
                this.dgOHist.ItemsSource = ds.Tables[0].DefaultView; 
            }),              
            null);
        }

        public void ChangLangInRuntime()
        {
            if (GosMdiChild != null)
            {
               // GosMdiChild.Title = strRs_OHistTitle;
                GosBzTool.setTitle(GosMdiChild, msgDistribute, strRs_OHistTitle);
            }
        }

        string strRs_OHistTitle
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("OHistTitle"); }
        }
    }










    public class OrderHistManager
    {
        private static OrderHistManager Manager;
        static OrderHistManager()
        { 
            StartScan(); 
        }

        //public static OrderHistManager getOrderHistManage()
        //{
        //    if (Manager == null)
        //    {
        //        Manager = new OrderHistManager();
                
        //    }
        //    return Manager;
        //}
        public static Thread t;
        public static bool bStop = false;
        static bool bStart = false;
        static void StartScan()
        {
            if (bStart == false)
            {
                bStart = true;
                bStop = false;
                t = new Thread(processTaskList);
                t.IsBackground = true;
                t.Start();
            }
        }

        public static void Reset()
        {
            try
            {
                if (t != null)
                {
                    try
                    {
                        t.Abort();
                    }
                    catch (Exception ex)
                    { }                    
                }
                bStop = true;
                bStart = false;
            }
            catch (Exception ex) { }
        }

        static void processTaskList()
        {
            while (!bStop)
            {
                try
                {
                    ProcessReqTask();
                }
                catch (Exception ex)
                {

                }
            }
        }      
        
        static object objLock = new object();
        //Dictionary<string, OHistParm> ls = new Dictionary<string, OHistParm>();
        static List<OHistParm> ls = new List<OHistParm>();

        /* Adding a request task of reading db*/
        public static void AddReqTask(OHistParm param) 
        {
            if (param == null) return;
            if (param.AccNo == null) return;
            if (param.OrderNo == null) return;
            if (param.AccNo.Trim() == "") return;
            if (param.OrderNo.Trim() =="") return;

            StartScan();
           // if (Monitor.TryEnter(objLock))
            {
                try
                {
                    ls.Add(param);
                }
                catch (Exception ex)
                {
                    TradeStationLog.WriteError("can not get lock to add request for reading orhist" + ex);
                }
                finally
                {
                  //  Monitor.Exit(objLock);
                }
            }
        }

        public static void ProcessReqTask()
        {
            OHistParm param = null;
       //     if (Monitor.TryEnter(objLock))
            {
                try
                {
                    if (ls.Count > 0)
                    {
                        OHistParm p1 = ls[0];
                        if (ls.Contains(p1))
                        {
                            ls.Remove(p1);
                        }
                        param = p1;
                    }
                }
                catch (Exception ex)
                {
                    param = null;
                    TradeStationLog.WriteError("can not get lock to remove request for reading orhist" + ex);
                }
                finally
                {
                  //  Monitor.Exit(objLock);
                }
            }
            if (param != null)
            {
                if (param.RecDataCallBack != null)
                {
                    dasResult rsInfo = dasResult.normal;
                    DataSet ds = ReadOrderHist(param,ref rsInfo);
                   // if (ds != null)
                    {
                        try
                        {
                            param.RecDataCallBack(ds, param.AccNo, param.OrderNo, rsInfo);
                        }
                        catch (Exception ex)
                        {
                            TradeStationLog.WriteError("render orhist error," + ex);
                        }
                    }
                }
            }
            else
            {
                
            }
        }



        public static DataSet ReadOrderHist(OHistParm parms, ref dasResult rsInfo)
        {
            rsInfo = dasResult.normal;
#if IsDelearVersion
            if (parms == null)
            {
                return null;
            }

            if (parms.AccNo == null)
            {
                return null;
            }

            if (parms.OrderNo == null)
            {
                return null;
            }

            string Acc_No = parms.AccNo;
            string Order = parms.OrderNo;
            string DNS = DHelper.GetCCS();
            if (DNS == "")
            {
                rsInfo = dasResult.nostr;
                return null;
            }
            string SQL = @"select top 100  dbo.UnixTStoDT(log_time)  as 'histTime',[user_id],acc_no,acc_order_no,[text] as detail 
                        from v_User_log where [acc_no]=@acc_no and acc_order_no=@acc_order_no";
            SqlConnection conn = new SqlConnection(DNS);
            try
            {
                SqlCommand cmd = new SqlCommand(SQL, conn);
                cmd.CommandType = CommandType.Text;
                SqlParameter param = new SqlParameter("@acc_no", SqlDbType.NVarChar, 16);
                param.Value = Acc_No;
                cmd.Parameters.Add(param);

                param = new SqlParameter("@acc_order_no", SqlDbType.NVarChar, 50);
                param.Value = Order;
                cmd.Parameters.Add(param);

                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                conn.Close();
                return ds;

            }
            catch (Exception ex)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
#endif
            return null;
        }    

        void tableScheme_forShow()
        {
            DataTable dt = new DataTable();
            DataColumn col = new DataColumn("tdTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add(col);

            col = new DataColumn("acc_no", System.Type.GetType("System.String"));
            dt.Columns.Add(col);

            col = new DataColumn("productCode", System.Type.GetType("System.DateTime"));
            dt.Columns.Add(col);

            col = new DataColumn("long", System.Type.GetType("System.Int32"));
            dt.Columns.Add(col);

            col = new DataColumn("short", System.Type.GetType("System.Int32"));
            dt.Columns.Add(col);

            col = new DataColumn("tradeNo", System.Type.GetType("System.String"));
            dt.Columns.Add(col);

            col = new DataColumn("orderNo", System.Type.GetType("System.String"));//Int32
            dt.Columns.Add(col);

            col = new DataColumn("GW", System.Type.GetType("System.String"));
            dt.Columns.Add(col);

            col = new DataColumn("Intiator", System.Type.GetType("System.String"));
            dt.Columns.Add(col);

            col = new DataColumn("ExtOrderNo", System.Type.GetType("System.String"));
            dt.Columns.Add(col);

            col = new DataColumn("Ref", System.Type.GetType("System.String"));
            dt.Columns.Add(col);

            col = new DataColumn("Channnel", System.Type.GetType("System.String"));
            dt.Columns.Add(col);
        }
        




       
    }

    public class OHistParm
    {
        public string OrderNo;
        public string AccNo;
        public Action<DataSet,string, string,dasResult> RecDataCallBack { get; set; }
    }

#if IsDelearVersion
    public class SymmetricMethod
    {
        private SymmetricAlgorithm mobjCryptoService;
        public string Key;
        ///// <summary>  
        ///// 对称加密类的构造函数 
        ///// </summary>
        public SymmetricMethod()
        {
            mobjCryptoService = new RijndaelManaged();
            Key = "Guz(%&hj7x89H$yuBI0456FtmaT5&fvHUFCy76*h%(HilJ$lhj!y6&(*jkP87jH7";
        }
        /// <summary> 
        /// 获得密钥 
        /// </summary>
        /// <returns>密钥</returns> 
        private byte[] GetLegalKey()
        {
            string sTemp = Key; mobjCryptoService.GenerateKey();
            byte[] bytTemp = mobjCryptoService.Key; int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength) sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength) sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /// <summary> 
        /// 获得初始向量IV  
        /// </summary>  
        /// <returns>初试向量IV</returns> 
        private byte[] GetLegalIV()
        {
            string sTemp = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";
            mobjCryptoService.GenerateIV();
            byte[] bytTemp = mobjCryptoService.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

        /// <summary> 
        /// 加密方法
        /// </summary>
        ///<param name="Source">待加密的串</param>  
        /// <returns>经过加密的串</returns> 
        public string Encrypto(string Source)
        {
            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);
            MemoryStream ms = new MemoryStream();
            mobjCryptoService.Key = GetLegalKey();
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(bytIn, 0, bytIn.Length);
            cs.FlushFinalBlock();
            ms.Close();
            byte[] bytOut = ms.ToArray();
            return Convert.ToBase64String(bytOut);
        }


        /// <summary> 
        ///  解密方法
        ///   </summary>  
        /// <param name="Source">待解密的串</param>  
        /// <returns>经过解密的串</returns> 
        public string Decrypto(string Source)
        {
            byte[] bytIn = Convert.FromBase64String(Source);
            MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
            mobjCryptoService.Key = GetLegalKey();
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
#endif







    public static class DHelper
    {
        static Dictionary<string, string> lsSvItems = new Dictionary<string, string>();
        public static string DbListFile ="";
        public static string DBListFileCode = "utf-8";
        public static string DBStr = "";

        static DHelper()
        {
           #if IsDelearVersion
             DbListFile =AppDomain.CurrentDomain.BaseDirectory.ToString() + @"\DS.txt";
             string dStrSplit = "-|>>|-";
             string DKey = "q1w2e3r4t5y6";
             List<string> ls = ReadLineList(DbListFile, DBListFileCode);
             foreach (string line in ls)
             {
                 if (line.Trim() != "")
                 {
                     try
                     {
                         SymmetricMethod sy = new SymmetricMethod();
                         sy.Key = DKey;
                         string str = sy.Decrypto(line);
                         int pos = str.IndexOf(dStrSplit);
                         if (pos < 0)
                         {
                             continue;
                         }

                         string k = str.Substring(0, pos);
                         string v = str.Substring(pos + dStrSplit.Length);
                         if (v.Trim() != "")
                         {
                             lsSvItems.Add(k, v);
                         }
                     }
                     catch { }
                 }
             }
           #endif
        }

        public static string GetCCS()
        {
#if IsDelearVersion           
            string str = getCCS(AppFlag.IPOMT);
            return str;
#endif
            return "";
        }

        public static string getCCS(string key)
        {        
            #if IsDelearVersion
            if (lsSvItems.ContainsKey(key))
            {
                return lsSvItems[key];
            }
            else
            {
                foreach (string link in lsSvItems.Values)
                {
                    if (link == null) continue;
                    if (link.Trim() == "") continue;
                    string[] arr = link.Split(';');
                    if (arr.Length < 1) continue;
                    string str = arr[0];
                    str = str.ToLower().Replace("server", "").Replace("=", "").Replace(" ", ""); ;
                    string key1 =key.ToLower().Replace(" ", "");// System.Text.RegularExpressions.Regex.Replace(key, @"\s+", "");
                    if (str == key1)
                    {
                        return link;
                    }
                }
            }
            return "";
            #endif
            return "";
        }

        public static DataSet ReadDTR(string acc, DateTime? dtTrade, ref dasResult intResult)
        {
            intResult = dasResult.normal;
            #if IsDelearVersion
            string DNS = GetCCS();
            if (DNS == "")
            {
                intResult = dasResult.nostr;
                return null;
            }
            string SQL = @"select * from v_done_trade_report
                Where  (IsNull(@Acc,'')='' Or acc_no=@Acc)                                           
                AND (STATUS<>20 AND STATUS<>120)
                And (@dtTrade is null or dbo.DTtoUnixTS(Convert(nvarchar(10),@dtTrade,120))<=trade_time)
                And (@dtTrade is null or dbo.DTtoUnixTS(DATEADD(d,1,Convert(nvarchar(10),@dtTrade,120)))>=trade_time)
                                            order by trade_time ";
            SqlConnection conn = new SqlConnection(DNS);
            try
            {
                SqlCommand cmd = new SqlCommand(SQL, conn);
                cmd.CommandType = CommandType.Text;
                SqlParameter param = new SqlParameter("@Acc", SqlDbType.NVarChar, 16);
                param.IsNullable = true;
                if(acc==null)
                {
                     param.Value =  DBNull.Value;
                }
                else{
                    param.Value = acc;
                }              
                cmd.Parameters.Add(param);

                param = new SqlParameter("@dtTrade", SqlDbType.DateTime, 50);
                param.IsNullable = true;
                if (dtTrade == null)
                {
                    param.Value = DBNull.Value;
                }
                else
                {
                    param.Value = dtTrade.Value;
                }
                cmd.Parameters.Add(param);
               
                conn.Open();// --- to catch open err  ***
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);                
                conn.Close();
                formatDoneTrade(ds);
                return ds;

            }
            catch (Exception ex)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            #endif
            return null;
        }



        static void formatDoneTrade(DataSet dsDoneTrade)
        {
            if (dsDoneTrade == null) return;
            if (dsDoneTrade.Tables.Count < 1) return;
            if (dsDoneTrade.Tables[0] == null) return;
            DataTable dt = dsDoneTrade.Tables[0];
            if (dt.Columns.Contains("TradePriceWithDec") == false)
            {
                DataColumn col = new DataColumn("TradePriceWithDec", System.Type.GetType("System.Decimal"));
                dt.Columns.Add(col);                
            }
            if (dt.Columns.Contains("ext_no_format") == false)
            {
                dt.Columns.Add(new DataColumn("ext_no_format"));
            }

            foreach (DataRow dr in dsDoneTrade.Tables[0].Rows)
            {
                string strExt = dr.getColValue("order_no");              
                strExt = ToGosHex(strExt);
                dr["ext_no_format"] = strExt;
               // =(FormatNumber(Fields!trade_price.Value/Fields!decpower.Value,Fields!dec_in_price.Value,0,0,0))
                decimal TradePrice = dr.getDecimalValue("trade_price",0M);
                int DecInPrice = dr.getColIntValue("dec_in_price",0);
                int DecPower = dr.getColIntValue("decpower", 1);

                decimal Price = 0M;
                if (TradePrice == 0M)
                {
                    dr["TradePriceWithDec"] = Price;
                    continue;
                }

                //if (DecPower == 1)
                //{
                //    dr["TradePriceWithDec"] = Price;
                //    continue;
                //}
                
                Price =TradePrice / DecPower;
                Price = Math.Round(Price, DecInPrice);
                dr["TradePriceWithDec"] = Price;
            }
        }


        public static string ToGosHex(string str)
        {
            try
            {
                string str1 = Convert.ToInt64(str).ToString("x");
                str1 = str1.PadLeft(16, '0');
                str1 = str1.Insert(8, ":");
                return str1;
            }
            catch
            {
                return str;
            }
        }

        public static List<string> ReadLineList(string filename, string encoding)
        {

            List<string> ls = new List<string>();
            if (File.Exists(filename))
            {
                try
                {
                    using (System.IO.StreamReader reader = new StreamReader(filename, System.Text.Encoding.GetEncoding(encoding)))
                    {
                        string s = reader.ReadLine();
                        while (s != null)
                        {
                            ls.Add(s);
                            s = reader.ReadLine();
                        }
                        reader.Close();
                    }
                }
                catch { }
            }
            return ls;
        }
    }

    public enum dasResult
    {
        normal=0,
        nostr=-1,
        cnetfail=-2
    }


    public class MaskMsg
    {
        public static void ShowMsgOnDataGrid(Grid gdparent,string Msg)
        {
            if (gdparent == null)
            {
                return;
            }
            //Border br = new Border();
            //br.BorderBrush = Brushes.Red;
            Canvas UIMask = new Canvas();
           // UIMask.Background = Brushes.Black; ;
           // UIMask.Opacity = 0;           
            TextBlock tbMsg = new TextBlock();
            tbMsg.Text = Msg;
            tbMsg.HorizontalAlignment = HorizontalAlignment.Center;
            tbMsg.VerticalAlignment = VerticalAlignment.Center;
            tbMsg.TextWrapping = TextWrapping.WrapWithOverflow;
            tbMsg.Foreground = Brushes.Red;
            tbMsg.Opacity = 1;
            tbMsg.FontSize = 20;
            UIMask.Children.Add(tbMsg);
            
            if (gdparent.ColumnDefinitions != null)
            {
                if (gdparent.ColumnDefinitions.Count > 0)
                {
                   Grid.SetColumnSpan(UIMask, gdparent.ColumnDefinitions.Count);
                }
                if (gdparent.RowDefinitions.Count > 0)
                {
                    Grid.SetRowSpan(UIMask, gdparent.RowDefinitions.Count);
                }               
            }
            
            double dpWidth = gdparent.ActualWidth;
            if (double.IsNaN(dpWidth))
            {
                dpWidth = gdparent.Width;
            }
            if (double.IsNaN(dpWidth))
            {
                dpWidth = 200;
            }

            double dpHeight = gdparent.ActualHeight;
            if (double.IsNaN(dpHeight))
            {
                dpHeight = gdparent.Height;
            }
            if (double.IsNaN(dpHeight))
            {
                dpHeight = 200;
            }
            Canvas.SetTop(tbMsg, 60);
            //UIMask.Width = dpWidth;
            //UIMask.Height = dpHeight;
            gdparent.Children.Add(UIMask);
          //  double left = (dpWidth - UIMask.Width)/2;
          //  double top=
            
          //  UIMask.po.Left = left;
            //this.AddChild(tbMsg);
          
        }
    }
}
