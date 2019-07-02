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
using System.Data;
using System.Collections.ObjectModel;
using System.Configuration;

namespace GOSTS.Dealer
{
    public partial class TradeConfirmation : UserControl
    {
        public MdiChild mdiChild { get; set; }
        private MessageDistribute msgDistribute;
        delegate void InitBind(DataTable dataTable);
        InitBind InitDelegate;

        delegate ObservableCollection<TradeOrderGroup> BindDataAsync(DataTable dataTable);

        // Show/hide reported
        public static readonly DependencyProperty isVisibilityConfProperty = DependencyProperty.Register("isVisibilityConf",
         typeof(Visibility), typeof(TradeConfirmation), new FrameworkPropertyMetadata(Visibility.Visible));
        public Visibility isVisibilityConf
        {
            get { return (Visibility)GetValue(isVisibilityConfProperty); }
            set { SetValue(isVisibilityConfProperty, value); }
        }

        //Group by (Show/hide  B/S )
        public static readonly DependencyProperty isVisibilityGroupProperty = DependencyProperty.Register("isVisibilityGroup",
         typeof(Visibility), typeof(TradeConfirmation), new FrameworkPropertyMetadata(Visibility.Collapsed));
        public Visibility isVisibilityGroup
        {
            get { return (Visibility)GetValue(isVisibilityGroupProperty); }
            set { SetValue(isVisibilityGroupProperty, value); }
        }

        //(Show reported color )
        public static readonly DependencyProperty tradeSimplysProperty = DependencyProperty.Register("tradeSimplys",
         typeof(ObservableCollection<TradeSimply>), typeof(TradeConfirmation), new FrameworkPropertyMetadata(null));
        public ObservableCollection<TradeSimply> tradeSimplys
        {
            get { return (ObservableCollection<TradeSimply>)GetValue(tradeSimplysProperty); }
            set { SetValue(tradeSimplysProperty, value); }
        }

        public TradeConfirmation(MessageDistribute _msgDistribute)
        {
            InitializeComponent();
            msgDistribute = _msgDistribute;
            msgDistribute.DisTradeConfOrder += msgDistribute_DisTradeConf;
            msgDistribute.DisTradeConfTrade += msgDistribute_DisTradeConf;
            msgDistribute.DisReportTradeConf += msgDistribute_DisReportTradeConf;
            msgDistribute.DisSendReportTradeConfs += msgDistribute_DisSendReportTradeConfs;
            InitDelegate = new InitBind(InitBindMethod);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TradeStationSend.Send(cmdClient.getTradeConfOrders);
            TradeStationSend.Send(cmdClient.getTradeConfTrades);
            this.mdiChild.Width = this.mdiChild.ActualWidth;
            this.mdiChild.Height = this.mdiChild.ActualHeight;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            msgDistribute.DisTradeConfOrder -= msgDistribute_DisTradeConf;
            msgDistribute.DisTradeConfTrade -= msgDistribute_DisTradeConf;
            msgDistribute.DisReportTradeConf -= msgDistribute_DisReportTradeConf;
            msgDistribute.DisSendReportTradeConfs -= msgDistribute_DisSendReportTradeConfs;
        }

