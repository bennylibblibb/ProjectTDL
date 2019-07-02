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
using GOSTS.WPFControls.Chart.SCI.SCIChartIndicatorSeries;
using Abt.Controls.SciChart.Visuals;
using Abt.Controls.SciChart.Visuals.Axes;
using Abt.Controls.SciChart.Utility.Mouse;
using System.Data;

namespace GOSTS.WPFControls.Chart.SCI
{
    public delegate void ChartHistoryReceivedDelegate(OHLCPriceSeries.PriceSeries priceSeries, long totalChartHistoryVolume);
    public delegate void MarketPriceReceivedDelegate(MarketPriceItem MktItems, bool newDataPoint);

    /// <summary>
    /// Interaction logic for SCIChartAnalysis.xaml
    /// </summary>
    public partial class SCIChartAnalysis : UserControl, INotifyPropertyChanged
    {
        #region Declares
        private delegate void ChartHistoryReceivedAsyncDelegate(OHLCPriceSeries.PriceSeries priceSeries, long totalChartHistoryVolume);
        private delegate void MarketPriceReceivedAsyncDelegate(MarketPriceItem MktItems, bool newDataPoint);
        private delegate void ReloadDataSourceToDispatcherDelegate(String obj);
        private delegate void distributeMsg_DisControlAsyncDelegate(object sender, string prodCode);
        private delegate void distributeMsg_DisDisGotProductListAsyncDelegate(object sender);

        private MessageDistribute distributeMsg;
        private MdiChild sciMDIChild;
        private String _productCode;
        private String _productName;
        private uint _chartPeriod;

        private bool thisLoaded;
        private bool productListReceived;
        private SCIChartAnalysisChartHistory chartHistory;
        private SCIChartAnalysisMarketPrice marketPrice;
        private ChartHistoryReceivedAsyncDelegate chartHistoryReceivedAsyncDelegate;
        private MarketPriceReceivedAsyncDelegate marketPriceReceivedAsyncDelegate;
        private HorizontalLineAnnotation _closeLineAnnotation;
        private LegendAnnotation _legendAnnotation;
        private OhlcDataSeries<DateTime, double> _chartDataSource;
        private XyDataSeries<DateTime, long> _chartVolumeSource;
        private bool _isPanEnabled;
        private bool _isTickerLineEnabled;
        private bool _isChartOverviewEnabled;
        private bool _isMouseCursorEnabled;
        private bool _isLockProductEnabled;
        private String _chartSettings;
        private String _toggleButtonZoomPanText;
        private String _toggleButtonRubberBandZoomText;
        private String _buttonZoomExtentsText;
        private String _buttonChartResetXPosition;
        private String _toggleButtonTickerLineText;
        private String _toggleButtonChartOverviewText;
        private String _checkBoxLockProductText;
        private String _buttonIndicatorMenuText;
        private String _indicatorMenuItemVolumeText;
        private String _indicatorMenuItemBollingerBandsText;
        private String _indicatorMenuItemSMAText;
        private String _indicatorMenuItemSMA1Text;
        private String _indicatorMenuItemSMA2Text;
        private String _indicatorMenuItemSMA3Text;
        private String _indicatorMenuItemMACDText;
        private String _indicatorMenuItemRSIText;
        private String _indicatorMenuItemWilliamsRText;
        private String _indicatorMenuItemRemoveAllText;
        private String _sciChartThemeText;
        private String _sciChartSelectedTheme;
        private String _sciChartThemeLightText;
        private String _sciChartThemeDarkText;
        private String _chartPeriodMenuText;
        private String _chartPeriodMenu1SecText;
        private String _chartPeriodMenu5SecText;
        private String _chartPeriodMenu10SecText;
        private String _chartPeriodMenu15SecText;
        private String _chartPeriodMenu30SecText;
        private String _chartPeriodMenu60SecText;
        private String _chartPeriodMenu300SecText;
        private String _chartPeriodMenu600SecText;
        private String _chartPeriodMenu900SecText;
        private String _chartPeriodMenu1800SecText;
        private String _chartPeriodMenu3600SecText;
        private readonly ChartIndicatorList _chartIndicatorList;
        private uint sma1Period;
        private uint sma2Period;
        private uint sma3Period;
        private uint bbPeriod;
        private double bbDevi;
        private uint macdFastPeriod;
        private uint macdSlowPeriod;
        private uint macdSignalPeriod;
        private uint rsiPeriod;
        private uint rsiOverBoughtLine;
        private uint rsiOverSoldLine;
        private uint rsiMidLine;
        private uint williamsRPeriod;
        private int williamsROverBoughtLine;
        private int williamsROverSoldLine;
        private int williamsRMidLine;
        private bool _showVolumeIndicator;
        private bool _showSMA1Indicator;
        private bool _showSMA2Indicator;
        private bool _showSMA3Indicator;
        private bool _showBBIndicator;
        private bool _showMACDIndicator;
        private bool _showRSIIndicator;
        private bool _showWilliamsRIndicator;
        private String _sma1Colour;
        private String _sma2Colour;
        private String _sma3Colour;
        private String _bbUpperLowerColour;
        private String _bbMiddleColour;
        private String _macdMACDColour;
        private String _macdSignalColour;
        private String _macdHistogramColour;
        private String _rsiColour;
        private String _rsiOverBoughtLineColour;
        private String _rsiOverSoldLineColour;
        private String _rsiMidLineColour;
        private String _williamsRColour;
        private String _williamsROverBoughtLineColour;
        private String _williamsROverSoldLineColour;
        private String _williamsRMidLineColour;
        private String _candlestickUpColour;
        private String _candlestickDownColour;
        private IndexRange _xVisibleRange;
        private bool bReceivedChartHistory;
        private bool showChartBreakPeriods;
        private long totalChartHistoryVolume;
        private MessageDistribute.OnDisGotProductList onDisGotProductListEvent;
        private int createTime;
        private List<SCIConstants.ChartIndicatorType> indicatorsOrderList;

        private String _marketPriceStateLabel;
        private String _marketPriceStateValue;
        private String _marketPriceTimeLabel;
        private String _marketPriceTimeValue;
        private String _marketPriceLastLabel;
        private String _marketPriceLastValue;
        private String _marketPriceChangeLabel;
        private String _marketPriceChangeValue;
        private String _marketPriceChangePercentLabel;
        private String _marketPriceChangePercentValue;
        private String _marketPriceLastQuantityLabel;
        private String _marketPriceLastQuantityValue;
        private String _marketPriceBidQuantityLabel;
        private String _marketPriceBidQuantityValue;
        private String _marketPriceBidLabel;
        private String _marketPriceBidValue;
        private String _marketPriceAskLabel;
        private String _marketPriceAskValue;
        private String _marketPriceAskQuantityLabel;
        private String _marketPriceAskQuantityValue;
        private String _marketPriceVolumeLabel;
        private String _marketPriceVolumeValue;
        private String _marketPriceBlockVolumeValue;
        private String _marketPriceHighLabel;
        private String _marketPriceHighValue;
        private String _marketPriceLowLabel;
        private String _marketPriceLowValue;
        private String _marketPriceOpenLabel;
        private String _marketPriceOpenValue;
        private String _marketPricePreviousCloseLabel;
        private String _marketPricePreviousCloseValue;
        private String _marketPriceCloseDateLabel;
        private String _marketPriceCloseDateValue;
        private String _marketPriceStrikeLabel;
        private String _marketPriceStrikeValue;
        #endregion

        public SCIChartAnalysis(MessageDistribute _msgDistribute, MdiChild mdi, String productCode)
        {
            this.createTime = this.GetHashCode();//(int)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;

            this.DataContext = this;

            this.distributeMsg = _msgDistribute;
            this.sciMDIChild = mdi;
            this._productCode = productCode;

            this._chartPeriod = 60;

            this.thisLoaded = false;
            this.productListReceived = false;
            this.chartHistory = null;
            this.marketPrice = null;
            this.chartHistoryReceivedAsyncDelegate = null;
            this.marketPriceReceivedAsyncDelegate = null;
            this._closeLineAnnotation = null;
            this._legendAnnotation = null;
            this._chartDataSource = null;
            this._chartVolumeSource = null;
            this._isPanEnabled = true;
            this._isTickerLineEnabled = true;
            this._isChartOverviewEnabled = true;
            this._isMouseCursorEnabled = false;
            this._isLockProductEnabled = false;
            this._chartSettings = "";
            this._toggleButtonZoomPanText = "Pan";
            this._toggleButtonRubberBandZoomText = "Zoom";
            this._buttonZoomExtentsText = "All";
            this._buttonChartResetXPosition = "Reset";
            this._toggleButtonTickerLineText = "Line";
            this._toggleButtonChartOverviewText = "Ovr";
            this._buttonIndicatorMenuText = "Indicator";
            this._checkBoxLockProductText = "Lock";
            this._indicatorMenuItemVolumeText = "Volume";
            this._indicatorMenuItemBollingerBandsText = "Bollinger Bands";
            this._indicatorMenuItemSMAText = "Simple Moving Average";
            this._indicatorMenuItemMACDText = "MACD";
            this._indicatorMenuItemRSIText = "RSI";
            this._indicatorMenuItemWilliamsRText = "Williams %R";
            this._indicatorMenuItemRemoveAllText = "Remove All";
            this._sciChartThemeText = "Theme";
            this._sciChartSelectedTheme = "Chrome";
            this._sciChartThemeLightText = "Light";
            this._sciChartThemeDarkText = "Dark";
            this._chartPeriodMenuText = "Period";
            this._chartPeriodMenu1SecText = "1 Sec";
            this._chartPeriodMenu5SecText = "5 Sec";
            this._chartPeriodMenu10SecText = "10 Sec";
            this._chartPeriodMenu15SecText = "15 Sec";
            this._chartPeriodMenu30SecText = "30 Sec";
            this._chartPeriodMenu60SecText = "1 Min";
            this._chartPeriodMenu300SecText = "5 Min";
            this._chartPeriodMenu600SecText = "10 Min";
            this._chartPeriodMenu900SecText = "15 Min";
            this._chartPeriodMenu1800SecText = "30 Min";
            this._chartPeriodMenu3600SecText = "60 Min";
            this._chartIndicatorList = new ChartIndicatorList();
            this.sma1Period = 0;
            this.sma2Period = 0;
            this.sma3Period = 0;
            this.bbPeriod = 0;
            this.bbDevi = 0;
            this.macdFastPeriod = 0;
            this.macdSlowPeriod = 0;
            this.macdSignalPeriod = 0;
            this.rsiPeriod = 0;
            this.williamsRPeriod = 0;
            this._showVolumeIndicator = false;
            this._showSMA1Indicator = false;
            this._showSMA2Indicator = false;
            this._showSMA3Indicator = false;
            this._showBBIndicator = false;
            this._showMACDIndicator = false;
            this._showRSIIndicator = false;
            this._showWilliamsRIndicator = false;
            this.bReceivedChartHistory = false;
            this.showChartBreakPeriods = false;
            this.totalChartHistoryVolume = 0;
            this.onDisGotProductListEvent = null;
            this.indicatorsOrderList = null;
            this._marketPriceStateLabel = "State";
            this._marketPriceStateValue = "";
            this._marketPriceTimeLabel = "Time";
            this._marketPriceTimeValue = "";
            this._marketPriceLastLabel = "Last";
            this._marketPriceLastValue = "";
            this._marketPriceChangeLabel = "Chg";
            this._marketPriceChangeValue = "";
            this._marketPriceChangePercentLabel = "Chg%";
            this._marketPriceChangePercentValue = "";
            this._marketPriceLastQuantityLabel = "LQty";
            this._marketPriceLastQuantityValue = "";
            this._marketPriceBidQuantityLabel = "BQty";
            this._marketPriceBidQuantityValue = "";
            this._marketPriceBidLabel = "Bid";
            this._marketPriceBidValue = "";
            this._marketPriceAskLabel = "Ask";
            this._marketPriceAskValue = "";
            this._marketPriceAskQuantityLabel = "AQty";
            this._marketPriceAskQuantityValue = "";
            this._marketPriceVolumeLabel = "Vol";
            this._marketPriceVolumeValue = "";
            this._marketPriceBlockVolumeValue = "";
            this._marketPriceHighLabel = "High";
            this._marketPriceHighValue = "";
            this._marketPriceLowLabel = "Low";
            this._marketPriceLowValue = "";
            this._marketPriceOpenLabel = "Open";
            this._marketPriceOpenValue = "";
            this._marketPricePreviousCloseLabel = "Prv Cl";
            this._marketPricePreviousCloseValue = "";
            this._marketPriceCloseDateLabel = "Cl Date";
            this._marketPriceCloseDateValue = "";
            this._marketPriceStrikeLabel = "Strike";
            this._marketPriceStrikeValue = "";

            this.UpdateChildWindowTitle();

            InitializeComponent();

            this.LoadUserChartSettings();
        }

