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
    /// AlertPriceMsg.xaml 的交互逻辑
    /// </summary>
    public partial class AlertPriceMsg : UserControl
    {
        public MdiChild mdiChild { get; set; }
        public PriceAlertData priceAlertData;
        private MessageDistribute msgDistribute;

        public AlertPriceMsg(MessageDistribute _msgDistribute)
        {
            InitializeComponent();
            if (_msgDistribute != null)
            {
                msgDistribute = _msgDistribute;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        { 
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DisplayAlert(this.priceAlertData);
         } 

        private void DisplayAlert(PriceAlertData priceAlertData)
        {
            if (priceAlertData == null) return;

            if (TradeStationTools.ConvertToInt(priceAlertData.Market) >= TradeStationTools.ConvertToInt(priceAlertData.Above))
            {
                this.lbAlert.Content = "Id : " + priceAlertData.ProductCode + "   Name : " + MarketPriceData.GetProductName(priceAlertData.ProductCode) + "\r\n" + "\r\n" + priceAlertData.AlertType + " : " +
                 priceAlertData.Market + "\r\nAbove : " + priceAlertData.Above;
            }
            else if (TradeStationTools.ConvertToInt(priceAlertData.Market) <= TradeStationTools.ConvertToInt(priceAlertData.Below))
            {
                this.lbAlert.Content = "Id : " + priceAlertData.ProductCode + "   Name : " + MarketPriceData.GetProductName(priceAlertData.ProductCode) + "\r\n" + "\r\n" + priceAlertData.AlertType + " : " +
                     priceAlertData.Market + "\r\nBelow : " + priceAlertData.Below;
            }
        }
    }
}
