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
using System.Windows .Data ;

namespace GOSTS.Dealer
{
    /// <summary>
    /// MarginCheck.xaml
    /// </summary>
    public partial class MarginCheck : UserControl
    {
        private MessageDistribute distributeMsg = null;
        delegate void BindData(ObservableCollection<MarginCheckAccData> data);
        BindData InitDelegate;

        public MdiChild mdiChild { get; set; }
        ObservableCollection<MarginCheckAccData> MarginCheckDataes;
        MarginCheckViewModel marginCheckViewModel;

        public MarginCheck(MessageDistribute _msgDistribute)
        {
            InitializeComponent();

            if (_msgDistribute != null)
            {
                distributeMsg = _msgDistribute;
                distributeMsg.DisMarginCheck += new MessageDistribute.OnDisMarginCheck(distributeMsg_MarginCheck);
                InitDelegate = new BindData(BindListItemMethod);
            }
            marginCheckViewModel = new MarginCheckViewModel();
            this.DataContext = marginCheckViewModel;
        }

        protected void distributeMsg_MarginCheck(object sender, ObservableCollection<MarginCheckAccData> data)
        {
            Application.Current.Dispatcher.BeginInvoke(this.InitDelegate, new Object[] { data });
        }

        public void BindListItemMethod(ObservableCollection<MarginCheckAccData> data)
        {
            if (data == null) return;
            MarginCheckDataes = data;
            marginCheckViewModel.MarginCheckViewModels(data);
            if (data!=null&&data.Count > 0)
            {
                this.mdiChild.Title = TradeStationSetting.ReturnWindowName(WindowTypes.MarginCheck, data[0].AE);
             }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TradeStationSend.Send(cmdClient.getMarginCheck); 
        }

        private void dgAE_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string AE = "";
            if (sender != null)
            {
                DataGridRow dgr = sender as DataGridRow;
                if (dgr != null)
                {
                    MarginCheckAEData drv = dgr.Item as MarginCheckAEData;
                    AE = (AE == null) ? "" : drv.AE.ToString();
                } 

                ObservableCollection<MarginCheckAccData> data = new ObservableCollection<MarginCheckAccData>();
                foreach (MarginCheckAccData dr in MarginCheckDataes)
                {
                    if (dr.AE == AE)
                    {
                        data.Add(dr);
                    }
                }

                marginCheckViewModel.MarginCheckAccDataes = data;
                this.mdiChild.Title = TradeStationSetting.ReturnWindowName(WindowTypes.MarginCheck , AE); 
            }
        }

        private void resultDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGridRow dgr = sender as DataGridRow;
                if (dgr != null)
                {
                    MarginCheckAccData marginCheckAccData = dgr.Item as MarginCheckAccData;
                    IDelearStatus.ChangeSelectedAcc(this, marginCheckAccData.AccNo);
                }
            }
        }
    }
}
