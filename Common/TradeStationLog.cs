using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;

namespace GOSTS
{
    public class TradeStationLog
    {
        private string m_FilePath;
        private string m_FileName;
        private string m_FullFilePath;
        private const string m_CR = "\r\n";
        private const string m_Delimiter = ".";
        private const string m_Separator = " ";
        private FileStream m_FS;
        private StreamWriter m_SW;
        private Encoding m_CodePage;
        //private static FileStream _FS;
        //private static StreamWriter _SW;
        private static Encoding _CodePage = Encoding.GetEncoding(950);

        public TradeStationLog()
        {
            m_CodePage = Encoding.GetEncoding(950);
        }

        public TradeStationLog(int iCodePage)
        {
            m_CodePage = Encoding.GetEncoding(iCodePage);
        }


        public string FilePath
        {
            set { m_FilePath = value; }
            get { return m_FilePath; }
        }

        public string AbsoluteFilePath
        {
            get { return m_FullFilePath; }
        }

        public string FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        public void SetFileName(int iFileType, string sType)
        {
            if (iFileType == 0)
            {
                m_FileName = DateTime.Now.ToString("yyMMdd");
                m_FileName += m_Delimiter + sType;
            }
            else if (iFileType == 1)
            {
                m_FileName = "";
                m_FileName = sType;
            }
            else
            {
                m_FileName = DateTime.Now.ToString("yyMMddHHmmss");
                m_FileName += m_Delimiter + sType;
            }

        }

