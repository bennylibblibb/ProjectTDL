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
    /// ucMaskMsg.xaml 的交互逻辑
    /// </summary>
    public partial class ucMaskMsg : UserControl
    {
        public ucMaskMsg(UIElementCollection _parent)
        {
            InitializeComponent();
            UIParent = _parent;
            this.DataContext = this;
            LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
            myLinearGradientBrush.StartPoint = new Point(0.504, 0.03);
            myLinearGradientBrush.EndPoint = new Point(0.504, 1.5);
            myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFD7D9F0"), 0.042));
            myLinearGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF5187D4"), 0.925));
            this.BG = myLinearGradientBrush;
        }

        UIElementCollection UIParent;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            closeMsg();
        }

        private void Grid_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            closeMsg();
        }

        void closeMsg()
        {
            if (UIParent != null)
            {
                UIParent.Remove(this);
            }
        }

        public static readonly DependencyProperty BGProperty = DependencyProperty.Register("BG", typeof(Brush), typeof(ucMaskMsg));
        public Brush BG
        {
            get { return (Brush)GetValue(BGProperty); }
            set { SetValue(BGProperty, value); }
        }

        public static readonly DependencyProperty FSizeProperty = DependencyProperty.Register("FSize", typeof(double), typeof(ucMaskMsg));
        public double FSize
        {
            get { return (double)GetValue(FSizeProperty); }
            set { SetValue(FSizeProperty, value); }
        }


        //public static RemoveUcMask(Control obj)
        //{
        //    List<ucMaskMsg> list=GetChildObjects<ucMaskMsg>(obj);
            
        //}
       
    }
}
