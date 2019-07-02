using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes; 
using WPF.MDI;
using System.Threading;

namespace GOSTS.WPFControls
{
    /// <summary>
    /// Clock.xaml 的交互逻辑
    /// </summary>
    public partial class Clock : UserControl
    {
        public MdiChild mdiChild { get; set; }

        public Clock()
        {
            InitializeComponent();
             
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           // this.mdiChild.Width = this.mdiChild.ActualWidth;
            //this.mdiChild.Height = this.mdiChild.ActualHeight;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        { 
        }
    }
}
