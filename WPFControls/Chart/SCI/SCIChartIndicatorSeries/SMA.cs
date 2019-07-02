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
using Abt.Controls.SciChart;
using Abt.Controls.SciChart.Model.DataSeries;
using System.Diagnostics;
using GOSTS;
using GOSTS.Common;
using WPF.MDI;
using GOSTS.WPFControls.Chart.SCI;
using System.Collections.ObjectModel;
using Abt.Controls.SciChart.Visuals.Annotations;
using Abt.Controls.SciChart.Visuals.RenderableSeries;
using System.ComponentModel;
using Abt.Controls.SciChart.ChartModifiers;
using System.Windows.Controls.Primitives;

namespace GOSTS.WPFControls.Chart.SCI.SCIChartIndicatorSeries
{
    public partial class ChartIndicators
    {
        public class MovingAverage
        {
            private readonly uint _length;
            private int _circIndex = -1;
            private bool _filled;
            private double _current = double.NaN;
            private readonly double _oneOverLength;
            private readonly double[] _circularBuffer;
            private double _total;

            public MovingAverage(uint length)
            {
                _length = length;
                _oneOverLength = 1.0 / _length;
                _circularBuffer = new double[_length];
            }

            public MovingAverage Update(double value)
            {
                double lostValue = _circularBuffer[_circIndex];
                _circularBuffer[_circIndex] = value;

                // Maintain totals for Push function
                _total += value;
                _total -= lostValue;

                // If not yet filled, just return. Current value should be double.NaN
                if (!_filled)
                {
                    _current = double.NaN;
                    return this;
                }

                // Compute the average
                double average = 0.0;
                for (uint i = 0; i < _length; i++)
                {
                    average += _circularBuffer[i];
                }

                _current = average * _oneOverLength;

                return this;
            }

            public MovingAverage Push(double value)
            {
                // Apply the circular buffer
                if (++_circIndex == _length)
                {
                    _circIndex = 0;
                }

                double lostValue = _circularBuffer[_circIndex];
                _circularBuffer[_circIndex] = value;

                // Compute the average
                _total += value;
                _total -= lostValue;

                // If not yet filled, just return. Current value should be double.NaN
                if (!_filled && _circIndex != _length - 1)
                {
                    _current = double.NaN;
                    return this;
                }
                else
                {
                    // Set a flag to indicate this is the first time the buffer has been filled
                    _filled = true;
                }

                _current = _total * _oneOverLength;

                return this;
            }

            public uint Length { get { return _length; } }
            public double Current { get { return _current; } }
        }
    }

    public class SMA : ChartIndicatorBase
    {
        private readonly SCIChartAnalysis parent;
        private readonly OhlcDataSeries<DateTime, double> chartDataSource;
        private readonly uint smaPeriod;
        private readonly String smaColour;

        private ChartIndicators.MovingAverage smaCircularBuffer;
        private XyDataSeries<DateTime, double> _smaDataSource;
        private FastLineRenderableSeries _smaRenderableSeries;

        public SMA(SCIConstants.ChartIndicatorType type, SCIChartAnalysis parent, OhlcDataSeries<DateTime, double> chartDataSource, uint smaPeriod, String smaColour)
        {
            this.chartType = type;
            this.parent = parent;
            this.chartDataSource = chartDataSource;
            this.smaPeriod = smaPeriod;
            this.smaColour = smaColour;

            this.CreateRenderSeries();
        }

        protected override void Dispose(bool disposing)
        {
            if (this.parent != null)
            {
                if (this.parent.legendAnnotation != null)
                {
                    this.parent.legendAnnotation.RemoveLabel(this.chartType);
                }
            }

            this.smaRenderableSeries = null;
            this.smaDataSource = null;
            this.smaCircularBuffer = null;
        }

        #region properties
        public XyDataSeries<DateTime, double> smaDataSource
        {
            get { return _smaDataSource; }
            set
            {
                if (_smaRenderableSeries != null)
                {
                    _smaRenderableSeries.DataSeries = null;
                }

                _smaDataSource = value;
            }
        }

        public FastLineRenderableSeries smaRenderableSeries
        {
            get { return _smaRenderableSeries; }
            set
            {
                if (_smaRenderableSeries != null)
                {
                    _smaRenderableSeries.DataSeries = null;

                    if (this.parent != null)
                    {
                        this.parent.sciChart.RenderableSeries.Remove(_smaRenderableSeries);
                    }
                }

                _smaRenderableSeries = value;
            }
        }
        #endregion