        public void Open()
        {
            try
            {
                m_FullFilePath = m_FilePath + m_FileName;
                m_FS = new FileStream(m_FullFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                m_SW = new StreamWriter(m_FS, m_CodePage);
            }
            catch (Exception exp)
            {
                WriteError(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + " Open(),Error: " + exp.Message);
            }
        }

        public void Close()
        {
            m_SW.Close();
            m_FS.Close();
        }

        public void Write(string sLogMsg)
        {
            m_SW.Write(sLogMsg + m_CR);
        }

        public void Write(ArrayList msgArrayList)
        {
            string sMsg = "";
            for (int index = 0; index < msgArrayList.Count; index++)
            {
                sMsg = (string)msgArrayList[index];
                m_SW.Write(sMsg + m_CR);
            }
        }

        public void Write(ArrayList msgArrayList, int iLength)
        {
            string[] arrMsg = new string[iLength];
            for (int index = 0; index < msgArrayList.Count; index++)
            {
                arrMsg = (string[])msgArrayList[index];
                for (int iArrayIdx = 0; iArrayIdx < arrMsg.Length; iArrayIdx++)
                {
                    m_SW.Write(arrMsg[iArrayIdx] + m_Separator);
                }
                m_SW.Write(m_CR);
            }
        }

        public static void WriteLog(string sEventMsg)
        {
            if (!AppFlag.Loged) return;

            TradeStationLog m_Log = new TradeStationLog();
            lock (m_Log)
            {
                m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder;
                if (!Directory.Exists(m_Log.FilePath))
                {
                    Directory.CreateDirectory(m_Log.FilePath);
                }
                m_Log.SetFileName(0, AppFlag.Log);
                m_Log.Open();
                m_Log.Write(sEventMsg);
                m_Log.Close();
            }
        }


        public static void WriteError(string sEventMsg)
        {
            if (!AppFlag.Loged) return;

            TradeStationLog m_Log = new TradeStationLog();
            lock (m_Log)
            {
                m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.ErrorFolder;
                if (!Directory.Exists(m_Log.FilePath))
                {
                    Directory.CreateDirectory(m_Log.FilePath);
                }
                m_Log.SetFileName(0, AppFlag.Error);
                m_Log.Open();
                m_Log.Write(sEventMsg);
                m_Log.Close();
            }
        }

        public static void WriteChannelDebug(string sEventMsg)
        {
            if (GOSTradeStation.isDealer || AppFlag.isDebugNonDelaer)
            {
                TradeStationLog m_Log = new TradeStationLog();
                lock (m_Log)
                {
                    m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder;
                    if (!Directory.Exists(m_Log.FilePath))
                    {
                        Directory.CreateDirectory(m_Log.FilePath);
                    }
                    m_Log.SetFileName(0, "ChannelDebug-" + DateTime.Now.ToString("yyMMdd") + ".txt");
                    m_Log.Open();
                    m_Log.Write(sEventMsg);
                    m_Log.Close();
                }
            }
        }

        public static void WriteComDebug(string sEventMsg)
        {
            if (GOSTradeStation.isDealer || AppFlag.isDebugNonDelaer)
            {
                TradeStationLog m_Log = new TradeStationLog();
                lock (m_Log)
                {
                    m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder;
                    if (!Directory.Exists(m_Log.FilePath))
                    {
                        Directory.CreateDirectory(m_Log.FilePath);
                    }
                    m_Log.SetFileName(0, "ComDebug-" + DateTime.Now.ToString("yyMMdd") + ".txt");
                    m_Log.Open();
                    m_Log.Write(sEventMsg);
                    m_Log.Close();
                }
            }
        }

        public static void WriteParserResult(string sEventMsg)
        {
            if (GOSTradeStation.isDealer || AppFlag.isDebugNonDelaer)
            {
                TradeStationLog m_Log = new TradeStationLog();
                lock (m_Log)
                {
                    m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder;
                    if (!Directory.Exists(m_Log.FilePath))
                    {
                        Directory.CreateDirectory(m_Log.FilePath);
                    }
                    m_Log.SetFileName(0, "Parsered-" + DateTime.Now.ToString("yyMMdd") + ".txt");
                    m_Log.Open();
                    m_Log.Write(sEventMsg);
                    m_Log.Close();
                }
            }
        }

        public static void WriteForCheckMarketPrice(string sEventMsg)
        {
            if (GOSTradeStation.isDealer || AppFlag.isDebugNonDelaer)
            {
                TradeStationLog m_Log = new TradeStationLog();
                lock (m_Log)
                {
                    m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder;
                    if (!Directory.Exists(m_Log.FilePath))
                    {
                        Directory.CreateDirectory(m_Log.FilePath);
                    }
                    m_Log.SetFileName(0, "ForMarketPrice-" + DateTime.Now.ToString("yyMMdd") + ".txt");
                    m_Log.Open();
                    m_Log.Write(sEventMsg);
                    m_Log.Close();
                }
            }
        }

        public static void WriteForNotificationsOpening(string sEventMsg)
        {
            if (GOSTradeStation.isDealer || AppFlag.isDebugNonDelaer)
            {
                TradeStationLog m_Log = new TradeStationLog();
                lock (m_Log)
                {
                    m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder;
                    if (!Directory.Exists(m_Log.FilePath))
                    {
                        Directory.CreateDirectory(m_Log.FilePath);
                    }
                    m_Log.SetFileName(0, "ForNotificationLog-" + DateTime.Now.ToString("yyMMdd") + ".txt");
                    m_Log.Open();
                    m_Log.Write(sEventMsg);
                    m_Log.Close();
                }
            }
        }

        public static void WriteForHearbeatandMarginCall(string sEventMsg)
        {
            if (GOSTradeStation.isDealer || AppFlag.isDebugNonDelaer)
            {
                TradeStationLog m_Log = new TradeStationLog();
                lock (m_Log)
                {
                    m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder;
                    if (!Directory.Exists(m_Log.FilePath))
                    {
                        Directory.CreateDirectory(m_Log.FilePath);
                    }
                    m_Log.SetFileName(0, "ForHeartbeatandMCL-" + DateTime.Now.ToString("yyMMdd") + ".txt");
                    m_Log.Open();
                    m_Log.Write(sEventMsg);
                    m_Log.Close();
                }
            }
        }
        public static void WriteForRegister(string sEventMsg)
        {
            if (GOSTradeStation.isDealer || AppFlag.isDebugNonDelaer)
            { 
                TradeStationLog m_Log = new TradeStationLog();
                lock (m_Log)
                {
                    m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder;
                    if (!Directory.Exists(m_Log.FilePath))
                    {
                        Directory.CreateDirectory(m_Log.FilePath);
                    }
                    m_Log.SetFileName(0, "ForRegisterCode-" + DateTime.Now.ToString("yyMMdd") + ".txt");
                    m_Log.Open();
                    m_Log.Write(sEventMsg);
                    m_Log.Close();
                }
            }
        }
       

        public static void WriteQCLog(string sEventMsg)
        {
            if (GOSTradeStation.isDealer || AppFlag.isDebugNonDelaer)
            {
                TradeStationLog m_Log = new TradeStationLog();
                lock (m_Log)
                {
                    m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder;
                    if (!Directory.Exists(m_Log.FilePath))
                    {
                        Directory.CreateDirectory(m_Log.FilePath);
                    }
                    m_Log.SetFileName(0, "QC-" + DateTime.Now.ToString("yyMMdd") + ".txt");
                    m_Log.Open();
                    m_Log.Write(sEventMsg);
                    m_Log.Close();
                }
            }
        }

        public static void WriteTestDirectiveLog(string sEventMsg)
        {
            if (GOSTradeStation.isDealer)
            { 
                TradeStationLog m_Log = new TradeStationLog();
                lock (m_Log)
                {
                    m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder;
                    if (!Directory.Exists(m_Log.FilePath))
                    {
                        Directory.CreateDirectory(m_Log.FilePath);
                    }
                    m_Log.SetFileName(0, "LogTest-" + DateTime.Now.ToString("yyMMdd") + ".txt");
                    m_Log.Open();
                    m_Log.Write(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + ":" + sEventMsg);
                    m_Log.Close();
                }
            }
        }

        public static void WriteTestDriverLog(string sEventMsg)
        {
            if (GOSTradeStation.isDealer || AppFlag.isDebugNonDelaer)
            { 
                TradeStationLog m_Log = new TradeStationLog();
                lock (m_Log)
                {
                    m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder;
                    if (!Directory.Exists(m_Log.FilePath))
                    {
                        Directory.CreateDirectory(m_Log.FilePath);
                    }
                    m_Log.SetFileName(0, "LogTD-" + DateTime.Now.ToString("yyMMdd") + ".txt");
                    m_Log.Open();
                    m_Log.Write(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + ":" + sEventMsg);
                    m_Log.Close();
                }
            }
        }

        public static void WriteLogPerformance(string sEventMsg)
        {
            if (GOSTradeStation.isDealer|| AppFlag.isDebugNonDelaer)
            {

                TradeStationLog m_Log = new TradeStationLog();
                lock (m_Log)
                {
                    m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder;
                    if (!Directory.Exists(m_Log.FilePath))
                    {
                        Directory.CreateDirectory(m_Log.FilePath);
                    }
                    m_Log.SetFileName(0, "performance-" + DateTime.Now.ToString("yyMMdd") + ".txt");
                    m_Log.Open();
                    m_Log.Write(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + ":" + sEventMsg);
                    m_Log.Close();
                }
            }
        }

        public static void WriteLogRead(string sEventMsg)
        {
            if (GOSTradeStation.isDealer || AppFlag.isDebugNonDelaer)
            {
                TradeStationLog m_Log = new TradeStationLog();
                lock (m_Log)
                {
                    m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder;
                    if (!Directory.Exists(m_Log.FilePath))
                    {
                        Directory.CreateDirectory(m_Log.FilePath);
                    }
                    m_Log.SetFileName(0, "Read-" + DateTime.Now.ToString("yyMMdd") + ".txt");
                    m_Log.Open();
                    m_Log.Write(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + ":" + sEventMsg);
                    m_Log.Close();
                }
            }
        }
    }
}
