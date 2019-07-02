using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GOSTS.Common;
using Abt.Controls.SciChart.Model.DataSeries;
using System.Diagnostics;
using System.Data;
using System.Windows;

namespace GOSTS.WPFControls.Chart.SCI
{
    class SCIChartAnalysisChartHistory : IDisposable
    {
        private delegate void ChartHistoryReceivedAsyncDelegate(TradeStationComm.infoClass.Chart.ChartHistoryData chartTable);

        private readonly MessageDistribute msgDistribute;
        private readonly String productCode;
        private readonly uint chartPeriod;
        private readonly ChartHistoryReceivedDelegate chartHistoryReceivedDelegate;
        private readonly bool showChartBreakPeriods;
        private MessageDistribute.onDisChartInfo onDisChartInfo;

        private ChartHistoryReceivedAsyncDelegate chartHistoryReceivedAsyncDelegate;

        public SCIChartAnalysisChartHistory(MessageDistribute _msgDistribute, String productCode, uint chartPeriod, ChartHistoryReceivedDelegate del, bool showChartBreakPeriods)
        {
            this.msgDistribute = _msgDistribute;
            this.productCode = productCode;
            this.chartPeriod = chartPeriod;
            this.chartHistoryReceivedDelegate = del;
            this.showChartBreakPeriods = showChartBreakPeriods;

            this.chartHistoryReceivedAsyncDelegate = new ChartHistoryReceivedAsyncDelegate(ChartHistoryMessageReceived);

            if (this.msgDistribute != null)
            {
                this.onDisChartInfo = new MessageDistribute.onDisChartInfo(distributeMsg_ChartInfo);
                this.msgDistribute.DisChartInfo += this.onDisChartInfo;

                List<String> stringList = new List<String>();
                stringList.Add(this.productCode);
                stringList.Add(this.chartPeriod.ToString());
                stringList.Add("0");
                TradeStationSend.Send(cmdClient.getChart, stringList);
                Debug.WriteLine("cmdClient.getChart:[{0}][{1}]", this.productCode, this.chartPeriod);
            }
        }

        public void Dispose()
        {
            if (this.onDisChartInfo != null)
            {
                this.msgDistribute.DisChartInfo -= this.onDisChartInfo;
                this.onDisChartInfo = null;
            }

            this.chartHistoryReceivedAsyncDelegate = null;
        }

        protected void distributeMsg_ChartInfo(object sender, TradeStationComm.infoClass.Chart.ChartHistoryData chartTable)
        {
            if (this.chartHistoryReceivedAsyncDelegate != null)
            {
                Application.Current.Dispatcher.Invoke(this.chartHistoryReceivedAsyncDelegate, new Object[] { chartTable });
            }
        }

        private void ChartHistoryMessageReceived(TradeStationComm.infoClass.Chart.ChartHistoryData chartTable)
        {
            if (this.chartHistoryReceivedAsyncDelegate == null)
            {
                return;
            }

            if (chartTable.productCode.Equals(this.productCode) == false)
            {
                Debug.WriteLine("ChartHistoryMessageReceived product mismatch recv=" + chartTable.productCode + " this=" + this.productCode);
                return;
            }

            this.Dispose();

            OHLCPriceSeries.PriceSeries priceSeries = new OHLCPriceSeries.PriceSeries();

            String oldRowDateTimeAsStr = null;
            String oldRowClose = null;
            String oldRowTotalVolume = "0";
            String oldRowHigh = null;
            String oldRowLow = null;
            String oldRowOpen = null;

            for (int i = 0; i < chartTable.points.Rows.Count; )
            {
                DataRow row = chartTable.points.Rows[i];

                String rowDate = row.getColValue("date").Substring(0, 8);
                String rowTime = row.getColValue("time").Substring(0, 6);
                String rowClose = row.getColValue("close");
                String rowTotalVolume = row.getColValue("volume");
                String rowHigh = row.getColValue("high");
                String rowLow = row.getColValue("low");
                String rowOpen = row.getColValue("open");

                //Debug.WriteLine("[{0}][{1}][{2}][{3}][{4}][{5}][{6}]", rowDate, rowTime, rowClose, rowTotalVolume, rowHigh, rowLow, rowOpen);

                long rowVol = Convert.ToInt64(rowTotalVolume);
                long oldVol = Convert.ToInt64(oldRowTotalVolume);

                if (rowVol >= oldVol)
                {
                    rowVol -= oldVol;
                }

                DateTime rowDateTime = DateTime.ParseExact(rowDate + rowTime, "yyyyMMddHHmmss", null);
                rowDateTime = SCICommon.FilterMarketPriceTime(rowDateTime, this.chartPeriod);
                rowDateTime.Subtract(new TimeSpan(0, 0, (int)this.chartPeriod));

                //Fill in missing points
                if (oldRowDateTimeAsStr != null)
                {
                    DateTime oldRowDateTime = DateTime.ParseExact(oldRowDateTimeAsStr, "yyyyMMddHHmmss", null);
                    TimeSpan time = new TimeSpan(0, 0, (int)this.chartPeriod);
                    oldRowDateTime = oldRowDateTime.Add(time);

                    int result = DateTime.Compare(oldRowDateTime, rowDateTime);
                    if (result < 0)
                    {
                        rowDateTime = oldRowDateTime;
                        rowClose = oldRowClose;
                        rowVol = 0;
                        rowHigh = oldRowClose;
                        rowLow = oldRowClose;
                        rowOpen = oldRowClose;
                        i--;
                    }
                }

                String rowDateTimeAsStr = rowDateTime.ToString("yyyyMMddHHmmss");
                rowDateTime = DateTime.ParseExact(rowDateTimeAsStr, "yyyyMMddHHmmss", null);

                OHLCPriceSeries.PriceBar priceBar = new OHLCPriceSeries.PriceBar();
                priceBar.DateTime = rowDateTime;
                priceBar.Open = Convert.ToDouble(rowOpen);
                priceBar.High = Convert.ToDouble(rowHigh);
                priceBar.Low = Convert.ToDouble(rowLow);
                priceBar.Close = Convert.ToDouble(rowClose);
                priceBar.Volume = rowVol;

                if (this.ShouldAddPointToChartHistory(priceBar) == true)
                {
                    priceSeries.Add(priceBar);
                }

                oldRowDateTimeAsStr = rowDateTimeAsStr;
                oldRowClose = rowClose;
                oldRowTotalVolume = rowTotalVolume;
                oldRowHigh = rowHigh;
                oldRowLow = rowLow;
                oldRowOpen = rowOpen;
                i++;
            }

            this.chartHistoryReceivedDelegate(priceSeries, Convert.ToInt64(oldRowTotalVolume));
        }

        private bool ShouldAddPointToChartHistory(OHLCPriceSeries.PriceBar priceBar)
        {
            bool shouldAddPoint = true;

            if (this.showChartBreakPeriods == false)
            {
                int pointTime = Convert.ToInt32(priceBar.DateTime.ToString("HHmmss"));

                if ((pointTime > 120000) && (pointTime < 130000) || ((pointTime > 161500) && (pointTime < 170000)))
                {
                    shouldAddPoint = false;
                }
            }

            return shouldAddPoint;
        }
    }
}
