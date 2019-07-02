using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace GOSTS
{
    public class PersonalXMLConfig
    {
        public static string FileName = @"UserData\accpref.xml";
        public static string Root="Customizes";
        public static string SecondRoot="Customize";
        public static string AccIDTag="ID";
    }

    public class PersonTransactionOrderHeight
    {
        public static double? Height = null;
      //  public static UserOrderInfo userOrderInfo;
        string ContainerTag = "OrderDridHeiht";
        string HeightTag = "Height";
       // double Height = 0;

        public double? ReadHeight(string Acc)
        {
            double? resultHeight =null;
            string str = System.AppDomain.CurrentDomain.BaseDirectory;
            string xmlpath = str + PersonalXMLConfig.FileName;

            try
            {
                XDocument XD = XDocument.Load(xmlpath);  //;  ("Account")
                XElement cs = XD.Root.Element(PersonalXMLConfig.Root);
                if (cs == null) return resultHeight;
                var result = from acc in cs.Elements(PersonalXMLConfig.SecondRoot)
                             where (string)acc.Attribute(PersonalXMLConfig.AccIDTag) == Acc
                             select acc;
                if (result.Count() > 0)
                {
                    PreferenceAccInputModel model = new PreferenceAccInputModel();
                    var custom = result.FirstOrDefault();
                    if (custom != null)
                    {
                        var ele = custom.Element(ContainerTag);
                        if (ele != null)
                        {
                            string strHeight = ele.Element(HeightTag).Value;
                            resultHeight = Utility.ConvertToDouble(strHeight);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyMMdd-HHmmss ") + "ReadHeight in PersonTransactionOrderHeight: " + ex.Message);
            }
            return resultHeight;
        }

        public void SaveHeight(string ID)
        {
            UserOrderInfo userOrderInfo = IDelearStatus.getLastUserOrderInfo();
            if (userOrderInfo == null) return;
            if (userOrderInfo.rowOrder != null)
            {
                if (userOrderInfo.rowOrder.ActualHeight != null)
                {
                    if(userOrderInfo.rowOrder.ActualHeight>0.001)
                    SaveHeight(userOrderInfo.rowOrder.ActualHeight, ID);
                }
            }
        }

        public  void SaveHeight(double height,string ID)
        {           
            string id = GOSTradeStation.UserID;
            string str = System.AppDomain.CurrentDomain.BaseDirectory;
            string xmlpath = str + PersonalXMLConfig.FileName;

            XDocument XD;
            try
            {
                XD = XDocument.Load(xmlpath);
                XElement cs = XD.Root.Element(PersonalXMLConfig.Root);
                if (cs == null)
                {
                    cs = new XElement(PersonalXMLConfig.Root);
                    XD.Add(cs);
                }
                // cs;
                var result = from acc in cs.Elements(PersonalXMLConfig.SecondRoot)
                             where (string)acc.Attribute(PersonalXMLConfig.AccIDTag) == ID
                             select acc;
                if (result.Count() <= 0)
                {

                    var ele = new XElement(PersonalXMLConfig.SecondRoot, new XAttribute(PersonalXMLConfig.AccIDTag, id),
                       new XElement(ContainerTag,
                            new XElement(HeightTag, height.ToString().Trim()) 
                        )
                      );
                    cs.Add(ele);
                }
                else
                {
                    var custom = result.FirstOrDefault();
                    if (custom != null)
                    {
                        var ele = custom.Element(ContainerTag);
                        if (ele != null)
                        {
                            ele.Element(HeightTag).Value = height.ToString().Trim();                           
                        }
                        else
                        {
                            var ele1 =
                               new XElement(ContainerTag,
                                   new XElement(HeightTag, height.ToString().Trim()) 
                              );
                            custom.Add(ele1);
                        }
                    }
                }

                XD.Save(xmlpath);             
                // GOSTradeStation.PrefAccInputModel = PrefAccInput.ReadAccPref(GOSTradeStation.UserID);
            }
            catch (Exception ex) {
                TradeStationLog.WriteError(DateTime.Now.ToString("yyMMdd-HHmmss ") + "SaveHeight in PersonTransactionOrderHeight: " + ex.Message);
            }
        }
    }
}
