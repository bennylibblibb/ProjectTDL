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
using System.Data;
using System.Linq; 
using System.ComponentModel; 
using GOSTS.ViewModel;
using GOSTS;

namespace GOSTS.ViewModel
{
    public class TabPageViewModel: INotifyPropertyChanged
    {
        //public string Header { get; set; }
        public DataTable GridTable { get; set; }
        //public ObservableCollection<MarketPriceItem> MarketPriceItems  { get; set; }

        private string header;
        public string Header
        {
            get { return header; }
            set
            {
                header = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Header"));
            }
        }

        private ObservableCollection<MarketPriceItem> marketPriceItems;
        public ObservableCollection<MarketPriceItem> MarketPriceItems 
        {
            get { return marketPriceItems; }
            set
            {
                marketPriceItems = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("MarketPriceItems"));
            }
        } 

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class MarketCodeViewModel : TabPageViewModel
    {
        public ReadOnlyCollection<InstmntViewModel> Instmnts { get; set; }

        public ReadOnlyCollection<InstmntViewModel> MarketCodeViewModels(Instmnt[] instmnts)
        {
            if (instmnts == null) return null;

            return new ReadOnlyCollection<InstmntViewModel>(
                (from region in instmnts
                 select new InstmntViewModel(region))
                .ToList());
        }
    }

    //public class MarketCodeViewModel : TabPageViewModel
    //{
    //    readonly ReadOnlyCollection<InstmntViewModel> _instmnts;
    //    public ReadOnlyCollection<InstmntViewModel> Instmnts
    //    {
    //        get { return _instmnts; }
    //    }

    //    public MarketCodeViewModel(string str, Instmnt[] instmnts)
    //    {
    //        base.Header = str;
    //        _instmnts = new ReadOnlyCollection<InstmntViewModel>(
    //            (from region in instmnts
    //             select new InstmntViewModel(region))
    //            .ToList());
    //    }
    //}  
}
