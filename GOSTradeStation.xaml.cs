using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using GOSTS.Common;
using GOSTS.WPFControls;
using GOSTS.Dealer;
using WPF.MDI;
using Microsoft.Windows.Controls.Ribbon;
using System.Xml;
using System.Xml.Linq;
using System.Collections;
using GOSTS.ViewModel;
using System.Collections.ObjectModel;
using GOSTS.Preference;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Media;
using GOSTS.WPFControls.Chart.SCI; 
using System.Windows.Media;
using GOSTSWindowsBox;

namespace GOSTS
{

    /// <summary>
    /// Interaction logic for GOSTS.xaml
    /// </summary> 
    public partial class GOSTradeStation : Window
    {
        #region property

        Thread threadOMT = null;
        Thread threadOMP = null;
        private static string _currentUICulture;
        private static string strVersion;
        public static string CurrentUICulture
        {
            get { return CultureInfo.CurrentUICulture.ToString().ToUpper(); }
            set
            {
                _currentUICulture = value;
            }
        }

        private static MessageChannel _msgChannel = null;
        public static MessageChannel msgChannel
        {
            get { return _msgChannel; }
            set { _msgChannel = value; }
        }

        private static MessageChannel _msgChannelOMP = null;
        public static MessageChannel msgChannelOMP
        {
            get { return _msgChannelOMP; }
            set { _msgChannelOMP = value; }
        }

        private static string _Chk_mrgin_opt = null;
        public static string Chk_mrgin_opt
        {
            get { return _Chk_mrgin_opt; }
            set { _Chk_mrgin_opt = value; }
        }

        private static string _UserID = null;
        public static string UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        private static string _Pwd = null;
        public static string Pwd
        {
            get { return _Pwd; }
            set { _Pwd = value; }
        }
        private static MessageDistribute _distributeMsg = null;
        public static MessageDistribute distributeMsg
        {
            get { return _distributeMsg; }
            set { _distributeMsg = value; }
        }

        private static MarketPriceData _marketPriceData = null;
        public static MarketPriceData marketPriceData
        {
            get { return _marketPriceData; }
            set { _marketPriceData = value; }
        }

        private static List<MarketPriceSentData> _marketPriceSentData = null;
        public static List<MarketPriceSentData> marketPriceSentData
        {
            get { return _marketPriceSentData; }
            set { _marketPriceSentData = value; }
        }

        public static UserData userData = null;

        private MdiChild mcLogin;
        delegate void BindListInfo(string str);
        BindListInfo InfoDelegate;

        private System.Timers.Timer timeOutTimerOMT;
        private static bool heartBeatStatusOMT;
        private System.Timers.Timer timeOutTimerOMP;
        private static bool heartBeatStatusOMP;
        public const bool isDealer = false; //Dealer or TradeSation
        private static bool _IsWindowInitialized = false; //Window initialized status
        public static bool IsWindowInitialized
        {
            get { return _IsWindowInitialized; }
            private set { _IsWindowInitialized = value; }
        }

        private static string DisclaimerText;
        private static bool PopupPwd;

        private static CustomizeData _customizeData;
        public static CustomizeData customizeData
        {
            get { return _customizeData; }
            private set { _customizeData = value; }
        }

        public static bool boolWindowOrderBook = false;

        private static string _AccountID = null;
        public static string AccountID
        {
            get { return _AccountID; }
            set { _AccountID = value; }
        }

        private static string _tw = null;
        public static string Tw
        {
            get { return _tw; }
            set { _tw = value; }
        }

        private static string _tc = null;
        public static string Tc
        {
            get { return _tc; }
            set
            {
                _tc = value;
            }
        }

        delegate void DisOrderConfirm(Notification notification, TableNotification tableNotification, ObservableCollection<MarginCheckAccData> data, OrderResponse orderResponse);//, string strDisText);
        DisOrderConfirm DisOrderConfirmDelegate;

        public static NotificationQueue<object> notifications;
        public static MdiContainer container;

        public static bool m_BeforeGet = false;  // (if true)will get/register MarketPrice data after get product list and instrument list 
        public static bool m_ChangedUser = false; //login the other username

        public static string deskFileName
        {
            private set;
            get;
        }

        public static bool isReLoaded
        {
            private set;
            get;
        }

        delegate void LoadDesktop();
        LoadDesktop LoadDesktopDelegate;

        public static PreferenceAccInputModel PrefAccInputModel = new PreferenceAccInputModel();


