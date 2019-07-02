using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Abt.Controls.SciChart.Visuals.Axes;
using System.Data;
using GOSTS.Common;
using System.Windows.Markup;
using System.Windows.Controls;

namespace GOSTS.WPFControls.Chart.SCI
{
    public static class SCIConstants
    {
        public const int SCI_DEFAULT_VIEW_POINTS = 60;

        public const bool SCI_DEFAULT_ISPAN = true;
        public const bool SCI_DEFAULT_SHOW_TICKER_LINE = true;
        public const bool SCI_DEFAULT_SHOW_VOLUME = false;
        public const bool SCI_DEFAULT_SHOW_OVERVIEW = false;
        public const bool SCI_DEFAULT_SHOW_BOLLINGER_BANDS = false;
        public const uint SCI_DEFAULT_BOLLINGER_BANDS_PERIOD = 20;
        public const double SCI_DEFAULT_BOLLINGER_BANDS_DEVIATION = 2;
        public const String SCI_DEFAULT_BOLLINGER_BANDS_OUTER_COLOUR = "#FFE63399";
        public const String SCI_DEFAULT_BOLLINGER_BANDS_SMA_COLOUR = "#FF6699FF";
        public const bool SCI_DEFAULT_SHOW_SMA1 = false;
        public const uint SCI_DEFAULT_SMA1_PERIOD = 5;
        public const String SCI_DEFAULT_SMA1_COLOUR = "#FF00CCFF";
        public const bool SCI_DEFAULT_SHOW_SMA2 = false;
        public const uint SCI_DEFAULT_SMA2_PERIOD = 10;
        public const String SCI_DEFAULT_SMA2_COLOUR = "#FFE6B333";
        public const bool SCI_DEFAULT_SHOW_SMA3 = false;
        public const uint SCI_DEFAULT_SMA3_PERIOD = 20;
        public const String SCI_DEFAULT_SMA3_COLOUR = "#FF686868";
        public const String SCI_DEFAULT_THEME = SCI_CHART_THEME_LIGHT;
        public const uint SCI_DEFAULT_CHART_PERIOD = 60;
        public const uint SCI_DEFAULT_MACD_MINIMUM_BUFFER_SIZE = 33;
        public const bool SCI_DEFAULT_MACD_SHOW_MACD = false;
        public const uint SCI_DEFAULT_MACD_FAST_PERIOD = 12;
        public const uint SCI_DEFAULT_MACD_SLOW_PERIOD = 26;
        public const uint SCI_DEFAULT_MACD_SIGNAL_PERIOD = 9;
        public const String SCI_DEFAULT_MACD_MACD_COLOUR = "#FF0000FF";
        public const String SCI_DEFAULT_MACD_SIGNAL_COLOUR = "#FFFF0000";
        public const String SCI_DEFAULT_MACD_HISTOGRAM_COLOUR = "#FFF0F0F0";
        public const bool SCI_DEFAULT_RSI_SHOW_RSI = false;
        public const uint SCI_DEFAULT_RSI_PERIOD = 14;
        public const uint SCI_DEFAULT_RSI_OVERBOUGHT_LINE = 70;
        public const uint SCI_DEFAULT_RSI_OVERSOLD_LINE = 30;
        public const uint SCI_DEFAULT_RSI_MID_LINE = 50;
        public const String SCI_DEFAULT_RSI_COLOUR = "#FF00D010";
        public const String SCI_DEFAULT_RSI_OVERBOUGHT_LINE_COLOUR = "#FF1E90FF";
        public const String SCI_DEFAULT_RSI_OVERSOLD_LINE_COLOUR = "#FF1E90FF";
        public const String SCI_DEFAULT_RSI_MID_LINE_COLOUR = "#FFC0C0C0";
        public const bool SCI_DEFAULT_WILLIAMSR_SHOW_WILLIAMSR = false;
        public const uint SCI_DEFAULT_WILLIAMSR_PERIOD = 14;
        public const int SCI_DEFAULT_WILLIAMSR_OVERBOUGHT_LINE = -20;
        public const int SCI_DEFAULT_WILLIAMSR_OVERSOLD_LINE = -80;
        public const int SCI_DEFAULT_WILLIAMSR_MID_LINE = -50;
        public const String SCI_DEFAULT_WILLIAMSR_COLOUR = "#FF66A3A3";
        public const String SCI_DEFAULT_WILLIAMSR_OVERBOUGHT_LINE_COLOUR = "#FF66A3A3";
        public const String SCI_DEFAULT_WILLIAMSR_OVERSOLD_LINE_COLOUR = "#FF66A3A3";
        public const String SCI_DEFAULT_WILLIAMSR_MID_LINE_COLOUR = "#FFC2C2D6";
//        public const String SCI_DEFAULT_CANDLESTICK_UP_COLOUR = "#FF00CC00";
//        public const String SCI_DEFAULT_CANDLESTICK_DOWN_COLOUR = "#FFFF0000";
        public const String SCI_DEFAULT_CANDLESTICK_UP_COLOUR = "#FFFF0000";
        public const String SCI_DEFAULT_CANDLESTICK_DOWN_COLOUR = "#FF00CC00";

