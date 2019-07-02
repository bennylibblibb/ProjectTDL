using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GOSTS.Common;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Timers;
using System.Windows;

namespace GOSTS.WPFControls.Chart.SCI
{
    class SCIChartAnalysisMarketPrice : IDisposable
    {
        private delegate void MarketPriceReceivedAsyncDelegate(MarketPriceItem MktItems);

        private readonly MessageDistribute msgDistribute;
        private readonly String productCode;
        private readonly uint chartPeriod;
        private readonly MarketPriceReceivedDelegate marketPriceReceivedDelegate;

        private MarketPriceReceivedAsyncDelegate marketPriceReceivedAsyncDelegate;

        public SCIChartAnalysisMarketPrice(MessageDistribute _msgDistribute, String productCode, uint chartPeriod, MarketPriceReceivedDelegate del, OHLCPriceSeries.PriceSeries priceSeries)
        {
            this.msgDistribute = _msgDistribute;
            this.productCode = productCode;
            this.chartPeriod = chartPeriod;
            this.marketPriceReceivedDelegate = del;

            this.marketPriceReceivedAsyncDelegate = new MarketPriceReceivedAsyncDelegate(MarketPriceMessageReceived);

            if (this.msgDistribute != null)
            {
                this.msgDistribute.DisMarketPrice += new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);

                if (this.productCode != null)
                {
                    if (this.productCode.Length > 0)
                    {
                        List<String> lsnew = new List<String>();
                        lsnew.Add(this.productCode);
                        TradeStationSend.Get(lsnew, cmdClient.getMarketPrice);
                        TradeStationSend.Send(null, this.productCode, cmdClient.registerMarketPrice);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (this.msgDistribute != null)
            {
                this.msgDistribute.DisMarketPrice -= new MessageDistribute.OnDisMarketPrice(distributeMsg_DisMarketPrice);

                if (this.productCode != null)
                {
                    if (this.productCode.Length > 0)
                    {
                        TradeStationSend.Send(this.productCode, null, cmdClient.registerMarketPrice);
                    }
                }
            }

            this.marketPriceReceivedAsyncDelegate = null;
        }

        protected void distributeMsg_DisMarketPrice(object sender, ObservableCollection<MarketPriceItem> MktItems)
        {
            MarketPriceItem results = MktItems.FirstOrDefault(x => x.ProductCode == this.productCode);

            if (results != null)
            {
                if (this.marketPriceReceivedAsyncDelegate != null)
                {
                    Application.Current.Dispatcher.Invoke(this.marketPriceReceivedAsyncDelegate, new Object[] { results });
                }
            }
        }

        private void MarketPriceMessageReceived(MarketPriceItem MktItems)
        {
            this.marketPriceReceivedDelegate(MktItems, false);
        }
    }
}