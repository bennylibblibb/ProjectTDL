using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Net; 
using System.Threading;
using GOSTS.Common;

namespace GOSTS
{
    /// <summary>
    /// Interaction logic for DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow : Window
    {
        private readonly CheckForUpdate checkForUpdate = null;
        UpdateProgressBarDelegate updatePbDelegate = null;
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);

        public DownloadWindow(CheckForUpdate checkForUpdate)
        {
            InitializeComponent();
            this.checkForUpdate = checkForUpdate;
            this.checkForUpdate.DownloadInstallerFinished += OnDownloadInstallerinished;
            this.checkForUpdate.DownloadedFinished += OnDownloadedFinished;
            this.checkForUpdate.ReplacedFinished += OnReplacedFinished;
            updatePbDelegate = new UpdateProgressBarDelegate(pbDownload.SetValue);
        }

        public void OnReplacedFinished()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + " DownloadFinished! ");

                btnInstall.IsEnabled = true;

            }));
        }

        // called after the checkForUpdate object downloaded the installer
        public void OnDownloadInstallerinished(DownloadInstallerInfo info)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                  {
                                      btnInstall.IsEnabled = true;
                                  }));
        }

        public void OnDownloadedFinished(int num)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    lbPer.Content = num + "/" + (pbDownload.Maximum - 100) / 100;

                                    this.pbDownload.Value = (num + 1) * 100;
                                    if (pbDownload.Value == pbDownload.Maximum)
                                    {
                                        // btnInstall.IsEnabled = true;
                                    }
                                }));
        }

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            // this.checkForUpdate.ReplaceFile();
            this.Close();
            System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory.ToString() + "GOSTSUpgrade.exe");

        }

        private void btnCannel_Click(object sender, RoutedEventArgs e)
        {
            this.checkForUpdate.StopThread();

            GOSTS.GOSTradeStation app = new GOSTS.GOSTradeStation();
            app.Show();

            this.Close();  
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.checkForUpdate.StopThread();
        }
    }
}

