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
using System.Data  ;
using GOSTS.WPFControls;  
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;



namespace GOSTS.Dealer
{ 
    public partial class TradeConfirmDetails : UserControl
    {
        public MdiChild mdiChild { get; set; }
        public string AccNo { get; set; }
        public List<DataRowView> drvs { get; set; }
        delegate void InitBind(DataTable dataTable, DataTable orderDetailTable);
        InitBind InitDelegate;
        private MessageDistribute msgDistribute; 
        ObservableCollection<TradeSimply> tradeSimplys = new ObservableCollection<TradeSimply>();

        public TradeConfirmDetails(MessageDistribute _msgDistribute)
        {
            InitializeComponent();
            if (_msgDistribute != null)
            {
                msgDistribute = _msgDistribute;
                msgDistribute.DisAccountMaster += msgDistribute_DisAccountMaster;
                msgDistribute.DisTradeConfOrderDetail += msgDistribute_DisTradeConfOrderDetail;
                InitDelegate = new InitBind(InitBindMethod);
            }
            dgSecond.ItemsSource = tradeSimplys; 
            dgSecond.SelectedIndex = 0;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Focus control
            msgDistribute.DistributeControlFocus(this.mdiChild.Title, this.mdiChild);

            if (drvs == null) return;

             TradeSimply dr2; 
            foreach (DataRowView drvdr in drvs)
            {
                dr2 = new TradeSimply();
                dr2.accNo = drvdr["accNo"].ToString();
                dr2.report = drvdr["status"].ToString();
                dr2.productCode = drvdr["productCode"].ToString();
                dr2.initiator = drvdr["initiator"].ToString();
                dr2.osBQty = drvdr["osBQty"].ToString();
                dr2.osSQty = drvdr["osSQty"].ToString();
                dr2.DepInPrice =Convert .ToInt32 (drvdr["DepInPrice"]);
                dr2.tradedPrice = drvdr["tradedPrice"].ToString();
                dr2.UpdateTime =(DateTime)drvdr["tStamp"];
                dr2.recNo = drvdr["recNo"].ToString();
                dr2.refNo = drvdr["refNo"].ToString();
                dr2.orderNo = drvdr["orderNo"].ToString();
                dr2.tradeNo = drvdr["tradeNo"].ToString();
                dr2.extOrderNo = drvdr["extOrderNo"].ToString();
                dr2.status = drvdr["status"].ToString();
                tradeSimplys.Add(dr2);
            }

            AccNo = drvs[0]["accNo"].ToString();
            TradeStationSend.Send(cmdClient.getMsgAccountMaster, drvs[0]["accNo"].ToString());
            TradeStationSend.Send(cmdClient.getTradeConfOrderDetail,drvs[0]["accNo"].ToString(), drvs[0]["orderNo"].ToString());
        }
           
        protected void msgDistribute_DisTradeConfOrderDetail(object sender, DataTable dataTable)
        {
            Application.Current.Dispatcher.BeginInvoke(this.InitDelegate, new Object[] {null, dataTable });
        }

        protected void msgDistribute_DisAccountMaster(object sender, DataTable dataTable, string AccNo)
        {
            Application.Current.Dispatcher.BeginInvoke(this.InitDelegate, new Object[] { dataTable,null});
        }

        public void InitBindMethod(DataTable dataTable, DataTable orderDetailsTable)
        {
            if (dataTable != null&&tradeSimplys.Count >0)
            {
                DataRow[] drs = dataTable.Select("accNo='" + tradeSimplys[0].accNo + "'");
                if (drs.Length <= 0) return;
                DataTable data = dataTable.Clone();
                data.Rows.Add(drs[0].ItemArray);
                this.dgMaster.ItemsSource = data.DefaultView;
                this.dgMasterSMS.Text = (this.dgMasterSMS.Text == "SMS:") ? "SMS:" + drs[0]["maxMargin"].ToString() : this.dgMasterSMS.Text;
            }
            else if (orderDetailsTable != null)
            {
                this.dgMain.ItemsSource = orderDetailsTable.DefaultView;
            }
        }

        private void btnMarkAll_Click(object sender, RoutedEventArgs e)
        { 
            msgDistribute.DistributeSendReportTradeConf(tradeSimplys, 1);
            btnExit_Click(sender, e);
        }

        private void btnUnMarkAll_Click(object sender, RoutedEventArgs e)
        {
            msgDistribute.DistributeSendReportTradeConf(tradeSimplys, 0);
            btnExit_Click(sender, e);
        }

        private void btnUnMarkInd_Click(object sender, RoutedEventArgs e)
        {
            if (dgSecond.SelectedItem != null)
            {
                ObservableCollection<TradeSimply > data = new ObservableCollection<TradeSimply>();
                data.Add((TradeSimply)this.dgSecond.SelectedItem);

                msgDistribute.DistributeSendReportTradeConf(data, 0);

                btnExit_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Please select Trade#!");
            }
        }

        private void btnOrderLog_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            //    this.mdiChild.Close();
            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
            foreach (MdiChild child in Container.Children)
            {
                if (child.Content.GetType() == this.GetType())
                {
                    child.Close();
                    break;
                }
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            msgDistribute.DisAccountMaster -= msgDistribute_DisAccountMaster;
            msgDistribute.DisTradeConfOrderDetail -= msgDistribute_DisTradeConfOrderDetail;
        }

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            ObservableCollection<TradeSimply> data = new ObservableCollection<TradeSimply>();
            TradeSimply tradeSimply = (TradeSimply)this.dgSecond.SelectedItem;
            data.Add(tradeSimply);

            MenuItem menuItem = (MenuItem)e.OriginalSource;
            switch ((string)menuItem.Tag)
            {
                case "miReported": 
                    if (tradeSimply.report == "100" || tradeSimply.report == "120" || tradeSimply.report == "121") return;
                     msgDistribute.DistributeSendReportTradeConf(data, 1);
                    break;
                case "miUnReported": 
                    if (tradeSimply.report != "100" && tradeSimply.report != "120" && tradeSimply.report != "121") return;
                    msgDistribute.DistributeSendReportTradeConf(data, 0);
                    break;
            }
        }
    } 
}