        public const String SCI_CHART_THEME_DARK = "DARK";
        public const String SCI_CHART_THEME_LIGHT = "LIGHT";

        public const uint SCI_MINIMUM_BOLLINGER_BANDS_PERIOD = 2;
        public const double SCI_MINIMUM_BOLLINGER_BANDS_DEVIATION = 1;
        public const double SCI_MAXIMUM_BOLLINGER_BANDS_DEVIATION = 4;

        public const String SCI_TOOLBAR_ROW_DEFINITION = "ToolbarRowDefinition";
        public const String SCI_MARKETPRICE_ROW_DEFINITION = "MarketPriceRowDefinition";
        public const String SCI_CANDLESTICK_ROW_DEFINITION = "CandlestickRowDefinition";
        public const String SCI_VOLUME_ROW_DEFINITION = "VolumeRowDefinition";
        public const String SCI_CHARTOVERVIEW_ROW_DEFINITION = "ChartOverviewRowDefinition";
        public const String SCI_MACD_ROW_DEFINITION = "MACDRowDefinition";
        public const String SCI_RSI_ROW_DEFINITION = "RSIRowDefinition";
        public const String SCI_WILLIAMSR_ROW_DEFINITION = "WilliamsRRowDefinition";

        public enum ChartIndicatorType
        {
            CANDLESTICK,
            VOLUME,
            BOLLINGERBANDS,
            SMA1,
            SMA2,
            SMA3,
            MACD,
            RSI,
            WILLIAMSR,
            UNDEFINED
        }
    }

    public static class SCICommon
    {
        public static DateTime FilterMarketPriceTime(DateTime marketPriceTime, uint chartPeriod)
        {
            DateTime filteredTime = marketPriceTime;
            int remainder = 0;

            switch (chartPeriod)
            {
                case 1: break;

                case 5:
                    remainder = filteredTime.Second % 5;
                    filteredTime = filteredTime.AddSeconds(-remainder);
                    break;

                case 10:
                    remainder = filteredTime.Second % 10;
                    filteredTime = filteredTime.AddSeconds(-remainder);
                    break;

                case 15:
                    remainder = filteredTime.Second % 15;
                    filteredTime = filteredTime.AddSeconds(-remainder);
                    break;

                case 30:
                    remainder = filteredTime.Second % 30;
                    filteredTime = filteredTime.AddSeconds(-remainder);
                    break;

                case 60:
                    filteredTime = filteredTime.AddSeconds(-filteredTime.Second);
                    break;

                case 300:
                    filteredTime = filteredTime.AddSeconds(-filteredTime.Second);
                    remainder = filteredTime.Minute % 5;
                    filteredTime = filteredTime.AddMinutes(-remainder);
                    break;

                case 600:
                    filteredTime = filteredTime.AddSeconds(-filteredTime.Second);
                    remainder = filteredTime.Minute % 10;
                    filteredTime = filteredTime.AddMinutes(-remainder);
                    break;

                case 900:
                    filteredTime = filteredTime.AddSeconds(-filteredTime.Second);
                    remainder = filteredTime.Minute % 15;
                    filteredTime = filteredTime.AddMinutes(-remainder);
                    break;

                case 1800:
                    filteredTime = filteredTime.AddSeconds(-filteredTime.Second);
                    remainder = filteredTime.Minute % 30;
                    filteredTime = filteredTime.AddMinutes(-remainder);
                    break;

                case 3600:
                    filteredTime = filteredTime.AddSeconds(-filteredTime.Second);
                    filteredTime = filteredTime.AddMinutes(-filteredTime.Minute);
                    break;

                default: break;
            }

            return filteredTime;
        }

