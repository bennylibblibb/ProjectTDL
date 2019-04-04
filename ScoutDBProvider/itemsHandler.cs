using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ScoutDBProvider
{
    public class itemsHandler : IConfigurationSectionHandler
    {
        public Object Create(Object parent, Object configContext, System.Xml.XmlNode section)
        {
            XmlNodeList itemsNodeList;
            ArrayList itemsList = new ArrayList();
            itemsNodeList = section.SelectNodes("item");

            foreach (XmlNode elementNode in itemsNodeList)
            {
                itemsList.Add(elementNode.InnerText);
            }

            return itemsList;
        }
    }
}
