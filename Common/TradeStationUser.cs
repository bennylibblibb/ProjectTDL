using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Data; 

namespace  GOSTS.Common
{

    public class UserData : LoginResp
    {
        private string _Pwd;
        public string Pwd
        {
            get
            {
                return _Pwd;
            }
            set
            {
                _Pwd = value;
            }
        }
        private string _AccountID;
        public string AccountID
        {
            get
            {
                return _AccountID;
            }
            set
            {
                _AccountID = value;
            }
        }


        private string _defaultUserID;
        public string DefaultUserID
        {
            get
            {
                return _defaultUserID;
            }
            set
            {
                _defaultUserID = value;
            }
        }
        private bool _IsDealer;
        public bool IsDealer
        {
            get
            {
                return _IsDealer;
            }
            set
            {
                _IsDealer = value;
            }
        }

        private List<int> _lsAccountID; 
        public List<int> LsAccountID
        {
            get
            {
                return _lsAccountID;
            }
            set
            {
                _lsAccountID = value;
            }
        }

        private DataTable _AccountTable;
        public DataTable AccountTable
        {
            get
            {
                return _AccountTable;
            }
            set
            {
                _AccountTable = value;
            }
        }
    }

    class TradeStationUser
    {
        public static UserData GetLsAccountID(string userID)
        {
            UserData data = new UserData();
            data.DefaultUserID = userID;


            List<int> lsID = new List<int>();
            for (int i = 1000; i < 1020; i++)
            {
                lsID.Add(i);
            }

            data.LsAccountID = lsID;


            return data;
        }

        public static UserData GetAccountTable(string userID)
        {
            UserData data = new UserData();
            data.DefaultUserID = userID;
            data.IsDealer = true;

            if (GOSTradeStation.userData.AccountTable == null)
            {
               // TradeStationSend.Send(cmdClient.getAccList);
            }
            else
            {
                data.AccountTable = GOSTradeStation.userData.AccountTable;
            }
            return data;
        }

    }
}
