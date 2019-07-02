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
    /// ucBtnNumSub.xaml 的交互逻辑
    /// </summary>
    public partial class ucBtnNumSub : UserControl
    {
        public ucBtnNumSub()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TNumBoxProperty = DependencyProperty.Register("TNumBox", typeof(ucTBNumber), typeof(ucBtnNumSub), null, null);// new ValidateValueCallback(new MyClass().MyValidateMethod));
        public ucTBNumber TNumBox
        {
            get { return (ucTBNumber)GetValue(TNumBoxProperty); }
            set { SetValue(TNumBoxProperty, value); }
        }   

        private void btnDown_Click_1(object sender, RoutedEventArgs e)
        {
            TNumBox.Down();
        }

        public INumUpDown UD { get; set; }
    }
}
