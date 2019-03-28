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

        int index = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void NewNavigate(string url)
        {
            CurrentProxy = null;
            IEProxy.InternetSetOption(String.Empty);

            if (Proxies == null || Proxies.Count == 0)
            {
                this.Navigate(url);
                return;
            }

            this.Navigate("about:blank");


            Task.Factory.StartNew(() =>
            {
                do
                {
                    Thread.Sleep(100);
                    if (index >= Proxies.Count)
                    {
                        break;
                    }

                    if (Proxies != null && Proxies.Count > 0)
                    {
                        CurrentProxy = Proxies[index];
                        index++;

                        if (index >= Proxies.Count)
                        {
                            CurrentProxy = null;
                            GeckoProxy.InternetSetOption("", "");
                        }
                        else
                        {
                            GeckoProxy.InternetSetOption(CurrentProxy.IP, CurrentProxy.Port);
                        }
                    }

                    this.Invoke(new Action(() =>
                    {
                        this.Navigate(url);
                    }));

                    Thread.Sleep(500);

                    //WebBrowserReadyState state = WebBrowserReadyState.Uninitialized;
                    //this.Invoke(new Action(() =>
                    //{
                    //    state = this.ReadyState;
                    //}));

                    if (!this.Url.ToString().StartsWith("res:") && this.Url.ToString() != "about:blank")//state != WebBrowserReadyState.Uninitialized && 
                    {
                        break;
                    }

                } while (true);
            });
        }

        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            e.Handled = true;
        }

    }
}
