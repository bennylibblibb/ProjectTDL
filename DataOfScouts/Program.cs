using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace DataOfScouts
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
                Application.Run(new DataOfScouts());
            }
            else
            {
                MessageBox.Show("This Application is Already Running!");
            }
        }
    }
}
