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

namespace GOSTS.WPFControls
{
    /// <summary>
    /// HandleMarketPrice.xaml 的交互逻辑
    /// </summary>
    public partial class HandleMarketPrice : UserControl
    {
        public delegate void OnHandleProduct(object sender, string handleMode, List<string> lsGridProd, List<string> lsInsertProd, string strName);
        public event OnHandleProduct HandleProduct;
        
        public event EventHandler Close;
        public List<string> ProdCode { get; set; }
        public List<string> RemindCode { get; set; }
        public string HandleType { get; set; }

        public HandleMarketPrice()
        {
            InitializeComponent(); 
        } 

        string InsertProdMsg
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("MkInsertProdMsg");
            }
        }

        string RemoveProdAlertMsg
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("MkRemoveProdAlertMsg");
            }
        }

        string ReNameProdListAlertMsg
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("MkReNameProdListAlertMsg");
            }
        }

        string ClearProdAlertMsg
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("MkClearProdAlertMsg");
            }
        }

        string ProdCodeError
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("MkProdCodeError");
            }
        }

        string ProdCodeExisted
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("MkProdCodeExisted");
            }
        }

        string ProdListError
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("ProdListError");
            }
        }

        private void submit_Click(object sender, RoutedEventArgs e)
        {
            //all product in datagrid
            List<string> lsGridProd = new List<string>();
            lsGridProd = (ProdCode == null) ? lsGridProd : ProdCode;

            if (HandleType == "Insert")
            {
                if (txtProdCode.Text.Trim() != "")
                {
                    //insert datagrid
                    List<string> lsInsertProd = new List<string>();
                    lsInsertProd.Add(txtProdCode.Text.Trim());
                    if (HandleProduct != null && lsInsertProd.Except(lsGridProd).Count ()>0)
                    {
                        //lsGridProd no needed
                        HandleProduct(this,"Insert", lsGridProd, lsInsertProd,null);
                        this.Close(sender, e);
                    }
                    else
                    {
                        lbProdCodeError.Content = ProdCodeExisted;
                    } 
                }
                else
                {
                    lbProdCodeError.Content = ProdCodeError;
                }
            }
            else if (HandleType == "Remove")
            {
                if (HandleProduct != null)
                {
                    HandleProduct(this,"Remove", ProdCode, null,null);
                }
                HandleType = "";
                this.Close(sender, e);
            }
            else if (HandleType == "Clear")
            {
                if (HandleProduct != null)
                {
                    //remove all product in current tab
                    HandleProduct(this,"Clear", ProdCode, null,null);
                }
                this.Close(sender, e);
            }
            else if (HandleType == "Rename")
            {
                if (this.txtProdCode.Text.Trim() == "")
                {
                    lbProdCodeError.Content = ProdListError ;
                }
                else
                {
                    if (HandleProduct != null)
                    {
                        //rename
                        HandleProduct(this,"Rename", null, null, this.txtProdCode.Text.Trim());
                    }
                    this.Close(sender, e);
                }
            }

            if (ProdCode == null) return;
            ProdCode.Clear();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close(sender, e); 
            HandleProduct(this, "Cancel", null, null, null);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtProdCode.Focus();
            string strProdCode = (ProdCode==null) ? "" : String.Join(",", ProdCode.ToArray());
            switch (HandleType)
            {
                case "Insert":
                    lbHandleType.Content = InsertProdMsg;
                    txtProdCode.Text = "";
                    	txtProdCode.Visibility = Visibility.Visible;
                    lbHandleType.VerticalAlignment = VerticalAlignment.Top;
                    lbProdCodeError.Content = "";
                    break;
                case "Remove":
                    lbHandleType.Content = String.Format(RemoveProdAlertMsg, strProdCode);
                    txtProdCode.Visibility = Visibility.Hidden;
                    lbProdCodeError.Content = "";
                    lbHandleType.VerticalAlignment = VerticalAlignment.Center;
                    break;
                case "Rename":
                    lbHandleType.Content = ReNameProdListAlertMsg; 
                    txtProdCode.Visibility = Visibility.Visible; 
                    lbHandleType.VerticalAlignment = VerticalAlignment.Top;
                    lbProdCodeError.Content = "";
                    txtProdCode.Text = "";
                    break;
                case "Clear":
                    lbHandleType.Content =ClearProdAlertMsg;
                    txtProdCode.Visibility = Visibility.Hidden;
                    lbProdCodeError.Content = ""; 
                    lbHandleType.VerticalAlignment = VerticalAlignment.Top;
                    break;
            }
           
        }

        private void txtProdCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                submit_Click(sender, e);
            }
        }
    }
}
