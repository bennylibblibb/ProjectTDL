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
using GOSTS;

namespace GOSTS.Preference
{
    /// <summary>
    /// AccPRF.xaml 的交互逻辑
    /// </summary>
    public partial class AccPRF : UserControl
    {
        public AccPRF()
        {
            InitializeComponent();
            Reload();    
        }
        public MdiChild mdiThis{get;set;}
        string FileName = @"UserData\accpref.xml";
        PreferenceAccInputModel model;
        void Reload()
        {
            model = PrefAccInput.ReadAccPref(GOSTradeStation.UserID);
            GOSTradeStation.PrefAccInputModel = model;
            if (model != null)
            {
                this.txtPre.Text = model.Prefix.Trim();
                this.txtLen.Text = model.strLen.ToString().Trim();
                this.txtSuf.Text = model.Suffix.Trim();
            }
        }

        #region RS Text
        public static string strRs_length_invalid
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("AccMask_length_invalid"); }
        }
        public static string strRs_Clear_Successfully
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("AccMask_Clear_Successfully"); }
        }
        
        public static string strRs_Clear_Failed
        {
            get { return GOSTS.GosCulture.CultureHelper.GetString("AccMask_Clear_Failed"); }
        }
        
        
        #endregion


        private void btnSave_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.txtLen.Text.Trim() != "")
            {
                try
                {
                    Convert.ToInt32(this.txtLen.Text.Trim());
                }
                catch
                {
                    MessageBox.Show(strRs_length_invalid);
                    return;
                }
            }
           // save();
            SaveAndClose();
        }

        private void btnCancel_Click_1(object sender, RoutedEventArgs e)
        {
           // Reload();  
           // Clear("", "", "");
            clear();
        }

        void clear()
        {
            this.txtPre.Text="";
            this.txtLen.Text= "";
            this.txtSuf.Text = "";
        }

        void Clear(string strPre, string strLen, string strSuf)
        {
            string id = GOSTradeStation.UserID;

            string str = System.AppDomain.CurrentDomain.BaseDirectory;
            string xmlpath = str + FileName;

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
                var result = from acc in cs.Elements("Customize")
                             where (string)acc.Attribute("ID") == id
                             select acc;
                if (result.Count() <= 0)
                {

                    var ele = new XElement("Customize", new XAttribute("ID", id),
                       new XElement("Account",
                            new XElement("Prefix", strPre),
                            new XElement("MidLen", strLen),
                            new XElement("Suffix", strSuf)
                        )
                      );
                    cs.Add(ele);
                }
                else
                {
                    var custom = result.FirstOrDefault();
                    if (custom != null)
                    {
                        var ele = custom.Element("Account");
                        if (ele != null)
                        {
                            // ele.Attribute("ID").Value = id;
                            ele.Element("Prefix").Value = strPre;
                            ele.Element("MidLen").Value = strLen;
                            ele.Element("Suffix").Value = strSuf;
                        }
                        else
                        {
                            var ele1 =
                               new XElement("Account",
                                    new XElement("Prefix", strPre),
                                    new XElement("MidLen", strLen),
                                    new XElement("Suffix", strSuf)
                              );
                            custom.Add(ele1);
                        }
                    }
                }

                XD.Save(xmlpath);
                MessageBox.Show(strRs_Clear_Successfully);
                Reload();              
            }
            catch (Exception ex) { MessageBox.Show(strRs_Clear_Failed); }
        }

        public void SaveAndClose()
        {
            save();
            if (mdiThis != null)
            {
                mdiThis.Close();
            }
        }

        public void save()
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
                XElement cs = XD.Root.Element("Customizes");
                if (cs == null)
                {
                    cs = new XElement("Customizes");
                    XD.Add(cs);
                }
                // cs;
                var result = from acc in cs.Elements("Customize")
                             where (string)acc.Attribute("ID") == id
                             select acc;
                if (result.Count() <= 0)
                {

                    var ele = new XElement("Customize", new XAttribute("ID", id),
                       new XElement("Account",
                            new XElement("Prefix", this.txtPre.Text.Trim()),
                            new XElement("MidLen", this.txtLen.Text.Trim()),
                            new XElement("Suffix", this.txtSuf.Text.Trim())
                        )
                      );
                    cs.Add(ele);
                }
                else
                {
                    var custom = result.FirstOrDefault();
                    if (custom != null)
                    {
                        var ele = custom.Element("Account");
                        if (ele != null)
                        {
                            // ele.Attribute("ID").Value = id;
                            ele.Element("Prefix").Value = this.txtPre.Text.Trim();
                            ele.Element("MidLen").Value = this.txtLen.Text.Trim();
                            ele.Element("Suffix").Value = this.txtSuf.Text.Trim();
                        }
                        else
                        {
                            var ele1 =
                               new XElement("Account",
                                    new XElement("Prefix", this.txtPre.Text.Trim()),
                                    new XElement("MidLen", this.txtLen.Text.Trim()),
                                    new XElement("Suffix", this.txtSuf.Text.Trim())
                              );
                            custom.Add(ele1);
                        }
                    }
                }

                XD.Save(xmlpath);
                MessageBox.Show(CommonRsText.strRs_Save_Successfully);//"saved successfullly");
                Reload();
               // GOSTradeStation.PrefAccInputModel = PrefAccInput.ReadAccPref(GOSTradeStation.UserID);
            }
            catch (Exception ex) { MessageBox.Show(CommonRsText.strRs_Save_Failed ); }
        }

        public bool bCheckChange()
        {
            if (model == null)
            {
                if (this.txtLen.Text.Trim() != "" || this.txtPre.Text.Trim() != "" || this.txtSuf.Text.Trim() != "")
                {
                    return true;
                }
            }

            if (model != null)
            {
                if (model.Prefix == null && this.txtPre.Text.Trim() != "")
                {
                    return true;
                }
                if (model.Prefix.Trim() != this.txtPre.Text.Trim())
                {
                    return true;
                }
                if (model.strLen == null && this.txtLen.Text.Trim() != "")
                {
                    return true;
                }
                if (model.strLen.ToString().Trim() != this.txtLen.Text.Trim())
                {
                    return true;
                }

                if (model.Suffix == null && this.txtSuf.Text.Trim() != "")
                {
                    return true;
                }
                if (model.Suffix.Trim() != this.txtSuf.Text.Trim())
                {
                    return true;
                }
                
            }
            return false;
        }

    }

   

}