        private void SCIChartAnalysis_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.thisLoaded = true;
            this.productListReceived = false;

            this.legendAnnotation = new LegendAnnotation(this.sciChart);

            if (this.IsChartOverviewEnabled == true)
            {
                this.gridChartAnalysis.RowDefinitions[this.getGridChartAnalysisRowByName(SCIConstants.SCI_CHARTOVERVIEW_ROW_DEFINITION)].Height = new GridLength(38);
            }
            else
            {
                this.gridChartAnalysis.RowDefinitions[this.getGridChartAnalysisRowByName(SCIConstants.SCI_CHARTOVERVIEW_ROW_DEFINITION)].Height = new GridLength(0);
            }

            this.sciChart.RenderPriority = RenderPriority.Low;
            this.sciChart.Padding = new Thickness(5, 5, 15, 5);
            this.sciChart.IsEnabled = false;
            this.sciOverviewChartGrid.IsEnabled = false;
            this.disableAllChartViews();

            this.tbProductCode.CaretIndex = this.tbProductCode.Text.Length;
            this.tbProductCode.Focus();

            this.distributeMsg.DisControl += new MessageDistribute.OnDisControl(distributeMsg_DisControl);

            if (MarketPriceData.ProductListExists() == false)
            {
                this.productListReceived = false;
                this.onDisGotProductListEvent = new MessageDistribute.OnDisGotProductList(distributeMsg_DisDisGotProductList);
                this.distributeMsg.DisGotProductList += this.onDisGotProductListEvent;
            }
            else
            {
                this.onDisGotProductListEvent = null;
                this.productListReceived = true;
                this.ReloadDataSource(this.chartPeriod);
            }
        }

        private void SCIChartAnalysis_OnUnLoaded(object sender, RoutedEventArgs e)
        {
            this.distributeMsg.DisControl -= new MessageDistribute.OnDisControl(distributeMsg_DisControl);

            if (this.onDisGotProductListEvent != null)
            {
                this.distributeMsg.DisGotProductList -= this.onDisGotProductListEvent;
                this.onDisGotProductListEvent = null;
            }

            this.RemoveChartIndicators(true);
            this.InvalidateObjects();
            this.legendAnnotation = null;

            this.thisLoaded = false;
        }

        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public String productCode
        {
            get { return _productCode; }
            set
            {
                _productCode = value;
                OnPropertyChanged("productCode");
            }
        }

        public String ProductName
        {
            get { return _productName; }
            set
            {
                _productName = value;
                OnPropertyChanged("ProductName");
            }
        }

        public uint chartPeriod
        {
            get { return _chartPeriod; }
            set
            {
                _chartPeriod = value;
                OnPropertyChanged("chartPeriod");
            }
        }

        private HorizontalLineAnnotation closeLineAnnotation
        {
            get { return _closeLineAnnotation; }
            set
            {
                if (_closeLineAnnotation != null)
                {
                    if (sciChart != null)
                    {
                        sciChart.Annotations.Remove(_closeLineAnnotation);
                    }
                }

                _closeLineAnnotation = value;
            }
        }

        public LegendAnnotation legendAnnotation
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

        public OhlcDataSeries<DateTime, double> chartDataSource
        {
            get { return _chartDataSource; }
            set
            {
                if (sciChart != null)
                {
                    sciChart.RenderableSeries[0].DataSeries = null;
                }

                _chartDataSource = value;
            }
        }

        public XyDataSeries<DateTime, long> chartVolumeSource
        {
            get { return _chartVolumeSource; }
            set
            {
                _chartVolumeSource = value;
            }
        }

        public bool IsPanEnabled
        {
            get { return _isPanEnabled; }
            set
            {
                _isPanEnabled = value;
                OnPropertyChanged("IsPanEnabled");
            }
        }

        public bool IsTickerLineEnabled
        {
            get { return _isTickerLineEnabled; }
            set
            {
                _isTickerLineEnabled = value;
                OnPropertyChanged("IsTickerLineEnabled");
            }
        }

        public bool IsChartOverviewEnabled
        {
            get { return _isChartOverviewEnabled; }
            set
            {
                _isChartOverviewEnabled = value;
                OnPropertyChanged("IsChartOverviewEnabled");
            }
        }

        public bool IsMouseCursorEnabled
        {
            get { return _isMouseCursorEnabled; }
            set
            {
                _isMouseCursorEnabled = value;
                OnPropertyChanged("IsMouseCursorEnabled");
            }
        }

        public bool IsLockProductEnabled
        {
            get { return _isLockProductEnabled; }
            set
            {
                _isLockProductEnabled = value;
                OnPropertyChanged("IsLockProductEnabled");
            }
        }

        public String chartSettings
        {
            get
            {
                _chartSettings = getChartSettings();
                return _chartSettings;
            }
            set
            {
                _chartSettings = setChartSettings(value);
            }
        }

        public String toggleButtonZoomPanText
        {
            get { return _toggleButtonZoomPanText; }
            set
            {
                _toggleButtonZoomPanText = value;
                OnPropertyChanged("toggleButtonZoomPanText");
            }
        }

        public String toggleButtonRubberBandZoomText
        {
            get { return _toggleButtonRubberBandZoomText; }
            set
            {
                _toggleButtonRubberBandZoomText = value;
                OnPropertyChanged("toggleButtonRubberBandZoomText");
            }
        }

        public String buttonZoomExtentsText
        {
            get { return _buttonZoomExtentsText; }
            set
            {
                _buttonZoomExtentsText = value;
                OnPropertyChanged("buttonZoomExtentsText");
            }
        }

        public String buttonChartResetXPositionText
        {
            get { return _buttonChartResetXPosition; }
            set
            {
                _buttonChartResetXPosition = value;
                OnPropertyChanged("buttonChartResetXPosition");
            }
        }

        public String toggleButtonTickerLineText
        {
            get { return _toggleButtonTickerLineText; }
            set
            {
                _toggleButtonTickerLineText = value;
                OnPropertyChanged("toggleButtonTickerLineText");
            }
        }

        public String toggleButtonChartOverviewText
        {
            get { return _toggleButtonChartOverviewText; }
            set
            {
                _toggleButtonChartOverviewText = value;
                OnPropertyChanged("toggleButtonChartOverviewText");
            }
        }

        public String buttonIndicatorMenuText
        {
            get { return _buttonIndicatorMenuText; }
            set
            {
                _buttonIndicatorMenuText = value;
                OnPropertyChanged("buttonIndicatorMenuText");
            }
        }

        public String checkBoxLockProductText
        {
            get { return _checkBoxLockProductText; }
            set
            {
                _checkBoxLockProductText = value;
                OnPropertyChanged("checkBoxLockProductText");
            }
        }

        public String indicatorMenuItemVolumeText
        {
            get { return _indicatorMenuItemVolumeText; }
            set
            {
                _indicatorMenuItemVolumeText = value;
                OnPropertyChanged("indicatorMenuItemVolumeText");
            }
        }

        public String indicatorMenuItemBollingerBandsText
        {
            get { return _indicatorMenuItemBollingerBandsText; }
            set
            {
                _indicatorMenuItemBollingerBandsText = value;
                OnPropertyChanged("indicatorMenuItemBollingerBandsText");
            }
        }

        public String indicatorMenuItemSMAText
        {
            get { return _indicatorMenuItemSMAText; }
            set
            {
                _indicatorMenuItemSMAText = value;
                OnPropertyChanged("indicatorMenuItemSMAText");
            }
        }

        public String indicatorMenuItemSMA1Text
        {
            get { return _indicatorMenuItemSMA1Text; }
            set
            {
                _indicatorMenuItemSMA1Text = value;
                OnPropertyChanged("indicatorMenuItemSMA1Text");
            }
        }

        public String indicatorMenuItemSMA2Text
        {
            get { return _indicatorMenuItemSMA2Text; }
            set
            {
                _indicatorMenuItemSMA2Text = value;
                OnPropertyChanged("indicatorMenuItemSMA2Text");
            }
        }

        public String indicatorMenuItemSMA3Text
        {
            get { return _indicatorMenuItemSMA3Text; }
            set
            {
                _indicatorMenuItemSMA3Text = value;
                OnPropertyChanged("indicatorMenuItemSMA3Text");
            }
        }

        public String indicatorMenuItemMACDText
        {
            get { return _indicatorMenuItemMACDText; }
            set
            {
                _indicatorMenuItemMACDText = value;
                OnPropertyChanged("indicatorMenuItemMACDText");
            }
        }

        public String indicatorMenuItemRSIText
        {
            get { return _indicatorMenuItemRSIText; }
            set
            {
                _indicatorMenuItemRSIText = value;
                OnPropertyChanged("indicatorMenuItemRSIText");
            }
        }

        public String indicatorMenuItemWilliamsRText
        {
            get { return _indicatorMenuItemWilliamsRText; }
            set
            {
                _indicatorMenuItemWilliamsRText = value;
                OnPropertyChanged("indicatorMenuItemWilliamsRText");
            }
        }

        public String indicatorMenuItemRemoveAllText
        {
            get { return _indicatorMenuItemRemoveAllText; }
            set
            {
                _indicatorMenuItemRemoveAllText = value;
                OnPropertyChanged("indicatorMenuItemRemoveAllText");
            }
        }

        public String sciChartThemeText
        {
            get { return _sciChartThemeText; }
            set
            {
                _sciChartThemeText = value;
                OnPropertyChanged("sciChartThemeText");
            }
        }

        public String sciChartSelectedTheme
        {
            get { return _sciChartSelectedTheme; }
            set
            {
                _sciChartSelectedTheme = value;
                UpdateSCIChartThemeStyle();
                OnPropertyChanged("sciChartSelectedTheme");
            }
        }

        public String sciChartThemeLightText
        {
            get { return _sciChartThemeLightText; }
            set
            {
                _sciChartThemeLightText = value;
                OnPropertyChanged("sciChartThemeLightText");
            }
        }

        public String sciChartThemeDarkText
        {
            get { return _sciChartThemeDarkText; }
            set
            {
                _sciChartThemeDarkText = value;
                OnPropertyChanged("sciChartThemeDarkText");
            }
        }

        public String chartPeriodMenuText
        {
            get { return _chartPeriodMenuText; }
            set
            {
                _chartPeriodMenuText = value;
                OnPropertyChanged("chartPeriodMenuText");
            }
        }

        public String chartPeriodMenu1SecText
        {
            get { return _chartPeriodMenu1SecText; }
            set
            {
                _chartPeriodMenu1SecText = value;
                OnPropertyChanged("chartPeriodMenu1SecText");
            }
        }

        public String chartPeriodMenu5SecText
        {
            get { return _chartPeriodMenu5SecText; }
            set
            {
                _chartPeriodMenu5SecText = value;
                OnPropertyChanged("chartPeriodMenu5SecText");
            }
        }

        public String chartPeriodMenu10SecText
        {
            get { return _chartPeriodMenu10SecText; }
            set
            {
                _chartPeriodMenu10SecText = value;
                OnPropertyChanged("chartPeriodMenu10SecText");
            }
        }

        public String chartPeriodMenu15SecText
        {
            get { return _chartPeriodMenu15SecText; }
            set
            {
                _chartPeriodMenu15SecText = value;
                OnPropertyChanged("chartPeriodMenu15SecText");
            }
        }

        public String chartPeriodMenu30SecText
        {
            get { return _chartPeriodMenu30SecText; }
            set
            {
                _chartPeriodMenu30SecText = value;
                OnPropertyChanged("chartPeriodMenu30SecText");
            }
        }

        public String chartPeriodMenu60SecText
        {
            get { return _chartPeriodMenu60SecText; }
            set
            {
                _chartPeriodMenu60SecText = value;
                OnPropertyChanged("chartPeriodMenu60SecText");
            }
        }

        public String chartPeriodMenu300SecText
        {
            get { return _chartPeriodMenu300SecText; }
            set
            {
                _chartPeriodMenu300SecText = value;
                OnPropertyChanged("chartPeriodMenu300SecText");
            }
        }

        public String chartPeriodMenu600SecText
        {
            get { return _chartPeriodMenu600SecText; }
            set
            {
                _chartPeriodMenu600SecText = value;
                OnPropertyChanged("chartPeriodMenu600SecText");
            }
        }

        public String chartPeriodMenu900SecText
        {
            get { return _chartPeriodMenu900SecText; }
            set
            {
                _chartPeriodMenu900SecText = value;
                OnPropertyChanged("chartPeriodMenu900SecText");
            }
        }

        public String chartPeriodMenu1800SecText
        {
            get { return _chartPeriodMenu1800SecText; }
            set
            {
                _chartPeriodMenu1800SecText = value;
                OnPropertyChanged("chartPeriodMenu1800SecText");
            }
        }