        protected void msgDistribute_DisSendReportTradeConfs(object sender, ObservableCollection<TradeSimply> dataReports, int type)
        {
            string[] recArr = dataReports.Select(d => d.recNo).ToArray<string>();

            if (dgSecond.Visibility == Visibility.Visible)
            {
                DataView dv = (DataView)dgSecond.ItemsSource;
                if (dv == null) return;
                foreach (DataRowView drv in dv)
                {
                    if (recArr.Contains(drv.Row["recNo"]))
                    {
                        if ((drv.Row[6].ToString().IndexOf("100") > -1 || drv.Row[6].ToString().IndexOf("120") > -1 || drv.Row[6].ToString().IndexOf("121") > -1) && type == 0)
                        {
                            drv.Row[6] = "UnReporting";
                        }
                        else if ((drv.Row[6].ToString().IndexOf("0") > -1 || drv.Row[6].ToString().IndexOf("20") > -1 || drv.Row[6].ToString().IndexOf("21") > -1) && type == 1)
                        {
                            drv.Row[6] = "Reporting";
                        }
                    }
                }

                //for (int i = 0; i < dgSecond.Items.Count; i++)
                //{
                //    DataGridRow dgr = (DataGridRow)dgSecond.ItemContainerGenerator.ContainerFromIndex(i);
                //    if (dgr == null) continue; ;

                //    TextBlock cellContent = dgSecond.Columns[0].GetCellContent(dgr) as TextBlock;
                //    if (cellContent == null) continue;

                //    DataRowView drv = dgr.Item as DataRowView;
                //    if (drv == null) continue;

                //    if (recArr.Contains(drv.Row["recNo"]))
                //    {
                //        if ((drv.Row[6].ToString().IndexOf("100;") > -1 || drv.Row[6].ToString().IndexOf("120;") > -1 || drv.Row[6].ToString().IndexOf("121;") > -1) && type == 0)
                //        {
                //            cellContent.Text = "UnReporting";
                //        }
                //        else if ((drv.Row[6].ToString().IndexOf("0;") > -1 || drv.Row[6].ToString().IndexOf("20;") > -1 || drv.Row[6].ToString().IndexOf("21;") > -1) && type == 1)
                //        {
                //            cellContent.Text = "Reporting";
                //        }
                //    }
                //}
            }
            else
            {
                ObservableCollection<TradeOrderGroup> togs = (ObservableCollection<TradeOrderGroup>)dgTradeGroup.ItemsSource;
                if (togs == null) return;
                foreach (TradeSimply ts in dataReports)
                {
                    TradeOrderGroup dr = togs.FirstOrDefault(x => x.accNo == ts.accNo && x.orderNo == ts.orderNo && x.productCode == ts.productCode) as TradeOrderGroup;
                    if ((dr.report.ToString() == "100" || dr.report.ToString() == "120" || dr.report.ToString() == "121") && type == 0)
                    {
                        dr.report = "UnReporting";
                    }
                    else if ((dr.report.ToString() == "0" || dr.report.ToString() == "20" || dr.report.ToString() == "21") && type == 1)
                    {
                        dr.report = "Reporting";
                    }
                }

                //for (int i = 0; i < dgTradeGroup.Items.Count; i++)
                //{
                //    DataGridRow dgr = (DataGridRow)dgTradeGroup.ItemContainerGenerator.ContainerFromIndex(i);
                //    if (dgr == null) continue;

                //    TradeOrderGroup dr = dgr.Item as TradeOrderGroup;
                //    if (dr == null) continue;

                //    if (dr.accNo == dataReports[0].accNo && dr.orderNo == dataReports[0].orderNo && dr.productCode == dataReports[0].productCode)
                //    {
                //        if ((dr.report.ToString() == "100" || dr.report.ToString() == "120" || dr.report.ToString() == "121") && type == 0)
                //        {
                //            ((TextBlock)dgTradeGroup.Columns[0].GetCellContent(dgr)).Text = "UnReporting"; 
                //        }
                //        else if ((dr.report.ToString() == "0" || dr.report.ToString() == "20" || dr.report.ToString() == "21") && type == 1)
                //        {
                //            ((TextBlock)dgTradeGroup.Columns[0].GetCellContent(dgr)).Text = "Reporting"; 
                //        }
                //    }
                //}
            }

            TradeStationSend.Send(cmdClient.reportTradeConf, recArr, type);
        }

        protected void msgDistribute_DisReportTradeConf(object sender, string str)
        {
            TradeStationSend.Send(cmdClient.getTradeConfTrades);
        }

        protected void msgDistribute_DisTradeConf(object sender, DataTable dataTable)
        {
            Application.Current.Dispatcher.BeginInvoke(this.InitDelegate, new Object[] { dataTable });
        }

        public void InitBindMethod(DataTable dataTable)
        {
            if (dataTable.TableName == "TradeConfOrder")
            {
                this.dgMain.ItemsSource = dataTable.DefaultView;
            }
            else
            {
                this.dgSecond.ItemsSource = dataTable.DefaultView;

                BindDataAsync bindData = new BindDataAsync(BindData);
                IAsyncResult iar = bindData.BeginInvoke(dataTable, new AsyncCallback(EndBindData), bindData);
            }
        }

