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
using GOSTS.Common;
using WPF.MDI;
using System.Threading;

namespace GOSTS
{
    /// <summary>
    /// ChangePWD.xaml 的交互逻辑
    /// </summary>
    public partial class ChangePWD : UserControl,IChangeLang
    {
        public static ChangePWD instance = null;
        #region rs text
        public string strRsTitle
        {
            get {
                return GOSTS.GosCulture.CultureHelper.GetString("changepwdTitle");
            }
        }

        public string strRs_Pwd_Pls
        {
            get {
                return GOSTS.GosCulture.CultureHelper.GetString("CHPwd_Pwd_pls");
            }
        }

        public string strRs_CHPwd_Pwd_NotEqua
        {
            get {
                return GOSTS.GosCulture.CultureHelper.GetString("CHPwd_Pwd_NotEqual");
            }
        }
        public string strRs_CHPwd_Pwd_ChangeOK
        {
            get {
                return GOSTS.GosCulture.CultureHelper.GetString("CHPwd_Pwd_ChangeOK");
            }
        }        
       
        #endregion
        public static ChangePWD GetChPWD(MessageDistribute _msgDistribute, MdiChild _mdi, ref bool bCreateNew)
        {
            bool b = false;
            if (instance == null)
            {
                b = true;
            }
            else if (_msgDistribute != instance.msgDistribute)
            {
                b = true;               
            }
            if (b)
            {
                bCreateNew = b;
                instance = new ChangePWD(_msgDistribute, _mdi);
            }
            else
            {
                instance.MdiChild.Focus();
            }
            return instance;
        }

        private ChangePWD(MessageDistribute _msgDistribute,MdiChild _mdi)
        {
            InitializeComponent();
            msgDistribute = _msgDistribute;
            MdiChild = _mdi;
            dlgCh = new MessageDistribute.onDisChangePassword(ChResult);
            initThreadProgress();
            GosBzTool.setTitle(MdiChild, msgDistribute, strRsTitle);
        }
        MessageDistribute msgDistribute;
        MdiChild MdiChild;
        public delegate void onDisChangePassword(object sender, string strResult);
        public event onDisChangePassword DisChangePassword;

        private void btnOK_Click_1(object sender, RoutedEventArgs e)
        {
            string PwdOld = this.psdOld.Password;
            string PwdNew = this.psdNew.Password;
            string PwdConfirm = this.psdNewRetype.Password;
            if (PwdNew == "")
            {
                MessageBox.Show(strRs_Pwd_Pls);//"please input new password");
                return;
            }
            if (PwdNew != PwdConfirm)
            {
                MessageBox.Show(strRs_CHPwd_Pwd_NotEqua);//"new password not equal to retype password");
                return;
            }
            TradeStationSend.Send(cmdClient.changePassword, PwdOld, PwdNew);          
            startProgress();
            RegisterSVResult();           
        }

        bool bRegister = false;
        public void RegisterSVResult()
        {
            if (!bRegister)
            {
                msgDistribute.DisChangePassword += new MessageDistribute.onDisChangePassword(DispatchRevResult);
                bRegister = true;
            }
        }
        public void UnRegisterSVResult()
        {
            if (bRegister)
            {
                if (msgDistribute != null)
                {
                    msgDistribute.DisChangePassword -= new MessageDistribute.onDisChangePassword(DispatchRevResult);
                }
                if (bRegister != null)
                {
                    bRegister = false;
                }
            }
        }

        MessageDistribute.onDisChangePassword dlgCh;
        public void DispatchRevResult(object sender, string strResult, bool bSuccess, string _UserID)
        {
            Dispatcher.Invoke(dlgCh,new object[]{sender,strResult,bSuccess,_UserID});
        }

        public void ChResult(object sender, string strResult,bool bSuccess,string _UserID)
        {
            if(GOSTradeStation.UserID ==null){
                return;
            }
            if (GOSTradeStation.UserID.Trim() == "")
            {
                return;
            }
            if (GOSTradeStation.UserID.Trim() == _UserID)
            {               
                SuspendProgress();
                if (bSuccess)
                {
                    MessageBox.Show(strRs_CHPwd_Pwd_ChangeOK);//"password changed successfully");

                    TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "UserName:" + GOSTradeStation.UserID + ", Password changed successfully!");
                          
                    if (MdiChild != null)
                    {
                        MdiChild.Close();
                    }
                    return;
                }
                else
                {
                    MessageBox.Show(strResult);
                }
            }
        }

        #region show Progress when receive  from server delay

        int delays = 500;
        DateTime? dtPress;
        Thread thread;
        void scan()
        {
            while (true)
            {
                if (dtPress == null) break;
                if (dtPress.Value.AddMilliseconds(delays) < DateTime.Now)
                {
                    
                    ShowProgress();
                }
                Thread.Sleep(100);
            }
        }

        void ShowProgress()
        {
            Dispatcher.Invoke((Action)delegate()
            {
                this.btnOK.IsEnabled = false;
                this.pg1.Visibility = Visibility.Visible;
                this.pg1.IsIndeterminate = true;
            });
        }

        void HideProgress()
        {
            Dispatcher.Invoke((Action)delegate()
            { 
                this.btnOK.IsEnabled = true;
                this.pg1.Visibility = Visibility.Collapsed;
                this.pg1.Value = 0;
                this.pg1.IsIndeterminate = false;
            });
        }

        public void initThreadProgress()
        {
            thread = new Thread(scan);
            thread.IsBackground = true;
        }
        public void AbortThreadProgress()
        {
            try
            {
                if (thread != null)
                {
                    thread.Abort();
                    thread.DisableComObjectEagerCleanup();
                    thread = null;
                }
            }
            catch { }
        }
        public void startProgress()
        {
            if (thread != null)
            {
                dtPress = DateTime.Now;
                try
                {
                    if (thread.ThreadState == ThreadState.Suspended || thread.ThreadState == ThreadState.SuspendRequested)
                    {
                        thread.Resume();
                    }
                    else
                    {
                        thread.Start();
                    }
                }
                catch { 
                    
                }
            }
        }

        public void SuspendProgress()
        {
            try
            {
                if (thread != null)
                {
                    // if (thread.ThreadState == ThreadState.Running)
                    {
                        thread.Suspend();
                    }
                }
            }
            catch { }
            HideProgress();
        }

        #endregion

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            //UnRegisterSVResult();
            //try
            //{
            //    if (thread != null)
            //    {
            //        thread.Abort();
            //        thread.DisableComObjectEagerCleanup();
            //    }
            //}
            //catch { }
            Clear();
        }

        public static void Clear()
        {
            try
            {
                if (instance != null)
                {
                    instance.UnRegisterSVResult();
                    if (instance.thread != null)
                    {
                        instance.thread.Abort();
                        instance.thread.DisableComObjectEagerCleanup();
                    }
                }
            }
            catch(Exception EX){ }
            if (instance != null)
            {
                instance = null;
            }
        }

        private void btnCancel_Click_1(object sender, RoutedEventArgs e)
        {
            if (MdiChild != null)
            {
                MdiChild.Close();
            }
        }



        public void ChangLangInRuntime()
        {
            GosBzTool.setTitle(MdiChild, msgDistribute, strRsTitle);
        }
    }
}
