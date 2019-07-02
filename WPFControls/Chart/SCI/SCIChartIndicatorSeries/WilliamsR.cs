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
        public class WilliamsR
        {
            private readonly uint williamsRPeriod;
            private readonly uint length;

            private List<double> highSource;
            private List<double> lowSource;
            private List<double> closeSource;

            public WilliamsR(uint period)
            {
                williamsRPeriod = period;

                length = (uint)Core.WillRLookback((int)williamsRPeriod);

                highSource = new List<double>();
                lowSource = new List<double>();
                closeSource = new List<double>();
            }

            public double Update(double high, double low, double close)
            {
                this.highSource[this.highSource.Count - 1] = high;
                this.lowSource[this.lowSource.Count - 1] = low;
                this.closeSource[this.closeSource.Count - 1] = close;

                int sourceCount = this.closeSource.Count();
                double[] outWilliamsR = new double[sourceCount + 1];
                int outBegIdx;
                int outNBElement;
                int endIdx = sourceCount - 1;
                double[] highSourceArray = this.highSource.ToArray();
                double[] lowSourceArray = this.lowSource.ToArray();
                double[] closeSourceArray = this.closeSource.ToArray();

                Core.RetCode res = Core.WillR(0, endIdx, highSourceArray, lowSourceArray, closeSourceArray, (int)this.williamsRPeriod, out outBegIdx, out outNBElement, outWilliamsR);

                double williamsRRes = double.NaN;

                if (res == Core.RetCode.Success)
                {
                    if (outNBElement > 0) { outNBElement--; }
                    williamsRRes = outWilliamsR[outNBElement];
                }

                return williamsRRes;
            }

            public double Push(double high, double low, double close)
            {
                this.highSource.Add(high);
                this.lowSource.Add(low);
                this.closeSource.Add(close);

                int sourceCount = this.closeSource.Count();
                double[] outWilliamsR = new double[sourceCount + 1];
                int outBegIdx;
                int outNBElement;
                int endIdx = sourceCount - 1;
                double[] highSourceArray = highSource.ToArray();
                double[] lowSourceArray = lowSource.ToArray();
                double[] closeSourceArray = closeSource.ToArray();

                Core.RetCode res = Core.WillR(0, endIdx, highSourceArray, lowSourceArray, closeSourceArray, (int)this.williamsRPeriod, out outBegIdx, out outNBElement, outWilliamsR);

                double williamsRRes = double.NaN;

                if (res == Core.RetCode.Success)
                {
                    if (outNBElement > 0) { outNBElement--; }
                    williamsRRes = outWilliamsR[outNBElement];
                }

                return williamsRRes;
            }

            public double[] Push(List<double> high, List<double> low, List<double> close)
            {
                this.highSource.AddRange(high);
                this.lowSource.AddRange(low);
                this.closeSource.AddRange(close);

                int sourceCount = this.closeSource.Count();
                double[] outWilliamsR = new double[sourceCount + 1];
                int outBegIdx;
                int outNBElement;
                int endIdx = sourceCount - 1;
                double[] highSourceArray = highSource.ToArray();
                double[] lowSourceArray = lowSource.ToArray();
                double[] closeSourceArray = closeSource.ToArray();

                Core.RetCode res = Core.WillR(0, endIdx, highSourceArray, lowSourceArray, closeSourceArray, (int)this.williamsRPeriod, out outBegIdx, out outNBElement, outWilliamsR);

                List<double> outWilliamsR1 = new List<double>();

                if (res == Core.RetCode.Success)
                {
                    for (int i = 0; i < outBegIdx; i++)
                    {
                        outWilliamsR1.Add(double.NaN);
                    }

                    for (int i = 0; i < outNBElement; i++)
                    {
                        outWilliamsR1.Add(outWilliamsR[i]);
                    }
                }

                return outWilliamsR1.ToArray();
            }
        }
    }

    class WilliamsR : ChartIndicatorBase
    {
        private readonly SCIChartAnalysis parent;
        private readonly OhlcDataSeries<DateTime, double> chartDataSource;
        private readonly uint chartPeriod;
        private readonly uint williamsRPeriod;
        private readonly int williamsROverBoughtLine;
        private readonly int williamsROverSoldLine;
        private readonly int williamsRMidLine;
        private readonly String williamsRColour;
        private readonly String williamsROverBoughtLineColour;
        private readonly String williamsROverSoldLineColour;
        private readonly String williamsRMidLineColour;

        private String sciChartSelectedTheme;

        private Grid _williamsRGrid;
        private RowDefinition _williamsRRowDefinition;
        private SciChartSurface _williamsRChartSurface;
        private GridSplitter _williamsRGridSplitter;
        private ChartIndicators.WilliamsR williamsRIndicator;
        private XyDataSeries<DateTime, double> _williamsRDataSource;
        private FastLineRenderableSeries _williamsRRenderableSeries;
        private HorizontalLineAnnotation _williamsROverBoughtLineAnnotation;
        private HorizontalLineAnnotation _williamsROverSoldLineAnnotation;
        private HorizontalLineAnnotation _williamsRMidLineAnnotation;
        private LegendAnnotation _legendAnnotation;

        public WilliamsR(SCIConstants.ChartIndicatorType type, SCIChartAnalysis parent, OhlcDataSeries<DateTime, double> chartDataSource, uint chartPeriod, uint williamsRPeriod, int williamsROverBoughtLine, int williamsROverSoldLine, int williamsRMidLine, String williamsRColour, String williamsROverBoughtLineColour, String williamsROverSoldLineColour, String williamsRMidLineColour, String sciChartSelectedTheme)
        {
            this.chartType = type;
            this.parent = parent;
            this.chartDataSource = chartDataSource;
            this.chartPeriod = chartPeriod;
            this.williamsRPeriod = williamsRPeriod;
            this.williamsROverBoughtLine = williamsROverBoughtLine;
            this.williamsROverSoldLine = williamsROverSoldLine;
            this.williamsRMidLine = williamsRMidLine;
            this.williamsRColour = williamsRColour;
            this.williamsROverBoughtLineColour = williamsROverBoughtLineColour;
            this.williamsROverSoldLineColour = williamsROverSoldLineColour;
            this.williamsRMidLineColour = williamsRMidLineColour;

            this.sciChartSelectedTheme = sciChartSelectedTheme;

            this.CreateRenderSeries();
        }

        protected override void Dispose(bool disposing)
        {
            this.legendAnnotation = null;
            this.williamsROverBoughtLineAnnotation = null;
            this.williamsROverSoldLineAnnotation = null;
            this.williamsRMidLineAnnotation = null;
            this.williamsRRenderableSeries = null;
            this.williamsRDataSource = null;
            this.williamsRIndicator = null;
            this.williamsRGridSplitter = null;
            this.williamsRChartSurface = null;
            this.williamsRRowDefinition = null;
            this.williamsRGrid = null;
        }

        #region properties
        public XyDataSeries<DateTime, double> williamsRDataSource
        {
            get { return _williamsRDataSource; }
            set
            {
                if (this.williamsRRenderableSeries != null)
                {
                    this.williamsRRenderableSeries.DataSeries = null;
                }

                _williamsRDataSource = value;
            }
        }

        public FastLineRenderableSeries williamsRRenderableSeries
        {
            get { return _williamsRRenderableSeries; }
            set
            {
                if (_williamsRRenderableSeries != null)
                {
                    _williamsRRenderableSeries.DataSeries = null;

                    if (this.williamsRChartSurface != null)
                    {
                        using (this.williamsRChartSurface.SuspendUpdates())
                        {
                            this.williamsRChartSurface.RenderableSeries.Remove(_williamsRRenderableSeries);
                        }
                    }
                }

                _williamsRRenderableSeries = value;
            }
        }

        public RowDefinition williamsRRowDefinition
        {
            get { return _williamsRRowDefinition; }
            set
            {
                if (_williamsRRowDefinition != null)
                {
                    if (this.parent != null)
                    {
                        if (this.parent.gridChartAnalysis != null)
                        {
                            this.parent.gridChartAnalysis.RowDefinitions.Remove(_williamsRRowDefinition);
                        }
                    }
                }

                _williamsRRowDefinition = value;
            }
        }

        private SciChartSurface williamsRChartSurface
        {
            get { return _williamsRChartSurface; }
            set
            {
                if (_williamsRChartSurface != null)
                {
                    using (_williamsRChartSurface.SuspendUpdates())
                    {
                        if (this.williamsRGrid != null)
                        {
                            this.williamsRGrid.Children.Remove(_williamsRChartSurface);
                        }
                    }
                }

                _williamsRChartSurface = value;
            }
        }

        private HorizontalLineAnnotation williamsROverBoughtLineAnnotation
        {
            get { return _williamsROverBoughtLineAnnotation; }
            set
            {
                if (_williamsROverBoughtLineAnnotation != null)
                {
                    if (this.williamsRChartSurface != null)
                    {
                        using (this.williamsRChartSurface.SuspendUpdates())
                        {
                            this.williamsRChartSurface.Annotations.Remove(_williamsROverBoughtLineAnnotation);
                        }
                    }
                }

                _williamsROverBoughtLineAnnotation = value;
            }
        }

        private HorizontalLineAnnotation williamsROverSoldLineAnnotation
        {
            get { return _williamsROverSoldLineAnnotation; }
            set
            {
                if (_williamsROverSoldLineAnnotation != null)
                {
                    if (this.williamsRChartSurface != null)
                    {
                        using (this.williamsRChartSurface.SuspendUpdates())
                        {
                            this.williamsRChartSurface.Annotations.Remove(_williamsROverSoldLineAnnotation);
                        }
                    }
                }

                _williamsROverSoldLineAnnotation = value;
            }
        }

        private HorizontalLineAnnotation williamsRMidLineAnnotation
        {
            get { return _williamsRMidLineAnnotation; }
            set
            {
                if (_williamsRMidLineAnnotation != null)
                {
                    if (this.williamsRChartSurface != null)
                    {
                        using (this.williamsRChartSurface.SuspendUpdates())
                        {
                            this.williamsRChartSurface.Annotations.Remove(_williamsRMidLineAnnotation);
                        }
                    }
                }

                _williamsRMidLineAnnotation = value;
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

        private Grid williamsRGrid
        {
            get { return _williamsRGrid; }
            set
            {
                if (_williamsRGrid != null)
                {
                    if (this.parent != null)
                    {
                        if (this.parent.gridChartAnalysis != null)
                        {
                            this.parent.gridChartAnalysis.Children.Remove(_williamsRGrid);
                        }
                    }

                    _williamsRGrid.Children.Clear();
                }

                _williamsRGrid = value;
            }
        }

        private GridSplitter williamsRGridSplitter
        {
            get { return _williamsRGridSplitter; }
            set
            {
                if (_williamsRGridSplitter != null)
                {
                    if (this.williamsRGrid != null)
                    {
                        this.williamsRGrid.Children.Remove(_williamsRGridSplitter);
                    }
                }

                _williamsRGridSplitter = value;
            }
        }
        #endregion

        #region Internal methods
        private void CreateRenderSeries()
        {
            if ((this.parent == null) || (this.chartDataSource == null) || (this.williamsRPeriod == 0))
            {
                return;
            }

            this.williamsRChartSurface = new SciChartSurface();

            using (this.williamsRChartSurface.SuspendUpdates())
            {
                SciChartGroup.SetVerticalChartGroup(this.williamsRChartSurface, "myCharts");
                this.williamsRChartSurface.Padding = new Thickness(5, 5, 15, 5);
                this.williamsRChartSurface.RenderPriority = RenderPriority.Low;

                CategoryDateTimeAxis williamsRXAxis = new CategoryDateTimeAxis();
                Binding visibleRangeBinding = new Binding("XAxis.VisibleRange");
                visibleRangeBinding.Source = this.parent.sciChart;
                williamsRXAxis.SetBinding(AxisBase.VisibleRangeProperty, visibleRangeBinding);
                williamsRXAxis.LabelFormatter = SCICommon.CreateTimeAxisFormatter(this.chartPeriod);
                williamsRXAxis.DrawLabels = false;
                williamsRXAxis.DrawMajorBands = false;
                williamsRXAxis.DrawMajorGridLines = true;
                williamsRXAxis.DrawMajorTicks = false;
                williamsRXAxis.DrawMinorGridLines = false;
                williamsRXAxis.DrawMinorTicks = false;
                this.williamsRChartSurface.XAxis = williamsRXAxis;

                NumericAxis williamsRYAxis = new NumericAxis();
                williamsRYAxis.DrawMajorGridLines = true;
                williamsRYAxis.DrawMinorGridLines = false;
                williamsRYAxis.DrawMajorTicks = false;
                williamsRYAxis.DrawMinorTicks = false;
                williamsRYAxis.DrawLabels = true;
                williamsRYAxis.MajorDelta = 10;
                williamsRYAxis.AutoRange = AutoRange.Never;
                williamsRYAxis.VisibleRange = new DoubleRange(-100, 0);
                williamsRYAxis.GrowBy = new DoubleRange(0.05, 0.05);
                this.williamsRChartSurface.YAxis = williamsRYAxis;

                ZoomPanModifier williamsRZoomPanModifier = new ZoomPanModifier();
                williamsRZoomPanModifier.IsEnabled = false;
                RubberBandXyZoomModifier williamsRRubberBandXyZoomModifier = new RubberBandXyZoomModifier();
                williamsRRubberBandXyZoomModifier.IsEnabled = false;
                MouseWheelZoomModifier williamsRMouseWheelZoomModifier = new MouseWheelZoomModifier();
                williamsRMouseWheelZoomModifier.IsEnabled = false;
                CursorModifier williamsRCursorModifier = new CursorModifier();
                Binding williamsRCursorModifieIsEnabledBinding = new Binding("IsMouseCursorEnabled");
                williamsRCursorModifieIsEnabledBinding.Source = this.parent;
                williamsRCursorModifier.SetBinding(CursorModifier.IsEnabledProperty, williamsRCursorModifieIsEnabledBinding);
                williamsRCursorModifier.ShowAxisLabels = true;
                williamsRCursorModifier.ShowTooltip = true;
                williamsRCursorModifier.ReceiveHandledEvents = true;
                this.williamsRChartSurface.ChartModifier = new ModifierGroup(williamsRZoomPanModifier, williamsRRubberBandXyZoomModifier, williamsRMouseWheelZoomModifier, williamsRCursorModifier);
                MouseManager.SetMouseEventGroup((ModifierGroup)this.williamsRChartSurface.ChartModifier, "MySharedMouseGroup");

                this.williamsRGrid = new Grid();

                this.williamsRGridSplitter = new GridSplitter();
                this.williamsRGridSplitter.IsEnabled = false;
                this.williamsRGridSplitter.Background = Brushes.Gray;
                this.williamsRGridSplitter.Width = double.NaN;
                this.williamsRGridSplitter.Height = 1;
                this.williamsRGridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.williamsRGridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
                this.williamsRGridSplitter.Margin = new Thickness(0);

                RowDefinition gridSplitterRow = new RowDefinition();
                gridSplitterRow.Height = new GridLength(1, GridUnitType.Pixel);
                this.williamsRGrid.RowDefinitions.Add(gridSplitterRow);
                Grid.SetRow(this.williamsRGridSplitter, this.williamsRGrid.Children.Count);
                this.williamsRGrid.Children.Add(this.williamsRGridSplitter);

                RowDefinition chartSurfaceRow = new RowDefinition();
                chartSurfaceRow.Height = new GridLength(1, GridUnitType.Star);
                this.williamsRGrid.RowDefinitions.Add(chartSurfaceRow);
                Grid.SetRow(this.williamsRChartSurface, this.williamsRGrid.Children.Count);
                this.williamsRGrid.Children.Add(this.williamsRChartSurface);

                this.williamsRRowDefinition = new RowDefinition();
                this.williamsRRowDefinition.Name = SCIConstants.SCI_WILLIAMSR_ROW_DEFINITION;
                this.williamsRRowDefinition.Height = new GridLength(2, GridUnitType.Star);

                this.parent.gridChartAnalysis.RowDefinitions.Add(this.williamsRRowDefinition);
                Grid.SetRow(this.williamsRGrid, this.parent.gridChartAnalysis.Children.Count);
                this.parent.gridChartAnalysis.Children.Add(this.williamsRGrid);

                this.williamsRRenderableSeries = new FastLineRenderableSeries();

                this.williamsRRenderableSeries.SeriesColor = (Color)ColorConverter.ConvertFromString(this.williamsRColour);
                this.williamsRRenderableSeries.StrokeThickness = 1;
                this.williamsRRenderableSeries.AntiAliasing = false;
                this.williamsRRenderableSeries.SnapsToDevicePixels = true;

                this.williamsRDataSource = new XyDataSeries<DateTime, double>();

                this.williamsRIndicator = new ChartIndicators.WilliamsR(this.williamsRPeriod);

                double[] williamsRBuffer = this.williamsRIndicator.Push(this.chartDataSource.HighValues.ToList(), this.chartDataSource.LowValues.ToList(), this.chartDataSource.CloseValues.ToList());
                this.williamsRDataSource.Append(this.chartDataSource.XValues, williamsRBuffer);
                this.williamsRRenderableSeries.DataSeries = this.williamsRDataSource;
                this.williamsRChartSurface.RenderableSeries.Add(this.williamsRRenderableSeries);
                this.UpdateSCIChartThemeStyle(this.sciChartSelectedTheme);

                this.williamsROverBoughtLineAnnotation = new HorizontalLineAnnotation();
                this.williamsROverBoughtLineAnnotation.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.williamsROverBoughtLineAnnotation.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.williamsROverBoughtLineColour));
                this.williamsROverBoughtLineAnnotation.StrokeThickness = 0.5;
                this.williamsROverBoughtLineAnnotation.LabelPlacement = LabelPlacement.Axis;
                this.williamsROverBoughtLineAnnotation.ShowLabel = false;
                this.williamsROverBoughtLineAnnotation.Y1 = this.williamsROverBoughtLine;
                this.williamsRChartSurface.Annotations.Add(this.williamsROverBoughtLineAnnotation);

                this.williamsROverSoldLineAnnotation = new HorizontalLineAnnotation();
                this.williamsROverSoldLineAnnotation.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.williamsROverSoldLineAnnotation.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.williamsROverSoldLineColour));
                this.williamsROverSoldLineAnnotation.StrokeThickness = 0.5;
                this.williamsROverSoldLineAnnotation.LabelPlacement = LabelPlacement.Axis;
                this.williamsROverSoldLineAnnotation.ShowLabel = false;
                this.williamsROverSoldLineAnnotation.Y1 = this.williamsROverSoldLine;
                this.williamsRChartSurface.Annotations.Add(this.williamsROverSoldLineAnnotation);

                this.williamsRMidLineAnnotation = new HorizontalLineAnnotation();
                this.williamsRMidLineAnnotation.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.williamsRMidLineAnnotation.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.williamsRMidLineColour));
                this.williamsRMidLineAnnotation.StrokeThickness = 0.5;
                this.williamsRMidLineAnnotation.LabelPlacement = LabelPlacement.Axis;
                this.williamsRMidLineAnnotation.ShowLabel = false;
                this.williamsRMidLineAnnotation.Y1 = this.williamsRMidLine;
                this.williamsRChartSurface.Annotations.Add(this.williamsRMidLineAnnotation);

                Binding backgroundBinding = new Binding("Background");
                backgroundBinding.Source = this.williamsRChartSurface;
                Binding williamsRForegroundBinding = new Binding("SeriesColor");
                williamsRForegroundBinding.Source = this.williamsRRenderableSeries;
                williamsRForegroundBinding.Converter = new ColorToBrushConverter();

                Label williamsRLabel = new Label();
                williamsRLabel.Content = "Williams %R (" + this.williamsRPeriod.ToString() + ")";
                williamsRLabel.FontSize = 13;
                williamsRLabel.FontWeight = FontWeights.SemiBold;
                williamsRLabel.SetBinding(Label.BackgroundProperty, backgroundBinding);
                williamsRLabel.SetBinding(Label.ForegroundProperty, williamsRForegroundBinding);

                this.legendAnnotation = new LegendAnnotation(this.williamsRChartSurface);
                List<Label> labelList = new List<Label>();
                labelList.Add(williamsRLabel);
                this.legendAnnotation.AddLabel(this.chartType, labelList); 
            }
        }
        #endregion
  
        #region Override Methods
        public override void UpdateRenderSeries(bool addNewPoint, MarketPriceItem MktItems, DateTime time)
        {
            if ((this.williamsRChartSurface == null) || (this.williamsRIndicator == null) || (this.williamsRDataSource == null))
            {
                return;
            }

            using (this.williamsRChartSurface.SuspendUpdates())
            {
                double yHigh = TradeStationTools.ConvertToDouble(MktItems.High);
                double yLow = TradeStationTools.ConvertToDouble(MktItems.Low);
                double yClose = TradeStationTools.ConvertToDouble(MktItems.Last);

                if (addNewPoint == true)
                {
                    double res = this.williamsRIndicator.Push(yHigh, yLow, yClose);
                    this.williamsRDataSource.Append(time, res);
                }
                else
                {
                    double res = this.williamsRIndicator.Update(yHigh, yLow, yClose);
                    this.williamsRDataSource.Update(time, res);
                }
            }
        }
 
        public override void UpdateSCIChartThemeStyle(String sciChartSelectedTheme)
        {
            this.sciChartSelectedTheme = sciChartSelectedTheme;

            if (this.williamsRChartSurface == null)
            {
                return;
            }

            using (this.williamsRChartSurface.SuspendUpdates())
            {
                if (this.sciChartSelectedTheme.Equals("Chrome") == true)
                {
                    this.williamsRChartSurface.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));

                    Style sciChartXAxisMajorGridLineStyle = new Style(typeof(Line));
                    sciChartXAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"))));
                    this.williamsRChartSurface.XAxis.MajorGridLineStyle = sciChartXAxisMajorGridLineStyle;

                    Style sciChartYAxisMajorGridLineStyle = new Style(typeof(Line));
                    sciChartYAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF6F6F6"))));
                    this.williamsRChartSurface.YAxis.MajorGridLineStyle = sciChartYAxisMajorGridLineStyle;

                    this.williamsRChartSurface.XAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
                    this.williamsRChartSurface.YAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
                }
                else
                {
                    this.williamsRChartSurface.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));

                    Style xAxisMajorGridLineStyle = new Style(typeof(Line));
                    xAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF404040"))));
                    this.williamsRChartSurface.XAxis.MajorGridLineStyle = xAxisMajorGridLineStyle;

                    Style yAxisMajorGridLineStyle = new Style(typeof(Line));
                    yAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF202020"))));
                    this.williamsRChartSurface.YAxis.MajorGridLineStyle = yAxisMajorGridLineStyle;

                    this.williamsRChartSurface.XAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                    this.williamsRChartSurface.YAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                }
            }
        }

        public override void RemoveFromGrid()
        {
            if ((this.williamsRRowDefinition == null) || (this.williamsRGrid == null) || (this.parent == null) || (this.williamsRChartSurface == null))
            {
                return;
            }

            if (this.parent.gridChartAnalysis != null)
            {
                using (this.williamsRChartSurface.SuspendUpdates())
                {
                    this.parent.gridChartAnalysis.Children.Remove(this.williamsRGrid);
                    this.parent.gridChartAnalysis.RowDefinitions.Remove(this.williamsRRowDefinition);
                }
            }
        }

        public override void AddToGrid()
        {
            if ((this.williamsRRowDefinition == null) || (this.williamsRGrid == null) || (this.parent == null) || (this.williamsRChartSurface == null))
            {
                return;
            }

            if (this.parent.gridChartAnalysis != null)
            {
                using (this.williamsRChartSurface.SuspendUpdates())
                {
                    this.parent.gridChartAnalysis.RowDefinitions.Add(this.williamsRRowDefinition);
                    Grid.SetRow(this.williamsRGrid, this.parent.gridChartAnalysis.Children.Count);
                    this.parent.gridChartAnalysis.Children.Add(this.williamsRGrid);
                }
            }
        }

        public override void ChartEnable()
        {
            if (this.williamsRChartSurface == null)
            {
                return;
            }

            this.williamsRChartSurface.IsEnabled = true;
        }

        public override void ChartDisable()
        {
            if (this.williamsRChartSurface == null)
            {
                return;
            }

            this.williamsRChartSurface.IsEnabled = false;
        }
        #endregion
    }
}
