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
using System.Xml;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace GOSTS.Preference
{
    /// <summary>
    /// AccManage.xaml 的交互逻辑
    /// </summary>
    public partial class AccManage : UserControl
    {
        public AccManage()
        {
            InitializeComponent();
            AccListConfigManage.LoadAccList(GOSTradeStation.UserID);
            this.lsAcc.ItemsSource = AccListConfigManage.accoutCL;
        }
      

        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            string strAcc = this.txtAcc.Text.Trim();
            if (strAcc == "")
            {
                return;
            }
            AccListConfigManage.Add(strAcc);
            this.txtAcc.Text = "";
        }

        private void btnDel_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.lsAcc.SelectedValue == null)
            {
                return;
            }
            string str=this.lsAcc.SelectedValue.ToString().Trim();
            if(str=="")return;
            AccListConfigManage.Delete(str);            
        }

        public bool bCheckChange()
        {
            if (this.txtAcc.Text.Trim() != "")
            {
                return true;
            }
            return false;
        }
    }

    public class AccListConfigManage
    {
        static string FileName = @"UserData\accpref.xml";
        static string xmlpath = System.AppDomain.CurrentDomain.BaseDirectory + FileName;
        public static ObservableCollection<string> accoutCL = new ObservableCollection<string>();
       
        static AccListConfigManage()
        {
            LoadAccList(GOSTradeStation.UserID);
        }
        public static void Delete(string strAcc)
        {          
            XDocument XD;
            try
            {
                XD = XDocument.Load(xmlpath);
                XElement cs = XD.Root.Element("Customizes");
                if (cs == null)
                {
                    return;
                }
                // cs;
                var result = from acc in cs.Elements("Customize")
                             where (string)acc.Attribute("ID") == GOSTradeStation.UserID
                             select acc;
                if (result.Count() <= 0)
                {
                    return;
                }

                XElement xe = result.FirstOrDefault();
                result = from ele in xe.Elements("AccList").Elements("acc")
                         where ele.Value == strAcc
                         select ele;
                if (result.Count() > 0)
                {
                    result.Remove();
                }
                XD.Save(xmlpath);

                if (accoutCL.Contains(strAcc))
                {
                    accoutCL.Remove(strAcc);
                }
            }
            catch
            {
                MessageBox.Show(CommonRsText.strRs_Del_Failed);//"Delete failed");
            }
        }

        public static void Add(string strAcc)
        {
            XDocument XD;
            try
            {
                XD = XDocument.Load(xmlpath);
                XElement cs = XD.Root.Element("Customizes");
                if (cs == null)
                {
                    cs = new XElement("Customizes");
                    XD.Add(cs);
                }
                // cs;

                XElement ele;
                var result = from acc in cs.Elements("Customize")
                             where (string)acc.Attribute("ID") == GOSTradeStation.UserID
                             select acc;
                if (result.Count() <= 0)
                {
                    ele = new XElement("Customize", new XAttribute("ID", GOSTradeStation.UserID));
                    cs.Add(ele);
                }
                else
                {
                    ele = result.FirstOrDefault();
                }
                result = from acc in ele.Elements("AccList")
                         select acc;
                //where (string)acc.Attribute("ID") == GOSTradeStation.UserID
                if (result.Count() <= 0)
                {
                    var ele1 = new XElement("AccList", new XAttribute("ID", GOSTradeStation.UserID));
                    ele.Add(ele1);
                    ele = ele1;
                }
                else
                {
                    ele = result.FirstOrDefault();
                }

                result = from acc in ele.Elements("acc")
                         where (string)acc.Value == strAcc
                         select acc;
                if (result.Count() > 0)
                {
                    MessageBox.Show(CommonRsText.strRs_Acc_Existed);
                    return;
                }
                else
                {
                    var accEle = new XElement("acc", strAcc);
                    ele.Add(accEle);
                }


                XD.Save(xmlpath);
                MessageBox.Show(CommonRsText.strRs_Save_Successfully);// "saved successfullly");
                // Reload();
                // GOSTradeStation.PrefAccInputModel = PrefAccInput.ReadAccPref(GOSTradeStation.UserID);

                LoadAccList(GOSTradeStation.UserID);
            }
            catch (Exception ex) { MessageBox.Show(CommonRsText.strRs_Add_Failed);  }
        }


        public static void LoadAccList(string UID)
        {
            List lsAc = new List();
            XDocument XD;
            try
            {
                XD = XDocument.Load(xmlpath);  //;  ("Account")
            }
            catch { return; }

            XElement cs = XD.Root.Element("Customizes");
            if (cs == null) return;
            var result = from acc in cs.Elements("Customize")
                         where (string)acc.Attribute("ID") == UID
                         select acc;
            if (result.Count() > 0)
            {
                var custom = result.FirstOrDefault();
                if (custom != null)
                {
                    var ele = custom.Element("AccList");
                    if (ele != null)
                    {
                        var list = from ls in ele.Elements("acc")
                                   select ls.Value;
                        List<string> lt = list.ToList();
                        foreach (string ac in lt)
                        {
                            if (!accoutCL.Contains(ac))
                            {
                                accoutCL.Add(ac);
                            }
                        }

                    }
                }
            }

        }
    }   
}
