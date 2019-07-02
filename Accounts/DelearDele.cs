/*
 *Create By Harry 
 * 
 *
 **/
#define cmTestD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GOSTS;
using GOSTS.Common;
using System.Data;
using System.Windows.Data;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Threading;


namespace GOSTS
{
    public class MsgQuee
    {
        public MsgQuee(Window w)
        {
            parent = w;
        }
        Window parent;
        List<InfoBox> ls = new List<InfoBox>();
        Thread t;
        public void Add(InfoBox info)
        {
            ls.Add(info);
        }

        public void remove(InfoBox info)
        {
            ls.Remove(info);
        }

        void ReadAndShow()
        {
            while (true)
            {
                Thread.Sleep(500);
                if (parent == null)
                {
                    break;
                }
                if (ls.Count <= 0)
                {
                    continue;
                }
                InfoBox info = ls[0];
                ls.Remove(info);
                if (info != null)
                {
                    parent.Dispatcher.Invoke((Action)delegate()
                    {
                        bool? Yes = null;
                        MsgForm msgbox = new MsgForm(info.Title, info.Msg, 15);

                        Window w = null;// App.Current.MainWindow;
                        for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
                        {
                            if (App.Current.Windows[intCounter] is GOSTradeStation)
                            {
                                w=App.Current.Windows[intCounter];
                                break;
                            }
                        }
                        if (w != null)
                        {
                            if (w != msgbox)
                            {
                                msgbox.Owner = w;
                            }
                        }
                        else
                        { TradeStationLog.WriteLog("no std"); }

                        try
                        {
                            Yes = msgbox.ShowDialog();
                        }
                        catch (Exception ex)
                        {
                            TradeStationLog.WriteError("err appr shw");
                            string err = ex.Message;
                        }
                        if (Yes == true)
                        {
                            info.InvokeDo();
                        }
                    });
                }
            }
          
          
        }

        public void initThread()
        {
            if (parent == null) return;
            t = new Thread(new ThreadStart(ReadAndShow));
            t.IsBackground = true;
            t.Start();
        }

        public void AbortThread()
        {
            try
            {
                t.Abort();
                t.DisableComObjectEagerCleanup();
            }
            catch { }
        }

        public void Suspend()
        {
            if (t != null)
            {
                try
                {
                    t.Suspend();
                }
                catch { }
            }
        }

        public void Run()
        {
            try
            {
                t.Resume();
            }
            catch { }
        }
    }


    /// <summary>
    /// 2014-03-28
    /// mantanance task lost at multi Thread competition,
    /// start a thread ,scan at times secends more than omt/omp
    /// </summary>
    public class MisTaskManager
    {
        List<InfoBox> ls = new List<InfoBox>();//store task miss  
        Thread t;
        public void Add(InfoBox info)
        {
            ls.Add(info);
        }

        public void remove(InfoBox info)
        {
            ls.Remove(info);
        }


    }

    public class InfoBox
    {
        public string Title { get; set; }
        public string Msg { get; set; }

        public delegate void Do();
        public event Do OnDo;

        public void InvokeDo()
        {
            if (OnDo != null)
            {
                OnDo();
            }
        }
    }

    /// <summary>
    /// IDelear 实例 IDobject init时调用IDelearStatus.AddIDelearCtrl(IDobject)
    /// IDelear 实例 IDobject unload时调用IDelearStatus.RemoveDelearCtrl(IDobject)
    /// IDelear窗体主动选择或输入account时，调用IDelearStatus.SetLatestAcc
    /// ACCUoAc属性为account info 和userOrderInfo实例设置，
    /// </summary>
    public class IDelearStatus
    {
        public static bool  IsFindAcc(string _acc)
        {
            if (_acc == null) return false;
            if (_acc.Trim() == "") return false;
            string id = getLatestAcc().Trim();
            if (id == null) return false;
            if (id.Trim() == "") return false;
            return (id == _acc.Trim()) ? true : false;
        }

        static List<IDelearUser> IUList = new List<IDelearUser>();
        static string LatestAcc = "";
        public static string ACCUoAc
        {
            get {
                if (IUList == null) return "";
                if (IUList.Count < 1) return "";
                foreach (var i in IUList)
                {
                    if (i is UserOrderInfo || i is AccountInfo)
                    {
                        return i.CurrentID;
                    }
                }
                return LatestAcc;
            }
        }

        public static void AddIDelearCtrl(IDelearUser obj)
        {
            if (IUList == null) return;
            if(IUList.Contains(obj)==false)
            {
                IUList.Add(obj);
            }
        }

        public static void RemoveDelearCtrl(IDelearUser obj)
        {
            if (IUList == null) return;
            if (IUList.Contains(obj))
            {
                IUList.Remove(obj);
            }
        }

        public static void SetLatestAcc(string value)
        {
           
            if (value == null) return;
            if (LatestAcc == null) { LatestAcc = value.Trim(); }

            if (LatestAcc.Trim() != value.Trim())
            {
                LatestAcc = value.Trim();
            }            
        }
        public static string getLatestAcc()
        {
            if (IUList.Count < 1) return "";
            return LatestAcc;
        }

