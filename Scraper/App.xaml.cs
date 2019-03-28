using Common;
using Scraper.BAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Scraper
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            Gecko.Xpcom.Initialize("Firefox");
            NoSQLHelper.DBPath = WorkPath.ExecPath;
            IEKernel.SetWebBrowserFeatures(11);
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            IEKernel.ReSetWebBroswer();
            IEProxy.InternetSetOption(String.Empty);
            base.OnExit(e);
        }

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            IEKernel.ReSetWebBroswer();
            IEProxy.InternetSetOption(String.Empty);
            base.OnSessionEnding(e);
        }
    }
}
