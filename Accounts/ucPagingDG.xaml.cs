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
using System.Data;
using System.Data.SqlClient;
using System.Windows.Threading;
using System.ComponentModel;


namespace GOSTS
{
    /// <summary>
    /// ucPagingDG.xaml 的交互逻辑
    /// </summary>
    public partial class ucPagingDG : UserControl
    {
        public ucPagingDG()
        {
            InitializeComponent();
            BindPageEvent();
            OnBindDg = new pushDataSource(BindDg);
        }
        ICollectionView cv;
        delegate void pushDataSource(DataTable dt);
        pushDataSource OnBindDg;
        void BindDg(DataTable dt)
        {
          
            if (this.dg == null) return;
            if (dt == null)
            {
                dg.ItemsSource = null;
                return;
            }
            else
            {
                dg.ItemsSource = dt.DefaultView;
            }           
            ShowPageinfo();
        }

        public static readonly DependencyProperty DgDataSourceProperty = DependencyProperty.Register("TNumBox", typeof(DataTable), typeof(ucPagingDG), null, null);// new ValidateValueCallback(new MyClass().MyValidateMethod));
        public DataTable DgDataSource
        {
            get { return (DataTable)GetValue(DgDataSourceProperty); }
            set { SetValue(DgDataSourceProperty, value); }
        }

        public DataTable dtTotal;
        public DataTable dtCurPage;
        public int PageCurrent = 1;
        public int PageTotal = 1;
        public string SortField = "";
        public string SortDirection = "";
        public int PageSize = AppFlag.intDgPageSize;
        CollectionViewSource viewSource = new CollectionViewSource();

        private DataGrid _dg;
        public DataGrid dg
        {
            get{return _dg;}
            set{_dg=value; BindDgSort();}
        }


