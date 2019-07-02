using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOSTS.Common
{
    public class BaseResponse
    {
        private String _Type;
        public String Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        private String datetime;
        public String Datetime
        {
            get
            {
                return datetime;
            }
            set
            {
                datetime = value;
            }
        }

        private String _UserID;
        public String UserID
        {
            get
            {
                return _UserID;
            }
            set
            {
                _UserID = value;
            }
        }

        private String _Acc_no;
        public String Acc_no
        {
            get
            {
                return _Acc_no;
            }
            set
            {
                _Acc_no = value;
            }
        }

        private String _Rinfo;
        public String Rinfo
        {
            get
            {
                return _Rinfo;
            }
            set
            {
                _Rinfo = value;
            }
        }

        private int _Result;
        public int Result
        {
            get
            {
                return _Result;
            }
            set
            {
                _Result = value;
            }
        }

        public String B64Message
        {
            set
            {
                _Message = (_Rinfo.ToLower().IndexOf("mg=b64") > -1) ? TradeStationTools.Base64StringToString(value) : value;
            }
        }

        private String _Message;
        public String Message
        {
            get
            {
                return _Message.Trim();
            }
            set
            {
                _Message = value; // =(_Rinfo.ToLower().IndexOf("mg=b64") > -1) ? TradeStationTools.Base64StringToString(value) : value;
            }
        }
    }

    public class OrderResponse : BaseResponse
    {
    }

    public class LoginResp
    {
        private DateTime datetime;
        public DateTime DateTime
        {
            get
            {
                return datetime;
            }
            set
            {
                datetime = value;
            }
        }

        private String _UserID;
        public String UserID
        {
            get
            {
                return _UserID;
            }
            set
            {
                _UserID = value;
            }
        }
        private String _Acc_no;
        public String AccNo
        {
            get
            {
                return _Acc_no;
            }
            set
            {
                _Acc_no = value;
            }
        }

        private String _Rinfo;
        public String RInfo
        {
            get
            {
                return _Rinfo;
            }
            set
            {
                _Rinfo = value;
                if (value.IndexOf("mo=") > -1)
                {
                    string[] allItem = value.Split(new Char[] { '&' });
                    foreach (string str in allItem)
                    {
                        if (str.IndexOf("mo=") > -1)
                        {
                            if (str.Substring(3, str.Length - 3) == "1")
                            {
                                _Chk_mrgin_opt = "x";
                            }
                        }
                        else if (str.IndexOf("bs=") > -1)
                        {
                            _bs = TradeStationTools.getDateTimeFromUnixTime(str.Substring(3, str.Length - 3)).ToString();
                        }
                        else if (str.IndexOf("cp=") > -1)
                        {
                            _cp = str.Substring(3, str.Length - 3);
                        }
                        else if (str.IndexOf("tw=") > -1)
                        {
                            // "tw=RGBFBBAA5production";
                            // "tw=production";
                            // "tw=RGBxxyyzz"; 
                            string strContents = str.Substring(3, str.Length - 3);
                            if (strContents.IndexOf("RGB") == 0 && strContents.Length > 8)
                            {
                                _tc = strContents.Substring(3, 6);
                                _tw = strContents.Substring(9, strContents.Length - 9);
                            }
                            else
                            {
                                _tw = strContents;
                                _tc = "";
                            } 
                        }

                    }
                }
                else
                {
                    _Chk_mrgin_opt = null;
                }
            }
        }

        private String _Chk_mrgin_opt;
        public String Chk_mrgin_opt
        {
            get
            {
                return _Chk_mrgin_opt;
            }
            set
            {
                _Chk_mrgin_opt = value;
            }
        }

        private String _bs;
        public String BS
        {
            get
            {
                return _bs;
            }
            set
            {
                _bs = value;
            }
        }


        private bool _Result;
        public bool Result
        {
            get
            {
                return _Result;
            }
            set
            {
                _Result = value;
            }
        }

        private String _SessionHash;
        public String SessionHash
        {
            get
            {
                return _SessionHash;
            }
            set
            {
                _SessionHash = value;
            }
        }

        private String _ErrorMsg;
        public String ErrorMsg
        {
            get
            {
                return _ErrorMsg;
            }
            set
            {
                _ErrorMsg = value;
            }
        }

        private String _DisText;
        public String DisText
        {
            get
            {
                return _DisText;
            }
            set
            {
                _DisText = value;
            }
        }

        private String _cp;
        public String Cp
        {
            get
            {
                return _cp;
            }
            set
            {
                _cp = value;
            }
        }

        private String _tw;
        public String Tw
        {
            get
            {
                return _tw;
            }
            set
            {
                _tw = value;
            }
        }

        private String _tc;
        public String Tc
        {
            get
            {
                return _tc;
            }
            set
            {
                _tc = value;
            }
        }
    }
}
