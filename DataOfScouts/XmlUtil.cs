using System;
using System.Collections.Generic; 
using System.Text;
using System.IO;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using FileLog;

namespace DataOfScouts
{
    public class XmlUtil
    {
        public static object Deserialize(Type type, string xml)
        {  
            try
            {
                if (xml == "")
                {
                    return null;
                }
                //else if (xml.IndexOf("PubSubInfo>") > 0 && xml.Length > xml.IndexOf("PubSubInfo>") + "PubSubInfo>".Length + 4)
                //{
                //    Files.WriteLog(LogType.SerializerLog, "Xml Deserialize Error: " + strName + ".xml,Try again.");
                //    xml = xml.Substring(0, xml.IndexOf("PubSubInfo>") + "PubSubInfo>".Length);
                //}

                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(type);
                    return xmldes.Deserialize(sr);
                }
            }
            catch (Exception e)
            {
                //Files.WriteLog(LogType.SerializerLog, "Xml Deserialize Error: " + strName+".xml");
               // Files.WriteError( strName + ".xml, " + e.Message.ToString());
                return null;
            }
        }

        public static object Deserialize(Type type, Stream stream)
        {
            XmlSerializer xmldes = new XmlSerializer(type);
            return xmldes.Deserialize(stream);
        }

        public static string Serializer(Type type, object obj)
        {
            MemoryStream Stream = new MemoryStream(); XmlSerializer xml = new XmlSerializer(type);
            try
            {
                //序列化对象  
                xml.Serialize(Stream, obj);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            Stream.Position = 0;
            StreamReader sr = new StreamReader(Stream);
            string str = sr.ReadToEnd();
            sr.Dispose();
            Stream.Dispose();
            return str;
        }
    }
}
