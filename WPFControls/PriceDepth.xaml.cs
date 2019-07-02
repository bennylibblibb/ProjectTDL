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
using System.Windows.Controls.Primitives;

namespace GOSTS.WPFControls
{
    /// <summary>
    /// PriceDepth.xaml 的交互逻辑
    /// </summary>
    public partial class PriceDepth : UserControl
    {
        private MessageDistribute distributeMsg = null;
        public string ProdCode { get; set; }
        public bool Locked { get; set; }
        public bool isVertical { get; set; }
        public MdiChild mdiChild { get; set; }
        delegate void BindListItem(DataTable dataTable);
        BindListItem PriceDepthDelegate;
        PriceDataViewModel viewModel;
        LinearGradientBrush askBrush;
        LinearGradientBrush buyBrush;

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

        string verticalView
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("VerticalView");
            }
        }

        string horizontalView
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("HorizontalView");
            }
        }

        public PriceDepth(MessageDistribute _msgDistribute)
        {
            InitializeComponent();
            if (_msgDistribute != null)
            {
                distributeMsg = _msgDistribute;
                distributeMsg.DisPMPriceDepth += new MessageDistribute.OnDisPMPriceDepth(distributeMsg_MPPriceDepthtable);
                distributeMsg.DisControl += new MessageDistribute.OnDisControl(distributeMsg_PriceDepth);
                distributeMsg.DisGotProductList += new MessageDistribute.OnDisGotProductList(distributeMsg_DisDisGotProductList);
                PriceDepthDelegate = new BindListItem(BindListItemMethod);
            }

            viewModel = new PriceDataViewModel();
            this.lvPriceDepth.ItemsSource = viewModel.DataList;
            this.dgPriceDepth.ItemsSource = viewModel.DataList2;

            askBrush = new LinearGradientBrush();
            askBrush.StartPoint = new Point(0.5, 0);
            askBrush.EndPoint = new Point(0.5, 1);
            askBrush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
            askBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFF3DCF3"), 0.042));
            askBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFE299D8"), 0.925));

            buyBrush = new LinearGradientBrush();
            buyBrush.StartPoint = new Point(0.5, 0);
            buyBrush.EndPoint = new Point(0.5, 1);
            buyBrush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
            buyBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFD7F6F9"), 0.042));
            buyBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF43CBC2"), 0.925));


        }

        protected void distributeMsg_DisDisGotProductList(object sender)
        {
            if (ProdCode == "") return;
            TradeStationSend.Send(null, ProdCode, cmdClient.registerPriceDepth);
            Application.Current.Dispatcher.BeginInvoke(this.PriceDepthDelegate, new Object[] { null });
        }

        protected void distributeMsg_PriceDepth(object sender, string prodCode)
        {
            if (prodCode != this.ProdCode)
            {
                string defaultTitle = this.mdiChild.Title;
                if (Locked == false)
                {
                    TradeStationDesktop.UpdateXmlAttribute(WindowTypes.PriceDepth, this.ProdCode, prodCode);

                    this.mdiChild.Title = TradeStationSetting.ReturnWindowName(WindowTypes.PriceDepth, prodCode);

                    viewModel.DataList.Clear();
                    viewModel.DataList2.Clear();

                    TradeStationSend.Send(ProdCode, prodCode, cmdClient.registerPriceDepth);
                    ProdCode = prodCode;

                    distributeMsg.DistributeControlFocus(defaultTitle, this.mdiChild);
                }
            }
            else
            {
                distributeMsg.DistributeControlFocus(this.mdiChild.Title, this.mdiChild);
            }
        }


        protected void distributeMsg_MPPriceDepthtable(object sender, DataTable dataTable)
        {
            if (dataTable == null) return;
            Application.Current.Dispatcher.BeginInvoke(this.PriceDepthDelegate, new Object[] { dataTable });
        }

        public void BindListItemMethod(DataTable dataTable)
        {
            if (dataTable == null)
            {
                string defaultTitle = this.mdiChild.Title;
                this.mdiChild.Title = (Locked == true) ? TradeStationSetting.ReturnWindowName(WindowTypes.PriceDepth, this.ProdCode) + " * " : TradeStationSetting.ReturnWindowName(WindowTypes.PriceDepth, this.ProdCode);
                distributeMsg.DistributeControlFocus(defaultTitle, this.mdiChild);
                return;
            }
            ObservableCollection<PriceDepthData> DataList;
            ObservableCollection<LongPriceDepthData> DataList2;
            viewModel.GetPriceDepthData(ProdCode, dataTable, out  DataList, out  DataList2);
            // string defaultTitle = this.mdiChild.Title;

            if (DataList.Count != 0 && DataList[0].ProdCode == this.ProdCode)
            {

                //this.mdiChild.Title = (Locked == true) ? TradeStationSetting.ReturnWindowName(WindowTypes.PriceDepth, this.ProdCode) + " * " : TradeStationSetting.ReturnWindowName(WindowTypes.PriceDepth, this.ProdCode);

                if (viewModel.DataList.Count == 0)
                {
                    foreach (PriceDepthData item in DataList)
                    {
                        viewModel.DataList.Add(item);
                    }
                }
                else
                {
                    foreach (PriceDepthData item in DataList)
                    {
                        var ls = viewModel.DataList.FirstOrDefault(x => (x.Item == item.Item));
                        if (ls != null)
                        {
                            ls.Item = item.Item;
                            ls.B1 = item.B1;
                            ls.B2 = item.B2;
                            ls.B3 = item.B3;
                            ls.B4 = item.B4;
                            ls.B5 = item.B5;
                            ls.A1 = item.A1;
                            ls.A2 = item.A2;
                            ls.A3 = item.A3;
                            ls.A4 = item.A4;
                            ls.A5 = item.A5;
                        }
                    }
                }
            }
            if (DataList2.Count != 0 && DataList2[0].ProdCode == this.ProdCode)
            {
                // this.mdiChild.Title = (Locked == true) ? TradeStationSetting.ReturnWindowName(WindowTypes.PriceDepth, this.ProdCode) + " * " : TradeStationSetting.ReturnWindowName(WindowTypes.PriceDepth, this.ProdCode);

                if (viewModel.DataList2.Count == 0)
                {
                    foreach (LongPriceDepthData item in DataList2)
                    {
                        viewModel.DataList2.Add(item);
                    }
                }
                else
                {
                    foreach (LongPriceDepthData item in DataList2)
                    {
                        var ls = viewModel.DataList2.FirstOrDefault(x => (x.ID == item.ID));
                        if (ls != null)
                        {
                            ls.Bid = item.Bid;
                            ls.BQty = item.BQty;
                            ls.Ask = item.Ask;
                            ls.AQty = item.AQty;
                        }
                    }
                }
            }

            //distributeMsg.DistributeControlFocus(defaultTitle, this.mdiChild);
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
                    child.Title = this.mdiChild.Title.Replace(" * ", "") + " * ";
                }
                distributeMsg.DistributeControlFocus(defaultTitle, this.mdiChild);
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
                distributeMsg.DistributeControlFocus(defaultTitle, this.mdiChild);
            }
            else if ((string)menuItem.Tag == verticalView)
            {
                this.drDG.Height = this.drLV.Height;
                this.drLV.Height = new GridLength(0);
                menuItem.Header = horizontalView;
                menuItem.Tag = horizontalView;
                this.isVertical = true;
                for (int i = 0; i < dgPriceDepth.Items.Count; i++)
                {
                    for (int j = 0; j < dgPriceDepth.Columns.Count; j++)
                    {
                        if (j < 2)
                        {
                            GetCell(dgPriceDepth, i, j).Background = buyBrush;
                        }
                        else
                        {
                            GetCell(dgPriceDepth, i, j).Background = askBrush;
                        }
                    }
                }
            }
            else if ((string)menuItem.Tag == horizontalView)
            {
                this.drLV.Height = this.drDG.Height;
                this.drDG.Height = new GridLength(0);
                menuItem.Header = verticalView;
                menuItem.Tag = verticalView;
                this.isVertical = false;

                for (int i = 0; i < lvPriceDepth.Items.Count; i++)
                {
                    for (int j = 0; j < lvPriceDepth.Columns.Count; j++)
                    {
                        if (j == 0)
                        {
                            GetCell(lvPriceDepth, i, j).Background = Brushes.White;
                        }
                        else if (j < 6)
                        {
                            GetCell(lvPriceDepth, i, j).Background = buyBrush;
                        }
                        else
                        {
                            GetCell(lvPriceDepth, i, j).Background = askBrush;
                        }
                    }
                }

            }
            //TradeStationDesktop.UpdateXmlData(WindowTypes.PriceDepth , this.ProdCode, "Locked", (this.Locked) ? "1" : "0");
            TradeStationDesktop.UpdateXmlData(WindowTypes.PriceDepth, this.ProdCode, "Locked", (this.Locked) ? "1" : "0");
            TradeStationDesktop.UpdateXmlData(WindowTypes.PriceDepth, this.ProdCode, "ArrayMode", (this.isVertical) ? "Vertical" : "Horizontal");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.mdiChild.Width = this.mdiChild.ActualWidth;
            this.mdiChild.Height = this.mdiChild.ActualHeight;

            if (isVertical)
            {
                this.drLV.Height = new GridLength(0);
                this.drDG.Height = GridLength.Auto;
            }
            else
            {
                this.drLV.Height = GridLength.Auto;
                this.drDG.Height = new GridLength(0);
            }

            MenuItem menuItem = this.FindName("miLocked") as MenuItem;
            menuItem = this.FindName("miVertical") as MenuItem;

            menuItem.Header = (this.isVertical) ? horizontalView : verticalView;
            menuItem.Tag = (this.isVertical) ? horizontalView : verticalView;

            menuItem = this.FindName("miLocked") as MenuItem;
            if (this.ProdCode != null)
            {
                menuItem.Header = (this.Locked) ? codeUnLocked : codeLocked;
                menuItem.Tag = (this.Locked) ? codeUnLocked : codeLocked;

                if (GOSTradeStation.m_BeforeGet == false || ProdCode == null || ProdCode == "") return;
                TradeStationSend.Send(null, ProdCode, cmdClient.registerPriceDepth);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            distributeMsg.DisPMPriceDepth -= new MessageDistribute.OnDisPMPriceDepth(distributeMsg_MPPriceDepthtable);
            distributeMsg.DisControl -= new MessageDistribute.OnDisControl(distributeMsg_PriceDepth);
            distributeMsg.DisGotProductList -= new MessageDistribute.OnDisGotProductList(distributeMsg_DisDisGotProductList);

            if ((!GOSTradeStation.IsWindowInitialized) || ProdCode == null || ProdCode == "") return;
            TradeStationSend.Send(ProdCode, null, cmdClient.registerPriceDepth);
        }

        private void PriceDepthPrice_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (GOSTradeStation.customizeData.AlertData.isDoubleClick == false) return;
                if (sender != null)
                {
                    DataGridCell cell = sender as DataGridCell;
                    float qty = 0;
                    float price = 0;
                    bool AOStatus = false;

                    if (isVertical != true)
                    {
                        for (int i = 0; i < lvPriceDepth.Items.Count; i++)
                        {
                            for (int j = 0; j < lvPriceDepth.Columns.Count; j++)
                            {
                                if (j == 0)
                                {
                                    GetCell(lvPriceDepth, i, j).Background = Brushes.White;
                                }
                                else if (j < 6)
                                {
                                    GetCell(lvPriceDepth, i, j).Background = buyBrush;
                                }
                                else
                                {
                                    GetCell(lvPriceDepth, i, j).Background = askBrush;
                                }
                            }
                        }

                        if (((PriceDepthData)cell.DataContext).Item == "Prc")
                        {
                            if (((TextBlock)cell.Content).Text == GOSTS.GosCulture.CultureHelper.GetString("PDPrice")) return;

                            cell.Background = Brushes.LightGreen;

                            if (((TextBlock)cell.Content).Text == "AO")
                            {
                                distributeMsg.DistributeControl(this.ProdCode, -1, 1,true);
                            }
                            else if (((TextBlock)cell.Content).Text == "")
                            {
                                distributeMsg.DistributeControl(this.ProdCode, 0, 1,false);
                            }
                            else
                            {
                                distributeMsg.DistributeControl(this.ProdCode, (float)Convert.ToDouble(((TextBlock)cell.Content).Text), 1,false);
                            }
                        }
                        else
                        {
                            if (((TextBlock)cell.Content).Text == GOSTS.GosCulture.CultureHelper.GetString("PDQty")) return;

                            cell.Background = Brushes.LightGreen;

                            PriceDepthData data = cell.DataContext as PriceDepthData;
                            PriceDepthData data2 = lvPriceDepth.Items[0] as PriceDepthData;
                            
                            switch (cell.Column.Header.ToString())
                            {
                                case "B5":
                                    qty=(data.B1 == AppFlag.InvalidNum || data.B2 == AppFlag.InvalidNum || data.B3 == AppFlag.InvalidNum || data.B4 == AppFlag.InvalidNum || data.B5 == AppFlag.InvalidNum) ? 0 : data.B5 + data.B4 + data.B3 + data.B2 + data.B1;
                                    price = (data2.B5 == AppFlag.InvalidNum) ? 0 : data2.B5;
                                    break;
                                case "B4":
                                    qty = (data.B1 == AppFlag.InvalidNum || data.B2 == AppFlag.InvalidNum || data.B3 == AppFlag.InvalidNum || data.B4 == AppFlag.InvalidNum) ? 0 : data.B4 + data.B3 + data.B2 + data.B1;
                                    price = (data2.B4 == AppFlag.InvalidNum) ? 0 : data2.B4;
                                    break;
                                case "B3":
                                    qty = (data.B1 == AppFlag.InvalidNum || data.B2 == AppFlag.InvalidNum || data.B3 == AppFlag.InvalidNum) ? 0 : data.B3 + data.B2 + data.B1;
                                    price = (data2.B3 == AppFlag.InvalidNum) ? 0 : data2.B3;
                                    break;
                                case "B2":
                                    qty = (data.B1 == AppFlag.InvalidNum || data.B2 == AppFlag.InvalidNum) ? 0 : data.B2 + data.B1;
                                    price = (data2.B2 == AppFlag.InvalidNum) ? 0 : data2.B2;
                                    break;
                                case "B1":
                                    qty = (data.B1 == AppFlag.InvalidNum) ? 0 : data.B1;
                                    price = (data2.B1 == AppFlag.InvalidNum) ? 0 : data2.B1;
                                    if (price == AppFlag.AONum) { AOStatus =true ; }
                                    break;
                                case "A5":
                                    qty = (data.A5 == AppFlag.InvalidNum || data.A4 == AppFlag.InvalidNum || data.A3 == AppFlag.InvalidNum || data.A2 == AppFlag.InvalidNum || data.A1 == AppFlag.InvalidNum) ? 0 : data.A5 + data.A4 + data.A3 + data.A2 + data.A1;
                                    price = (data2.A5 == AppFlag.InvalidNum) ? 0 : data2.A5;
                                    break;
                                case "A4":
                                    qty =( data.A4 == AppFlag.InvalidNum || data.A3 == AppFlag.InvalidNum || data.A2 == AppFlag.InvalidNum || data.A1 == AppFlag.InvalidNum) ? 0 : data.A4 + data.A3 + data.A2 + data.A1;
                                    price = (data2.A4 == AppFlag.InvalidNum) ? 0 : data2.A4;
                                    break;
                                case "A3":
                                    qty = ( data.A3 == AppFlag.InvalidNum || data.A2 == AppFlag.InvalidNum || data.A1 == AppFlag.InvalidNum) ? 0 : data.A3 + data.A2 + data.A1;
                                    price = (data2.A3 == AppFlag.InvalidNum) ? 0 : data2.A3;
                                    break;
                                case "A2":
                                    qty =  (data.A2 == AppFlag.InvalidNum || data.A1 == AppFlag.InvalidNum) ? 0 :data.A2 + data.A1;
                                    price = (data2.A2 == AppFlag.InvalidNum) ? 0 : data2.A2;
                                    break;
                                case "A1":
                                    qty =  (data.A1 == AppFlag.InvalidNum) ? 0 :data.A1;
                                    price = (data2.A1 == AppFlag.InvalidNum) ? 0 : data2.A1;
                                    if (price == AppFlag.AONum) { AOStatus = true ; }
                                    break;
                            }

                            distributeMsg.DistributeControl(this.ProdCode, price, Convert.ToInt32(qty), AOStatus);
                        }

                        this.lvPriceDepth.UnselectAllCells();
                    }
                    else
                    {
                        for (int i = 0; i < dgPriceDepth.Items.Count; i++)
                        {
                            for (int j = 0; j < dgPriceDepth.Columns.Count; j++)
                            {
                                if (j < 2)
                                {
                                    GetCell(dgPriceDepth, i, j).Background = buyBrush;
                                }
                                else
                                {
                                    GetCell(dgPriceDepth, i, j).Background = askBrush;
                                }
                            }
                        }

                        cell.Background = Brushes.LightGreen;

                        if (cell.Column.Header.ToString() == GOSTS.GosCulture.CultureHelper.GetString("MkBid") || cell.Column.Header.ToString() == GOSTS.GosCulture.CultureHelper.GetString("MkAsk"))
                        {
                            if (((TextBlock)cell.Content).Text == "AO")
                            {
                                distributeMsg.DistributeControl(this.ProdCode, -1, 1,true );
                            }
                            else if (((TextBlock)cell.Content).Text == "")
                            {
                                distributeMsg.DistributeControl(this.ProdCode, 0, 1,false);
                            }
                            else
                            {
                                distributeMsg.DistributeControl(this.ProdCode, (float)Convert.ToDouble(((TextBlock)cell.Content).Text), 1,false );
                            }
                        }
                        else
                        {
                            string col = cell.Column.Header.ToString();
                            LongPriceDepthData data = (LongPriceDepthData)cell.DataContext;
                            if (col == GOSTS.GosCulture.CultureHelper.GetString("MkBQty"))
                            {
                                price = data.Bid;
                                if (price == AppFlag.AONum) { AOStatus = true; }
                                for (int i = 0; i < data.ID + 1; i++)
                                {
                                    qty += ((LongPriceDepthData)dgPriceDepth.Items[i]).BQty;
                                }
                            }
                            else
                            {
                                price = data.Ask;
                                if (price == AppFlag.AONum) { AOStatus = true; }
                                for (int i = 0; i < data.ID + 1; i++)
                                {
                                    qty += ((LongPriceDepthData)dgPriceDepth.Items[i]).AQty;
                                }
                            }

                            distributeMsg.DistributeControl(this.ProdCode, price, Convert.ToInt32(qty), AOStatus);
                        }

                        this.dgPriceDepth.UnselectAllCells();
                    }
                }
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " PriceDepthPrice_MouseDoubleClick,error:" + exp.ToString() + "\r\n");
            }
        }

        private void PriceDepth_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (GOSTradeStation.customizeData.AlertData.isSingleClick == false) return;

                if (e.AddedCells == null || e.AddedCells.Count == 0) return;

                float qty = 0;
                float price = 0;
                bool AOStatus = false;
                if (isVertical != true)
                {
                    for (int i = 0; i < lvPriceDepth.Items.Count; i++)
                    {
                        for (int j = 0; j < lvPriceDepth.Columns.Count; j++)
                        {
                            if (j == 0)
                            {
                                GetCell(lvPriceDepth, i, j).Background = Brushes.White;
                            }
                            else if (j < 6)
                            {
                                GetCell(lvPriceDepth, i, j).Background = buyBrush;
                            }
                            else
                            {
                                GetCell(lvPriceDepth, i, j).Background = askBrush;
                            }
                        }
                    }

                    DataGridCell cell = GetCell(e.AddedCells[0]);
                    cell.Background = Brushes.LightGreen;

                    if (((PriceDepthData)cell.DataContext).Item == "Prc")
                    {
                        if (((TextBlock)cell.Content).Text == GOSTS.GosCulture.CultureHelper.GetString("PDPrice")) return;
                        if (((TextBlock)cell.Content).Text == "AO")
                        {
                            distributeMsg.DistributeControl(this.ProdCode, -1, 1,true );
                        }
                        else if (((TextBlock)cell.Content).Text == "")
                        {
                            distributeMsg.DistributeControl(this.ProdCode, 0, 1,false );
                        }
                        else
                        {
                            distributeMsg.DistributeControl(this.ProdCode, (float)Convert.ToDouble(((TextBlock)cell.Content).Text), 1,false );
                        }
                    }
                    else
                    {

                        if (((TextBlock)cell.Content).Text == GOSTS.GosCulture.CultureHelper.GetString("PDQty")) return;
                        PriceDepthData data = cell.DataContext as PriceDepthData;
                        PriceDepthData data2 = lvPriceDepth.Items[0] as PriceDepthData;
                        
                        switch (cell.Column.Header.ToString())
                        {
                            case "B5":
                                qty = (data.B1 == AppFlag.InvalidNum || data.B2 == AppFlag.InvalidNum || data.B3 == AppFlag.InvalidNum || data.B4 == AppFlag.InvalidNum || data.B5 == AppFlag.InvalidNum) ? 0 : data.B5 + data.B4 + data.B3 + data.B2 + data.B1;
                                price = (data2.B5 == AppFlag.InvalidNum) ? 0 : data2.B5;
                                break;
                            case "B4":
                                qty = (data.B1 == AppFlag.InvalidNum || data.B2 == AppFlag.InvalidNum || data.B3 == AppFlag.InvalidNum || data.B4 == AppFlag.InvalidNum) ? 0 : data.B4 + data.B3 + data.B2 + data.B1;
                                price = (data2.B4 == AppFlag.InvalidNum) ? 0 : data2.B4;
                                break;
                            case "B3":
                                qty = (data.B1 == AppFlag.InvalidNum || data.B2 == AppFlag.InvalidNum || data.B3 == AppFlag.InvalidNum) ? 0 : data.B3 + data.B2 + data.B1;
                                price = (data2.B3 == AppFlag.InvalidNum) ? 0 : data2.B3;
                                break;
                            case "B2":
                                qty = (data.B1 == AppFlag.InvalidNum || data.B2 == AppFlag.InvalidNum) ? 0 : data.B2 + data.B1;
                                price = (data2.B2 == AppFlag.InvalidNum) ? 0 : data2.B2;
                                break;
                            case "B1":
                                qty = (data.B1 == AppFlag.InvalidNum) ? 0 : data.B1;
                                price = (data2.B1 == AppFlag.InvalidNum) ? 0 : data2.B1;
                                if (price == AppFlag.AONum) { AOStatus = true ; }
                                break;
                            case "A5":
                                qty = (data.A5 == AppFlag.InvalidNum || data.A4 == AppFlag.InvalidNum || data.A3 == AppFlag.InvalidNum || data.A2 == AppFlag.InvalidNum || data.A1 == AppFlag.InvalidNum) ? 0 : data.A5 + data.A4 + data.A3 + data.A2 + data.A1;
                                price = (data2.A5 == AppFlag.InvalidNum) ? 0 : data2.A5;
                                break;
                            case "A4":
                                qty = (data.A4 == AppFlag.InvalidNum || data.A3 == AppFlag.InvalidNum || data.A2 == AppFlag.InvalidNum || data.A1 == AppFlag.InvalidNum) ? 0 : data.A4 + data.A3 + data.A2 + data.A1;
                                price = (data2.A4 == AppFlag.InvalidNum) ? 0 : data2.A4;
                                break;
                            case "A3":
                                qty = (data.A3 == AppFlag.InvalidNum || data.A2 == AppFlag.InvalidNum || data.A1 == AppFlag.InvalidNum) ? 0 : data.A3 + data.A2 + data.A1;
                                price = (data2.A3 == AppFlag.InvalidNum) ? 0 : data2.A3;
                                break;
                            case "A2":
                                qty = (data.A2 == AppFlag.InvalidNum || data.A1 == AppFlag.InvalidNum) ? 0 : data.A2 + data.A1;
                                price = (data2.A2 == AppFlag.InvalidNum) ? 0 : data2.A2;
                                break;
                            case "A1":
                                qty = (data.A1 == AppFlag.InvalidNum) ? 0 : data.A1;
                                price = (data2.A1 == AppFlag.InvalidNum) ? 0 : data2.A1;
                                if (price == AppFlag.AONum) { AOStatus = true ; }
                                break;
                        }

                        distributeMsg.DistributeControl(this.ProdCode, price, Convert.ToInt32(qty), AOStatus);
                    }
                }
                else
                {
                    for (int i = 0; i < dgPriceDepth.Items.Count; i++)
                    {
                        for (int j = 0; j < dgPriceDepth.Columns.Count; j++)
                        {
                            if (j < 2)
                            {
                                GetCell(dgPriceDepth, i, j).Background = buyBrush;
                            }
                            else
                            {
                                GetCell(dgPriceDepth, i, j).Background = askBrush;
                            }
                        }
                    }

                    DataGridCell cell = GetCell(e.AddedCells[0]);
                    cell.Background = Brushes.LightGreen;

                    if (cell.Column.Header.ToString() == GOSTS.GosCulture.CultureHelper.GetString("MkBid") || cell.Column.Header.ToString() == GOSTS.GosCulture.CultureHelper.GetString("MkAsk"))
                    {
                        if (((TextBlock)cell.Content).Text == "AO")
                        {
                            distributeMsg.DistributeControl(this.ProdCode, -1, 1,true );
                        }
                        else if (((TextBlock)cell.Content).Text == "")
                        {
                            distributeMsg.DistributeControl(this.ProdCode, 0, 1,false );
                        }
                        else
                        {
                            distributeMsg.DistributeControl(this.ProdCode, (float)Convert.ToDouble(((TextBlock)cell.Content).Text), 1,false );
                        }
                    }
                    else
                    {
                        string col = cell.Column.Header.ToString();
                        LongPriceDepthData data = (LongPriceDepthData)cell.DataContext;
                        if (col == GOSTS.GosCulture.CultureHelper.GetString("MkBQty"))
                        {
                            price = data.Bid;
                            if (price == AppFlag.AONum) { AOStatus = true; }
                            for (int i = 0; i < data.ID + 1; i++)
                            {
                                qty += ((LongPriceDepthData)dgPriceDepth.Items[i]).BQty;
                            }
                        }
                        else
                        {
                            price = data.Ask;
                            if (price == AppFlag.AONum) { AOStatus = true; }
                            for (int i = 0; i < data.ID + 1; i++)
                            {
                                qty += ((LongPriceDepthData)dgPriceDepth.Items[i]).AQty;
                            }
                        }

                        distributeMsg.DistributeControl(this.ProdCode, price, Convert.ToInt32(qty),AOStatus  );
                    }
                }
                this.dgPriceDepth.UnselectAllCells();
                this.lvPriceDepth.UnselectAllCells();
            }
            catch (Exception exp)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " PriceDepth_SelectedCellsChanged,error:" + exp.ToString() + "\r\n");
            }
        }

        public static DataGridCell GetCell(DataGridCellInfo dataGridCellInfo)
        {
            if (!dataGridCellInfo.IsValid)
            {
                return null;
            }
            var cellContent = dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item);
            if (cellContent != null)
            {
                return (DataGridCell)cellContent.Parent;
            }
            else
            {
                return null;
            }
        }

        public DataGridCell GetCell(DataGrid dataGrid, int row, int column)
        {
            DataGridRow rowContainer = GetRow(dataGrid, row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                // try to get the cell but it may possibly be virtualized
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        public DataGridRow GetRow(DataGrid dataGrid, int index)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // may be virtualized, bring into view and try again
                dataGrid.ScrollIntoView(dataGrid.Items[index]);
                row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        private void ContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = (ContextMenu)e.Source;
            if (contextMenu == null) return;
            ((MenuItem)contextMenu.Items[0]).Header = (this.Locked) ? codeUnLocked : codeLocked;
            ((MenuItem)contextMenu.Items[0]).Tag = (this.Locked) ? codeUnLocked : codeLocked;
            ((MenuItem)contextMenu.Items[2]).Header = (this.isVertical) ? horizontalView : verticalView;
            ((MenuItem)contextMenu.Items[2]).Tag = (this.isVertical) ? horizontalView : verticalView; 
        }
    }
}
