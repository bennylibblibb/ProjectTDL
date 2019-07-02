using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GOSTS.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;  
using System.Collections.Specialized;

namespace GOSTS.Common
{
    public class BaseNotification
    {
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
                if (value.IndexOf("lv=") > -1)
                {
                    string[] allItem = value.Split(new Char[] { '&' });
                    foreach (string str in allItem)
                    {
                        if (str.IndexOf("lv=") > -1)
                        {
                            _Level = Convert.ToInt32(str.Substring(3, str.Length - 3));
                            break;
                        }
                    }
                }
                else
                {
                    _Level = -1;
                }
            }
        }

        private int _Level;
        public int Level
        {
            get
            {
                return _Level;
            }
            set
            {

                _Level = value;
            }
        }

        private String _Message;
        public String Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
            }
        }

        private String _AllMessage;
        public String AllMessage
        {
            get
            {
                return _AllMessage;
            }
            set
            {
                _AllMessage = value;
            }
        }

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
    }

    public class Notification : BaseNotification
    {
        private String _SeqNo;
        public String SeqNo
        {
            get
            {
                return _SeqNo;
            }
            set
            {
                _SeqNo = value;
            }
        }

        private int _Notify_Code;
        public int Notify_Code
        {
            get
            {
                return _Notify_Code;
            }
            set
            {
                _Notify_Code = value;
            }

        }

    }

    public class TableNotification : BaseNotification
    {
        private int _TableCode;
        public int TableCode
        {
            get
            {
                return _TableCode;
            }
            set
            {
                _TableCode = value;
            }
        }

        private String _VersionNo;
        public String VersionNo
        {
            get
            {
                return _VersionNo;
            }
            set
            {
                _VersionNo = value;
            }
        }

    }

    public class NotificationQueue<T> : INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public NotificationQueue()
        {
            queue = new Queue<T>();
            lockObj = new object();
        }

        Queue<T> queue;
        object lockObj;

        public T Dequeue()
        {
            lock (lockObj)
            {
                return queue.Dequeue();
            }
        }

        public void Enqueue(T item)
        {
            lock (lockObj)
            {
                queue.Enqueue(item);
            }
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public bool IsEmpty()
        {
            lock (lockObj)
            {
                return queue.Count == 0;
            }
        }

    }

    public class TradeStationNotification
    {
        private Notification notification = new Notification();
        private TableNotification tableNotification = new TableNotification();

        public Notification GetNotification(string[] items)
        {
            if (items != null && items.Count() > 0)
            {
                string dateTime = TradeStationTools.getDateTimeFromUnixTime(items[0].ToString()).ToString();
                notification.Datetime = dateTime;
                notification.UserID = items[1];
                notification.Acc_no = items[2];
                notification.Rinfo = items[3];
                if (items[3] != null && items[3].IndexOf("ak=1") > -1)
                {
                    if (items[3].IndexOf("sn=") > -1)
                    {
                        string[] allItem = items[3].Split(new Char[] { '&' });
                        foreach (string str in allItem)
                        {
                            if (str.IndexOf("sn=") > -1)
                            {
                                notification.SeqNo = str.Substring(3, str.Length - 3);
                                break;
                            }
                        }
                    }
                    else
                    {
                        notification.SeqNo = null;
                    }
                }
                else
                {
                    notification.SeqNo = null;
                }
                notification.Notify_Code = TradeStationTools.ConvertToInt(items[4]);
                notification.Message = (items[3].Trim().IndexOf("mg=b64") > -1) ? TradeStationTools.Base64StringToString(items[5].Trim()) : items[5].Trim();//items[5].Trim();
                notification.AllMessage = dateTime + "\r\n Acc_No:" + items[2] + " " + notification.Message;// items[5].Trim();
            }
            return notification;
        }

        public TableNotification GetTableNotification(string[] items)
        {
            if (items != null && items.Count() > 0)
            {
                string dateTime = TradeStationTools.getDateTimeFromUnixTime(items[0].ToString()).ToString();
                tableNotification.Datetime = dateTime;
                tableNotification.UserID = items[1];
                tableNotification.Acc_no = items[2];
                tableNotification.Rinfo = items[3];

                tableNotification.TableCode = TradeStationTools.ConvertToInt(items[4]);
                tableNotification.VersionNo = items[5];
                tableNotification.Message = (items[3].Trim().IndexOf("mg=b64") > -1) ? TradeStationTools.Base64StringToString(items[6].Trim()) : items[6].Trim(); ;
                tableNotification.AllMessage = dateTime + "\r\n Acc_No:" + items[2] + " " + tableNotification.Message;// items[6].Trim();
            }
            return tableNotification;
        }
    }
}