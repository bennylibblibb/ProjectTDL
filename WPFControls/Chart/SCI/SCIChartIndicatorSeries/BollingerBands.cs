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
        public class BollingerBands
        {
            private readonly uint _length;
            private readonly double _bbDevi;
            private int _circIndex = -1;
            private bool _filled;
            private double _upper = double.NaN;
            private double _middle = double.NaN;
            private double _lower = double.NaN;
            private readonly double _oneOverLength;
            private readonly double[] _averageCircularBuffer;
            private double _averageTotal;

            public BollingerBands(uint length, double bbDevi)
            {
                _length = length;
                _bbDevi = bbDevi;

                _oneOverLength = 1.0 / _length;
                _averageCircularBuffer = new double[_length];
            }

            public BollingerBands Update(double value)
            {
                double lostValue = _averageCircularBuffer[_circIndex];
                _averageCircularBuffer[_circIndex] = value;

                // Maintain totals for Push function
                _averageTotal += value;
                _averageTotal -= lostValue;

                // If not yet filled, just return. Current value should be double.NaN
                if (!_filled)
                {
                    _upper = double.NaN;
                    _middle = double.NaN;
                    _lower = double.NaN;
                    return this;
                }

                // Compute the average
                double average = 0.0;
                double sd = 0;

                for (uint i = 0; i < _length; i++)
                {
                    average += _averageCircularBuffer[i];
                }

                _middle = average * _oneOverLength;

                for (uint i = 0; i < _length; i++)
                {
                    double diff = _averageCircularBuffer[i] - _middle;
                    diff = diff * diff;
                    sd += diff;
                }

                sd = Math.Sqrt(sd / _length) * _bbDevi;
                _upper = _middle + sd;
                _lower = _middle - sd;

                return this;
            }

            public BollingerBands Push(double value)
            {
                // Apply the circular buffer
                if (++_circIndex == _length)
                {
                    _circIndex = 0;
                }

                double lostValue = _averageCircularBuffer[_circIndex];
                _averageCircularBuffer[_circIndex] = value;

                // Compute the average
                _averageTotal += value;
                _averageTotal -= lostValue;

                // If not yet filled, just return. Current value should be double.NaN
                if (!_filled && _circIndex != _length - 1)
                {
                    _upper = double.NaN;
                    _middle = double.NaN;
                    _lower = double.NaN;
                    return this;
                }
                else
                {
                    // Set a flag to indicate this is the first time the buffer has been filled
                    _filled = true;
                }

                _middle = _averageTotal * _oneOverLength;

                double sd = 0;

                for (uint i = 0; i < _averageCircularBuffer.Length; i++)
                {
                    double diff = _averageCircularBuffer[i] - _middle;
                    diff = diff * diff;
                    sd += diff;
                }

                sd = Math.Sqrt(sd / _length) * _bbDevi;
                _upper = _middle + sd;
                _lower = _middle - sd;

                return this;
            }

            public uint Length { get { return _length; } }
            public double Upper { get { return _upper; } }
            public double Middle { get { return _middle; } }
            public double Lower { get { return _lower; } }
        }
    }

    public class BollingerBands : ChartIndicatorBase
    {
        private readonly SCIChartAnalysis parent;
        private readonly OhlcDataSeries<DateTime, double> chartDataSource;
        private readonly uint bbPeriod;
        private readonly double bbDevi;
        private readonly String bbUpperLowerColour;
        private readonly String bbMiddleColour;

        private ChartIndicators.BollingerBands bbCircularBuffer;
        private XyDataSeries<DateTime, double> _bbUpperDataSource;
        private XyDataSeries<DateTime, double> _bbMiddleDataSource;
        private XyDataSeries<DateTime, double> _bbLowerDataSource;
        private FastLineRenderableSeries _bbUpperRenderableSeries;
        private FastLineRenderableSeries _bbMiddleRenderableSeries;
        private FastLineRenderableSeries _bbLowerRenderableSeries;

        public BollingerBands(SCIConstants.ChartIndicatorType type, SCIChartAnalysis parent, OhlcDataSeries<DateTime, double> chartDataSource, uint period, double devi, String outerBandColour, String smaColour)
        {
            this.chartType = type;
            this.parent = parent;
            this.chartDataSource = chartDataSource;
            this.bbPeriod = period;
            this.bbDevi = devi;
            this.bbUpperLowerColour = outerBandColour;
            this.bbMiddleColour = smaColour;

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

            this.bbUpperRenderableSeries = null;
            this.bbMiddleRenderableSeries = null;
            this.bbLowerRenderableSeries = null;
            this.bbUpperDataSource = null;
            this.bbMiddleDataSource = null;
            this.bbLowerDataSource = null;
            this.bbCircularBuffer = null;
        }

        #region properties
        public XyDataSeries<DateTime, double> bbUpperDataSource
        {
            get { return _bbUpperDataSource; }
            set
            {
                if (_bbUpperRenderableSeries != null)
                {
                    _bbUpperRenderableSeries.DataSeries = null;
                }

                _bbUpperDataSource = value;
            }
        }

        public XyDataSeries<DateTime, double> bbMiddleDataSource
        {
            get { return _bbMiddleDataSource; }
            set
            {
                if (_bbMiddleRenderableSeries != null)
                {
                    _bbMiddleRenderableSeries.DataSeries = null;
                }

                _bbMiddleDataSource = value;
            }
        }

        public XyDataSeries<DateTime, double> bbLowerDataSource
        {
            get { return _bbLowerDataSource; }
            set
            {
                if (_bbLowerRenderableSeries != null)
                {
                    _bbLowerRenderableSeries.DataSeries = null;
                }

                _bbLowerDataSource = value;
            }
        }

        public FastLineRenderableSeries bbUpperRenderableSeries
        {
            get { return _bbUpperRenderableSeries; }
            set
            {
                if (_bbUpperRenderableSeries != null)
                {
                    _bbUpperRenderableSeries.DataSeries = null;

                    if (this.parent != null)
                    {
                        using (this.parent.sciChart.SuspendUpdates())
                        {
                            this.parent.sciChart.RenderableSeries.Remove(_bbUpperRenderableSeries);
                        }
                    }
                }

                _bbUpperRenderableSeries = value;
            }
        }

        public FastLineRenderableSeries bbMiddleRenderableSeries
        {
            get { return _bbMiddleRenderableSeries; }
            set
            {
                if (_bbMiddleRenderableSeries != null)
                {
                    _bbMiddleRenderableSeries.DataSeries = null;

                    if (this.parent != null)
                    {
                        using (this.parent.sciChart.SuspendUpdates())
                        {
                            this.parent.sciChart.RenderableSeries.Remove(_bbMiddleRenderableSeries);
                        }
                    }
                }

                _bbMiddleRenderableSeries = value;
            }
        }

        public FastLineRenderableSeries bbLowerRenderableSeries
        {
            get { return _bbLowerRenderableSeries; }
            set
            {
                if (_bbLowerRenderableSeries != null)
                {
                    _bbLowerRenderableSeries.DataSeries = null;

                    if (this.parent != null)
                    {
                        using (this.parent.sciChart.SuspendUpdates())
                        {
                            this.parent.sciChart.RenderableSeries.Remove(_bbLowerRenderableSeries);
                        }
                    }
                }

                _bbLowerRenderableSeries = value;
            }
        }
        #endregion

        #region Internal methods
        private void CreateRenderSeries()
        {
            if ((this.parent == null) || (this.chartDataSource == null) || (this.bbPeriod <= 0) || (this.bbDevi <= 0))
            {
                return;
            }

            this.bbUpperRenderableSeries = new FastLineRenderableSeries();
            this.bbMiddleRenderableSeries = new FastLineRenderableSeries();
            this.bbLowerRenderableSeries = new FastLineRenderableSeries();

            this.bbUpperRenderableSeries.AntiAliasing = false;
            this.bbMiddleRenderableSeries.AntiAliasing = false;
            this.bbLowerRenderableSeries.AntiAliasing = false;

            this.bbUpperRenderableSeries.StrokeThickness = 1;
            this.bbMiddleRenderableSeries.StrokeThickness = 1;
            this.bbLowerRenderableSeries.StrokeThickness = 1;

            this.bbUpperRenderableSeries.SnapsToDevicePixels = true;
            this.bbMiddleRenderableSeries.SnapsToDevicePixels = true;
            this.bbLowerRenderableSeries.SnapsToDevicePixels = true;

            this.bbUpperRenderableSeries.SeriesColor = (Color)ColorConverter.ConvertFromString(this.bbUpperLowerColour);
            this.bbMiddleRenderableSeries.SeriesColor = (Color)ColorConverter.ConvertFromString(this.bbMiddleColour);
            this.bbLowerRenderableSeries.SeriesColor = (Color)ColorConverter.ConvertFromString(this.bbUpperLowerColour);

            this.bbUpperDataSource = new XyDataSeries<DateTime, double>();
            this.bbMiddleDataSource = new XyDataSeries<DateTime, double>();
            this.bbLowerDataSource = new XyDataSeries<DateTime, double>();

            this.bbCircularBuffer = new ChartIndicators.BollingerBands(this.bbPeriod, this.bbDevi);
            double[] bbUpperBuffer = new double[this.chartDataSource.Count];
            double[] bbMiddleBuffer = new double[this.chartDataSource.Count];
            double[] bbLowerBuffer = new double[this.chartDataSource.Count];

            for (int i = 0; i < this.chartDataSource.Count; i++)
            {
                this.bbCircularBuffer.Push(this.chartDataSource.CloseValues[i]);
                bbUpperBuffer[i] = this.bbCircularBuffer.Upper;
                bbMiddleBuffer[i] = this.bbCircularBuffer.Middle;
                bbLowerBuffer[i] = this.bbCircularBuffer.Lower;
            }

            this.bbUpperDataSource.Append(this.chartDataSource.XValues, bbUpperBuffer);
            this.bbMiddleDataSource.Append(this.chartDataSource.XValues, bbMiddleBuffer);
            this.bbLowerDataSource.Append(this.chartDataSource.XValues, bbLowerBuffer);

            this.bbUpperRenderableSeries.DataSeries = this.bbUpperDataSource;
            this.bbMiddleRenderableSeries.DataSeries = this.bbMiddleDataSource;
            this.bbLowerRenderableSeries.DataSeries = this.bbLowerDataSource;

            using (this.parent.sciChart.SuspendUpdates())
            {
                this.parent.sciChart.RenderableSeries.Add(this.bbUpperRenderableSeries);
                this.parent.sciChart.RenderableSeries.Add(this.bbMiddleRenderableSeries);
                this.parent.sciChart.RenderableSeries.Add(this.bbLowerRenderableSeries);
            }

            if (this.parent.legendAnnotation != null)
            {
                Binding backgroundBinding = new Binding("Background");
                backgroundBinding.Source = this.parent.sciChart;
                Binding bbDeviForegroundBinding = new Binding("SeriesColor");
                bbDeviForegroundBinding.Source = this.bbUpperRenderableSeries;
                bbDeviForegroundBinding.Converter = new ColorToBrushConverter();
                Binding bbPeriodForegroundBinding = new Binding("SeriesColor");
                bbPeriodForegroundBinding.Source = this.bbMiddleRenderableSeries;
                bbPeriodForegroundBinding.Converter = new ColorToBrushConverter();

                Label bbLabel = new Label();
                bbLabel.SetBinding(Label.BackgroundProperty, backgroundBinding);
                StackPanel panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;
                TextBlock text1 = new TextBlock();
                text1.SetBinding(TextBlock.ForegroundProperty, bbDeviForegroundBinding);
                text1.SetBinding(TextBlock.BackgroundProperty, backgroundBinding);
                text1.FontSize = 13;
                text1.FontWeight = FontWeights.SemiBold;
                text1.Text = "BB (";
                panel.Children.Add(text1);
                TextBlock text2 = new TextBlock();
                text2.SetBinding(TextBlock.ForegroundProperty, bbPeriodForegroundBinding);
                text2.SetBinding(TextBlock.BackgroundProperty, backgroundBinding);
                text2.FontSize = 13;
                text2.FontWeight = FontWeights.SemiBold;
                text2.Text = this.bbPeriod.ToString();
                panel.Children.Add(text2);
                TextBlock text3 = new TextBlock();
                text3.SetBinding(TextBlock.ForegroundProperty, bbDeviForegroundBinding);
                text3.SetBinding(TextBlock.BackgroundProperty, backgroundBinding);
                text3.FontSize = 13;
                text3.FontWeight = FontWeights.SemiBold;
                text3.Text = ", " + this.bbDevi.ToString() + ")";
                panel.Children.Add(text3);
                bbLabel.Content = panel;
                List<Label> labelList = new List<Label>();
                labelList.Add(bbLabel);

                using (this.parent.sciChart.SuspendUpdates())
                {
                    this.parent.legendAnnotation.AddLabel(this.chartType, labelList);
                }
            }
        }
        #endregion

        #region Public methods
        public override void UpdateRenderSeries(bool addNewPoint, MarketPriceItem MktItems, DateTime time)
        {
            if ((this.parent == null) || (this.bbCircularBuffer == null) || (this.bbUpperDataSource == null) || (this.bbMiddleDataSource == null) || (this.bbLowerDataSource == null))
            {
                return;
            }

            using (this.parent.sciChart.SuspendUpdates())
            {
                double yClose = TradeStationTools.ConvertToDouble(MktItems.Last);

                if (addNewPoint == true)
                {
                    this.bbCircularBuffer.Push(yClose);
                    this.bbUpperDataSource.Append(time, this.bbCircularBuffer.Upper);
                    this.bbMiddleDataSource.Append(time, this.bbCircularBuffer.Middle);
                    this.bbLowerDataSource.Append(time, this.bbCircularBuffer.Lower);
                }
                else
                {
                    this.bbCircularBuffer.Update(yClose);
                    this.bbUpperDataSource.Update(time, this.bbCircularBuffer.Upper);
                    this.bbMiddleDataSource.Update(time, this.bbCircularBuffer.Middle);
                    this.bbLowerDataSource.Update(time, this.bbCircularBuffer.Lower);
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
