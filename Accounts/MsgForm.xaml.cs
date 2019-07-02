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
using System.Windows.Shapes;

namespace GOSTS
{
    /// <summary>
    /// MsgForm.xaml 的交互逻辑
    /// </summary>
    public partial class MsgForm : Window
    {
        public MsgForm()
        {
            InitializeComponent();
            this.DataContext = this;
            fontsize = 13;
        }
        public MsgForm(string title,string msg,int _fontsize)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Title = title;
            fontsize = _fontsize;
            this.txtMsg.Text = msg;
            if (this.Owner != null)
            {
                double d=this.Owner.Width;
            }
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
           // Parent.
            this.DialogResult = true;
        }

        public int fontsize { get; set; }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
           // return false;
            this.DialogResult = false;
        }
       
    }
}
