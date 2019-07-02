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
using Abt.Controls.SciChart.Model;
using Abt.Controls.SciChart.Visuals;
using Abt.Controls.SciChart.Visuals.Annotations;
using Abt.Controls.SciChart.Visuals.RenderableSeries;
using System.ComponentModel;
using Abt.Controls.SciChart.ChartModifiers;
using System.Windows.Controls.Primitives;
using TicTacTec.TA.Library;
using Abt.Controls.SciChart.Visuals.Axes;
using Abt.Controls.SciChart.Utility.Mouse;

namespace GOSTS.WPFControls.Chart.SCI.SCIChartIndicatorSeries
{
    public partial class ChartIndicators
    {
        public class Macdres
        {
            public double Macd { get; set; }
            public double Signal { get; set; }
            public double MacdHistogram { get; set; }
        }

        public class MACD
        {
            private readonly uint fastPeriod;
            private readonly uint slowPeriod;
            private readonly uint signalPeriod;

            private List<double> closeSource;

            public MACD(uint fast, uint slow, uint signal)
            {
                fastPeriod = fast;
                slowPeriod = slow;
                signalPeriod = signal;

                closeSource = new List<double>();
            }

            public Macdres Update(double value)
            {
                this.closeSource[this.closeSource.Count - 1] = value;

                int sourceCount = this.closeSource.Count();
                double[] outMACD = new double[sourceCount + 1];
                double[] outMACDSignal = new double[sourceCount + 1];
                double[] outMACDHist = new double[sourceCount + 1];
                int outBegIdx;
                int outNBElement;

                Macdres macdRes = new Macdres();
                macdRes.Macd = double.NaN;
                macdRes.Signal = double.NaN;
                macdRes.MacdHistogram = double.NaN;

                int endIdx = sourceCount - 1;
                Core.RetCode res = Core.Macd(0, endIdx, closeSource.ToArray(), (int)this.fastPeriod, (int)this.slowPeriod, (int)this.signalPeriod, out outBegIdx, out outNBElement, outMACD, outMACDSignal, outMACDHist);

                if (res == Core.RetCode.Success)
                {
                    if (outNBElement > 0) { outNBElement--; }
                    double outMACDElement = outMACD[outNBElement];
                    double outMACDSignalElement = outMACDSignal[outNBElement];
                    double outMACDHistElement = outMACDHist[outNBElement];
                    macdRes.Macd = outMACDElement;
                    macdRes.Signal = outMACDSignalElement;
                    macdRes.MacdHistogram = outMACDHistElement;
                }

                return macdRes;
            }

            public Macdres Push(double value)
            {
                this.closeSource.Add(value);

                int sourceCount = this.closeSource.Count();
                double[] outMACD = new double[sourceCount + 1];
                double[] outMACDSignal = new double[sourceCount + 1];
                double[] outMACDHist = new double[sourceCount + 1];
                int outBegIdx;
                int outNBElement;

                Macdres macdRes = new Macdres();
                macdRes.Macd = double.NaN;
                macdRes.Signal = double.NaN;
                macdRes.MacdHistogram = double.NaN;

                int endIdx = sourceCount - 1;
                double[] closeSourceArray = this.closeSource.ToArray();
                Core.RetCode res = Core.Macd(0, endIdx, closeSourceArray, (int)this.fastPeriod, (int)this.slowPeriod, (int)this.signalPeriod, out outBegIdx, out outNBElement, outMACD, outMACDSignal, outMACDHist);

                if (res == Core.RetCode.Success)
                {
                    if (outNBElement > 0) { outNBElement--; }
                    macdRes.Macd = outMACD[outNBElement];
                    macdRes.Signal = outMACDSignal[outNBElement];
                    macdRes.MacdHistogram = outMACDHist[outNBElement];
                }

                return macdRes;
            }

