﻿using System;
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
            if (IEProxies == null || IEProxies.Count == 0)
            {
                return;
            }

            Task.Factory.StartNew(() =>
            {
                do
                {
                    Thread.Sleep(500);

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

                    WebBrowserReadyState state = WebBrowserReadyState.Uninitialized;
                    this.Invoke(new Action(() =>
                    {
                        state = this.ReadyState;
                    }));

                    if (state != WebBrowserReadyState.Uninitialized)
                    {
                        break;
                    }

                } while (true);
            });
        }

        protected override void OnNavigating(WebBrowserNavigatingEventArgs e)
        {
            //if (e.Url.ToString().Contains("javascript:void(0)"))
            //{
            //    e.Cancel = true;
            //}

            base.OnNavigating(e);
        }

        SHDocVw.WebBrowser nativeBrowser;

        protected override void OnNewWindow(CancelEventArgs e)
        {
            this.Navigate(this.StatusText);
            e.Cancel = true;

            base.OnNewWindow(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            if (nativeBrowser == null)
            {
                nativeBrowser = (SHDocVw.WebBrowser)this.ActiveXInstance;
                nativeBrowser.NewWindow2 += NativeBrowser_NewWindow2;
            }

            this.Document.Window.Error += Window_Error;


            ////将所有的链接的目标，指向本窗体
            //foreach (HtmlElement archor in this.Document.Links)
            //{
            //    archor.SetAttribute("target", "_self");
            //}
            ////将所有的FORM的提交目标，指向本窗体
            //foreach (HtmlElement form in this.Document.Forms)
            //{
            //    form.SetAttribute("target", "_self");
            //}

            //var aElements = this.Document.GetElementsByTagName("a");

            //foreach (HtmlElement aElement in aElements)
            //{
            //    if (aElement.GetAttribute("href").Contains("javascript") && String.IsNullOrWhiteSpace(aElement.GetAttribute("onclick")))
            //    {
            //        aElement.SetAttribute("onclick", aElement.GetAttribute("href"));
            //        aElement.SetAttribute("href", "#");
            //    }
            //    //if (aElement.GetAttribute("target") != "")
            //    //{
            //    //    aElement.SetAttribute("target", "_self");
            //    //}
            //}

            //var head = this.Document.GetElementsByTagName("head")[0];

            ////创建script标签
            //HtmlElement scriptEl2 = this.Document.CreateElement("script");
            //mshtml.IHTMLScriptElement element2 = (mshtml.IHTMLScriptElement)scriptEl2.DomElement;
            //element2.src = new Uri(System.IO.Path.GetFullPath(WorkPath.ExecPath + @"js/showModalDialog.js")).ToString();
            //element2.type = "text/javascript";
            //head.AppendChild(scriptEl2);

            ////创建script标签
            //HtmlElement scriptEl = this.Document.CreateElement("script");
            //mshtml.IHTMLScriptElement element = (mshtml.IHTMLScriptElement)scriptEl.DomElement;
            //element.src = new Uri(System.IO.Path.GetFullPath(WorkPath.ExecPath + @"js/edge-support.js")).ToString();
            //element.type = "text/javascript";
            //head.AppendChild(scriptEl);

            base.OnDocumentCompleted(e);
        }

        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            e.Handled = true;
        }

        private void NativeBrowser_NewWindow2(ref object ppDisp, ref bool Cancel)
        {
            ppDisp = this.ActiveXInstance;
            //var popup = new WinBroswer();
            //popup.Show(this.Parent);
            //ppDisp = popup.Browser.ActiveXInstance;
        }

    }
}
