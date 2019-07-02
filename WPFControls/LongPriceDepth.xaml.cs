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

namespace GOSTS.WPFControls
{
    /// <summary>
    /// PriceDepth.xaml 的交互逻辑
    /// </summary>
    public partial class LongPriceDepth : UserControl
    {
        private MessageDistribute distributeMsg = null;
        public bool Locked { get; set; }
        public string ProdCode { get; set; }
        public MdiChild mdiChild { get; set; }
        delegate void BindListItem(List<LongPriceDepthData> data);
        BindListItem LongPriceDepthDelegate;

        public LongPriceDepth(MessageDistribute _msgDistribute)
        {
            InitializeComponent();
            if (_msgDistribute != null)
            {
                distributeMsg = _msgDistribute;
                distributeMsg.DisPMLongPriceDepth += new MessageDistribute.OnDisPMLongPriceDepth(distributeMsg_MPLongPriceDepth);
                distributeMsg.DisControl += new MessageDistribute.OnDisControl(distributeMsg_LongPriceDepth);
                LongPriceDepthDelegate = new BindListItem(BindListItemMethod);
            }
        }

        protected void distributeMsg_LongPriceDepth(object sender, string prodCode)
        { 
            if (prodCode != this.ProdCode)
            {
                string defaultTitle = this.mdiChild.Title;
                if (Locked == false)
                {
                    TradeStationDesktop.UpdateXmlAttribute(WindowTypes.LongPriceDepth, this.ProdCode, prodCode);
           
                    this.mdiChild.Title = TradeStationSetting.ReturnWindowName(WindowTypes.LongPriceDepth, prodCode);
                 
                    dgLongPriceDepth.ItemsSource = null;

                    TradeStationSend.Send(ProdCode, prodCode, cmdClient.registerLongPriceDepth);
                    ProdCode = prodCode;

                    distributeMsg.DistributeControlFocus(defaultTitle, this.mdiChild);
                } 
            }
            else
            {
                distributeMsg.DistributeControlFocus(this.mdiChild.Title, this.mdiChild);
            } 

            //if (prodCode == this.ProdCode)
            //{
            //    distributeMsg.DistributeControlFocus(this.mdiChild.Title, this.mdiChild);
            //}
            //else
            //{
            //    if (Locked == false)
            //    {
            //        List<string> listProd = new List<string>();
            //        listProd.Add(prodCode);

            //        string defaultTitle = this.mdiChild.Title;
            //        this.mdiChild.Title = prodCode + "- " + WindowTypes.LongPriceDepth;

            //        dgLongPriceDepth.ItemsSource = null;

            //        if (ProdCode != prodCode)  //Unregister ths same 
            //        {
            //            List<string> UnlistProd = new List<string>();
            //            if (ProdCode != null)
            //            {
            //                UnlistProd.Add(ProdCode);
            //            }
            //            TradeStationSend.Send(UnlistProd, listProd, cmdClient.registerLongPriceDepth);
            //            ProdCode = prodCode;
            //        }

            //        distributeMsg.DistributeControlFocus(defaultTitle, this.mdiChild);
            //    }
            //} 
        }

        protected void distributeMsg_MPLongPriceDepth(object sender, List<LongPriceDepthData> data)
        {
            Application.Current.Dispatcher.BeginInvoke(this.LongPriceDepthDelegate, new Object[] { data });
        }

        public void BindListItemMethod(List<LongPriceDepthData> data)
        {
               
            if (data != null)
            {
                List<string> list = data.Select(d => d.ProdCode).Distinct().ToList();
                if (list.Contains(ProdCode))
                {
                    this.mdiChild.Title = (Locked == true) ? TradeStationSetting.ReturnWindowName(WindowTypes.LongPriceDepth, ProdCode) + " * " : TradeStationSetting.ReturnWindowName(WindowTypes.LongPriceDepth, ProdCode);
           
                    this.dgLongPriceDepth.ItemsSource   = (from filter in data
                                                         where filter.ProdCode.Equals(ProdCode)
                                                         select filter).ToList();
                }
            }
        }

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            MdiChild child = this.mdiChild as MdiChild;
            MenuItem menuItem = (MenuItem)e.OriginalSource;
            string defaultTitle = this.mdiChild.Title;
            switch ((string)menuItem.Tag)
            {
                case "Lock ID":
                    this.Locked = true;
                    menuItem.Header = (this.Locked) ? "Unlock ID" : "Lock ID";
                    menuItem.Tag = (this.Locked) ? "Unlock ID" : "Lock ID";
                    if (child != null)
                    {
                        child.Title = this.mdiChild.Title + " * ";
                    } 
                    break;
                case "Unlock ID":
                    this.Locked = false;
                    menuItem.Header = (this.Locked) ? "Unlock ID" : "Lock ID";
                    menuItem.Tag = (this.Locked) ? "Unlock ID" : "Lock ID";
                    if (child != null)
                    {
                        child.Title = this.mdiChild.Title.Replace(" * ", "");
                    } 
                    break;
            }

            //TradeStationDesktop.UpdateXmlData(WindowTypes.LongPriceDepth, this.ProdCode, "Locked", (this.Locked) ? "1" : "0");
         
            distributeMsg.DistributeControlFocus(defaultTitle, this.mdiChild);
        }

        private void LongPriceDepth_Loaded(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = this.FindName("miLocked") as MenuItem;
            if (this.ProdCode != null)
            {
                menuItem.Header = (this.Locked) ? "Unlock ID" : "Lock ID";
                menuItem.Tag = (this.Locked) ? "Unlock ID" : "Lock ID";
           
                List<string> listProd = new List<string>();
                listProd.Add(ProdCode);
                TradeStationSend.Send(null, listProd, cmdClient.registerLongPriceDepth);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //if (this.ProdCode != null)
            //{
            //    List<string> UnlistProd = new List<string>();
            //    UnlistProd.Add(ProdCode);
            //    TradeStationSend.Send(UnlistProd, null, cmdClient.registerLongPriceDepth);
            //} 
            distributeMsg.DisPMLongPriceDepth -= new MessageDistribute.OnDisPMLongPriceDepth(distributeMsg_MPLongPriceDepth);
            distributeMsg.DisControl -= new MessageDistribute.OnDisControl(distributeMsg_LongPriceDepth);

            if (!GOSTradeStation.IsWindowInitialized) return;
            TradeStationSend.Send(ProdCode, null, cmdClient.registerLongPriceDepth);
        }
    }
}