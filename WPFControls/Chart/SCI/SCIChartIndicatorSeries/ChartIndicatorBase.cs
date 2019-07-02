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
    public class ChartIndicatorList : List<ChartIndicatorBase>
    {
        public void RemoveIndicator(SCIChartAnalysis parent, SCIConstants.ChartIndicatorType indicatorType)
        {
            foreach (ChartIndicatorBase indicator in this)
            {
                if (indicator.chartType == indicatorType)
                {
                    indicator.Dispose();
                    this.Remove(indicator);
                    break;
                }
            }

            foreach (ChartIndicatorBase indicator in this)
            {
                indicator.RemoveFromGrid();
            }

            foreach (ChartIndicatorBase indicator in this)
            {
                indicator.AddToGrid();
            }
        }

        public void RemoveAllIndicators()
        {
            foreach (ChartIndicatorBase indicator in this)
            {
                indicator.Dispose();
            }

            this.Clear();
        }

        public List<SCIConstants.ChartIndicatorType> GetIndicatorsListOrder()
        {
            List<SCIConstants.ChartIndicatorType> listOrder = new List<SCIConstants.ChartIndicatorType>();

            foreach (ChartIndicatorBase indicator in this)
            {
                listOrder.Add(indicator.chartType);
            }

            return listOrder;
        }
    }

    public abstract class ChartIndicatorBase : IDisposable
    {
        public SCIConstants.ChartIndicatorType chartType;
        private bool _disposed = false;
        public abstract void UpdateRenderSeries(bool addNewPoint, MarketPriceItem MktItems, DateTime time);
        public abstract void UpdateSCIChartThemeStyle(String sciChartSelectedTheme);
        public abstract void RemoveFromGrid();
        public abstract void AddToGrid();
        public abstract void ChartEnable();
        public abstract void ChartDisable();

        public ChartIndicatorBase()
        {
            chartType = SCIConstants.ChartIndicatorType.UNDEFINED;
        }

        ~ChartIndicatorBase()
        {
            //            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                //release managed resources
            }

            //release unmanaged resources

            _disposed = true;
        }
    }
}
