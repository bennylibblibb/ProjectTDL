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
using WPF.MDI;

namespace GOSTS
{
    /// <summary>
    /// AccPreference.xaml 的交互逻辑
    /// </summary>
    public partial class AccPreference : UserControl
    {
        MdiChild mdiThis;
        string FileName = @"UserData\accpref.xml";
        public AccPreference()
        {
            InitializeComponent();
            Reload();           
        }

        void Reload()
        {
            PreferenceAccInputModel model = PrefAccInput.ReadAccPref(GOSTradeStation.UserID);
            if (model != null)
            {
                this.txtPre.Text = model.Prefix.Trim();
                this.txtLen.Text = model.Len.ToString().Trim();
                this.txtSuf.Text = model.Suffix.Trim();
            }
        }

        private void btnSave_Click_1(object sender, RoutedEventArgs e)
        {
            string strPre = this.txtPre.Text.Trim();
            string strLen = this.txtLen.Text.Trim();
            string strSuf = this.txtSuf.Text.Trim();
            string id = GOSTradeStation.UserID;
          
            string str = System.AppDomain.CurrentDomain.BaseDirectory;
            string xmlpath = str + FileName;

            XDocument XD;
            try
            {
                XD = XDocument.Load(xmlpath);
                var result = from acc in XD.Descendants("Account")
                             where (string)acc.Attribute("ID") == id
                             select acc;
                if (result.Count() <= 0)
                {
                    var ele = new XElement("Account",
                        new XAttribute("ID", id),
                        new XElement("Prefix", this.txtPre.Text.Trim()),
                        new XElement("MidLen", this.txtLen.Text.Trim()),
                        new XElement("Suffix", this.txtSuf.Text.Trim())
                        );
                    XD.Root.Add(ele);
                }
                else
                {
                    var ele = result.FirstOrDefault();
                    ele.Attribute("ID").Value = id;
                    ele.Element("Prefix").Value = this.txtPre.Text.Trim();
                    ele.Element("MidLen").Value = this.txtLen.Text.Trim();
                    ele.Element("Suffix").Value = this.txtSuf.Text.Trim();
                }
                XD.Save(xmlpath);
                MessageBox.Show("saved successfullly");
                GOSTradeStation.PrefAccInputModel = PrefAccInput.ReadAccPref(GOSTradeStation.UserID);
            }
            catch (Exception ex) { MessageBox.Show("havs no right to write file"); }
        }

        private void btnCancel_Click_1(object sender, RoutedEventArgs e)
        {
            Reload();   
        } 

    }

    //public class PreferenceAccInputModel
    //{

    //    public string Prefix;
    //    public int Len;
    //    public string Suffix;
    //}

    //public class PrefAccInput
    //{
    //    static string FileName = @"UserData\accpref.xml";
    //    public static PreferenceAccInputModel ReadAccPref(string Acc)
    //    {
    //        string str = System.AppDomain.CurrentDomain.BaseDirectory;
    //        string xmlpath = str + FileName;

    //        XDocument XD = XDocument.Load(xmlpath);
    //        var result = from acc in XD.Descendants("Account")
    //                     where (string)acc.Attribute("ID") == Acc
    //                     select acc;
    //        if (result.Count() > 0)
    //        {
    //            PreferenceAccInputModel model = new PreferenceAccInputModel();
    //            var ele = result.FirstOrDefault();
    //            model.Prefix = ele.Element("Prefix").Value;
    //            string strLen = ele.Element("MidLen").Value;
    //            model.Len = Utility.ConvertToInt(strLen);
    //            model.Suffix = ele.Element("Suffix").Value;
    //            return model;
    //        }
    //        return null;
    //    }

    //    public static string ToPref(string str, PreferenceAccInputModel model)
    //    {
    //        if (model == null) return str;
    //        string str1 = str.PadLeft(model.Len, '0');
    //        string pre = model.Prefix == null ? "" : model.Prefix;
    //        str1 = pre + str1 + model.Suffix;
    //        return str1;
    //    }
    //}
}
