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

namespace GOSTS.Accounts
{
    /// <summary>
    /// ucBtnNumAdd.xaml 的交互逻辑
    /// </summary>
    public partial class ucBtnNumAdd : UserControl
    {
        public ucBtnNumAdd()
        {
            InitializeComponent();
         //   IDelearUser.DeleNoticeUserChange(this, "1001");
        }
        public static readonly DependencyProperty TNumBoxProperty = DependencyProperty.Register("TNumBox", typeof(ucTBNumber), typeof(ucBtnNumAdd), null, null);// new ValidateValueCallback(new MyClass().MyValidateMethod));
        public ucTBNumber TNumBox
        {
            get { return (ucTBNumber)GetValue(TNumBoxProperty); }
            set { SetValue(TNumBoxProperty, value); }
        }   

        private void btnUp_Click_1(object sender, RoutedEventArgs e)
        {
            TNumBox.Up();
        }

        //void doCal()
        //{
        //    if (TBoxNum == null) return;
        //    string strText = this.TBoxNum.Text.Trim();
        //    decimal I = 0;
        //    try
        //    {
        //        I = Convert.ToDecimal(strText);
        //    }
        //    catch { }
        //    if (I >= UpBound)
        //    {
        //        return;
        //    }
        //    I += Step;
        //    this.TBoxNum.Text = I.ToString();
        //}


        public static readonly DependencyProperty BTWidthProperty = DependencyProperty.Register("BTWidth", typeof(double), typeof(ucBtnNumAdd), new PropertyMetadata(25.0), null);// new ValidateValueCallback(new MyClass().MyValidateMethod));
        public double BTWidth
        {
            get { return (double)GetValue(BTWidthProperty); }
            set { SetValue(BTWidthProperty, value); }
        }

        public static readonly DependencyProperty BTHeightProperty = DependencyProperty.Register("BTHeight", typeof(double), typeof(ucBtnNumAdd), new PropertyMetadata(20.0), null);// new ValidateValueCallback(new MyClass().MyValidateMethod));
        public double BTHeight
        {
            get { return (double)GetValue(BTHeightProperty); }
            set { SetValue(BTHeightProperty, value); }
        }

    }
}
