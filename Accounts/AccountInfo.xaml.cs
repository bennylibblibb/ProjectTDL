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

namespace GOSTS
{
    /// <summary>
    /// AccountInfo.xaml 的交互逻辑
    /// </summary>
    public partial class AccountInfo_bk030402 : UserControl
    {
        delegate void BingUserAccountInfoData(DataTable dtAccountInfo);
        BingUserAccountInfoData AccountInfoData;

        delegate void BingCashInfoData(DataTable dtCashInfo);
        BingCashInfoData CashInfoData;

          UserData userAccountID = new UserData();

        public AccountInfo_bk030402(MessageDistribute _msgDistribute)
        {
            AccountInfoData = new BingUserAccountInfoData(BindAccountInfoItemMethod);
            CashInfoData = new BingCashInfoData(BindCashInfoItemMethod);

            InitializeComponent();

            userAccountID = TradeStationUser.GetLsAccountID(GOSTradeStation.UserID);
          
                if (userAccountID.LsAccountID != null)
                {
                    cb_AccountId.ItemsSource = userAccountID.LsAccountID;
                }
                else
                {
                    cb_AccountId.Items.Add(GOSTradeStation.UserID);
                    cb_AccountId.SelectedValue   = GOSTradeStation.UserID;
                }


            if (_msgDistribute != null)
            {
                distributeMsg = _msgDistribute;
                distributeMsg.DisUserAccountInfo += new MessageDistribute.OnDisUserAccountInfo(distributeMsg_DisAccountInfo);
                distributeMsg.DisCashInfo += new MessageDistribute.OnDisCashInfo(distributeMsg_DisCashInfo);
            }
        }

        public MessageDistribute distributeMsg = null;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TradeStationSend .Send (cmdClient.getAccountInfo, GOSTradeStation.AccountID);
           // GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetAccountInfo(GOSTradeStation.UserID,GOSTradeStation.AccountID));
            GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetCashInfo(GOSTradeStation.UserID,GOSTradeStation.AccountID));
        }

        protected void distributeMsg_DisAccountInfo(object sender, DataTable dtAccount)
        {
            Application.Current.Dispatcher.Invoke(AccountInfoData, new object[] { dtAccount });
        }

        protected void distributeMsg_DisCashInfo(object sender, DataTable dtCashInfo)
        {
            Application.Current.Dispatcher.Invoke(CashInfoData, new object[] { dtCashInfo });
        }

        public void BindAccountInfoItemMethod(DataTable dtAccountInfo)
        {
            bingAccount(dtAccountInfo);
        }

        public void BindCashInfoItemMethod(DataTable dtCashInfo)
        {
            bingCash(dtCashInfo);
        }

        private void bingAccount(DataTable dtAccountInfo)
        {
            if (dtAccountInfo != null)
            {
                this.dg_AccountInfo.DataContext = dtAccountInfo;
            }
        }

        private void bingCash(DataTable dtCashInfo)
        {
            if (dtCashInfo != null)
            {
                this.dg_CashInfo.DataContext = dtCashInfo;
            }
        }

        private void cb_AccountId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_AccountId.SelectedItem != null)
            {
                GOSTradeStation.AccountID = cb_AccountId.SelectedItem.ToString();
                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetAccountInfo(GOSTradeStation.UserID, GOSTradeStation.AccountID));
                GOSTradeStation.msgChannel.Send(TradeStationComm.MsgRequest.getMsgGetCashInfo(GOSTradeStation.UserID, GOSTradeStation.AccountID));
            }
            else
            {
                cb_AccountId.SelectedIndex = 0;
            }
        }

    }
} 
