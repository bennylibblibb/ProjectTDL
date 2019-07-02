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

namespace GOSTS.WPFControls
{
    /// <summary>
    /// AddPriceAlert.xaml 的交互逻辑
    /// </summary>
    public partial class AddPriceAlert : UserControl
    {
        public MdiChild mdiChild { get; set; }
        private MessageDistribute msgDistribute;
        public string ProdCode { get; set; }
        public string No { get; set; }
        public PriceAlertData priceAlertData { get; set; }
        private bool m_AboveInitialized;
        private bool m_BelowInitialized;

        public AddPriceAlert(MessageDistribute _msgDistribute )
        {
            InitializeComponent();
            if (_msgDistribute != null)
            {
                msgDistribute = _msgDistribute;
                msgDistribute.DisMarketPrice += new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);
            }
               this.rbLast.IsChecked = true;
        }
         
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ProdCode == null || ProdCode == "") return; 
            TradeStationSend.Send(null, this.ProdCode, cmdClient.registerMarketPrice);  
            this.tbAbove.SetProdCode(this.ProdCode); 
            this.tbBelow.SetProdCode(this.ProdCode);
          }

        protected void distributeMsg_DisMarketPrice(object sender, ObservableCollection<MarketPriceItem> data)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MarketPriceItem mpi = (from c in data
                                 where  this.ProdCode ==c.ProductCode
                                 select c).FirstOrDefault ();

                if (mpi == null) return;

                this.lbLast.Content = mpi.Last;
                this.lbBid.Content = mpi.Bid;
                this.lbAsk.Content = mpi.Ask ;

            }));
        }

        private void Above_Click(object sender, RoutedEventArgs e)
        {
            if (m_AboveInitialized) return;
            this.tbAbove.Text = (rbLast.IsChecked == true && tbAbove.Text.Trim() == "") ? this.lbLast.Content.ToString() : (rbBid.IsChecked == true && tbAbove.Text.Trim() == "") ? this.lbBid.Content.ToString() : (rbAsk.IsChecked == true && tbAbove.Text.Trim() == "") ? this.lbAsk.Content.ToString() : this.tbAbove.Text;
            m_AboveInitialized = true;
        }

        private void Below_Click(object sender, RoutedEventArgs e)
        {
            if (m_BelowInitialized) return;
            this.tbBelow.Text = (rbLast.IsChecked == true && tbBelow.Text.Trim() == "") ? this.lbLast.Content.ToString() : (rbBid.IsChecked == true && tbBelow.Text.Trim() == "") ? this.lbBid.Content.ToString() : (rbAsk.IsChecked == true && tbBelow.Text.Trim() == "") ? this.lbAsk.Content.ToString() : this.tbBelow.Text;
            m_BelowInitialized = true;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            m_BelowInitialized = false;
            m_AboveInitialized = false;
            this.tbBelow.Text = "";
            this.tbAbove.Text = "";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (tbBelow.Text == "" && tbAbove.Text == "" && priceAlertData != null)
            {
                if (System.Windows.Forms.MessageBox.Show("Are You Sure To Delete?",
                          "Delete Alert", System.Windows.Forms.MessageBoxButtons.YesNo,
                          System.Windows.Forms.MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                {
                    TradeStationDesktop.DeletePriceAlert("Alert", priceAlertData.No);

                    this.mdiChild.Close();
                    msgDistribute.DisSyncAlert(HandleType.Remove,priceAlertData);
                }
                return;
            }
            if (rbLast.IsChecked == true)
            {
                if (TradeStationTools.ConvertToInt(lbLast.Content) != 0 && tbAbove.Text!="" && TradeStationTools.ConvertToInt(tbAbove.Text) <= TradeStationTools.ConvertToInt(lbLast.Content))
                {
                    this.tbAbove.txtAmount.Foreground = Brushes.Red;
                    return;
                }
                if (TradeStationTools.ConvertToInt(lbLast.Content) != 0 && tbBelow.Text != "" && TradeStationTools.ConvertToInt(tbBelow.Text) >= TradeStationTools.ConvertToInt(lbLast.Content))
                {
                    this.tbBelow.txtAmount.Foreground = Brushes.Red;
                    return;
                }
            }
            else if (rbBid.IsChecked == true)
            {
                if (TradeStationTools.ConvertToInt(lbBid.Content) != 0 && TradeStationTools.ConvertToInt(tbAbove.Text) <= TradeStationTools.ConvertToInt(lbBid.Content))
                {
                    this.tbAbove.txtAmount.Foreground = Brushes.Red;
                    return;
                }
                else if (TradeStationTools.ConvertToInt(lbBid.Content) != 0 && TradeStationTools.ConvertToInt(tbBelow.Text) >= TradeStationTools.ConvertToInt(lbBid.Content))
                {
                    this.tbBelow.txtAmount.Foreground = Brushes.Red;
                    return;
                }
            }
            else if (rbAsk.IsChecked == true)
            {
                if (TradeStationTools.ConvertToInt(lbAsk.Content) != 0 && TradeStationTools.ConvertToInt(tbAbove.Text) <= TradeStationTools.ConvertToInt(lbAsk.Content))
                {
                    this.tbAbove.txtAmount.Foreground = Brushes.Red;
                    return;
                }
                else if (TradeStationTools.ConvertToInt(lbAsk.Content) != 0 && TradeStationTools.ConvertToInt(tbBelow.Text) >= TradeStationTools.ConvertToInt(lbAsk.Content))
                {
                    this.tbBelow.txtAmount.Foreground = Brushes.Red;
                    return;
                }
            }

            if (priceAlertData != null)
            {
                priceAlertData.AlertType = (rbLast.IsChecked == true) ? "Last" : (rbBid.IsChecked == true) ? "Bid" : (rbAsk.IsChecked == true) ? "Ask" : "";
                priceAlertData.Above = tbAbove.Text;
                priceAlertData.Below = tbBelow.Text;
                priceAlertData.Action = "Popup";
                priceAlertData.Status = "0";
                priceAlertData.ArrivedType = "";
                priceAlertData.AlertedTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                
                if (TradeStationDesktop.UpdatePriceAlert(priceAlertData))
                { 
                    msgDistribute.DisSyncAlert(HandleType .Update,priceAlertData);
                }
            }
            else
            {
                PriceAlertData data = new PriceAlertData();
                data.ProductCode = this.ProdCode;
                data.AlertType = (rbLast.IsChecked == true) ? "Last" : (rbBid.IsChecked == true) ? "Bid" : (rbAsk.IsChecked == true) ? "Ask" : "";
                data.Market = "";
                data.Above = tbAbove.Text;
                data.Below = tbBelow.Text;
                data.Action = "Popup";
                data.Status = "0"; 
                data.ArrivedType = "";
                data.AlertedTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                int iNo = TradeStationDesktop.AddPriceAlerts(data);
                if (iNo > 0)
                {
                    data.No = iNo.ToString(); 
                    msgDistribute.DisSyncAlert(HandleType.Add,data);
                }

                MdiChild mcPriceAlert = new MdiChild();
                mcPriceAlert.Title = "Price Alert";
                mcPriceAlert.Width = 500;// priceAlert.Width;
                mcPriceAlert.Height = 300; //priceAlert.Height;
                mcPriceAlert.Position = new System.Windows.Point(Mouse.GetPosition(this).X, Mouse.GetPosition(this).Y);

                if (!mcPriceAlert.ExistInContainer(mcPriceAlert, GOSTradeStation.container))
                {
                    PriceAlert priceAlert = new PriceAlert(msgDistribute);
                    priceAlert.mdiChild = mcPriceAlert;
                    mcPriceAlert.Content = priceAlert;
                    GOSTradeStation.container.Children.Add(mcPriceAlert);
                }
            }

            this.mdiChild.Close();
        }
         
        private void btnCannel_Click(object sender, RoutedEventArgs e)
        {
            if (priceAlertData != null)
            {
                msgDistribute.DisSyncAlert(HandleType.Update, priceAlertData);
            }
            this.mdiChild.Close();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //TradeStationSend.Send(this.ProdCode, null, cmdClient.registerMarketPrice);  
            msgDistribute.DisMarketPrice -= new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);
         } 
    }
}