        public String chartPeriodMenu3600SecText
        {
            get { return _chartPeriodMenu3600SecText; }
            set
            {
                _chartPeriodMenu3600SecText = value;
                OnPropertyChanged("chartPeriodMenu3600SecText");
            }
        }

        public bool showVolumeIndicator
        {
            get { return _showVolumeIndicator; }
            set
            {
                _showVolumeIndicator = value;

                if (value == false)
                {
                    this.removeVolumeChartView();
                }
                else
                {
                    this.drawVolumeChartView();
                }

                OnPropertyChanged("showVolumeIndicator");
            }
        }

        public bool showSMA1Indicator
        {
            get { return _showSMA1Indicator; }
            set
            {
                _showSMA1Indicator = value;

                if (value == false)
                {
                    this.removeSMA1ChartView();
                }
                else
                {
                    this.drawSMA1ChartView();
                }

                OnPropertyChanged("showSMA1Indicator");
            }
        }

        public bool showSMA2Indicator
        {
            get { return _showSMA2Indicator; }
            set
            {
                _showSMA2Indicator = value;

                if (value == false)
                {
                    this.removeSMA2ChartView();
                }
                else
                {
                    this.drawSMA2ChartView();
                }

                OnPropertyChanged("showSMA2Indicator");
            }
        }

        public bool showSMA3Indicator
        {
            get { return _showSMA3Indicator; }
            set
            {
                _showSMA3Indicator = value;

                if (value == false)
                {
                    this.removeSMA3ChartView();
                }
                else
                {
                    this.drawSMA3ChartView();
                }

                OnPropertyChanged("showSMA3Indicator");
            }
        }

        public bool showBBIndicator
        {
            get { return _showBBIndicator; }
            set
            {
                _showBBIndicator = value;

                if (value == false)
                {
                    this.removeBollingerBands();
                }
                else
                {
                    this.drawBollingerBands();
                }

                OnPropertyChanged("showBBIndicator");
            }
        }

        public bool showMACDIndicator
        {
            get { return _showMACDIndicator; }
            set
            {
                _showMACDIndicator = value;

                if (value == false)
                {
                    this.removeMACDChartView();
                }
                else
                {
                    this.drawMACDChartView();
                }

                OnPropertyChanged("showMACDIndicator");
            }
        }

        public bool showRSIIndicator
        {
            get { return _showRSIIndicator; }
            set
            {
                _showRSIIndicator = value;

                if (value == false)
                {
                    this.removeRSIChartView();
                }
                else
                {
                    this.drawRSIChartView();
                }

                OnPropertyChanged("showRSIIndicator");
            }
        }

        public bool showWilliamsRIndicator
        {
            get { return _showWilliamsRIndicator; }
            set
            {
                _showWilliamsRIndicator = value;

                if (value == false)
                {
                    this.removeWilliamsRChartView();
                }
                else
                {
                    this.drawWilliamsRChartView();
                }

                OnPropertyChanged("showWilliamsRIndicator");
            }
        }

        private ChartIndicatorList chartIndicatorList
        {
            get { return _chartIndicatorList; }
        }

        public String sma1Colour
        {
            get { return _sma1Colour; }
            set
            {
                _sma1Colour = value;
                OnPropertyChanged("sma1Colour");
            }
        }

        public String sma2Colour
        {
            get { return _sma2Colour; }
            set
            {
                _sma2Colour = value;
                OnPropertyChanged("sma2Colour");
            }
        }

        public String sma3Colour
        {
            get { return _sma3Colour; }
            set
            {
                _sma3Colour = value;
                OnPropertyChanged("sma3Colour");
            }
        }

        public String bbUpperLowerColour
        {
            get { return _bbUpperLowerColour; }
            set
            {
                _bbUpperLowerColour = value;
                OnPropertyChanged("bbUpperLowerColour");
            }
        }

        public String bbMiddleColour
        {
            get { return _bbMiddleColour; }
            set
            {
                _bbMiddleColour = value;
                OnPropertyChanged("bbMiddleColour");
            }
        }

        public String macdMACDColour
        {
            get { return _macdMACDColour; }
            set
            {
                _macdMACDColour = value;
                OnPropertyChanged("macdMACDColour");
            }
        }

        public String macdSignalColour
        {
            get { return _macdSignalColour; }
            set
            {
                _macdSignalColour = value;
                OnPropertyChanged("macdSignalColour");
            }
        }

        public String macdHistogramColour
        {
            get { return _macdHistogramColour; }
            set
            {
                _macdHistogramColour = value;
                OnPropertyChanged("macdHistogramColour");
            }
        }

        public String rsiColour
        {
            get { return _rsiColour; }
            set
            {
                _rsiColour = value;
                OnPropertyChanged("rsiColor");
            }
        }

        public String rsiOverBoughtLineColour
        {
            get { return _rsiOverBoughtLineColour; }
            set
            {
                _rsiOverBoughtLineColour = value;
                OnPropertyChanged("rsiOverBoughtLineColour");
            }
        }

        private String rsiOverSoldLineColour
        {
            get { return _rsiOverSoldLineColour; }
            set
            {
                _rsiOverSoldLineColour = value;
                OnPropertyChanged("rsiOverSoldLineColour");
            }
        }

        public String rsiMidLineColour
        {
            get { return _rsiMidLineColour; }
            set
            {
                _rsiMidLineColour = value;
                OnPropertyChanged("rsiMidLineColour");
            }
        }

        public String williamsRColour
        {
            get { return _williamsRColour; }
            set
            {
                _williamsRColour = value;
                OnPropertyChanged("williamsRColour");
            }
        }

        public String williamsROverBoughtLineColour
        {
            get { return _williamsROverBoughtLineColour; }
            set
            {
                _williamsROverBoughtLineColour = value;
                OnPropertyChanged("williamsROverBoughtLineColour");
            }
        }

        public String williamsROverSoldLineColour
        {
            get { return _williamsROverSoldLineColour; }
            set
            {
                _williamsROverSoldLineColour = value;
                OnPropertyChanged("williamsROverSoldLineColour");
            }
        }

        public String williamsRMidLineColour
        {
            get { return _williamsRMidLineColour; }
            set
            {
                _williamsRMidLineColour = value;
                OnPropertyChanged("williamsRMidLineColour");
            }
        }

        public String candlestickUpColour
        {
            get { return _candlestickUpColour; }
            set
            {
                _candlestickUpColour = value;
                OnPropertyChanged("candlestickUpColour");
            }
        }

        public String candlestickDownColour
        {
            get { return _candlestickDownColour; }
            set
            {
                _candlestickDownColour = value;
                OnPropertyChanged("candlestickDownColour");
            }
        }

        public IndexRange XVisibleRange
        {
            get { return _xVisibleRange; }
            set
            {
                if (Equals(_xVisibleRange, value))
                    return;
                _xVisibleRange = value;
                OnPropertyChanged("XVisibleRange");
            }
        }

        public String marketPriceStateLabel
        {
            get { return _marketPriceStateLabel; }
            set { _marketPriceStateLabel = value; OnPropertyChanged("marketPriceStateLabel"); }
        }

        public String marketPriceStateValue
        {
            get { return _marketPriceStateValue; }
            set { _marketPriceStateValue = value; OnPropertyChanged("marketPriceStateValue"); }
        }

        public String marketPriceTimeLabel
        {
            get { return _marketPriceTimeLabel; }
            set { _marketPriceTimeLabel = value; OnPropertyChanged("marketPriceTimeLabel"); }
        }

        public String marketPriceTimeValue
        {
            get { return _marketPriceTimeValue; }
            set { _marketPriceTimeValue = value; OnPropertyChanged("marketPriceTimeValue"); }
        }

        public String marketPriceLastLabel
        {
            get { return _marketPriceLastLabel; }
            set { _marketPriceLastLabel = value; OnPropertyChanged("marketPriceLastLabel"); }
        }

        public String marketPriceLastValue
        {
            get { return _marketPriceLastValue; }
            set { _marketPriceLastValue = value; OnPropertyChanged("marketPriceLastValue"); }
        }

        public String marketPriceChangeLabel
        {
            get { return _marketPriceChangeLabel; }
            set { _marketPriceChangeLabel = value; OnPropertyChanged("marketPriceChangeLabel"); }
        }

        public String marketPriceChangeValue
        {
            get { return _marketPriceChangeValue; }
            set { _marketPriceChangeValue = value; OnPropertyChanged("marketPriceChangeValue"); }
        }

        public String marketPriceChangePercentLabel
        {
            get { return _marketPriceChangePercentLabel; }
            set { _marketPriceChangePercentLabel = value; OnPropertyChanged("marketPriceChangePercentLabel"); }
        }

        public String marketPriceChangePercentValue
        {
            get { return _marketPriceChangePercentValue; }
            set { _marketPriceChangePercentValue = value; OnPropertyChanged("marketPriceChangePercentValue"); }
        }

        public String marketPriceLastQuantityLabel
        {
            get { return _marketPriceLastQuantityLabel; }
            set { _marketPriceLastQuantityLabel = value; OnPropertyChanged("marketPriceLastQuantityLabel"); }
        }

        public String marketPriceLastQuantityValue
        {
            get { return _marketPriceLastQuantityValue; }
            set { _marketPriceLastQuantityValue = value; OnPropertyChanged("marketPriceLastQuantityValue"); }
        }

        public String marketPriceBidQuantityLabel
        {
            get { return _marketPriceBidQuantityLabel; }
            set { _marketPriceBidQuantityLabel = value; OnPropertyChanged("marketPriceBidQuantityLabel"); }
        }

        public String marketPriceBidQuantityValue
        {
            get { return _marketPriceBidQuantityValue; }
            set { _marketPriceBidQuantityValue = value; OnPropertyChanged("marketPriceBidQuantityValue"); }
        }

        public String marketPriceBidLabel
        {
            get { return _marketPriceBidLabel; }
            set { _marketPriceBidLabel = value; OnPropertyChanged("marketPriceBidLabel"); }
        }

        public String marketPriceBidValue
        {
            get { return _marketPriceBidValue; }
            set { _marketPriceBidValue = value; OnPropertyChanged("marketPriceBidValue"); }
        }

        public String marketPriceAskLabel
        {
            get { return _marketPriceAskLabel; }
            set { _marketPriceAskLabel = value; OnPropertyChanged("marketPriceAskLabel"); }
        }

        public String marketPriceAskValue
        {
            get { return _marketPriceAskValue; }
            set { _marketPriceAskValue = value; OnPropertyChanged("marketPriceAskValue"); }
        }

        public String marketPriceAskQuantityLabel
        {
            get { return _marketPriceAskQuantityLabel; }
            set { _marketPriceAskQuantityLabel = value; OnPropertyChanged("marketPriceAskQuantityLabel"); }
        }

        public String marketPriceAskQuantityValue
        {
            get { return _marketPriceAskQuantityValue; }
            set { _marketPriceAskQuantityValue = value; OnPropertyChanged("marketPriceAskQuantityValue"); }
        }

        public String marketPriceVolumeLabel
        {
            get { return _marketPriceVolumeLabel; }
            set { _marketPriceVolumeLabel = value; OnPropertyChanged("marketPriceVolumeLabel"); }
        }

        public String marketPriceVolumeValue
        {
            get { return _marketPriceVolumeValue; }
            set { _marketPriceVolumeValue = value; OnPropertyChanged("marketPriceVolumeValue"); }
        }

        public String marketPriceBlockVolumeValue
        {
            get { return _marketPriceBlockVolumeValue; }
            set { _marketPriceBlockVolumeValue = value; OnPropertyChanged("marketPriceBlockVolumeValue"); }
        }

        public String marketPriceHighLabel
        {
            get { return _marketPriceHighLabel; }
            set { _marketPriceHighLabel = value; OnPropertyChanged("marketPriceHighLabel"); }
        }

        public String marketPriceHighValue
        {
            get { return _marketPriceHighValue; }
            set { _marketPriceHighValue = value; OnPropertyChanged("marketPriceHighValue"); }
        }

        public String marketPriceLowLabel
        {
            get { return _marketPriceLowLabel; }
            set { _marketPriceLowLabel = value; OnPropertyChanged("marketPriceLowLabel"); }
        }

        public String marketPriceLowValue
        {
            get { return _marketPriceLowValue; }
            set { _marketPriceLowValue = value; OnPropertyChanged("marketPriceLowValue"); }
        }

        public String marketPriceOpenLabel
        {
            get { return _marketPriceOpenLabel; }
            set { _marketPriceOpenLabel = value; OnPropertyChanged("marketPriceOpenLabel"); }
        }

        public String marketPriceOpenValue
        {
            get { return _marketPriceOpenValue; }
            set { _marketPriceOpenValue = value; OnPropertyChanged("marketPriceOpenValue"); }
        }

        public String marketPricePreviousCloseLabel
        {
            get { return _marketPricePreviousCloseLabel; }
            set { _marketPricePreviousCloseLabel = value; OnPropertyChanged("marketPricePreviousCloseLabel"); }
        }

