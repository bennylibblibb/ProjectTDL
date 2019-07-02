using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using GOSTS.Common;
using GOSTS.ViewModel;
using WPF.MDI;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Data;
using System.Timers;
using GOSTS;
 
namespace GOSTS.Dealer
{
    /// <summary>
    /// MarginCallList.xaml 的交互逻辑
    /// </summary>
    public partial class MarginCallList : UserControl
    { 
        public MdiChild mdiChild { get; set; }
         
        public MarginCheckViewModel marginCheckViewModel { get; set; }
        private System.Timers.Timer TimerMargin;

        MarginColorData data;
             
        public MarginCallList(MessageDistribute _msgDistribute)
        {
            InitializeComponent();

            marginCheckViewModel = new MarginCheckViewModel();
            this.DataContext = marginCheckViewModel; 

            TimerMargin = new System.Timers.Timer(AppFlag.MCLReload);
            TimerMargin.Elapsed += OnTimerMarginEvent;
            TimerMargin.Enabled = false;

            data = TradeStationSetting.ReturnCustomizeData(GOSTradeStation.UserID);
         }

        //Reload MarginCallList
        private void OnTimerMarginEvent(object source, ElapsedEventArgs e)
        {
            if (this.marginCheckViewModel.MarginCheckAccDataes!=null &&this.marginCheckViewModel.MarginCheckAccDataes.Count > 0)
            {
                TradeStationSend.Send(cmdClient.getMarginCallList);
                TradeStationLog.WriteForHearbeatandMarginCall(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " Reload MarginCallList.");
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
             TimerMargin.Start();
        }

         private void UserControl_Unloaded(object sender, RoutedEventArgs e)
         {
             TimerMargin.Elapsed -= OnTimerMarginEvent;
         }

         private void dgMarginCallList_LoadingRow(object sender, DataGridRowEventArgs e)
         {
             if (data == null || data.isEnabled == true || data.MarginColors==null) return;
             int per = 100;
             int per2 ; 
             foreach (MarginColor s in data.MarginColors)
             {
                 per2 = s.Percent ;
                 if (((MarginCheckAccData)e.Row.Item).MLevel < per && ((MarginCheckAccData)e.Row.Item).MLevel > per2)
                 {
                     e.Row.Background = s.Color;
                     break;
                 }
                 else if (((MarginCheckAccData)e.Row.Item).MLevel >= per)
                 {
                     e.Row.Background = s.Color;
                     break;
                 }
                 per = s.Percent ;
             } 
         }

         private void dgMarginCallList_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
         { 
         }

         private void resultDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
         {
             if (sender != null)
             {
                 DataGridRow dgr = sender as DataGridRow;
                 if (dgr != null)
                 {
                     MarginCheckAccData marginCheckAccData = dgr.Item as MarginCheckAccData;
                     IDelearStatus.ChangeSelectedAcc(this, marginCheckAccData.AccNo );
                 }
             }
         }
    }
}
