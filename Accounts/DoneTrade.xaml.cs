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
using WPF.MDI;
using System.ComponentModel;

namespace GOSTS.Accounts
{
    /// <summary>
    /// DoneTrade.xaml 的交互逻辑
    /// </summary>
    public partial class DoneTrade : UserControl, GOSTS.IChangeLang 
    {
        public MdiChild GosMdiChild { get; set; }
        GOSTS.Common.MessageDistribute msgDistribute;
        DoneTradeSearchModel SearchModel = new DoneTradeSearchModel();
        DateTime lastSearchTime;

        public DoneTrade(MdiChild _GosMdiChild, GOSTS.Common.MessageDistribute _msgDistribute)
        {
            InitializeComponent();
            GosMdiChild = _GosMdiChild;
            GosMdiChild.Title = strRs_DoneTradeTitle;           
            SearchModel.DtDone = DateTime.Now;

            this.DataContext = SearchModel;
           // this.KeyDown += new KeyEventHandler(DoneTrade_KeyDown);
            msgDistribute = _msgDistribute;
            ParamCommon parm = new ParamCommon();
            this.btnSearch.Click += new RoutedEventHandler(btnSearch_Click);
            Search();
        }

        //void DoneTrade_KeyDown(object sender, KeyEventArgs e)
        //{            
        //    if (e.Key == System.Windows.Input.Key.Enter)
        //    {
        //        Search();
        //    }
        //}

        void btnSearch_Click(object sender, RoutedEventArgs e)
        {            
            Search();
        }

        void Search()
        {
            this.btnSearch.IsEnabled = false;
            lastSearchTime = DateTime.Now;
            ParamCommon param = new ParamCommon();
            param.CallBack = asynAction;
            param.dtRequestTime = DateTime.Now;
            asyTaskManager.AddReqTask(param);
        }


        void asynAction(DateTime dt)
        {
            if (dt == null) return;
            if (lastSearchTime > dt)
            {
                return;
            }
            if (SearchModel.DtDone.HasValue == false)
            {
                SearchModel.DtDone = DateTime.Now;
            }
            dasResult dResult = dasResult.normal;
            DataSet ds = DHelper.ReadDTR(this.SearchModel.Acc, SearchModel.DtDone, ref dResult);
            if (dResult == dasResult.nostr)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    MaskMsg.ShowMsgOnDataGrid(this.gridContainer, "can not connect to db,please check and restart app");
                    //Canvas UIMask = new Canvas();
                    //UIMask.Width = 200;
                    //UIMask.Height = 200;
                    //TextBlock tbMsg = new TextBlock();
                    //tbMsg.Text = "can not connect to db,please check and restart app";
                    //tbMsg.Foreground = Brushes.Red;
                    //tbMsg.FontSize = 30;
                    //UIMask.Children.Add(tbMsg);
                
                    //Grid.SetRowSpan(UIMask, gridContainer.RowDefinitions.Count);
                    //gridContainer.Children.Add(UIMask);                    
                  
                }), null);
                return;               
               // MessageBox.Show("can not connect to db");
            }
            if (ds == null) return;
            if (ds.Tables.Count < 1) return;            
            Dispatcher.Invoke((Action)(() =>
            {
                if (lastSearchTime > dt)
                {
                    return;
                }
                this.dgOHist.ItemsSource = ds.Tables[0].DefaultView;
                this.btnSearch.IsEnabled = true;
            }),
            null);
        }

        string strRs_DoneTradeTitle
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("DoneTradeTitle"); }
        }

        public void ChangLangInRuntime()
        {
            if (GosMdiChild != null)
            {
                GosBzTool.setTitle(GosMdiChild, msgDistribute, strRs_DoneTradeTitle);
            }
        }
    }

    public class DoneTradeSearchModel : INotifyPropertyChanged
    {
        private string _Acc;
        public string Acc
        {
            get { return _Acc; }
            set { _Acc = value; OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Acc")); }
        }

        private DateTime? _Date;
        public DateTime? DtDone
        {
            get { return _Date; }
            set { _Date = value; OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("DtDone")); }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, (propertyName));
            }
        }
        #endregion
    }

    public class ParamCommon
    {
        public DateTime dtRequestTime;       
        public Action<DateTime> CallBack { get; set; }
        public object Data { get; set; }
    }

    public class asyTaskManager
    {
        private static asyTaskManager Manager;
        static asyTaskManager()
        {
            StartScan();
        }
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
                t.Abort();
            }
            catch (Exception ex)
            {}
            t = null;
            bStart = false;
            bStop = true;
            ls.Clear();
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
        static List<ParamCommon> ls = new List<ParamCommon>();

        /* Adding a request task of reading db*/
        public static void AddReqTask(ParamCommon param)
        {
            if (param == null) return;
            if (param.dtRequestTime == null) return;           
            if (param.CallBack == null) return;

            StartScan();
            try
            {
                ls.Add(param);
            }
            catch (Exception ex)
            {
                TradeStationLog.WriteError("can not get lock to add request for reading DT" + ex);
            }
            finally
            {
                //  Monitor.Exit(objLock);
            }
           
        }

        public static void ProcessReqTask()
        {
            ParamCommon param = null;
            //if (Monitor.TryEnter(objLock))
            {
                try
                {
                    if (ls.Count > 0)
                    {
                        ParamCommon p1 = ls[0];
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
                    TradeStationLog.WriteError("can not get lock to remove request for reading DT," + ex);
                }
                finally
                {
                    //  Monitor.Exit(objLock);
                }
            }
            if (param != null)
            {
                if (param.CallBack != null)
                {
                    param.CallBack(param.dtRequestTime);                  
                }
            }
            else
            {

            }
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
}
