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
            Gecko.GeckoPreferences.User["gfx.font_rendering.graphite.enabled"] = true;//设置偏好：字体
            Gecko.GeckoPreferences.User["privacy.donottrackheader.enabled"] = true;//设置浏览器不被追踪
            Gecko.GeckoPreferences.User["general.useragent.override"] = "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64; rv:59.0) Gecko/20100101 Firefox/59.0";
            Gecko.GeckoPreferences.User["intl.accept_languages"] = "zh-CN,zh;q=0.9,en;q=0.8";//不设置的话默认是英文区

            NoSQLHelper.DBPath = WorkPath.ExecPath;
            //IEKernel.SetWebBrowserFeatures(11);
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //IEKernel.ReSetWebBroswer();
            GeckoProxy.InternetSetOption(String.Empty, String.Empty);
            base.OnExit(e);
        }

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            //IEKernel.ReSetWebBroswer();
            GeckoProxy.InternetSetOption(String.Empty, String.Empty);
            base.OnSessionEnding(e);
        }
    }
}
