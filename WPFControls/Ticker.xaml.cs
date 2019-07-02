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
using System.Collections.ObjectModel;

namespace GOSTS.WPFControls
{
    /// <summary>
    /// Ticker.xaml 的交互逻辑
    /// </summary>
    public partial class Ticker : UserControl
    {
        private MessageDistribute distributeMsg = null;
        public MdiChild mdiChild { get; set; }
        public string ProdCode { get; set; }
        public bool Locked { get; set; }

        delegate void BindTickerItem(ObservableCollection<TickerData> TickerDataes);
        BindTickerItem TickerDataDelegate;
        TickerViewModel viewModel;

        string codeUnLocked
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("CodeUnLocked");
            }
        }

        string codeLocked
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("CodeLocked");
            }
        }

        public Ticker(MessageDistribute _msgDistribute)
        {
            InitializeComponent();
            if (_msgDistribute != null)
            {
                distributeMsg = _msgDistribute;
                distributeMsg.DisPMTickerItemData += new MessageDistribute.OnDisPMTickerItemData(distributeMsg_MPTickerData);
                distributeMsg.DisControl += new MessageDistribute.OnDisControl(distributeMsg_Ticker);
                distributeMsg.DisGotProductList += new MessageDistribute.OnDisGotProductList(distributeMsg_DisDisGotProductList);
                TickerDataDelegate = new BindTickerItem(BindTickerDataMethod);

                viewModel = new TickerViewModel();
                viewModel.TickerList.Reverse();
                this.dgTicker.ItemsSource = viewModel.TickerList;
            }
        }

        protected void distributeMsg_DisDisGotProductList(object sender)
        {
            if (ProdCode == "") return;
            TradeStationSend.Send(null, ProdCode, cmdClient.registerTicker);
            Application.Current.Dispatcher.BeginInvoke(this.TickerDataDelegate, new Object[] { null });
        }

        protected void distributeMsg_Ticker(object sender, string prodCode)
        {
            if (prodCode != this.ProdCode)
            {
                string defaultTitle = this.mdiChild.Title;
                if (Locked == false)
                {
                    TradeStationDesktop.UpdateXmlAttribute(WindowTypes.Ticker, this.ProdCode, prodCode);

                    this.mdiChild.Title = TradeStationSetting.ReturnWindowName(WindowTypes.Ticker, prodCode);

                    viewModel.TickerList.Clear();

                    TradeStationSend.Send(ProdCode, prodCode, cmdClient.registerTicker);
                    ProdCode = prodCode;

                    distributeMsg.DistributeControlFocus(defaultTitle, this.mdiChild);
                }
            }
            else
            {
                distributeMsg.DistributeControlFocus(this.mdiChild.Title, this.mdiChild);
            }

        }

        protected void distributeMsg_MPTickerData(object sender, ObservableCollection<TickerData> TickerDataes)
        {
            if (TickerDataes == null) return;
            Application.Current.Dispatcher.BeginInvoke(this.TickerDataDelegate, new Object[] { TickerDataes });
        }

        public void BindTickerDataMethod(ObservableCollection<TickerData> TickerDataes)
        {
            if (TickerDataes == null)
            {
                string defaultTitle = this.mdiChild.Title;
                this.mdiChild.Title = (Locked == true) ? TradeStationSetting.ReturnWindowName(WindowTypes.Ticker, this.ProdCode) + " * " : TradeStationSetting.ReturnWindowName(WindowTypes.Ticker, this.ProdCode);
                distributeMsg.DistributeControlFocus(defaultTitle, this.mdiChild);
                return;
            }
            // IEnumerable<TickerData> data = TickerDataes.TakeWhile(c => c.productCode == this.ProdCode);
            // string defaultTitle = this.mdiChild.Title;
            var data = from c in TickerDataes
                       where c.productCode == this.ProdCode
                       select c;

            if (data.Count() > 0)
            {
                bool arrivedMax = false;
                foreach (TickerData item in data)
                {
                    viewModel.TickerList.Insert(0, item);

                    if (viewModel.TickerList.Count > AppFlag.MaximaTicker)
                    {
                        viewModel.TickerList.RemoveAt(viewModel.TickerList.Count - 1);
                        arrivedMax = true;
                    }
                }
                if (arrivedMax == true)
                {
                    for (int i = AppFlag.MaximaTicker - 1; i > AppFlag.MaximaTicker / 2 - 1; i--)
                    {
                        viewModel.TickerList.RemoveAt(i);
                    }

                    //int i = 0;
                    //foreach (TickerData item in viewModel.TickerList)
                    //{
                    //    if (i >= AppFlag.MaximaTicker / 2)
                    //    {
                    //        viewModel.TickerList.Remove(item);
                    //    }
                    //    i++;
                    //}

                }
            }

            //  distributeMsg.DistributeControlFocus(defaultTitle, this.mdiChild);
        }

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            MdiChild child = this.mdiChild as MdiChild;
            MenuItem menuItem = (MenuItem)e.OriginalSource;
            string defaultTitle = this.mdiChild.Title;
            if ((string)menuItem.Tag == codeLocked)
            {
                this.Locked = true;
                menuItem.Header = (this.Locked) ? codeUnLocked : codeLocked;
                menuItem.Tag = (this.Locked) ? codeUnLocked : codeLocked;
                if (child != null)
                {
                    child.Title = this.mdiChild.Title + " * ";
                }
            }
            else if ((string)menuItem.Tag == codeUnLocked)
            {
                this.Locked = false;
                menuItem.Header = (this.Locked) ? codeUnLocked : codeLocked;
                menuItem.Tag = (this.Locked) ? codeUnLocked : codeLocked;
                if (child != null)
                {
                    child.Title = this.mdiChild.Title.Replace(" * ", "");
                }
            }

            TradeStationDesktop.UpdateXmlData(WindowTypes.Ticker, this.ProdCode, "Locked", (this.Locked) ? "1" : "0");
            distributeMsg.DistributeControlFocus(defaultTitle, this.mdiChild);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.mdiChild.Width = this.mdiChild.ActualWidth;
            this.mdiChild.Height = this.mdiChild.ActualHeight;

            MenuItem miLocked = this.FindName("miLocked") as MenuItem;
            if (this.ProdCode != null)
            {
                miLocked.Header = (this.Locked) ? codeUnLocked : codeLocked;
                miLocked.Tag = (this.Locked) ? codeUnLocked : codeLocked;

                if (GOSTradeStation.m_BeforeGet == false || ProdCode == null || ProdCode == "") return;
                TradeStationSend.Send(null, ProdCode, cmdClient.registerTicker);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            distributeMsg.DisPMTickerItemData -= new MessageDistribute.OnDisPMTickerItemData(distributeMsg_MPTickerData);
            distributeMsg.DisControl -= new MessageDistribute.OnDisControl(distributeMsg_Ticker);
            distributeMsg.DisGotProductList -= new MessageDistribute.OnDisGotProductList(distributeMsg_DisDisGotProductList);

            if ((!GOSTradeStation.IsWindowInitialized) || ProdCode == null || ProdCode == "") return;
            TradeStationSend.Send(ProdCode, null, cmdClient.registerTicker);
        }

        private void ContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = (ContextMenu)e.Source;
            if (contextMenu == null) return;
            ((MenuItem)contextMenu.Items[0]).Header = (this.Locked) ? codeUnLocked : codeLocked;
            ((MenuItem)contextMenu.Items[0]).Tag = (this.Locked) ? codeUnLocked : codeLocked;
        }
    }
}
