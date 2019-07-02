/*
 * Created by SharpDevelop.
 * User: Benny
 * Date: 2012-10-8
 * Time: 13:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace GOSTS
{
    /// <summary>
    /// Description of MessageException.
    /// </summary>
    public class MessageException : Exception
    {
        public int Code { get; set; }

        public string ErrorDescription
        {
            get
            {
                var msg = string.Empty;
                switch (Code)
                {
                    case 0:
                        msg = "No errors encountered";
                        break;
                    case 1:
                        msg = "Read Connection is unavailable!";
                        break;
                    case 2:
                        msg = "Send Connection is unavailable!";
                        break;
                    case 3:
                        msg = "Too many connection attempts!";
                        break;
                    case 4:
                        msg = "NetWorkStream is not Writable!";
                        break;
                    case 5:
                        msg = "Failed to connect!";
                        break;
                    default:
                        msg = "Undocumented error status code";
                        break;
                }
                return msg;
            }
        }

        public MessageException(int code)
            : base()
        {
            this.Code = code;
        }

        public MessageException(int code, string message)
            : base(message)
        {
            this.Code = code;
        }
    }
}