            public List<object> Push(List<double> values)
            {
                this.closeSource.AddRange(values);

                int sourceCount = this.closeSource.Count();
                double[] outMACD = new double[sourceCount];
                double[] outMACDSignal = new double[sourceCount];
                double[] outMACDHist = new double[sourceCount];
                int outBegIdx;
                int outNBElement;

                List<object> macdRes = new List<object>();

                int endIdx = sourceCount - 1;
                double[] closeSourceArray = this.closeSource.ToArray();
                Core.RetCode res = Core.Macd(0, endIdx, closeSourceArray, (int)this.fastPeriod, (int)this.slowPeriod, (int)this.signalPeriod, out outBegIdx, out outNBElement, outMACD, outMACDSignal, outMACDHist);

                List<double> outMACD1 = new List<double>();
                List<double> outMACDSignal1 = new List<double>();
                List<double> outMACDHist1 = new List<double>();

                if (res == Core.RetCode.Success)
                {
                    for (int i = 0; i < outBegIdx; i++)
                    {
                        outMACD1.Add(double.NaN);
                        outMACDSignal1.Add(double.NaN);
                        outMACDHist1.Add(double.NaN);
                    }

                    for (int i = 0; i < outNBElement; i++)
                    {
                        outMACD1.Add(outMACD[i]);
                        outMACDSignal1.Add(outMACDSignal[i]);
                        outMACDHist1.Add(outMACDHist[i]);
                    }
                }

                macdRes.Add(outMACD1.ToArray());
                macdRes.Add(outMACDSignal1.ToArray());
                macdRes.Add(outMACDHist1.ToArray());

                return macdRes;
            }
        }
    }

    class MACD : ChartIndicatorBase
    {
        private readonly SCIChartAnalysis parent;
        private readonly OhlcDataSeries<DateTime, double> chartDataSource;
        private readonly uint chartPeriod;
        private readonly uint fastPeriod;
        private readonly uint slowPeriod;
        private readonly uint signalPeriod;
        private readonly String macdColour;
        private readonly String signalColour;
        private readonly String histogramColour;

        private String sciChartSelectedTheme;

        private Grid _macdGrid;
        private RowDefinition _macdRowDefinition;
        private SciChartSurface _macdChartSurface;
        private GridSplitter _macdGridSplitter;
        private ChartIndicators.MACD macdIndicator;
        private XyDataSeries<DateTime, double> _macdDataSource;
        private XyDataSeries<DateTime, double> _signalDataSource;
        private XyDataSeries<DateTime, double> _histogramDataSource;
        private FastLineRenderableSeries _macdRenderableSeries;
        private FastLineRenderableSeries _signalRenderableSeries;
        private FastColumnRenderableSeries _histogramRenderableSeries;
        private LegendAnnotation _legendAnnotation;
        private HorizontalLineAnnotation _macdZeroLineAnnotation;

        public MACD(SCIConstants.ChartIndicatorType type, SCIChartAnalysis parent, OhlcDataSeries<DateTime, double> chartDataSource, uint chartPeriod, uint fastPeriod, uint slowPeriod, uint signalPeriod, String macdColour, String signalColour, String histogramColour, String sciChartSelectedTheme)
        {
            this.chartType = type;
            this.parent = parent;
            this.chartDataSource = chartDataSource;
            this.chartPeriod = chartPeriod;
            this.fastPeriod = fastPeriod;
            this.slowPeriod = slowPeriod;
            this.signalPeriod = signalPeriod;
            this.macdColour = macdColour;
            this.signalColour = signalColour;
            this.histogramColour = histogramColour;

            this.sciChartSelectedTheme = sciChartSelectedTheme;

            this.CreateRenderSeries();
        }

