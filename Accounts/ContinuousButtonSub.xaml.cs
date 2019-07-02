/*
 * Create By Harry
 * 连减按钮
 *  
*/
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
using System.Windows.Media.Animation;

namespace GOSTS.Accounts
{
    /// <summary>
    /// ContinuousButtonSub.xaml 的交互逻辑
    /// </summary>
    public partial class ContinuousButtonSub : UserControl
    {
        public ContinuousButtonSub()
        {
            InitializeComponent();
            this.DataContext = this;           
        }

       

        public bool MyValidateMethod(object value)
        {
            return true;//这里实现验证
        }
        private void btnDown_Click_1(object sender, RoutedEventArgs e)
        {
            doCal();
        }
       
        void doCal()
        {
            if (TBoxNum == null) return;
            string strText = this.TBoxNum.Text.Trim();
            decimal I = 0;
            try
            {
                I = Convert.ToDecimal(strText);
            }
            catch { }
            if (I <=DownBound)
            {
                return;
            }
            I -= Step;
            this.TBoxNum.Text = I.ToString();
        }


       


        public static readonly DependencyProperty TBoxNumProperty = DependencyProperty.Register("TBoxNum", typeof(TextBox), typeof(ContinuousButtonSub), null, null);// new ValidateValueCallback(new MyClass().MyValidateMethod));
        public TextBox TBoxNum
        {
            get { return (TextBox)GetValue(TBoxNumProperty); }
            set { SetValue(TBoxNumProperty, value); }
        }        

        public static readonly DependencyProperty BTWidthProperty = DependencyProperty.Register("BTWidth", typeof(double), typeof(ContinuousButtonSub), new PropertyMetadata(25.0), null);// new ValidateValueCallback(new MyClass().MyValidateMethod));
        public double BTWidth
        {
            get { return (double)GetValue(BTWidthProperty); }
            set { SetValue(BTWidthProperty, value); }
        }

        public static readonly DependencyProperty BTHeightProperty = DependencyProperty.Register("BTHeight", typeof(double), typeof(ContinuousButtonSub), new PropertyMetadata(25.0), null);// new ValidateValueCallback(new MyClass().MyValidateMethod));
        public double BTHeight
        {
            get { return (double)GetValue(BTHeightProperty); }
            set { SetValue(BTHeightProperty, value); }
        }


        private const Int32 defaultUpBound = -11111;
        private double _UpBound = defaultUpBound;
        public double UpBound { get { return _UpBound; } set { _UpBound = value; } }

        private const Int32 defaultDownBound = 0;
        private Int32 _DownBound = defaultDownBound;
        public Int32 DownBound { get { return _DownBound; } set { _DownBound = value; } }

        private decimal _step = 1;
        public decimal Step
        {
            get { return _step; }
            set { _step = value; }
        }
    }
}