        string AlertTilte
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("mainAletTitle");
            }
        }

        string AlertLogout
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("mainAlertLogout");
            }
        }

        string AlertClose
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("mainAlertClose");
            }
        }
        string Disclaimer
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("mainDisclaimer");
            }
        }

        string SyncTimeMsg
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("mainSyncTimeMsg");
            }
        }

        string SyncTimeTitle
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("mainSyncTimeTitle");
            }
        }

        string SyncTimeAlert
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("mainSyncTimeAlert");
            }
        }

        string AddedNotificationMsg
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("notificationMsg");
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GOSTS"/> class.
        /// </summary>
        public GOSTradeStation()
        {
            try
            {
                InitializeComponent();

                if (_distributeMsg == null)
                {
                    _distributeMsg = new MessageDistribute();
                    _distributeMsg.DisControlFocus += new MessageDistribute.OnDisControlFocus(distributeMsg_ControlFocus);
                    _distributeMsg.DisControlChangeTitle += new MessageDistribute.OnDisControlChangeTitle(distributeMsg_ControlChangeTitle);
                    _distributeMsg.DisNotification += new MessageDistribute.OnDisNotification(distributeMsg_Notification);
                    _distributeMsg.DisGetError += distributeMsg_GetError;
                    _distributeMsg.DisMarginCallList += distributeMsg_MarginCallList;
                    _distributeMsg.DisAddOrder += new MessageDistribute.OnDisAddOrder(distributeMsg_AddOrderInfo);

                    //harry,order operation                
                    _distributeMsg.DisInactivateOrder += (distributeMsg_InActiveOrderfConfirmInfo);
                    _distributeMsg.DisActivateOrder += (distributeMsg_OPOrderfConfirmInfo);
                    _distributeMsg.DisDeleteOrder += distributeMsg_OPOrderfConfirmInfo;
                    _distributeMsg.DisChangeOrder += distributeMsg_OPOrderfConfirmInfo;

                    _distributeMsg.DisGotProductList += distributeMsg_DisDisGotProductList;
                    _distributeMsg.SyncTime += distributeMsg_SyncTime;

                    distributeMsg.DisPriceManager += distributeMsg_DisPriceManager;
                    distributeMsg.DisChangeLanguage += distributeMsg_DisChangeLanguage;
                }

                container = this.Container;

                if (_marketPriceData == null)
                {
                    _marketPriceData = new MarketPriceData();
                }
                if (_marketPriceSentData == null)
                {
                    _marketPriceSentData = new List<MarketPriceSentData>();
                }
                if (userData == null)
                {
                    userData = new UserData();
                }
                if (_customizeData == null)
                {
                    _customizeData = new CustomizeData();
                }

                InfoDelegate = new BindListInfo(BindListInfoMethod);
                DisOrderConfirmDelegate = new DisOrderConfirm(OpenOrderConfrim);

                this.RibbonMTabUseful.Visibility = Visibility.Hidden;
                this.RibbonMTabAcount.Visibility = Visibility.Hidden;
                this.RibbonMTabMarket.Visibility = Visibility.Hidden;
                this.RibbonMTabOrders.Visibility = Visibility.Hidden;
                this.RibbonMTabTrades.Visibility = Visibility.Hidden;
                this.RibbonTabTools.Visibility = Visibility.Hidden;
                this.RibbonPrefer.Visibility = Visibility.Hidden;
                this.RibbonDesktop.Visibility = Visibility.Hidden;
                this.RibbonHelp.Visibility = Visibility.Hidden;
                this.RibbonMTabDealer.Visibility = Visibility.Hidden;

                this.RibbonBTLogout.Visibility = Visibility.Hidden;
                this.RibbonBTChartAnalysisSCI.Visibility = Visibility.Hidden;
                this.RibbonBTChartAnalysisSCITradesTab.Visibility = Visibility.Hidden;

                if (GOSTS.AppFlag.EnabledRandomTest())
                {
                    if (AppFlag.bRobbin == false)
                    {
                        this.btnTestInst.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (AppFlag.bRobbin == false)
                    {
                        this.btnTestInst.Visibility = Visibility.Collapsed;
                    }
                }

                //harry 20140318
                if (AppFlag.bRobbin == false)
                {
                    switchShowMenu(false);
                }
                else
                {
                    this.gdTBMenu.Visibility = Visibility.Collapsed;
                    this.RibbonMain.Visibility = Visibility.Visible;
                }
                //2015-01-01
                if (GOSTradeStation.isDealer==false)
                {
                    this.MenuDoneTrade.Visibility = Visibility.Collapsed ;
                }

                timeOutTimerOMT = new System.Timers.Timer(AppFlag.TimeOutDelay);
                timeOutTimerOMT.Elapsed += new ElapsedEventHandler(OnTimeOutEvent);
                timeOutTimerOMT.Enabled = false;

                timeOutTimerOMP = new System.Timers.Timer(AppFlag.TimeOutDelay);
                timeOutTimerOMP.Elapsed += new ElapsedEventHandler(OnTimeOutEventOMP);
                timeOutTimerOMP.Enabled = false;
                strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();//.Substring(0, 3);
                _original_title = Title + strVersion + " " + AppFlag.BuildDate;
                SetMenuBG("FFFFFF");

                Container.Children.CollectionChanged += (o, e) => Menu_RefreshWindows();
                Container.MdiChildTitleChanged += Container_MdiChildTitleChanged;
                Container.Children.CollectionChanged += Children_CollectionChanged;
                msgQueen = new MsgQuee(this);
                msgQueen.initThread();

                LoadDesktopDelegate = new LoadDesktop(LoadDesktopWindows);
                notifications = new NotificationQueue<object>();
                notifications.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(notifications_CollectionChanged);

            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " GOSTradeStation.cs, Error:" + exp.Message.ToString());

            }
        }

        protected void distributeMsg_DisChangeLanguage(object sender, string language)
        {
            //Change language
            if (GOSTradeStation.marketPriceData == null || GOSTradeStation.marketPriceData.ProdListTable == null || GOSTradeStation.marketPriceData.InstListTable == null)
            {
                TradeStationSend.Send(cmdClient.getProductList);
                TradeStationSend.Send(cmdClient.getInstrumentList);

                //Clear register
                TradeStationSend.ClearRegister(cmdClient.registerMarketPrice);
                TradeStationSend.ClearRegister(cmdClient.registerPriceDepth);
                TradeStationSend.ClearRegister(cmdClient.registerTicker);
            }
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                // GosCulture.CultureHelper.ChangeLanguage(AppFlag.DefaultLanguage);
                checkLangMenuItem(AppFlag.DefaultLanguage);
                foreach (MdiChild mdiChild in Container.Children)
                {
                    string oldTitle = mdiChild.Title;
                    if (mdiChild.Content.GetType() != typeof(AccountInfo) && mdiChild.Content.GetType() != typeof(UserOrderInfo)
                         && mdiChild.Content.GetType() != typeof(ChangeOrder) && mdiChild.Content.GetType() != typeof(PosSumary)
                         && mdiChild.Content.GetType() != typeof(ClosePosition) && mdiChild.Content.GetType() != typeof(OrderConfirm)
                         && mdiChild.Content.GetType() != typeof(ChangePWD) && mdiChild.Content.GetType() != typeof(AccPreference)
                         && mdiChild.Content.GetType() != typeof(MsgForm) && mdiChild.Content.GetType() != typeof(OrderEntry)
                         && mdiChild.Content.GetType() != typeof(GOSTS.Accounts.OrderHistory) && mdiChild.Content.GetType() != typeof(GOSTS.Accounts.DoneTrade))
                    {
                        if (mdiChild.Content.GetType() == typeof(PriceDepth))
                        {
                            TradeStationSend.Get(((PriceDepth)mdiChild.Content).ProdCode, cmdClient.getPriceDepth);
                        }
                        else if (mdiChild.Content.GetType() == typeof(TradeConfirmation))
                        {
                            TradeStationSend.Send(cmdClient.getTradeConfOrders);
                            TradeStationSend.Send(cmdClient.getTradeConfTrades);
                        }
                        else if (mdiChild.Content.GetType() == typeof(MarginCheck))
                        {
                            TradeStationSend.Send(cmdClient.getMarginCheck);
                        }
                        else if (mdiChild.Content.GetType() == typeof(MarginCallList))
                        {
                            TradeStationSend.Send(cmdClient.getMarginCallList);
                        }
                        mdiChild.Title = TradeStationSetting.ChangeWindowLanguage(mdiChild);
                        this.distributeMsg_ControlChangeTitle(null, oldTitle, mdiChild);
                    }
                }
            }));
        }

        protected void distributeMsg_GetError(object sender, MessageException message)
        {
            TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " Error: " + message.Code + "/" + message.Message);
        }

        protected void distributeMsg_DisDisGotProductList(object sender)
        {
            m_BeforeGet = true;
            //this.InitWindows();
        }

        protected void distributeMsg_DisPriceManager(object sender, MarketPriceData data)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.InitWindows();
                // GosCulture.CultureHelper.ChangeLanguage(AppFlag.DefaultLanguage);                
            }));
        }

        protected void distributeMsg_MarginCallList(object sender, ObservableCollection<MarginCheckAccData> data)
        {
            Application.Current.Dispatcher.BeginInvoke(DisOrderConfirmDelegate, new Object[] { null, null, data, null });
        }

        #region  Initialization

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AccountLogin_Click(sender, e);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //Colse without alert when logout.
            if (!_IsWindowInitialized) return;

            if (MessageBox.Show(AlertClose,
                           AlertTilte, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (UnLoadLogout())
                {
                    if (msgQueen != null)
                    {
                        msgQueen.AbortThread();
                    }
                }
            }
            else
            {
                e.Cancel = true;
            }
        }
        //Delegate Login
        protected void handle_LoginReslut(object sender, string strStatus, string strDetails, DateTime dateTime, bool bPopupPwd)
        {
            if (strStatus == " Access to OMT!")
            {
                mcLogin.Close();
                _Pwd = "";
                DisclaimerText = strDetails;
                PopupPwd = bPopupPwd;

                _original_title = "GOSTrade Station" + strVersion + " " + AppFlag.BuildDate + " - " + "[" + AppFlag.IPOMT + "] " + "[" + _UserID + "] " + _tw;
                SetMenuBG(_tc);

                this.lbStatus.Content = strStatus;

                this.RibbonBTLogout.Visibility = Visibility.Visible; ;
                this.RibbonBTLogin.Visibility = Visibility.Hidden; ;
                this.RibbonMTabUseful.Visibility = Visibility.Visible;
                this.RibbonMTabAcount.Visibility = Visibility.Visible;
                this.RibbonMTabMarket.Visibility = Visibility.Visible;
                this.RibbonMTabOrders.Visibility = Visibility.Visible;
                this.RibbonMTabTrades.Visibility = Visibility.Visible;
                this.RibbonTabTools.Visibility = Visibility.Visible;
                this.RibbonPrefer.Visibility = Visibility.Visible;
                this.RibbonDesktop.Visibility = Visibility.Visible;
                this.RibbonHelp.Visibility = Visibility.Visible;
                this.RibbonMTabDealer.Visibility = Visibility.Visible;
                this.RibbonBTChartAnalysisSCI.Visibility = Visibility.Visible;
                this.RibbonBTChartAnalysisSCITradesTab.Visibility = Visibility.Visible;

                this.RibbonMain.SelectedIndex = 1;

                //harry 20140318
                if (AppFlag.bRobbin == false)
                {
                    switchShowMenu(true);
                }

                if (_marketPriceData == null)
                {
                    _marketPriceData = new MarketPriceData();
                }

                if (_marketPriceSentData == null)
                {
                    _marketPriceSentData = new List<MarketPriceSentData>();
                }

                if (_msgChannelOMP == null)
                {
                    _msgChannelOMP = new MessageChannel(AppFlag.IPOMP, AppFlag.PortOMP);
                }

                _customizeData = TradeStationSetting.ReturnCustomizeData(_UserID, "General");

                threadOMT = new Thread(new ThreadStart(CheckRedundOMT));
                threadOMT.IsBackground = true;
                threadOMT.Start();

                threadOMP = new Thread(new ThreadStart(CheckRedundOMP));
                threadOMP.IsBackground = true;
                threadOMP.Start();

                if (msgQueen != null)
                {
                    msgQueen.Run();
                }

                //If Dealer
                if (GOSTradeStation.isDealer)
                {
                    userData = TradeStationUser.GetAccountTable(_UserID);
                }

                DateTime preTime = DateTime.Now;

                //display Disclaimer
                //MessageBox.Show(this, DisclaimerText, Disclaimer , MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                ScrollableMessageBox msgBox = new ScrollableMessageBox();
                msgBox.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                msgBox.MaximizeBox = false;
                msgBox.MinimizeBox = false;
                //msgBox.Width = 400;
                //msgBox.Height = 280;
                msgBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly ;
                msgBox.Show("\r\n   " + DisclaimerText, " " + Disclaimer, System.Windows.Forms.MessageBoxButtons.OK);

                DateTime endTime = DateTime.Now;

                if (distributeMsg != null)
                {
                    //get current server time to sync  
                    distributeMsg.SyncServerTime(dateTime.AddSeconds((endTime - preTime).TotalSeconds));
                }

                TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + _UserID + " Logged on.");
            }
            else
            {
                this.lbStatus.Content = strDetails;

                if (_distributeMsg == null)
                {
                    _distributeMsg = new MessageDistribute();
                    _distributeMsg.DisControlFocus += distributeMsg_ControlFocus;
                    _distributeMsg.DisControlChangeTitle += distributeMsg_ControlChangeTitle;
                    _distributeMsg.DisNotification += distributeMsg_Notification;
                    _distributeMsg.DisGetError += distributeMsg_GetError;
                    _distributeMsg.DisMarginCallList += distributeMsg_MarginCallList;
                    _distributeMsg.DisAddOrder += distributeMsg_AddOrderInfo;

                    //harry,order operation 
                    _distributeMsg.DisInactivateOrder += (distributeMsg_InActiveOrderfConfirmInfo);  //new MessageDistribute.OnDisInactivateOrder
                    _distributeMsg.DisActivateOrder += (distributeMsg_OPOrderfConfirmInfo);  //new MessageDistribute.OnDisActivateOrder
                    _distributeMsg.DisDeleteOrder += distributeMsg_OPOrderfConfirmInfo;// new MessageDistribute.OnDisDeleteOrder(distributeMsg_OrderfConfirmInfo);

                    _distributeMsg.DisChangeOrder += distributeMsg_OPOrderfConfirmInfo;//  

                    _distributeMsg.DisGotProductList += distributeMsg_DisDisGotProductList;
                    _distributeMsg.SyncTime += distributeMsg_SyncTime;

                    distributeMsg.DisPriceManager += distributeMsg_DisPriceManager;
                    distributeMsg.DisChangeLanguage += distributeMsg_DisChangeLanguage;
                }
            }
        }

        private void InitWindows()
        {
            _IsWindowInitialized = true;

            //load user's desktop
            List<DesktopData> lsDesktopData = TradeStationDesktop.ParserXmlWindow(false);
            foreach (DesktopData data in lsDesktopData)
            {
                BuildWindow(data);
            }

            //Force change pwd
            if (PopupPwd == true)
            {
                GosBzTool.OpenCHPWD(_distributeMsg);
            }

            PrefAccInputModel = PrefAccInput.ReadAccPref(GOSTradeStation.UserID);

            if (isDealer)
            {
                TradeStationSend.Send(cmdClient.getMarginCallList);
            }
        }

        bool UnLoadLogout()
        {
            try
            {
                //TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + "  UnLoadLogout.");

                if (!_IsWindowInitialized) return true;
                _IsWindowInitialized = false;
                isReLoaded = false;
                deskFileName = null;
                m_BeforeGet = false;
                m_ChangedUser = false;

                //Clear NotificationQueue
                notifications = new NotificationQueue<object>();

                timeOutTimerOMP.Enabled = false;
                timeOutTimerOMT.Enabled = false;

                IDelearStatus.CloseMsgBoxWindows(); PositionBus.clearBusInstance(); GosBzTool.CloseCHPWD();
                if (GOSTS.AppFlag.EnabledRandomTest())
                {
                    TraderTest test = TraderTest.getTraderTest();
                    if (test != null)
                    {
                        test.Reset();
                        if (GOSTS.AppFlag.bRobbin)
                        {
                            //if (btnRBTestInst != null)
                            //{
                            //    btnRBTestInst.Label = "Test Instruct";
                            //}
                        }
                        else
                        {
                            if (this.btnTestInst != null)
                            {
                                this.btnTestInst.Header = "Test Instruct";
                            }
                        }
                    }
                }
                if (msgQueen != null)
                {
                    msgQueen.Suspend();
                }

                if (threadOMT != null && threadOMT.IsAlive)
                    threadOMT.Abort();
                if (threadOMP != null && threadOMP.IsAlive)
                    threadOMP.Abort();

                _msgChannel.ReceiveMessageSuccessUsingStream -= channel_ReceiveMessageSuccessUsingStream;
                _msgChannel.Error -= channel_Error;
                _msgChannel.Connecting -= channel_Connecting;
                _msgChannel.Connected -= channel_Connected;
                _msgChannel.Disconnected -= channel_Disconnected;
                _msgChannel.SendHeartBeat -= channel_SendHeartBeat;

                try
                {
                    _msgChannel.Dispose();
                }
                catch { }

                _msgChannelOMP.ReceiveMessageSuccessUsingStream -= channel_ReceiveMessageSuccessUsingStreamOMP;
                _msgChannelOMP.Error -= channel_ErrorOMP;
                _msgChannelOMP.Connecting -= channel_ConnectingOMP;
                _msgChannelOMP.Connected -= channel_ConnectedOMP;
                _msgChannelOMP.Disconnected -= channel_DisconnectedOMP;
                _msgChannelOMP.SendHeartBeat -= channel_SendHeartBeat;
                try
                {
                    _msgChannelOMP.Dispose();
                }
                catch { }

                _distributeMsg.DisControlFocus -= distributeMsg_ControlFocus;
                _distributeMsg.DisControlChangeTitle -= distributeMsg_ControlChangeTitle;
                _distributeMsg.DisNotification -= distributeMsg_Notification;
                _distributeMsg.DisGetError -= distributeMsg_GetError;
                _distributeMsg.DisMarginCallList -= distributeMsg_MarginCallList;
                _distributeMsg.DisAddOrder -= distributeMsg_AddOrderInfo;

                //harry,order operation                
                _distributeMsg.DisInactivateOrder -= (distributeMsg_InActiveOrderfConfirmInfo);
                _distributeMsg.DisActivateOrder -= (distributeMsg_OPOrderfConfirmInfo);
                _distributeMsg.DisDeleteOrder -= distributeMsg_OPOrderfConfirmInfo;
                _distributeMsg.DisChangeOrder -= distributeMsg_OPOrderfConfirmInfo;
                _distributeMsg.DisAddOrder -= distributeMsg_AddOrderInfo;

                _distributeMsg.DisGotProductList -= distributeMsg_DisDisGotProductList;
                _distributeMsg.SyncTime -= distributeMsg_SyncTime;

                distributeMsg.DisPriceManager -= distributeMsg_DisPriceManager;
                distributeMsg.DisChangeLanguage -= distributeMsg_DisChangeLanguage;
                _distributeMsg.Dispose();

                TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + _UserID + " Loged off. \r\n");
                GOSTS.Accounts.asyTaskManager.Reset();//.bStop = true;
                GOSTS.Accounts.OrderHistManager.Reset();//.bStop = true;
                return true;
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + "  UnLoadLogout ,Error: " + exp.ToString());
                return true;
            }
            finally
            {
                //Send Logout
                TradeStationSend.Send(cmdClient.getMsgLogout);

                _AccountID = null;
                _UserID = null;
                _Pwd = null;
                _msgChannel = null;
                _msgChannelOMP = null;
                _marketPriceData = null;
                _marketPriceSentData = null;
                _distributeMsg = null;
                bTestInstClick = false;
            }
        }

        #endregion

        #region SYNC SERVER TIME

        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMiliseconds;
        }

        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SystemTime sysTime);

        public static bool SetLocalTimeByStr(DateTime dateTime)
        {
            bool flag = false;
            SystemTime sysTime = new SystemTime();

            sysTime.wYear = (ushort)dateTime.Year;
            sysTime.wMonth = (ushort)dateTime.Month;
            sysTime.wDay = (ushort)dateTime.Day;
            sysTime.wHour = (ushort)dateTime.Hour;
            sysTime.wMinute = (ushort)dateTime.Minute;
            sysTime.wSecond = (ushort)dateTime.Second;

            try
            {
                flag = SetLocalTime(ref sysTime);
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "SetLocalTimeByStr(DateTime  dateTime), Error:SetLocalTime函数执行异常" + exp.Message);
            }

            return flag;
        }

        private void distributeMsg_SyncTime(object sender, DateTime dateTime)
        {
            TimeSpan timeSpan = (new TimeSpan(DateTime.Now.Ticks)).Subtract(new TimeSpan(dateTime.Ticks)).Duration();

            if (timeSpan.TotalSeconds < AppFlag.SyncDuration || customizeData.AlertData.isTimeCorrAlertT == false) return;
            if (MessageBox.Show(SyncTimeMsg + " " + ((DateTime.Compare(DateTime.Now, dateTime) > 0) ? "" : "-") + Math.Round(timeSpan.TotalSeconds, 0) + "s !",
                          SyncTimeTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                //SYNC SERVER TIME 
                if (SetLocalTimeByStr(dateTime) == false)
                {
                    MessageBox.Show(SyncTimeAlert, AlertTilte, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                }
                else
                {
                    // System.Windows.Application.Current.Dispatcher.BeginInvoke(this.InfoDelegate, new Object[] { " Please login again!" });
                }
            }
        }

        #endregion

        #region Load UI

        void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (isReLoaded == false) return;
            if (Container.Children.Count == 0 && _IsWindowInitialized == true)
            {
                isReLoaded = false;
                //distributeMsg.DistributeClearEvent(sender);
                Thread thread = new Thread(new ThreadStart(LoadDesktopThread));
                thread.Start();
            }
        }

        void LoadDesktopThread()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(this.LoadDesktopDelegate, new Object[] { });
        }

        void LoadDesktopWindows()
        {
            //Clear register and  mdiwindows tigger for load desktop
            TradeStationSend.ClearRegister(cmdClient.registerMarketPrice);
            TradeStationSend.ClearRegister(cmdClient.registerPriceDepth);
            TradeStationSend.ClearRegister(cmdClient.registerTicker);

            if (isDealer)
            {
                TradeStationSend.Send(cmdClient.getMarginCallList);
            }

            List<DesktopData> lsDesktopData;
            if (deskFileName == null)
            {
                lsDesktopData = TradeStationDesktop.ParserXmlWindow(true);
                if (lsDesktopData == null) return;
                foreach (DesktopData data in lsDesktopData)
                {
                    BuildWindow(data);
                }
                AppFlag.ConfigFile = GOSTradeStation.UserID + "-" + "GOSTS.XML";
                TradeStationSetting.UpdateConfig("ConfigFile", GOSTradeStation.UserID + "-" + "GOSTS.XML");
            }
            else
            {
                lsDesktopData = TradeStationDesktop.ParserXmlWindow(deskFileName);
                if (lsDesktopData == null) return;
                foreach (DesktopData data in lsDesktopData)
                {
                    BuildWindow(data);
                }
                string[] strs = deskFileName.Split(new Char[] { '\\' });
                if (strs[strs.Count() - 1] == "GOSTS.XML") return;
                AppFlag.ConfigFile = strs[strs.Count() - 1];
                TradeStationSetting.UpdateConfig("ConfigFile", strs[strs.Count() - 1]);
            }
        }

        private void BuildWindow(DesktopData data)
        {
            switch (data.Type)
            {
                case (int)WindowTypes.UserOrderInfo:
                    MdiChild mcOrderInfo = new MdiChild();
                    UserOrderInfo orderInfo = new UserOrderInfo(_distributeMsg, mcOrderInfo);
                    mcOrderInfo.Content = orderInfo;
                    mcOrderInfo.Position = new System.Windows.Point(data.X, data.Y);
                    mcOrderInfo.Width = (data.Width == 0) ? orderInfo.Width : data.Width;
                    mcOrderInfo.Height = (data.Height == 0) ? orderInfo.Height : data.Height;
                    if (!mcOrderInfo.ExistInContainer(mcOrderInfo, Container))
                    {
                        Container.Children.Add(mcOrderInfo);
                    }
                    break;
                case (int)WindowTypes.OrderEntry:
                    MdiChild mcOrderEntry = new MdiChild();
                    OrderEntry orderEntry = new OrderEntry(_distributeMsg, mcOrderEntry);
                    orderEntry.mdiChild = mcOrderEntry;
                    mcOrderEntry.Content = orderEntry;
                    mcOrderEntry.Position = new System.Windows.Point(data.X, data.Y);
                    mcOrderEntry.Width = (data.Width == 0) ? 312 : data.Width;// 312
                    mcOrderEntry.Height = (data.Height == 0) ? 250 : data.Height;// //250
                    orderEntry.LoadLastProduct();
                    //if (!mcOrderEntry.ExistInContainer(mcOrderEntry, Container))
                    //{
                    Container.Children.Add(mcOrderEntry);
                    //} 
                    break;
                case (int)WindowTypes.AccountInfo:
                    MdiChild mcAccountInfo = new MdiChild();
                    AccountInfo accountInfo = new AccountInfo(_distributeMsg, mcAccountInfo);
                    accountInfo.mdiChild = mcAccountInfo;
                    mcAccountInfo.Content = accountInfo;
                    mcAccountInfo.Position = new System.Windows.Point(data.X, data.Y);
                    mcAccountInfo.Width = (data.Width == 0) ? accountInfo.Width : data.Width;
                    mcAccountInfo.Height = (data.Height == 0) ? accountInfo.Height : data.Height;
                    if (!mcAccountInfo.ExistInContainer(mcAccountInfo, Container))
                    {
                        Container.Children.Add(mcAccountInfo);
                    }
                    break;
                case (int)WindowTypes.PriceDepth:
                    PriceDepth priceDepth = new PriceDepth(_distributeMsg);
                    priceDepth.Locked = data.Locked;
                    priceDepth.ProdCode = data.Code;
                    priceDepth.isVertical = data.ArrayMode;
                    MdiChild mcPriceDepth = new MdiChild();
                    priceDepth.mdiChild = mcPriceDepth;
                    // mcPriceDepth.Title = (priceDepth.Locked == true) ? "Price Depth – " + data.Code + " * " : "Price Depth – " + data.Code;
                    mcPriceDepth.Title = (priceDepth.Locked == true) ? TradeStationSetting.ReturnWindowName(WindowTypes.PriceDepth, data.Code) + " * " : TradeStationSetting.ReturnWindowName(WindowTypes.PriceDepth, data.Code);
                    mcPriceDepth.Content = priceDepth;
                    mcPriceDepth.Position = new System.Windows.Point(data.X, data.Y);
                    mcPriceDepth.Width = (data.Width == 0) ? priceDepth.Width : data.Width;
                    mcPriceDepth.Height = (data.Height == 0) ? priceDepth.Height : data.Height;
                    if (!mcPriceDepth.ExistInContainer(mcPriceDepth, Container))
                    {
                        Container.Children.Add(mcPriceDepth);
                    }
                    break;
                case (int)WindowTypes.LongPriceDepth:
                    LongPriceDepth longPriceDepth = new LongPriceDepth(_distributeMsg);
                    longPriceDepth.Locked = data.Locked;
                    longPriceDepth.ProdCode = data.Code;
                    MdiChild mcLongPriceDepth = new MdiChild();
                    longPriceDepth.mdiChild = mcLongPriceDepth;
                    //mcLongPriceDepth.Title = TradeStationSetting.ReturnWindowName(WindowTypes.LongPriceDepth, data.Code);
                    mcLongPriceDepth.Title = (longPriceDepth.Locked == true) ? "Long Price Depth – " + data.Code + " * " : "Long Price Depth – " + data.Code;
                    mcLongPriceDepth.Content = longPriceDepth;
                    mcLongPriceDepth.Position = new System.Windows.Point(data.X, data.Y);
                    mcLongPriceDepth.Width = (data.Width == 0) ? longPriceDepth.Width : data.Width;
                    mcLongPriceDepth.Height = (data.Height == 0) ? longPriceDepth.Height : data.Height;
                    if (!mcLongPriceDepth.ExistInContainer(mcLongPriceDepth, Container))
                    {
                        Container.Children.Add(mcLongPriceDepth);
                    }
                    break;
                case (int)WindowTypes.Ticker:
                    Ticker ticker = new Ticker(_distributeMsg);
                    ticker.Locked = data.Locked;
                    ticker.ProdCode = data.Code;
                    MdiChild mcTicker = new MdiChild();
                    ticker.mdiChild = mcTicker;
                    //mcTicker.Title = (ticker.Locked == true) ? "Ticker – " + data.Code + " * " : "Ticker – " + data.Code;
                    mcTicker.Title = (ticker.Locked == true) ? TradeStationSetting.ReturnWindowName(WindowTypes.Ticker, data.Code) + " * " : TradeStationSetting.ReturnWindowName(WindowTypes.Ticker, data.Code);
                    mcTicker.Content = ticker;
                    mcTicker.Position = new System.Windows.Point(data.X, data.Y);
                    mcTicker.Width = (data.Width == 0) ? ticker.Width : data.Width;
                    mcTicker.Height = (data.Height == 0) ? ticker.Height : data.Height;
                    if (!mcTicker.ExistInContainer(mcTicker, Container))
                    {
                        Container.Children.Add(mcTicker);
                    }
                    break;
                case (int)WindowTypes.MarginCallList:
                    if (!isDealer) break;
                    MarginCallList marginCallList = new MarginCallList(_distributeMsg);
                    MdiChild mcMarginCallList = new MdiChild();
                    marginCallList.mdiChild = mcMarginCallList;
                    mcMarginCallList.Title = TradeStationSetting.ReturnWindowName(WindowTypes.MarginCallList, "");
                    mcMarginCallList.Content = marginCallList;
                    mcMarginCallList.Width = (data.Width == 0) ? 200 : data.Width;
                    mcMarginCallList.Height = (data.Height == 0) ? 200 : data.Height;
                    mcMarginCallList.Position = (data.Height == 0) ? new System.Windows.Point(Container.ActualWidth - 200, Container.ActualHeight - 200) : new System.Windows.Point(data.X, data.Y);
                    mcMarginCallList.ShowButtonBox = false;
                    if (!mcMarginCallList.ExistInContainer(mcMarginCallList, Container))
                    {
                        Container.Children.Add(mcMarginCallList);
                    }
                    break;
                case (int)WindowTypes.PosSumary:
                    MdiChild mcPosSumary = new MdiChild();
                    PosSumary posSumary = new PosSumary(_distributeMsg, mcPosSumary);
                    // posSumary.mdiChild = mcPosSumary;
                    // mcPosSumary.Title ="Position Summary";//_UserID + "- " + WindowTypes.PosSumary;
                    mcPosSumary.Content = posSumary;

                    mcPosSumary.Position = new System.Windows.Point(data.X, data.Y);
                    mcPosSumary.Width = (data.Width == 0) ? posSumary.Width : data.Width;
                    mcPosSumary.Height = (data.Height == 0) ? posSumary.Height : data.Height;
                    if (!mcPosSumary.ExistInContainer(mcPosSumary, Container))
                    {
                        Container.Children.Add(mcPosSumary);
                    }
                    break;
                case (int)WindowTypes.MarginCheck:
                    MdiChild mcMarginCheck = new MdiChild();
                    MarginCheck marginCheck = new MarginCheck(_distributeMsg);
                    marginCheck.mdiChild = mcMarginCheck;
                    mcMarginCheck.Title = TradeStationSetting.ReturnWindowName(WindowTypes.MarginCheck, "");
                    mcMarginCheck.Content = marginCheck;

                    mcMarginCheck.Position = new System.Windows.Point(data.X, data.Y);
                    mcMarginCheck.Width = (data.Width == 0) ? marginCheck.Width : data.Width;
                    mcMarginCheck.Height = (data.Height == 0) ? marginCheck.Height : data.Height;
                    if (!mcMarginCheck.ExistInContainer(mcMarginCheck, Container))
                    {
                        Container.Children.Add(mcMarginCheck);
                    }
                    break;
                case (int)WindowTypes.TradeConfirmation:
                    MdiChild mcTradeConfirm = new MdiChild();
                    TradeConfirmation tradeConfirm = new TradeConfirmation(_distributeMsg);
                    tradeConfirm.mdiChild = mcTradeConfirm;
                    mcTradeConfirm.Title = TradeStationSetting.ReturnWindowName(WindowTypes.TradeConfirmation, "");
                    mcTradeConfirm.Content = tradeConfirm;
                    mcTradeConfirm.Position = new System.Windows.Point(data.X, data.Y);
                    mcTradeConfirm.Width = (data.Width == 0) ? tradeConfirm.Width : data.Width;
                    mcTradeConfirm.Height = (data.Height == 0) ? tradeConfirm.Height : data.Height;

                    if (!mcTradeConfirm.ExistInContainer(mcTradeConfirm, Container))
                    {
                        Container.Children.Add(mcTradeConfirm);
                    }
                    break;
                case (int)WindowTypes.MarketPriceControl:
                    MarketPriceControl marketPriceControl = new MarketPriceControl(_distributeMsg);
                    MdiChild mcMarketPriceControl = new MdiChild();
                    marketPriceControl.mdiChild = mcMarketPriceControl;
                    marketPriceControl.countSymbol = data.Code;
                    mcMarketPriceControl.Title = TradeStationSetting.ReturnWindowName(WindowTypes.MarketPriceControl, data.Code);
                    mcMarketPriceControl.Content = marketPriceControl;
                    mcMarketPriceControl.Position = new System.Windows.Point(data.X, data.Y);
                    mcMarketPriceControl.Width = (data.Width == 0) ? marketPriceControl.Width : data.Width; ;
                    mcMarketPriceControl.Height = (data.Height == 0) ? marketPriceControl.Height : data.Height; ;
                    if (!mcMarketPriceControl.ExistInContainer(mcMarketPriceControl, Container))
                    {
                        Container.Children.Add(mcMarketPriceControl);
                    }
                    if (_distributeMsg == null) return;
                    _distributeMsg.DistributeLoadProduct(data.Codes, mcMarketPriceControl.Title);

                    break;
                case (int)WindowTypes.Clock:
                    MdiChild mcClock = new MdiChild();
                    Clock clock = new Clock();
                    clock.mdiChild = mcClock;
                    mcClock.Title = TradeStationSetting.ReturnWindowName(WindowTypes.Clock, "");
                    mcClock.Content = clock;
                    mcClock.Position = new System.Windows.Point(data.X, data.Y);
                    mcClock.Width = (data.Width == 0) ? clock.Width : data.Width;
                    mcClock.Height = (data.Height == 0) ? clock.Height : data.Height;

                    if (!mcClock.ExistInContainer(mcClock, Container))
                    {
                        Container.Children.Add(mcClock);
                    }
                    break;
                case (int)WindowTypes.SCIChartAnalysis:
                    MdiChild sciMDIChild = new MdiChild();
                    SCIChartAnalysis sciChart = new SCIChartAnalysis(_distributeMsg, sciMDIChild, data.Code);
                    sciChart.IsLockProductEnabled = data.Locked;
                    sciChart.chartSettings = data.chartSettings;
                    sciMDIChild.Content = sciChart;
                    sciMDIChild.Width = (data.Width == 0) ? sciChart.Width : data.Width;
                    sciMDIChild.Height = (data.Height == 0) ? sciChart.Height : data.Height;
                    sciMDIChild.Position = new System.Windows.Point(data.X, data.Y);
                    Container.Children.Add(sciMDIChild);
                    break;
                case (int)WindowTypes.OptionMaster:
                    OptionMaster optionMaster = new OptionMaster(_distributeMsg);
                    optionMaster.ProdCode = data.Code;
                    MdiChild mcOptionMaster = new MdiChild();
                    optionMaster.mdiChild = mcOptionMaster;
                    mcOptionMaster.Content = optionMaster;
                    mcOptionMaster.Position = new System.Windows.Point(data.X, data.Y);
                    mcOptionMaster.Width = (data.Width == 0) ? optionMaster.Width : data.Width;
                    mcOptionMaster.Height = (data.Height == 0) ? optionMaster.Height : data.Height;
                    Container.Children.Add(mcOptionMaster);
                    break;
            }
        }

        #endregion

        #region Desktop Button

        private void LoadDefaultDesktop_Click(object sender, RoutedEventArgs e)
        {
            string strRs_conf_title = GOSTS.GosCulture.CultureHelper.GetString("confirm_Load_DefaultDeskTitle");
            string strRs_conf_Content = GOSTS.GosCulture.CultureHelper.GetString("confirm_Load_DefaultDesk");
            if (MessageBox.Show(strRs_conf_Content, strRs_conf_title, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            deskFileName = null;
            isReLoaded = true;
            Container.Children.Clear();
        }

        private void LoadUserDesktop_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
                open.FilterIndex = 1;
                open.InitialDirectory = AppFlag.UserData.Replace("\\\\", "\\");
                open.Multiselect = false;
                open.FileName = AppFlag.ConfigFile;
                open.Filter = "Desktop Files(*.XML)|*.XML";
                if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    deskFileName = open.FileName;
                    TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " LoadUserDesktop_Click(),deskFileName=" + open.FileName);
                    isReLoaded = true;
                    Container.Children.Clear();
                }
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " LoadUserDesktop_Click()" + exp.Message);
            }
        }

        private List<DesktopData> ReturnDesktopData()
        {
            List<DesktopData> lsDesktopData = new List<DesktopData>();
            DesktopData data;
            foreach (MdiChild mdiChild in Container.Children)
            {
                data = new DesktopData();
                data.Height = Math.Round(mdiChild.Height, 0, MidpointRounding.AwayFromZero);
                data.Width = Math.Round(mdiChild.Width, 0, MidpointRounding.AwayFromZero);
                //data.Height = (double.IsNaN(mdiChild.Height)) ? Math.Round(mdiChild.ActualHeight, 0, MidpointRounding.AwayFromZero) : Math.Round(mdiChild.Height, 0, MidpointRounding.AwayFromZero);
                //data.Width = (double.IsNaN(mdiChild.Width)) ? Math.Round(mdiChild.ActualWidth, 0, MidpointRounding.AwayFromZero) : Math.Round(mdiChild.Width, 0, MidpointRounding.AwayFromZero);
                data.X = mdiChild.Position.X;
                data.Y = mdiChild.Position.Y;
                Type windowType = mdiChild.Content.GetType();
                if (windowType != typeof(AddPriceAlert) && windowType != typeof(OrderConfirm) && windowType != typeof(AppPreference)
                    && windowType != typeof(ClosePosition) && windowType != typeof(ChangeOrder) && windowType != typeof(TradeConfirmDetails)
                   && windowType != typeof(ChangePWD) && windowType != typeof(GOSTS.Accounts.OrderHistory)
                     && windowType != typeof(GOSTS.Accounts.DoneTrade))
                {
                    if (windowType == typeof(AccountInfo))
                    {
                        data.Type = (int)WindowTypes.AccountInfo;
                    }
                    else if (windowType == typeof(OrderEntry))
                    {
                        OrderEntry orderEntry = (OrderEntry)mdiChild.Content;
                        data.Type = (int)WindowTypes.OrderEntry;
                        data.Code = orderEntry.ProdCode;
                        //data.Locked = orderEntry.Locked;
                    }
                    else if (windowType == typeof(Ticker))
                    {
                        Ticker ticker = (Ticker)mdiChild.Content;
                        data.Type = (int)WindowTypes.Ticker;
                        data.Code = ticker.ProdCode;
                        data.Locked = ticker.Locked;
                    }
                    else if (windowType == typeof(UserOrderInfo))
                    {
                        data.Type = (int)WindowTypes.UserOrderInfo;
                    }
                    else if (windowType == typeof(PriceDepth))
                    {
                        PriceDepth priceDepth = (PriceDepth)mdiChild.Content;
                        data.Type = (int)WindowTypes.PriceDepth;
                        data.Code = priceDepth.ProdCode;
                        data.Locked = priceDepth.Locked;
                        data.ArrayMode = priceDepth.isVertical;
                    }
                    else if (windowType == typeof(MarketPriceControl))
                    {
                        MarketPriceControl marketPrice = (MarketPriceControl)mdiChild.Content;
                        data.Type = (int)WindowTypes.MarketPriceControl;
                        List<string> arr = new List<string>();
                        TabControlViewModel tab = marketPrice.TabGrid.DataContext as TabControlViewModel;
                        if (tab != null)
                        {
                            foreach (TabPageViewModel page in tab.Pages)
                            {
                                if (page.MarketPriceItems != null)
                                {
                                    arr.Add(page.Header + "," + string.Concat("", string.Join(" ", page.MarketPriceItems.Select(d => d.ProductCode).ToList<string>()), " "));
                                }
                                else
                                {
                                    arr.Add(page.Header + "," + "");
                                }
                            }
                        }
                        data.Codes = arr;
                        data.Code = TradeStationSetting.GetMarketPriceCode(mdiChild.Title);
                    }
                    else if (windowType == typeof(LongPriceDepth))
                    {
                        LongPriceDepth longPriceDepth = (LongPriceDepth)mdiChild.Content;
                        data.Type = (int)WindowTypes.LongPriceDepth;
                        data.Code = longPriceDepth.ProdCode;
                        data.Locked = longPriceDepth.Locked;
                    }
                    else if (windowType == typeof(MarginCallList))
                    {
                        data.Type = (int)WindowTypes.MarginCallList;
                    }
                    else if (windowType == typeof(PosSumary))
                    {
                        data.Type = (int)WindowTypes.PosSumary;
                    }
                    else if (windowType == typeof(MarginCheck))
                    {
                        data.Type = (int)WindowTypes.MarginCheck;
                    }
                    else if (windowType == typeof(TradeConfirmation))
                    {
                        data.Type = (int)WindowTypes.TradeConfirmation;
                    }
                    else if (windowType == typeof(Clock))
                    {
                        data.Type = (int)WindowTypes.Clock;
                    }
                    else if (windowType == typeof(SCIChartAnalysis))
                    {
                        SCIChartAnalysis chartAnalysis = (SCIChartAnalysis)mdiChild.Content;
                        data.Type = (int)WindowTypes.SCIChartAnalysis;
                        data.Code = chartAnalysis.productCode;
                        data.Locked = chartAnalysis.IsLockProductEnabled;
                        data.chartSettings = chartAnalysis.chartSettings;
                    }
                    else if (windowType == typeof(OptionMaster))
                    {
                        OptionMaster optionMaster = (OptionMaster)mdiChild.Content;
                        data.Type = (int)WindowTypes.OptionMaster;
                        data.Code = optionMaster.ProdCode;
                    }
                    else
                    {
                        data.Type = -1;
                    }
                    lsDesktopData.Add(data);
                }
            }
            return lsDesktopData;
        }

        private void SaveAsCurrentDesktop_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.FilterIndex = 1;
            save.InitialDirectory = AppFlag.UserData.Replace("\\\\", "\\");
            save.Filter = "Desktop Files(*.XML)|*.XML";
            save.FileName = AppFlag.ConfigFile;
            if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //string[] strs = save.FileName.Split(new Char[] { '\\' });
                //TradeStationDesktop.SaveDesktop(strs[strs.Count() - 1], ReturnDesktopData());
                //AppFlag.ConfigFile = strs[strs.Count() - 1]; 

                TradeStationDesktop.SaveDesktop(save.FileName, ReturnDesktopData());
            }
        }

        private void SaveCurrentDesktop_Click(object sender, RoutedEventArgs e)
        {
            // TradeStationDesktop.SaveDesktop(AppFlag.ConfigFile, ReturnDesktopData());
            if (TradeStationDesktop.SaveDesktop(AppFlag.UserData + AppFlag.ConfigFile, ReturnDesktopData()))
            {
                if (GOSTradeStation.UserID != null)
                {
                    if (GOSTradeStation.UserID.Trim() != "")
                    {
                        PersonTransactionOrderHeight orderHConfig = new PersonTransactionOrderHeight();
                        orderHConfig.SaveHeight(GOSTradeStation.UserID);
                    }
                }
            }
        }

        #endregion

        #region Hnadle Notification

        void notifications_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                distributeMsg_ControlFocus(null, null, null);
            }));
        }

        protected void distributeMsg_Notification(object sender, Notification notification, TableNotification tableNotification)
        {
            //1: order book updated (level 5)
            //2: done trade updated (level 6)
            //3. order add success (level 5)
            //4. order add failure (level 6)
            //5 order change success (level 5)
            //6 order change failure (level 6)
            //7 order delete success (level 5)
            //8 order delete failure (level 6)
            //9 order activate success (level 5)
            //10 order activate failure (level 6)
            //11 order inactivate success (level 5)
            //12 order inactivate failure (level 6)
            //13 OM server disconnect (level 6)
            //14 OM server connected (level 6)
            //15 Force user logout (level 6)
            //100: general message (level 6)  

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (notification != null)
                {
                    PlaySound(notification);

                    TradeStationSend.SendNotification(cmdClient.notificationAck, notification.Acc_no, notification.Notify_Code, notification.SeqNo);
                    if ((notification.Message != "" && notification.Level >= _customizeData.AlertData.NotifyLevel && notification.Notify_Code != (int)NotifyTypeCode.DoneTradeUpdated) || (notification.Message != "" && notification.Level >= _customizeData.AlertData.NotifyLevel && notification.Notify_Code == (int)NotifyTypeCode.DoneTradeUpdated && _customizeData.AlertData.isTradeAlertT == true))
                    {
                        if (notification.Message != "" && notification.Level >= _customizeData.AlertData.NotifyLevel && (notification.Notify_Code == (int)NotifyTypeCode.OrderAddFailure || notification.Notify_Code == (int)NotifyTypeCode.OrderChangeFailure || notification.Notify_Code == (int)NotifyTypeCode.OrderDeleteFailure || notification.Notify_Code == (int)NotifyTypeCode.OrderActivateFailure || notification.Notify_Code == (int)NotifyTypeCode.OrderInactivateFailure) && _customizeData.AlertData.isReqErrAlertT == false) return;

                        if (notifications.IsEmpty())
                        {
                            // Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { notification, null, null, null });
                            OpenOrderConfrim(notification, null, null, null);
                            notification.Type = "Frist";
                            TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " GOSTradeStation.notification:" + notification.Message);
                        }
                        //  notification.AllMessage = notification.AllMessage + "\r\n" + (notifications.Count+1).ToString();
                        notification.AllMessage = AddedNotificationMsg + " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "\r\n\r\n" + notification.AllMessage;
                        notifications.Enqueue(notification);
                        //TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " GOSTradeStation.Add6:" + notification.Message);
                    }
                    bool isCurrectAcc = IDelearStatus.IsFindAcc(notification.Acc_no);
                    switch (notification.Notify_Code)
                    {
                        case 0:
                            break;
                        case 1:
                            if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                            {
                                TradeStationSend.Send(cmdClient.getTradeConfOrders);
                            }
                            if (!isCurrectAcc && notification.Acc_no != "@") return;
                            if (notification.Acc_no == "@" && IDelearStatus.ACCUoAc != null && IDelearStatus.ACCUoAc != "")
                            {
                                TradeStationSend.Send(cmdClient.getAccountInfo, IDelearStatus.ACCUoAc);
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, IDelearStatus.ACCUoAc);
                            }
                            else
                            {
                                TradeStationSend.Send(cmdClient.getAccountInfo, notification.Acc_no);
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, notification.Acc_no);
                            }
                            break;
                        case 2:
                            if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                            {
                                TradeStationSend.Send(cmdClient.getTradeConfOrders);
                                TradeStationSend.Send(cmdClient.getTradeConfTrades);
                            }
                            if (!isCurrectAcc && notification.Acc_no != "@") return;
                            if (notification.Acc_no == "@" && IDelearStatus.ACCUoAc != null && IDelearStatus.ACCUoAc != "")
                            {
                                TradeStationSend.Send(cmdClient.getAccountInfo, IDelearStatus.ACCUoAc);
                                TradeStationSend.Send(cmdClient.getCashInfo, IDelearStatus.ACCUoAc);
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, IDelearStatus.ACCUoAc);
                                TradeStationSend.Send(cmdClient.getPositionInfo, IDelearStatus.ACCUoAc);
                                TradeStationSend.Send(cmdClient.getDoneTradeInfo, IDelearStatus.ACCUoAc);
                                TradeStationSend.Send(cmdClient.getClearTradeInfo, IDelearStatus.ACCUoAc);
                            }
                            else
                            {
                                TradeStationSend.Send(cmdClient.getAccountInfo, notification.Acc_no);
                                TradeStationSend.Send(cmdClient.getCashInfo, notification.Acc_no);
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, notification.Acc_no);
                                TradeStationSend.Send(cmdClient.getPositionInfo, notification.Acc_no);
                                TradeStationSend.Send(cmdClient.getDoneTradeInfo, notification.Acc_no);
                                TradeStationSend.Send(cmdClient.getClearTradeInfo, notification.Acc_no);
                            }
                            break;
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                            if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                            {
                                TradeStationSend.Send(cmdClient.getTradeConfOrders);
                            }
                            if (!isCurrectAcc && notification.Acc_no != "@") return;
                            if (notification.Acc_no == "@" && IDelearStatus.ACCUoAc != null && IDelearStatus.ACCUoAc != "")
                            {
                                TradeStationSend.Send(cmdClient.getAccountInfo, IDelearStatus.ACCUoAc);
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, IDelearStatus.ACCUoAc);
                            }
                            else
                            {
                                TradeStationSend.Send(cmdClient.getAccountInfo, notification.Acc_no);
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, notification.Acc_no);
                            }
                            break;
                        case 13:
                            break;
                        case 14:
                            break;
                        case 15:  //Force user logout  
                            System.Windows.Application.Current.Dispatcher.BeginInvoke(this.InfoDelegate, new Object[] { " Force logout!" });
                            break;
                        default:
                            break;
                    }
                }
                if (tableNotification != null)
                {
                    PlaySound(tableNotification);

                    TradeStationSend.SendNotification(cmdClient.tableNotificationAck, tableNotification.Acc_no, tableNotification.TableCode, tableNotification.VersionNo);
                    if ((tableNotification.Message != "" && tableNotification.Level >= _customizeData.AlertData.NotifyLevel && tableNotification.TableCode != (int)TableNotifyCode.DoneTradeUpdated) || (tableNotification.Message != "" && tableNotification.Level >= _customizeData.AlertData.NotifyLevel && tableNotification.TableCode == (int)TableNotifyCode.DoneTradeUpdated && _customizeData.AlertData.isTradeAlertT == true))
                    {
                        if (notifications.IsEmpty())
                        {
                            //   Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { null, tableNotification, null, null });
                            OpenOrderConfrim(null, tableNotification, null, null);
                            tableNotification.Type = "Frist";
                            TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " GOSTradeStation.tableNotification:" + tableNotification.Message);
                        }
                        // tableNotification.AllMessage = tableNotification.AllMessage + "\r\n" + (notifications.Count+1).ToString();
                        tableNotification.AllMessage = AddedNotificationMsg + " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "\r\n\r\n" + tableNotification.AllMessage;
                        notifications.Enqueue(tableNotification);
                        //TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " GOSTradeStation.Add7:" + tableNotification.Message);
                    }
                    bool isCurrectAcc = IDelearStatus.IsFindAcc(tableNotification.Acc_no);
                    switch (tableNotification.TableCode)
                    {
                        //0: reserved
                        //1: Order book updated (table: acc_order_hist)
                        //2: Done trade updated (table: done_trade)
                        //3: Position updated (table: acc_pos)
                        //4: Cash Info update (table: acc_bal)
                        //5: Account Info updated (table: acc_mkt)
                        //6. MarginCAllList updated (table: )
                        //7: System parameters updated (table:sys_setting) 
                        case 0:
                            break;
                        case 1:
                            if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                            {
                                TradeStationSend.Send(cmdClient.getTradeConfOrders);
                            }
                            if (!isCurrectAcc && tableNotification.Acc_no != "@") return;
                            if (tableNotification.Acc_no == "@" && IDelearStatus.ACCUoAc != null && IDelearStatus.ACCUoAc != "")
                            {
                                TradeStationSend.Send(cmdClient.getAccountInfo, IDelearStatus.ACCUoAc);
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, IDelearStatus.ACCUoAc);
                            }
                            else
                            {
                                TradeStationSend.Send(cmdClient.getAccountInfo, tableNotification.Acc_no);
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, tableNotification.Acc_no);
                            }
                            break;
                        case 2:
                            if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                            {
                                TradeStationSend.Send(cmdClient.getTradeConfOrders);
                                TradeStationSend.Send(cmdClient.getTradeConfTrades);
                            }
                            if (!isCurrectAcc && tableNotification.Acc_no != "@") return;
                            if (tableNotification.Acc_no == "@" && IDelearStatus.ACCUoAc != null && IDelearStatus.ACCUoAc != "")
                            {
                                TradeStationSend.Send(cmdClient.getAccountInfo, IDelearStatus.ACCUoAc);
                                TradeStationSend.Send(cmdClient.getCashInfo, IDelearStatus.ACCUoAc);
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, IDelearStatus.ACCUoAc);
                                TradeStationSend.Send(cmdClient.getPositionInfo, IDelearStatus.ACCUoAc);
                                TradeStationSend.Send(cmdClient.getDoneTradeInfo, IDelearStatus.ACCUoAc);
                                TradeStationSend.Send(cmdClient.getClearTradeInfo, IDelearStatus.ACCUoAc);
                            }
                            else
                            {
                                TradeStationSend.Send(cmdClient.getAccountInfo, tableNotification.Acc_no);
                                TradeStationSend.Send(cmdClient.getCashInfo, tableNotification.Acc_no);
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, tableNotification.Acc_no);
                                TradeStationSend.Send(cmdClient.getPositionInfo, tableNotification.Acc_no);
                                TradeStationSend.Send(cmdClient.getDoneTradeInfo, tableNotification.Acc_no);
                                TradeStationSend.Send(cmdClient.getClearTradeInfo, tableNotification.Acc_no);
                            }
                            break;
                        case 3:
                            if (!isCurrectAcc && tableNotification.Acc_no != "@") return;
                            if (tableNotification.Acc_no == "@" && IDelearStatus.ACCUoAc != null && IDelearStatus.ACCUoAc != "")
                            {
                                TradeStationSend.Send(cmdClient.getPositionInfo, IDelearStatus.ACCUoAc);
                            }
                            else
                            {
                                TradeStationSend.Send(cmdClient.getPositionInfo, tableNotification.Acc_no);
                            }
                            break;
                        case 4:
                            if (!isCurrectAcc && tableNotification.Acc_no != "@") return;
                            if (tableNotification.Acc_no == "@" && IDelearStatus.ACCUoAc != null && IDelearStatus.ACCUoAc != "")
                            {
                                TradeStationSend.Send(cmdClient.getCashInfo, IDelearStatus.ACCUoAc);
                            }
                            else
                            {
                                TradeStationSend.Send(cmdClient.getCashInfo, tableNotification.Acc_no);
                            }
                            break;
                        case 5:
                            //Kenlo referred 20141224 
                            if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                            {
                                TradeStationSend.Send(cmdClient.getTradeConfOrders);
                            }

                            if (!isCurrectAcc && tableNotification.Acc_no != "@") return;
                            if (tableNotification.Acc_no == "@" && IDelearStatus.ACCUoAc != null && IDelearStatus.ACCUoAc != "")
                            {
                                TradeStationSend.Send(cmdClient.getAccountInfo, IDelearStatus.ACCUoAc);
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, IDelearStatus.ACCUoAc);
                            }
                            else
                            {
                                TradeStationSend.Send(cmdClient.getAccountInfo, tableNotification.Acc_no);
                                TradeStationSend.Send(cmdClient.getOrderBookInfo, tableNotification.Acc_no);
                            }
                            break;
                        case 6:
                            TradeStationSend.Send(cmdClient.getMarginCallList);
                            TradeStationLog.WriteForHearbeatandMarginCall(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " Send MarginCallList..");

                            break;
                    }
                }
            }));
        }

        //Play Sound
        void PlaySound(Object obj)
        {
            bool bPlaySound = false;
            int iResult = -1;

            Type type = obj.GetType();
            if (type == typeof(OrderResponse))
            {
                OrderResponse orderResponse = obj as OrderResponse;
                if ((orderResponse.Result == 0 && _customizeData.AlertData.isOrderConfirmS) || (orderResponse.Result > 0 && _customizeData.AlertData.isReqAccAlertS) || (orderResponse.Result < 0 && _customizeData.AlertData.isReqErrAlertS))
                {
                    bPlaySound = true;
                    iResult = orderResponse.Result;
                }
            }
            else if (type == typeof(Notification))
            {
                Notification notification = obj as Notification;
                if ((notification.Message != "" && notification.Level >= _customizeData.AlertData.NotifyLevel && notification.Notify_Code != (int)NotifyTypeCode.DoneTradeUpdated) || (notification.Message != "" && notification.Level >= _customizeData.AlertData.NotifyLevel && notification.Notify_Code == (int)NotifyTypeCode.DoneTradeUpdated && _customizeData.AlertData.isTradeAlertS == true))
                {
                    bPlaySound = true;
                    iResult = notification.Notify_Code;
                }
            }
            else if (type == typeof(TableNotification))
            {
                TableNotification tableNotification = obj as TableNotification;
                if ((tableNotification.Message != "" && tableNotification.Level >= _customizeData.AlertData.NotifyLevel && tableNotification.TableCode != (int)TableNotifyCode.DoneTradeUpdated) || (tableNotification.Message != "" && tableNotification.Level >= _customizeData.AlertData.NotifyLevel && tableNotification.TableCode == (int)TableNotifyCode.DoneTradeUpdated && _customizeData.AlertData.isTradeAlertS == true))
                {
                    bPlaySound = true;
                    iResult = tableNotification.TableCode;
                }
            }
            else
            {
            }

            string fileName = "";
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (!bPlaySound) return;
                if (type == typeof(OrderResponse))
                {
                    if (iResult == 0)
                    {
                        fileName = "OC.wav";
                    }
                    else if (iResult < 0)
                    {
                        fileName = "REA.wav";
                    }
                    else if (iResult > 0)
                    {
                        fileName = "RAA.wav";
                    }
                }
                else
                {
                    if (iResult == 2)
                    {
                        fileName = "TA.wav";
                    }
                    else if ((type == typeof(Notification))
                        && (iResult == (int)NotifyTypeCode.OrderAddFailure || iResult == (int)NotifyTypeCode.OrderChangeFailure || iResult == (int)NotifyTypeCode.OrderDeleteFailure || iResult == (int)NotifyTypeCode.OrderActivateFailure || iResult == (int)NotifyTypeCode.OrderInactivateFailure)
                        && _customizeData.AlertData.isReqErrAlertS == true)
                    {
                        fileName = "REA.wav";
                    }
                }

                //string type = "";
                //switch (obj.GetType().FullName)
                //{
                //    case "GOSTS.Common.OrderResponse":
                //        OrderResponse orderResponse = obj as OrderResponse;
                //        if ((orderResponse.Result == 0 && _customizeData.AlertData.isOrderConfirmS) || (orderResponse.Result > 0 && _customizeData.AlertData.isReqAccAlertS) || (orderResponse.Result < 0 && _customizeData.AlertData.isReqErrAlertS))
                //        {
                //            type = "OrderResponse";
                //            bPlaySound = true;
                //            iResult = orderResponse.Result;
                //        }

                //        break;
                //    case "GOSTS.Common.Notification":
                //        Notification notification = obj as Notification;
                //        if ((notification.Message != "" && notification.Level >= _customizeData.AlertData.NotifyLevel && notification.Notify_Code != 2) || (notification.Message != "" && notification.Level >= _customizeData.AlertData.NotifyLevel && notification.Notify_Code == 2 && _customizeData.AlertData.isTradeAlertS == true))
                //        {
                //            type = "Notification";
                //            bPlaySound = true;
                //            iResult = notification.Notify_Code;
                //        }
                //        break;
                //    case "GOSTS.Common.TableNotification":
                //        TableNotification tableNotification = obj as TableNotification;
                //        if ((tableNotification.Message != "" && tableNotification.Level >= _customizeData.AlertData.NotifyLevel && tableNotification.TableCode != 2) || (tableNotification.Message != "" && tableNotification.Level >= _customizeData.AlertData.NotifyLevel && tableNotification.TableCode == 2 && _customizeData.AlertData.isTradeAlertS == true))
                //        {
                //            type = "TableNotification";
                //            bPlaySound = true;
                //            iResult = tableNotification.TableCode;
                //        }
                //        break;
                //    default:
                //        break;
                //}


                //string fileName = ""; 
                //Application.Current.Dispatcher.Invoke(new Action(() =>
                //{
                //    if (!bPlaySound) return;
                //    if (type == "OrderResponse")
                //    {
                //        if (iResult == 0)
                //        {
                //            fileName = "OC.wav";
                //        }
                //        else if (iResult < 0)
                //        {
                //            fileName = "REA.wav";
                //        }
                //        else if (iResult > 0)
                //        {
                //            fileName = "RAA.wav";
                //        }
                //    }
                //    else
                //    {
                //        if (iResult == 2)
                //        {
                //            fileName = "TA.wav";
                //        }
                //        else if ((type == "Notification") && (iResult == 4 || iResult == 6 || iResult == 8 || iResult == 10 || iResult == 12) && _customizeData.AlertData.isReqErrAlertS == true)
                //        {
                //            fileName = "REA.wav";
                //        }
                //    }

                using (SoundPlayer player = new SoundPlayer())
                {
                    string strLocation = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Audio\\" + fileName;
                    if (!File.Exists(strLocation)) return;
                    //TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " Play sound url: " + strLocation);
                    player.SoundLocation = strLocation;
                    player.Play();
                }
            }));
        }

        //Hand Popup Window
        void OpenOrderConfrim(Notification notification, TableNotification tableNotification, ObservableCollection<MarginCheckAccData> data, OrderResponse orderResponse)
        {
            if (data != null)
            {
                //MarginCallList
                MdiChild mcMarginCallList = new MdiChild();
                MarginCallList marginCallList = new MarginCallList(_distributeMsg);
                marginCallList.marginCheckViewModel.MarginCheckAccDataes = data;
                marginCallList.mdiChild = mcMarginCallList;
                foreach (MdiChild child in Container.Children)
                {
                    //if (child.Title == mcMarginCallList.Title)
                    if (child.Content.GetType() == typeof(MarginCallList))
                    {
                        child.Content = marginCallList;
                        child.Focus();
                        break;
                    }
                }
            }
            else
            {
                MdiChild orderChild = null;
                OrderConfirm orderConfirm = null;
                bool Existed = false;
                foreach (MdiChild child in container.Children)
                {
                    orderConfirm = child.Content as OrderConfirm;
                    if (orderConfirm != null)
                    {
                        Existed = true;
                        orderChild = child;
                        break;
                    }
                }

                if (!Existed)
                {
                    orderChild = new MdiChild();
                    orderConfirm = new OrderConfirm(orderChild);
                    orderChild.Position = new System.Windows.Point(0, 0);
                    orderChild.Width = orderConfirm.Width;
                    orderChild.Height = orderConfirm.Height;
                    if (orderResponse != null)
                    {
                        if ((orderResponse.Result == 0 && _customizeData.AlertData.isOrderConfirmT) || (orderResponse.Result > 0 && _customizeData.AlertData.isReqAccAlertT) || (orderResponse.Result < 0 && _customizeData.AlertData.isReqErrAlertT))
                        {
                            orderResponse.Message = AddedNotificationMsg + " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "\r\n\r\n" + orderResponse.Message;
                            orderConfirm.lbl_Message.Text = orderResponse.Message;
                            if (orderResponse.Result < 0)
                            {
                                orderConfirm.lbl_Message.Foreground = Brushes.Red;
                            }
                            else
                            {
                                orderConfirm.lbl_Message.Foreground = Brushes.Green;
                            }
                            orderChild.Title = GOSTradeStation.UserID + " - Order Confirm";
                        }
                    }
                    else if (notification != null)
                    {
                        notification.AllMessage = AddedNotificationMsg + " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "\r\n\r\n" + notification.AllMessage;
                        orderConfirm.lbl_Message.Text = notification.AllMessage;
                        orderChild.Title = notification.UserID + " - Notification Order Confirm";
                        if (notification.Notify_Code == (int)NotifyTypeCode.OrderActivateFailure ||
                            notification.Notify_Code == (int)NotifyTypeCode.OrderAddFailure ||
                            notification.Notify_Code == (int)NotifyTypeCode.OrderChangeFailure ||
                            notification.Notify_Code == (int)NotifyTypeCode.OrderDeleteFailure)
                        {
                            orderConfirm.lbl_Message.Foreground = Brushes.Red;
                        }
                        else
                        {
                            orderConfirm.lbl_Message.Foreground = Brushes.Green;
                        }
                    }
                    else if (tableNotification != null)
                    {
                        tableNotification.AllMessage = AddedNotificationMsg + " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "\r\n\r\n" + tableNotification.AllMessage;
                        orderConfirm.lbl_Message.Text = tableNotification.AllMessage;
                        orderConfirm.lbl_Message.Foreground = Brushes.Green;
                        orderChild.Title = tableNotification.UserID + " - TableNotification Order Confirm";
                    }
                    orderChild.Content = orderConfirm;
                    container.Children.Add(orderChild);
                }
                else
                {
                    if (orderResponse != null)
                    {
                        if ((orderResponse.Result == 0 && _customizeData.AlertData.isOrderConfirmT) || (orderResponse.Result > 0 && _customizeData.AlertData.isReqAccAlertT) || (orderResponse.Result < 0 && _customizeData.AlertData.isReqErrAlertT))
                        {
                            orderResponse.Message = AddedNotificationMsg + " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "\r\n\r\n" + orderResponse.Message;
                            orderConfirm.lbl_Message.Text = orderResponse.Message;
                            if (orderResponse.Result < 0)
                            {
                                orderConfirm.lbl_Message.Foreground = Brushes.Red;
                            }
                            else
                            {
                                orderConfirm.lbl_Message.Foreground = Brushes.Green;
                            }
                            orderChild.Title = GOSTradeStation.UserID + " - Order Confirm";
                        }
                    }
                    else if (notification != null)
                    {
                        notification.AllMessage = AddedNotificationMsg + " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "\r\n\r\n" + notification.AllMessage;
                        orderConfirm.lbl_Message.Text = notification.AllMessage;
                        if (notification.Notify_Code == (int)NotifyTypeCode.OrderActivateFailure ||
                                 notification.Notify_Code == (int)NotifyTypeCode.OrderAddFailure ||
                                 notification.Notify_Code == (int)NotifyTypeCode.OrderChangeFailure ||
                                 notification.Notify_Code == (int)NotifyTypeCode.OrderDeleteFailure)
                        {
                            orderConfirm.lbl_Message.Foreground = Brushes.Red;
                        }
                        else
                        {
                            orderConfirm.lbl_Message.Foreground = Brushes.Green;
                        }
                        orderChild.Title = notification.UserID + " - Notification Order Confirm";
                    }
                    else if (tableNotification != null)
                    {
                        tableNotification.AllMessage = AddedNotificationMsg + " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "\r\n\r\n" + tableNotification.AllMessage;
                        orderConfirm.lbl_Message.Text = tableNotification.AllMessage;
                        orderChild.Title = tableNotification.UserID + " - TableNotification Order Confirm";
                        orderConfirm.lbl_Message.Foreground = Brushes.Green;
                    }
                }
            }
        }

        #endregion

        #region Handles Order

        MsgQuee msgQueen;
        public void ResponseDisAddOrder(object invoker, OrderResponse sorderResponse)
        {
            if (sorderResponse == null) return;
            bool bReset = false;
            if (sorderResponse.Result < 0)
            {
                if (GOSTradeStation.Chk_mrgin_opt != null)
                {
                    if (GOSTradeStation.Chk_mrgin_opt != "" && sorderResponse.Result == -105)
                    {
                        bReset = true;
                    }
                }
            }

            //  bReset = true;
            if (bReset)
            {
                if (sorderResponse == null)
                    return;
                if (sorderResponse.Rinfo == null)
                    return;

                string RInfo = sorderResponse.Rinfo;
                string spLeft = "sp=";
                string bdLeft = "&bd=";
                int pos = RInfo.IndexOf(spLeft);
                if (pos < 0)
                {
                    return;
                }
                int posEnd = RInfo.IndexOf(bdLeft);
                if (posEnd < 0) return;
                if (pos >= posEnd) return;
                int len = posEnd - pos;
                string sp = RInfo.Substring(pos, len);
                sp = sp.Replace("sp=", "").Trim();
                string str = RInfo.Substring(posEnd).Replace(bdLeft, "");
                string[] strArry = str.Split('"');
                if (strArry.Length > 1)
                {
                    str = strArry[1];
                }

                //bool b64 = false;
                //if (strArry.Length > 2)
                //{
                //    if (strArry[2].ToLower().IndexOf("mg=b64") > -1)
                //    {
                //        b64 = true;
                //        sorderResponse.ErrorMsg = TradeStationTools.Base64StringToString(sorderResponse.ErrorMsg);
                //    }
                //}

                str = str.Replace("\"", "").Replace("\\", "").Replace(sp, ",");


                // if (MessageBox.Show(sorderResponse.Message + "\n\n continue to add order?", "Add Order Confirm ", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    //bool? Yes=null;
                    //Dispatcher.Invoke((Action)delegate() {
                    //MsgForm msgbox = new MsgForm("Add Order Confirm", sorderResponse.Message + "\n\n continue to add order?", 15);
                    //msgbox.Top = 300;
                    //msgbox.Left = 450;
                    //Yes = msgbox.ShowDialog();

                    //if (Yes != true)
                    InfoBox info = new InfoBox();
                    info.Title = "Approval Request ";
                    info.Msg = sorderResponse.Message;// +"\n\n continue to add order?";
                    info.OnDo += delegate()
                    {
                        if (GOSTradeStation.isDealer)
                        {
                            GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, sorderResponse.Acc_no,
                           str, GOSTradeStation.Chk_mrgin_opt
                                ));
                            TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "confirm and replace order acc:" + sorderResponse.Acc_no + "\r --" + sorderResponse.Message);
                        }
                        else
                        {
                            GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgAddOrder(GOSTradeStation.UserID, GOSTradeStation.Pwd, "",
                           str, GOSTradeStation.Chk_mrgin_opt));

                        }
                    };
                    if (msgQueen != null)
                    {
                        msgQueen.Add(info);
                    }
                }
            }
            //else
            //{
            //    Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { null, null, null, sorderResponse });
            //}
            //if (GOSTradeStation.isDealer)
            //{
            //    TradeStationSend.Send(cmdClient.getOrderBookInfo, sorderResponse.Acc_no);
            //    TradeStationSend.Send(cmdClient.getAccountInfo, sorderResponse.Acc_no);
            //    TradeStationSend.Send(cmdClient.getPositionInfo, sorderResponse.Acc_no);
            //}
            //else
            //{
            //    TradeStationSend.Send(cmdClient.getOrderBookInfo);
            //    TradeStationSend.Send(cmdClient.getAccountInfo);
            //    TradeStationSend.Send(cmdClient.getPositionInfo);
            //} 
        }

        protected void distributeMsg_AddOrderInfo(object sender, OrderResponse orderResponse)
        {
            ResponseOrderReSend(sender, orderResponse);

            PlaySound(orderResponse);

            if (GOSTradeStation.Chk_mrgin_opt != "" && orderResponse.Result == -105)
            {
                Application.Current.Dispatcher.Invoke(new MessageDistribute.OnDisAddOrder(ResponseDisAddOrder), new Object[] { sender, orderResponse });

                if (notifications.IsEmpty())
                {
                    Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { null, null, null, orderResponse });
                    orderResponse.Type = "Frist";
                }
                orderResponse.Message = AddedNotificationMsg + " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "\r\n\r\n" + orderResponse.Message;
                notifications.Enqueue(orderResponse);
            }
            else
            {
                if ((orderResponse.Result == 0 && _customizeData.AlertData.isOrderConfirmT) || (orderResponse.Result > 0 && _customizeData.AlertData.isReqAccAlertT) || (orderResponse.Result < 0 && _customizeData.AlertData.isReqErrAlertT))
                {
                    if (notifications.IsEmpty())
                    {
                        Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { null, null, null, orderResponse });
                        orderResponse.Type = "Frist";
                        TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " GOSTradeStation.AddOrder2:" + orderResponse.Message);
                    }
                    orderResponse.Message = AddedNotificationMsg + " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "\r\n\r\n" + orderResponse.Message;
                    notifications.Enqueue(orderResponse);
                }
            }
        }

        delegate void deleResendOrder(object sender, OrderResponse orderResponse);
        public void ResponseOrderReSend(object sender, OrderResponse orderResponse)
        {
            Application.Current.Dispatcher.BeginInvoke((deleResendOrder)delegate(object invoker, OrderResponse sorderResponse)
            {
                if (GOSTradeStation.isDealer)
                {
                    TradeStationSend.Send(cmdClient.getOrderBookInfo, sorderResponse.Acc_no);
                    TradeStationSend.Send(cmdClient.getAccountInfo, sorderResponse.Acc_no);
                    TradeStationSend.Send(cmdClient.getPositionInfo, sorderResponse.Acc_no);
                    if (TradeStationSend.ExistInContainer(WindowTypes.TradeConfirmation))
                    {
                        TradeStationSend.Send(cmdClient.getTradeConfOrders);
                    }
                }
                else
                {
                    TradeStationSend.Send(cmdClient.getOrderBookInfo);
                    TradeStationSend.Send(cmdClient.getAccountInfo);
                    TradeStationSend.Send(cmdClient.getPositionInfo);
                }
            }, new Object[] { sender, orderResponse });
        }

        protected void distributeMsg_InActiveOrderfConfirmInfo(object sender, OrderResponse orderResponse)
        {
            ResponseOrderReSend(sender, orderResponse);

            PlaySound(orderResponse);

            if ((orderResponse.Result == 0 && _customizeData.AlertData.isOrderConfirmT) || (orderResponse.Result > 0 && _customizeData.AlertData.isReqAccAlertT) || (orderResponse.Result < 0 && _customizeData.AlertData.isReqErrAlertT))
            {
                if (notifications.IsEmpty())
                {
                    Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { null, null, null, orderResponse });
                    orderResponse.Type = "Frist";
                    TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " GOSTradeStation.InActive:" + orderResponse.Message);
                }
                orderResponse.Message = AddedNotificationMsg + " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "\r\n\r\n" + orderResponse.Message;
                notifications.Enqueue(orderResponse);
            }
        }

        protected void distributeMsg_OPOrderfConfirmInfo(object sender, OrderResponse orderResponse)
        {
            ResponseOrderReSend(sender, orderResponse);

            PlaySound(orderResponse);

            if ((orderResponse.Result == 0 && _customizeData.AlertData.isOrderConfirmT) || (orderResponse.Result > 0 && _customizeData.AlertData.isReqAccAlertT) || (orderResponse.Result < 0 && _customizeData.AlertData.isReqErrAlertT))
            {
                if (notifications.IsEmpty())
                {
                    Application.Current.Dispatcher.Invoke(DisOrderConfirmDelegate, new object[] { null, null, null, orderResponse });
                    orderResponse.Type = "Frist";
                    TradeStationLog.WriteForNotificationsOpening(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " GOSTradeStation.OPOrder:" + orderResponse.Message);
                }
                orderResponse.Message = AddedNotificationMsg + " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "\r\n\r\n" + orderResponse.Message;
                notifications.Enqueue(orderResponse);
            }
        }

        #endregion

        #region Handles ActiveMdiwindow

        protected void distributeMsg_ControlFocus(object sender, string defaultTitle, MdiChild child)
        {
            if (sender == null)
            {
                foreach (MdiChild mdiChild in Container.Children)
                {
                    if (mdiChild.Content.GetType() == typeof(OrderConfirm))
                    {
                        mdiChild.Focus();
                        break;
                    }
                }
                return;
            }

            //HARRY 2014-03-18
            if (AppFlag.bRobbin == false)
            {
                for (int i = 0; i < this.miOpen.Items.Count; i++)
                {
                    MenuItem rmi = miOpen.Items[i] as MenuItem;
                    if (rmi != null)
                    {
                        if (rmi.Header == null) continue;
                        if (rmi.Header.ToString() == defaultTitle)
                        {
                            rmi.Header = child.Title;
                            this.ActiveMenuItem_Click(rmi, new RoutedEventArgs(System.Windows.Controls.MenuItem.ClickEvent, rmi));
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < RibbonWindowsMenu.Items.Count; i++)
                {
                    RibbonMenuItem rmi = RibbonWindowsMenu.Items[i] as RibbonMenuItem;
                    if (rmi != null)
                    {
                        if (rmi.Header == null) continue;
                        if (rmi.Header.ToString() == defaultTitle)
                        {
                            rmi.Header = child.Title;
                            this.RibbonMenuItem_Click(rmi, new RoutedEventArgs(System.Windows.Controls.MenuItem.ClickEvent, rmi));
                            break;
                        }
                        //else
                        //{
                        //    rmi.IsChecked = false;
                        //}
                    }
                }

                for (int i = 0; i < RibbonWindowsMenu2.Items.Count; i++)
                {
                    RibbonMenuItem rmi = RibbonWindowsMenu2.Items[i] as RibbonMenuItem;
                    if (rmi != null)
                    {
                        if (rmi.Header == null) continue;
                        if (rmi.Header.ToString() == defaultTitle)
                        {
                            rmi.Header = child.Title;
                            this.RibbonMenuItem_Click(rmi, new RoutedEventArgs(System.Windows.Controls.MenuItem.ClickEvent, rmi));
                            break;
                        }
                    }
                }
            }
        }

        protected void distributeMsg_ControlChangeTitle(object sender, string defaultTitle, MdiChild child)
        {
            //HARRY 2014-03-18
            if (AppFlag.bRobbin == false)
            {
                for (int i = 0; i < this.miOpen.Items.Count; i++)
                {
                    MenuItem rmi = miOpen.Items[i] as MenuItem;
                    if (rmi != null)
                    {
                        if (rmi.Header == null) continue;
                        if (rmi.Header.ToString() == defaultTitle)
                        {
                            rmi.Header = child.Title;
                            // this.ActiveMenuItem_Click(rmi, new RoutedEventArgs(System.Windows.Controls.MenuItem.ClickEvent, rmi));
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < RibbonWindowsMenu.Items.Count; i++)
                {
                    RibbonMenuItem rmi = RibbonWindowsMenu.Items[i] as RibbonMenuItem;
                    if (rmi != null)
                    {
                        if (rmi.Header.ToString() == defaultTitle)
                        {
                            rmi.Header = child.Title;
                            break;
                        }
                    }
                }

                for (int i = 0; i < RibbonWindowsMenu2.Items.Count; i++)
                {
                    RibbonMenuItem rmi = RibbonWindowsMenu2.Items[i] as RibbonMenuItem;
                    if (rmi != null)
                    {
                        if (rmi.Header.ToString() == defaultTitle)
                        {
                            rmi.Header = child.Title;
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Heartbeat

        protected void channel_SendHeartBeat(object sender, string type)
        {
            if (type == AppFlag.IPOMT + ":" + AppFlag.PortOMT)
            {
                timeOutTimerOMT.Start();
                heartBeatStatusOMT = false;
                TradeStationSend.Send(cmdClient.getMsgHeartBeat);
                TradeStationLog.WriteForHearbeatandMarginCall(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " Send OMT HeartBeat.");
            }
            else
            {
                timeOutTimerOMP.Start();
                heartBeatStatusOMP = false;
                TradeStationSend.Send(cmdClient.getMsgHeartBeatOMP);
                TradeStationLog.WriteForHearbeatandMarginCall(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " Send OMP HeartBeat.");
            }
        }

        //OMT
        private void OnTimeOutEvent(object source, ElapsedEventArgs e)
        {
            timeOutTimerOMT.Enabled = false;
            if (heartBeatStatusOMT == false)
            {
                //Close connection 
                TradeStationLog.WriteForHearbeatandMarginCall(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " a. OMT TimeOut.");
                if (_msgChannel != null)
                {
                    _msgChannel.EnsureDisconnected();
                    _msgChannelOMP.EnsureDisconnected();
                    TradeStationLog.WriteForHearbeatandMarginCall(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " b. OMT EnsureConnection for TimeOut.");
                }
            }
        }

        //OMP
        private void OnTimeOutEventOMP(object source, ElapsedEventArgs e)
        {
            timeOutTimerOMP.Enabled = false;
            if (heartBeatStatusOMP == false)
            {
                //Close connection 
                TradeStationLog.WriteForHearbeatandMarginCall(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " a. OMP TimeOut.");
                if (_msgChannelOMP != null)
                {
                    _msgChannel.EnsureDisconnected();
                    _msgChannelOMP.EnsureDisconnected();
                    TradeStationLog.WriteForHearbeatandMarginCall(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " b. OMP EnsureConnection for TimeOut.");
                }
            }
        }

        #endregion

        #region channel

        private void CheckRedundOMT()
        {
            if (_msgChannel != null)
            {
                //_msgChannel.SendMessageSuccess += new MessageChannel.OnSendMessageSuccess(channel_SendMessageSuccess);
                _msgChannel.ReceiveMessageSuccessUsingStream += new MessageChannel.OnReceiveMessageSuccessUsingStream(channel_ReceiveMessageSuccessUsingStream);
                _msgChannel.Error += new MessageChannel.OnError(channel_Error);
                _msgChannel.Connecting += new MessageChannel.OnConnecting(channel_Connecting);
                _msgChannel.Connected += new MessageChannel.OnConnected(channel_Connected);
                _msgChannel.Disconnected += new MessageChannel.OnDisconnected(channel_Disconnected);
                _msgChannel.SendHeartBeat += new MessageChannel.OnSendHeartBeat(channel_SendHeartBeat);

                while (true)
                {
                    try
                    {
                        _msgChannel.Read();
                    }
                    catch (Exception exp)
                    {
                        TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  _msgChannel.Read(),error: " + exp.ToString());
                        break;
                    }
                }
            }
        }

        private void CheckRedundOMP()
        {
            if (_msgChannelOMP != null)
            {
                //_msgChannelOMP.SendMessageSuccess += new MessageChannel.OnSendMessageSuccess(channel_SendMessageSuccessOMP);
                _msgChannelOMP.ReceiveMessageSuccessUsingStream += new MessageChannel.OnReceiveMessageSuccessUsingStream(channel_ReceiveMessageSuccessUsingStreamOMP);
                _msgChannelOMP.Error += new MessageChannel.OnError(channel_ErrorOMP);
                _msgChannelOMP.Connecting += new MessageChannel.OnConnecting(channel_ConnectingOMP);
                _msgChannelOMP.Connected += new MessageChannel.OnConnected(channel_ConnectedOMP);
                _msgChannelOMP.Disconnected += new MessageChannel.OnDisconnected(channel_DisconnectedOMP);
                _msgChannelOMP.SendHeartBeat += new MessageChannel.OnSendHeartBeat(channel_SendHeartBeat);

                _msgChannelOMP.AsynConnect();

                while (true)
                {
                    try
                    {
                        _msgChannelOMP.Read();
                    }
                    catch (Exception exp)
                    {
                        TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  _msgChannelOMP.Read(),error: " + exp.ToString());
                        break;
                    }
                }
            }
        }

        //OMP
        protected void channel_ReceiveMessageSuccessUsingStreamOMP(object sender, MemoryStream responseData)
        {
            bool reLogin = false;
            TradeStationComm.MsgResponse.ResponseObject theMsgObj = TradeStationComm.MsgResponse.getResponseAnyMsg(responseData);
            if (theMsgObj != null)
            {
                TradeStationComm.infoClass.infoBasic theInfo = theMsgObj.InfoObject as TradeStationComm.infoClass.infoBasic;
                if (theInfo is TradeStationComm.infoClass.infoBasic && theInfo != null)
                {
                    switch (theInfo.ErrorCode)
                    {
                        case (int)TradeStationComm.MsgResponse.errCodeDef.invalidSessionHash:
                            {
                                //show login form
                                System.Windows.Application.Current.Dispatcher.BeginInvoke(this.InfoDelegate, new Object[] { " Another User Logined! " });
                            }
                            break;
                        case (int)TradeStationComm.MsgResponse.errCodeDef.ok:

                            break;
                    }
                }

                if (reLogin) return;

                switch (theMsgObj.ResponseType)
                {
                    case TradeStationComm.MsgResponse.responseType.heartbeat:
                        timeOutTimerOMP.Enabled = false;
                        heartBeatStatusOMP = true;
                        break;
                    default:
                        if (_distributeMsg != null)
                        {
                            //_distributeMsg.responseObject = theMsgObj;
                            _distributeMsg.DistributeMsg(theMsgObj);
                        }
                        break;
                }
            }
        }

        //OMT
        protected void channel_ReceiveMessageSuccessUsingStream(object sender, MemoryStream responseData)
        {
            // bool reLogin = false;
            TradeStationComm.MsgResponse.ResponseObject theMsgObj = TradeStationComm.MsgResponse.getResponseAnyMsg(responseData);
            if (theMsgObj != null)
            {
                TradeStationComm.infoClass.infoBasic theInfo = theMsgObj.InfoObject as TradeStationComm.infoClass.infoBasic;
                if (theInfo is TradeStationComm.infoClass.infoBasic && theInfo != null)
                {
                    switch (theInfo.ErrorCode)
                    {
                        case (int)TradeStationComm.MsgResponse.errCodeDef.invalidSessionHash:
                            //show login form
                            System.Windows.Application.Current.Dispatcher.BeginInvoke(this.InfoDelegate, new Object[] { " Another User Logined! " });

                            break;
                        case (int)TradeStationComm.MsgResponse.errCodeDef.ok:

                            break;
                    }
                }

                //if (reLogin) return;
                switch (theMsgObj.ResponseType)
                {
                    case TradeStationComm.MsgResponse.responseType.login:
                        TradeStationComm.infoClass.LoginResponse infoLoginResponse = theMsgObj.InfoObject as TradeStationComm.infoClass.LoginResponse;
                        if (infoLoginResponse != null)
                        {
                            LoginResp data = infoLoginResponse.GetLoginResponse(infoLoginResponse.InfoItem); ;
                            if (data != null && data.Result)
                            {
                                TradeStationComm.SessionHash = data.SessionHash;
                                TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + "  Access to OMT!");
                                System.Windows.Application.Current.Dispatcher.BeginInvoke(this.InfoDelegate, new Object[] { " Access to OMT!" });
                            }
                        }
                        break;
                    //if (theInfo.IsSuccess)
                    //{
                    //    TradeStationComm.SessionHash = theInfo.ResponseString;
                    //    TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  Access to OMT!");
                    //    System.Windows.Application.Current.Dispatcher.BeginInvoke(this.InfoDelegate, new Object[] { " Access to OMT!" });
                    //}
                    //break;
                    case TradeStationComm.MsgResponse.responseType.heartbeat:
                        timeOutTimerOMT.Enabled = false;
                        heartBeatStatusOMT = true;
                        break;
                    default:
                        if (_distributeMsg != null)
                        {
                            //  _distributeMsg.responseObject = theMsgObj;
                            _distributeMsg.DistributeMsg(theMsgObj);
                        }
                        break;
                }
            }
        }

        //OMP
        protected void channel_ConnectedOMP(object sender)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(this.InfoDelegate, new Object[] { " Access to OMP!" });
            TradeStationSend.Send(cmdClient.getMsgHeartBeatOMP);
            TradeStationLog.WriteForHearbeatandMarginCall(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " Send OMP HeartBeat..");

            TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + AppFlag.IPOMP + " OMP Connected.");
        }

        //OMT
        protected void channel_Connected(object sender)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(this.InfoDelegate, new Object[] { " OMT Connected!" });
            TradeStationSend.Send(cmdClient.getMsgHeartBeat);
            TradeStationLog.WriteForHearbeatandMarginCall(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " Send OMT HeartBeat.");

            TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + AppFlag.IPOMT + " OMT Connected.");
        }

        void BindListInfoMethod(string strStatus)
        {
            try
            {
                if (strStatus == " Another User Logined! " || strStatus == " OMP Disconnected!" || strStatus == " OMT Disconnected!" || strStatus == " Force logout!" || strStatus == " Please login again!")
                {
                    if (UnLoadLogout())
                    {
                        Container.Children.Clear();
                        _original_title = "GOSTrade Station" + strVersion + " " + AppFlag.BuildDate + " - " + "[" + AppFlag.IPOMT + "] ";//+ _tw;
                        SetMenuBG("FFFFFF");

                        this.RibbonBTLogout.Visibility = Visibility.Hidden; ;
                        this.RibbonBTLogin.Visibility = Visibility.Visible; ;
                        this.RibbonMTabUseful.Visibility = Visibility.Hidden;
                        this.RibbonMTabAcount.Visibility = Visibility.Hidden;
                        this.RibbonMTabMarket.Visibility = Visibility.Hidden;
                        this.RibbonMTabOrders.Visibility = Visibility.Hidden;
                        this.RibbonMTabTrades.Visibility = Visibility.Hidden;
                        this.RibbonTabTools.Visibility = Visibility.Hidden;
                        this.RibbonPrefer.Visibility = Visibility.Hidden;
                        this.RibbonDesktop.Visibility = Visibility.Hidden;
                        this.RibbonHelp.Visibility = Visibility.Hidden;
                        this.RibbonMTabDealer.Visibility = Visibility.Hidden;
                        this.RibbonBTChartAnalysisSCI.Visibility = Visibility.Hidden;
                        this.RibbonBTChartAnalysisSCITradesTab.Visibility = Visibility.Hidden;
                        this.RibbonMain.SelectedIndex = 0;

                        //harry ,new menu
                        if (AppFlag.bRobbin == false)
                        {
                            switchShowMenu(false);

                        }

                        AccountLogin_Click(null, null);
                    }
                }
                else
                {
                    if (!strStatus.Equals(" Access to OMP!") || _IsWindowInitialized) return;

                    TradeStationSend.Send(cmdClient.changeLanguage, (AppFlag.DefaultLanguage == "en-US") ? TradeStationComm.Attribute.Language.English : (AppFlag.DefaultLanguage == "zh-CN") ? TradeStationComm.Attribute.Language.ChineseSimplified : TradeStationComm.Attribute.Language.ChinesTraditional);

                    //if (GOSTradeStation.marketPriceData == null || GOSTradeStation.marketPriceData.ProdListTable == null || GOSTradeStation.marketPriceData.InstListTable == null)
                    //{
                    //    TradeStationSend.Send(cmdClient.getProductList);
                    //    TradeStationSend.Send(cmdClient.getInstrumentList); 
                    //}

                    ////Clear register
                    //TradeStationSend.ClearRegister(cmdClient.registerMarketPrice);
                    //TradeStationSend.ClearRegister(cmdClient.registerPriceDepth);
                    //TradeStationSend.ClearRegister(cmdClient.registerTicker);

                }
                this.lbStatus.Content = strStatus;
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " BindListInfoMethod(string strStatus) Status:" + strStatus + "  Error:: " + exp.ToString());
            }
        }

        protected void channel_Disconnected(object sender)
        {
            TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + AppFlag.IPOMT + " OMT Disconnected.....  ");
            System.Windows.Application.Current.Dispatcher.BeginInvoke(this.InfoDelegate, new Object[] { " OMT Disconnected!!" });
        }
        protected void channel_DisconnectedOMP(object sender)
        {
            TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + AppFlag.IPOMP + " OMP Disconnected.....  ");
            System.Windows.Application.Current.Dispatcher.BeginInvoke(this.InfoDelegate, new Object[] { " OMP Disconnected!!" });
        }

        protected void channel_Connecting(object sender)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(this.InfoDelegate, new Object[] { "OMT Connecting...." });
            TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + AppFlag.IPOMT + " OMT Connecting.....  ");
        }
        protected void channel_ConnectingOMP(object sender)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(this.InfoDelegate, new Object[] { "OMP Connecting...." });
            TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + AppFlag.IPOMP + " OMP Connecting...  ");
        }

        protected void channel_Error(object sender, Exception ex)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(this.InfoDelegate, new Object[] { " OMT Disconnected!" });
            TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + AppFlag.IPOMT + " OMT Error:: " + ex.ToString());
        }
        protected void channel_ErrorOMP(object sender, Exception ex)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(this.InfoDelegate, new Object[] { " OMP Disconnected!" });
            TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + AppFlag.IPOMP + " OMP Error:: " + ex.ToString());
        }

        #endregion

        #region Mdi-like title

        string _original_title;

        void Container_MdiChildTitleChanged(object sender, RoutedEventArgs e)
        {
            if (Container.ActiveMdiChild != null && Container.ActiveMdiChild.WindowState == WindowState.Maximized)
                Title = _original_title + " - [" + Container.ActiveMdiChild.Title + "]";
            else
                Title = _original_title;
        }

        int countMarketPrice = 1;

        #endregion

        #region Menu Events

        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AccountLogin_Click(object sender, RoutedEventArgs e)
        {
            if (mcLogin == null || !mcLogin.ExistInContainer(mcLogin, Container))
            {
                //GosCulture.CultureHelper.ChangeLanguage(AppFlag.DefaultLanguage);

                LoginIn login = new LoginIn();
                login.LoginReslut += handle_LoginReslut;
                mcLogin = new MdiChild();
                login.mdiChild = mcLogin;
                mcLogin.Title = TradeStationSetting.ReturnWindowName(WindowTypes.Login, "");
                mcLogin.Content = login;
                mcLogin.Width = login.Width;
                mcLogin.Height = login.Height;
                double x = Container.ActualWidth / 2 - login.Width / 2;
                double y = Container.ActualHeight / 2 - login.Height / 1.8;
                if (x < 0) x = 0;
                if (y < 0) y = 0;
                mcLogin.Position = new System.Windows.Point(x, y);
                mcLogin.MaximizeBox = false;

                container.Children.Add(mcLogin);
            }
        }

        private void AccountLogout_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show(AlertLogout,
                            AlertTilte, System.Windows.Forms.MessageBoxButtons.YesNo,
                            System.Windows.Forms.MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
            {
                if (UnLoadLogout())
                {
                    Container.Children.Clear();
                    this.lbStatus.Content = "Logout!";
                    Title = _original_title = "GOSTrade Station" + strVersion + " " + AppFlag.BuildDate;
                    SetMenuBG("FFFFFF");

                    this.RibbonBTLogout.Visibility = Visibility.Hidden;
                    this.RibbonBTLogin.Visibility = Visibility.Visible;
                    this.RibbonMTabUseful.Visibility = Visibility.Hidden;
                    this.RibbonMTabAcount.Visibility = Visibility.Hidden;
                    this.RibbonMTabMarket.Visibility = Visibility.Hidden;
                    this.RibbonMTabOrders.Visibility = Visibility.Hidden;
                    this.RibbonMTabTrades.Visibility = Visibility.Hidden;
                    this.RibbonTabTools.Visibility = Visibility.Hidden;
                    this.RibbonPrefer.Visibility = Visibility.Hidden;
                    this.RibbonDesktop.Visibility = Visibility.Hidden;
                    this.RibbonHelp.Visibility = Visibility.Hidden;
                    this.RibbonMTabDealer.Visibility = Visibility.Hidden;
                    this.RibbonBTChartAnalysisSCI.Visibility = Visibility.Hidden;
                    this.RibbonBTChartAnalysisSCITradesTab.Visibility = Visibility.Hidden;
                    this.RibbonMain.SelectedIndex = 0;

                    //harry ,new menu
                    switchShowMenu(false);
                }
            }
        }

        private void TradeStatistic_Click(object sender, RoutedEventArgs e)
        {
            TradeStatistic statistic = new TradeStatistic(_distributeMsg);
            MdiChild mcStatistic = new MdiChild();
            //statistic.mdiChild = mcStatistic;
            mcStatistic.Title = "Trade Statistic - [" + _UserID + "]";
            mcStatistic.Content = statistic;
            mcStatistic.Width = 600;
            mcStatistic.Height = 180;
            if (!mcStatistic.ExistInContainer(mcStatistic, Container))
            {
                Container.Children.Add(mcStatistic);
            }
        }

        private void Ticker_Click(object sender, RoutedEventArgs e)
        {
            Ticker ticker = new Ticker(_distributeMsg);
            ticker.Locked = false;
            MdiChild mcTicker = new MdiChild();
            ticker.mdiChild = mcTicker;
            mcTicker.Title = TradeStationSetting.ReturnWindowName(WindowTypes.Ticker, "");
            mcTicker.Content = ticker;
            //mcTicker.Width = ticker.Width;
            //mcTicker.Height = ticker.Height;
            mcTicker.Width = 310;
            mcTicker.Height = 140;
            if (!mcTicker.ExistInContainer(mcTicker, Container))
            {
                Container.Children.Add(mcTicker);
            }
        }

        private void MarketPrice_Click(object sender, RoutedEventArgs e)
        {
            MarketPriceControl marketPrice = new MarketPriceControl(_distributeMsg);
            MdiChild mcMarketPrice = new MdiChild();
            marketPrice.mdiChild = mcMarketPrice;
            marketPrice.countSymbol = (countMarketPrice++).ToString();
            mcMarketPrice.Title = TradeStationSetting.ReturnWindowName(WindowTypes.MarketPriceControl, marketPrice.countSymbol);
            mcMarketPrice.Content = marketPrice;
            //mcMarketPrice.Width = 650;
            //mcMarketPrice.Height = 140;
            mcMarketPrice.Width = marketPrice.Width;
            mcMarketPrice.Height = marketPrice.Height;
            if (!mcMarketPrice.ExistInContainer(mcMarketPrice, Container))
            {
                Container.Children.Add(mcMarketPrice);
            }
        }

        private void QuickTrade_Click(object sender, RoutedEventArgs e)
        {
            MdiChild orderChild = new MdiChild();
            OrderEntry orderEntry = new OrderEntry(_distributeMsg, orderChild);
            orderEntry.tabc.SelectedIndex = 1;
            orderEntry.mdiChild = orderChild;
            //  orderChild.Title = WindowTypes.OrderEntry.ToString();
            orderChild.Content = orderEntry;
            orderChild.Width = 312;// orderEntry.Width;
            orderChild.Height = 250;
            orderEntry.initCtrlData();
            orderEntry.LoadLastProduct();
            //orderEntry.SetTradeType(1);
            Container.Children.Add(orderChild);
        }

        private void BuyAndSale_Click(object sender, RoutedEventArgs e)
        {
            MdiChild orderChild = new MdiChild();
            OrderEntry orderEntry = new OrderEntry(_distributeMsg, orderChild);

            orderEntry.mdiChild = orderChild;
            //   orderChild.Title = WindowTypes.OrderEntry.ToString();
            orderChild.Content = orderEntry;
            orderChild.Width = 312;// orderEntry.Width;
            orderChild.Height = 250; //orderEntry.Height;
            orderEntry.initCtrlData();
            orderEntry.LoadLastProduct();
            Container.Children.Add(orderChild);
        }

        //Account_Click
        private void Account_Click(object sender, RoutedEventArgs e)
        {
            MdiChild AccountInfoChild = new MdiChild();
            AccountInfo AccountInfo = new AccountInfo(_distributeMsg, AccountInfoChild);

            //  AccountInfoChild.Title = "Account Info ";// _UserID + "- " + WindowTypes.AccountInfo;
            AccountInfoChild.Content = AccountInfo;
            //  AccountInfo.mdiChild = AccountInfoChild;
            AccountInfoChild.Width = 312;//AccountInfo.Width;
            AccountInfoChild.Height = 195;// AccountInfo.Height;
            AccountInfoChild.Position = new System.Windows.Point(0, 0);
            AccountInfo.initCtrlData();
            if (!AccountInfoChild.ExistInContainer(AccountInfoChild, Container))
            {
                Container.Children.Add(AccountInfoChild);
            }

        }

        private void Records_Click(object sender, RoutedEventArgs e)
        {
            MdiChild orderInfoChild = new MdiChild();
            UserOrderInfo OrderInfo = new UserOrderInfo(_distributeMsg, orderInfoChild);
            // orderInfoChild.Title = _UserID + "- " + WindowTypes.UserOrderInfo;
            orderInfoChild.Content = OrderInfo;
            orderInfoChild.Width = 454;
            orderInfoChild.Height = 397;
            orderInfoChild.Position = new System.Windows.Point(0, 299);
            boolWindowOrderBook = true;
            OrderInfo.initCtrlData();
            if (!orderInfoChild.ExistInContainer(orderInfoChild, Container))
            {
                Container.Children.Add(orderInfoChild);
            }
        }

        private void PosSumary_Click(object sender, RoutedEventArgs e)
        {
            //UserOrderInfo OrderInfo = new UserOrderInfo(_distributeMsg);
            MdiChild orderInfoChild = new MdiChild();
            PosSumary pos = new PosSumary(_distributeMsg, orderInfoChild);
            // orderInfoChild.Title ="Position Summary";
            orderInfoChild.Content = pos;
            orderInfoChild.Width = 300;
            orderInfoChild.Height = 260;
            orderInfoChild.Position = new System.Windows.Point(0, 199);
            pos.initCtrlData();
            boolWindowOrderBook = true;
            if (!orderInfoChild.ExistInContainer(orderInfoChild, Container))
            {
                Container.Children.Add(orderInfoChild);
            }
        }

        //2015-01-01
        private void DoneTrade_Click(object sender, RoutedEventArgs e)
        {
            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
            MdiChild MdiChildOrHist = new MdiChild();
            GOSTS.Accounts.DoneTrade oh = new Accounts.DoneTrade(MdiChildOrHist, _distributeMsg);

            MdiChildOrHist.Width = 980;
            MdiChildOrHist.Height = 410;

            MdiChildOrHist.Position = new System.Windows.Point(50, 55);
            MdiChildOrHist.Content = oh;
            if (!Container.Children.Contains(MdiChildOrHist))
            {
                Container.Children.Add(MdiChildOrHist);
            }
            MdiChildOrHist.Focus();

        }

        private void PriceDepth_Click(object sender, RoutedEventArgs e)
        {
            PriceDepth priceDepth = new PriceDepth(_distributeMsg);
            priceDepth.Locked = false;
            MdiChild mcPriceDepth = new MdiChild();
            priceDepth.mdiChild = mcPriceDepth;
            mcPriceDepth.Title = TradeStationSetting.ReturnWindowName(WindowTypes.PriceDepth, "");
            mcPriceDepth.Content = priceDepth;
            mcPriceDepth.Width = 650;
            mcPriceDepth.Height = priceDepth.Height;
            //mcPriceDepth.WindowState = WindowState.Normal;
            if (!mcPriceDepth.ExistInContainer(mcPriceDepth, Container))
            {
                Container.Children.Add(mcPriceDepth);
            }
        }

        private void LongPriceDepth_Click(object sender, RoutedEventArgs e)
        {
            MdiChild mcLongPriceDepth = new MdiChild();
            //_distributeMsg.MdiChild = mcLongPriceDepth;
            LongPriceDepth longPriceDepth = new LongPriceDepth(_distributeMsg);
            longPriceDepth.Locked = false;
            longPriceDepth.mdiChild = mcLongPriceDepth;
            //MdiChild mcLongPriceDepth = new MdiChild();
            mcLongPriceDepth.Title = "Long Price Depth";
            mcLongPriceDepth.Content = longPriceDepth;
            mcLongPriceDepth.Width = 300;
            mcLongPriceDepth.Height = 180;
            if (!mcLongPriceDepth.ExistInContainer(mcLongPriceDepth, Container))
            {
                Container.Children.Add(mcLongPriceDepth);
            }
        }

        private void MarginCheck_Click(object sender, RoutedEventArgs e)
        {
            MdiChild mcMarginCheck = new MdiChild();
            MarginCheck marginCheck = new MarginCheck(_distributeMsg);
            marginCheck.mdiChild = mcMarginCheck;
            mcMarginCheck.Title = TradeStationSetting.ReturnWindowName(WindowTypes.MarginCheck, "");
            mcMarginCheck.Content = marginCheck;
            mcMarginCheck.Width = 800;
            mcMarginCheck.Height = 300;
            //mcMarginCheck.Position = new System.Windows.Point(Container.ActualWidth - 800, Container.ActualHeight - 294 - 200);
            mcMarginCheck.Position = new System.Windows.Point((((container.ActualWidth - Mouse.GetPosition(container).X) > mcMarginCheck.Width) ? Mouse.GetPosition(container).X : container.ActualWidth - 800), 0);

            if (!mcMarginCheck.ExistInContainer(mcMarginCheck, Container))
            {
                Container.Children.Add(mcMarginCheck);
            }
        }

        private void TradeConfirmation_Click(object sender, RoutedEventArgs e)
        {
            MdiChild mcTradeConfirm = new MdiChild();
            TradeConfirmation tradeConfirm = new TradeConfirmation(_distributeMsg);
            tradeConfirm.mdiChild = mcTradeConfirm;
            mcTradeConfirm.Title = TradeStationSetting.ReturnWindowName(WindowTypes.TradeConfirmation, "");
            mcTradeConfirm.Content = tradeConfirm;
            mcTradeConfirm.Width = 800;// tradeConfirm.Width;
            mcTradeConfirm.Height = 300;// tradeConfirm.Height;
            mcTradeConfirm.Position = new System.Windows.Point((((container.ActualWidth - Mouse.GetPosition(container).X) > mcTradeConfirm.Width) ? Mouse.GetPosition(container).X : container.ActualWidth - 800), 0);
            if (!mcTradeConfirm.ExistInContainer(mcTradeConfirm, Container))
            {
                Container.Children.Add(mcTradeConfirm);
            }
        }

        private void MarginCallList_Click(object sender, RoutedEventArgs e)
        {
            foreach (MdiChild child in Container.Children)
            {
                if (child.Title == GOSTS.GosCulture.CultureHelper.GetString("WinTitleMCL"))
                {
                    child.Position = new System.Windows.Point(Container.ActualWidth - child.Width, Container.ActualHeight - child.Height);
                    child.Focus();
                    break;
                }
            }
        }

        private void Clock_Click(object sender, RoutedEventArgs e)
        {
            Clock clock = new Clock();
            MdiChild mcClock = new MdiChild();
            clock.mdiChild = mcClock;
            mcClock.Title = TradeStationSetting.ReturnWindowName(WindowTypes.Clock, "");
            mcClock.Content = clock;
            mcClock.Width = 100;// clock.Width;
            mcClock.Height = 80;// clock.Height;
            mcClock.Position = Mouse.GetPosition(this);

            if (!mcClock.ExistInContainer(mcClock, Container))
            {
                Container.Children.Add(mcClock);
            }
        }

        private void Customize_Click(object sender, RoutedEventArgs e)
        {
            AppPreference appPreference = new AppPreference();
            MdiChild mcAppPreference = new MdiChild();
            appPreference.mdiChild = mcAppPreference;
            mcAppPreference.Title = TradeStationSetting.ReturnWindowName(WindowTypes.AppPreference, GOSTradeStation.UserID);
            mcAppPreference.Content = appPreference;
            mcAppPreference.Width = double.NaN;
            mcAppPreference.Height = double.NaN;
            mcAppPreference.Position = new Point(300, 40);

            if (!mcAppPreference.ExistInContainer(mcAppPreference, Container))
            {
                Container.Children.Add(mcAppPreference);
            }
        }

        private void TradeChart_Click(object sender, RoutedEventArgs e)
        {
            MdiChild sciMDIChild = new MdiChild();
            SCIChartAnalysis sciChart = new SCIChartAnalysis(_distributeMsg, sciMDIChild, "");
            sciMDIChild.Content = sciChart;
            sciMDIChild.Width = 1000;
            sciMDIChild.Height = 500;
            sciMDIChild.Position = new System.Windows.Point(50, 50);
            Container.Children.Add(sciMDIChild);
        }

        private void OptionMaster_Click(object sender, RoutedEventArgs e)
        {
            OptionMaster optionMaster = new OptionMaster(_distributeMsg);
            MdiChild mcOptionMaster = new MdiChild();
            optionMaster.mdiChild = mcOptionMaster;
            mcOptionMaster.Title = TradeStationSetting.ReturnWindowName(WindowTypes.OptionMaster, "");
            mcOptionMaster.Content = optionMaster;
            mcOptionMaster.Width = optionMaster.Width;
            mcOptionMaster.Height = optionMaster.Height;

            if (!mcOptionMaster.ExistInContainer(mcOptionMaster, Container))
            {
                Container.Children.Add(mcOptionMaster);
            }
        }

        private void PriceAlert_Click(object sender, RoutedEventArgs e)
        {
            MdiChild mcPriceAlert = new MdiChild();
            PriceAlert priceAlert = new PriceAlert(_distributeMsg);
            priceAlert.mdiChild = mcPriceAlert;
            mcPriceAlert.Title = TradeStationSetting.ReturnWindowName(WindowTypes.PriceAlert, "");
            mcPriceAlert.Content = priceAlert;
            mcPriceAlert.Width = 500;// priceAlert.Width;
            mcPriceAlert.Height = 300; //priceAlert.Height;
            mcPriceAlert.Position = new System.Windows.Point(Mouse.GetPosition(this).X, Mouse.GetPosition(this).Y);

            if (!mcPriceAlert.ExistInContainer(mcPriceAlert, Container))
            {
                Container.Children.Add(mcPriceAlert);
            }
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            GosBzTool.OpenCHPWD(_distributeMsg);
        }

        bool bTestInstClick = false;
        private void TestInst_Click(object sender, RoutedEventArgs e)
        {
            // GosBzTool.OpenCHPWD(_distributeMsg);
            TraderTest test = TraderTest.getTraderTest();
            if (test.State == 0)// (bTestInstClick == false)
            {
                //bTestInstClick = true;                              
                test.Test();
                this.btnTestInst.Header = "Pause Test";
            }
            else if (test.State == 1)
            {
                test.Pause();
                this.btnTestInst.Header = "Continue Test";
            }
            else if (test.State == 2)
            {
                test.Resume();
                this.btnTestInst.Header = "Pause Test";
            }

        }

        private void TEST_Click(object sender, RoutedEventArgs e)
        {
            List<String> ls = new List<string>();
            ls.Add("HSIU3");
            TradeStationSend.Get(ls, cmdClient.getTicker);
        }
        private void TEST2_Click(object sender, RoutedEventArgs e)
        {
            TradeStationSend.Send(cmdClient.getMsgLogout);
        }

        /// <summary>
        /// Handles the Click event of the 'Fixed window' menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddFixedWindow_Click(object sender, RoutedEventArgs e)
        {
            //   Container.Children.Add(new MdiChild { Content = new System.Windows.Controls.Label { Content = "Fixed width window" }, Title = "Window " + ooo++, Resizable = false });
        }

        /// <summary>
        /// Handles the Click event of the 'Scroll window' menu item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddScrollWindow_Click(object sender, RoutedEventArgs e)
        {
            StackPanel sp = new StackPanel { Orientation = System.Windows.Controls.Orientation.Vertical };
            sp.Children.Add(new TextBlock { Text = "Window with scroll", Margin = new Thickness(5) });
            sp.Children.Add(new System.Windows.Controls.ComboBox { Margin = new Thickness(20), Width = 300 });
            ScrollViewer sv = new ScrollViewer { Content = sp, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto };

            Container.Children.Add(new MdiChild { Content = sv, Title = "Window " + countMarketPrice++ });
        }

        #endregion

        #region Handles menu windows
        /// <summary>
        /// Refresh windows list
        /// </summary> 
        void Menu_RefreshWindows()
        {
            if (AppFlag.bRobbin == false)
            {
                Menu_ShowOpen();
                return;
            }

            this.RibbonWindowsMenu.Items.Clear();
            RibbonMenuItem RMIWindows;
            for (int i = 0; i < Container.Children.Count; i++)
            {
                MdiChild child = Container.Children[i];
                RMIWindows = new RibbonMenuItem { Header = child.Title };
                RMIWindows.Click += new RoutedEventHandler(this.RibbonMenuItem_Click);
                RibbonWindowsMenu.Items.Add(RMIWindows);
            }
            RibbonWindowsMenu.Items.Add(new Separator());
            //RibbonWindowsMenu.Items.Add(RMIWindows = new RibbonMenuItem { Header = "Cascade" });
            //RMIWindows.Click += (o, e) => Container.MdiLayout = WPF.MDI.MdiLayout.Cascade;
            RibbonWindowsMenu.Items.Add(RMIWindows = new RibbonMenuItem { Header = "Horizontally" });
            RMIWindows.Click += (o, e) => Container.MdiLayout = WPF.MDI.MdiLayout.TileHorizontal;
            RibbonWindowsMenu.Items.Add(RMIWindows = new RibbonMenuItem { Header = "Vertically" });
            RMIWindows.Click += (o, e) => Container.MdiLayout = WPF.MDI.MdiLayout.TileVertical;

            RibbonWindowsMenu.Items.Add(new Separator());
            RibbonWindowsMenu.Items.Add(RMIWindows = new RibbonMenuItem { Header = "Close all" });
            RMIWindows.Click += (o, e) => Container.Children.Clear();

            this.RibbonWindowsMenu2.Items.Clear();
            RibbonMenuItem RMIWindows2;
            for (int i = 0; i < Container.Children.Count; i++)
            {
                MdiChild child = Container.Children[i];
                RMIWindows2 = new RibbonMenuItem { Header = child.Title };
                RMIWindows2.Click += new RoutedEventHandler(this.RibbonMenuItem_Click2);
                RibbonWindowsMenu2.Items.Add(RMIWindows2);
            }
            RibbonWindowsMenu2.Items.Add(new Separator());
            //RibbonWindowsMenu.Items.Add(RMIWindows = new RibbonMenuItem { Header = "Cascade" });
            //RMIWindows.Click += (o, e) => Container.MdiLayout = WPF.MDI.MdiLayout.Cascade;
            RibbonWindowsMenu2.Items.Add(RMIWindows2 = new RibbonMenuItem { Header = "Horizontally" });
            RMIWindows2.Click += (o, e) => Container.MdiLayout = WPF.MDI.MdiLayout.TileHorizontal;
            RibbonWindowsMenu2.Items.Add(RMIWindows2 = new RibbonMenuItem { Header = "Vertically" });
            RMIWindows2.Click += (o, e) => Container.MdiLayout = WPF.MDI.MdiLayout.TileVertical;

            RibbonWindowsMenu2.Items.Add(new Separator());
            RibbonWindowsMenu2.Items.Add(RMIWindows2 = new RibbonMenuItem { Header = "Close all" });
            RMIWindows2.Click += (o, e) => Container.Children.Clear();

        }

        private void RibbonWindowsMenu_DropDownOpened(object sender, EventArgs e)
        {
            if (Container.ActiveMdiChild != null)
            {
                for (int i = 0; i < RibbonWindowsMenu.Items.Count; i++)
                {
                    RibbonMenuItem rmi = RibbonWindowsMenu.Items[i] as RibbonMenuItem;
                    if (rmi != null)
                    {
                        if (rmi.Header == null) return;
                        if (rmi.Header.ToString() == Container.ActiveMdiChild.Title)
                        {
                            rmi.IsChecked = true;
                        }
                        else
                        {
                            rmi.IsChecked = false;
                        }
                    }
                }
            }
        }

        private void RibbonWindowsMenu_DropDownOpened2(object sender, EventArgs e)
        {
            if (Container.ActiveMdiChild != null)
            {
                for (int i = 0; i < RibbonWindowsMenu2.Items.Count; i++)
                {
                    RibbonMenuItem rmi = RibbonWindowsMenu2.Items[i] as RibbonMenuItem;
                    if (rmi != null)
                    {
                        if (rmi.Header == null) return;
                        if (rmi.Header.ToString() == Container.ActiveMdiChild.Title)
                        {
                            rmi.IsChecked = true;
                        }
                        else
                        {
                            rmi.IsChecked = false;
                        }
                    }
                }
            }
        }

        private void RibbonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RibbonMenuItem selectedItem = sender as RibbonMenuItem;
            Separator s = selectedItem.Header as Separator;
            if (s == null)
            {
                for (int i = 0; i < RibbonWindowsMenu.Items.Count; i++)
                {
                    RibbonMenuItem rmi = RibbonWindowsMenu.Items[i] as RibbonMenuItem;
                    if (rmi != null)
                    {
                        rmi.IsChecked = false;
                    }
                }
                selectedItem.IsChecked = true;
                for (int i = 0; i < Container.Children.Count; i++)
                {
                    MdiChild child = Container.Children[i];
                    if (child.Title == null || selectedItem.Header == null) return;
                    if (child.Title.IndexOf(selectedItem.Header.ToString()) > -1)
                    {
                        child.Focus();
                    }
                }
            }
            else
            {
                selectedItem.IsChecked = false;
            }
        }

        private void RibbonMenuItem_Click2(object sender, RoutedEventArgs e)
        {
            RibbonMenuItem selectedItem = sender as RibbonMenuItem;
            Separator s = selectedItem.Header as Separator;
            if (s == null)
            {
                for (int i = 0; i < RibbonWindowsMenu2.Items.Count; i++)
                {
                    RibbonMenuItem rmi = RibbonWindowsMenu2.Items[i] as RibbonMenuItem;
                    if (rmi != null)
                    {
                        rmi.IsChecked = false;
                    }
                }
                selectedItem.IsChecked = true;
                for (int i = 0; i < Container.Children.Count; i++)
                {
                    MdiChild child = Container.Children[i];
                    if (child.Title == null || selectedItem.Header == null) return;
                    if (child.Title.IndexOf(selectedItem.Header.ToString()) > -1)
                    {
                        child.Focus();
                    }
                }
            }
            else
            {
                selectedItem.IsChecked = false;
            }
        }

        #endregion

        #region Handles Language

        private void LangChangeUS(object sender, RoutedEventArgs e)
        {
            changeLangInRuntime(TradeStationComm.Attribute.Language.English);
        }

        private void LangChangeCN(object sender, RoutedEventArgs e)
        {
            changeLangInRuntime(TradeStationComm.Attribute.Language.ChineseSimplified);
        }

        private void LangChangeTW(object sender, RoutedEventArgs e)
        {
            changeLangInRuntime(TradeStationComm.Attribute.Language.ChinesTraditional);
        }
        void changeLangInRuntime(TradeStationComm.Attribute.Language lang)
        {
            string strLang = "";
            switch (lang)
            {
                case TradeStationComm.Attribute.Language.English:
                    strLang = "en-US";
                    break;
                case TradeStationComm.Attribute.Language.ChineseSimplified:
                    strLang = "zh-CN";
                    break;
                case TradeStationComm.Attribute.Language.ChinesTraditional:
                    strLang = "zh-TW";
                    break;
            }
            AppFlag.DefaultLanguage = strLang;
            GosCulture.CultureHelper.ChangeLanguage(strLang);
            TradeStationSend.Send(cmdClient.changeLanguage, lang);
            GOSTS.IDelearStatus.ReSendRequst();
            checkLangMenuItem(lang);
        }

        void checkLangMenuItem(TradeStationComm.Attribute.Language lang)
        {
            try
            {
                MizhCN.IsChecked = false;
                MizhTW.IsChecked = false;
                MiEnUS.IsChecked = false;
                switch (lang)
                {
                    case TradeStationComm.Attribute.Language.English:
                        MiEnUS.IsChecked = true;
                        break;
                    case TradeStationComm.Attribute.Language.ChinesTraditional:
                        MizhTW.IsChecked = true;
                        break;
                    case TradeStationComm.Attribute.Language.ChineseSimplified:
                        MizhCN.IsChecked = true;
                        break;
                }
            }
            catch (Exception ex) { }
        }

        void checkLangMenuItem(string strLang)
        {
            strLang = strLang.ToLower().Trim();
            TradeStationComm.Attribute.Language enumLang = TradeStationComm.Attribute.Language.English;
            switch (strLang)
            {
                case "zh-cn":
                    enumLang = TradeStationComm.Attribute.Language.ChineseSimplified;
                    break;
                case "zh-tw":
                    enumLang = TradeStationComm.Attribute.Language.ChinesTraditional;
                    break;
                case "en-us":
                    enumLang = TradeStationComm.Attribute.Language.English;
                    break;
            }

            checkLangMenuItem(enumLang);
        }

        #endregion

        #region menu ctrl
        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            if (toolBar == null) return;
            if (toolBar.HasOverflowItems == false)
            {
                var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
                if (overflowGrid != null)
                {
                    overflowGrid.Visibility = Visibility.Collapsed;
                }

                //var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
                //if (mainPanelBorder != null)
                //{
                //    mainPanelBorder.Margin = new Thickness(0);
                //}
            }
            else
            {
                var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
                if (overflowGrid != null)
                {
                    overflowGrid.Visibility = Visibility.Visible;
                }
            }
        }

        private void ToolBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            if (toolBar == null) return;
            if (toolBar.HasOverflowItems == false)
            {
                var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
                if (overflowGrid != null)
                {
                    overflowGrid.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
                if (overflowGrid != null)
                {
                    overflowGrid.Visibility = Visibility.Visible;
                }
            }
        }


        private void OnMiSubmenuOpened(object sender, RoutedEventArgs e)
        {
            if (Container.ActiveMdiChild != null)
            {
                for (int i = 0; i < this.miOpen.Items.Count; i++)
                {
                    MenuItem rmi = miOpen.Items[i] as MenuItem;
                    if (rmi != null)
                    {
                        bool bSet = false;
                        if (Container.ActiveMdiChild.Title != null)
                        {
                            if (Container.ActiveMdiChild.Title.Trim() != "")
                            {
                                if (rmi.Header == null) continue;
                                if (rmi.Header.ToString() == Container.ActiveMdiChild.Title)
                                {
                                    rmi.IsChecked = true;
                                    bSet = true;
                                }
                            }
                        }
                        if (!bSet)
                        {
                            rmi.IsChecked = false;
                        }
                    }
                }
            }
        }

        void switchShowMenu(bool bLogin)
        {
            if (AppFlag.bRobbin)
            {
                return;
            }
            if (bLogin)
            {
                this.bar1.Visibility = Visibility.Visible;
                this.bar2.Visibility = Visibility.Visible;
                this.bar3.Visibility = Visibility.Visible;
                this.bar4.Visibility = Visibility.Visible;
                this.bar5.Visibility = Visibility.Visible;
                this.bar6.Visibility = Visibility.Visible;
                this.btnLogin.Visibility = Visibility.Collapsed;
                this.btnLogout.Visibility = Visibility.Visible;
            }
            else
            {
                this.bar1.Visibility = Visibility.Hidden;
                this.bar2.Visibility = Visibility.Hidden;
                this.bar3.Visibility = Visibility.Hidden;
                this.bar4.Visibility = Visibility.Hidden;
                this.bar5.Visibility = Visibility.Hidden;
                this.bar6.Visibility = Visibility.Hidden;
                this.btnLogin.Visibility = Visibility.Visible;
                this.btnLogout.Visibility = Visibility.Collapsed;
            }
            SwitchDearControl(GOSTradeStation.isDealer);
        }

        void SwitchDearControl(bool _bDelear)
        {
            if (!_bDelear)
            {
                btnTCF.Visibility = Visibility.Collapsed;
                btnMGCList.Visibility = Visibility.Collapsed;
              //  btnOptionMatrix.Visibility = Visibility.Collapsed;
                btnMGCheck.Visibility = Visibility.Collapsed;
            }
        }

        void Menu_ShowOpen()
        {
            miOpen.Items.Clear();

            MenuItem RMIWindows;
            for (int i = 0; i < Container.Children.Count; i++)
            {
                MdiChild child = Container.Children[i];
                RMIWindows = new MenuItem { Header = child.Title };
                RMIWindows.Click += new RoutedEventHandler(this.ActiveMenuItem_Click);
                miOpen.Items.Add(RMIWindows);
            }
            RibbonWindowsMenu.Items.Add(new Separator());
        }


        private void ActiveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem selectedItem = sender as MenuItem;
            if (selectedItem != null)
            {
                if (selectedItem.Header is Separator)
                { }
                else
                {
                    for (int i = 0; i < miOpen.Items.Count; i++)
                    {
                        MenuItem rmi = miOpen.Items[i] as MenuItem;
                        if (rmi != null)
                        {
                            rmi.IsChecked = false;
                        }
                    }

                    selectedItem.IsChecked = true;
                    for (int i = 0; i < Container.Children.Count; i++)
                    {
                        MdiChild child = Container.Children[i];
                        if (child.Title == null || selectedItem.Header == null) return;
                        if (child.Title.IndexOf(selectedItem.Header.ToString()) > -1)
                        {
                            child.Focus();
                        }
                    }
                }
            }

        }

        void SetMenuBG(string BgColor)
        {
            if (BgColor == null) return;
            BgColor = BgColor.Trim();
            if (BgColor == "") return;
            System.Windows.Media.BrushConverter brushConverter = new System.Windows.Media.BrushConverter();
            if (BgColor.StartsWith("#") == false)
            {
                BgColor = "#" + BgColor;
            }
            try
            {
                System.Windows.Media.Brush brush = (System.Windows.Media.Brush)brushConverter.ConvertFromString(BgColor);
                Dispatcher.Invoke((Action)delegate()
                {
                    this.gdTBMenu.Background = brush;
                }, null);
            }
            catch (Exception ex)
            { }
        }

        #endregion

        #region Content Button Events

        /// <summary>
        /// Handles the Click event of the DisableMinimize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableMinimize_Click(object sender, RoutedEventArgs e)
        {
            //Window1.MinimizeBox = false;
        }

        /// <summary>
        /// Handles the Click event of the EnableMinimize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void EnableMinimize_Click(object sender, RoutedEventArgs e)
        {
            //Window1.MinimizeBox = true;
        }

        /// <summary>
        /// Handles the Click event of the DisableMaximize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DisableMaximize_Click(object sender, RoutedEventArgs e)
        {
            //Window1.MaximizeBox = false;
        }

        /// <summary>
        /// Handles the Click event of the EnableMaximize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void EnableMaximize_Click(object sender, RoutedEventArgs e)
        {
            //	Window1.MaximizeBox = true;
        }

        /// <summary>
        /// Handles the Click event of the ShowIcon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ShowIcon_Click(object sender, RoutedEventArgs e)
        {
            //Window1.ShowIcon = true;
        }

        /// <summary>
        /// Handles the Click event of the HideIcon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void HideIcon_Click(object sender, RoutedEventArgs e)
        {
            //	Window1.ShowIcon = false;
        }
        #endregion
    }
}