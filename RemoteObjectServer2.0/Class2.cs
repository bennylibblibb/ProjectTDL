/////// <summary>
/////// Project created:
/////// 	30 Jan 2004
/////// Last Updated:
/////// 	30 Jan 2004, Chapman Choi
/////// Overview:
/////// 	Host for actual sports remote object
/////// Remote Object Registry information (place at exe folder):
/////// 	RemoteObjectHost.exe.config
/////// 
/////// Deployment use:
/////// 	this.Text = APPTITLE;
/////// 	this.Icon = new System.Drawing.Icon(@"..\img\app.ico");
/////// </summary>
////
////using System;
////using System.Configuration;
////using System.Runtime.InteropServices;
////using System.Runtime.Remoting;
////using System.Threading;
////using System.Windows.Forms;
////using RemoteService.Win32;
////using TDL.IO; 
////
////namespace RemoteObjectHost 
////{
////	class ROHForm : System.Windows.Forms.Form
////	{
////		private System.Windows.Forms.GroupBox ErrorGpBox;
////		private System.Windows.Forms.GroupBox EventGpBox;
////		private System.Windows.Forms.ListBox errorList;
////		private System.Windows.Forms.ListBox eventList;
////		private System.Windows.Forms.ListBox objectList;
////		private System.Windows.Forms.GroupBox ObjectGpBox;
////		/// Defining Windows UI components
////
////
////		/// Defining Constant
////		private const ulong HWND_BROADCAST = 0x000000000000FFFF;
////		private const int SUCCESS_CODE = 100000;
////		private const int MESSAGEITEMS = 30;
////		private const string APPTITLE = "Remote Object Host v1.0";
////
////		/// Defining Parameters
////		private Thread m_HostThread;
////		private Files m_Log;
////
////		/// Platform Invocation Services (Calling Windows API)
////		/// Defining extern methods in C# for Win32/COM Interoperability
////		[DllImport("kernel32.dll")]
////		private static extern int GetLastError();
////
////		[DllImport("user32.dll")]
////		public static extern uint RegisterWindowMessage(string lpString);
////
////		[DllImport("user32.dll")]
////		public static extern IntPtr PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
////
////
////		public ROHForm()
////		{
////			InitializeComponent();
////			m_Log = new Files();
////			m_Log.FilePath = ConfigurationSettings.AppSettings["EvtLogPath"];
////			m_Log.SetFileName(0, ConfigurationSettings.AppSettings["EvtLogFile"]);
////			m_Log.Open();
////			m_Log.Write(DateTime.Now.ToString("HH:mm:ss") + " " + APPTITLE + " started");
////			m_Log.Close();
////
////			Win32Message.OnBroadcasted += new Win32Message.BroadcastEventDelegate(BroadcastReceived);
////		}
////
////		// THIS METHOD IS MAINTAINED BY THE FORM DESIGNER
////		// DO NOT EDIT IT MANUALLY! YOUR CHANGES ARE LIKELY TO BE LOST
////		void InitializeComponent() 
////		{
////			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ROHForm));
////			this.ObjectGpBox = new System.Windows.Forms.GroupBox();
////			this.objectList = new System.Windows.Forms.ListBox();
////			this.eventList = new System.Windows.Forms.ListBox();
////			this.errorList = new System.Windows.Forms.ListBox();
////			this.EventGpBox = new System.Windows.Forms.GroupBox();
////			this.ErrorGpBox = new System.Windows.Forms.GroupBox();
////			this.ObjectGpBox.SuspendLayout();
////			this.EventGpBox.SuspendLayout();
////			this.ErrorGpBox.SuspendLayout();
////			this.SuspendLayout();
////			// 
////			// ObjectGpBox
////			// 
////			this.ObjectGpBox.Controls.Add(this.objectList);
////			this.ObjectGpBox.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
////			this.ObjectGpBox.Location = new System.Drawing.Point(8, 8);
////			this.ObjectGpBox.Name = "ObjectGpBox";
////			this.ObjectGpBox.Size = new System.Drawing.Size(728, 130);
////			this.ObjectGpBox.TabIndex = 0;
////			this.ObjectGpBox.TabStop = false;
////			this.ObjectGpBox.Text = "Remote Object Registered:";
////			// 
////			// objectList
////			// 
////			this.objectList.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
////			this.objectList.ItemHeight = 16;
////			this.objectList.Location = new System.Drawing.Point(8, 20);
////			this.objectList.Name = "objectList";
////			this.objectList.ScrollAlwaysVisible = true;
////			this.objectList.Size = new System.Drawing.Size(712, 100);
////			this.objectList.TabIndex = 0;
////			// 
////			// eventList
////			// 
////			this.eventList.BackColor = System.Drawing.Color.AliceBlue;
////			this.eventList.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
////			this.eventList.ForeColor = System.Drawing.Color.Navy;
////			this.eventList.ItemHeight = 16;
////			this.eventList.Location = new System.Drawing.Point(8, 24);
////			this.eventList.Name = "eventList";
////			this.eventList.ScrollAlwaysVisible = true;
////			this.eventList.Size = new System.Drawing.Size(712, 148);
////			this.eventList.TabIndex = 0;
////			// 
////			// errorList
////			// 
////			this.errorList.BackColor = System.Drawing.Color.MistyRose;
////			this.errorList.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
////			this.errorList.ForeColor = System.Drawing.Color.DarkRed;
////			this.errorList.ItemHeight = 16;
////			this.errorList.Location = new System.Drawing.Point(8, 24);
////			this.errorList.Name = "errorList";
////			this.errorList.ScrollAlwaysVisible = true;
////			this.errorList.Size = new System.Drawing.Size(712, 148);
////			this.errorList.TabIndex = 0;
////			// 
////			// EventGpBox
////			// 
////			this.EventGpBox.Controls.Add(this.eventList);
////			this.EventGpBox.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
////			this.EventGpBox.ForeColor = System.Drawing.Color.Blue;
////			this.EventGpBox.Location = new System.Drawing.Point(8, 152);
////			this.EventGpBox.Name = "EventGpBox";
////			this.EventGpBox.Size = new System.Drawing.Size(728, 180);
////			this.EventGpBox.TabIndex = 1;
////			this.EventGpBox.TabStop = false;
////			this.EventGpBox.Text = "Event List:";
////			// 
////			// ErrorGpBox
////			// 
////			this.ErrorGpBox.Controls.Add(this.errorList);
////			this.ErrorGpBox.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
////			this.ErrorGpBox.ForeColor = System.Drawing.Color.Red;
////			this.ErrorGpBox.Location = new System.Drawing.Point(8, 336);
////			this.ErrorGpBox.Name = "ErrorGpBox";
////			this.ErrorGpBox.Size = new System.Drawing.Size(728, 180);
////			this.ErrorGpBox.TabIndex = 2;
////			this.ErrorGpBox.TabStop = false;
////			this.ErrorGpBox.Text = "Error List:";
////			// 
////			// ROHForm
////			// 
////			this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
////			this.ClientSize = new System.Drawing.Size(742, 523);
////			this.Controls.Add(this.ErrorGpBox);
////			this.Controls.Add(this.EventGpBox);
////			this.Controls.Add(this.ObjectGpBox);
////			this.Icon = new System.Drawing.Icon("app.ico");
////			this.Name = "ROHForm";
////			this.Text = APPTITLE;
////			this.Closing += new System.ComponentModel.CancelEventHandler(this.ROHFormClosing);
////			this.Load += new System.EventHandler(this.ROHFormLoad);
////			this.ObjectGpBox.ResumeLayout(false);
////			this.EventGpBox.ResumeLayout(false);
////			this.ErrorGpBox.ResumeLayout(false);
////			this.ResumeLayout(false);
////		}
////
////		/// Methods to update UI list box
////		void UpdateObjectList(string sMsg) 
////		{
////			if(this.objectList.Items.Count < MESSAGEITEMS) 
////			{
////				this.objectList.Items.Add(sMsg);
////			} 
////			else 
////			{
////				this.objectList.Items.RemoveAt(0);
////				this.objectList.Items.Add(sMsg);
////			}
////		}
////
////		void UpdateEventList(string sMsg) 
////		{
////			if(this.eventList.Items.Count < MESSAGEITEMS) 
////			{
////				this.eventList.Items.Add(sMsg);
////			} 
////			else 
////			{
////				this.eventList.Items.RemoveAt(0);
////				this.eventList.Items.Add(sMsg);
////			}
////		}
////
////		void UpdateErrorList(string sMsg) 
////		{
////			if(this.errorList.Items.Count < MESSAGEITEMS) 
////			{
////				this.errorList.Items.Add(sMsg);
////			} 
////			else 
////			{
////				this.errorList.Items.RemoveAt(0);
////				this.errorList.Items.Add(sMsg);
////			}
////		}
////
////
////		[STAThread]
////		public static void Main(string[] args)
////		{
////			Application.Run(new ROHForm());
////		}
////
////		void ROHFormLoad(object sender, System.EventArgs e)
////		{
////			m_HostThread = new Thread(new ThreadStart(RegisterObject));
////			m_HostThread.Start();
////		}
////
////		void RegisterObject() 
////		{
////			try 
////			{
////				RemotingConfiguration.Configure("RemoteObjectHost.exe.config");
////				UpdateEventList(DateTime.Now.ToString("dd/MM HH:mm:ss") + " Register remote object from config file success");
////			} 
////			catch(Exception ex) 
////			{
////				m_Log.FilePath = ConfigurationSettings.AppSettings["ErrLogPath"];
////				m_Log.SetFileName(0, ConfigurationSettings.AppSettings["ErrLogFile"]);
////				m_Log.Open();
////				m_Log.Write(DateTime.Now.ToString("HH:mm:ss") + " Read config file throws exception: " + ex.ToString());
////				m_Log.Close();
////
////				UpdateErrorList(DateTime.Now.ToString("dd/MM HH:mm:ss") + " Read config file failed. Check log at " + ConfigurationSettings.AppSettings["ErrLogPath"]);
////			}
////
////			//Display registered object to UI
////			try 
////			{
////				WellKnownServiceTypeEntry[] sptEntries = RemotingConfiguration.GetRegisteredWellKnownServiceTypes();
////				for(int i = 0; i < sptEntries.Length; i++) 
////				{
////					m_Log.FilePath = ConfigurationSettings.AppSettings["EvtLogPath"];
////					m_Log.SetFileName(0, ConfigurationSettings.AppSettings["EvtLogFile"]);
////					m_Log.Open();
////					m_Log.Write(DateTime.Now.ToString("HH:mm:ss") + " Object registered: [Type]" + sptEntries[i].ObjectType + " [Uri]" + sptEntries[i].ObjectUri + " [Mode]" + sptEntries[i].Mode);
////					m_Log.Close();
////
////					UpdateObjectList(DateTime.Now.ToString("dd/MM HH:mm:ss") + " Object registered. " + sptEntries[i].ObjectType + ", " + sptEntries[i].ObjectUri + ", " + sptEntries[i].Mode);
////				}
////			} 
////			catch(Exception ex) 
////			{
////				m_Log.FilePath = ConfigurationSettings.AppSettings["ErrLogPath"];
////				m_Log.SetFileName(0, ConfigurationSettings.AppSettings["ErrLogFile"]);
////				m_Log.Open();
////				m_Log.Write(DateTime.Now.ToString("HH:mm:ss") + " Retrieve registered object throws exception: " + ex.ToString());
////				m_Log.Close();
////
////				UpdateErrorList(DateTime.Now.ToString("dd/MM HH:mm:ss") + " Retrieve registered object failed. Check log at " + ConfigurationSettings.AppSettings["ErrLogPath"]);
////			}
////		}
////
////		/// <summary>
////		/// This method handle event raised from remote object.
////		/// </summary>
////		public int BroadcastReceived(string sBroadcast) 
////		{
////			int iResultCode = 0;
////			try 
////			{
////				uint uiRegisteredStr = RegisterWindowMessage(sBroadcast);
////
////				IntPtr BroadcastHandle = (IntPtr)HWND_BROADCAST;
////				IntPtr handle = PostMessage(BroadcastHandle, uiRegisteredStr, Convert.ToInt32(DateTime.Now.ToString("HHmmssfff")), 0);
////				iResultCode = (int)handle;
////				if(iResultCode != 0) 
////				{
////					iResultCode = SUCCESS_CODE;
////					m_Log.FilePath = ConfigurationSettings.AppSettings["EvtLogPath"];
////					m_Log.SetFileName(0, ConfigurationSettings.AppSettings["EvtLogFile"]);
////					m_Log.Open();
////					m_Log.Write(DateTime.Now.ToString("HH:mm:ss") + " Broadcast message success, registered handle = " + ((int)uiRegisteredStr).ToString());
////					m_Log.Close();
////
////					UpdateEventList(DateTime.Now.ToString("dd/MM HH:mm:ss") + " Broadcast message success, registered handle = " + ((int)uiRegisteredStr).ToString());
////				} 
////				else 
////				{
////					iResultCode = GetLastError();
////					m_Log.FilePath = ConfigurationSettings.AppSettings["ErrLogPath"];
////					m_Log.SetFileName(0, ConfigurationSettings.AppSettings["ErrLogFile"]);
////					m_Log.Open();
////					m_Log.Write(DateTime.Now.ToString("HH:mm:ss") + " Broadcast message failed, error code = " + iResultCode.ToString());
////					m_Log.Close();
////
////					UpdateErrorList(DateTime.Now.ToString("dd/MM HH:mm:ss") + " Broadcast message failed, error code = " + iResultCode.ToString());
////				}
////			} 
////			catch(Exception ex) 
////			{
////				iResultCode = -1;
////				m_Log.FilePath = ConfigurationSettings.AppSettings["ErrLogPath"];
////				m_Log.SetFileName(0, ConfigurationSettings.AppSettings["ErrLogFile"]);
////				m_Log.Open();
////				m_Log.Write(DateTime.Now.ToString("HH:mm:ss") + " Broadcast message throws exception: " + ex.ToString());
////				m_Log.Close();
////
////				UpdateErrorList(DateTime.Now.ToString("dd/MM HH:mm:ss") + " Broadcast message failed, Check log at " + ConfigurationSettings.AppSettings["ErrLogPath"]);
////			}
////
////			return iResultCode;
////		}
////
////		void ROHFormClosing(object sender, System.ComponentModel.CancelEventArgs e)
////		{
////			if(MessageBox.Show("Confirm to exit?", APPTITLE, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) ==  DialogResult.OK) 
////			{
////				m_HostThread.Join(0);
////
////				m_Log.FilePath = ConfigurationSettings.AppSettings["EvtLogPath"];
////				m_Log.SetFileName(0, ConfigurationSettings.AppSettings["EvtLogFile"]);
////				m_Log.Open();
////				m_Log.Write(DateTime.Now.ToString("HH:mm:ss") + " " + APPTITLE + " closed");
////				m_Log.Close();
////				Environment.Exit(0);
////			} 
////			else 
////			{
////				e.Cancel = true;
////			}		
////		}
////	}
////}
