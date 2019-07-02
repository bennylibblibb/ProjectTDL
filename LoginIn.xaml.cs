using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WPF.MDI;
using System.Windows.Input;
using GOSTS.Common;
using GOSTS.ViewModel;
using System.IO;
using System.Windows .Forms ; 
using System.Configuration;
using System.Text.RegularExpressions;
using System.Timers;

namespace GOSTS
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class LoginIn : System.Windows.Controls.UserControl
    {
        string AlertTile
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("loginAletTitle");
            }
        }

        string AlertInvaildMsg
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("loginAletInvaildMsg");
            }
        }

        string AletPassword
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("loginAletPassword");
            }
        }

        string AletUser
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("loginAletUser");
            }
        }
        string AletConnect
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("loginAletConnect");
            }
        }

        string AletLogin
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("loginAletLogin");
            }
        }

        private MessageChannel channel = null;
        private string userName = "";
        private string Pwd = "";
        public MdiChild mdiChild { get; set; }
        delegate void UpdateLoginStatus(string strStatus, string strDetails, DateTime dateTime, bool PopupPwd);
        UpdateLoginStatus UpdateDelegate;

        public delegate void OnLoginReslut(object sender, string result, string strDetails, DateTime dateTime, bool PopupPwd);
        public event OnLoginReslut LoginReslut;
        private bool Done = false;

        private System.Timers.Timer timeOutTimerOMT; 

        public LoginIn()
        {
            InitializeComponent();
            UpdateDelegate = new UpdateLoginStatus(UpdateLoginStatusMethod);

            timeOutTimerOMT = new System.Timers.Timer(AppFlag.TimeOutDelay);
            timeOutTimerOMT.Elapsed += new ElapsedEventHandler(OnTimeOutEvent);
            timeOutTimerOMT.Enabled = false;
        }

        private void OnTimeOutEvent(object source, ElapsedEventArgs e)
        {
            timeOutTimerOMT.Enabled = false;
            //Close connection 
            TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " OMT Login TimeOut.");
            if (channel != null)
            {
                channel.EnsureDisconnected();
                TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " OMT EnsureConnection for Login TimeOut.");
            }
        }

        #region channel
        protected void channel_SendMessageSuccess(object sender, string sentData)
        {
            channel.Read();
            //  channel.AsynRead();
        }

        protected void channel_ReceiveMessageSuccessUsingStream(object sender, MemoryStream responseData)
        {
            timeOutTimerOMT.Enabled = false;
            
            TradeStationComm.MsgResponse.ResponseObject theMsgObj = TradeStationComm.MsgResponse.getResponseAnyMsg(responseData);
            if (theMsgObj != null)
            {
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
                                GOSTradeStation.msgChannel = channel;

                                GOSTradeStation.UserID = userName;
                                // GOSTradeStation.Pwd = Pwd;
                                GOSTradeStation.AccountID = userName;
                                GOSTradeStation.Chk_mrgin_opt = data.Chk_mrgin_opt;

                                GOSTradeStation.userData.SessionHash = data.SessionHash;
                                GOSTradeStation.userData.UserID = data.UserID;
                                GOSTradeStation.userData.AccountID = userName;
                                // GOSTradeStation.userData.Pwd = Pwd;
                                GOSTradeStation.userData.Chk_mrgin_opt = data.Chk_mrgin_opt;
                                GOSTradeStation.Tw = (data.Tw == null || data.Tw.Trim() == "") ? "" : " - " + data.Tw.Trim();
                                GOSTradeStation.Tc = (data.Tc == null || data.Tc.Trim() == "") ? "" : data.Tc.Trim();

                                channel.SendMessageSuccess -= channel_SendMessageSuccess;
                                channel.ReceiveMessageSuccessUsingStream -= channel_ReceiveMessageSuccessUsingStream;
                                channel.Error -= channel_Error;
                                channel.Connecting -= channel_Connecting;
                                channel.Connected -= channel_Connected;
                                channel.Disconnected -= channel_Disconnected;
                                channel = null;
                                // string strDisText = TradeStationTools.Base64StringToString(data.DisText);
                                System.Windows.Application.Current.Dispatcher.BeginInvoke(this.UpdateDelegate, new Object[] { " Access to OMT!", data.DisText, data.DateTime, (data.Cp == "1") ? true : false });
                                TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + "  Access to OMT!");
                            }
                            else
                            {
                                TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + data.ErrorMsg);
                                System.Windows.Application.Current.Dispatcher.BeginInvoke(this.UpdateDelegate, new Object[] { "UnLogin!", data.ErrorMsg + "!", null, null });
                            }
                        }
                        break;
                }
            }
            else
            {
                string strbody = TradeStationTools.getMsg(responseData);
                TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " Error:" + strbody);
                System.Windows.Application.Current.Dispatcher.BeginInvoke(this.UpdateDelegate, new Object[] { "UnLogin!", strbody + "!", null, null });
            }
        }

        protected void channel_Disconnected(object sender)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(this.UpdateDelegate, new Object[] { "TimeOut", "Login Timeout!", null, null });
            TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + AppFlag.IPOMT + " OMT Error:: Login timeout!");
        }

        protected void channel_Connected(object sender)
        {
             timeOutTimerOMT.Start();

            TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + AppFlag.IPOMT + " OMT Connected.");
            System.Windows.Application.Current.Dispatcher.BeginInvoke(this.UpdateDelegate, new Object[] { " OMT Connected....", " OMT Connected....", null, null });
          
            if (channel == null) return;
            channel.Send(TradeStationComm.MsgRequest.getMsgLogin(userName, Pwd, GOSTradeStation.isDealer, (AppFlag.DefaultLanguage == "en-US") ? TradeStationComm.Attribute.Language.English : (AppFlag.DefaultLanguage == "zh-CN") ? TradeStationComm.Attribute.Language.ChineseSimplified : TradeStationComm.Attribute.Language.ChinesTraditional));
        }

        protected void channel_Connecting(object sender)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(this.UpdateDelegate, new Object[] { " OMT Connecting....", " OMT Connecting....", null, null });
            TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + AppFlag.IPOMT + " OMT Connecting...  ");
        }

        protected void channel_Error(object sender, Exception ex)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(this.UpdateDelegate, new Object[] { "Error", "Please Check Connection!", null, null });
            TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + AppFlag.IPOMT + " OMT Error:: " + ex.ToString());
        }

        #endregion

        private void UpdateLoginStatusMethod(string strStatus, string strDetails, DateTime dateTime, bool bPopupPwd)
        {
            if (chkSaveAccount.IsChecked == true)
            {
                AppFlag.DefaultUser = txtAccount.Text.Trim();
            }
            else
            {
                AppFlag.DefaultUser = "";
            }

            AppFlag.DefaultLanguage = (rdzhCN.IsChecked == true) ? "zh-CN" : (rdzhTW.IsChecked == true) ? "zh-TW" : "en-US";

            if (LoginReslut != null)
            {
                LoginReslut(this, strStatus, strDetails, dateTime, bPopupPwd);
            }

            if (strStatus == "UnLogin!")
            {
                System.Windows.Forms.MessageBox.Show(AletLogin + strDetails,
                               AlertTile,
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
                Done = false;
            }
            else if (strStatus == "Error")
            {
                System.Windows.Forms.MessageBox.Show(AletConnect,
                                 AlertTile,
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information);
                Done = false;
            }
            else if (strStatus == "TimeOut")
            {
                System.Windows.Forms.MessageBox.Show(AletConnect,
                                 AlertTile,
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information);
                Done = false;
            }
            else if (strStatus == " Access to OMT!")
            {
                if (chkSaveAccount.IsChecked == true)
                {
                    TradeStationSetting.UpdateConfig("DefaultUser", txtAccount.Text.Trim());
                    TradeStationSetting.UpdateConfig("DefaultLanguage", AppFlag.DefaultLanguage);

                    string strServerName = tbName.Text.Trim();
                    string ip = (cbServer.Text.IndexOf(":") > 0) ? cbServer.Text.Substring(cbServer.Text.IndexOf(":") + 1, cbServer.Text.Length - cbServer.Text.IndexOf(":") - 1) : cbServer.Text.Trim();
                    TradeStationSetting.SaveServerSettings(strServerName, ip);
                }
            }
        }

        private void imgLogin_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Done) return;

            TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "BuildDate:" + AppFlag.BuildDate + ",Login: UserName:" + txtAccount.Text + ", Start login!");
            string strIp = "";
            if (ValidateInfo())
            {
                strIp = TradeStationSetting.ReturnServerIp(cbServer.Text);
                if (strIp == "")
                {
                    System.Windows.Forms.MessageBox.Show(AlertInvaildMsg, AlertTile, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                return;
            }
            if (channel == null)
            {
                //string strServerName = tbName.Text.Trim();
                //string ip = (cbServer.Text.IndexOf(":") > 0) ? cbServer.Text.Substring(cbServer.Text.IndexOf(":") + 1, cbServer.Text.Length - cbServer.Text.IndexOf(":") - 1) : cbServer.Text.Trim();

                //AppFlag.IPOMP = ip;
                //AppFlag.IPOMT = ip;

                AppFlag.IPOMP = strIp;
                AppFlag.IPOMT = strIp;

                channel = new MessageChannel(AppFlag.IPOMT, AppFlag.PortOMT);
                channel.SendMessageSuccess += this.channel_SendMessageSuccess;
                channel.ReceiveMessageSuccessUsingStream += channel_ReceiveMessageSuccessUsingStream;
                channel.Error += channel_Error;
                channel.Connecting += channel_Connecting;
                channel.Connected += channel_Connected;
                channel.Disconnected += channel_Disconnected;
                channel.AsynConnect();
            }
            else
            {
                channel.AsynConnect();
            }

            Done = true;

            if (AppFlag.DefaultUser != txtAccount.Text.Trim() && chkSaveAccount.IsChecked == true)
            {
                GOSTradeStation.m_ChangedUser = true;
            } 
        }

        private bool ValidateInfo()
        {
            if (txtAccount.Text.Trim() == string.Empty)
            {
                System.Windows.Forms.MessageBox.Show(AletUser,
                                     AlertTile,
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Information);
                return false;
            }
            if (txtPwd.Password.Trim() == string.Empty)
            {
                System.Windows.Forms.MessageBox.Show(AletPassword,
                                AlertTile,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return false;
            }
            if ((Regex.IsMatch(cbServer.Text.Trim(), @"(^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$)|([a-zA-Z0-9]:(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$)"))
            || (Regex.IsMatch(cbServer.Text.Trim(), @"(^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$)|([a-zA-Z0-9]:(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$)")))
            {
                userName = txtAccount.Text.Trim().ToString();
                Pwd = txtPwd.Password.Trim().ToString();
                return true;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(AlertInvaildMsg,
                               AlertTile,
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
                return false;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            List<Server> servers = new List<Server>();
            servers = TradeStationSetting.ReturnServers();
            cbServer.ItemsSource = servers;
            string selectServer = (servers == null || (servers.FirstOrDefault(a => a.Selected == true)) == null) ? "" : servers.FirstOrDefault(a => a.Selected == true).Name;

            if (selectServer == "")
            {
                cbServer.SelectedIndex = 0;
                cbServer.Focus();
                rdEN.IsChecked = true;
            }
            else
            {
                cbServer.SelectedValue = selectServer;

                if (AppFlag.DefaultUser == "")
                {
                    txtAccount.Focus();
                }
                else
                {
                    txtAccount.Text = AppFlag.DefaultUser;
                    chkSaveAccount.IsChecked = true;
                    txtPwd.Focus();

                }
                if (AppFlag.DefaultLanguage == "en-US")
                {
                    rdEN.IsChecked = true;
                }
                else if (AppFlag.DefaultLanguage == "zh-TW")
                {
                    rdzhTW.IsChecked = true;
                }
                else if (AppFlag.DefaultLanguage == "zh-CN")
                {
                    rdzhCN.IsChecked = true;
                }
                else
                {
                    rdEN.IsChecked = true;
                }
            }
        }

        private void UserControl_UnLoaded(object sender, RoutedEventArgs e)
        {
            timeOutTimerOMT.Elapsed -= new ElapsedEventHandler(OnTimeOutEvent);
            //    if (chkSaveAccount.IsChecked == true)
            //    {
            //        TradeStationSetting.UpdateConfig("DefaultUser", txtAccount.Text.Trim());
            //        TradeStationSetting.UpdateConfig("DefaultLanguage", AppFlag.DefaultLanguage);

            //        //string strServerName = tbName.Text.Trim();
            //        //string ip = (cbServer.Text.IndexOf(":") > 0) ? cbServer.Text.Substring(cbServer.Text.IndexOf(":") + 1, cbServer.Text.Length - cbServer.Text.IndexOf(":") - 1) : cbServer.Text.Trim();
            //        //TradeStationSetting.SaveServerSettings(strServerName, ip);
            //    }
            //    else
            //    {
            //        AppFlag.DefaultLanguage = (rdEN.IsChecked == true) ? "en-US" : (rdzhCN.IsChecked == true) ? "zh-CN" : "zh-TW";
            //    }
            //    //else
            //    //{
            //    //    TradeStationSetting.UpdateConfig("DefaultUser", "");
            //    //}
        }

        private void cbServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (channel != null)
            {
                channel.Dispose();
                channel = null;
            }
        }

        private void cbServer_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                tbName.Focus();
            }
        }

        private void tbName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtAccount.Focus();
            }
        }

        private void txtAccount_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtPwd.Focus();
            }
        }

        private void txtPwd_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                imgLogin_MouseDown(sender, new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left));
            }
        }

        private void rdzhTW_Checked(object sender, RoutedEventArgs e)
        {
            AppFlag.DefaultLanguage = "zh-TW";
            GosCulture.CultureHelper.ChangeLanguage(AppFlag.DefaultLanguage);
            if (this.mdiChild != null) this.mdiChild.Title = TradeStationSetting.ReturnWindowName(WindowTypes.Login, "");
        }

        private void rdzhCN_Checked(object sender, RoutedEventArgs e)
        {
            AppFlag.DefaultLanguage = "zh-CN";
            GosCulture.CultureHelper.ChangeLanguage(AppFlag.DefaultLanguage);
            if (this.mdiChild != null) this.mdiChild.Title = TradeStationSetting.ReturnWindowName(WindowTypes.Login, "");
        }

        private void rdEN_Checked(object sender, RoutedEventArgs e)
        {
            AppFlag.DefaultLanguage = "en-US";
            GosCulture.CultureHelper.ChangeLanguage(AppFlag.DefaultLanguage);
            if (this.mdiChild != null) this.mdiChild.Title = TradeStationSetting.ReturnWindowName(WindowTypes.Login, "");
        }
    }
}
