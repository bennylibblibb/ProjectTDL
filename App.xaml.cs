using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Threading; 
using System.Reflection; 
using System.Xml;
using System.Net; 
using System.IO; 
using GOSTS.Common;

namespace GOSTS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        public App()
        {
            this.checkForUpdate = new CheckForUpdate();
            this.checkForUpdate.CheckForUpdateFinished += OnCheckForUpdateFinished;
            // this.checkForUpdate.DownloadInstallerFinished += OnDownloadInstallerinished;
            GosCulture.CultureHelper.ChangeLanguage(AppFlag.DefaultLanguage);
        }

        DownloadWindow win;

        private readonly CheckForUpdate checkForUpdate = null;

        string UpdateAlertTitle
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("VersionUpdateAlertTitle");
            }
        }

        string UpdateMsg
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("VersionUpdateMsg");
            }
        }

        string UpdateRequiredMsg
        {
            get
            {
                return GOSTS.GosCulture.CultureHelper.GetString("VersionUpdateRequiredMsg");
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "Update step 1  - start check for update!");
            this.checkForUpdate.OnCheckForUpdate();
            // this.checkForUpdate.CheckForUpdateFunction();
        }

        public bool OnCheckForUpdateFinished(DownloadedVersionInfo versionInfo, AppInfo appInfo)
        {
            string str = "";

            if ((versionInfo.error) || (versionInfo.updateFileUrl.Length == 0) || (versionInfo.latestVersion == null))
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                               {
                                   GOSTradeStation win = new GOSTradeStation();
                                   win.Show();
                                   this.checkForUpdate.StopThread();
                               }));
                return false;
            }

            // compare the current version with the downloaded version number 
            if (appInfo.version.CompareTo(versionInfo.latestVersion) >= 0)
            {
                // no new version
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                 {
                                     GOSTradeStation win = new GOSTradeStation();
                                     win.Show();

                                     this.checkForUpdate.StopThread();
                                 }));
                return false;
            }

            // compare the current version with MinimumRequiredVersion 
            if (appInfo.version.CompareTo(versionInfo.MinimumRequiredVersion) < 0)
            {
                str = String.Format(UpdateRequiredMsg, appInfo.version, versionInfo.MinimumRequiredVersion);
                MessageBox.Show(str, UpdateAlertTitle, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);

                // Required  Minimum Version
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    GOSTradeStation win = new GOSTradeStation();
                    win.Show();

                    this.checkForUpdate.StopThread();
                }));
                return false;
            }

            // new version found, ask the user if he wants to download the installer
            str = String.Format(UpdateMsg, appInfo.version, versionInfo.latestVersion);
            if (MessageBox.Show(str,
                            UpdateAlertTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {

                TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "Update step 2  -found new version and download to update! ");

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    win = new DownloadWindow(checkForUpdate);

                    win.pbDownload.Minimum = 0;
                    win.pbDownload.Maximum = (versionInfo.updateFileList.Count + 1) * 100;
                    win.pbDownload.Value = 100;
                    win.lbPer.Content = "0/" + versionInfo.updateFileList.Count;
                    win.Show();

                }));
                return true;
            }
            else
            {
                TradeStationLog.WriteLog(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff ") + "Update step 2  -found new version and not update! ");
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    GOSTradeStation win = new GOSTradeStation();
                    win.Show();
                    this.checkForUpdate.StopThread();
                }));
                return false;
            }
        }
    }
}