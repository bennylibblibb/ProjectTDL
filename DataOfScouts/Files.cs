

namespace FileLog
{
    using System;
    using System.Collections;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;

    public class Files
    {
        private static Encoding _CodePage = Encoding.GetEncoding(65001);
        //private static FileStream _FS;
        //private static StreamWriter _SW;
        private Encoding m_CodePage;
        private const string m_CR = "\r\n";
        private const string m_Delimiter = ".";
        private string m_FileName;
        private string m_FilePath;
        private FileStream m_FS;
        private string m_FullFilePath;
        private const string m_Separator = " ";
        private StreamWriter m_SW;

        public Files()
        {
            this.m_CodePage = Encoding.GetEncoding(65001);
        }

        public Files(int iCodePage)
        {
            this.m_CodePage = Encoding.GetEncoding(iCodePage);
        }

        public void Close()
        {
            this.m_SW.Close();
            this.m_FS.Close();
        }

        public void Open()
        {
            this.m_FullFilePath = this.m_FilePath + this.m_FileName;
            this.m_FS = new FileStream(this.m_FullFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            this.m_SW = new StreamWriter(this.m_FS, this.m_CodePage);
        }

        public void SetFileName(int iFileType, string sType)
        {
            if (iFileType == 0)
            {
                this.m_FileName = DateTime.Now.ToString("yyyyMMdd");
                this.m_FileName = this.m_FileName + "." + sType;
            }
            else if (iFileType == 1)
            {
                this.m_FileName = "";
                this.m_FileName = sType;
            }
            else if (iFileType == 2)
            {
                this.m_FileName = DateTime.Now.ToString("yyyyMMddHHmmfff");
                this.m_FileName = this.m_FileName + "." + sType;
            }
        }

        public void Write(ArrayList msgArrayList)
        {
            for (int i = 0; i < msgArrayList.Count; i++)
            {
                this.m_SW.Write(((string)msgArrayList[i]) + "\r\n");
            }
        }

        public void Write(string sLogMsg)
        {
            this.m_SW.Write(sLogMsg + "\r\n");
        }

        public void Write(ArrayList msgArrayList, int iLength)
        {
            string[] strArray = new string[iLength];
            for (int i = 0; i < msgArrayList.Count; i++)
            {
                strArray = (string[])msgArrayList[i];
                for (int j = 0; j < strArray.Length; j++)
                {
                    this.m_SW.Write(strArray[j] + " ");
                }
                this.m_SW.Write("\r\n");
            }
        }

        public static void WriteError(string sErrorMsg)
        {
            Files files = new Files();
            lock (files)
            {
                files.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "ErrorFolder\\";
                if (!Directory.Exists(files.FilePath))
                {
                    Directory.CreateDirectory(files.FilePath);
                }
                files.SetFileName(0, "Err.txt");
                files.Open();
                files.Write(DateTime.Now.ToString("HH:mm:ss fff ") + sErrorMsg);
                files.Close();
            }
        }

        public static void WriteLog(DateTime dateTime, string sEventMsg)
        {
            Files files = new Files();
            lock (files)
            {
                files.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "EventFolder\\";
                if (!Directory.Exists(files.FilePath))
                {
                    Directory.CreateDirectory(files.FilePath);
                } files.SetFileName(0, "Evt.txt");
                files.Open();
                files.Write(dateTime.ToString("HH:mm:ss fff ") + sEventMsg);
                files.Close();
            }
        }

        public static void WriteLog(string sEventMsg)
        {
            try
            {
                Files files = new Files();
                lock (files)
                {
                    files.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "EventFolder\\";
                    if (!Directory.Exists(files.FilePath))
                    {
                        Directory.CreateDirectory(files.FilePath);
                    } files.SetFileName(0, "Evt.txt");
                    files.Open();
                    files.Write(DateTime.Now.ToString("HH:mm:ss fff ") + sEventMsg);
                    files.Close();
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + exp.Message);
            }
        }

        public static void WriteLog(string name, string sEventMsg)
        {
            try
            {
                Files files = new Files();
                lock (files)
                {
                    files.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "EventFolder\\";
                    if (!Directory.Exists(files.FilePath))
                    {
                        Directory.CreateDirectory(files.FilePath);
                    } // files.SetFileName(0, AppFlag.Eventlog);
                    files.FileName = name + "Err.txt";
                    files.Open();
                    files.Write(DateTime.Now.ToString("HH:mm:ss fff ") + sEventMsg);
                    files.Close();
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + exp.Message);
            }
        }

        public static string[] FileToStrings(string fileName)
        {
            if (File.Exists(fileName))
            {
                StreamReader sr = new StreamReader(fileName, Encoding.Default);
                string content = sr.ReadToEnd();
                return content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            }
            return null;
        }
         public static void WriteXml(string sXmlMsg)
        {
            Files files = new Files();
            lock (files)
            {
                files.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "XmlFolder" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                if (!Directory.Exists(files.FilePath))
                {
                    Directory.CreateDirectory(files.FilePath);
                }
                files.SetFileName(2, "xml");
                files.Open();
                files.Write(sXmlMsg);
                files.Close();
            }
        }
        public static void WriteXml(string name, string sXmlMsg)
        {
            Files files = new Files();
            lock (files)
            {
                files.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "XmlFolder" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                if (!Directory.Exists(files.FilePath))
                {
                    Directory.CreateDirectory(files.FilePath);
                }
                files.FileName = name + ".xml";
                files.Open();
                files.Write(sXmlMsg);
                files.Close();
            }
        }

        public static void UpdateConfig(string AppKey, string AppValue)
        {
            XmlDocument document = new XmlDocument();
            document.Load(Application.ExecutablePath + ".config");
            XmlNode node = document.SelectSingleNode("//appSettings");
            XmlElement element = (XmlElement)node.SelectSingleNode("//add[@key=\"" + AppKey + "\"]");
            if (element != null)
            {
                element.SetAttribute("value", AppValue);
            }
            else
            {
                XmlElement newChild = document.CreateElement("add");
                newChild.SetAttribute("key", AppKey);
                newChild.SetAttribute("value", AppValue);
                node.AppendChild(newChild);
            }
            document.Save(Application.ExecutablePath + ".config");
        }

        public string AbsoluteFilePath
        {
            get
            {
                return this.m_FullFilePath;
            }
        }

        public string FileName
        {
            get
            {
                return this.m_FileName;
            }
            set
            {
                this.m_FileName = value;
            }
        }

        public string FilePath
        {
            get
            {
                return this.m_FilePath;
            }
            set
            {
                this.m_FilePath = value;
            }
        }

    }
}