        protected override void Dispose(bool disposing)
        {
            this.legendAnnotation = null;
            this.macdRenderableSeries = null;
            this.signalRenderableSeries = null;
            this.histogramRenderableSeries = null;
            this.macdDataSource = null;
            this.signalDataSource = null;
            this.histogramDataSource = null;
            this.macdIndicator = null;
            this.macdGridSplitter = null;
            this.macdChartSurface = null;
            this.macdRowDefinition = null;
            this.macdGrid = null;
            this.macdZeroLineAnnotation = null;
        }

        #region properties
        public XyDataSeries<DateTime, double> macdDataSource
        {
            get { return _macdDataSource; }
            set
            {
                if (_macdRenderableSeries != null)
                {
                    _macdRenderableSeries.DataSeries = null;
                }

                _macdDataSource = value;
            }
        }

        public XyDataSeries<DateTime, double> signalDataSource
        {
            get { return _signalDataSource; }
            set
            {
                if (_signalRenderableSeries != null)
                {
                    _signalRenderableSeries.DataSeries = null;
                }

                _signalDataSource = value;
            }
        }

        public XyDataSeries<DateTime, double> histogramDataSource
        {
            get { return _histogramDataSource; }
            set
            {
                if (_histogramRenderableSeries != null)
                {
                    _histogramRenderableSeries.DataSeries = null;
                }

                _histogramDataSource = value;
            }
        }

        public FastLineRenderableSeries macdRenderableSeries
        {
            get { return _macdRenderableSeries; }
            set
            {
                if (_macdRenderableSeries != null)
                {
                    _macdRenderableSeries.DataSeries = null;

                    if (this.macdChartSurface != null)
                    {
                        using (this.macdChartSurface.SuspendUpdates())
                        {
                            this.macdChartSurface.RenderableSeries.Remove(_macdRenderableSeries);
                        }
                    }
                }

                _macdRenderableSeries = value;
            }
        }

        public FastLineRenderableSeries signalRenderableSeries
        {
            get { return _signalRenderableSeries; }
            set
            {
                if (_signalRenderableSeries != null)
                {
                    _signalRenderableSeries.DataSeries = null;

                    if (this.macdChartSurface != null)
                    {
                        using (this.macdChartSurface.SuspendUpdates())
                        {
                            this.macdChartSurface.RenderableSeries.Remove(_signalRenderableSeries);
                        }
                    }
                }

                _signalRenderableSeries = value;
            }
        }

        public FastColumnRenderableSeries histogramRenderableSeries
        {
            get { return _histogramRenderableSeries; }
            set
            {
                if (_histogramRenderableSeries != null)
                {
                    _histogramRenderableSeries.DataSeries = null;

                    if (this.macdChartSurface != null)
                    {
                        using (this.macdChartSurface.SuspendUpdates())
                        {
                            this.macdChartSurface.RenderableSeries.Remove(_histogramRenderableSeries);
                        }
                    }
                }

                _histogramRenderableSeries = value;
            }
        }

        public RowDefinition macdRowDefinition
        {
            get { return _macdRowDefinition; }
            set
            {
                if (_macdRowDefinition != null)
                {
                    if (this.parent != null)
                    {
                        if (this.parent.gridChartAnalysis != null)
                        {
                            this.parent.gridChartAnalysis.RowDefinitions.Remove(_macdRowDefinition);
                        }
                    }
                }

                _macdRowDefinition = value;
            }
        }

        private SciChartSurface macdChartSurface
        {
            get { return _macdChartSurface; }
            set
            {
                if (_macdChartSurface != null)
                {
                    using (_macdChartSurface.SuspendUpdates())
                    {
                        if (this.macdGrid != null)
                        {
                            this.macdGrid.Children.Remove(_macdChartSurface);
                        }
                    }
                }

                _macdChartSurface = value;
            }
        }

        private LegendAnnotation legendAnnotation
        {
            get { return _legendAnnotation; }
            set
            {
                if (_legendAnnotation != null)
                {
                    _legendAnnotation.Dispose();
                }

                _legendAnnotation = value;
            }
        }

