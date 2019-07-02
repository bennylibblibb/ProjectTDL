using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOSTS.WPFControls.Chart.SCI
{
    public class OHLCPriceSeries
    {
        public class PriceBar
        {
            public PriceBar()
            {
            }

            public PriceBar(DateTime date, double open, double high, double low, double close, long volume)
            {
                DateTime = date;
                Open = open;
                High = high;
                Low = low;
                Close = close;
                Volume = volume;
            }

            public DateTime DateTime { get; set; }
            public double Open { get; set; }
            public double High { get; set; }
            public double Low { get; set; }
            public double Close { get; set; }
            public long Volume { get; set; }
        }

        public class PriceSeries : List<PriceBar>
        {
            public PriceSeries()
            {
            }

            public PriceSeries(int capacity) : base(capacity)
            {
            }

            /// <summary>
            /// Extracts the DateTime column of the PriceSeries as an array
            /// </summary>
            public IList<DateTime> TimeData { get { return this.Select(x => x.DateTime).ToArray(); } }

            /// <summary>
            /// Extracts the Open column of the PriceSeries as an array
            /// </summary>
            public IList<double> OpenData { get { return this.Select(x => x.Open).ToArray(); } }

            /// <summary>
            /// Extracts the High column of the PriceSeries as an array
            /// </summary>
            public IList<double> HighData { get { return this.Select(x => x.High).ToArray(); } }

            /// <summary>
            /// Extracts the Low column of the PriceSeries as an array
            /// </summary>
            public IList<double> LowData { get { return this.Select(x => x.Low).ToArray(); } }

            /// <summary>
            /// Extracts the Close column of the PriceSeries as an array
            /// </summary>
            public IList<double> CloseData { get { return this.Select(x => x.Close).ToArray(); } }

            /// <summary>
            /// Extracts the Volume column of the PriceSeries as an array
            /// </summary>
            public IList<long> VolumeData { get { return this.Select(x => x.Volume).ToArray(); } }
        }
    }
}
