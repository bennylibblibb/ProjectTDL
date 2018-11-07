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
        public static object Deserialize(string message)
        {
            try
            {
                if (message == "")
                {
                    return null;
                }
                var objJSON = JObject.Parse(message);
                return JsonConvert.DeserializeObject<DOSEventJson.api>(objJSON.ToString());
            }
            catch (Exception e)
            {
                Files.WriteError("AMQP:" + message + "\r\n" + e.Message.ToString());
                return null;
            }
        }
    }
}