        public static String ValidateColourValue(String value, String defaultString)
        {
            String regexString = "^#[0-9a-fA-F]{8}\b";

            if (System.Text.RegularExpressions.Regex.IsMatch(value, regexString) == false)
            {
                value = defaultString;
            }

            return value;
        }

        public static String ConvertChartThemeToBasicName(String theme)
        {
            String basicName = SCIConstants.SCI_DEFAULT_THEME;

            switch (theme)
            {
                case "Chrome":
                    basicName = SCIConstants.SCI_CHART_THEME_LIGHT;
                    break;

                case "BlackSteel":
                    basicName = SCIConstants.SCI_CHART_THEME_DARK;
                    break;

                default:
                    break;
            }

            return basicName;
        }

        public static String ConvertBasicNameToChartTheme(String basicName)
        {
            String theme = "Chrome";

            switch (basicName)
            {
                case SCIConstants.SCI_CHART_THEME_LIGHT:
                    theme = "Chrome";
                    break;

                case SCIConstants.SCI_CHART_THEME_DARK:
                    theme = "BlackSteel";
                    break;

                default:
                    break;
            }

            return theme;
        }

        public static String ValidateChartTheme(String theme)
        {
            theme = theme.ToUpper();

            String validTheme = SCIConstants.SCI_DEFAULT_THEME;

            if (theme.Equals(SCIConstants.SCI_CHART_THEME_LIGHT) ||
               theme.Equals(SCIConstants.SCI_CHART_THEME_DARK))
            {
                validTheme = theme;
            }

            return validTheme;
        }

        public static SCIChartTimeFormatter CreateTimeAxisFormatter(uint chartPeriod)
        {
            SCIChartTimeFormatter format;

            if (chartPeriod < 60)
            {
                format = new SCIChartTimeFormatter("{0:HH:mm:ss}");
            }
            else
            {
                format = new SCIChartTimeFormatter("{0:HH:mm}");
            }

            return format;
        }
    }

    public class DoubleToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new GridLength((double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InvertedBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    public class ChartPeriodToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            uint period = (uint)value;
            uint param = uint.Parse((string)parameter);

            return (period == param);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SCIChartTimeFormatter : ILabelFormatter
    {
        private readonly String labelFormat;

        public SCIChartTimeFormatter(String format)
        {
            labelFormat = format;
        }

        public string FormatCursorLabel(IComparable dataValue)
        {
            return String.Format(labelFormat, dataValue);
        }

        public string FormatLabel(IComparable dataValue)
        {
            return String.Format(labelFormat, dataValue);
        }

        public void Init(IAxis parentAxis)
        {
        }

        public void OnBeginAxisDraw()
        {
        }
    }

    public class ChartThemeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String theme = (String)value;
            String param = (String)parameter;

            return (theme.Equals(param));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MarketPriceStateToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String state = (String)value;
            state = state.ToUpper();
            String colour = "White";

            switch (state)
            {
                case "OPEN":
                    colour = "LightGreen";
                    break;

                default:
                    break;
            }

            return colour;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MarketPriceChangeToBackgroundColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String change = (String)value;
            double changePrice = TradeStationTools.ConvertToDouble(change);
            String colour = "LightGreen";

            if (change.Length == 0)
            {
                colour = "White";
            }
            else
            {
                if (changePrice < 0)
                {
                    colour = "Red";
                }
                else if (changePrice == 0)
                {
                    colour = "White";
                }
                else
                {
                    colour = "LightGreen";
                }
            }

            return colour;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MarketPriceChangeToForegroundColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String change = (String)value;
            double changePrice = TradeStationTools.ConvertToDouble(change);
            String colour = "Black";

            if (changePrice < 0)
            {
                colour = "White";
            }
            else if (changePrice == 0)
            {
                colour = "Black";
            }
            else
            {
                colour = "Black";
            }

            return colour;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GridDefinitionExtension : MarkupExtension
    {
        public string Name { get; set; }

        public GridDefinitionExtension(string name)
        {
            Name = name;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var refExt = new Reference(Name);
            var definition = refExt.ProvideValue(serviceProvider);
            if (definition is DefinitionBase)
            {
                var grid = (definition as FrameworkContentElement).Parent as Grid;
                if (definition is RowDefinition)
                {
                    return grid.RowDefinitions.IndexOf(definition as RowDefinition);
                }
                else
                {
                    return grid.ColumnDefinitions.IndexOf(definition as ColumnDefinition);
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
