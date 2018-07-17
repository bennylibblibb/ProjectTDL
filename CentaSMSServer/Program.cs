using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace CentaSMSServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string name = Directory.GetCurrentDirectory().Replace(@"\", "");
            Mutex mutex = new Mutex(false, name);
            bool Running = !mutex.WaitOne(0, false);
            if (!Running)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new CentaSMSServer());
            }
            else
            {
                MessageBox.Show("This Application is Already Running！");
            }

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new CentaSMSServer());
        }
    }
}