        public static UserOrderInfo getLastUserOrderInfo()
        {
            if (IUList == null) return null;
            IDelearUser UO=IUList.FindLast(x=>x is UserOrderInfo);
            if(UO!=null)
            {
                return UO as UserOrderInfo;
            }
            return null;
        }
        #region order entry special status
        public static string OrderEntryProdCode="";
        public static void SetOrderEntryProduct(string prodCode)
        {
            if (prodCode != OrderEntryProdCode)
            {
                OrderEntryProdCode = prodCode;
            }           
        }

        public static string GetOrderEntryProduct()
        {
            if( OrderEntryProdCode == null )return  "" ;
            return OrderEntryProdCode.Trim();
        }
        #endregion


        #region Notice IDelearDeles Control CurrentID Change
        /// <summary>
        /// notcie account control to change current selected acc no
        /// </summary>
        /// <param name="sender">sender form</param>
        /// <param name="_acc">account no</param>
        public static void ChangeSelectedAcc(object sender,string _acc)
        {
            if (_acc == null) return;
            if (_acc.Trim() == "")
            {
                return;
            }
            if (IUList.Count < 1) return;
            IDelearUser UO = IUList[0];
            if (UO == null) return;
            if (UO.CurrentID == _acc)
            {
                return;
            }
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(),"[IDelearStatus.ChangeSelectedAcc],invoker:"+ sender.GetType().FullName);
            UO.SetCobBoxSelectedUser(_acc);
            UO.UnpromptedChangeAction(_acc);
        }
        #endregion

        public static void ReSendRequst()
        {
            string curAcc = ACCUoAc;
            foreach (IDelearUser ou in IUList)
            {               
                ou.getTitleFormat();
                ou.SetFormTitle();
            }
            foreach (WPF.MDI.MdiChild mdiChild in GOSTradeStation.container.Children)
            {
                if (mdiChild.Content is IChangeLang)
                {
                    IChangeLang iCHL = mdiChild.Content as IChangeLang;
                    if (iCHL != null)
                    {
                        iCHL.ChangLangInRuntime();
                    }

                }
            }
            //if (curAcc == null) return;

            //if (curAcc.Trim() == "")
            //{
            //    return;
            //}
           
            //ChangeSelectedAcc(new object(), curAcc);
            if (curAcc == null) return;

            if (curAcc.Trim() == "")
            {
                return;
            }
            if (!GOSTradeStation.isDealer)
            {
                TradeStationSend.Send(cmdClient.getMsgAccountMaster);
                TradeStationSend.Send(cmdClient.getPositionInfo);
            }
            else
            {
                TradeStationSend.Send(cmdClient.getMsgAccountMaster, curAcc);
                TradeStationSend.Send(cmdClient.getPositionInfo, curAcc);
            }

            //if (GOSTradeStation.isDealer == false)
            {
                foreach (IDelearUser ou in IUList)
                {
                    ou.IDelearUser_OnUserChange(curAcc);
                }
            }
           
        }

