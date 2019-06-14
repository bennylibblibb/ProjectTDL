using System; 
using System.Configuration; 
using System.Runtime.InteropServices; 
using System.Runtime.Remoting; 
using System.Text; 
using System.Threading ; 
using RemoteService.Win32; 
using TDL.IO;

namespace RemoteObjectServer
{
    internal class RemoteObjectHost
    {
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);
        private const int SUCCESS_CODE = 100000;
        private static readonly string struiSkSvrNotify = ConfigurationManager.AppSettings["SkSvrNotify"];
        private static readonly string NotifyApplication = ConfigurationManager.AppSettings["NotifyApplication"];
        private static readonly uint uiSkSvrNotify = RegisterWindowMessage(struiSkSvrNotify);
        private static readonly string WEBSITELABEL = ConfigurationManager.AppSettings["WEBSITELABEL"];

        private static Files m_SportsLog = new Files();
        private static int iResultCode;
        [STAThread]
        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        //[DllImport("user32.dll", EntryPoint = "FindWindow")]
        //public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);


        //[DllImport("kernel32.dll")]
        //private static extern int GetLastError();
        //private static readonly uint uiSkSvrNotify = RegisterWindowMessage(AppFlag.lpString);
        //private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);


        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);
        //private const int SUCCESS_CODE = 100000;
        private static readonly string struiSkSvrNotify2 = ConfigurationManager.AppSettings["SkSvrNotify2"];
        private static readonly uint uiSkSvrNotify2 = RegisterWindowMessage(struiSkSvrNotify2);

        private static readonly string struiSkSvrNotify3 = ConfigurationManager.AppSettings["SkSvrNotify3"];
        private static readonly uint uiSkSvrNotify3 = RegisterWindowMessage(struiSkSvrNotify3);

        private static void Main(string[] args)
        {
            //Console.WriteLine(String.Concat("Start Register Remote Server ... ")); 
            //Win32Message.OnBroadcasted += new Win32Message.BroadcastEventDelegate(BroadcastReceived); 
            //Thread m_HostThread = new Thread(new ThreadStart(Run)); 
            //m_HostThread.Start(); 
            //Console.WriteLine("Press \'q\' to quit the program."); 
            //while (Console.Read() != 'q') ; 
            Console.WriteLine(String.Concat("Start Register Remote Server By [" + struiSkSvrNotify + "/" + struiSkSvrNotify2+"/" + struiSkSvrNotify3 + "] ... "));
            Win32Message.OnBroadcasted += new Win32Message.BroadcastEventDelegate(BroadcastReceived);
            Thread m_HostThread = new Thread(new ThreadStart(Run));
            m_HostThread.Start();
            Console.WriteLine("Press \'q\' to quit the program.");
            while (Console.Read() != 'q') ;
        }

        private static void Run()
        {
            try
            {
                RemotingConfiguration.Configure("RemoteObjectServer.exe.config", false);
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Register remote object from config file SUCCESS!");
                m_SportsLog.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + ConfigurationManager.AppSettings["EventPath"];
                m_SportsLog.SetFileName(0, ConfigurationManager.AppSettings["EventLog"]);
                m_SportsLog.Open();
                m_SportsLog.Write("\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Register remote object By [" + struiSkSvrNotify + "/" + struiSkSvrNotify2 +"/"+ struiSkSvrNotify3+ "] from config file SUCCESS!");
                m_SportsLog.Close();
            }
            catch (Exception exp)
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Register remote object from config file  FAILURE!");
                m_SportsLog.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + ConfigurationManager.AppSettings["ErrorPath"];
                m_SportsLog.SetFileName(0, ConfigurationManager.AppSettings["ErrorLog"]);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Register remote object from config file  FAILURE. Error:" + exp.ToString());
                m_SportsLog.Close();
            }


        }


        //public struct COPYDATASTRUCT 
        //  { 
        //      public IntPtr dwData; 
        //      public int cbData; 
        //      [MarshalAs(UnmanagedType.LPStr)] 
        //      public string lpData; 
        //  } 


        public static int BroadcastReceived(string sBroadcast)
        {
            string sResult = "";
            //	int iResultCode = 0;
            try
            {
                //int WINDOW_HANDLER = FindWindow(null, NotifyApplication);
                //if (WINDOW_HANDLER == 0)
                //{
                //    iResultCode = GetLastError();
                //    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " [FAILURE] BATCHID:" + sBroadcast + "; Please run " + NotifyApplication + " application;");
                //    throw (new Exception("GetLastError()"));
                //}
                //else
                //{
                m_SportsLog.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + ConfigurationManager.AppSettings["EventPath"];
                m_SportsLog.SetFileName(0, ConfigurationManager.AppSettings["EventLog"]);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + "  [SUCCESS] Received: " + sBroadcast);
                m_SportsLog.Close();

                if (sBroadcast.IndexOf("S") == 0)
                {
                    string path = ConfigurationManager.AppSettings["NotifyApplication"];
                    int WINDOW_HANDLER = FindWindow(null, path);
                    if (WINDOW_HANDLER != 0)
                    {
                        // int id = Convert.ToInt32(sBroadcast.Substring(1, sBroadcast.Length - 1)); 
                        int eid = Convert.ToInt32(sBroadcast.Substring(1, sBroadcast.IndexOf("-") - 1));
                        int sid = Convert.ToInt32(sBroadcast.Substring(sBroadcast.IndexOf("/") + 1, sBroadcast.Length - sBroadcast.IndexOf("/") - 1));
                        IntPtr handle = PostMessage(HWND_BROADCAST, uiSkSvrNotify2, sid, eid);
                        iResultCode = (int)handle;
                        sResult = sBroadcast.Substring(1, sBroadcast.Length - 1) + " from ScoutsWeb to ScoutsApp."; ;
                    }
                }
                else if (sBroadcast.IndexOf("D") == 0)
                {
                    string path = ConfigurationManager.AppSettings["NotifyDataOfScouts"];
                    int WINDOW_HANDLER = FindWindow(null, path);
                    if (WINDOW_HANDLER != 0)
                    {
                        int eid = Convert.ToInt32(sBroadcast.Substring(1, sBroadcast.IndexOf("-") - 1));
                        // int sid = Convert.ToInt32(sBroadcast.Substring(sBroadcast.IndexOf("/") + 1, sBroadcast.Length - sBroadcast.IndexOf("/") - 1));
                        IntPtr handle = PostMessage(HWND_BROADCAST, uiSkSvrNotify3, 0, eid);
                        iResultCode = (int)handle;
                        sResult = sBroadcast + " from ScoutsWeb to DataOfScoutsApp."; ;
                    }
                }
                else
                {
                    int id = Convert.ToInt32(sBroadcast);
                    //IntPtr handle = PostMessage((IntPtr)WINDOW_HANDLER, uiSkSvrNotify, id, 999);
                    IntPtr handle = PostMessage(HWND_BROADCAST, uiSkSvrNotify, Convert.ToInt32(DateTime.Now.ToString("HHmmssfff")), id);
                    iResultCode = (int)handle;
                    sResult = sBroadcast + " from MangoDbWeb";
                }

                if (iResultCode != 0)
                {
                    iResultCode = SUCCESS_CODE;
                }
                else
                {
                    iResultCode = GetLastError();
                    throw (new Exception("GetLastError()"));
                }
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + (sResult == "" ? " Please check client." : "  [SUCCESS] Received/sent:" + sResult));

                m_SportsLog.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + ConfigurationManager.AppSettings["EventPath"];
                m_SportsLog.SetFileName(0, ConfigurationManager.AppSettings["EventLog"]);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + (sResult==""?" Please check client.": "  [SUCCESS] Received/sent: " + sResult));
                m_SportsLog.Close();
                //      }
            }
            catch (Exception ex)
            {
                iResultCode = -1;
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " [FAILURE] Received/sent:" + sResult + " GetLastError()" + ex.ToString());
                m_SportsLog.FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + ConfigurationManager.AppSettings["ErrorPath"];
                m_SportsLog.SetFileName(0, ConfigurationManager.AppSettings["ErrorLog"]);
                m_SportsLog.Open();
                m_SportsLog.Write(DateTime.Now.ToString("HH:mm:ss") + " [FAILURE] Update by UI and Received MSG Error,GetLastError()" + GetLastError() + ex.ToString());
                m_SportsLog.Close();

            }

            return iResultCode;
        }
    }
}