namespace GOSTS
{
    public class PreferenceAccInputModel
    {
        public string Prefix;
        public int Len;
        public string strLen;
        public string Suffix;
    }

    public class PrefAccInput
    {
        static string  FileName = @"UserData\accpref.xml";
        public static PreferenceAccInputModel ReadAccPref(string Acc)
        {
            string str = System.AppDomain.CurrentDomain.BaseDirectory;
            string xmlpath = str + FileName;
            try
            {
                XDocument XD = XDocument.Load(xmlpath);  //;  ("Account")
                XElement cs = XD.Root.Element("Customizes");
                if (cs == null) return null;
                var result = from acc in cs.Elements("Customize")
                             where (string)acc.Attribute("ID") == Acc
                             select acc;
                if (result.Count() > 0)
                {
                    PreferenceAccInputModel model = new PreferenceAccInputModel();
                    var custom = result.FirstOrDefault();
                    if (custom != null)
                    {
                        var ele = custom.Element("Account");
                        if (ele != null)
                        {
                            model.Prefix = ele.Element("Prefix").Value;
                            string strLen = ele.Element("MidLen").Value;
                            model.strLen = strLen;
                            model.Len = Utility.ConvertToInt(strLen);
                            model.Suffix = ele.Element("Suffix").Value;
                            return model;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyMMdd-HHmmss ") + "ReadAccPref(string Acc),error:" + ex.Message);
                return null;
            }
        }

        public static string ToPref(string str, PreferenceAccInputModel model)
        {
            if (model == null) return str;
            if (model.Prefix != null || model.Suffix != null)
             {
                 if (model.Prefix.Length > 0 || model.Suffix.Length>0)
                 {
                     int iLen = model.Len;
                  
                     if (model.Prefix != null)
                     {
                         if (model.Prefix.Length > 0)
                         {
                             iLen = iLen + model.Prefix.Length;
                         }
                     }
                     if (model.Suffix != null)
                     {
                         if (model.Suffix.Length > 0)
                         {
                             iLen = iLen + model.Suffix.Length;
                         }
                     }
                     if (iLen == str.Length)
                     {
                         bool bSame = true;
                         if( model.Prefix!=null )
                         {
                             if (model.Prefix.Length > 0)
                             {
                                 if (str.Substring(0, model.Prefix.Length) != model.Prefix)
                                 {
                                     // return str + (model.Suffix != null ? model.Suffix : "");                                     
                                     bSame = false;
                                 }
                             }
                         }
                         if (bSame)
                         {
                             if (model.Suffix != null)
                             {
                                 if (model.Suffix.Length > 0)
                                 {
                                     if (!str.EndsWith(model.Suffix))
                                     {
                                         // return str + (model.Suffix != null ? model.Suffix : "");
                                         bSame = false;
                                     }
                                 }
                             }
                         }
                         if (bSame)
                         {
                             return str;
                         }
                     }
                    // if ((model.Len + model.Prefix.Length) == str.Length)
                     //{
                     //    if (str.Substring(0, model.Prefix.Length) == model.Prefix)
                     //    {
                     //        return str + (model.Suffix != null ? model.Suffix : "");
                     //    }
                     //}
                 }
             }
            string str1 = str.PadLeft(model.Len, '0');
            string pre = model.Prefix == null ? "" : model.Prefix;
            str1 = pre + str1 + model.Suffix;
            return str1;
        }
    }
}