        #region 
        public static void CloseMsgBoxWindows()
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
            {
                if (App.Current.Windows[intCounter] is MsgForm)
                {
                    App.Current.Windows[intCounter].Close();
                }
            }
           // KillMessageBox();
        }
        #endregion

        static void KillMessageBox()
        {
            //查找MessageBox的弹出窗口,注意对应标题
            IntPtr ptr = FindWindow("MessageBox",null);
            if(ptr != IntPtr.Zero)
            {
                //查找到窗口则关闭
                PostMessage(ptr,WM_CLOSE,IntPtr.Zero,IntPtr.Zero);
            }
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet=CharSet.Auto)]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public const int WM_CLOSE = 0x10;
   

        #region clearAll Data relative to current select account in each  instance of uc subclass inherit from IDelearDele
        public static void ClearIDelearAccData()
        {
            if (IUList == null) return;
            if (IUList.Count < 1) return ;
            foreach (var i in IUList)
            {
                i.ClearControlData();
            }
        }
        #endregion

        public static void ClearCurAcc()
        {
            if (IUList == null) return;
            if (IUList.Count < 1) return;
            foreach (var i in IUList)
            {
                i.CurrentID = "";               
            }
        }
    }

    #region 2013-05-30 new
    public class IDelearUser : UserControl
    {
        public IDelearUser() { }
        public PositionBus PosBus;
        public MessageDistribute distributeMsg=null;


        bool bHookProdList = false;
        public void InitCurUser(AEUseCOB ucAEUser, MessageDistribute _distributeMsg)
        {
            distributeMsg = _distributeMsg;           
           // GegisterGetDelearUser();
            if (PosBus == null)
                PosBus = PositionBus.GetSinglePositionBus(distributeMsg);
            getTitleFormat();
            if (!GOSTradeStation.isDealer)
            {
                //20140530
                ucAEUser.Visibility = Visibility.Collapsed;
                if (GOSTradeStation.m_BeforeGet == false)
                {
                    if (_distributeMsg != null)
                    {
                        _distributeMsg.DisGotProductList += new MessageDistribute.OnDisGotProductList(_distributeMsg_DisGotProductList);
                    }
                    bHookProdList = true;
                }
                else
                {
                    CurrentID = GOSTradeStation.UserID;
                    this.Loaded += IDelearUser_Loaded;
                    IDelearStatus.SetLatestAcc(CurrentID);                   
                }
            }
            else
            {
                ucAEUsers = ucAEUser;
                SetcmbUser(ucAEUsers.cbbUsers);
              //  if (GOSTradeStation.IsWindowInitialized)
                //{
                //    initCtrlData();
                //}
            
                BindCmbChangeEvent();
                RegisterLockEvent();
                RegisterNoticeUserChange();

                //2014-09-11
                if (GOSTradeStation.m_BeforeGet == false && bHookProdList == false)
                {
                    if (this.cmbUser != null)
                    {
                        this.cmbUser.IsEnabled = false;
                    }
                    if (_distributeMsg != null)
                    {
                        _distributeMsg.DisGotProductList += new MessageDistribute.OnDisGotProductList(_distributeMsg_DisGotProductList);
                    }
                    bHookProdList = true;
                }
            }
            InitDeleAccountMaster();
            RegisterReceiveAccMaster();
            IDelearStatus.AddIDelearCtrl(this);
        }

        #region 20140530

        public bool bLoadedOnce = false;
        /// <summary>
        /// Modifier date: 2014-09-11 add statement for dealer case
        /// </summary>
        /// <param name="sender"></param>
        void _distributeMsg_DisGotProductList(object sender)
        {
            if (bLoadedOnce) return;           
            bLoadedOnce = true;
            try
            {               
                if (GOSTradeStation.isDealer == false)
                {
                    CurrentID = GOSTradeStation.UserID;
                    IDelearStatus.SetLatestAcc(CurrentID);
                    if (this is AccountInfo)
                    {
                        SetGetAccMaster(CurrentID);
                    }
                    AccChange(CurrentID);//for send request to server.
                    if (PosBus != null)
                    {
                        PosBus.AccID = CurrentID;
                    }
                }
                else
                {
                    Dispatcher.Invoke((Action)(() =>
                    {
                        if (this.cmbUser != null)
                        {
                            this.cmbUser.IsEnabled = true;
                        }
                    }), null);

                }
            }
            catch (Exception ex)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " delearDele, _DisGotProductList event hander error,  " + ex);
            }
        }

        #endregion

       
        void IDelearUser_Loaded(object sender, RoutedEventArgs e)
        {
            if (this is AccountInfo)
            {
                SetGetAccMaster(CurrentID);
            }
            AccChange(CurrentID);//for send request to server.
            if (PosBus != null)
            {
                PosBus.AccID = CurrentID;
            }
        }

        #region 注册委托从服务器获得delear所有用户
        delegate void BindListItemAccout(DataTable dataTable);
        BindListItemAccout AccoutDelegate; //注册获得当前用户代理的所有用户列表

        public void BindListItemMethodAccout(DataTable dataTable)
        {
            if (cmbUser == null) return;
            cmbUser.Items.Clear();
            if (dataTable == null) return;
            if (dataTable.Rows.Count < 1) return;
            foreach (DataRow dr in dataTable.Rows)
            {
                cmbUser.Items.Add(dr["acNo"].ToString());
            }
            cmbUser.SelectedIndex = -1;
        }

        protected void distributeMsg_DisAccoutList(object sender, DataTable dataTable)
        {
            Application.Current.Dispatcher.BeginInvoke(this.AccoutDelegate, new Object[] { dataTable });
        }

        public void GegisterGetDelearUser()
        {
            AccoutDelegate = new BindListItemAccout(BindListItemMethodAccout);
            if (distributeMsg != null)
            {
                distributeMsg.DisAccoutList += distributeMsg_DisAccoutList;
            }
            
             
        }
        #endregion

        #region 处理combobox设置值

        public ComboBox cmbUser { get; set; }
        public void SetcmbUser(ComboBox cbo)
        {
            cmbUser = cbo;
            cmbUser.IsEditable = true;
            //--enter to change
            cmbUser.IsTextSearchEnabled = false;
            LoadDelearUser(cbo);           
        }

        void BindCmbChangeEvent()
        {
            if (this.cmbUser == null) return;
            cmbUser.SelectionChanged += cmbUser_SelectionChanged;
            cmbUser.KeyDown += cmbUser_KeyDown;        
            cmbUser.LostKeyboardFocus += cmbUser_LostKeyboardFocus;        
        }

        void UnBindCmbChangeEvent()
        {
            if (this.cmbUser == null) return;
            cmbUser.SelectionChanged -= cmbUser_SelectionChanged;
            cmbUser.KeyDown -= cmbUser_KeyDown;
            cmbUser.LostKeyboardFocus -= cmbUser_LostKeyboardFocus;        
        }

        void cmbUser_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            //TradeStationLog.WriteQCHarry(DateTime.Now.ToString("yyMMdd-HHmmss ") + "on cmbUser_LostKeyboardFocus ");
            DoInputAccChange();
        }

        void DoInputAccChange()
        {
            if (this.cmbUser.IsEnabled == false) return;
            string str = this.cmbUser.Text.Trim();
            if (str == "") return;
            string strPref = "";
            if (GOSTradeStation.PrefAccInputModel != null)
            {
                strPref = PrefAccInput.ToPref(str, GOSTradeStation.PrefAccInputModel);
            }
            if (str == this.CurrentID || strPref == this.CurrentID)
            {
                //2014-11-03
                if (strPref != "" && strPref != str)
                {
                    this.cmbUser.Text = strPref.Trim();
                }
                return;
            }

            if (GOSTradeStation.PrefAccInputModel != null)
            {
                str = strPref;
                this.cmbUser.Text = str.Trim();
            }
            foreach (object item in this.cmbUser.Items)
            {
                if (item.ToString().Trim() == str)
                {
                    cmbUser.SelectedItem = item;
                    return;
                }
            }
            this.cmbUser.SelectedIndex = -1;
            this.cmbUser.Text = str;
            // CurrentID = this.cmbUser.Text;
            if (this.cmbUser.IsEnabled == true)
                UnpromptedChangeAction(str);
        }

        #region unprompted set value
        /// <summary>
        /// enter to cmbuser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        void cmbUser_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (cmbUser == null) return;
            if (e.Key == System.Windows.Input.Key.Enter)
            {

                string str = this.cmbUser.Text.Trim();
                if (str == "") return;

                string strPref = "";
                if (GOSTradeStation.PrefAccInputModel != null)
                {
                    strPref = PrefAccInput.ToPref(str, GOSTradeStation.PrefAccInputModel);
                }
                if (str == this.CurrentID || (strPref == this.CurrentID && this.CurrentID!=null &&strPref != ""))
                {
                    List<TextBox> list = GetChildObjects <TextBox>(this.cmbUser, typeof(TextBox));
                    if (list.Count > 0)
                    {
                        TextBox textBox = list[0];
                        if (textBox != null)
                        {
                            //2014-11-03
                            if(strPref!=null)
                            {
                                if (str != strPref && strPref != "")
                                {
                                    textBox.Text = strPref;
                                }
                            }
                            textBox.SelectionStart = 0;
                            textBox.SelectionLength = textBox.Text.Length;
                        }
                    }
                    //  TextBox textBox = (TextBox)GetDescendantByType(this.cmbUser, typeof(TextBox));
                   // cmbUser..SelectionStart = 0;
                    //cmbUser.SelectionLength = this.cmbUser.Text.Length;
                    // this.cmbUser.
                    return;
                }
                ///2013-06-17 Preference input 
                if (GOSTradeStation.PrefAccInputModel != null)
                {
                    str = strPref;
                    this.cmbUser.Text = str.Trim();
                }
                foreach (object item in this.cmbUser.Items)
                {
                    if (item.ToString().Trim() == str)
                    {
                        cmbUser.SelectedItem = item;
                        return;
                    }
                }
                this.cmbUser.SelectedIndex = -1;
                this.cmbUser.Text = str;
                // CurrentID = this.cmbUser.Text;
                if (this.cmbUser.IsEnabled == true)
                    UnpromptedChangeAction(str);
            }
        }

        //for reqirement: click mdi title bar,select cmbuser text , as click cmbuser
        public void SetSelect(object sender)
        {
            if (this.cmbUser!=null)
            {
                List<TextBox> list = GetChildObjects<TextBox>(this.cmbUser, typeof(TextBox));
                if (list.Count > 0)
                {
                    TextBox textBox = list[0];
                    if (textBox != null)
                    {
                        textBox.Focus();
                        if (textBox.Text != "")
                        {
                            textBox.SelectionStart = 0;
                            textBox.SelectionLength = textBox.Text.Length;
                        }
                    }
                }
                //  TextBox textBox = (TextBox)GetDescendantByType(this.cmbUser, typeof(TextBox));
                // cmbUser..SelectionStart = 0;
                //cmbUser.SelectionLength = this.cmbUser.Text.Length;
                // this.cmbUser.
            }
        }

        public List<T> GetChildObjects<T>(DependencyObject obj, Type typename) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();
            if (obj == null)
            {
                return childList;
            }
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).GetType() == typename))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects<T>(child, typename));
            }
            return childList;
        }

        void cmbUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), this.GetType().FullName + @",[cmbUser_SelectionChanged] beginn");            
            if (this.cmbUser.SelectedValue != null)
            {
                if (this.cmbUser.IsEnabled == true)
                {
                    traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), this.GetType().FullName + @",[cmbUser_SelectionChanged],user cmb enabled " );
                    UnpromptedChangeAction(this.cmbUser.SelectedValue.ToString().Trim());
                }
                else
                {
                    traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), this.GetType().FullName + @",[cmbUser_SelectionChanged],user cmb disabled ");      
                }
            }
        }


        /// <summary>
        /// 当前ＡＣＣ更改时，当前窗体要做的事情
        /// </summary>
        void AccChange(string _uid)
        {
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), this.GetType().FullName + @".[AccChange],before IDelearUser_OnUserChange,pass uid:" + _uid);            
            IDelearUser_OnUserChange(_uid);
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), this.GetType().FullName + @"[AccChange],after IDelearUser_OnUserChange,pass uid:" + _uid );            
        }

        /// <summary>
        /// when unprompted change cmbuser,actions to do
        /// </summary>
        public void UnpromptedChangeAction(string _uid)
        {
            #region 2014-10-27
            if (this.cmbUser == null)
            {
                return;
            }
            #endregion 2014-10-27
            string GuidID = GosTestGlobal.GetBlockGuid();
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), this.GetType().Name + @".[UnpromptedChangeAction_" + GuidID + "],BEGIN================================================");
                
            if (this.cmbUser.IsEnabled == false)
            {
                traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), this.GetType().Name + @".[UnpromptedChangeAction_" + GuidID + "],is enabled false,return;");
                return; //非主动发出改变的窗体里的cmbuser,退出
            }
                      
            CurrentID = _uid;
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), this.GetType().Name + @".[UnpromptedChangeAction_" + GuidID + "],after set CurrentID='" + this.CurrentID + @"'");
         
            IDelearStatus.ClearIDelearAccData();
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), @".[UnpromptedChangeAction_" + GuidID + "],after IDelearStatus.ClearIDelearAccData()");
            
            //if (PosBus != null)
            //{
            //    PosBus.ClearControlData();
            //}
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), @".[UnpromptedChangeAction_" + GuidID + "],send request get master,CurrentID='" + this.CurrentID + "'");            
            SetGetAccMaster(_uid);

            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), @".[UnpromptedChangeAction_" + GuidID + "],before invoke AccChange(),CurrentID='" + this.CurrentID + "'");            
            AccChange(_uid);  
        
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), @".[UnpromptedChangeAction_" + GuidID + "],before SetLatestAcc,CurrentID='" + this.CurrentID + "'");            
            IDelearStatus.SetLatestAcc(CurrentID);

            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), @".[UnpromptedChangeAction_" + GuidID + "],invokde OnNoticeUserChang,CurrentID='" + this.CurrentID + "'");             
            if (OnNoticeUserChang != null)
            {
                OnNoticeUserChang(this, _uid);
            }
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), @".[UnpromptedChangeAction"  + GuidID + "],before asign PosBus.AccID ,CurrentID='" + this.CurrentID + "'");
           
            if (PosBus != null)
            {
                PosBus.AccID = _uid;
            }
            
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), @".[UnpromptedChangeAction" + GuidID + "],after asign PosBus.AccID ,CurrentID='" + this.CurrentID + "'");
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), this.GetType().Name + @"(UnpromptedChangeAction_" + GuidID + "),End================================================");
            
        }

        #endregion

        #region passive cmb chanage

        void PassiveChangeAction(object sender, string _uid)
        {
            string GuidID = GosTestGlobal.GetBlockGuid();
            string currentClass = this.GetType().FullName;
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), currentClass + @".[PassiveChangeAction" + GuidID + "]," + sender.GetType().FullName+"===================B");
            
            if ((this.IsLock ?? false) == true)
            {
                traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), currentClass + @".[PassiveChangeAction" + GuidID + "]," + ",islock:yes");
                if (sender is UserOrderInfo && this is AccountInfo)
                {
                }
                else if (sender is AccountInfo && this is UserOrderInfo)
                {
                }
                else
                {
                    return;
                }
            }
            else
            {
                traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), currentClass + @".[PassiveChangeAction" + GuidID + "],islock:false");
            }
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), currentClass+@".[PassiveChangeAction],sender:" + sender.GetType().FullName + " ,pass uid='" + _uid + "',before SetCobBoxSelectedUser()");
            
            SetCobBoxSelectedUser(_uid);            
            CurrentID = _uid;
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), currentClass+@".[PassiveChangeAction],after set this.CurrentID:" + _uid + ",bedore invoke AccChange(),sender:" + sender.GetType().FullName);            
            AccChange(_uid);
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), currentClass+@".[PassiveChangeAction" + GuidID + "] end,sender:" + sender.GetType().FullName+"===================E" );            
        }

        /// <summary>
        /// 单纯设置下拉框的值，不触发其它事件的处理
        /// </summary>
        /// <param name="selectedUser"></param>
        public void SetCobBoxSelectedUser(string selectedUser)
        {
            if (this.cmbUser == null) return;         
            foreach (object o in this.cmbUser.Items)
            {
                if (o.ToString().Trim() == selectedUser)
                {
                    traceAccChange.Log(enumItemType.uiChangeAcc.ToString(),this.GetType().FullName + @"[SetCobBoxSelectedUser],before set selected cmbox item,pass selectedUser='" + selectedUser + "'");                    
                    this.cmbUser.IsEnabled = false;                   
                    cmbUser.SelectedItem = o;                   
                    this.cmbUser.IsEnabled = true;                    
                    this.cmbUser.Text = selectedUser;                   
                    return;
                }
            }
            
            this.cmbUser.IsEnabled = false;
            this.cmbUser.SelectedIndex = -1;
            this.cmbUser.Text = selectedUser;
            this.cmbUser.IsEnabled = true;            
        }

        #endregion

        /// <summary>
        /// 如果是delear,载入代理用列表

        /// </summary>
        /// <param name="cbo"></param>
        /// <param name="dtUser"></param>
        public void LoadDelearUser(ComboBox cbo)
        {
            if (GOSTradeStation.isDealer)
            {
                //cbo.Items.Clear();
                //if (GOSTradeStation.userData != null)
                //{
                //    if (GOSTradeStation.userData.IsDealer)
                //    {
                //        if (GOSTradeStation.userData.AccountTable != null)
                //        {
                //            foreach (DataRow dr in GOSTradeStation.userData.AccountTable.Rows)
                //            {
                //                cbo.Items.Add(dr["acNo"].ToString());
                //            }
                //            cmbUser.SelectedIndex = -1;
                //        }
                //    }
                //}

                if (cmbUser == null) return;
                cmbUser.Items.Clear();
                if (GOSTS.Preference.AccListConfigManage.accoutCL == null) return;
              //  if (GOSTS.Preference.AccListConfigManage.accoutCL.Count < 1) return;
                //foreach (DataRow dr in dataTable.Rows)
                //{
                //    cmbUser.Items.Add(dr["acNo"].ToString());
                //}
                cmbUser.ItemsSource = GOSTS.Preference.AccListConfigManage.accoutCL;
                cmbUser.SelectedIndex = -1;
            }
            else
            {
                cbo.Visibility = Visibility.Collapsed;
            }
        }


        /// <summary>
        /// virtual function,开放给引用窗体实现，用来放置特定引用窗体在ＡＣＣ改变时要做的事情
        /// </summary>
        /// <param name="ClientID"></param>
        public virtual void IDelearUser_OnUserChange(string ClientID)
        {
        }
        #endregion

        public virtual void initCtrlData()
        {
            string acid="";
            if ((this is AccountInfo) || (this is UserOrderInfo))
            {
                acid = IDelearStatus.ACCUoAc;                
            }
            else
            {
                acid = IDelearStatus.getLatestAcc();
            }

            if (acid != null)
            {
                if (acid != "")
                {
                    SetCobBoxSelectedUser(acid);
                    CurrentID = acid;
                    AccChange(acid);
                    SetGetAccMaster(acid);
                    //if (PosBus == null)
                    //    PosBus = PositionBus.GetSinglePositionBus(distributeMsg);
                    //if (PosBus != null)
                    //{
                    //    PosBus.sendGetSVPosition(acid);
                    //}
                }
            }
        }


        #region 存放delear版本下的ＡＣＣ或者非delear版本下的user
        private string _CurrentID;
        /// <summary>
        /// 存放控件当前操作的用户

        /// </summary>
        public string CurrentID
        {
            get { return _CurrentID; }
            set
            {
                _CurrentID = value;
                // string cl = (this.GetType().Name); MessageBox.Show("class:" + cl + " current id:" + _CurrentID);

            }
        }
        #endregion

        private AEUseCOB _ucAEUsers;
        public AEUseCOB ucAEUsers
        {
            get { return _ucAEUsers; }
            set { _ucAEUsers = value; }
        }


        public delegate void DeleNoticeUserChange(object sender, string CurrentUser);
        /// <summary>
        /// notice other forms that current user changed
        /// </summary>
        public static event DeleNoticeUserChange OnNoticeUserChang;

        delegate void DeleSelectUserChange(object sender, string CurrentUser);       

        void RegisterNoticeUserChange()
        {
            OnNoticeUserChang += DelearDele_OnNoticeUserChang;
        }

        void DelearDele_OnNoticeUserChang(object sender, string CurrentUser)
        {
            traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), this.GetType().FullName+@".[DelearDele_OnNoticeUserChang],PARAM:"+sender.GetType().FullName+",CurrentID='" + CurrentUser + "'");           
            if (sender == this)
            {
                traceAccChange.Log(enumItemType.uiChangeAcc.ToString(), this.GetType().FullName + @".[DelearDele_OnNoticeUserChang],sender is current form ,return,sender:" + sender.GetType().FullName );
                return;
            }           
            Application.Current.Dispatcher.BeginInvoke(new DeleSelectUserChange(PassiveChangeAction), new object[] { sender, CurrentUser });           
        }



        #region Is Lock users
        public static readonly DependencyProperty IsLockVisibleProperty = DependencyProperty.Register("IsLockVisible",
            typeof(Visibility), typeof(IDelearUser),new FrameworkPropertyMetadata(Visibility.Collapsed));
        public Visibility IsLockVisible
        {
            get { return (Visibility)GetValue(IDelearUser.IsLockVisibleProperty); }
            set { SetValue(IDelearUser.IsLockVisibleProperty,value); }
        }
          

        private bool? _IsLock = false;
        public bool? IsLock
        {
            get { return _IsLock; }
            set { _IsLock = value; }
        }

        void RegisterLockEvent()
        {
            if (ucAEUsers == null)
            {
                return;
            }
            if (ucAEUsers.tbnLockUser == null) return;
            ucAEUsers.tbnLockUser.Click += tbnLockUser_Click;
            RegisterNoticeLockChange();
        }

        void tbnLockUser_Click(object sender, RoutedEventArgs e)
        {
            if (ucAEUsers == null)
            {
                return;
            }
            if (ucAEUsers.tbnLockUser == null) return;
            IsLock = ucAEUsers.tbnLockUser.IsChecked;
            if (OnNoticeLockChang != null)
            {
                OnNoticeLockChang(this, IsLock);
            }
        }

        public delegate void DeleNoticeLockChange(object sender, bool? bLock);       
        public static  event DeleNoticeLockChange OnNoticeLockChang;

        void RegisterNoticeLockChange()
        {
            OnNoticeLockChang += DelearDele_OnNoticeLockChang;
        }

        void DelearDele_OnNoticeLockChang(object sender,bool? bLock)
        {
            if (sender == this)
            {
                return;
            }

            Application.Current.Dispatcher.BeginInvoke(new DeleNoticeLockChange(SetOtherContrlLock), new object[] { sender, bLock });
          
        }

        void SetOtherContrlLock(object sender,bool? bLock)
        {
            bool b = false;
            if (sender is UserOrderInfo && this is AccountInfo)
            {
                b = true;
            }
            else if (sender is AccountInfo && this is UserOrderInfo)
            {
                b = true;
            }
            if (!b) return;
            if (ucAEUsers == null)
            {
                return;
            }
            if (ucAEUsers.tbnLockUser == null) return;
            ucAEUsers.tbnLockUser.IsChecked = bLock;
            IsLock = ucAEUsers.tbnLockUser.IsChecked;
        }
        #endregion

        protected void IDelearUser_Unloaded_1(object sender, RoutedEventArgs e)
        {
            UnBindCmbChangeEvent();
            OnNoticeUserChang -= DelearDele_OnNoticeUserChang;
            OnNoticeLockChang -= DelearDele_OnNoticeLockChang;           
            if (distributeMsg != null)
            {
                distributeMsg.DisAccoutList -= distributeMsg_DisAccoutList;
                //GOSTradeStation.distributeMsg.DisAccoutList += distributeMsg_DisAccoutList;
                //20140530
                if (bHookProdList == true)
                {
                    distributeMsg.DisGotProductList -= new MessageDistribute.OnDisGotProductList(_distributeMsg_DisGotProductList);
                }
            }
            UnRegisterReceiveAccMaster();
            IDelearStatus.RemoveDelearCtrl(this);
        }

        #region AccontMaster 
        /*
        * 每次在cmbox选择或输入账户acc，调用SetGetAccMaster发送getAccountMaster，返回的datable零行即调用ClearControlData()清空控件数据，基类ClearControlData清空cmbox中值
        * 
        */
        //subclass override to clear their own control data
        public virtual void ClearControlData()
        {
            
        }

        public void setSelectUserNull()
        {
            if (cmbUser == null) return;
            cmbUser.IsEnabled = false;
            cmbUser.SelectedIndex = -1;
            cmbUser.Text = "";
            cmbUser.IsEnabled = true;
        }

        /// <summary>
        /// 调用时机在主动改变当前acc的窗体的UnpromptedChangeAction
        /// </summary>
        /// <param name="uid"></param>
        public void SetGetAccMaster(string uid)
        {
            if (uid == "") return;
            if (!GOSTradeStation.isDealer)
            {
                TradeStationSend.Send(cmdClient.getMsgAccountMaster);
#if cmTestD
                GosTradeTest.TestExcelLog.WriteLog(enumItemType.getMast.ToString(),"send request ");
#endif
            
            }
            else
            {
                TradeStationSend.Send(cmdClient.getMsgAccountMaster, uid);
#if cmTestD
                GosTradeTest.TestExcelLog.WriteLog(enumItemType.getMast.ToString(), @"send request acc:" + uid + "'");
#endif
            }
        }
        void RegisterReceiveAccMaster()
        {
            if (distributeMsg != null)
            {
                distributeMsg.DisAccountMaster += DispatcherInvokeGetAccountMaster;
            }
        }
        void UnRegisterReceiveAccMaster()
        {
            if (distributeMsg != null)
            {
                distributeMsg.DisAccountMaster -= DispatcherInvokeGetAccountMaster;
            }
        }
        public delegate void deleAccountMaster(object sender, DataTable tableMaster,string RecAcc);
        deleAccountMaster odeleAccountMaster;
        void InitDeleAccountMaster()
        {
            odeleAccountMaster = new deleAccountMaster(GetAccountMaster);
        }

        void ClearAccData()
        {
            setSelectUserNull();
            this.CurrentID = "";
            this.ClearControlData();
            //IDelearStatus.ClearCurAcc();
            //IDelearStatus.ClearIDelearAccData();
            if (this.PosBus != null)
            {
                this.PosBus.AccID = "";
                this.PosBus.posCollection.Clear();
            }
        }

        protected DataTable dtMaster;
        public virtual void GetAccountMaster(object sender, DataTable tableMaster,string RecAcc)
        {
            if (RecAcc != this.CurrentID)
            {
                #if cmTestD
                GosTradeTest.TestExcelLog.WriteLog(enumItemType.getMast.ToString(), @"return ,receive Data,rec Acc:" + (RecAcc != null ? RecAcc : "") +",current select Id:"+this.CurrentID );           
#endif
                return;
            }
#if cmTestD
            GosTradeTest.TestExcelLog.WriteLog(enumItemType.getMast.ToString(), @"receive Data, Acc:"+(RecAcc!=null?RecAcc:"") +@"\r\n" + GosTradeTestHelper.RecTableToString(tableMaster));
#endif
            dtMaster = tableMaster;
            if (tableMaster == null)
            {                
                //this.CurrentID = "";
                //ClearControlData();
                if(GOSTradeStation.isDealer)
                ClearAccData();
#if cmTestD
                GosTradeTest.TestExcelLog.WriteLog(enumItemType.getMast.ToString(), @"return,receive Data, Acc:" + (RecAcc != null ? RecAcc : "")+",null data" + @"\r\n" + GosTradeTestHelper.RecTableToString(tableMaster));
#endif           
                return;
            }
            if (tableMaster.Rows.Count < 1)
            {
                if (GOSTradeStation.isDealer)
                ClearAccData();
#if cmTestD
                GosTradeTest.TestExcelLog.WriteLog(enumItemType.getMast.ToString(), @"return,receive Data, Acc:" + (RecAcc != null ? RecAcc : "") + ",null data" + @"\r\n" + GosTradeTestHelper.RecTableToString(tableMaster));
#endif           
                return;
            }

            if (tableMaster.Columns.Contains("accNO") == false)
            {
                if (GOSTradeStation.isDealer)
                ClearAccData();
#if  cmTestD
                GosTradeTest.TestExcelLog.WriteLog(enumItemType.getMast.ToString(), @"return,receive Data, Acc:" + (RecAcc != null ? RecAcc : "") + ",no column named 'accNO'" + @"\r\n" + GosTradeTestHelper.RecTableToString(tableMaster));
#endif            
                return;
            }
            DataRow[] drs = tableMaster.Select("accNO='" + this.CurrentID + "'");
            if(drs.Length<1)
            {
                if (GOSTradeStation.isDealer)
                ClearAccData();
#if  cmTestD
                GosTradeTest.TestExcelLog.WriteLog(enumItemType.getMast.ToString(), @"return ,receive Data, Acc:" + (RecAcc != null ? RecAcc : "") + ",can not find rows has accNo='"+this.CurrentID+"'" + @"\r\n" + GosTradeTestHelper.RecTableToString(tableMaster));
#endif            
                return;
            }
            //DataRow dr = tableMaster.Rows[0];
            //string Sex = FormatSex(dr.getColValue("sex"));
            //string accName = dr.getColValue("accName");
            //string idBrNo = dr.getColValue("idBrNo");
            //if (idBrNo != "")
            //{
            //    idBrNo = ",HKID:" + idBrNo;
            //}
            //else
            //{
            //    idBrNo = "";
            //}            
        }       

        protected void DispatcherInvokeGetAccountMaster(object sender, DataTable tableMaster,string RecAcc)
        {
            Application.Current.Dispatcher.BeginInvoke(odeleAccountMaster, new Object[] { sender, tableMaster, RecAcc });
        }

       
        #endregion

        #region 2013 -11 -05 setTitle and update to Menu
        public void SetMdITitle(MessageDistribute odistributeMsg, WPF.MDI.MdiChild mchild, string NewTitle)
        {

            Dispatcher.Invoke((Action)(() =>
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
                catch(Exception EX)
                {
                    MessageBox.Show(EX.Message);
                }           
            }), null);               
        }
        #endregion


        //2014-10-10 get title foramt from .resx file
        GosCulture.LabelTextExtension ltTitleExtension = null;
        protected string strTitleFormat = "";
        public string getTitleFormat()
        {
            try
            {
                string sType = this.GetType().Name.ToString().ToLower();
                string key = sType + "Title";
                if (ltTitleExtension == null)
                {
                    ltTitleExtension = new GosCulture.LabelTextExtension(key);
                }

                strTitleFormat = ltTitleExtension.Value;// GosCulture.CultureHelper.GetString("AccInfoTitle");
                //return strTitleFormat;
            }
            catch (Exception ex) { }
            return "";
        }

        public virtual void SetFormTitle(int itype, MessageDistribute _distributeMsg, WPF.MDI.MdiChild _mdiChild, params string[] Params)
        {
        }
        public virtual void SetFormTitle()
        {
        }
    }

    #endregion

    #region acc change log
    class traceAccChange
    {
        static bool bLog
        {
            get {
                try
                {
                    return GOSTradeStation.isDealer;
                }
                catch {
                    return false;
                }
                return false;
            }
        }
        public static void Log(string stype, string Msg)
        {
            if (bLog)
            {
                GosTradeTest.TestExcelLog.WriteLog(stype, Msg);
            }
        }
    }
    #endregion

    public class UnixToWinTimeConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "";
            if (value.ToString().Trim() == "0") return "";
            try
            {
                long s = System.Convert.ToInt64(value.ToString());
                return UnixToWinTime(s).ToString("yyyy-MM-dd");
            }
            catch { }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        public static DateTime UnixToWinTime(long timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);

            return dtResult;
        }
    }

    /// <summary>
    /// Format: yyyy-MM-dd HH:mm:ss
    /// </summary>
    public class FormatTimeConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "";
            if (value.ToString().Trim() == "") return "";
            try
            {
                DateTime dt = System.Convert.ToDateTime(value.ToString());
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch(Exception ex)
            {
                TradeStationLog.WriteLogPerformance(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " FormatTimeC,convert datetime to ui:" + ex);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
       
    }

    public class OrderActiveConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "";
            if (value.ToString().Trim() == "0") return false;
            if (value.ToString().Trim() == "1") return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        public static DateTime UnixToWinTime(long timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);

            return dtResult;
        }
    }

    public class OrderValidConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "";
            string valid = value.ToString().ToUpper();
            switch (valid)
            {
                case "TODAY":
                    return "即日";
                    break;
                case "FAK":
                    return "成交並取消";
                    break;
                case "FOK":
                    return "成交或取消";
                    break;
                case "GTC":
                    return "到期日";
                    break;
                case "SPECTIME":
                    return "指定日期";
                    break;
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        public static DateTime UnixToWinTime(long timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);

            return dtResult;
        }
    }


    public class ZeroConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "";
            if (value.ToString() == "0")
            {
                return "";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }


    public class IntValidate : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value.ToString().Trim();
            if (!System.Text.RegularExpressions.Regex.IsMatch(str, @"^\d+$"))
            {
                return new ValidationResult(false, "must int");
            }
            return ValidationResult.ValidResult;
        }
    }

   

    public class windowHelper
    {
        //public static Window FindWin(typeof Class)
        //{
        //    for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
        //    {
        //        if (App.Current.Windows[intCounter] is MsgForm)
        //        {
        //            App.Current.Windows[intCounter].Close();
        //        }                

        //    }
        //}
    }
}