        void SetConfirmationDetails(DataRowView drv)
        {
            string orderNo = drv["orderNo"].ToString();
            string accNO = drv["accNo"].ToString();
            var drvs = new List<DataRowView>();
            DataView data = (DataView)dgSecond.ItemsSource;
            foreach (DataRowView dv in data)
            {
                if (null != dv && dv["orderNo"].ToString() == orderNo && dv["accNo"].ToString() == accNO)
                {
                    drvs.Add(dv);
                }
            }

            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
            MdiChild mcTradeConfirm = new MdiChild();
            TradeConfirmDetails tradeConfirm = new TradeConfirmDetails(msgDistribute);
            tradeConfirm.mdiChild = mcTradeConfirm;
            tradeConfirm.drvs = drvs;
            mcTradeConfirm.Title = TradeStationSetting.ReturnWindowName(WindowTypes.TradeConfirmDetails ,"");
            mcTradeConfirm.Content = tradeConfirm;
            mcTradeConfirm.Width = 800;
            mcTradeConfirm.Height = 380;
            double y = (((Container.ActualHeight - Mouse.GetPosition(Container).Y) > mcTradeConfirm.Height) ? Mouse.GetPosition(Container).Y + 22 : Mouse.GetPosition(Container).Y - 300-22);
            double x = (((Container.ActualWidth - Mouse.GetPosition(Container).X) > mcTradeConfirm.Width) ? this.mdiChild.Position.X : Container.ActualWidth - 800);
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            mcTradeConfirm.Position = new System.Windows.Point(x,y); 

            if (!mcTradeConfirm.ExistInContainer(mcTradeConfirm, Container))
            {
                Container.Children.Add(mcTradeConfirm);
            }
            else
            {
                foreach (MdiChild child in Container.Children)
                {
                    if (child.Title == mcTradeConfirm.Title)
                    {
                        child.Content = tradeConfirm;
                        child.Focus();
                        break;
                    }
                }
            }

            IDelearStatus.ChangeSelectedAcc(this, drv["accNo"].ToString());
        }

        private void dgTradeGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (tradeSimplys != null && tradeSimplys.Count > 0)
            //{
            //    string orderNo = tradeSimplys[0].orderNo;
            //    for (int i = 0; i < dgTradeGroup.Items.Count; i++)
            //    {
            //        DataGridRow dgr = (DataGridRow)dgTradeGroup.ItemContainerGenerator.ContainerFromIndex(i);
            //        if (dgr == null) return;
            //        TradeOrderGroup tradeOrderGroup = dgr.Item as TradeOrderGroup;
            //        if (tradeOrderGroup == null) return;
            //        if (orderNo == tradeOrderGroup.orderNo)
            //        {
            //            dgr.Background = new SolidColorBrush(Colors.White);
            //        }
            //    }
            //    tradeSimplys = null;
            //}
            //else
            //{
            //    for (int i = 0; i < dgTradeGroup.Items.Count; i++)
            //    {
            //        DataGridRow dgr = dgTradeGroup.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
            //        if (dgr == null) return;
            //        dgr.Background = new SolidColorBrush(Colors.White);
            //    }
            //}
        }

        private void dgSecond_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (dgSecond.Visibility == Visibility.Visible)
            //{
            //    if (tradeSimplys != null)
            //    {
            //        string[] recArr = tradeSimplys.Select(d => d.recNo).ToArray<string>();
            //        for (int i = 0; i < dgSecond.Items.Count; i++)
            //        {
            //            DataGridRow dgr = (DataGridRow)dgSecond.ItemContainerGenerator.ContainerFromIndex(i);
            //            if (dgr == null) return;

            //            DataRowView drv = dgr.Item as DataRowView;
            //            if (recArr.Contains(drv["recNo"].ToString()))
            //            {
            //                dgr.Background = new SolidColorBrush(Colors.White);
            //            }
            //            //TextBlock cellContent = dgSecond.Columns[0].GetCellContent(row) as TextBlock;
            //            //if (cellContent != null && cellContent.Text.Equals("Reported"))
            //            //{
            //            //    object item = dgSecond.Items[i];
            //            //    dgSecond.SelectedItem = item;
            //            //    dgSecond.ScrollIntoView(item);
            //            //    row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            //            //    break;
            //            //}
            //        }
            //        tradeSimplys = null;
            //    }
            //    else
            //    {
            //        for (int i = 0; i < dgSecond.Items.Count; i++)
            //        {
            //            DataGridRow dgr = dgSecond.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
            //            if (dgr == null) return;
            //            dgr.Background = new SolidColorBrush(Colors.White);
            //        }
            //    }
            //}

            if (GOSTradeStation.customizeData.AlertData.isSingleClick == false) return;