        public String marketPricePreviousCloseValue
        {
            get { return _marketPricePreviousCloseValue; }
            set { _marketPricePreviousCloseValue = value; OnPropertyChanged("marketPricePreviousCloseValue"); }
        }

        public String marketPriceCloseDateLabel
        {
            get { return _marketPriceCloseDateLabel; }
            set { _marketPriceCloseDateLabel = value; OnPropertyChanged("marketPriceCloseDateLabel"); }
        }

        public String marketPriceCloseDateValue
        {
            get { return _marketPriceCloseDateValue; }
            set { _marketPriceCloseDateValue = value; OnPropertyChanged("marketPriceCloseDateValue"); }
        }

        public String marketPriceStrikeLabel
        {
            get { return _marketPriceStrikeLabel; }
            set { _marketPriceStrikeLabel = value; OnPropertyChanged("marketPriceStrikeLabel"); }
        }

        public String marketPriceStrikeValue
        {
            get { return _marketPriceStrikeValue; }
            set { _marketPriceStrikeValue = value; OnPropertyChanged("marketPriceStrikeValue"); }
        }
        #endregion

        #region Event Handlers
        private void toggleButtonZoomPan_Click(object sender, RoutedEventArgs e)
        {
            bool bChecked = (sender as ToggleButton).IsChecked.Value;

            if (bChecked == true)
            {
                this.IsPanEnabled = true;
            }
            else
            {
                this.IsPanEnabled = false;
            }
        }

        private void toggleButtonRubberBandZoom_Click(object sender, RoutedEventArgs e)
        {
            bool bChecked = (sender as ToggleButton).IsChecked.Value;

            if (bChecked == true)
            {
                this.IsPanEnabled = false;
            }
            else
            {
                this.IsPanEnabled = true;
            }
        }

        private void buttonZoomExtents_Click(object sender, RoutedEventArgs e)
        {
            if (this.sciChart == null)
            {
                return;
            }

            this.SCIChartResetXPosition(true);
        }

        private void buttonChartResetXPosition_Click(object sender, RoutedEventArgs e)
        {
            if (this.sciChart == null)
            {
                return;
            }

            SCIChartResetXPosition(false);
        }

        private void toggleButtonTickerLine_Clicked(object sender, RoutedEventArgs e)
        {
            bool bChecked = (sender as ToggleButton).IsChecked.Value;

            if (bChecked == true)
            {
                this.IsTickerLineEnabled = true;
                CreateTickerLine();
            }
            else
            {
                this.IsTickerLineEnabled = false;
                this.closeLineAnnotation = null;
            }
        }

        private void toggleButtonChartOverview_Click(object sender, RoutedEventArgs e)
        {
            bool bChecked = (sender as ToggleButton).IsChecked.Value;

            if (bChecked == true)
            {
                this.IsChartOverviewEnabled = true;
                this.gridChartAnalysis.RowDefinitions[this.getGridChartAnalysisRowByName(SCIConstants.SCI_CHARTOVERVIEW_ROW_DEFINITION)].Height = new GridLength(38);
            }
            else
            {
                this.IsChartOverviewEnabled = false;
                this.gridChartAnalysis.RowDefinitions[this.getGridChartAnalysisRowByName(SCIConstants.SCI_CHARTOVERVIEW_ROW_DEFINITION)].Height = new GridLength(0);
            }
        }

        private void buttonIndicatorMenu_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void indicatorMenuItemVolume_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (this.sciChart == null)
            {
                return;
            }

