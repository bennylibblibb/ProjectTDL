using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using GOSTS.Common;

namespace GOSTS.ViewModel
{
    public class MainWindowViewModel
    {
        public static MainWindowViewModel Create(MarketPriceData  data)
        {
            return new MainWindowViewModel
            {
                GridTab = new TabControlViewModel("Grid",data),
                TreeTab = new TabControlViewModel("Tree",data)
            };
        }

        public static MainWindowViewModel CreateMarketPrice(MarketPriceData data)
        {
            return new MainWindowViewModel
            {
                GridTab = new TabControlViewModel("Grid", data)
            };
        }

        public TabControlViewModel GridTab { get; set; }
        public TabControlViewModel TreeTab { get; set; }
    }
}
