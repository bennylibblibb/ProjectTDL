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
    public class LegendAnnotation : IDisposable
    {
        private class LegendAnnotationInfo
        {
            private readonly SCIConstants.ChartIndicatorType _chartType;
            private readonly List<Label> _labelList;

            public LegendAnnotationInfo(SCIConstants.ChartIndicatorType type, List<Label> list)
            {
                _chartType  = type;
                _labelList = list;
            }

            public SCIConstants.ChartIndicatorType chartType { get { return _chartType; } }
            public List<Label> labelList { get { return _labelList; } }
        }

        private readonly SciChartSurface parentSurface;

        private CustomAnnotation _legendAnnotation;
        private StackPanel _stackPanel;
        private List<LegendAnnotationInfo> _infoList;

        private CustomAnnotation legendAnnotation
        {
            get { return _legendAnnotation; }
            set
            {
                if (_legendAnnotation != null)
                {
                    if (this.parentSurface != null)
                    {
                        using (this.parentSurface.SuspendUpdates())
                        {
                            this.parentSurface.Annotations.Remove(_legendAnnotation);
                        }
                    }
                }

                _legendAnnotation = value;
            }
        }

        private StackPanel stackPanel
        {
            get { return _stackPanel; }
            set
            {
                if(_stackPanel != null)
                {
                    _stackPanel.Children.Clear();
                }

                _stackPanel = value;
            }
        }

        private List<LegendAnnotationInfo> infoList
        {
            get { return _infoList; }
            set 
            {
                if(_infoList != null)
                {
                    foreach (LegendAnnotationInfo info in _infoList)
                    {
                        info.labelList.Clear();
                    }

                    _infoList.Clear();
                }

                _infoList = value;
            }
        }
        
        public LegendAnnotation(SciChartSurface parentSurface)
        {
            this.parentSurface = parentSurface;

            this.infoList = new List<LegendAnnotationInfo>();

            this.stackPanel = new StackPanel();
            this.stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.stackPanel.VerticalAlignment = VerticalAlignment.Center;
            this.stackPanel.Orientation = Orientation.Horizontal;

            this.legendAnnotation = new CustomAnnotation();
            this.legendAnnotation.CoordinateMode = AnnotationCoordinateMode.Relative;
            this.legendAnnotation.X1 = 0.005;
            this.legendAnnotation.Y1 = 0.01;
            this.legendAnnotation.AnnotationCanvas = AnnotationCanvas.AboveChart;
            this.legendAnnotation.HorizontalAnchorPoint = HorizontalAnchorPoint.Left;
            this.legendAnnotation.VerticalAnchorPoint = VerticalAnchorPoint.Top;
            this.legendAnnotation.VerticalAlignment = VerticalAlignment.Top;
            this.legendAnnotation.HorizontalAlignment = HorizontalAlignment.Left;
            this.legendAnnotation.Padding = new Thickness(5);
            Binding backgroundBinding = new Binding("Background");
            backgroundBinding.Source = this.parentSurface;
            this.legendAnnotation.SetBinding(CustomAnnotation.BackgroundProperty, backgroundBinding);
            this.legendAnnotation.Content = this.stackPanel;
            
            using (this.parentSurface.SuspendUpdates())
            {
                this.parentSurface.Annotations.Add(this.legendAnnotation);
            }
        }

        public void Dispose()
        {
            this.legendAnnotation = null;
            this.stackPanel = null;
            this.infoList = null;
        }

        public void AddLabel(SCIConstants.ChartIndicatorType type, List<Label> label)
        {
            LegendAnnotationInfo info = new LegendAnnotationInfo(type, label);
            this.infoList.Add(info);
            foreach (Label l in label)
            {
                this.stackPanel.Children.Add(l);
            }
        }

        public void RemoveLabel(SCIConstants.ChartIndicatorType type)
        {
            foreach (LegendAnnotationInfo info in this.infoList)
            {
                if (info.chartType == type)
                {
                    foreach (Label i in info.labelList)
                    {
                        this.stackPanel.Children.Remove(i);
                    }

                    this.infoList.Remove(info);
                    break;
                }
            }
        }
    }
}