            if (sender != null)
            {
                DataRowView drv = dgSecond.SelectedItem as DataRowView;
                if (drv != null)
                {
                    SetConfirmationDetails(drv);
                }
            }
        }

        private void dgSecond_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (GOSTradeStation.customizeData.AlertData.isDoubleClick == false) return;

            if (sender != null)
            {
                DataGridRow dgr = sender as DataGridRow;
                if (dgr != null)
                {
                    DataRowView drv = dgr.Item as DataRowView;
                    if (drv == null) return;
                    SetConfirmationDetails(drv);
                }
            }
        }

        private void dgMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.dgMain.SelectedItem != null)
            {
                try
                {
                    DataRowView drv = this.dgMain.SelectedItem as DataRowView;
                    if (drv == null) return;

                    DataTable table = drv.DataView.Table.Clone();
                    DataRowView _row = table.DefaultView.AddNew();
                    _row.Row.ItemArray = (object[])drv.Row.ItemArray.Clone();

                    if (drv["DepInPrice"].ToString() != "0")
                    {
                        _row["price"] = TradeStationTools.ConvertToFormatStringNull(drv["price"], Convert.ToInt32(drv["DepInPrice"]));
                    }

                    string accNo = drv["accNo"].ToString();
                    uint osBQty = Convert.ToUInt32(drv["osBQty"]);
                    uint osSQty = Convert.ToUInt32(drv["osSQty"]);
                    string tone = Utility.getColumnValue(drv.Row, "tOne");
                    bool bInitMdi = false;

                    string cond = drv.Row.getColValue("cond");
                    //if (!GosBzTool.CanChangeOrder(cond))
                    //{
                    //    MessageBox.Show("sorry,cond '" + cond + "' can't change now");
                    //}
                    //else
                    {

                        if (osBQty > 0)
                        {
                            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;
                            MdiChild ChangeOrderChild = new MdiChild();
                            ChangeOrder changeOrder = ChangeOrder.getChangeOrder(msgDistribute, _row, "buy", osBQty, ChangeOrderChild, accNo, tone, ref bInitMdi);

                            if (changeOrder == null)
                            {
                                return;
                            }
                            if (bInitMdi)
                            {
                                ChangeOrderChild.Width = GosBzTool.ChOrderWidth;
                                ChangeOrderChild.Height = GosBzTool.ChOrderHeight;
                                //  ChangeOrderChild.MaximizeBox = true;
                                ChangeOrderChild.Position = new System.Windows.Point(80, 30);
                                ChangeOrderChild.Content = changeOrder;

                                if (!Container.Children.Contains(ChangeOrderChild))
                                {
                                    Container.Children.Add(ChangeOrderChild);
                                }
                            }
                        }
                        else
                        {
                            MdiContainer Container = App.Current.MainWindow.FindName("Container") as MdiContainer;

                            MdiChild ChangeOrderChild = new MdiChild();
                            ChangeOrder ChangeOrderSell = ChangeOrder.getChangeOrder(msgDistribute, _row, "sell", osSQty, ChangeOrderChild, accNo, tone, ref bInitMdi);
                            if (bInitMdi)
                            {
                                ChangeOrderChild.Width = GosBzTool.ChOrderWidth;
                                ChangeOrderChild.Height = GosBzTool.ChOrderHeight;
                                //  ChangeOrderChild.MaximizeBox = true;
                                ChangeOrderChild.Position = new System.Windows.Point(180, 30);
                                ChangeOrderChild.Content = ChangeOrderSell;

                                if (!Container.Children.Contains(ChangeOrderChild))
                                {
                                    Container.Children.Add(ChangeOrderChild);
                                }
                            }
                        }
                    }

                    IDelearStatus.ChangeSelectedAcc(this, accNo);
                }
                catch { }
            }
        }

        private void dgTradeGroup_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow dgr = sender as DataGridRow;
            if (dgr == null) return;
            TradeOrderGroup tradeOrderGroup = dgr.Item as TradeOrderGroup;
            if (tradeOrderGroup == null) return;
            DataTable dt = new DataTable();
            dt.Columns.Add("accNo", System.Type.GetType("System.String"));
            dt.Columns.Add("orderNo", System.Type.GetType("System.String"));
            DataRow dr = dt.NewRow();
            dr["accNo"] = tradeOrderGroup.accNo;
            dr["orderNo"] = tradeOrderGroup.orderNo;
            dt.Rows.Add(dr);

            DataRowView drv2 = dt.DefaultView[dt.Rows.IndexOf(dr)];
            DataGridRow dgr2 = new DataGridRow();
            dgr2.Item = drv2;

            this.dgSecond_MouseDoubleClick(dgr2, e);
        }

        private void cmGroupby_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            //Grid gridInTemplate = (Grid)myButton1.Template.FindName("grid", myButton1);

            ContextMenu cmGroupby = (ContextMenu)dgSecond.Template.FindName("cmReported", dgSecond);
            if (cmGroupby == null) return;

            for (int i = 0; i < cmGroupby.Items.Count; i++)
            {
                MenuItem rmi = cmGroupby.Items[i] as MenuItem;
                if (i == 0)
                {
                    rmi.IsChecked = (isVisibilityConf == Visibility.Collapsed) ? true : false;
                }
                else if (i == 1)
                {
                    rmi.IsChecked = (isVisibilityConf == Visibility.Collapsed) ? false : true;
                }
                else if (i == 2)
                {
                    rmi.IsChecked = (isVisibilityGroup == Visibility.Collapsed) ? false : true;
                }
            }
        }

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.OriginalSource;
            switch ((string)menuItem.Tag)
            {
                case "miHide":
                    isVisibilityConf = Visibility.Collapsed;
                    menuItem.Header = "Show Confirmed Trade";
                    menuItem.Tag = "miShow";
                    break;
                case "miShow":
                    isVisibilityConf = Visibility.Visible;
                    menuItem.Header = "Hide Confirmed Trade";
                    menuItem.Tag = "miHide";
                    break;
                case "miGroup":
                    this.dgSecond.Visibility = Visibility.Collapsed;
                    this.dgTradeGroup.Visibility = Visibility.Visible;
                    menuItem.Header = "UnGroup Trades";
                    menuItem.Tag = "miUnGroup";
                    break;
                case "miUnGroup":
                    this.dgSecond.Visibility = Visibility.Visible;
                    this.dgTradeGroup.Visibility = Visibility.Collapsed;
                    menuItem.Header = "Group Trades";
                    menuItem.Tag = "miGroup";
                    break;
            }
        }

        private void EndBindData(IAsyncResult iar)
        {
            BindDataAsync bindData = (BindDataAsync)iar.AsyncState;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.dgTradeGroup.ItemsSource = bindData.EndInvoke(iar);
            }));
        }

        private ObservableCollection<TradeOrderGroup> BindData(DataTable dataTable)
        {
            ObservableCollection<TradeOrderGroup> tradeOrderGroups = new ObservableCollection<TradeOrderGroup>();
            TradeOrderGroup tradeOrderGroup;

            foreach (DataRow dr in dataTable.Rows)
            {
                var list = (from filter in tradeOrderGroups
                            where filter.accNo.Equals(dr["accNo"]) && filter.orderNo.Equals(dr["orderNo"]) && filter.productCode.Equals(dr["productCode"])
                            select filter).ToList();
                if (list.Count == 0)
                {
                    tradeOrderGroup = new TradeOrderGroup();
                    tradeOrderGroup.accNo = dr["accNo"].ToString();
                    tradeOrderGroup.orderNo = dr["orderNo"].ToString();
                    tradeOrderGroup.productCode = dr["productCode"].ToString();
                    tradeOrderGroup.DepInPrice = MarketPriceData.GetDecInPrice(dr["productCode"].ToString());

                    string strfilter = "accNo='" + dr["accNo"] + "'and orderNo='" + dr["orderNo"] + "'and productCode='" + dr["productCode"] + "'";
                    DataRow[] groupRow = dataTable.Select(strfilter);
                    if (groupRow != null)
                    {
                        tradeOrderGroup.bs = (groupRow[0]["osBQty"].ToString() == "" || groupRow[0]["osBQty"].ToString() == "0") ? "SELL" : "BUY";

                        int sum = 0;
                        bool bReported = true;
                        foreach (DataRow dr2 in groupRow)
                        {
                            if (bReported)
                            {
                                if (dr2["status"].ToString().IndexOf("100") == -1 && dr2["status"].ToString().IndexOf("120") == -1 && dr2["status"].ToString().IndexOf("121") == -1)
                                {
                                    bReported = false;
                                }
                            }

                            if (tradeOrderGroup.bs == "SELL")
                            {
                                tradeOrderGroup.tradeQty += TradeStationTools.ConvertToInt(dr2["osSQty"]);
                                sum += Convert.ToInt32(dr2["osSQty"]) * TradeStationTools.ConvertToInt(dr2["tradedPrice"]);
                            }
                            else
                            {
                                tradeOrderGroup.tradeQty += TradeStationTools.ConvertToInt(dr2["osBQty"]);
                                sum += Convert.ToInt32(dr2["osBQty"]) * TradeStationTools.ConvertToInt(dr2["tradedPrice"]);
                            }

                        }

                        tradeOrderGroup.report = (bReported == false) ? "0" : "100";
                        tradeOrderGroup.avgTrdPrc = (tradeOrderGroup.tradeQty != 0) ? (sum / tradeOrderGroup.tradeQty).ToString() : "";

                        tradeOrderGroups.Add(tradeOrderGroup);
                    }
                }
            }

            return tradeOrderGroups; 
        }
    }
}