namespace TDL.DB
{
    using System;
    using System.Data;
    using FirebirdSql.Data.FirebirdClient;

    public class DBManagerFB : IDisposable
    {
        private string m_ConnStr;
        private FbCommand m_DBCmd;
        private FbConnection m_DBConn;
        private FbDataReader m_DBReader;
        private DataSet m_DS;
        private FbDataAdapter m_OleDbAdapter;
        private int m_UpdateCount = 0;

        public void Close()
        {
            if (this.m_DBReader != null)
            {
                this.m_DBReader.Close();
            }
            if (this.m_DBConn == null)
            {
                throw new Exception("Connection had not initialize");
            }
            this.m_DBConn.Close();
        }

        public void Dispose()
        {
            this.Close();
            if (this.m_DBConn != null)
            {
                this.m_DBConn.Dispose();
                this.m_DBConn = null;
            }
            if (this.m_DBCmd != null)
            {
                this.m_DBCmd.Dispose();
                this.m_DBCmd = null;
            }
            this.m_DBReader = null;
            GC.SuppressFinalize(this);
        }

        public int ExecuteNonQuery(string sSQL)
        {
            if (this.m_DBConn == null)
            {
                this.GetConnected();
            }
            if (this.m_DBCmd == null)
            {
                this.GetCommand();
            }
            this.m_DBCmd.CommandText = sSQL;
            this.m_DBConn.Open();
            this.m_UpdateCount = this.m_DBCmd.ExecuteNonQuery();
            this.m_DBConn.Close();
            return this.m_UpdateCount;
        }

        public FbDataReader ExecuteQuery(string sSQL)
        {
            if (this.m_DBConn == null)
            {
                this.GetConnected();
            }
            if (this.m_DBCmd == null)
            {
                this.GetCommand();
            }
            this.m_DBCmd.CommandText = sSQL;
            this.m_DBConn.Open();
            this.m_DBReader = this.m_DBCmd.ExecuteReader();
            return this.m_DBReader;
        }

        public DataSet ExecuteQueryDataSet(string sSQL)
        {
            if (this.m_DBConn == null)
            {
                this.GetConnected();
            }
            if (this.m_DBCmd == null)
            {
                this.GetCommand();
            }
            if (this.m_OleDbAdapter == null)
            {
                this.GetAdapter();
            }
            this.m_DBCmd.CommandText = sSQL;
            this.m_DS = new DataSet();
            this.m_OleDbAdapter.SelectCommand = this.m_DBCmd;
            this.m_OleDbAdapter.Fill(this.m_DS);
            return this.m_DS;
        }

        public string ExecuteQueryString(string sSQL)
        {
            string str = "";
            if (this.m_DBConn == null)
            {
                this.GetConnected();
            }
            if (this.m_DBCmd == null)
            {
                this.GetCommand();
            }
            this.m_DBCmd.CommandText = sSQL;
            this.m_DBConn.Open();
            this.m_DBReader = this.m_DBCmd.ExecuteReader();
            if (this.m_DBReader.Read() && !this.m_DBReader.IsDBNull(0))
            {
                str = this.m_DBReader.GetString(0).Trim();
            }
            this.m_DBReader.Close();
            this.m_DBConn.Close();
            return str;
        }

        public int ExecuteScalar(string sSQL)
        {
            if (this.m_DBConn == null)
            {
                this.GetConnected();
            }
            if (this.m_DBCmd == null)
            {
                this.GetCommand();
            }
            this.m_DBCmd.CommandText = sSQL;
            this.m_DBCmd.Connection = this.m_DBConn;
            this.m_DBConn.Open();
            //int num = (int) this.m_DBCmd.ExecuteScalar();
            int num =Convert.ToInt32(this.m_DBCmd.ExecuteScalar());
            this.m_DBConn.Close();
            return num;
        }

        private void GetAdapter()
        {
            this.m_OleDbAdapter = new FbDataAdapter();
        }

        private void GetCommand()
        {
            this.m_DBCmd = new FbCommand(null, this.m_DBConn);
        }

        private void GetConnected()
        {
            this.m_DBConn = new FbConnection(this.m_ConnStr);
        }

        public string ConnectionString
        {
            get
            {
                return this.m_ConnStr;
            }
            set
            {
                this.m_ConnStr = value;
            }
        }
    }
}

