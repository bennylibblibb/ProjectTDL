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
        public class RSI
        {
            private readonly uint rsiPeriod;
            private readonly uint length;

            private List<double> closeSource;

            public RSI(uint period)
            {
                rsiPeriod = period;

                length = (uint)Core.RsiLookback((int)rsiPeriod);
                closeSource = new List<double>();
            }

            public double Update(double value)
            {
                this.closeSource[this.closeSource.Count - 1] = value;

                int sourceCount = this.closeSource.Count();
                double[] outRSI = new double[sourceCount + 1];
                int outBegIdx;
                int outNBElement;

                double rsiRes = double.NaN;

                int endIdx = sourceCount - 1;
                double[] closeSourceArray = this.closeSource.ToArray();
                Core.RetCode res = Core.Rsi(0, endIdx, closeSourceArray, (int)this.rsiPeriod, out outBegIdx, out outNBElement, outRSI);

                if (res == Core.RetCode.Success)
                {
                    if (outNBElement > 0) { outNBElement--; }
                    rsiRes = outRSI[outNBElement];
                }

                return rsiRes;
            }

            public double Push(double value)
            {
                this.closeSource.Add(value);

                int sourceCount = this.closeSource.Count();
                double[] outRSI = new double[sourceCount + 1];
                int outBegIdx;
                int outNBElement;

                double rsiRes = double.NaN;

                int endIdx = sourceCount - 1;
                double[] closeSourceArray = this.closeSource.ToArray();
                Core.RetCode res = Core.Rsi(0, endIdx, closeSourceArray, (int)this.rsiPeriod, out outBegIdx, out outNBElement, outRSI);

                if (res == Core.RetCode.Success)
                {
                    if (outNBElement > 0) { outNBElement--; }
                    rsiRes = outRSI[outNBElement];
                }

                return rsiRes;
            }

            public double[] Push(List<double> values)
            {
                this.closeSource.AddRange(values);

                int sourceCount = this.closeSource.Count();
                double[] outRSI = new double[sourceCount + 1];
                int outBegIdx;
                int outNBElement;

                int endIdx = sourceCount - 1;
                double[] closeSourceArray = this.closeSource.ToArray();
                Core.RetCode res = Core.Rsi(0, endIdx, closeSourceArray, (int)this.rsiPeriod, out outBegIdx, out outNBElement, outRSI);

                List<double> outRSI1 = new List<double>();

                if (res == Core.RetCode.Success)
                {
                    for (int i = 0; i < outBegIdx; i++)
                    {
                        outRSI1.Add(double.NaN);
                    }

                    for (int i = 0; i < outNBElement; i++)
                    {
                        outRSI1.Add(outRSI[i]);
                    }
                }

                return outRSI1.ToArray();
            }
        }
    }

    class RSI : ChartIndicatorBase
    {
        private readonly SCIChartAnalysis parent;
        private readonly OhlcDataSeries<DateTime, double> chartDataSource;
        private readonly uint chartPeriod;
        private readonly uint rsiPeriod;
        private readonly uint rsiOverBoughtLine;
        private readonly uint rsiOverSoldLine;
        private readonly uint rsiMidLine;
        private readonly String rsiColour;
        private readonly String rsiOverBoughtLineColour;
        private readonly String rsiOverSoldLineColour;
        private readonly String rsiMidLineColour;

        private String sciChartSelectedTheme;

        private Grid _rsiGrid;
        private RowDefinition _rsiRowDefinition;
        private SciChartSurface _rsiChartSurface;
        private GridSplitter _rsiGridSplitter;
        private ChartIndicators.RSI rsiIndicator;
        private XyDataSeries<DateTime, double> _rsiDataSource;
        private FastLineRenderableSeries _rsiRenderableSeries;
        private HorizontalLineAnnotation _rsiOverBoughtLineAnnotation;
        private HorizontalLineAnnotation _rsiOverSoldLineAnnotation;
        private HorizontalLineAnnotation _rsiMidLineAnnotation;
        private LegendAnnotation _legendAnnotation;

        public RSI(SCIConstants.ChartIndicatorType type, SCIChartAnalysis parent, OhlcDataSeries<DateTime, double> chartDataSource, uint chartPeriod, uint rsiPeriod, uint rsiOverBoughtLine, uint rsiOverSoldLine, uint rsiMidLine, String rsiColour, String rsiOverBoughtLineColour, String rsiOverSoldLineColour, String rsiMidLineColour, String sciChartSelectedTheme)
        {
            this.chartType = type;
            this.parent = parent;
            this.chartDataSource = chartDataSource;
            this.chartPeriod = chartPeriod;
            this.rsiPeriod = rsiPeriod;
            this.rsiOverBoughtLine = rsiOverBoughtLine;
            this.rsiOverSoldLine = rsiOverSoldLine;
            this.rsiMidLine = rsiMidLine;
            this.rsiColour = rsiColour;
            this.rsiOverBoughtLineColour = rsiOverBoughtLineColour;
            this.rsiOverSoldLineColour = rsiOverSoldLineColour;
            this.rsiMidLineColour = rsiMidLineColour;

            this.sciChartSelectedTheme = sciChartSelectedTheme;

            this.CreateRenderSeries();
        }

        protected override void Dispose(bool disposing)
        {
            this.rsiOverBoughtLineAnnotation = null;
            this.rsiOverSoldLineAnnotation = null;
            this.rsiMidLineAnnotation = null;
            this.legendAnnotation = null;
            this.rsiRenderableSeries = null;
            this.rsiDataSource = null;
            this.rsiIndicator = null;
            this.rsiGridSplitter = null;
            this.rsiChartSurface = null;
            this.rsiRowDefinition = null;
            this.rsiGrid = null;
        }

        #region properties
        public XyDataSeries<DateTime, double> rsiDataSource
        {
            get { return _rsiDataSource; }
            set
            {
                if (this.rsiRenderableSeries != null)
                {
                    this.rsiRenderableSeries.DataSeries = null;
                }

                _rsiDataSource = value;
            }
        }

        public FastLineRenderableSeries rsiRenderableSeries
        {
            get { return _rsiRenderableSeries; }
            set
            {
                if (_rsiRenderableSeries != null)
                {
                    _rsiRenderableSeries.DataSeries = null;

                    if (this.rsiChartSurface != null)
                    {
                        using (this.rsiChartSurface.SuspendUpdates())
                        {
                            this.rsiChartSurface.RenderableSeries.Remove(_rsiRenderableSeries);
                        }
                    }
                }

                _rsiRenderableSeries = value;
            }
        }

        public RowDefinition rsiRowDefinition
        {
            get { return _rsiRowDefinition; }
            set
            {
                if (_rsiRowDefinition != null)
                {
                    if (this.parent != null)
                    {
                        if (this.parent.gridChartAnalysis != null)
                        {
                            this.parent.gridChartAnalysis.RowDefinitions.Remove(_rsiRowDefinition);
                        }
                    }
                }

                _rsiRowDefinition = value;
            }
        }

        private SciChartSurface rsiChartSurface
        {
            get { return _rsiChartSurface; }
            set
            {
                if (_rsiChartSurface != null)
                {
                    using (_rsiChartSurface.SuspendUpdates())
                    {
                        if (this.rsiGrid != null)
                        {
                            this.rsiGrid.Children.Remove(_rsiChartSurface);
                        }
                    }
                }

                _rsiChartSurface = value;
            }
        }

        private HorizontalLineAnnotation rsiOverBoughtLineAnnotation
        {
            get { return _rsiOverBoughtLineAnnotation; }
            set
            {
                if (_rsiOverBoughtLineAnnotation != null)
                {
                    if (this.rsiChartSurface != null)
                    {
                        using (this.rsiChartSurface.SuspendUpdates())
                        {
                            this.rsiChartSurface.Annotations.Remove(_rsiOverBoughtLineAnnotation);
                        }
                    }
                }

                _rsiOverBoughtLineAnnotation = value;
            }
        }

        private HorizontalLineAnnotation rsiOverSoldLineAnnotation
        {
            get { return _rsiOverSoldLineAnnotation; }
            set
            {
                if (_rsiOverSoldLineAnnotation != null)
                {
                    if (this.rsiChartSurface != null)
                    {
                        using (this.rsiChartSurface.SuspendUpdates())
                        {
                            this.rsiChartSurface.Annotations.Remove(_rsiOverSoldLineAnnotation);
                        }
                    }
                }

                _rsiOverSoldLineAnnotation = value;
            }
        }

        private HorizontalLineAnnotation rsiMidLineAnnotation
        {
            get { return _rsiMidLineAnnotation; }
            set
            {
                if (_rsiMidLineAnnotation != null)
                {
                    if (this.rsiChartSurface != null)
                    {
                        using (this.rsiChartSurface.SuspendUpdates())
                        {
                            this.rsiChartSurface.Annotations.Remove(_rsiMidLineAnnotation);
                        }
                    }
                }

                _rsiMidLineAnnotation = value;
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

        private Grid rsiGrid
        {
            get { return _rsiGrid; }
            set
            {
                if (_rsiGrid != null)
                {
                    if (this.parent != null)
                    {
                        if (this.parent.gridChartAnalysis != null)
                        {
                            this.parent.gridChartAnalysis.Children.Remove(_rsiGrid);
                        }
                    }

                    _rsiGrid.Children.Clear();
                }

                _rsiGrid = value;
            }
        }

        private GridSplitter rsiGridSplitter
        {
            get { return _rsiGridSplitter; }
            set
            {
                if (_rsiGridSplitter != null)
                {
                    if (this.rsiGrid != null)
                    {
                        this.rsiGrid.Children.Remove(_rsiGridSplitter);
                    }
                }

                _rsiGridSplitter = value;
            }
        }
        #endregion

        #region Internal methods
        private void CreateRenderSeries()
        {
            if ((this.parent == null) || (this.chartDataSource == null) || (this.rsiPeriod <= 0))
            {
                return;
            }

            this.rsiChartSurface = new SciChartSurface();

            using (this.rsiChartSurface.SuspendUpdates())
            {
                SciChartGroup.SetVerticalChartGroup(this.rsiChartSurface, "myCharts");
                this.rsiChartSurface.Padding = new Thickness(5, 5, 15, 5);
                this.rsiChartSurface.RenderPriority = RenderPriority.Low;

                CategoryDateTimeAxis rsiXAxis = new CategoryDateTimeAxis();
                Binding visibleRangeBinding = new Binding("XAxis.VisibleRange");
                visibleRangeBinding.Source = this.parent.sciChart;
                rsiXAxis.SetBinding(AxisBase.VisibleRangeProperty, visibleRangeBinding);
                rsiXAxis.LabelFormatter = SCICommon.CreateTimeAxisFormatter(this.chartPeriod);
                rsiXAxis.DrawLabels = false;
                rsiXAxis.DrawMajorBands = false;
                rsiXAxis.DrawMajorGridLines = true;
                rsiXAxis.DrawMajorTicks = false;
                rsiXAxis.DrawMinorGridLines = false;
                rsiXAxis.DrawMinorTicks = false;
                this.rsiChartSurface.XAxis = rsiXAxis;

                NumericAxis rsiYAxis = new NumericAxis();
                rsiYAxis.DrawMajorGridLines = true;
                rsiYAxis.DrawMinorGridLines = false;
                rsiYAxis.DrawMajorTicks = false;
                rsiYAxis.DrawMinorTicks = false;
                rsiYAxis.DrawLabels = true;
                rsiYAxis.MajorDelta = 10;
                rsiYAxis.AutoRange = AutoRange.Never;
                rsiYAxis.VisibleRange = new DoubleRange(0, 100);
                rsiYAxis.GrowBy = new DoubleRange(0.05, 0.05);
                this.rsiChartSurface.YAxis = rsiYAxis;

                ZoomPanModifier rsiZoomPanModifier = new ZoomPanModifier();
                rsiZoomPanModifier.IsEnabled = false;
                RubberBandXyZoomModifier rsiRubberBandXyZoomModifier = new RubberBandXyZoomModifier();
                rsiRubberBandXyZoomModifier.IsEnabled = false;
                MouseWheelZoomModifier rsiMouseWheelZoomModifier = new MouseWheelZoomModifier();
                rsiMouseWheelZoomModifier.IsEnabled = false;
                CursorModifier rsiCursorModifier = new CursorModifier();
                Binding rsiCursorModifieIsEnabledBinding = new Binding("IsMouseCursorEnabled");
                rsiCursorModifieIsEnabledBinding.Source = this.parent;
                rsiCursorModifier.SetBinding(CursorModifier.IsEnabledProperty, rsiCursorModifieIsEnabledBinding);
                rsiCursorModifier.ShowAxisLabels = true;
                rsiCursorModifier.ShowTooltip = true;
                rsiCursorModifier.ReceiveHandledEvents = true;
                this.rsiChartSurface.ChartModifier = new ModifierGroup(rsiZoomPanModifier, rsiRubberBandXyZoomModifier, rsiMouseWheelZoomModifier, rsiCursorModifier);
                MouseManager.SetMouseEventGroup((ModifierGroup)this.rsiChartSurface.ChartModifier, "MySharedMouseGroup");

                this.rsiGrid = new Grid();

                this.rsiGridSplitter = new GridSplitter();
                this.rsiGridSplitter.IsEnabled = false;
                this.rsiGridSplitter.Background = Brushes.Gray;
                this.rsiGridSplitter.Width = double.NaN;
                this.rsiGridSplitter.Height = 1;
                this.rsiGridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.rsiGridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
                this.rsiGridSplitter.Margin = new Thickness(0);

                RowDefinition gridSplitterRow = new RowDefinition();
                gridSplitterRow.Height = new GridLength(1, GridUnitType.Pixel);
                this.rsiGrid.RowDefinitions.Add(gridSplitterRow);
                Grid.SetRow(this.rsiGridSplitter, this.rsiGrid.Children.Count);
                this.rsiGrid.Children.Add(this.rsiGridSplitter);

                RowDefinition chartSurfaceRow = new RowDefinition();
                chartSurfaceRow.Height = new GridLength(1, GridUnitType.Star);
                this.rsiGrid.RowDefinitions.Add(chartSurfaceRow);
                Grid.SetRow(this.rsiChartSurface, this.rsiGrid.Children.Count);
                this.rsiGrid.Children.Add(this.rsiChartSurface);

                this.rsiRowDefinition = new RowDefinition();
                this.rsiRowDefinition.Name = SCIConstants.SCI_RSI_ROW_DEFINITION;
                this.rsiRowDefinition.Height = new GridLength(2, GridUnitType.Star);

                this.parent.gridChartAnalysis.RowDefinitions.Add(this.rsiRowDefinition);
                Grid.SetRow(this.rsiGrid, this.parent.gridChartAnalysis.Children.Count);
                this.parent.gridChartAnalysis.Children.Add(this.rsiGrid);

                this.rsiRenderableSeries = new FastLineRenderableSeries();

                this.rsiRenderableSeries.SeriesColor = (Color)ColorConverter.ConvertFromString(this.rsiColour);
                this.rsiRenderableSeries.StrokeThickness = 1;
                this.rsiRenderableSeries.SnapsToDevicePixels = true;
                this.rsiRenderableSeries.AntiAliasing = false;

                this.rsiDataSource = new XyDataSeries<DateTime, double>();

                this.rsiIndicator = new ChartIndicators.RSI(this.rsiPeriod);

                double[] rsiBuffer = this.rsiIndicator.Push(this.chartDataSource.CloseValues.ToList());
                this.rsiDataSource.Append(this.chartDataSource.XValues, rsiBuffer);
                this.rsiRenderableSeries.DataSeries = this.rsiDataSource;
                this.rsiChartSurface.RenderableSeries.Add(this.rsiRenderableSeries);
                this.UpdateSCIChartThemeStyle(this.sciChartSelectedTheme);

                this.rsiOverBoughtLineAnnotation = new HorizontalLineAnnotation();
                this.rsiOverBoughtLineAnnotation.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.rsiOverBoughtLineAnnotation.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.rsiOverBoughtLineColour));
                this.rsiOverBoughtLineAnnotation.StrokeThickness = 0.5;
                this.rsiOverBoughtLineAnnotation.LabelPlacement = LabelPlacement.Axis;
                this.rsiOverBoughtLineAnnotation.ShowLabel = false;
                this.rsiOverBoughtLineAnnotation.Y1 = this.rsiOverBoughtLine;
                this.rsiChartSurface.Annotations.Add(this.rsiOverBoughtLineAnnotation);

                this.rsiOverSoldLineAnnotation = new HorizontalLineAnnotation();
                this.rsiOverSoldLineAnnotation.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.rsiOverSoldLineAnnotation.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.rsiOverSoldLineColour));
                this.rsiOverSoldLineAnnotation.StrokeThickness = 0.5;
                this.rsiOverSoldLineAnnotation.LabelPlacement = LabelPlacement.Axis;
                this.rsiOverSoldLineAnnotation.ShowLabel = false;
                this.rsiOverSoldLineAnnotation.Y1 = this.rsiOverSoldLine;
                this.rsiChartSurface.Annotations.Add(this.rsiOverSoldLineAnnotation);

                this.rsiMidLineAnnotation = new HorizontalLineAnnotation();
                this.rsiMidLineAnnotation.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.rsiMidLineAnnotation.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.rsiMidLineColour));
                this.rsiMidLineAnnotation.StrokeThickness = 0.5;
                this.rsiMidLineAnnotation.LabelPlacement = LabelPlacement.Axis;
                this.rsiMidLineAnnotation.ShowLabel = false;
                this.rsiMidLineAnnotation.Y1 = this.rsiMidLine;
                this.rsiChartSurface.Annotations.Add(this.rsiMidLineAnnotation);

                Binding backgroundBinding = new Binding("Background");
                backgroundBinding.Source = this.rsiChartSurface;
                Binding rsiForegroundBinding = new Binding("SeriesColor");
                rsiForegroundBinding.Source = this.rsiRenderableSeries;
                rsiForegroundBinding.Converter = new ColorToBrushConverter();

                Label rsiLabel = new Label();
                rsiLabel.Content = "RSI (" + this.rsiPeriod.ToString() + ")";
                rsiLabel.FontSize = 13;
                rsiLabel.FontWeight = FontWeights.SemiBold;
                rsiLabel.SetBinding(Label.BackgroundProperty, backgroundBinding);
                rsiLabel.SetBinding(Label.ForegroundProperty, rsiForegroundBinding);

                this.legendAnnotation = new LegendAnnotation(this.rsiChartSurface);
                List<Label> labelList = new List<Label>();
                labelList.Add(rsiLabel);
                this.legendAnnotation.AddLabel(this.chartType, labelList);
            }
        }
        #endregion

        #region Override Methods
        public override void UpdateRenderSeries(bool addNewPoint, MarketPriceItem MktItems, DateTime time)
        {
            if ((this.rsiChartSurface == null) || (this.rsiIndicator == null) || (this.rsiDataSource == null))
            {
                return;
            }

            using (this.rsiChartSurface.SuspendUpdates())
            {
                double yClose = TradeStationTools.ConvertToDouble(MktItems.Last);

                if (addNewPoint == true)
                {
                    double res = this.rsiIndicator.Push(yClose);
                    this.rsiDataSource.Append(time, res);
                }
                else
                {
                    double res = this.rsiIndicator.Update(yClose);
                    this.rsiDataSource.Update(time, res);
                }
            }
        }

        public override void UpdateSCIChartThemeStyle(String sciChartSelectedTheme)
        {
            this.sciChartSelectedTheme = sciChartSelectedTheme;

            if (this.rsiChartSurface == null)
            {
                return;
            }

            using (this.rsiChartSurface.SuspendUpdates())
            {
                if (this.sciChartSelectedTheme.Equals("Chrome") == true)
                {
                    this.rsiChartSurface.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));

                    Style sciChartXAxisMajorGridLineStyle = new Style(typeof(Line));
                    sciChartXAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"))));
                    this.rsiChartSurface.XAxis.MajorGridLineStyle = sciChartXAxisMajorGridLineStyle;

                    Style sciChartYAxisMajorGridLineStyle = new Style(typeof(Line));
                    sciChartYAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF6F6F6"))));
                    this.rsiChartSurface.YAxis.MajorGridLineStyle = sciChartYAxisMajorGridLineStyle;

                    this.rsiChartSurface.XAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
                    this.rsiChartSurface.YAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
                }
                else
                {
                    this.rsiChartSurface.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));

                    Style xAxisMajorGridLineStyle = new Style(typeof(Line));
                    xAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF404040"))));
                    this.rsiChartSurface.XAxis.MajorGridLineStyle = xAxisMajorGridLineStyle;

                    Style yAxisMajorGridLineStyle = new Style(typeof(Line));
                    yAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF202020"))));
                    this.rsiChartSurface.YAxis.MajorGridLineStyle = yAxisMajorGridLineStyle;

                    this.rsiChartSurface.XAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                    this.rsiChartSurface.YAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                }
            }
        }

        public override void RemoveFromGrid()
        {
            if ((this.rsiRowDefinition == null) || (this.rsiGrid == null) || (this.parent == null) || (this.rsiChartSurface == null))
            {
                return;
            }

            if (this.parent.gridChartAnalysis != null)
            {
                using (this.rsiChartSurface.SuspendUpdates())
                {
                    this.parent.gridChartAnalysis.Children.Remove(this.rsiGrid);
                    this.parent.gridChartAnalysis.RowDefinitions.Remove(this.rsiRowDefinition);
                }
            }
        }

        public override void AddToGrid()
        {
            if ((this.rsiRowDefinition == null) || (this.rsiGrid == null) || (this.parent == null) || (this.rsiChartSurface == null))
            {
                return;
            }

            if (this.parent.gridChartAnalysis != null)
            {
                using (this.rsiChartSurface.SuspendUpdates())
                {
                    this.parent.gridChartAnalysis.RowDefinitions.Add(this.rsiRowDefinition);
                    Grid.SetRow(this.rsiGrid, this.parent.gridChartAnalysis.Children.Count);
                    this.parent.gridChartAnalysis.Children.Add(this.rsiGrid);
                }
            }
        }

        public override void ChartEnable()
        {
            if (this.rsiChartSurface == null)
            {
                return;
            }

            this.rsiChartSurface.IsEnabled = true;
        }

        public override void ChartDisable()
        {
            if (this.rsiChartSurface == null)
            {
                return;
            }

            this.rsiChartSurface.IsEnabled = false;
        }
        #endregion
    }
}
