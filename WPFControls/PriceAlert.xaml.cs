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
using System.Collections.ObjectModel;
using System.Data;

namespace GOSTS.WPFControls
{
    /// <summary>
    /// PriceAlert.xaml 的交互逻辑
    /// </summary>
    public partial class PriceAlert : UserControl
    {

        public MdiChild mdiChild { get; set; }
        private MessageDistribute msgDistribute;
        private ObservableCollection<PriceAlertData> priceAlertDatas { get; set; }
        private ObservableCollection<PriceAlertData> AlertedpriceAlertDatas { get; set; }
        private bool m_Popup; 

        public PriceAlert(MessageDistribute _msgDistribute)
        {
            InitializeComponent();
            if (_msgDistribute != null)
            {
                msgDistribute = _msgDistribute;
                msgDistribute.SyncAlert += new MessageDistribute.OnSyncAlert(distributeMsg_SyncAlert); 
                msgDistribute.DisMarketPrice += new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);
            }
        }

        protected void distributeMsg_DisMarketPrice(object sender, ObservableCollection<MarketPriceItem> data)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var updateData = from c in data
                                 where priceAlertDatas.Select(x => x.ProductCode).ToList().Contains(c.ProductCode)
                                 select c;

                if (updateData == null ) return;
                if (priceAlertDatas == null)
                {
                    return;
                }
                foreach (PriceAlertData item in priceAlertDatas)
                {
                    bool alerted = true;

                    var ls = updateData.FirstOrDefault(x => (x.ProductCode == item.ProductCode));
                    if (ls == null) return;
                    item.Market = (item.AlertType == "Last") ? ls.Last : (item.AlertType == "Bid") ? ls.Bid : (item.AlertType == "Ask") ? ls.Ask : "";
                    if (item.Market == "") continue;
                    if (item.ArrivedType != "Above" &&
                        (TradeStationTools.ConvertToInt(item.Market) >= TradeStationTools.ConvertToInt(item.Above))
                        && item.Status == "0" && item.Above != "")
                    {
                        if (item.ArrivedType == "")
                        {
                            item.ArrivedType = "Above";
                            if (item.ArrivedType == "Above" && item.Below == "")
                            {
                                item.Status = "2";
                            }
                            alerted = false;
                        }
                        else if (item.ArrivedType == "Below")
                        {
                            item.ArrivedType = "Above";
                            item.Status = "2";
                            alerted = false;
                        }
                    }
                    else if (item.ArrivedType != "Below"
                        && (TradeStationTools.ConvertToInt(item.Market) <= TradeStationTools.ConvertToInt(item.Below))
                        && item.Status == "0" && item.Below != "")
                    {
                        if (item.ArrivedType == "")
                        {
                            item.ArrivedType = "Below";
                            if (item.ArrivedType == "Below" && item.Above == "")
                            {
                                item.Status = "2";
                            }
                            alerted = false;
                        }
                        else if (item.ArrivedType == "Above")
                        {
                            item.ArrivedType = "Below";
                            item.Status = "2";
                            alerted = false;
                        }
                    }

                    if (!alerted)
                    {
                        alerted = true;

                        TradeStationDesktop.UpdateTrigerAlerts(item); 
                        AlertedpriceAlertDatas.Add(new PriceAlertData()
                        {
                            No = (AlertedpriceAlertDatas.Count + 1).ToString(),
                            ProductCode = item.ProductCode,
                            ProductName = item.ProductName,
                            Above = item.Above,
                            Action = item.Action,
                            Below = item.Below,
                            AlertedTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm fff"),
                            AlertType = item.AlertType,
                            ArrivedType = item.ArrivedType,
                            Status = item.Status,
                            Market = item.Market,
                        });

                        MdiChild mcAlert = new MdiChild();
                        AlertPriceMsg alertPriceMsg = new AlertPriceMsg(msgDistribute);
                        alertPriceMsg.priceAlertData = item;
                        alertPriceMsg.mdiChild = mcAlert;
                        mcAlert.Title = item.ProductCode + "- Price Alert";
                        mcAlert.Content = alertPriceMsg;
                        mcAlert.Width = alertPriceMsg.Width;
                        mcAlert.Height = alertPriceMsg.Height;

                        //if (!mcAlert.ExistInContainer(mcAlert, GOSTradeStation.container))
                        //{
                            GOSTradeStation.container.Children.Add(mcAlert);
                        //}
                    }
                }
            }));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            priceAlertDatas = TradeStationDesktop.ReturnPriceAlerts(false);
            this.dgAlerts.ItemsSource = priceAlertDatas;
            AlertedpriceAlertDatas = TradeStationDesktop.ReturnPriceAlerts(true);
            this.dgAlerted.ItemsSource = AlertedpriceAlertDatas;

            TradeStationSend.Get(priceAlertDatas.Select(x => x.ProductCode).ToList<string>(), cmdClient.getMarketPrice);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            msgDistribute.DisMarketPrice -= new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);
            msgDistribute.SyncAlert -= new MessageDistribute.OnSyncAlert(distributeMsg_SyncAlert);
        }

        protected void distributeMsg_SyncAlert(object sender, HandleType type, PriceAlertData data)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            { 
                if (priceAlertDatas == null) return;

                if (type == HandleType.Update )
                foreach (PriceAlertData item in priceAlertDatas)
                {
                    if (item.No == data.No)
                    { 
                        item.Status = data.Status ;
                        item.AlertType = data.AlertType;
                        item.Below = data.Below;
                        item.Above = data.Above;
                        item.Action = data.Action; 
                        break;
                    }
                }
                else if (type == HandleType.Add)
                {
                    data.ProductName = MarketPriceData.GetProductName(data.ProductCode);
                    priceAlertDatas.Add(data);
                }
                else if (type == HandleType.Remove )
                {
                    foreach (PriceAlertData item in priceAlertDatas)
                    {
                        if (item.No == data.No)
                        {
                            priceAlertDatas.Remove(item);
                            break;
                        }
                    }
                }

                TradeStationSend.Get( new List<string>{ data .ProductCode} , cmdClient.getMarketPrice);
     
            }));
        } 

        private void cbHandle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.dgAlerts.SelectedItem == null) return;

            if (((ComboBox)e.Source).SelectedIndex == 0)
            {
                if (((PriceAlertData)this.dgAlerts.SelectedValue).Status == "2")
                {
                    //((PriceAlertData)this.dgAlerts.SelectedValue).Status = "0";
                    LoadPriceAlertDetails(); 
                } 
                else
                {
                    ((PriceAlertData)this.dgAlerts.SelectedValue).Status = "0";
                    TradeStationDesktop.UpdatePriceAlert((PriceAlertData)this.dgAlerts.SelectedValue); 
                }
            }
            else if (((ComboBox)e.Source).SelectedIndex == 1)
            {
                ((PriceAlertData)this.dgAlerts.SelectedValue).Status = "1";
                TradeStationDesktop.UpdatePriceAlert((PriceAlertData)this.dgAlerts.SelectedValue);

            }
            else if (((ComboBox)e.Source).SelectedIndex == 2)
            {
                ((PriceAlertData)this.dgAlerts.SelectedValue).Status = "2";
                TradeStationDesktop.UpdatePriceAlert((PriceAlertData)this.dgAlerts.SelectedValue); 
            }
            else if (((ComboBox)e.Source).SelectedIndex == 3)
            {
                if (System.Windows.Forms.MessageBox.Show("Are You Sure To Delete?",
                            "Delete Alert", System.Windows.Forms.MessageBoxButtons.YesNo,
                            System.Windows.Forms.MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                {
                    TradeStationDesktop.DeletePriceAlert("Alert", ((PriceAlertData)this.dgAlerts.SelectedValue).No);
                    priceAlertDatas.Remove((PriceAlertData)this.dgAlerts.SelectedValue);
                    int no=0; 
                    foreach (PriceAlertData item in priceAlertDatas)
                    {
                        no++;
                        item.No = no.ToString();
                    }
                }
                else
                {
                    m_Popup = true;
                    ((ComboBox)e.Source).SelectedIndex = Convert.ToInt32(((PriceAlertData)this.dgAlerts.SelectedValue).Status);
                }
            }
        }

        private void LoadPriceAlertDetails()
        {
            if (m_Popup)
            {   
            }
            else
            {
                PriceAlertData data = ((PriceAlertData)this.dgAlerts.SelectedValue);

                MdiChild mcAddPriceAlert = new MdiChild();
                AddPriceAlert addPriceAlert = new AddPriceAlert(GOSTradeStation.distributeMsg);
                addPriceAlert.mdiChild = mcAddPriceAlert;
                addPriceAlert.ProdCode = data.ProductCode;
                addPriceAlert.No = data.No;
                addPriceAlert.priceAlertData = data;

                if (data.AlertType == "Last") addPriceAlert.rbLast.IsChecked = true;
                else if (data.AlertType == "Ask") addPriceAlert.rbAsk.IsChecked = true;
                else if (data.AlertType == "Bid") addPriceAlert.rbBid.IsChecked = true; 
                addPriceAlert.tbID.Text = data.ProductCode ;
                addPriceAlert.lbName.Content = MarketPriceData.GetProductName(data.ProductCode);
                addPriceAlert.tbAbove.Text = data.Above;
                addPriceAlert.tbBelow.Text = data.Below;

                mcAddPriceAlert.Title = data.ProductCode + "- Price Alert Settings";
                mcAddPriceAlert.Content = addPriceAlert;
                mcAddPriceAlert.Width = addPriceAlert.Width;
                mcAddPriceAlert.Height = addPriceAlert.Height;
                mcAddPriceAlert.Position = new System.Windows.Point(this.mdiChild.Position.X, (this.mdiChild.Position.Y + Mouse.GetPosition(this.mdiChild).Y + 20));

                if (!mcAddPriceAlert.ExistInContainer(mcAddPriceAlert, GOSTradeStation.container))
                {
                    GOSTradeStation.container.Children.Add(mcAddPriceAlert);
                } 
            }  

            m_Popup = false; 
        }

        private void dgAlerts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.dgAlerts.SelectedItem != null)
            {
                LoadPriceAlertDetails();
            }
        }

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.OriginalSource;

            switch ((string)menuItem.Tag)
            {
                case "Delete":
                    PriceAlertData pad = this.dgAlerted.SelectedItem as PriceAlertData;
                    if (pad == null) return;
                    TradeStationDesktop.DeletePriceAlert("History", pad.No);
                    AlertedpriceAlertDatas.Remove(pad);
                    int no=0;
                    foreach (PriceAlertData item in AlertedpriceAlertDatas)
                    {
                        no++;
                        item.No = no.ToString();
                    }
                    break;
                case "DeleteAll":
                    ObservableCollection<PriceAlertData> data = (ObservableCollection<PriceAlertData>)dgAlerted.ItemsSource;
                    if (data == null) return;
                    foreach (PriceAlertData item in data)
                    {
                        TradeStationDesktop.DeletePriceAlert("History", item.No);
                    }
                    AlertedpriceAlertDatas.Clear();
                    break;
            }
        }
    }
}
