using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataOfScouts
{
    public class JsonUtil
    {
        public static object Deserialize(Type type, string message)
        {
            try
            {
                if (message == "")
                {
                    return null;
                }
                var objJSON = JObject.Parse(message);
                if (typeof(DOSEventJson.EventJson) == type)
                {
                    return JsonConvert.DeserializeObject<DOSEventJson.EventJson>(objJSON.ToString());
                }
                else if (typeof(DOSIncidentJson.IncidentJson) == type)
                {
                    return JsonConvert.DeserializeObject<DOSIncidentJson.IncidentJson>(objJSON.ToString());
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Files.WriteError("AMQP:" + message + "\r\n" + e.Message.ToString());
                return null;
            }
        }
    }
}