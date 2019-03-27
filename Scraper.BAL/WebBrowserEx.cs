using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
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
        /// 当WebBrowser关闭后
        /// </summary>
        public event EventHandler WindowClosed;

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

        protected override void OnNavigating(WebBrowserNavigatingEventArgs e)
        {
            //if (e.Url.ToString().Contains("javascript:void(0)"))
            //{
            //    e.Cancel = true;
            //}

            base.OnNavigating(e);
        }

        SHDocVw.WebBrowser nativeBrowser;


        protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            if (nativeBrowser == null)
            {
                nativeBrowser = (SHDocVw.WebBrowser)this.ActiveXInstance;
                nativeBrowser.NewWindow2 += NativeBrowser_NewWindow2;
            }

            this.Document.Window.Error += Window_Error;

            var aElements = this.Document.GetElementsByTagName("a");

            foreach (HtmlElement aElement in aElements)
            {
                if (aElement.GetAttribute("href").Contains("javascript") && String.IsNullOrWhiteSpace(aElement.GetAttribute("onclick")))
                {
                    aElement.SetAttribute("onclick", aElement.GetAttribute("href"));
                    aElement.SetAttribute("href", "#");
                }
                //if (aElement.GetAttribute("target") != "")
                //{
                //    aElement.SetAttribute("target", "_self");
                //}
            }

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
            var popup = new WinBroswer();
            popup.Show(this.Parent);
            ppDisp = popup.Browser.ActiveXInstance;
        }

    }
}
