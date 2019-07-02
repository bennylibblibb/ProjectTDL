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
    public class Volume : ChartIndicatorBase
    {
        private readonly SCIChartAnalysis parent;
        private readonly XyDataSeries<DateTime, long> volumeDataSource;
        private String sciChartSelectedTheme;
        private readonly uint chartPeriod;

        private Grid _volumeGrid;
        private SciChartSurface _volumeChartSurface;
        private GridSplitter _volumeGridSplitter;
        private RowDefinition _volumeRowDefinition;
        private FastColumnRenderableSeries _volumeRenderableSeries;
        private LegendAnnotation _legendAnnotation;

        public Volume(SCIConstants.ChartIndicatorType type, SCIChartAnalysis parent, XyDataSeries<DateTime, long> volumeDataSource, String sciChartSelectedTheme, uint chartPeriod)
        {
            this.chartType = type;
            this.parent = parent;
            this.volumeDataSource = volumeDataSource;
            this.sciChartSelectedTheme = sciChartSelectedTheme;
            this.chartPeriod = chartPeriod;

            this.CreateRenderSeries();
        }

        protected override void Dispose(bool disposing)
        {
            this.legendAnnotation = null;
            this.volumeRenderableSeries = null;
            this.volumeGridSplitter = null;
            this.volumeChartSurface = null;
            this.volumeRowDefinition = null;
            this.volumeGrid = null;
        }

        #region properties
        public FastColumnRenderableSeries volumeRenderableSeries
        {
            get { return _volumeRenderableSeries; }
            set
            {
                if (_volumeRenderableSeries != null)
                {
                    _volumeRenderableSeries.DataSeries = null;

                    if (this.volumeChartSurface != null)
                    {
                        using (this.volumeChartSurface.SuspendUpdates())
                        {
                            this.volumeChartSurface.RenderableSeries.Remove(_volumeRenderableSeries);
                        }
                    }
                }

                _volumeRenderableSeries = value;
            }
        }

        public RowDefinition volumeRowDefinition
        {
            get { return _volumeRowDefinition; }
            set
            {
                if (_volumeRowDefinition != null)
                {
                    if (this.parent != null)
                    {
                        if (this.parent.gridChartAnalysis != null)
                        {
                            this.parent.gridChartAnalysis.RowDefinitions.Remove(_volumeRowDefinition);
                        }
                    }
                }

                _volumeRowDefinition = value;
            }
        }

        private SciChartSurface volumeChartSurface
        {
            get { return _volumeChartSurface; }
            set
            {
                if (_volumeChartSurface != null)
                {
                    using (_volumeChartSurface.SuspendUpdates())
                    {
                        if (this.volumeGrid != null)
                        {
                            this.volumeGrid.Children.Remove(_volumeChartSurface);
                        }
                    }
                }

                _volumeChartSurface = value;
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

        private Grid volumeGrid
        {
            get { return _volumeGrid; }
            set
            {
                if (_volumeGrid != null)
                {
                    if (this.parent != null)
                    {
                        if (this.parent.gridChartAnalysis != null)
                        {
                             this.parent.gridChartAnalysis.Children.Remove(_volumeGrid);
                        }
                    }

                    _volumeGrid.Children.Clear();
                }

                _volumeGrid = value;
            }
        }

        private GridSplitter volumeGridSplitter
        {
            get { return _volumeGridSplitter; }
            set
            {
                if (_volumeGridSplitter != null)
                {
                    if(this.volumeGrid != null)
                    {
                        this.volumeGrid.Children.Remove(_volumeGridSplitter);
                    }
                }

                _volumeGridSplitter = value;
            }
        }
        #endregion

        #region Internal methods
        private void CreateRenderSeries()
        {
            if ((this.parent == null) || (this.volumeDataSource == null))
            {
                return;
            }

            this.volumeChartSurface = new SciChartSurface();

            using (this.volumeChartSurface.SuspendUpdates())
            {
                SciChartGroup.SetVerticalChartGroup(this.volumeChartSurface, "myCharts");
                this.volumeChartSurface.Padding = new Thickness(5, 5, 15, 5);
                this.volumeChartSurface.RenderPriority = RenderPriority.Low;

                CategoryDateTimeAxis volumeXAxis = new CategoryDateTimeAxis();
                Binding visibleRangeBinding = new Binding("XAxis.VisibleRange");
                visibleRangeBinding.Source = this.parent.sciChart;
                volumeXAxis.SetBinding(AxisBase.VisibleRangeProperty, visibleRangeBinding);
                volumeXAxis.LabelFormatter = SCICommon.CreateTimeAxisFormatter(this.chartPeriod);
                volumeXAxis.DrawLabels = false;
                volumeXAxis.DrawMajorBands = false;
                volumeXAxis.DrawMajorGridLines = true;
                volumeXAxis.DrawMajorTicks = false;
                volumeXAxis.DrawMinorGridLines = false;
                volumeXAxis.DrawMinorTicks = false;
                this.volumeChartSurface.XAxis = volumeXAxis;

                NumericAxis volumeYAxis = new NumericAxis();
                volumeYAxis.DrawMajorGridLines = false;
                volumeYAxis.DrawMinorGridLines = false;
                volumeYAxis.DrawMajorTicks = true;
                volumeYAxis.DrawMinorTicks = false;
                volumeYAxis.DrawLabels = true;
                volumeYAxis.MajorDelta = 10;
                volumeYAxis.AutoRange = AutoRange.Always;
                volumeYAxis.VisibleRange = new DoubleRange(0, 100);
                volumeYAxis.GrowBy = new DoubleRange(0.0, 0.1);
                this.volumeChartSurface.YAxis = volumeYAxis;

                ZoomPanModifier volumeZoomPanModifier = new ZoomPanModifier();
                volumeZoomPanModifier.IsEnabled = false;
                RubberBandXyZoomModifier volumeRubberBandXyZoomModifier = new RubberBandXyZoomModifier();
                volumeRubberBandXyZoomModifier.IsEnabled = false;
                MouseWheelZoomModifier volumeMouseWheelZoomModifier = new MouseWheelZoomModifier();
                volumeMouseWheelZoomModifier.IsEnabled = false;
                CursorModifier volumeCursorModifier = new CursorModifier();
                Binding volumeCursorModifieIsEnabledBinding = new Binding("IsMouseCursorEnabled");
                volumeCursorModifieIsEnabledBinding.Source = this.parent;
                volumeCursorModifier.SetBinding(CursorModifier.IsEnabledProperty, volumeCursorModifieIsEnabledBinding);
                volumeCursorModifier.ShowAxisLabels = true;
                volumeCursorModifier.ShowTooltip = true;
                volumeCursorModifier.ReceiveHandledEvents = true;
                this.volumeChartSurface.ChartModifier = new ModifierGroup(volumeZoomPanModifier, volumeRubberBandXyZoomModifier, volumeMouseWheelZoomModifier, volumeCursorModifier);
                MouseManager.SetMouseEventGroup((ModifierGroup)this.volumeChartSurface.ChartModifier, "MySharedMouseGroup");

                this.volumeGrid = new Grid();

                this.volumeGridSplitter = new GridSplitter();
                this.volumeGridSplitter.IsEnabled = false;
                this.volumeGridSplitter.Background = Brushes.Gray;
                this.volumeGridSplitter.Width = double.NaN;
                this.volumeGridSplitter.Height = 1;
                this.volumeGridSplitter.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.volumeGridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
                this.volumeGridSplitter.Margin = new Thickness(0);

                RowDefinition gridSplitterRow = new RowDefinition();
                gridSplitterRow.Height = new GridLength(1, GridUnitType.Pixel);
                this.volumeGrid.RowDefinitions.Add(gridSplitterRow);
                Grid.SetRow(this.volumeGridSplitter, this.volumeGrid.Children.Count);
                this.volumeGrid.Children.Add(this.volumeGridSplitter);

                RowDefinition chartSurfaceRow = new RowDefinition();
                chartSurfaceRow.Height = new GridLength(1, GridUnitType.Star);
                this.volumeGrid.RowDefinitions.Add(chartSurfaceRow);
                Grid.SetRow(this.volumeChartSurface, this.volumeGrid.Children.Count);
                this.volumeGrid.Children.Add(this.volumeChartSurface);

                this.volumeRowDefinition = new RowDefinition();
                this.volumeRowDefinition.Name = SCIConstants.SCI_VOLUME_ROW_DEFINITION;
                this.volumeRowDefinition.Height = new GridLength(2, GridUnitType.Star);

                this.parent.gridChartAnalysis.RowDefinitions.Add(this.volumeRowDefinition);
                Grid.SetRow(this.volumeGrid, this.parent.gridChartAnalysis.Children.Count);
                this.parent.gridChartAnalysis.Children.Add(this.volumeGrid);

                this.volumeRenderableSeries = new FastColumnRenderableSeries();
                this.volumeRenderableSeries.SeriesColor = (Color)ColorConverter.ConvertFromString("#FFA1BACE");
                this.volumeRenderableSeries.AntiAliasing = false;
                this.volumeRenderableSeries.StrokeThickness = 1;
                this.volumeRenderableSeries.SnapsToDevicePixels = true;

                this.volumeRenderableSeries.DataPointWidth = 0.4;
                this.volumeRenderableSeries.FillBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D04682B4"));
                this.volumeRenderableSeries.DataSeries = this.volumeDataSource;
                this.volumeChartSurface.RenderableSeries.Add(this.volumeRenderableSeries);
                this.UpdateSCIChartThemeStyle(this.sciChartSelectedTheme);

                Binding backgroundBinding = new Binding("Background");
                backgroundBinding.Source = this.volumeChartSurface;
                Binding volumeForegroundBinding = new Binding("SeriesColor");
                volumeForegroundBinding.Source = this.volumeRenderableSeries;
                volumeForegroundBinding.Converter = new ColorToBrushConverter();

                Label volumeLabel = new Label();
                volumeLabel.Content = "Volume";
                volumeLabel.FontSize = 13;
                volumeLabel.FontWeight = FontWeights.SemiBold;
                volumeLabel.SetBinding(Label.BackgroundProperty, backgroundBinding);
                volumeLabel.SetBinding(Label.ForegroundProperty, volumeForegroundBinding);

                this.legendAnnotation = new LegendAnnotation(this.volumeChartSurface);
                List<Label> labelList = new List<Label>();
                labelList.Add(volumeLabel);
                this.legendAnnotation.AddLabel(this.chartType, labelList); 
            }
        }
        #endregion

        #region Override Methods
        public override void UpdateRenderSeries(bool addNewPoint, MarketPriceItem MktItems, DateTime time)
        {
        }

        public override void UpdateSCIChartThemeStyle(String sciChartSelectedTheme)
        {
            this.sciChartSelectedTheme = sciChartSelectedTheme;

            if (this.volumeChartSurface == null)
            {
                return;
            }

            using (this.volumeChartSurface.SuspendUpdates())
            {
                if (this.sciChartSelectedTheme.Equals("Chrome") == true)
                {
                    Style sciVolumeChartXAxisMajorGridLineStyle = new Style(typeof(Line));
                    sciVolumeChartXAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF2F2F2"))));
                    this.volumeChartSurface.XAxis.MajorGridLineStyle = sciVolumeChartXAxisMajorGridLineStyle;

                    this.volumeChartSurface.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                    this.volumeChartSurface.XAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
                    this.volumeChartSurface.YAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
                }
                else
                {
                    Style sciVolumeChartXAxisMajorGridLineStyle = new Style(typeof(Line));
                    sciVolumeChartXAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF404040"))));
                    this.volumeChartSurface.XAxis.MajorGridLineStyle = sciVolumeChartXAxisMajorGridLineStyle;

                    this.volumeChartSurface.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
                    this.volumeChartSurface.XAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                    this.volumeChartSurface.YAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                }
            }
        }

        public override void RemoveFromGrid()
        {
            if ((this.volumeRowDefinition == null) || (this.volumeGrid == null) || (this.parent == null) || (this.volumeChartSurface == null))
            {
                return;
            }

            if (this.parent.gridChartAnalysis != null)
            {
                using (this.volumeChartSurface.SuspendUpdates())
                {
                    this.parent.gridChartAnalysis.Children.Remove(this.volumeGrid);
                    this.parent.gridChartAnalysis.RowDefinitions.Remove(this.volumeRowDefinition);
                }
            }
        }

        public override void AddToGrid()
        {
            if ((this.volumeRowDefinition == null) || (this.volumeGrid == null) || (this.parent == null) || (this.volumeChartSurface == null))
            {
                return;
            }

            if (this.parent.gridChartAnalysis != null)
            {
                using (this.volumeChartSurface.SuspendUpdates())
                {
                    this.parent.gridChartAnalysis.RowDefinitions.Add(this.volumeRowDefinition);
                    Grid.SetRow(this.volumeGrid, this.parent.gridChartAnalysis.Children.Count);
                    this.parent.gridChartAnalysis.Children.Add(this.volumeGrid);
                }
            }
        }

        public override void ChartEnable()
        {
            if (this.volumeChartSurface == null)
            {
                return;
            }

            this.volumeChartSurface.IsEnabled = true;
        }

        public override void ChartDisable()
        {
            if (this.volumeChartSurface == null)
            {
                return;
            }

            this.volumeChartSurface.IsEnabled = false;
        }
        #endregion
    }
}
