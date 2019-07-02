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
    /// AppPreference.xaml 的交互逻辑
    /// </summary>
    public partial class AppPreference : UserControl
    {
        private MdiChild _mdi;
        public MdiChild mdiChild { get { return _mdi; } set { _mdi = value; PassMdiToChild(value); } }

        // CustomizeDataModel customizeDataModel;
        // CustomizeDataModel2 customizeDataModel2;
        CustomizeData customizeData;

        public AppPreference()
        {
            InitializeComponent();
            // customizeDataModel = new CustomizeDataModel();
            //customizeDataModel2 = new CustomizeDataModel2();
            customizeData = new CustomizeData();
            if (GOSTradeStation.isDealer == false)
            {
                //tabAcMask.Visibility = Visibility.Collapsed;
                //tabAcMn.Visibility = Visibility.Collapsed;

                this.tabAppPre.Items.Remove(tabAcMask);
                this.tabAppPre.Items.Remove(tabMgColor);
                this.tabAppPre.Items.Remove(tabAcMn);
            }
        }

        private void txtPer1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPer1.Text == "" || txtPer2.Text == "" || txtPer3.Text == "" || txtPer4.Text == "" || txtPer5.Text == "" || txtPer6.Text == "") return;

            TextBox textbox = (TextBox)e.OriginalSource;
            switch ((string)textbox.Tag)
            {
                case "txtPer1":
                    if (Convert.ToInt32(txtPer1.Text.Trim()) == 99)
                    {
                        this.CB1.IsEnabled = false;
                        this.CB2.IsEnabled = true;
                    }
                    else if (Convert.ToInt32((txtPer1.Text.Trim())) == Convert.ToInt32(txtPer2.Text.Trim()) + 1)
                    {
                        this.CB1.IsEnabled = true;
                        this.CB2.IsEnabled = false;
                    }
                    else
                    {
                        this.CB1.IsEnabled = true;
                        this.CB2.IsEnabled = true;
                    }
                    break;
                case "txtPer2":
                    if (Convert.ToInt32(txtPer2.Text.Trim()) == Convert.ToInt32(txtPer1.Text.Trim()) - 1)
                    {
                        this.CB3.IsEnabled = false;
                        this.CB4.IsEnabled = true;
                    }
                    else if (Convert.ToInt32((txtPer2.Text.Trim())) == Convert.ToInt32(txtPer3.Text.Trim()) + 1)
                    {
                        this.CB3.IsEnabled = true;
                        this.CB4.IsEnabled = false;
                    }
                    else
                    {
                        this.CB3.IsEnabled = true;
                        this.CB4.IsEnabled = true;
                    }
                    break;
                case "txtPer3":
                    if (Convert.ToInt32(txtPer3.Text.Trim()) == Convert.ToInt32(txtPer2.Text.Trim()) - 1)
                    {
                        this.CB5.IsEnabled = false;
                        this.CB6.IsEnabled = true;
                    }
                    else if (Convert.ToInt32((txtPer3.Text.Trim())) == Convert.ToInt32(txtPer4.Text.Trim()) + 1)
                    {
                        this.CB5.IsEnabled = true;
                        this.CB6.IsEnabled = false;
                    }
                    else
                    {
                        this.CB5.IsEnabled = true;
                        this.CB6.IsEnabled = true;
                    }
                    break;
                case "txtPer4":
                    if (Convert.ToInt32(txtPer4.Text.Trim()) == Convert.ToInt32(txtPer3.Text.Trim()) - 1)
                    {
                        this.CB7.IsEnabled = false;
                        this.CB8.IsEnabled = true;
                    }
                    else if (Convert.ToInt32((txtPer4.Text.Trim())) == Convert.ToInt32(txtPer5.Text.Trim()) + 1)
                    {
                        this.CB7.IsEnabled = true;
                        this.CB8.IsEnabled = false;
                    }
                    else
                    {
                        this.CB7.IsEnabled = true;
                        this.CB8.IsEnabled = true;
                    }
                    break;
                case "txtPer5":
                    if (Convert.ToInt32(txtPer5.Text.Trim()) == Convert.ToInt32(txtPer4.Text.Trim()) - 1)
                    {
                        this.CB9.IsEnabled = false;
                        this.CB10.IsEnabled = true;
                    }
                    else if (Convert.ToInt32((txtPer5.Text.Trim())) == Convert.ToInt32(txtPer6.Text.Trim()) + 1)
                    {
                        this.CB9.IsEnabled = true;
                        this.CB10.IsEnabled = false;
                    }
                    else
                    {
                        this.CB9.IsEnabled = true;
                        this.CB10.IsEnabled = true;
                    }
                    break;
                case "txtPer6":
                    if (Convert.ToInt32(txtPer6.Text.Trim()) == Convert.ToInt32(txtPer5.Text.Trim()) - 1)
                    {
                        this.CB11.IsEnabled = false;
                        this.CB12.IsEnabled = true;
                    }
                    else if (Convert.ToInt32((txtPer6.Text.Trim())) == 1)
                    {
                        this.CB11.IsEnabled = true;
                        this.CB12.IsEnabled = false;
                    }
                    else
                    {
                        this.CB11.IsEnabled = true;
                        this.CB12.IsEnabled = true;
                    }
                    break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mdiChild.Closing += new RoutedEventHandler((this.MdiCloseing_Click));
        }

        private void MdiCloseing_Click(object sender, RoutedEventArgs e)
        {
            //ClosingEventArgs eventArgs = e as ClosingEventArgs;
            //eventArgs.Cancel = true;
            int sindex = this.tabAppPre.SelectedIndex;
            if (sindex != -1)
            {
                TabItem ti = this.tabAppPre.SelectedItem as TabItem;
                if (ti != null)
                {
                    string tiName = ti.Name.ToLower().Trim();
                    switch (tiName)
                    {
                        case "tabacmask":
                            if (accprf.bCheckChange())
                            {
                                if (MessageBox.Show(CommonRsText.strRs_confirm_accMask_SaveAsk, CommonRsText.strRs_confirmTitle, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    accprf.save();
                                }
                            }
                            break;
                        case "tabmgcolor":
                            if (isChanged("tabmgcolor"))
                            {
                                if (MessageBox.Show(CommonRsText.strRs_ask_Save_Changes, CommonRsText.strRs_confirmTitle, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    this.btnOK_Click(sender, null);
                                }
                            }
                            break;
                        case "tabgeneral":
                            if (isChanged("tabgeneral"))
                            {
                                string str = (((TabItem)this.tabAppPre.SelectedItem).Header).ToString();
                                if (MessageBox.Show(CommonRsText.strRs_ask_Save_Changes, CommonRsText.strRs_confirmTitle, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    this.generalPreference1.Save_Click(this, e);
                                }
                            }
                            break; 
                        case "taborderlm":
                            if (accOrderLM.bCheckChange())
                            {
                                if (MessageBox.Show(CommonRsText.strRs_confirm_OrderDeviation_change_AskSave, CommonRsText.strRs_confirmTitle, MessageBoxButton.YesNo) == MessageBoxResult.Yes)//"Order deviation config changed,want to save?"
                                {
                                    accOrderLM.Save();
                                }                               
                            }
                            break;
                    }
                }
            }
            //switch (this.CurrentTabItem)
            //{
            //    case 0:
            //        if (accprf.bCheckChange())
            //        {
            //            if (MessageBox.Show("Acc mast config changed,save?", "Confirm ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //            {
            //                accprf.save();
            //            }
            //        }
            //        break;
            //    case 1:
            //        if (isChanged(1))
            //        {
            //            if (MessageBox.Show("Do you want to save the changes?", "Confirm ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //            {
            //                this.btnOK_Click(sender, null);
            //            }
            //        }
            //        break;
            //}
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.mdiChild.Close();
        }

        int CurrentTabItem = 0;
        private void tabAppPre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentTabItem = this.tabAppPre.SelectedIndex;
            if (CurrentTabItem != -1)
            {
                TabItem ti = this.tabAppPre.SelectedItem as TabItem;
                if (ti != null)
                {
                    string tiName = ti.Name.ToLower().Trim();
                    switch (tiName)
                    {
                        case "tabacmask":

                            break;
                        case "tabmgcolor":
                            customizeData.MarginColorData = (customizeData.MarginColorData == null) ? CustomizeDataModel2.Create2(GOSTradeStation.UserID, (((TabItem)this.tabAppPre.SelectedItem).Tag).ToString()).MarginColorData : customizeData.MarginColorData;
                            MarginColorData data = customizeData.MarginColorData;
                            if (data == null) return;
                            txtPer1.Text = data.MarginColors[0].Percent.ToString();
                            ForeColorPicker1.CurrentColor = data.MarginColors[0].Color;
                            txtPer2.Text = data.MarginColors[1].Percent.ToString();
                            ForeColorPicker2.CurrentColor = data.MarginColors[1].Color;
                            txtPer3.Text = data.MarginColors[2].Percent.ToString();
                            ForeColorPicker3.CurrentColor = data.MarginColors[2].Color;
                            txtPer4.Text = data.MarginColors[3].Percent.ToString();
                            ForeColorPicker4.CurrentColor = data.MarginColors[3].Color;
                            txtPer5.Text = data.MarginColors[4].Percent.ToString();
                            ForeColorPicker5.CurrentColor = data.MarginColors[4].Color;
                            txtPer6.Text = data.MarginColors[5].Percent.ToString();
                            ForeColorPicker6.CurrentColor = data.MarginColors[5].Color;
                            ForeColorPicker7.CurrentColor = data.MarginColors[6].Color;
                            if (data.isEnabled)
                            {
                                this.chkNoColor.IsChecked = true;
                            }
                            break;
                        case "tabgeneral":
                            customizeData.AlertData =CustomizeDataModel2.Create2(GOSTradeStation.UserID, (((TabItem)this.tabAppPre.SelectedItem).Tag).ToString()).AlertData;//(customizeData.AlertData == null) ? CustomizeDataModel2.Create2(GOSTradeStation.UserID, (((TabItem)this.tabAppPre.SelectedItem).Header).ToString()).AlertData : customizeData.AlertData;
                            this.generalPreference1.DataContext = customizeData.AlertData;
                            break;
                    }
                }
            }


            //switch (this.CurrentTabItem)
            //{
            //    case 0:

            //        break;
            //    case 1:
            //        customizeData.MarginColorData = (customizeData.MarginColorData == null) ? CustomizeDataModel2.Create2(GOSTradeStation.UserID, (((TabItem)this.tabAppPre.SelectedItem).Header).ToString()).MarginColorData : customizeData.MarginColorData;
            //        MarginColorData data = customizeData.MarginColorData;

            //        //customizeDataModel2.CustomizeDataes.MarginColorData = (customizeDataModel2.CustomizeDataes.MarginColorData == null) ? CustomizeDataModel2.Create(GOSTradeStation.UserID, (((TabItem)this.tabAppPre.SelectedItem).Header).ToString()).CustomizeDataes.MarginColorData : customizeDataModel2.CustomizeDataes.MarginColorData;
            //        //  MarginColorData data = customizeDataModel2.CustomizeDataes.MarginColorData;

            //        //customizeDataModel.MarginColorData = (customizeDataModel.MarginColorData == null) ? CustomizeDataModel.Create(GOSTradeStation.UserID, (((TabItem)this.tabAppPre.SelectedItem).Header).ToString()).MarginColorData : customizeDataModel.MarginColorData;
            //        //  customizeDataModel.MarginColorData = (customizeDataModel.MarginColorData == null) ? (new CustomizeDataModel(GOSTradeStation.UserID, (((TabItem)this.tabAppPre.SelectedItem).Header).ToString())).MarginColorData : customizeDataModel.MarginColorData;

            //        // MarginColorData data = customizeDataModel.MarginColorData;

            //        txtPer1.Text = data.MarginColors[0].Percent.ToString();
            //        ForeColorPicker1.CurrentColor = data.MarginColors[0].Color;
            //        txtPer2.Text = data.MarginColors[1].Percent.ToString();
            //        ForeColorPicker2.CurrentColor = data.MarginColors[1].Color;
            //        txtPer3.Text = data.MarginColors[2].Percent.ToString();
            //        ForeColorPicker3.CurrentColor = data.MarginColors[2].Color;
            //        txtPer4.Text = data.MarginColors[3].Percent.ToString();
            //        ForeColorPicker4.CurrentColor = data.MarginColors[3].Color;
            //        txtPer5.Text = data.MarginColors[4].Percent.ToString();
            //        ForeColorPicker5.CurrentColor = data.MarginColors[4].Color;
            //        txtPer6.Text = data.MarginColors[5].Percent.ToString();
            //        ForeColorPicker6.CurrentColor = data.MarginColors[5].Color;
            //        ForeColorPicker7.CurrentColor = data.MarginColors[6].Color;
            //        if (data.isEnabled)
            //        {
            //            this.chkNoColor.IsChecked = true;
            //        }
            //        break;
            //    case 2:
            //        customizeData.AlertData = (customizeData.AlertData == null) ? CustomizeDataModel2.Create2(GOSTradeStation.UserID, (((TabItem)this.tabAppPre.SelectedItem).Header).ToString()).AlertData : customizeData.AlertData;
            //        this.generalPreference1.DataContext = customizeData.AlertData;
            //        break;
            //}

        }

        private bool isChanged(string IndexName)
        {
            switch (IndexName)
            {
                case "":
                    return false;
                case "tabmgcolor":
                    MarginColorData data = customizeData.MarginColorData; 
                    if (data == null) return false;
                    MarginColorData newData = LoadCurrentData(IndexName).MarginColorData;
                    if (newData.isEnabled != data.isEnabled)
                    {
                        return true;
                    }
                    else
                    {
                        for (int i = 0; i < newData.MarginColors.Count(); i++)
                        {
                            if (newData.MarginColors[i].Color.ToString() != data.MarginColors[i].Color.ToString() || newData.MarginColors[i].Percent != data.MarginColors[i].Percent)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                case "tabgeneral":
                    AlertData alertData = customizeData.AlertData;
                    if (alertData == null) return false;
                    AlertData oldAlertData = LoadCurrentData(IndexName).AlertData;
                    if (oldAlertData.isNewAppAlertS != alertData.isNewAppAlertS ||
                        oldAlertData.isOrderConfirmS != alertData.isOrderConfirmS ||
                        oldAlertData.isOrderConfirmT != alertData.isOrderConfirmT ||
                        oldAlertData.isReqAccAlertS != alertData.isReqAccAlertS ||
                        oldAlertData.isReqAccAlertT != alertData.isReqAccAlertT ||
                        oldAlertData.isReqErrAlertS != alertData.isReqErrAlertS ||
                        oldAlertData.isReqErrAlertT != alertData.isReqErrAlertT ||
                        oldAlertData.isTimeCorrAlertT != alertData.isTimeCorrAlertT ||
                        oldAlertData.isTradeAlertS != alertData.isTradeAlertS ||
                        oldAlertData.isTradeAlertT != alertData.isTradeAlertT ||
                        oldAlertData.NotifyLevel != alertData.NotifyLevel)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }

        private CustomizeData LoadCurrentData(string IndexName)
        {
            CustomizeData data = new CustomizeData();
            data.UserID = GOSTradeStation.UserID;

            switch (IndexName)
            {
                case "tabmgcolor":
                    MarginColorData marginColorData = new MarginColorData();
                    marginColorData.UserID = GOSTradeStation.UserID;
                    marginColorData.Type = "MarginColor";
                    if (chkNoColor.IsChecked == true)
                    {
                        data.MarginColorData.isEnabled = true;
                    }
                    for (int i = 1; i < 8; i++)
                    {
                        if (i == 7)
                        {
                            marginColorData.MarginColors.Add(new MarginColor
                            {
                                Percent =AppFlag.InvalidNum,
                                Color = ((ColorPickerControlView)this.FindName("ForeColorPicker" + i.ToString())).CurrentColor
                            });
                        }
                        else
                        {
                            marginColorData.MarginColors.Add(new MarginColor
                            {
                                Percent = Convert.ToInt32(((TextBox)this.FindName("txtPer" + i.ToString())).Text.Trim()),
                                Color = ((ColorPickerControlView)this.FindName("ForeColorPicker" + i.ToString())).CurrentColor
                            });
                        }
                    }

                    data.Type = "MarginColorData";
                    data.MarginColorData = marginColorData;
                    return data;
                case "tabgeneral":
                    data.Type = "AlertData";
                    data.AlertData = CustomizeDataModel2.Create2(GOSTradeStation.UserID, ((TabItem)(tabAppPre.SelectedItem)).Tag.ToString()).AlertData;
                    return data;
                default:
                    return null;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            TabItem ti = this.tabAppPre.SelectedItem as TabItem;
            if (ti != null)
            {
                string tiName = ti.Name.ToLower().Trim();
                if (TradeStationSetting.SaveCustomizeData(LoadCurrentData(tiName)))
                {
                    customizeData = CustomizeDataModel2.Create2(GOSTradeStation.UserID, (((TabItem)this.tabAppPre.SelectedItem).Tag).ToString());
                    TradeStationSend.Send(cmdClient.getMarginCallList);
                    TradeStationLog.WriteForHearbeatandMarginCall(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " ReSend MarginCallList.");
                }
            }
            // this.mdiChild.Close();
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            MarginColorData data = TradeStationSetting.ReturnCustomizeData("DEFAULT");
            if (data == null || data.MarginColors == null) return;

            txtPer1.Text = data.MarginColors[0].Percent.ToString();
            ForeColorPicker1.CurrentColor = data.MarginColors[0].Color;
            txtPer2.Text = data.MarginColors[1].Percent.ToString();
            ForeColorPicker2.CurrentColor = data.MarginColors[1].Color;
            txtPer3.Text = data.MarginColors[2].Percent.ToString();
            ForeColorPicker3.CurrentColor = data.MarginColors[2].Color;
            txtPer4.Text = data.MarginColors[3].Percent.ToString();
            ForeColorPicker4.CurrentColor = data.MarginColors[3].Color;
            txtPer5.Text = data.MarginColors[4].Percent.ToString();
            ForeColorPicker5.CurrentColor = data.MarginColors[4].Color;
            txtPer6.Text = data.MarginColors[5].Percent.ToString();
            ForeColorPicker6.CurrentColor = data.MarginColors[5].Color;
            ForeColorPicker7.CurrentColor = data.MarginColors[6].Color;
            if (data.isEnabled)
            {
                this.chkNoColor.IsChecked = true;
            }

            TradeStationSend.Send(cmdClient.getMarginCallList);
            TradeStationLog.WriteForHearbeatandMarginCall(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff") + " ReSend MarginCallList..");

        }

        private void bd_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            bool bChange = false;
            int selectIndex = this.tabAppPre.SelectedIndex;
            if (selectIndex != -1)
            {
                TabItem ti = this.tabAppPre.SelectedItem as TabItem;
                if (ti != null)
                {
                    string tiName = ti.Name.ToLower().Trim();
                    switch (tiName)
                    {
                        case "tabacmask":
                            if (accprf.bCheckChange())
                            {
                                bChange = true;
                                if (bChange)
                                {
                                    if (MessageBox.Show(CommonRsText.strRs_confirm_accMask_Ask_StaySave, CommonRsText.strRs_confirmTitle, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                    {
                                        e.Handled = true;
                                    }
                                }
                            }
                            break;
                        case "tabmgcolor":
                            if (isChanged("tabmgcolor"))
                            {
                                if (MessageBox.Show(CommonRsText.strRs_ask_Save_Changes, CommonRsText.strRs_confirmTitle, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    this.btnOK_Click(sender, null);
                                }
                            }
                            break;
                        case "tabgeneral":
                            if (isChanged("tabgeneral"))
                            {
                                string str = (((TabItem)this.tabAppPre.SelectedItem).Header).ToString();
                                if (MessageBox.Show(CommonRsText.strRs_ask_Save_Changes, CommonRsText.strRs_confirmTitle, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    this.generalPreference1.Save_Click(this, e);
                                }
                            }
                            break;
                        case "taborderlm":
                            if (accOrderLM.bCheckChange())
                            {
                                bChange = true;
                                if (bChange)
                                {
                                    if (MessageBox.Show(CommonRsText.strRs_confirm_OrderDeviation_change_Ask_StaySave, CommonRsText.strRs_confirmTitle, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                    {
                                        e.Handled = true;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            //bool bChange = false;
            //switch (this.tabAppPre.SelectedIndex)
            //{
            //    case 0:
            //        if (accprf.bCheckChange())
            //        {
            //            bChange = true;
            //            if (bChange)
            //            {
            //                if (MessageBox.Show("Acc mast config changed,stay to save?", "Confirm ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //                {
            //                    e.Handled = true;
            //                }
            //            }
            //        }
            //        break;
            //    case 1:
            //        //  if (isChanged(data))
            //        if (isChanged(1))
            //        {
            //            if (MessageBox.Show("Do you want to save the changes?", "Confirm ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //            {
            //                this.btnOK_Click(sender, null);
            //            }
            //        }
            //        break;
            //    case 2:
            //        if (isChanged(2))
            //        {
            //            string str = (((TabItem)this.tabAppPre.SelectedItem).Header).ToString();
            //            if (MessageBox.Show("Do you want to save the changes?", "Confirm ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //            {
            //                this.generalPreference1.Save_Click(this, e);
            //            }
            //        }
            //        break;
            //    case 4:
            //        if (accOrderLM.bCheckChange())
            //        {
            //            bChange = true;
            //            if (bChange)
            //            {
            //                if (MessageBox.Show("Order deviation config changed,stay to save?", "Confirm ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //                {
            //                    e.Handled = true;
            //                }
            //            }
            //        }
            //        break;
            //    default:
            //        break;
            //}
        }

        void PassMdiToChild(MdiChild mdi)
        {
            // if (mdi != null)
            {
                if (accprf != null)
                {
                    accprf.mdiThis = mdi;
                }
            }
        }
    }
}
