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
using System.Collections.ObjectModel;
using GOSTS.Common;
using System.Data;
using System.Threading;
using WPF.MDI;
using System.ComponentModel;

namespace GOSTS
{
    /// <summary>
    /// PosSumary.xaml 的交互逻辑
    /// </summary>
    public partial class PosSumary : IDelearUser
    {
        PositionBus PosBus;
        ObservableCollection<OrderPosition> posCollection = new ObservableCollection<OrderPosition>();        
        public MdiChild mdiChild { get; set; }
        public PosSumary(MessageDistribute _msgDistribute,MdiChild _mdiChild)
        {            
            InitializeComponent();
            mdiChild = _mdiChild;         
          //  SetMdITitle(_msgDistribute, mdiChild, "Position Summary");            
            InitCurUser(this.cbbUsers, _msgDistribute);
            SetFormTitle(0, _msgDistribute, mdiChild, new string[] { "" });
            PosBus = PositionBus.GetSinglePositionBus(_msgDistribute);
            if (PosBus != null)
            {
                PosDataChange = new PosInfoChange(PositionBusDataChange);
                PosBus.RegisterOnPosInfoChange(new PositionBus.PosInfoChange(PosBus_DataChangeInvoke));
                this.posCollection = PosBus.getPosCL();
                if (this.posCollection != null)
                {
                    viewSource.Source = this.posCollection;                 
                    this.dgPosition.ItemsSource = viewSource.View;                 
                }
            }

             this.dgPosition.PreviewMouseDown += PosSumary_PreviewMouseDown;           
        }
        string RL = "L";
        void PosSumary_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                RL = "L";
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                RL = "R";
            }
        }
        

        public override void GetAccountMaster(object sender, DataTable tableMaster, string RecAcc)
        {
            base.GetAccountMaster(sender, tableMaster, RecAcc);
            if (tableMaster == null)
            {
                clearACMast();
                return;
            }
            if (tableMaster == null)
            {
                clearACMast();
                return;
            }
            if (tableMaster.Rows.Count < 1)
            {
                clearACMast();
                return;
            }
            if (!tableMaster.Columns.Contains("accNo"))
            {
                clearACMast();
                return;
            }
            DataRow[] drs = tableMaster.Select("accNo='" + this.CurrentID + "'"); //.Rows[0];
            if (drs.Length < 1)
            {
                clearACMast();
                return;
            }
            if (this.CurrentID != null)
            {
                if (this.CurrentID.Trim() != "")
                {                    
                   // SetMdITitle(distributeMsg, mdiChild, "Position Summary - " + this.CurrentID.ToString());
                    SetFormTitle(1, distributeMsg, mdiChild, new string[] { this.CurrentID.ToString() });
                }
            }
        }

        public void clearACMast()
        {          
           // SetMdITitle(distributeMsg, mdiChild, "Position Summary ");
            SetFormTitle();
        }

        #region position data receive
        delegate void PosInfoChange(ObservableCollection<OrderPosition> posCL, PosChangeType sType, string _Acc);
        PosInfoChange PosDataChange;
        protected void PosBus_DataChangeInvoke(object sender, ObservableCollection<OrderPosition> posCL, PosChangeType sType, string _Acc)
        {
            Application.Current.Dispatcher.BeginInvoke(PosDataChange, new Object[] { posCL, sType, _Acc });
        }

        public void PositionBusDataChange(ObservableCollection<OrderPosition> posCL, PosChangeType sType, string _Acc)
        {           
            //this.posCollection = posCL;
            //this.dgPosition.ItemsSource = this.posCollection;

            viewSource.Source = this.posCollection;          
            this.dgPosition.ItemsSource = viewSource.View;
        }
        #endregion

        public override void ClearControlData()
        {
            base.ClearControlData();
            if (mdiChild != null)
            {               
                // SetMdITitle(distributeMsg, mdiChild,  "Position Summary ");
                 SetFormTitle();
            }
        }

        public override void IDelearUser_OnUserChange(string _CurUserID)
        {
            //清空旧的数据         
           // posCollection.Clear();
           
        }

        private void IDelearUser_Unloaded_1(object sender, RoutedEventArgs e)
        {  
            if (PosBus != null)
            {
                PosBus.UnRegisterOnPosInfoChange(new PositionBus.PosInfoChange(PosBus_DataChangeInvoke));
            }
            base.IDelearUser_Unloaded_1(sender, e);
        }

        private void resultDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (GOSTradeStation.customizeData.AlertData.isDoubleClick == false) return;

            if (sender != null)
            {
                DataGridRow dgr = sender as DataGridRow;
                if (dgr == null) return;
                OrderPosition orderPosition = dgr.Item as OrderPosition;
                if (orderPosition == null) return;
                MarketPriceItem mpi = new MarketPriceItem();
                mpi.ProductCode = orderPosition.ProductCode;
                mpi.Last = orderPosition.MktPrice;
                mpi.Ask = orderPosition.Ask.ToString();
                mpi.Bid = orderPosition.Bid.ToString();
                //mpi.Last 
                if (distributeMsg != null)
                {
                    distributeMsg.DistributeControl(orderPosition.ProductCode );
                    distributeMsg.DistributeControlOrder(mpi, "Last");
                }
                if(mdiChild!=null)
                mdiChild.Focus();
            }

            // if (sender != null)
            //{
            //    DataGridRow dgr = sender as DataGridRow;
            //    if (dgr != null)
            //    {
            //        OrderPosition orderPosition = dgr.Item as OrderPosition;
            //        if (distributeMsg != null)
            //        {
            //            distributeMsg.DistributeControl(orderPosition.ProductCode );
            //            DataRow[] drs=GOSTradeStation .marketPriceData .MarketListTable.Select("productCode='"+orderPosition.ProductCode+"'");
            //            if (drs.Count() > 0)
            //            {
            //                DataRowView drv = GOSTradeStation.marketPriceData.MarketListTable.DefaultView[GOSTradeStation.marketPriceData.MarketListTable.Rows.IndexOf(drs[0])];
            //                distributeMsg.DistributeControlOrder(drv);
            //            }
            //            else
            //            {
            //                DataTable dt = GOSTradeStation.marketPriceData.MarketListTable;
            //                DataRow dr = dt.NewRow();
            //                dr["ProductCode"] = orderPosition.ProductCode;
            //                dt.Rows.Add(dr);
            //                DataRowView drv = GOSTradeStation.marketPriceData.MarketListTable.DefaultView[GOSTradeStation.marketPriceData.MarketListTable.Rows.IndexOf(dr)];
            //                distributeMsg.DistributeControlOrder(drv);
                     
            //            }
            //        }
            //    }
            //}
        }

        private void dgPosition_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            List<string> ls = new List<string>();
            MarketPriceItem mpi = new MarketPriceItem();

            IList<DataGridCellInfo> selectedcells = e.AddedCells;
            foreach (DataGridCellInfo dci in selectedcells)
            {
                OrderPosition orderPosition = dci.Item as OrderPosition;
                if (orderPosition == null) return;
                mpi.ProductCode = orderPosition.ProductCode;
                mpi.Last = orderPosition.MktPrice;
                mpi.Ask = orderPosition.Ask.ToString();
                mpi.Bid = orderPosition.Bid.ToString();
            }

            if (GOSTradeStation.customizeData.AlertData.isSingleClick == false) return;

            if (distributeMsg != null && mpi != null )
            {
                distributeMsg.DistributeControl(mpi.ProductCode);
                distributeMsg.DistributeControlOrder(mpi, "Last");
            }
        }
       
        //harry 2013-11-12
        private void dgPosition_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (this.dgPosition.SelectedIndex == -1) return;
            if (RL == "R")
            {
                return;
            }
            List<string> ls = new List<string>();          
            object o = this.dgPosition.SelectedItem;
            OrderPosition orderPosition = o as OrderPosition;
            if (orderPosition == null) return;
            if (orderPosition.ProductCode == null) return;
            if (orderPosition.ProductCode.Trim() == "") return;
            MarketPriceItem mpi = new MarketPriceItem();

            mpi.ProductCode = orderPosition.ProductCode;
            mpi.Last = orderPosition.MktPrice;
            mpi.Ask = orderPosition.Ask.ToString();
            mpi.Bid = orderPosition.Bid.ToString();

            if (GOSTradeStation.customizeData.AlertData.isSingleClick == false) return;

            if (distributeMsg != null && mpi != null)
            {
                distributeMsg.DistributeControl(mpi.ProductCode);
                distributeMsg.DistributeControlOrder(mpi,"Last");
            }
            if (mdiChild != null)
                mdiChild.Focus();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            Visibility v = Visibility.Visible;
            if (item.IsChecked == true)
            {
                v = Visibility.Collapsed;
            }

            if (dgPosition.Columns[1] != null)
            {
                dgPosition.Columns[1].Visibility =v;
            }
            if (dgPosition.Columns[2] != null)
            {
                dgPosition.Columns[2].Visibility = v;
            }
            if (dgPosition.Columns[3] != null)
            {
                dgPosition.Columns[3].Visibility = v;
            }
        }

        CollectionViewSource viewSource = new CollectionViewSource();
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;         
            bool bFiler=false;
            if (item.IsChecked )
            {
               bFiler=true;
            }

          
          //  ICollectionView view = viewSource.View;
                //CollectionViewSource.GetDefaultView(this.posCollection);
            if (viewSource.View.CanFilter)
            {
                if (bFiler)
                {
                    //view.CanFilter = true;               
                    viewSource.View.Filter = delegate(object o)
                    {
                        OrderPosition pos = o as OrderPosition;
                        if (pos == null) return false;
                        if (pos.Net == null) return false;
                        if (pos.Net.Trim() == "0") return false;
                        if (pos.Net.Trim() == "")
                            return false;
                        return true;
                    };
                }
                else
                {
                    viewSource.View.Filter = null;
                }
            }
        }

        int iTitleType = 0;
        public virtual void SetFormTitle(int itype, MessageDistribute _distributeMsg, WPF.MDI.MdiChild _mdiChild, params string[] Params)
        {
            string str = "";
            iTitleType = itype;
            switch (itype)
            {
                case 0:
                    str = this.strTitleFormat.Split('|')[0];
                    SetMdITitle(distributeMsg, mdiChild, str);
                    break;
                case 1:
                    str = string.Format(this.strTitleFormat, Utility.getArrayItemValue(Params,0));
                    str = str.Replace("|", "");
                    SetMdITitle(distributeMsg, mdiChild, str);
                    break;
            }
        }

        public override void SetFormTitle()
        {          
            string _CurID = "";
            if (this.CurrentID != null)
            {
                if (this.CurrentID.Trim() != "")
                {
                    _CurID = this.CurrentID.ToString().Trim();
                }
            }
            SetFormTitle(iTitleType, distributeMsg, mdiChild, new string[] { _CurID });
        }
    }
}