        private Grid macdGrid
        {
            get { return _macdGrid; }
            set
            {
                if (_macdGrid != null)
                {
                    if (this.parent != null)
                    {
                        if (this.parent.gridChartAnalysis != null)
                        {
                            this.parent.gridChartAnalysis.Children.Remove(_macdGrid);
                        }
                    }

                    _macdGrid.Children.Clear();
                }

                _macdGrid = value;
            }
        }

        private GridSplitter macdGridSplitter
        {
            get { return _macdGridSplitter; }
            set
            {
                if (_macdGridSplitter != null)
                {
                    if (this.macdGrid != null)
                    {
                        this.macdGrid.Children.Remove(_macdGridSplitter);
                    }
                }

                _macdGridSplitter = value;
            }
        }

        private HorizontalLineAnnotation macdZeroLineAnnotation
        {
            get { return _macdZeroLineAnnotation; }
            set
            {
                if (_macdZeroLineAnnotation != null)
                {
                    if (this.macdChartSurface != null)
                    {
                        using (this.macdChartSurface.SuspendUpdates())
                        {
                            this.macdChartSurface.Annotations.Remove(_macdZeroLineAnnotation);
                        }
                    }
                }

                _macdZeroLineAnnotation = value;
            }
        }
        #endregion

        #region Internal methods
        private void CreateRenderSeries()
        {
            if ((this.parent == null) || (this.chartDataSource == null) || (this.fastPeriod <= 0) || (this.slowPeriod <= 0) || (this.signalPeriod <= 0))
            {
                return;
            }

            this.macdChartSurface = new SciChartSurface();

            using (this.macdChartSurface.SuspendUpdates())
            {
                this.macdChartSurface.RenderPriority = RenderPriority.Low;
                SciChartGroup.SetVerticalChartGroup(this.macdChartSurface, "myCharts");
                this.macdChartSurface.Padding = new Thickness(5, 5, 15, 5);

                CategoryDateTimeAxis macdXAxis = new CategoryDateTimeAxis();
                Binding visibleRangeBinding = new Binding("XAxis.VisibleRange");
                visibleRangeBinding.Source = this.parent.sciChart;
                macdXAxis.SetBinding(AxisBase.VisibleRangeProperty, visibleRangeBinding);
                macdXAxis.LabelFormatter = SCICommon.CreateTimeAxisFormatter(this.chartPeriod);
                macdXAxis.DrawLabels = false;
                macdXAxis.DrawMajorBands = false;
                macdXAxis.DrawMajorGridLines = true;
                macdXAxis.DrawMajorTicks = false;
                macdXAxis.DrawMinorGridLines = false;
                macdXAxis.DrawMinorTicks = false;
                this.macdChartSurface.XAxis = macdXAxis;

                NumericAxis macdYAxis = new NumericAxis();
                macdYAxis.AutoRange = AutoRange.Always;
                macdYAxis.VisibleRange = new DoubleRange(-10, 10);
                macdYAxis.DrawMajorGridLines = true;
                macdYAxis.DrawMinorGridLines = false;
                macdYAxis.DrawLabels = true;
                macdYAxis.DrawMajorTicks = true;
                macdYAxis.DrawMinorTicks = false;
                macdYAxis.GrowBy = new DoubleRange(0.1, 0.1);
                this.macdChartSurface.YAxis = macdYAxis;

                ZoomPanModifier macdZoomPanModifier = new ZoomPanModifier();
                macdZoomPanModifier.IsEnabled = false;
                RubberBandXyZoomModifier macdRubberBandXyZoomModifier = new RubberBandXyZoomModifier();
                macdRubberBandXyZoomModifier.IsEnabled = false;
                MouseWheelZoomModifier macdMouseWheelZoomModifier = new MouseWheelZoomModifier();
                macdMouseWheelZoomModifier.IsEnabled = false;
                CursorModifier macdCursorModifier = new CursorModifier();
                Binding macdCursorModifieIsEnabledBinding = new Binding("IsMouseCursorEnabled");
                macdCursorModifieIsEnabledBinding.Source = this.parent;
                macdCursorModifier.SetBinding(CursorModifier.IsEnabledProperty, macdCursorModifieIsEnabledBinding);
                macdCursorModifier.ShowAxisLabels = true;
                macdCursorModifier.ShowTooltip = true;
                macdCursorModifier.ReceiveHandledEvents = true;
                this.macdChartSurface.ChartModifier = new ModifierGroup(macdZoomPanModifier, macdRubberBandXyZoomModifier, macdMouseWheelZoomModifier, macdCursorModifier);
                MouseManager.SetMouseEventGroup((ModifierGroup)this.macdChartSurface.ChartModifier, "MySharedMouseGroup");

                this.macdGrid = new Grid();

                this.macdGridSplitter = new GridSplitter();
                this.macdGridSplitter.IsEnabled = false;
                this.macdGridSplitter.Background = Brushes.Gray;
                this.macdGridSplitter.Width = double.NaN;
                this.macdGridSplitter.Height = 1;
                this.macdGridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.macdGridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
                this.macdGridSplitter.Margin = new Thickness(0);

                RowDefinition gridSplitterRow = new RowDefinition();
                gridSplitterRow.Height = new GridLength(1, GridUnitType.Pixel);
                this.macdGrid.RowDefinitions.Add(gridSplitterRow);
                Grid.SetRow(this.macdGridSplitter, this.macdGrid.Children.Count);
                this.macdGrid.Children.Add(this.macdGridSplitter);

                RowDefinition chartSurfaceRow = new RowDefinition();
                chartSurfaceRow.Height = new GridLength(1, GridUnitType.Star);
                this.macdGrid.RowDefinitions.Add(chartSurfaceRow);
                Grid.SetRow(this.macdChartSurface, this.macdGrid.Children.Count);
                this.macdGrid.Children.Add(this.macdChartSurface);

                this.macdRowDefinition = new RowDefinition();
                this.macdRowDefinition.Name = SCIConstants.SCI_MACD_ROW_DEFINITION;
                this.macdRowDefinition.Height = new GridLength(2, GridUnitType.Star);

                this.parent.gridChartAnalysis.RowDefinitions.Add(this.macdRowDefinition);
                Grid.SetRow(this.macdGrid, this.parent.gridChartAnalysis.Children.Count);
                this.parent.gridChartAnalysis.Children.Add(this.macdGrid);

                this.macdRenderableSeries = new FastLineRenderableSeries();
                this.signalRenderableSeries = new FastLineRenderableSeries();
                this.histogramRenderableSeries = new FastColumnRenderableSeries();

                this.macdRenderableSeries.AntiAliasing = false;
                this.signalRenderableSeries.AntiAliasing = false;
                this.histogramRenderableSeries.AntiAliasing = false;

                this.macdRenderableSeries.StrokeThickness = 1;
                this.signalRenderableSeries.StrokeThickness = 1;
                this.histogramRenderableSeries.StrokeThickness = 1;

                this.macdRenderableSeries.SnapsToDevicePixels = true;
                this.signalRenderableSeries.SnapsToDevicePixels = true;
                this.histogramRenderableSeries.SnapsToDevicePixels = true;

                this.macdRenderableSeries.SeriesColor = (Color)ColorConverter.ConvertFromString(this.macdColour);
                this.signalRenderableSeries.SeriesColor = (Color)ColorConverter.ConvertFromString(this.signalColour);
                this.histogramRenderableSeries.SeriesColor = (Color)ColorConverter.ConvertFromString(this.histogramColour);

                this.histogramRenderableSeries.DataPointWidth = 0.5;

                this.macdDataSource = new XyDataSeries<DateTime, double>();
                this.signalDataSource = new XyDataSeries<DateTime, double>();
                this.histogramDataSource = new XyDataSeries<DateTime, double>();

                this.macdIndicator = new ChartIndicators.MACD(this.fastPeriod, this.slowPeriod, this.signalPeriod);

                List<object> res = this.macdIndicator.Push(this.chartDataSource.CloseValues.ToList());
                double[] macdBuffer = (double[])res[0];
                double[] signalBuffer = (double[])res[1];
                double[] histogramBuffer = (double[])res[2];

                this.macdDataSource.Append(this.chartDataSource.XValues, macdBuffer);
                this.signalDataSource.Append(this.chartDataSource.XValues, signalBuffer);
                this.histogramDataSource.Append(this.chartDataSource.XValues, histogramBuffer);

                this.macdRenderableSeries.DataSeries = this.macdDataSource;
                this.signalRenderableSeries.DataSeries = this.signalDataSource;
                this.histogramRenderableSeries.DataSeries = this.histogramDataSource;

                this.macdChartSurface.RenderableSeries.Add(this.histogramRenderableSeries);
                this.macdChartSurface.RenderableSeries.Add(this.signalRenderableSeries);
                this.macdChartSurface.RenderableSeries.Add(this.macdRenderableSeries);

                this.macdZeroLineAnnotation = new HorizontalLineAnnotation();
                this.macdZeroLineAnnotation.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.macdZeroLineAnnotation.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
                this.macdZeroLineAnnotation.StrokeThickness = 0.5;
                this.macdZeroLineAnnotation.LabelPlacement = LabelPlacement.Axis;
                this.macdZeroLineAnnotation.ShowLabel = false;
                this.macdZeroLineAnnotation.Y1 = 0;
                this.macdChartSurface.Annotations.Add(this.macdZeroLineAnnotation);

                this.UpdateSCIChartThemeStyle(this.sciChartSelectedTheme);

                Binding backgroundBinding = new Binding("Background");
                backgroundBinding.Source = this.macdChartSurface;
                Binding macdForegroundBinding = new Binding("SeriesColor");
                macdForegroundBinding.Source = this.macdRenderableSeries;
                macdForegroundBinding.Converter = new ColorToBrushConverter();
                Binding signalForegroundBinding = new Binding("SeriesColor");
                signalForegroundBinding.Source = this.signalRenderableSeries;
                signalForegroundBinding.Converter = new ColorToBrushConverter();

                Label macdLabel = new Label();
                macdLabel.Content = "MACD (" + this.fastPeriod.ToString() + ", " + this.slowPeriod.ToString() + ")";
                macdLabel.FontSize = 13;
                macdLabel.FontWeight = FontWeights.SemiBold;
                macdLabel.SetBinding(Label.BackgroundProperty, backgroundBinding);
                macdLabel.SetBinding(Label.ForegroundProperty, macdForegroundBinding);

                Label signalLabel = new Label();
                signalLabel.Content = "Signal (" + this.signalPeriod.ToString() + ")";
                signalLabel.FontSize = 13;
                signalLabel.FontWeight = FontWeights.SemiBold;
                signalLabel.SetBinding(Label.BackgroundProperty, backgroundBinding);
                signalLabel.SetBinding(Label.ForegroundProperty, signalForegroundBinding);

                this.legendAnnotation = new LegendAnnotation(this.macdChartSurface);
                List<Label> labelList = new List<Label>();
                labelList.Add(macdLabel);
                labelList.Add(signalLabel);
                this.legendAnnotation.AddLabel(this.chartType, labelList);
            }
        }
        #endregion

