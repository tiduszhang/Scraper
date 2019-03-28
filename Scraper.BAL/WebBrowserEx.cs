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
    public class WebBrowserEx : WebBrowser
    {
        public WebBrowserEx()
        {
            AllowWebBrowserDrop = false;
            ScriptErrorsSuppressed = true;
            Dock = DockStyle.Fill;
            IEProxies = new List<IEProxy>();
        }
         
        /// <summary>
        /// 代理列表
        /// </summary> 
        public List<IEProxy> IEProxies { get; set; }

        /// <summary>
        /// 当前使用的代理
        /// </summary>
        public IEProxy CurrentIEProxy { get; set; }

        /// <summary>
        /// 当WebBrowser关闭后
        /// </summary>
        public event EventHandler WindowClosed;

        /// <summary>
        /// 当WebBrowser关闭后
        /// </summary>
        protected void OnWindowClosed(EventArgs e)
        {
            WindowClosed?.Invoke(this, e);
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x210/*WM_PARENTNOTIFY*/)
            {
                int wp = m.WParam.ToInt32();

                int X = wp & 0xFFFF;
                if (X == 0x2/*WM_DESTROY*/)//若收到该消息，引发WindowClosed事件
                {
                    OnWindowClosed(EventArgs.Empty);
                }
            }
            //else
            //{
            //    var file = @"D:\WndProc.txt";
            //    if (!System.IO.File.Exists(file))
            //    {
            //        System.IO.File.WriteAllText(file, " WndProc.log " + System.Environment.NewLine);
            //    }
            //    var log = System.IO.File.AppendText(file);
            //    log.WriteLine("log : WndProc.Msg = > { " + m.Msg + " }, WndProc.WParam = > { " + m.WParam.ToInt32() + " } ");
            //    log.Close();
            //}

            base.WndProc(ref m);
        }

        int index = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void NewNavigate(string url)
        {
            CurrentIEProxy = null;
            IEProxy.InternetSetOption(String.Empty);
            if (IEProxies == null || IEProxies.Count == 0)
            {
                this.Navigate(url, null, null, null);
                return;
            }

            this.Navigate("about:blank", null, null, null);


            Task.Factory.StartNew(() =>
            {
                do
                {
                    Thread.Sleep(100);
                    if (index >= IEProxies.Count)
                    {
                        break;
                    }

                    if (IEProxies != null && IEProxies.Count > 0)
                    {
                        CurrentIEProxy = IEProxies[index];
                        index++;

                        if (index >= IEProxies.Count)
                        {
                            CurrentIEProxy = null;
                            IEProxy.InternetSetOption(String.Empty);
                        }
                        else
                        {
                            IEProxy.InternetSetOption(CurrentIEProxy.IP + ":" + CurrentIEProxy.Port);
                        }
                    }

                    this.Invoke(new Action(() =>
                    {
                        this.Navigate(url, null, null, null);
                    }));

                    Thread.Sleep(500);

                    WebBrowserReadyState state = WebBrowserReadyState.Uninitialized;
                    this.Invoke(new Action(() =>
                    {
                        state = this.ReadyState;
                    }));

                    if (state != WebBrowserReadyState.Uninitialized && !this.Url.ToString().StartsWith("res:") && this.Url.ToString() != "about:blank")
                    {
                        break;
                    }

                } while (true);
            });
        }
         
        SHDocVw.WebBrowser nativeBrowser;

        protected override void OnNewWindow(CancelEventArgs e)
        {
            this.Navigate(this.StatusText);
            e.Cancel = true;

            base.OnNewWindow(e);
        }
         
        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            e.Handled = true;
        }
        
    }
}