        public void GetCurPageData(DataTable _dtTotal, DataGrid _dg, int? _CurPage = null)
        {
            if (_dg == null)
            {
                //Dispatcher.BeginInvoke((Action)(()=>{
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Collapsed;
                }
                // }), DispatcherPriority.Send, null);
                return;
            }
            dtTotal = _dtTotal;
            this.dg = _dg;
            if (dtTotal == null)
            {
                this.dg.ItemsSource = null;
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Collapsed;
                }
                return;
            }
            int rsCount = dtTotal.Rows.Count;
            if (rsCount < PageSize)
            {
                //Dispatcher.BeginInvoke((Action)(() =>
                //{
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Collapsed;
                }
                //}), DispatcherPriority.Send, null);
                this.dg.ItemsSource = dtTotal.DefaultView;
                return;
            }
            // Dispatcher.BeginInvoke((Action)(() =>
            // {
            if (this.Visibility != Visibility.Visible)
            {
                this.Visibility = Visibility.Visible;
            }
            // }
            //), DispatcherPriority.Send, null);

            if (_CurPage.HasValue)
            {
                PageCurrent = _CurPage.Value;
            }
            else
            {
                this.SortField = "";//clear sortFiled
            }
            if (PageCurrent < 1)
            {
                PageCurrent = 1;
            }
            if (rsCount % PageSize == 0)
            {
                PageTotal = rsCount / PageSize;
            }
            else
            {
                PageTotal = rsCount / PageSize + 1;
            }
            if (PageCurrent > PageTotal)
            {
                PageCurrent = PageTotal;
            }
            int intBegin = PageSize * (PageCurrent - 1) + 1;
            DataTable dtCur = CopyTable(intBegin, PageSize, _dtTotal);
            if (dtCur != null)
            {
                dg.ItemsSource = dtCur.DefaultView;
            }
            else
            {
                dg.ItemsSource = null;
            }
            ShowPageinfo();
            PageBtnState();

        }


        /// <summary>
        /// 设置行头排序箭头图标
        /// </summary>
        void SetSortColumn()
        {
            if (this.SortField == "")
            {
                return;
            }
            if (SortDirection == "")
            {
                return;
            }
            if (this.dg == null)
            {
                return;
            }
            foreach (DataGridColumn col in dg.Columns)
            {
                if (col.SortMemberPath == this.SortField)
                {
                    switch (SortDirection)
                    {
                        case " asc":
                            col.SortDirection = ListSortDirection.Ascending;
                            break;
                        case " desc":
                            col.SortDirection = ListSortDirection.Descending;
                            break;
                        default :
                            col.SortDirection = ListSortDirection.Ascending;
                            break;
                    }
                    break;
                }
            }
        }

        public void ClearDgAndPageUI(DataGrid _dg)
        {
            if (_dg != null)
            {
                _dg.ItemsSource = null;
            }
            this.PageCurrent = 1;
            this.PageTotal = 1;
            if (this.Visibility == Visibility.Visible)
            {
                this.Visibility = Visibility.Collapsed;
            }
        }

        public void ClearPageUI()
        {           
            this.PageCurrent = 1;
            this.PageTotal = 1;
            if (this.Visibility == Visibility.Visible)
            {
                this.Visibility = Visibility.Collapsed;
            }
        }

        public void ChagePageData(int PageTo)
        {
            if (dg == null) return;
            if (dtTotal == null)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Collapsed;
                }
                return;
            }

            int rsCount = dtTotal.Rows.Count;
            if (rsCount < PageSize)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Collapsed;
                }
                return;
            }
            if (this.Visibility != Visibility.Visible)
            {
                this.Visibility = Visibility.Visible;
            }
            PageCurrent = PageTo;

            if (PageCurrent < 1)
            {
                PageCurrent = 1;
            }
            if (rsCount % PageSize == 0)
            {
                PageTotal = rsCount / PageSize;
            }
            else
            {
                PageTotal = rsCount / PageSize + 1;
            }
            if (PageCurrent > PageTotal)
            {
                PageCurrent = PageTotal;
            }
            PageBtnState();
            int intBegin = PageSize * (PageCurrent - 1) + 1;
            DataTable dtCur = CopyTable(intBegin, PageSize, dtTotal);//,SortField,SortDirection);
            //  viewSource=CollectionViewSource.GetDefaultView(
            
            Dispatcher.Invoke(OnBindDg, new object[] { dtCur });

        }

        void PageBtnState()
        {
            if (PageCurrent == PageTotal)
            {
                Dispatcher.Invoke((Action)delegate()
                {
                    this.btnPageNext.IsEnabled = false;
                }, null);
            }
            else
            {
                Dispatcher.Invoke((Action)delegate()
                {
                    this.btnPageNext.IsEnabled = true;
                }, null);
            }
            if (PageCurrent == 1)
            {
                Dispatcher.Invoke((Action)delegate()
                {
                    this.btnPagePrev.IsEnabled = false;
                }, null);
            }
            else
            {
                Dispatcher.Invoke((Action)delegate()
                {
                    this.btnPagePrev.IsEnabled = true;
                }, null);
            }
        }

        void BindPageEvent()
        {
            this.btnPageNext.Click += btnPageNext_Click;
            this.btnPagePrev.Click += btnPagePrev_Click;
            this.btnGoto.Click += btnGoto_Click;
            if (dg != null)
            {
                BindDgSort();
            }
        }

        bool bBindSort = false;
        void BindDgSort()
        {
            if (!bBindSort)
            {
                bBindSort = true;
                dg.Sorting += dg_Sorting;                
            }
        }

        void dg_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (this.dg.ItemsSource == null) return;
            cv = CollectionViewSource.GetDefaultView(this.dg.ItemsSource);
            if (cv == null) return;
            if (cv.SortDescriptions.Count < 0) return;
            SortField = cv.SortDescriptions[0].PropertyName;
            ListSortDirection sortD = cv.SortDescriptions[0].Direction;
            switch (sortD)
            {
                case ListSortDirection.Ascending:
                    SortDirection = " desc";
                    break;
                case ListSortDirection.Descending:
                    SortDirection = " asc";
                    break;
                default:
                    SortDirection = "";
                    break;
            }
            SortDataSource();
            DataTable dtCur = CopyTable(PageCurrent, PageSize, dtTotal);
            BindDg(dtCur);
            SetSortColumn();


            //string str = e.Column.SortMemberPath;
            //if (str.Trim() != "")
            //{
            //    this.SortField = str.Trim() ;
            //    ListSortDirection? SD = e.Column.SortDirection;
            //    if (SD.HasValue)
            //    {
            //        switch (SD)
            //        {
            //            case ListSortDirection.Ascending:
            //                SortDirection = " desc";
            //                break;
            //            case ListSortDirection.Descending:
            //                SortDirection = " asc";
            //                break;
            //            default:
            //                SortDirection = "";
            //                break;
            //        }
            //    }
            //    else
            //    {
            //        SortDirection = " asc";
            //    }
            //    SortDataSource();
            //    DataTable dtCur = CopyTable(PageCurrent, PageSize, dtTotal);
            //    BindDg(dtCur);
            //    SetSortColumn();
            //   // Dispatcher.Invoke(OnBindDg, new object[] { dtCur });
            //}
        }

        void btnGoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string str = this.txtGoToPage.Text.Trim();
                int i = Convert.ToInt32(str);
                if (i < 1) return;
                PageCurrent = i;
                ChagePageData(PageCurrent);
            }
            catch (Exception ex)
            {

            }
        }

        void btnPagePrev_Click(object sender, RoutedEventArgs e)
        {
            PageCurrent -= 1;
            ChagePageData(PageCurrent);
        }

        void btnPageNext_Click(object sender, RoutedEventArgs e)
        {
            PageCurrent += 1;
            ChagePageData(PageCurrent);
        }

        void ShowPageinfo()
        {
            //  Dispatcher.Invoke((Action)
            //  (()=>{
            string str = string.Format("Page:{0}/{1}", PageCurrent, PageTotal);
            this.tbPagesInfo.Text = str;
            //}),DispatcherPriority.Send, null);
        }

        void SortDataSource()
        {
            if (this.dtTotal == null)
            {
                return;
            }
            if (this.dtTotal.Rows.Count < 2) return;
            if (this.SortField.Trim() != "")
            {
                string SDirect=this.SortDirection;
                DataView dv = dtTotal.DefaultView;
                dv.Sort = SortField + " " + SDirect;
                dtTotal = dv.ToTable();//.Table;
            }
        }

        public static DataTable CopyTable(int intBeg, int len, DataTable dt)//, string _SortFiled = "", string SDirect = "")
        {

            if (dt == null) return null;
            if (intBeg < 1) intBeg = 1; ;
            if (len < 1) return null;
            if (dt.Rows.Count < intBeg)
            {
                return null;
            }
            int intTo = intBeg + len;
            if (dt.Rows.Count < intTo)
            {
                intTo = dt.Rows.Count;
            }
            //if (dt != null)
            //{
            //    if (_SortFiled.Trim() != "")
            //    {
            //        DataView dv = dt.DefaultView;
            //        dv.Sort = _SortFiled + " " + SDirect;
            //        dt = dv.Table;
            //    }
            //}
            DataTable dtNew = dt.Clone();            
            for (int i = intBeg; i <= intTo; i++)
            {
                if (dt.Rows.Count < i)
                {
                    break;
                }
                DataRow drNew = dtNew.NewRow();
                drNew.ItemArray = dt.Rows[i - 1].ItemArray;
                dtNew.Rows.Add(drNew);
            }
            dtNew.AcceptChanges();
           
            return dtNew;
        }

    }
}