        #region Override Methods
        public override void UpdateRenderSeries(bool addNewPoint, MarketPriceItem MktItems, DateTime time)
        {
            if ((this.macdChartSurface == null) || (this.macdIndicator == null) || (this.macdDataSource == null) || (this.signalDataSource == null) || (this.histogramDataSource == null))
            {
                return;
            }

            using (this.macdChartSurface.SuspendUpdates())
            {
                double yClose = TradeStationTools.ConvertToDouble(MktItems.Last);

                if (addNewPoint == true)
                {
                    ChartIndicators.Macdres res = this.macdIndicator.Push(yClose);
                    this.macdDataSource.Append(time, res.Macd);
                    this.signalDataSource.Append(time, res.Signal);
                    this.histogramDataSource.Append(time, res.MacdHistogram);
                }
                else
                {
                    ChartIndicators.Macdres res = this.macdIndicator.Update(yClose);
                    this.macdDataSource.Update(time, res.Macd);
                    this.signalDataSource.Update(time, res.Signal);
                    this.histogramDataSource.Update(time, res.MacdHistogram);
                }
            }
        }

        public override void UpdateSCIChartThemeStyle(String sciChartSelectedTheme)
        {
            this.sciChartSelectedTheme = sciChartSelectedTheme;

            if (this.macdChartSurface == null)
            {
                return;
            }

            using (this.macdChartSurface.SuspendUpdates())
            {
                if (this.sciChartSelectedTheme.Equals("Chrome") == true)
                {
                    this.macdChartSurface.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));

                    Style sciChartXAxisMajorGridLineStyle = new Style(typeof(Line));
                    sciChartXAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF2F2F2"))));
                    this.macdChartSurface.XAxis.MajorGridLineStyle = sciChartXAxisMajorGridLineStyle;

                    Style sciChartYAxisMajorGridLineStyle = new Style(typeof(Line));
                    sciChartYAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF2F2F2"))));
                    this.macdChartSurface.YAxis.MajorGridLineStyle = sciChartYAxisMajorGridLineStyle;

                    this.macdChartSurface.XAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
                    this.macdChartSurface.YAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
                }
                else
                {
                    this.macdChartSurface.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));