            this.showVolumeIndicator = menuItem.IsChecked;
        }

        private void indicatorMenuItemBollingerBands_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (this.sciChart == null)
            {
                return;
            }

            this.showBBIndicator = menuItem.IsChecked;
        }

        private void indicatorMenuItemSMA1_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (this.sciChart == null)
            {
                return;
            }

            this.showSMA1Indicator = menuItem.IsChecked;
        }

        private void indicatorMenuItemSMA2_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (this.sciChart == null)
            {
                return;
            }

            this.showSMA2Indicator = menuItem.IsChecked;
        }

        private void indicatorMenuItemSMA3_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (this.sciChart == null)
            {
                return;
            }

            this.showSMA3Indicator = menuItem.IsChecked;
        }

        private void indicatorMenuItemMACD_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            this.showMACDIndicator = menuItem.IsChecked;
        }

        private void indicatorMenuItemRSI_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            this.showRSIIndicator = menuItem.IsChecked;
        }

        private void indicatorMenuItemWilliamsR_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            this.showWilliamsRIndicator = menuItem.IsChecked;
        }

        private void indicatorMenuItemRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            this.RemoveChartIndicators(true);
        }

        private void buttonSCIChartTheme_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void sciChartThemeLight_Click(object sender, RoutedEventArgs e)
        {
            if (this.sciChartSelectedTheme.Equals("Chrome") == false)
            {
                this.sciChartSelectedTheme = "Chrome";
            }
        }

        private void sciChartThemeDark_Click(object sender, RoutedEventArgs e)
        {
            if (this.sciChartSelectedTheme.Equals("BlackSteel") == false)
            {
                this.sciChartSelectedTheme = "BlackSteel";
            }
        }

        private void buttonChartPeriodMenu_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void chartPeriodMenu_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            uint period = uint.Parse((String)menuItem.CommandParameter);

            if (period != this.chartPeriod)
            {
                this.ReloadDataSource(period);
            }
        }

        private void sciChart_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.IsMouseCursorEnabled = !this.IsMouseCursorEnabled;
        }

        private void checkBoxLockProduct_Click(object sender, RoutedEventArgs e)
        {
            this.IsLockProductEnabled = (sender as CheckBox).IsChecked.Value;
            this.UpdateChildWindowTitle();
        }
        #endregion

        private void LoadUserChartSettings()
        {
            ChartData data = new ChartData();

            if ((this._chartSettings == null) || (this._chartSettings.Length == 0))
            {
                //Load defaults. If user not exist will always return app defaults.
                data = TradeStationSetting.ReturnCustomizeData(GOSTradeStation.UserID, "ChartData").ChartData;
            }

            this.IsPanEnabled = data.isPan;
            this.IsTickerLineEnabled = data.showTickerLine;
            this.showVolumeIndicator = data.showVolume;
            this.IsChartOverviewEnabled = data.showOverview;
            this.showBBIndicator = data.showBollingerBands;
            this.bbPeriod = data.bollingerBandsPeriod;
            this.bbDevi = data.bollingerBandsDeviation;
            this.bbUpperLowerColour = data.bollingerBandsOuterColour;
            this.bbMiddleColour = data.bollingerBandsSMAColour;
            this.showSMA1Indicator = data.showSMA1;
            this.sma1Period = data.sma1Period;
            this.sma1Colour = data.sma1Colour;
            this.showSMA2Indicator = data.showSMA2;
            this.sma2Period = data.sma2Period;
            this.sma2Colour = data.sma2Colour;
            this.showSMA3Indicator = data.showSMA3;
            this.sma3Period = data.sma3Period;
            this.sma3Colour = data.sma3Colour;
            this.chartPeriod = data.chartPeriod;
            this.showMACDIndicator = data.showMACDIndicator;
            this.macdFastPeriod = data.macdFastPeriod;
            this.macdSlowPeriod = data.macdSlowPeriod;
            this.macdSignalPeriod = data.macdSignalPeriod;
            this.macdMACDColour = data.macdMACDColour;
            this.macdSignalColour = data.macdSignalColour;
            this.macdHistogramColour = data.macdHistogramColour;
            this.showRSIIndicator = data.showRSIIndicator;
            this.rsiPeriod = data.rsiPeriod;
            this.rsiOverBoughtLine = data.rsiOverBoughtLine;
            this.rsiOverSoldLine = data.rsiOverSoldLine;
            this.rsiMidLine = data.rsiMidLine;
            this.rsiColour = data.rsiColour;
            this.rsiOverBoughtLineColour = data.rsiOverBoughtLineColour;
            this.rsiOverSoldLineColour = data.rsiOverSoldLineColour;
            this.rsiMidLineColour = data.rsiMidLineColour;
            this.showWilliamsRIndicator = data.showWilliamsRIndicator;
            this.williamsRPeriod = data.williamsRPeriod;
            this.williamsROverBoughtLine = data.williamsROverBoughtLine;
            this.williamsROverSoldLine = data.williamsROverSoldLine;
            this.williamsRMidLine = data.williamsRMidLine;
            this.williamsRColour = data.williamsRColour;
            this.williamsROverBoughtLineColour = data.williamsROverBoughtLineColour;
            this.williamsROverSoldLineColour = data.williamsROverSoldLineColour;
            this.williamsRMidLineColour = data.williamsRMidLineColour;
            this.candlestickUpColour = data.candlestickUpColour;
            this.candlestickDownColour = data.candlestickDownColour;

            this.chartPeriodMenuText = "Period: " + this.GetChartPeriodAsDisplayText(this.chartPeriod);

            this.indicatorMenuItemSMA1Text = "SMA1 (" + this.sma1Period.ToString() + ")";
            this.indicatorMenuItemSMA2Text = "SMA2 (" + this.sma2Period.ToString() + ")";
            this.indicatorMenuItemSMA3Text = "SMA3 (" + this.sma3Period.ToString() + ")";
            this.indicatorMenuItemBollingerBandsText = "Bollinger Bands (" + this.bbPeriod.ToString() + ", " + this.bbDevi.ToString() + ")";
            this.indicatorMenuItemMACDText = "MACD (" + this.macdFastPeriod.ToString() + "," + this.macdSlowPeriod.ToString() + "," + this.macdSignalPeriod.ToString() + ")";
            this.indicatorMenuItemRSIText = "RSI (" + this.rsiPeriod.ToString() + ")";
            this.indicatorMenuItemWilliamsRText = "Williams %R (" + this.williamsRPeriod.ToString() + ")";

            //Put sciChartSelectedTheme last due to possible dependencies
            this.sciChartSelectedTheme = SCICommon.ConvertBasicNameToChartTheme(data.theme);
        }

        private void InvalidateObjects()
        {
            UpdateMarketPriceControlValues(null);

            this.sciChart.IsEnabled = false;
            this.sciOverviewChartGrid.IsEnabled = false;
            this.disableAllChartViews();

            this.bReceivedChartHistory = false;

            this.closeLineAnnotation = null;
            this.chartHistoryReceivedAsyncDelegate = null;
            this.marketPriceReceivedAsyncDelegate = null;

            try
            {
                if (this.chartHistory != null)
                {
                    this.chartHistory.Dispose();
                }

                if (this.marketPrice != null)
                {
                    this.marketPrice.Dispose();
                }
            }
            catch { }

            this.chartHistory = null;
            this.marketPrice = null;
            this.chartDataSource = null;
            this.chartVolumeSource = null;
            this.totalChartHistoryVolume = 0;
        }

        private void ReloadDataSource(uint chartPeriod)
        {
            if (this.productListReceived == false)
            {
                return;
            }

            this.RemoveChartIndicators(false);
            this.InvalidateObjects();
            this.IsMouseCursorEnabled = false;

            String newProductCode = this.tbProductCode.Text.Trim();
            this.productCode = newProductCode;
            this.ProductName = MarketPriceData.GetProductName(this.productCode);
            this.UpdateChildWindowTitle();

            if (MarketPriceData.ExistInProduct(newProductCode) == false)
            {
                if (newProductCode.Length > 0)
                {
                    this.ProductName = "Not Exist";
                }

                return;
            }

            try
            {
                String obj = Convert.ToString(chartPeriod);
                ReloadDataSourceToDispatcherDelegate del = new ReloadDataSourceToDispatcherDelegate(ReloadDataSourceToDispatcher);
                Application.Current.Dispatcher.Invoke(del, new Object[] { obj });
            }
            catch { }
        }

        private void ReloadDataSourceToDispatcher(String obj)
        {
            if (this.thisLoaded == false)
            {
                return;
            }

            if (this.productCode == null)
            {
                return;
            }

            if (this.productCode.Length == 0)
            {
                return;
            }

            this.chartPeriod = Convert.ToUInt32(obj);
            this.chartPeriodMenuText = "Period: " + this.GetChartPeriodAsDisplayText(this.chartPeriod);
            this.chartHistoryReceivedAsyncDelegate = new ChartHistoryReceivedAsyncDelegate(ChartHistoryReceivedAsyncDelegateMethod);
            this.chartHistory = new SCIChartAnalysisChartHistory(this.distributeMsg, this.productCode, this.chartPeriod, new ChartHistoryReceivedDelegate(ChartHistoryReceived), this.showChartBreakPeriods);
        }

        public void ChartHistoryReceived(OHLCPriceSeries.PriceSeries priceSeries, long totalChartHistoryVolume)
        {
            try
            {
                if (this.chartHistoryReceivedAsyncDelegate != null)
                {
                    object vol = totalChartHistoryVolume;
                    Application.Current.Dispatcher.Invoke(this.chartHistoryReceivedAsyncDelegate, new Object[] { priceSeries, vol });
                }
            }
            catch { }
        }

        public void MarketPriceReceived(MarketPriceItem MktItems, bool newDataPoint)
        {
            try
            {
                if (this.marketPriceReceivedAsyncDelegate != null)
                {
                    Application.Current.Dispatcher.Invoke(this.marketPriceReceivedAsyncDelegate, new Object[] { MktItems, (System.Boolean)newDataPoint });
                }
            }
            catch { }
        }

        private void tbProductCodeOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.enteredNewProduct();
            }
        }

        private void tbProductCodeLostFocus(object sender, RoutedEventArgs e)
        {
            this.enteredNewProduct();
        }

        private void enteredNewProduct()
        {
            String newProductCode = tbProductCode.Text.Trim();

            if (newProductCode.Equals(this.productCode) == true)
            {
                return;
            }

            if (newProductCode == System.String.Empty)
            {
                return;
            }

            this.ReloadDataSource(this.chartPeriod);
        }

        private void ChartHistoryReceivedAsyncDelegateMethod(OHLCPriceSeries.PriceSeries priceSeries, long totalChartHistoryVolume)
        {
            try
            {
                if (this.chartHistory != null)
                {
                    this.chartHistory.Dispose();
                }
            }
            catch { }

            this.chartHistory = null;
            this.chartHistoryReceivedAsyncDelegate = null;

            if ((this.thisLoaded == false) && (this.bReceivedChartHistory == true))
            {
                return;
            }

            this.bReceivedChartHistory = true;

            this.chartDataSource = new OhlcDataSeries<DateTime, double>();
            this.chartVolumeSource = new XyDataSeries<DateTime, long>();
            this.totalChartHistoryVolume = 0;

            if (priceSeries.Count > 0)
            {
                this.chartDataSource.Append(priceSeries.TimeData, priceSeries.OpenData, priceSeries.HighData, priceSeries.LowData, priceSeries.CloseData);
                this.chartVolumeSource.Append(priceSeries.TimeData, priceSeries.VolumeData);
                this.totalChartHistoryVolume = totalChartHistoryVolume;

                this.AddCharts();
            }

            this.marketPriceReceivedAsyncDelegate = new MarketPriceReceivedAsyncDelegate(MarketPriceReceivedAsyncDelegateMethod);
            this.marketPrice = new SCIChartAnalysisMarketPrice(this.distributeMsg, this.productCode, this.chartPeriod, new MarketPriceReceivedDelegate(MarketPriceReceived), priceSeries);
        }

        private void MarketPriceReceivedAsyncDelegateMethod(MarketPriceItem MktItems, bool newDataPoint)
        {
            this.UpdateMarketPriceControlValues(MktItems);

            if (this.bReceivedChartHistory == false) return;
            if (this.marketPriceReceivedAsyncDelegate == null) return;
            if ((this.chartDataSource == null) || (this.chartVolumeSource == null)) return;
            if (this.sciChart == null) return;
            if (MktItems == null) return;
            if (MktItems.Datetime == null) return;

            double last = TradeStationTools.ConvertToDouble(MktItems.Last);
            long volume = TradeStationTools.ConvertToLong(MktItems.Volume) - TradeStationTools.ConvertToLong(MktItems.EP);

            if (last == 0) return;
            if (volume == 0) return;
            if (volume == this.totalChartHistoryVolume) return;

            using (this.sciChart.SuspendUpdates())
            {
                DateTime mktDateTime = (DateTime)MktItems.Datetime;
                int dataSourceIndex = -1;

                mktDateTime = SCICommon.FilterMarketPriceTime(mktDateTime, this.chartPeriod);
                String mktDateTimeAsStr = mktDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                mktDateTime = DateTime.ParseExact(mktDateTimeAsStr, "yyyy-MM-dd HH:mm:ss", null);

                if (this.chartDataSource.Count > 0)
                {
                    dataSourceIndex = this.chartDataSource.FindIndex(mktDateTime, Abt.Controls.SciChart.Common.Extensions.SearchMode.Exact);
                }

                if (dataSourceIndex == -1)
                {
                    this.chartDataSource.Append(mktDateTime, last, last, last, last);

                    if (this.chartDataSource.Count <= 1)
                    {
                        this.AddCharts();
                        this.SCIChartResetXPosition(false);
                    }
                    else
                    {
                        if ((int)this.sciChart.XAxis.VisibleRange.Max >= this.chartDataSource.Count)
                        {
                            if (this.chartDataSource.Count > SCIConstants.SCI_DEFAULT_VIEW_POINTS)
                            {
                                var existingRange = _xVisibleRange;
                                var newRange = new IndexRange(existingRange.Min + 1, existingRange.Max + 1);
                                this.XVisibleRange = newRange;
                            }
                            else
                            {
                                this.SCIChartResetXPosition(false);
                            }
                        }

                        UpdateChartIndicators(true, MktItems, mktDateTime);
                    }
                }
                else
                {
                    double open = this.chartDataSource.OpenValues[dataSourceIndex];
                    double high = this.chartDataSource.HighValues[dataSourceIndex];
                    double low = this.chartDataSource.LowValues[dataSourceIndex];

                    if (last > high)
                    {
                        high = last;
                    }

                    if (last < low)
                    {
                        low = last;
                    }

                    this.chartDataSource.Update(mktDateTime, open, high, low, last);

                    UpdateChartIndicators(false, MktItems, mktDateTime);
                }

                dataSourceIndex = -1;

                if (this.chartVolumeSource.Count > 0)
                {
                    dataSourceIndex = this.chartVolumeSource.FindIndex(mktDateTime, Abt.Controls.SciChart.Common.Extensions.SearchMode.Exact);
                }

                long volDiff = volume;

                if (volume >= this.totalChartHistoryVolume)
                {
                    volDiff -= this.totalChartHistoryVolume;
                }

                if (dataSourceIndex == -1)
                {
                    this.chartVolumeSource.Append(mktDateTime, volDiff);
                }
                else
                {
                    if (this.chartVolumeSource.Count > 0)
                    {
                        long currentVolume = this.chartVolumeSource.YValues[this.chartVolumeSource.Count - 1];
                        currentVolume += volDiff;
                        this.chartVolumeSource.Update(mktDateTime, currentVolume);
                    }
                }

                this.totalChartHistoryVolume = volume;

                if (this.closeLineAnnotation != null)
                {
                    this.closeLineAnnotation.Y1 = MktItems.Last;
                }
            }
        }

        private void AddCharts()
        {
            if (this.sciChart == null)
            {
                return;
            }

            if (this.chartDataSource == null)
            {
                return;
            }

            if (this.chartDataSource.Count <= 0)
            {
                return;
            }

            this.sciChart.RenderableSeries[0].DataSeries = this.chartDataSource;
            this.sciChart.XAxis.LabelFormatter = SCICommon.CreateTimeAxisFormatter(this.chartPeriod);
            this.sciChart.YAxis.AutoRange = Abt.Controls.SciChart.Visuals.Axes.AutoRange.Always;
            this.sciChart.XAxis.GrowBy.Max = 0.01;
            SCIChartResetXPosition(false);

            if (this.indicatorsOrderList != null)
            {
                if (this.indicatorsOrderList.Count == 0)
                {
                    this.indicatorsOrderList = null;
                }
                else
                {
                    foreach (SCIConstants.ChartIndicatorType type in this.indicatorsOrderList)
                    {
                        switch (type)
                        {
                            case SCIConstants.ChartIndicatorType.CANDLESTICK:
                                break;

                            case SCIConstants.ChartIndicatorType.VOLUME:
                                this.showVolumeIndicator = this.showVolumeIndicator;
                                break;

                            case SCIConstants.ChartIndicatorType.BOLLINGERBANDS:
                                this.showBBIndicator = this.showBBIndicator;
                                break;

                            case SCIConstants.ChartIndicatorType.SMA1:
                                this.showSMA1Indicator = this.showSMA1Indicator;
                                break;

                            case SCIConstants.ChartIndicatorType.SMA2:
                                this.showSMA2Indicator = this.showSMA2Indicator;
                                break;

                            case SCIConstants.ChartIndicatorType.SMA3:
                                this.showSMA3Indicator = this.showSMA3Indicator;
                                break;

                            case SCIConstants.ChartIndicatorType.MACD:
                                this.showMACDIndicator = this.showMACDIndicator;
                                break;

                            case SCIConstants.ChartIndicatorType.RSI:
                                this.showRSIIndicator = this.showRSIIndicator;
                                break;

                            case SCIConstants.ChartIndicatorType.WILLIAMSR:
                                this.showWilliamsRIndicator = this.showWilliamsRIndicator;
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            if (this.indicatorsOrderList == null)
            {
                this.showVolumeIndicator = this.showVolumeIndicator;
                this.showSMA1Indicator = this.showSMA1Indicator;
                this.showSMA2Indicator = this.showSMA2Indicator;
                this.showSMA3Indicator = this.showSMA3Indicator;
                this.showBBIndicator = this.showBBIndicator;
                this.showMACDIndicator = this.showMACDIndicator;
                this.showRSIIndicator = this.showRSIIndicator;
                this.showWilliamsRIndicator = this.showWilliamsRIndicator;
            }

            this.indicatorsOrderList = null;

            CreateTickerLine();

            this.sciChart.IsEnabled = true;
            this.sciOverviewChartGrid.IsEnabled = true;
            this.enableAllChartViews();
        }

        private void RemoveChartIndicators(bool clearFlags)
        {
            if (this.sciChart == null)
            {
                return;
            }

            if (clearFlags == false)
            {
                this.indicatorsOrderList = this.chartIndicatorList.GetIndicatorsListOrder();
            }

            this.chartIndicatorList.RemoveAllIndicators();

            if (clearFlags == true)
            {
                this.indicatorsOrderList = null;

                this._showVolumeIndicator = false;
                this._showSMA1Indicator = false;
                this._showSMA2Indicator = false;
                this._showSMA3Indicator = false;
                this._showBBIndicator = false;
                this._showMACDIndicator = false;
                this._showRSIIndicator = false;
                this._showWilliamsRIndicator = false;
            }
        }

        private void CreateTickerLine()
        {
            if (this.IsTickerLineEnabled == false)
            {
                return;
            }

            if (this.sciChart == null)
            {
                return;
            }

            if (this.chartDataSource == null)
            {
                return;
            }

            if (this.chartDataSource.Count <= 0)
            {
                return;
            }

            double YValue = this.chartDataSource.CloseValues[this.chartDataSource.Count - 1];

            this.closeLineAnnotation = new HorizontalLineAnnotation();
            this.closeLineAnnotation.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.closeLineAnnotation.Stroke = Brushes.DarkOrange;
            this.closeLineAnnotation.StrokeThickness = 0.5;
            this.closeLineAnnotation.LabelPlacement = LabelPlacement.Axis;
            this.closeLineAnnotation.ShowLabel = true;
            this.closeLineAnnotation.Y1 = YValue;
            this.closeLineAnnotation.AnnotationLabels[0].FontWeight = FontWeights.SemiBold;
            this.closeLineAnnotation.AnnotationLabels[0].Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));

            using (this.sciChart.SuspendUpdates())
            {
                this.sciChart.Annotations.Add(this.closeLineAnnotation);
            }
        }

        private void UpdateChartIndicators(bool addNewPoint, MarketPriceItem MktItems, DateTime time)
        {
            if (this.sciChart == null)
            {
                return;
            }

            foreach (ChartIndicatorBase indicator in this.chartIndicatorList)
            {
                indicator.UpdateRenderSeries(addNewPoint, MktItems, time);
            }
        }

        private void UpdateSCIChartThemeStyle()
        {
            if (this.sciChart == null)
            {
                return;
            }

            FastCandlestickRenderableSeries candlestickSeries = (FastCandlestickRenderableSeries)this.sciChart.RenderableSeries[0];

            if (this.sciChartSelectedTheme.Equals("Chrome") == true)
            {
                this.sciChart.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));

                candlestickSeries.UpBodyBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.candlestickUpColour));
                candlestickSeries.DownBodyBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.candlestickDownColour));
                candlestickSeries.UpWickColor = (Color)ColorConverter.ConvertFromString(this.candlestickUpColour);
                candlestickSeries.DownWickColor = (Color)ColorConverter.ConvertFromString(this.candlestickDownColour);
                candlestickSeries.MinWidth = 0.1;

                Style sciChartXAxisMajorGridLineStyle = new Style(typeof(Line));
                sciChartXAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF2F2F2"))));
                this.sciChart.XAxis.MajorGridLineStyle = sciChartXAxisMajorGridLineStyle;

                Style sciChartYAxisMajorGridLineStyle = new Style(typeof(Line));
                sciChartYAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF2F2F2"))));
                this.sciChart.YAxis.MajorGridLineStyle = sciChartYAxisMajorGridLineStyle;

                this.sciChart.XAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
                this.sciChart.YAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));

                CreateTickerLine();

                this.sciOverviewChartGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            }
            else
            {
                this.sciChart.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));

                candlestickSeries.UpBodyBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.candlestickUpColour));
                candlestickSeries.DownBodyBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.candlestickDownColour));
                candlestickSeries.UpWickColor = (Color)ColorConverter.ConvertFromString(this.candlestickUpColour);
                candlestickSeries.DownWickColor = (Color)ColorConverter.ConvertFromString(this.candlestickDownColour);

                Style xAxisMajorGridLineStyle = new Style(typeof(Line));
                xAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF404040"))));
                this.sciChart.XAxis.MajorGridLineStyle = xAxisMajorGridLineStyle;

                Style yAxisMajorGridLineStyle = new Style(typeof(Line));
                yAxisMajorGridLineStyle.Setters.Add(new Setter(Line.StrokeProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF202020"))));
                this.sciChart.YAxis.MajorGridLineStyle = yAxisMajorGridLineStyle;

                this.sciChart.XAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                this.sciChart.YAxis.TickTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));

                CreateTickerLine();

                this.sciOverviewChartGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
            }

            foreach (ChartIndicatorBase indicator in this.chartIndicatorList)
            {
                indicator.UpdateSCIChartThemeStyle(this.sciChartSelectedTheme);
            }
        }

        private void SCIChartResetXPosition(bool zoomExtents)
        {
            if (this.chartDataSource == null)
            {
                return;
            }

            int maxRange = this.chartDataSource.Count;

            if (maxRange <= 0)
            {
                return;
            }

            int minRange = maxRange - SCIConstants.SCI_DEFAULT_VIEW_POINTS;
            if (minRange <= 0)
            {
                minRange = 0;
            }

            double padding = (maxRange - minRange) * 0.05;

            if (padding < 1)
            {
                padding = 1;
            }

            if (minRange > 0)
            {
                minRange += (int)padding;
            }

            if (zoomExtents == true)
            {
                minRange = 0;
            }

            maxRange += (int)padding;

            this.XVisibleRange = new IndexRange(minRange, maxRange);
        }

        private String GetChartPeriodAsDisplayText(uint chartPeriod)
        {
            String text = "";

            switch (chartPeriod)
            {
                case 1: text = "1 Sec"; break;
                case 5: text = "5 Sec"; break;
                case 10: text = "10 Sec"; break;
                case 15: text = "15 Sec"; break;
                case 30: text = "30 Sec"; break;
                case 60: text = "1 Min"; break;
                case 300: text = "5 Min"; break;
                case 600: text = "10 Min"; break;
                case 900: text = "15 Min"; break;
                case 1800: text = "30 Min"; break;
                case 3600: text = "60 Min"; break;
            }

            return text;
        }

        protected void distributeMsg_DisControl(object sender, string prodCode)
        {
            distributeMsg_DisControlAsyncDelegate del = new distributeMsg_DisControlAsyncDelegate(distributeMsg_DisControlAsyncDelegateMethod);
            Application.Current.Dispatcher.Invoke(del, new Object[] { sender, prodCode });
        }

        private void distributeMsg_DisControlAsyncDelegateMethod(object sender, string prodCode)
        {
            if (this.thisLoaded == false)
            {
                return;
            }

            if (prodCode.Equals(this.productCode) == true)
            {
                return;
            }

            if (this.IsLockProductEnabled == true)
            {
                return;
            }

            this.tbProductCode.Text = prodCode;
            tbProductCode.Text.Trim();
            tbProductCode.CaretIndex = tbProductCode.Text.Length;

            this.ReloadDataSource(this.chartPeriod);
        }

        protected void distributeMsg_DisDisGotProductList(object sender)
        {
            distributeMsg_DisDisGotProductListAsyncDelegate del = new distributeMsg_DisDisGotProductListAsyncDelegate(distributeMsg_DisDisGotProductListAsyncDelegateMethod);
            Application.Current.Dispatcher.Invoke(del, new Object[] { sender });
        }

        private void distributeMsg_DisDisGotProductListAsyncDelegateMethod(object sender)
        {
            //Debug.WriteLine("distributeMsg_DisDisGotProductListAsyncDelegateMethod [{0}]" ,this.GetHashCode());

            this.productListReceived = true;

            if (this.thisLoaded == false)
            {
                return;
            }

            if ((this.chartHistory != null) || (this.marketPrice != null))
            {
                return;
            }

            this.ReloadDataSource(this.chartPeriod);
        }

        private void UpdateChildWindowTitle()
        {
            String oldTitle = this.sciMDIChild.Title;
            String title = TradeStationSetting.ReturnWindowName(WindowTypes.SCIChartAnalysis, this.productCode); ;
            if (this.IsLockProductEnabled == true)
            {
                title += " *";
            }

            title += " [" + this.createTime.ToString() + "]";

            this.sciMDIChild.Title = title;
            this.distributeMsg.DistributeControlFocus(oldTitle, this.sciMDIChild);
        }

        private String getChartSettings()
        {
            String setting = "";

            setting += "IsPanEnabled=" + this.IsPanEnabled.ToString().ToUpper() + ",";
            setting += "IsTickerLineEnabled=" + this.IsTickerLineEnabled.ToString().ToUpper() + ",";
            setting += "showVolumeIndicator=" + this.showVolumeIndicator.ToString().ToUpper() + ",";
            setting += "IsChartOverviewEnabled=" + this.IsChartOverviewEnabled.ToString().ToUpper() + ",";
            setting += "showBBIndicator=" + this.showBBIndicator.ToString().ToUpper() + ",";
            setting += "bbPeriod=" + this.bbPeriod.ToString().ToUpper() + ",";
            setting += "bbDevi=" + this.bbDevi.ToString().ToUpper() + ",";
            setting += "bbUpperLowerColour=" + this.bbUpperLowerColour.ToUpper() + ",";
            setting += "bbMiddleColour=" + this.bbMiddleColour.ToUpper() + ",";
            setting += "showSMA1Indicator=" + this.showSMA1Indicator.ToString().ToUpper() + ",";
            setting += "sma1Period=" + this.sma1Period.ToString().ToUpper() + ",";
            setting += "sma1Colour=" + this.sma1Colour.ToUpper() + ",";
            setting += "showSMA2Indicator=" + this.showSMA2Indicator.ToString().ToUpper() + ",";
            setting += "sma2Period=" + this.sma2Period.ToString().ToUpper() + ",";
            setting += "sma2Colour=" + this.sma2Colour.ToUpper() + ",";
            setting += "showSMA3Indicator=" + this.showSMA3Indicator.ToString().ToUpper() + ",";
            setting += "sma3Period=" + this.sma3Period.ToString().ToUpper() + ",";
            setting += "sma3Colour=" + this.sma3Colour.ToUpper() + ",";
            setting += "sciChartSelectedTheme=" + SCICommon.ConvertChartThemeToBasicName(this.sciChartSelectedTheme) + ",";
            setting += "chartPeriod=" + this.chartPeriod.ToString().ToUpper() + ",";
            setting += "showMACDIndicator=" + this.showMACDIndicator.ToString().ToUpper() + ",";
            setting += "macdFastPeriod=" + this.macdFastPeriod.ToString().ToUpper() + ",";
            setting += "macdSlowPeriod=" + this.macdSlowPeriod.ToString().ToUpper() + ",";
            setting += "macdSignalPeriod=" + this.macdSignalPeriod.ToString().ToUpper() + ",";
            setting += "macdMACDColour=" + this.macdMACDColour.ToUpper() + ",";
            setting += "macdSignalColour=" + this.macdSignalColour.ToUpper() + ",";
            setting += "macdHistogramColour=" + this.macdHistogramColour.ToUpper() + ",";
            setting += "showRSIIndicator=" + this.showRSIIndicator.ToString().ToUpper() + ",";
            setting += "rsiPeriod=" + this.rsiPeriod.ToString().ToUpper() + ",";
            setting += "rsiOverBoughtLine=" + this.rsiOverBoughtLine.ToString().ToUpper() + ",";
            setting += "rsiOverSoldLine=" + this.rsiOverSoldLine.ToString().ToUpper() + ",";
            setting += "rsiMidLine=" + this.rsiMidLine.ToString().ToUpper() + ",";
            setting += "rsiColour=" + this.rsiColour.ToUpper() + ",";
            setting += "rsiOverBoughtLineColour=" + this.rsiOverBoughtLineColour.ToUpper() + ",";
            setting += "rsiOverSoldLineColour=" + this.rsiOverSoldLineColour.ToUpper() + ",";
            setting += "rsiMidLineColour=" + this.rsiMidLineColour.ToUpper() + ",";
            setting += "showWilliamsRIndicator=" + this.showWilliamsRIndicator.ToString().ToUpper() + ",";
            setting += "williamsRPeriod=" + this.williamsRPeriod.ToString().ToUpper() + ",";
            setting += "williamsROverBoughtLine=" + this.williamsROverBoughtLine.ToString().ToUpper() + ",";
            setting += "williamsROverSoldLine=" + this.williamsROverSoldLine.ToString().ToUpper() + ",";
            setting += "williamsRMidLine=" + this.williamsRMidLine.ToString().ToUpper() + ",";
            setting += "williamsRColour=" + this.williamsRColour.ToUpper() + ",";
            setting += "williamsROverBoughtLineColour=" + this.williamsROverBoughtLineColour.ToUpper() + ",";
            setting += "williamsROverSoldLineColour=" + this.williamsROverSoldLineColour.ToUpper() + ",";
            setting += "williamsRMidLineColour=" + this.williamsRMidLineColour.ToUpper() + ",";
            setting += "candlestickUpColour=" + this.candlestickUpColour.ToUpper() + ",";
            setting += "candlestickDownColour=" + this.candlestickDownColour.ToUpper();

            return setting;
        }

        private String setChartSettings(String settings)
        {
            String[] split = settings.Split(',');
            uint i = 0;

            if (split.Length >= (i + 1))
            {
                String[] IsPanEnabledSplit = split[i++].Split('=');
                if (IsPanEnabledSplit.Length >= 2)
                {
                    this.IsPanEnabled = IsPanEnabledSplit[1].ToUpper().Equals("TRUE") ? true : false;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] IsTickerLineEnabledSplit = split[i++].Split('=');
                if (IsTickerLineEnabledSplit.Length >= 2)
                {
                    this.IsTickerLineEnabled = IsTickerLineEnabledSplit[1].ToUpper().Equals("TRUE") ? true : false;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] showVolumeIndicatorSplit = split[i++].Split('=');
                if (showVolumeIndicatorSplit.Length >= 2)
                {
                    this.showVolumeIndicator = showVolumeIndicatorSplit[1].ToUpper().Equals("TRUE") ? true : false;
                }
            }

            if (split.Length >= (i + 1)) 
            {
                String[] IsChartOverviewEnabledSplit = split[i++].Split('=');
                if (IsChartOverviewEnabledSplit.Length >= 2)
                {
                    this.IsChartOverviewEnabled = IsChartOverviewEnabledSplit[1].ToUpper().Equals("TRUE") ? true : false;
                }
            }

            if (split.Length >= (i + 1)) 
            {
                String[] showBBIndicatorSplit = split[i++].Split('=');
                if (showBBIndicatorSplit.Length >= 2)
                {
                    this.showBBIndicator = showBBIndicatorSplit[1].ToUpper().Equals("TRUE") ? true : false;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] bbPeriodSplit = split[i++].Split('=');
                if (bbPeriodSplit.Length >= 2)
                {
                    uint bbPeriodSetting = TradeStationTools.ConvertToUInt(bbPeriodSplit[1]);
                    this.bbPeriod = (bbPeriodSetting < SCIConstants.SCI_MINIMUM_BOLLINGER_BANDS_PERIOD) ? SCIConstants.SCI_DEFAULT_BOLLINGER_BANDS_PERIOD : bbPeriodSetting;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] bbDeviSplit = split[i++].Split('=');
                if (bbDeviSplit.Length >= 2)
                {
                    double bbDeviSetting = TradeStationTools.ConvertToDouble(bbDeviSplit[1]);
                    this.bbDevi = ((bbDeviSetting < SCIConstants.SCI_MINIMUM_BOLLINGER_BANDS_DEVIATION) || (bbDeviSetting > SCIConstants.SCI_MAXIMUM_BOLLINGER_BANDS_DEVIATION)) ? SCIConstants.SCI_DEFAULT_BOLLINGER_BANDS_DEVIATION : bbDeviSetting;
                }
            }

            if (split.Length >= (i + 1)) 
            {
                String[] bbUpperLowerColourSplit = split[i++].Split('=');
                if (bbUpperLowerColourSplit.Length >= 2)
                {
                    this.bbUpperLowerColour = SCICommon.ValidateColourValue(bbUpperLowerColourSplit[1], SCIConstants.SCI_DEFAULT_BOLLINGER_BANDS_OUTER_COLOUR);
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] bbMiddleColourSplit = split[i++].Split('=');
                if (bbMiddleColourSplit.Length >= 2)
                {
                    this.bbMiddleColour = SCICommon.ValidateColourValue(bbMiddleColourSplit[1], SCIConstants.SCI_DEFAULT_BOLLINGER_BANDS_SMA_COLOUR);
                }
            }

            if (split.Length >= (i + 1)) 
            {
                String[] showSMA1IndicatorSplit = split[i++].Split('=');
                if (showSMA1IndicatorSplit.Length >= 2)
                {
                    this.showSMA1Indicator = showSMA1IndicatorSplit[1].ToUpper().Equals("TRUE") ? true : false;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] sma1PeriodSplit = split[i++].Split('=');
                if (sma1PeriodSplit.Length >= 2)
                {
                    uint sma1PeriodSetting = TradeStationTools.ConvertToUInt(sma1PeriodSplit[1]);
                    this.sma1Period = (sma1PeriodSetting == 0) ? SCIConstants.SCI_DEFAULT_SMA1_PERIOD : sma1PeriodSetting;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] sma1ColourSplit = split[i++].Split('=');
                if (sma1ColourSplit.Length >= 2)
                {
                    this.sma1Colour = SCICommon.ValidateColourValue(sma1ColourSplit[1], SCIConstants.SCI_DEFAULT_SMA1_COLOUR);
                }
            }

            if (split.Length >= (i + 1)) 
            {
                String[] showSMA2IndicatorSplit = split[i++].Split('=');
                if (showSMA2IndicatorSplit.Length >= 2)
                {
                    this.showSMA2Indicator = showSMA2IndicatorSplit[1].ToUpper().Equals("TRUE") ? true : false;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] sma2PeriodSplit = split[i++].Split('=');
                if (sma2PeriodSplit.Length >= 2)
                {
                    uint sma2PeriodSetting = TradeStationTools.ConvertToUInt(sma2PeriodSplit[1]);
                    this.sma2Period = (sma2PeriodSetting == 0) ? SCIConstants.SCI_DEFAULT_SMA2_PERIOD : sma2PeriodSetting;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] sma2ColourSplit = split[i++].Split('=');
                if (sma2ColourSplit.Length >= 2)
                {
                    this.sma2Colour = SCICommon.ValidateColourValue(sma2ColourSplit[1], SCIConstants.SCI_DEFAULT_SMA2_COLOUR);
                }
            }

            if (split.Length >= (i + 1)) 
            {
                String[] showSMA3IndicatorSplit = split[i++].Split('=');
                if (showSMA3IndicatorSplit.Length >= 2)
                {
                    this.showSMA3Indicator = showSMA3IndicatorSplit[1].ToUpper().Equals("TRUE") ? true : false;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] sma3PeriodSplit = split[i++].Split('=');
                if (sma3PeriodSplit.Length >= 2)
                {
                    uint sma3PeriodSetting = TradeStationTools.ConvertToUInt(sma3PeriodSplit[1]);
                    this.sma3Period = (sma3PeriodSetting == 0) ? SCIConstants.SCI_DEFAULT_SMA3_PERIOD : sma3PeriodSetting;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] sma3ColourSplit = split[i++].Split('=');
                if (sma3ColourSplit.Length >= 2)
                {
                    this.sma3Colour = SCICommon.ValidateColourValue(sma3ColourSplit[1], SCIConstants.SCI_DEFAULT_SMA3_COLOUR);
                }
            }

            if (split.Length >= (i + 1)) 
            {
                String[] sciChartSelectedThemeSplit = split[i++].Split('=');
                if (sciChartSelectedThemeSplit.Length >= 2)
                {
                    this.sciChartSelectedTheme = SCICommon.ConvertBasicNameToChartTheme(sciChartSelectedThemeSplit[1]);
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] chartPeriodSplit = split[i++].Split('=');
                if (chartPeriodSplit.Length >= 2)
                {
                    uint chartPeriodSetting = TradeStationTools.ConvertToUInt(chartPeriodSplit[1]);
                    this.chartPeriod = (chartPeriodSetting == 0) ? SCIConstants.SCI_DEFAULT_CHART_PERIOD : chartPeriodSetting;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] showMACDIndicatorSplit = split[i++].Split('=');
                if (showMACDIndicatorSplit.Length >= 2)
                {
                    this.showMACDIndicator = showMACDIndicatorSplit[1].ToUpper().Equals("TRUE") ? true : false;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] macdFastPeriodSplit = split[i++].Split('=');
                if (macdFastPeriodSplit.Length >= 2)
                {
                    uint macdFastPeriodSetting = TradeStationTools.ConvertToUInt(macdFastPeriodSplit[1]);
                    this.macdFastPeriod = (macdFastPeriodSetting == 0) ? SCIConstants.SCI_DEFAULT_MACD_FAST_PERIOD : macdFastPeriodSetting;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] macdSlowPeriodSplit = split[i++].Split('=');
                if (macdSlowPeriodSplit.Length >= 2)
                {
                    uint macdSlowPeriodSetting = TradeStationTools.ConvertToUInt(macdSlowPeriodSplit[1]);
                    this.macdSlowPeriod = (macdSlowPeriodSetting == 0) ? SCIConstants.SCI_DEFAULT_MACD_SLOW_PERIOD : macdSlowPeriodSetting;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] macdSignalPeriodSplit = split[i++].Split('=');
                if (macdSignalPeriodSplit.Length >= 2)
                {
                    uint macdSignalPeriodSetting = TradeStationTools.ConvertToUInt(macdSignalPeriodSplit[1]);
                    this.macdSignalPeriod = (macdSignalPeriodSetting == 0) ? SCIConstants.SCI_DEFAULT_MACD_SIGNAL_PERIOD : macdSignalPeriodSetting;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] macdMACDColourSplit = split[i++].Split('=');
                if (macdMACDColourSplit.Length >= 2)
                {
                    this.macdMACDColour = SCICommon.ValidateColourValue(macdMACDColourSplit[1], SCIConstants.SCI_DEFAULT_MACD_MACD_COLOUR);
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] macdSignalColourSplit = split[i++].Split('=');
                if (macdSignalColourSplit.Length >= 2)
                {
                    this.macdSignalColour = SCICommon.ValidateColourValue(macdSignalColourSplit[1], SCIConstants.SCI_DEFAULT_MACD_SIGNAL_COLOUR);
                }
            }

            if (split.Length >= (i + 1)) 
            {
                String[] macdHistogramColourSplit = split[i++].Split('=');
                if (macdHistogramColourSplit.Length >= 2)
                {
                    this.macdHistogramColour = SCICommon.ValidateColourValue(macdHistogramColourSplit[1], SCIConstants.SCI_DEFAULT_MACD_HISTOGRAM_COLOUR);
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] showRSIIndicatorSplit = split[i++].Split('=');
                if (showRSIIndicatorSplit.Length >= 2)
                {
                    this.showRSIIndicator = showRSIIndicatorSplit[1].ToUpper().Equals("TRUE") ? true : false;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] rsiPeriodSplit = split[i++].Split('=');
                if (rsiPeriodSplit.Length >= 2)
                {
                    uint rsiPeriodSetting = TradeStationTools.ConvertToUInt(rsiPeriodSplit[1]);
                    this.rsiPeriod = (rsiPeriodSetting == 0) ? SCIConstants.SCI_DEFAULT_RSI_PERIOD : rsiPeriodSetting;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] rsiOverBoughtLineSplit = split[i++].Split('=');
                if (rsiOverBoughtLineSplit.Length >= 2)
                {
                    uint rsiOverBoughtLineSetting = TradeStationTools.ConvertToUInt(rsiOverBoughtLineSplit[1]);
                    this.rsiOverBoughtLine = (rsiOverBoughtLineSetting > 100) ? SCIConstants.SCI_DEFAULT_RSI_OVERBOUGHT_LINE : rsiOverBoughtLineSetting;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] rsiOverSoldLineSplit = split[i++].Split('=');
                if (rsiOverSoldLineSplit.Length >= 2)
                {
                    uint rsiOverSoldLineSetting = TradeStationTools.ConvertToUInt(rsiOverSoldLineSplit[1]);
                    this.rsiOverSoldLine = (rsiOverSoldLineSetting > 100) ? SCIConstants.SCI_DEFAULT_RSI_OVERSOLD_LINE : rsiOverSoldLineSetting;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] rsiMidLineSplit = split[i++].Split('=');
                if (rsiMidLineSplit.Length >= 2)
                {
                    uint rsiMidLineSetting = TradeStationTools.ConvertToUInt(rsiMidLineSplit[1]);
                    this.rsiMidLine = (rsiMidLineSetting > 100) ? SCIConstants.SCI_DEFAULT_RSI_MID_LINE : rsiMidLineSetting;
                }
            }

            if (split.Length >= (i + 1)) 
            {
                String[] rsiColourSplit = split[i++].Split('=');
                if (rsiColourSplit.Length >= 2)
                {
                    this.rsiColour = SCICommon.ValidateColourValue(rsiColourSplit[1], SCIConstants.SCI_DEFAULT_RSI_COLOUR);
                }
            }

            if (split.Length >= (i + 1)) 
            {
                String[] rsiOverBoughtLineColourSplit = split[i++].Split('=');
                if (rsiOverBoughtLineColourSplit.Length >= 2)
                {
                    this.rsiOverBoughtLineColour = SCICommon.ValidateColourValue(rsiOverBoughtLineColourSplit[1], SCIConstants.SCI_DEFAULT_RSI_OVERBOUGHT_LINE_COLOUR);
                }
            }

            if (split.Length >= (i + 1)) 
            {
                String[] rsiOverSoldLineColourSplit = split[i++].Split('=');
                if (rsiOverSoldLineColourSplit.Length >= 2)
                {
                    this.rsiOverSoldLineColour = SCICommon.ValidateColourValue(rsiOverSoldLineColourSplit[1], SCIConstants.SCI_DEFAULT_RSI_OVERSOLD_LINE_COLOUR);
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] rsiMidLineColourSplit = split[i++].Split('=');
                if (rsiMidLineColourSplit.Length >= 2)
                {
                    this.rsiMidLineColour = SCICommon.ValidateColourValue(rsiMidLineColourSplit[1], SCIConstants.SCI_DEFAULT_RSI_MID_LINE_COLOUR);
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] showWilliamsRIndicatorSplit = split[i++].Split('=');
                if (showWilliamsRIndicatorSplit.Length >= 2)
                {
                    this.showWilliamsRIndicator = showWilliamsRIndicatorSplit[1].ToUpper().Equals("TRUE") ? true : false;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] showWilliamsRIndicatorSplit = split[i++].Split('=');
                if (showWilliamsRIndicatorSplit.Length >= 2)
                {
                    uint williamsRPeriodSetting = TradeStationTools.ConvertToUInt(showWilliamsRIndicatorSplit[1]);
                    this.williamsRPeriod = (williamsRPeriodSetting == 0) ? SCIConstants.SCI_DEFAULT_WILLIAMSR_PERIOD : williamsRPeriodSetting;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] williamsROverBoughtLineSplit = split[i++].Split('=');
                if (williamsROverBoughtLineSplit.Length >= 2)
                {
                    int williamsROverBoughtLineSetting = TradeStationTools.ConvertToInt(williamsROverBoughtLineSplit[1]);
                    this.williamsROverBoughtLine = ((williamsROverBoughtLineSetting > 0) || (williamsROverBoughtLineSetting < -100)) ? SCIConstants.SCI_DEFAULT_WILLIAMSR_OVERBOUGHT_LINE : williamsROverBoughtLineSetting;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] williamsROverSoldLineSplit = split[i++].Split('=');
                if (williamsROverSoldLineSplit.Length >= 2)
                {
                    int williamsROverSoldLineSetting = TradeStationTools.ConvertToInt(williamsROverSoldLineSplit[1]);
                    this.williamsROverSoldLine = ((williamsROverSoldLineSetting > 0) || (williamsROverSoldLineSetting < -100)) ? SCIConstants.SCI_DEFAULT_WILLIAMSR_OVERSOLD_LINE : williamsROverSoldLineSetting;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] williamsRMidLineSplit = split[i++].Split('=');
                if (williamsRMidLineSplit.Length >= 2)
                {
                    int williamsRMidLineSetting = TradeStationTools.ConvertToInt(williamsRMidLineSplit[1]);
                    this.williamsRMidLine = ((williamsRMidLineSetting > 0) || (williamsRMidLineSetting < -100)) ? SCIConstants.SCI_DEFAULT_WILLIAMSR_MID_LINE : williamsRMidLineSetting;
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] williamsRColourSplit = split[i++].Split('=');
                if (williamsRColourSplit.Length >= 2)
                {
                    this.williamsRColour = SCICommon.ValidateColourValue(williamsRColourSplit[1], SCIConstants.SCI_DEFAULT_WILLIAMSR_COLOUR);
                }
            
            }

            if (split.Length >= (i + 1))
            {
                String[] williamsROverBoughtLineColourSplit = split[i++].Split('=');
                if (williamsROverBoughtLineColourSplit.Length >= 2)
                {
                    this.williamsROverBoughtLineColour = SCICommon.ValidateColourValue(williamsROverBoughtLineColourSplit[1], SCIConstants.SCI_DEFAULT_WILLIAMSR_OVERBOUGHT_LINE_COLOUR);
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] williamsROverSoldLineColourSplit = split[i++].Split('=');
                if (williamsROverSoldLineColourSplit.Length >= 2)
                {
                    this.williamsROverSoldLineColour = SCICommon.ValidateColourValue(williamsROverSoldLineColourSplit[1], SCIConstants.SCI_DEFAULT_WILLIAMSR_OVERSOLD_LINE_COLOUR);
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] williamsRMidLineColourSplit = split[i++].Split('=');
                if (williamsRMidLineColourSplit.Length >= 2)
                {
                    this.williamsRMidLineColour = SCICommon.ValidateColourValue(williamsRMidLineColourSplit[1], SCIConstants.SCI_DEFAULT_WILLIAMSR_MID_LINE_COLOUR);
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] candlestickUpColourSplit = split[i++].Split('=');
                if (candlestickUpColourSplit.Length >= 2)
                {
                    this.candlestickUpColour = SCICommon.ValidateColourValue(candlestickUpColourSplit[1], SCIConstants.SCI_DEFAULT_CANDLESTICK_UP_COLOUR);
                }
            }

            if (split.Length >= (i + 1))
            {
                String[] candlestickDownColourSplit = split[i++].Split('=');
                if (candlestickDownColourSplit.Length >= 2)
                {
                    this.candlestickDownColour = SCICommon.ValidateColourValue(candlestickDownColourSplit[1], SCIConstants.SCI_DEFAULT_CANDLESTICK_DOWN_COLOUR);
                }
            }

            return this.getChartSettings();
        }

        void UpdateMarketPriceControlValues(MarketPriceItem mktItems)
        {
            if (mktItems == null)
            {
                this.marketPriceStateValue = "";
                this.marketPriceTimeValue = "";
                this.marketPriceLastValue = "";
                this.marketPriceChangeValue = "";
                this.marketPriceChangePercentValue = "";
                this.marketPriceLastQuantityValue = "";
                this.marketPriceBidQuantityValue = "";
                this.marketPriceBidValue = "";
                this.marketPriceAskValue = "";
                this.marketPriceAskQuantityValue = "";
                this.marketPriceVolumeValue = "";
                this.marketPriceBlockVolumeValue = "";
                this.marketPriceHighValue = "";
                this.marketPriceLowValue = "";
                this.marketPriceOpenValue = "";
                this.marketPricePreviousCloseValue = "";
                this.marketPriceCloseDateValue = "";
                this.marketPriceStrikeValue = "";
                return;
            }

            this.marketPriceStateValue = (mktItems.ProductStatus == null) ? "" : mktItems.ProductStatus;
            this.marketPriceTimeValue = (this.ValidateDateTime((DateTime)mktItems.Datetime) == true) ? ((DateTime)mktItems.Datetime).ToString("HH:mm:ss") : "";
            this.marketPriceLastValue = (mktItems.Last == null) ? "" : mktItems.Last;
            this.marketPriceChangeValue = (mktItems.Change == null) ? "" : AddSignToValue(mktItems.Change);
            this.marketPriceChangePercentValue = (mktItems.ChangePer == null) ? "" : (AddSignToValue(mktItems.ChangePer) + "%");
            this.marketPriceLastQuantityValue = (mktItems.LQty == null) ? "" : mktItems.LQty;
            this.marketPriceBidQuantityValue = (mktItems.BQty == null) ? "" : mktItems.BQty;
            this.marketPriceBidValue = (mktItems.Bid == null) ? "" : mktItems.Bid;
            this.marketPriceAskValue = (mktItems.Ask == null) ? "" : mktItems.Ask;
            this.marketPriceAskQuantityValue = (mktItems.AQty == null) ? "" : mktItems.AQty;
            this.marketPriceVolumeValue = (mktItems.Volume == null) ? "" : mktItems.Volume;
            this.marketPriceBlockVolumeValue = (mktItems.EP == null) ? "" : mktItems.EP;
            this.marketPriceHighValue = (mktItems.High == null) ? "" : mktItems.High;
            this.marketPriceLowValue = (mktItems.Low == null) ? "" : mktItems.Low;
            this.marketPriceOpenValue = (mktItems.Open == null) ? "" : mktItems.Open;
            this.marketPricePreviousCloseValue = (mktItems.PreClose == null) ? "" : mktItems.PreClose;
            this.marketPriceCloseDateValue = (this.ValidateDateTime((DateTime)mktItems.CloseDate) == true) ? ((DateTime)mktItems.CloseDate).ToString("dd/MM/yy") : "";
            this.marketPriceStrikeValue = (mktItems.Strike == null) ? "" : mktItems.Strike;
        }

        private String AddSignToValue(String value)
        {
            if (value == null)
            {
                return "";
            }

            if (value.Length == 0)
            {
                return value;
            }

            double valueAsDouble = TradeStationTools.ConvertToDouble(value);

            if (valueAsDouble > 0)
            {
                value = "+" + valueAsDouble.ToString();
            }

            return value;
        }

        private bool ValidateDateTime(DateTime dateTimeObj)
        {
            if (dateTimeObj == null)
            {
                return false;
            }
            else if (dateTimeObj.ToString() == "0" || dateTimeObj.ToString() == "")
            {
                return false;
            }
            else if ((dateTimeObj.GetType().ToString() == "System.DateTime") && (DateTime.Compare((DateTime)dateTimeObj, DateTime.MinValue) == 0))
            {
                return false;
            }

            return true;
        }

        private void drawVolumeChartView()
        {
            if ((this.sciChart == null) ||
                (this.thisLoaded == false) ||
                (this.bReceivedChartHistory == false) ||
                (this.chartVolumeSource == null))
            {
                return;
            }

            if (this.chartVolumeSource.Count > 0)
            {
                this.chartIndicatorList.Add(new Volume(SCIConstants.ChartIndicatorType.VOLUME, this, this.chartVolumeSource, this.sciChartSelectedTheme, this.chartPeriod));
            }
        }

        private void removeVolumeChartView()
        {
            this.chartIndicatorList.RemoveIndicator(this, SCIConstants.ChartIndicatorType.VOLUME);
        }

        private void drawSMA1ChartView()
        {
            if ((this.sciChart == null) ||
                (this.thisLoaded == false) ||
                (this.bReceivedChartHistory == false) ||
                (this.chartDataSource == null))
            {
                return;
            }

            if (this.chartDataSource.Count > 0)
            {
                this.chartIndicatorList.Add(new SMA(SCIConstants.ChartIndicatorType.SMA1, this, this.chartDataSource, this.sma1Period, this.sma1Colour));
            }
        }

        private void removeSMA1ChartView()
        {
            this.chartIndicatorList.RemoveIndicator(this, SCIConstants.ChartIndicatorType.SMA1);
        }

        private void drawSMA2ChartView()
        {
            if ((this.sciChart == null) ||
                (this.thisLoaded == false) ||
                (this.bReceivedChartHistory == false) ||
                (this.chartDataSource == null))
            {
                return;
            }

            if (this.chartDataSource.Count > 0)
            {
                this.chartIndicatorList.Add(new SMA(SCIConstants.ChartIndicatorType.SMA2, this, this.chartDataSource, this.sma2Period, this.sma2Colour));
            }
        }

        private void removeSMA2ChartView()
        {
            this.chartIndicatorList.RemoveIndicator(this, SCIConstants.ChartIndicatorType.SMA2);
        }

        private void drawSMA3ChartView()
        {
            if ((this.sciChart == null) ||
                (this.thisLoaded == false) ||
                (this.bReceivedChartHistory == false) ||
                (this.chartDataSource == null))
            {
                return;
            }
                
            if (this.chartDataSource.Count > 0)
            {
                this.chartIndicatorList.Add(new SMA(SCIConstants.ChartIndicatorType.SMA3, this, this.chartDataSource, this.sma3Period, this.sma3Colour));
            }
        }

        private void removeSMA3ChartView()
        {
            this.chartIndicatorList.RemoveIndicator(this, SCIConstants.ChartIndicatorType.SMA3);
        }

        private void drawBollingerBands()
        {
            if ((this.sciChart == null) ||
                (this.thisLoaded == false) ||
                (this.bReceivedChartHistory == false) ||
                (this.chartDataSource == null))
            {
                return;
            }

            if (this.chartDataSource.Count > 0)
            {
                this.chartIndicatorList.Add(new BollingerBands(SCIConstants.ChartIndicatorType.BOLLINGERBANDS, this, this.chartDataSource, this.bbPeriod, this.bbDevi, this.bbUpperLowerColour, this.bbMiddleColour));
            }
        }

        private void removeBollingerBands()
        {
            this.chartIndicatorList.RemoveIndicator(this, SCIConstants.ChartIndicatorType.BOLLINGERBANDS);
        }

        private void drawMACDChartView()
        {
            if ((this.sciChart == null) ||
                (this.thisLoaded == false) ||
                (this.bReceivedChartHistory == false) ||
                (this.chartDataSource == null))
            {
                return;
            }

            if (this.chartDataSource.Count > 0)
            {
                this.chartIndicatorList.Add(new MACD(SCIConstants.ChartIndicatorType.MACD, this, this.chartDataSource, this.chartPeriod, this.macdFastPeriod, this.macdSlowPeriod, this.macdSignalPeriod, this.macdMACDColour, this.macdSignalColour, this.macdHistogramColour, this.sciChartSelectedTheme));
            }
        }

        private void removeMACDChartView()
        {
            this.chartIndicatorList.RemoveIndicator(this, SCIConstants.ChartIndicatorType.MACD);
        }

        private void drawRSIChartView()
        {
            if ((this.sciChart == null) ||
                (this.thisLoaded == false) ||
                (this.bReceivedChartHistory == false) ||
                (this.chartDataSource == null))
            {
                return;
            }

            if (this.chartDataSource.Count > 0)
            {
                this.chartIndicatorList.Add(new RSI(SCIConstants.ChartIndicatorType.RSI, this, this.chartDataSource, this.chartPeriod, this.rsiPeriod, this.rsiOverBoughtLine, this.rsiOverSoldLine, this.rsiMidLine, this.rsiColour, this.rsiOverBoughtLineColour, this.rsiOverSoldLineColour, this.rsiMidLineColour, this.sciChartSelectedTheme));
            }
        }

        private void removeRSIChartView()
        {
            this.chartIndicatorList.RemoveIndicator(this, SCIConstants.ChartIndicatorType.RSI);
        }

        private void drawWilliamsRChartView()
        {
            if ((this.sciChart == null) ||
                (this.thisLoaded == false) ||
                (this.bReceivedChartHistory == false) ||
                (this.chartDataSource == null))
            {
                return;
            }

            if (this.chartDataSource.Count > 0)
            {
                this.chartIndicatorList.Add(new WilliamsR(SCIConstants.ChartIndicatorType.WILLIAMSR, this, this.chartDataSource, this.chartPeriod, this.williamsRPeriod, this.williamsROverBoughtLine, this.williamsROverSoldLine, this.williamsRMidLine, this.williamsRColour, this.williamsROverBoughtLineColour, this.williamsROverSoldLineColour, this.williamsRMidLineColour, this.sciChartSelectedTheme));
            }
        }

        private void removeWilliamsRChartView()
        {
            this.chartIndicatorList.RemoveIndicator(this, SCIConstants.ChartIndicatorType.WILLIAMSR);
        }

        private int getGridChartAnalysisRowByName(String rowName)
        {
            int rowIndex = -1;

            for (int i = 0; i < this.gridChartAnalysis.RowDefinitions.Count; i++)
            {
                if (this.gridChartAnalysis.RowDefinitions[i].Name.Equals(rowName) == true)
                {
                    return i;
                }
            }

            return rowIndex;
        }

        private void disableAllChartViews()
        {
            foreach (ChartIndicatorBase indicator in this.chartIndicatorList)
            {
                indicator.ChartDisable();
            }
        }

        private void enableAllChartViews()
        {
            foreach (ChartIndicatorBase indicator in this.chartIndicatorList)
            {
                indicator.ChartEnable();
            }
        }
    }
}
