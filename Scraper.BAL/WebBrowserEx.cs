using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gecko;

namespace Scraper.BAL
{
    /// <summary>
    /// 增强型浏览器
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    [System.ComponentModel.DesignerCategory("Code")]
    //[System.ComponentModel.Designer("Code")]
    public class WebBrowserEx : Gecko.GeckoWebBrowser
    {
        public WebBrowserEx()
        {
            //AllowWebBrowserDrop = false;
            //ScriptErrorsSuppressed = true;
            Dock = DockStyle.Fill;
            NoDefaultContextMenu = false;
            Proxies = new List<GeckoProxy>();
        }

        /// <summary>
        /// 代理列表
        /// </summary> 
        public List<GeckoProxy> Proxies { get; set; }

        /// <summary>
        /// 当前使用的代理
        /// </summary>
        public GeckoProxy CurrentProxy { get; set; }

        /// <summary>
        /// 新建窗口
        /// </summary>
        public Action<WebBrowserEx> NewWindowAction { get; set; }

        //int index = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void NewNavigate(string url)
        {
            this.Navigate(url);
            return;

            //代理无法生效（暂时不使用代理功能）
            //if (Proxies == null || Proxies.Count == 0 || index >= Proxies.Count)
            //{
            //    CurrentProxy = null;
            //    GeckoProxy.InternetSetOption(String.Empty, String.Empty);
            //    this.Navigate(url);
            //}
            //else
            //{
            //    CurrentProxy = Proxies[index];
            //    index++;
            //    GeckoProxy.InternetSetOption(CurrentProxy.IP, CurrentProxy.Port);
            //    this.Navigate(url, Gecko.GeckoLoadFlags.BypassProxy);
            //}


            //this.Navigate("about:blank");

            //Task.Factory.StartNew(() =>
            //{
            //    do
            //    {
            //        Thread.Sleep(100);
            //        if (index >= Proxies.Count)
            //        {
            //            break;
            //        }

            //        if (Proxies != null && Proxies.Count > 0)
            //        {
            //            CurrentProxy = Proxies[index];
            //            index++;

            //            if (index >= Proxies.Count)
            //            {
            //                CurrentProxy = null;
            //                GeckoProxy.InternetSetOption("", "");
            //            }
            //            else
            //            {
            //                GeckoProxy.InternetSetOption(CurrentProxy.IP, CurrentProxy.Port);
            //            }
            //        }

            //        this.Invoke(new Action(() =>
            //        {
            //            this.Navigate(url);
            //        }));

            //        Thread.Sleep(500);

            //        //WebBrowserReadyState state = WebBrowserReadyState.Uninitialized;
            //        //this.Invoke(new Action(() =>
            //        //{
            //        //    state = this.ReadyState;
            //        //}));

            //        if (!this.Url.ToString().StartsWith("res:") && this.Url.ToString() != "about:blank")//state != WebBrowserReadyState.Uninitialized && 
            //        {
            //            break;
            //        }

            //    } while (true);
            //});
        }

        protected override void OnDocumentTitleChanged(EventArgs e)
        {
            base.OnDocumentTitleChanged(e);
        }

        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            e.Handled = true;
        }

        private WebBrowserEx ChildWebBrowser { get; set; }

        protected override void OnCreateWindow(GeckoCreateWindowEventArgs e)
        {
            if (ChildWebBrowser == null)
            {
                ChildWebBrowser = new WebBrowserEx();
            }
            e.WebBrowser = ChildWebBrowser;
            if (NewWindowAction != null)
            {
                NewWindowAction(ChildWebBrowser);
            }
            base.OnCreateWindow(e);
        }


    }
}