                    Style xAxisMajorGridLineStyle = new Style(typeof(Line));
                    xAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF404040"))));
                    this.macdChartSurface.XAxis.MajorGridLineStyle = xAxisMajorGridLineStyle;

                    Style yAxisMajorGridLineStyle = new Style(typeof(Line));
                    yAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF202020"))));
                    this.macdChartSurface.YAxis.MajorGridLineStyle = yAxisMajorGridLineStyle;

                    this.macdChartSurface.XAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                    this.macdChartSurface.YAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                }
            }
        }

        public override void RemoveFromGrid()
        {
            if ((this.macdRowDefinition == null) || (this.macdGrid == null) || (this.parent == null) || (this.macdChartSurface == null))
            {
                return;
            }

            if (this.parent.gridChartAnalysis != null)
            {
                using (this.macdChartSurface.SuspendUpdates())
                {
                    this.parent.gridChartAnalysis.Children.Remove(this.macdGrid);
                    this.parent.gridChartAnalysis.RowDefinitions.Remove(this.macdRowDefinition);
                }
            }
        }

        public override void AddToGrid()
        {
            if ((this.macdRowDefinition == null) || (this.macdGrid == null) || (this.parent == null) || (this.macdChartSurface == null))
            {
                return;
            }

            if (this.parent.gridChartAnalysis != null)
            {
                using (this.macdChartSurface.SuspendUpdates())
                {
                    this.parent.gridChartAnalysis.RowDefinitions.Add(this.macdRowDefinition);
                    Grid.SetRow(this.macdGrid, this.parent.gridChartAnalysis.Children.Count);
                    this.parent.gridChartAnalysis.Children.Add(this.macdGrid);
                }
            }
        }

        public override void ChartEnable()
        {
            if (this.macdChartSurface == null)
            {
                return;
            }

            this.macdChartSurface.IsEnabled = true;
        }

        public override void ChartDisable()
        {
            if (this.macdChartSurface == null)
            {
                return;
            }

            this.macdChartSurface.IsEnabled = false;
        }
        #endregion
    }
}
