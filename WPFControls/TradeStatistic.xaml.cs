using System;
using System.Windows;
using System.Windows.Media.Imaging;
using GOSTS.WPFControls;
using WPF.MDI;
using System.Windows.Controls;
using System.Globalization;
using GOSTS.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.Sockets; 
using System.Threading;
using System.Resources;
using GOSTS;
using Microsoft.Windows.Controls.Ribbon;
using System.Windows.Media;
using System.Timers;
using System.Linq;
using System.Windows.Documents;
using GOSTS.ViewModel;
using System.IO;

namespace GOSTS.WPFControls
{
    /// <summary>
    /// TradeStatistic.xaml 的交互逻辑
    /// </summary>
    public partial class TradeStatistic : UserControl
    {
        private MessageDistribute distributeMsg = null;
        delegate void BindListItem(DataTable dataTable);
        BindListItem TradeStatisticDelegate;

        public TradeStatistic (MessageDistribute _msgDistribute) 
        {
            InitializeComponent();
            if (_msgDistribute != null)
            {
                distributeMsg = _msgDistribute;
                distributeMsg.DisTradeStatistic += new MessageDistribute.OnDisTradeStatistic(distributeMsg_MPDisTradeStatistic);
                TradeStatisticDelegate = new BindListItem(BindListItemMethod);
            }
        }

        protected void distributeMsg_MPDisTradeStatistic(object sender, DataTable dataTable)
        {
            Application.Current.Dispatcher.BeginInvoke(this.TradeStatisticDelegate, new Object[] { dataTable });
        }

        public void BindListItemMethod(DataTable dataTable)
        {
            this.dgTradeStatistic.ItemsSource = dataTable.DefaultView  ;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           TradeStationSend .Send(cmdClient .getTradeStatistics ,GOSTradeStation .UserID);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            distributeMsg.DisTradeStatistic -= new MessageDistribute.OnDisTradeStatistic(distributeMsg_MPDisTradeStatistic);
               
        }
    }
}
