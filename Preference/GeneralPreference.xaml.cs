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
using GOSTS.Common;
using WPF.MDI;
using CustomWPFColorPicker;

namespace GOSTS.Preference
{
    /// <summary>
    /// GeneralPreference.xaml 的交互逻辑
    /// </summary>
    public partial class GeneralPreference : UserControl
    {
        public GeneralPreference()
        {
            InitializeComponent();
        }

        public void Save_Click(object sender, RoutedEventArgs e)
        {
            CustomizeData data = new CustomizeData();
            data.Type = "AlertData";
            data.UserID = GOSTradeStation.UserID;
            data.AlertData = (AlertData)this.DataContext;
            if (TradeStationSetting.SaveCustomizeData(data))
            {
                GOSTradeStation.customizeData.AlertData = data.AlertData;
                MessageBox.Show("Saved!");
            }
            else
            {
                MessageBox.Show("Unsaved!");
            }
           // Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Close()
        {
            ((AppPreference)((Grid)((TabControl)((TabItem)this.Parent).Parent).Parent).Parent).mdiChild.Close();
        }
    }
}