        #region Internal methods
        private void CreateRenderSeries()
        {
            if ((this.parent == null) || (this.chartDataSource == null) || (this.smaPeriod <= 0))
            {
                return;
            }

            this.smaRenderableSeries = new FastLineRenderableSeries();
            this.smaDataSource = new XyDataSeries<DateTime, double>();
            this.smaCircularBuffer = new ChartIndicators.MovingAverage(this.smaPeriod);

            this.smaRenderableSeries.SeriesColor = (Color)ColorConverter.ConvertFromString(this.smaColour);
            this.smaRenderableSeries.AntiAliasing = false;
            this.smaRenderableSeries.StrokeThickness = 1;
            this.smaRenderableSeries.SnapsToDevicePixels = true;

            double[] smaYBuffer = new double[this.chartDataSource.Count];

            for (int i = 0; i < this.chartDataSource.Count; i++)
            {
                smaYBuffer[i] = this.smaCircularBuffer.Push(this.chartDataSource.CloseValues[i]).Current;
            }

            this.smaDataSource.Append(this.chartDataSource.XValues, smaYBuffer);

            this.smaRenderableSeries.DataSeries = this.smaDataSource;

            using (this.parent.sciChart.SuspendUpdates())
            {
                this.parent.sciChart.RenderableSeries.Add(this.smaRenderableSeries);
            }

            if (this.parent.legendAnnotation != null)
            {
                Binding backgroundBinding = new Binding("Background");
                backgroundBinding.Source = this.parent.sciChart;
                Binding foregroundBinding = new Binding("SeriesColor");
                foregroundBinding.Source = this.smaRenderableSeries;
                foregroundBinding.Converter = new ColorToBrushConverter();

                Label smaLabel = new Label();
                smaLabel.SetBinding(Label.BackgroundProperty, backgroundBinding);
                StackPanel panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;
                TextBlock text1 = new TextBlock();
                text1.SetBinding(TextBlock.ForegroundProperty, foregroundBinding);
                text1.SetBinding(TextBlock.BackgroundProperty, backgroundBinding);
                text1.FontSize = 13;
                text1.FontWeight = FontWeights.SemiBold;
                text1.Text = this.getSMAText() + " (" + this.smaPeriod.ToString() + ")";
                panel.Children.Add(text1);
                smaLabel.Content = panel;
                List<Label> labelList = new List<Label>();
                labelList.Add(smaLabel);

                using (this.parent.sciChart.SuspendUpdates())
                {
                    this.parent.legendAnnotation.AddLabel(this.chartType, labelList);
                }
            }
        }

        private String getSMAText()
        {
            String smaText = "";

            switch (this.chartType)
            {
                case SCIConstants.ChartIndicatorType.SMA1:
                    smaText = "SMA1";
                    break;

                case SCIConstants.ChartIndicatorType.SMA2:
                    smaText = "SMA2";
                    break;

                case SCIConstants.ChartIndicatorType.SMA3:
                    smaText = "SMA3";
                    break;

                default:
                    break;
            }

            return smaText;
        }
        #endregion

        #region Public methods
        public override void UpdateRenderSeries(bool addNewPoint, MarketPriceItem MktItems, DateTime time)
        {
            if ((this.parent == null) || (this.smaCircularBuffer == null) || (this.smaDataSource == null))
            {
                return;
            }

            using (this.parent.sciChart.SuspendUpdates())
            {
                double yClose = TradeStationTools.ConvertToDouble(MktItems.Last);

                if (addNewPoint == true)
                {
                    double smaValue = this.smaCircularBuffer.Push(yClose).Current;
                    this.smaDataSource.Append(time, smaValue);
                }
                else
                {
                    double smaValue = this.smaCircularBuffer.Update(yClose).Current;
                    this.smaDataSource.Update(time, smaValue);
                }
            }
        }

        public override void UpdateSCIChartThemeStyle(String sciChartSelectedTheme)
        {
        }

        public override void RemoveFromGrid()
        {
        }

        public override void AddToGrid()
        {
        }

        public override void ChartEnable()
        {
        }

        public override void ChartDisable()
        {
        }
        #endregion
    }
}
