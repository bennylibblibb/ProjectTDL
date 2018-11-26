using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;

namespace JC_SoccerWeb.Common
{
	public class Files
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
		private static FileStream _FS;
		private static StreamWriter _SW;
        private static Encoding _CodePage = Encoding.GetEncoding("utf-8");

		public Files()
		{
            m_CodePage = Encoding.GetEncoding("utf-8");
		}

		public Files(int iCodePage)
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
			m_FullFilePath = m_FilePath + m_FileName;
			m_FS = new FileStream(m_FullFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
			m_SW = new StreamWriter(m_FS, m_CodePage);
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
				sMsg = (string) msgArrayList[index];
				m_SW.Write(sMsg + m_CR);
			}
		}

		public void Write(ArrayList msgArrayList, int iLength)
		{
			string[] arrMsg = new string[iLength];
			for (int index = 0; index < msgArrayList.Count; index++)
			{
				arrMsg = (string[]) msgArrayList[index];
				for (int iArrayIdx = 0; iArrayIdx < arrMsg.Length; iArrayIdx++)
				{
					m_SW.Write(arrMsg[iArrayIdx] + m_Separator);
				}
				m_SW.Write(m_CR);
			}
		}

		public static void CicsWriteLog(string sEventMsg)
		{
			Files m_Log = new Files();
			lock (m_Log)
			{
				m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.EventFolder;
				m_Log.SetFileName(0, AppFlag.Log);
				m_Log.Open();
				m_Log.Write(sEventMsg);
				m_Log.Close();
			}
		}


		public static void CicsWriteError(string sEventMsg)
		{
			Files m_Log = new Files();
			lock (m_Log)
			{
				m_Log.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + AppFlag.ErrorFolder;
				m_Log.SetFileName(0, AppFlag.Error);
				m_Log.Open();
				m_Log.Write(sEventMsg);
				m_Log.Close();
			}
		}


		public static void WriteCSVFile(string FilePath, string sHead, DataSet ds, int col)
		{
			_FS = new FileStream(FilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
			_SW = new StreamWriter(_FS, _CodePage);

			string str = "";
			if (ds.Tables[0].Rows.Count > 0)
			{
				_SW.Write(sHead + "\r\n");
				foreach (DataRow r in ds.Tables[0].Rows)
				{
					str = "";
					for (int i = 0; i < col; i++)
					{
						string strconent ="\""+ r[i].ToString().Replace("\r\n", "¡ö")+"\"";
						if (i == 0)
							str = strconent;
						else if (i == col - 1)
							str = str + "," + strconent + "\r\n";
						else
							str = str + "," + strconent;
					}
					_SW.Write(str);
				}
			}

			_SW.Flush();
			_SW.Close();
			_FS.Close();

		}

	}
}