using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoutDBProvider
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string name = Directory.GetCurrentDirectory().Replace(@"\", "");
            Mutex mutex = new Mutex(false, name);
            if (mutex.WaitOne(0, false))
            {
                Application.Run(new ScoutDBProvider());
            }
            else
            {
                MessageBox.Show("This Application is Already Running!");
            }
        }
    }
}
