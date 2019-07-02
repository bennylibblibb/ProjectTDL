using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using GOSTS.Behavior;
using System.Windows.Interactivity;
using GOSTS.Common ;


namespace GOSTS.WPFControls
{
    public partial class TreeMarketPriceControl : UserControl
    { 
        AdornerLayer mAdornerLayer = null; 
       //TreeViewItem treeViewItem = null;

        public TreeMarketPriceControl()
        {
            InitializeComponent();
            this.treeMarketCode.PreviewMouseMove += OnPreviewTreeViewMouseMove;
            treeMarketCode.QueryContinueDrag += OnQueryContinueDrag;
            this.PreviewMouseDown += PreviewMouseButtonDown;
            this.PreviewMouseUp += PreviewMouseButtonUp;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.treeMarketCode.PreviewMouseMove -= OnPreviewTreeViewMouseMove;
            treeMarketCode.QueryContinueDrag -= OnQueryContinueDrag;
            this.PreviewMouseDown -= PreviewMouseButtonDown;
            this.PreviewMouseUp -= PreviewMouseButtonUp;
        }

        private void OnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (mAdornerLayer != null)
            {
                mAdornerLayer.Update();
            }
        }

        private void OnPreviewTreeViewMouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(treeMarketCode);
            HitTestResult result = VisualTreeHelper.HitTest(treeMarketCode, pos);
            if (result == null)
                return;

            TreeViewItem treeViewItem = Utils.FindVisualParent<TreeViewItem>(result.VisualHit); // Find your actual visual you want to drag
          
            if (treeViewItem == null || !treeViewItem.IsSelected || (!treeViewItem.IsExpanded && treeViewItem.Items.Count > 0))
                return; 

            List<string> lsProd = new List<string>();
            if (treeViewItem.DataContext is ViewModel.ProdViewModel)
            {
                ViewModel.ProdViewModel prod = treeViewItem.DataContext as ViewModel.ProdViewModel;
                if (prod != null)
                {
                    //   lsProd.Add(prod.ProdCode+ "," + prod.ProdName );
                    lsProd.Add(prod.ProdCode);
                }
            }
            else if (treeViewItem.DataContext is ViewModel.ProdTypeViewModel)
            {
                ViewModel.ProdTypeViewModel prodType = treeViewItem.DataContext as ViewModel.ProdTypeViewModel;
                if (prodType != null)
                {
                    for (int i = 0; i < prodType.Children.Count; i++)
                    {
                        ViewModel.ProdViewModel prod = prodType.Children[i] as ViewModel.ProdViewModel;
                        if (prod != null)
                        {
                            //   lsProd.Add(prod.ProdCode+ "," + prod.ProdName );
                            lsProd.Add(prod.ProdCode);
                        }
                    }
                }
            }
            else if (treeViewItem.DataContext is ViewModel.ProdsViewModel)
            {
                ViewModel.ProdsViewModel prod = treeViewItem.DataContext as ViewModel.ProdsViewModel;
                if (prod != null)
                {
                    //   lsProd.Add(prod.ProdCode+ "," + prod.ProdName );
                    lsProd.Add(prod.ProdCode);
                }
            }
            else if (treeViewItem.DataContext is ViewModel.ProdOptionTypeViewModel)
            {
                ViewModel.ProdOptionTypeViewModel prodOption = treeViewItem.DataContext as ViewModel.ProdOptionTypeViewModel;
                if (prodOption != null)
                {
                    for (int i = 0; i < prodOption.Children.Count; i++)
                    {
                        ViewModel.ProdsViewModel prod = prodOption.Children[i] as ViewModel.ProdsViewModel;
                        if (prod != null)
                        {
                            lsProd.Add(prod.ProdCode);
                        }
                    }
                }
            }

            //DragDropAdorner adorner = new DragDropAdorner(treeViewItem); 

            //TreeViewItem tvi = new TreeViewItem();
            //for (int i = 0; i < treeViewItem.Items.Count; i++)
            //{
            //    tvi.Items.Add(treeViewItem.Items[i]);
            //} 
            DragDropAdorner adorner = new DragDropAdorner(treeViewItem);
            mAdornerLayer = AdornerLayer.GetAdornerLayer(treeMarketCode); // Window class do not have AdornerLayer
            mAdornerLayer.Add(adorner);

            if (treeViewItem.IsSelected && treeViewItem.Items.Count > 0)
            {
                treeViewItem.IsSelected = false;
                treeViewItem.Focus();
            }

            System.Windows.DragDrop.DoDragDrop(treeViewItem, lsProd, DragDropEffects.Copy);

            if (mAdornerLayer != null)
            {
                mAdornerLayer.Remove(adorner);
                mAdornerLayer = null;
            }
             
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }

        void PreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        void PreviewMouseButtonUp(object sender, MouseButtonEventArgs e)
        { 
        }

        private void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {

        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {

        }

        private void treeMarketCode_Loaded(object sender, RoutedEventArgs e)
        {
            if (GOSTradeStation.marketPriceData == null || GOSTradeStation.marketPriceData.ProdListTable == null || GOSTradeStation.marketPriceData.InstListTable == null)
            {
                TradeStationSend.Send(cmdClient.getInstrumentList);
                TradeStationSend.Send(cmdClient.getProductList);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (GOSTradeStation.marketPriceData == null || GOSTradeStation.marketPriceData.ProdListTable == null || GOSTradeStation.marketPriceData.InstListTable == null)
            {
                TradeStationSend.Send(cmdClient.getInstrumentList);
                TradeStationSend.Send(cmdClient.getProductList);
            }
        }
    }
}