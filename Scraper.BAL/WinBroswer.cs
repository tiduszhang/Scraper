using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scraper.BAL
{
    public partial class WinBroswer : Form
    {
        public WinBroswer()
        {
            InitializeComponent();
            this.Browser.DocumentCompleted += Browser_DocumentCompleted;
        }

        /// <summary>
        /// 获取或设置访问url
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 代理列表
        /// </summary>
        public List<IEProxy> IEProxies { get; set; }

        int index = 0;

        protected override void OnShown(EventArgs e)
        {
            if (IEProxies != null && IEProxies.Count > 0)
            {
                index++;
                IEProxy.InternetSetOption(IEProxies[index].IP + ":" + IEProxies[index].Port);
                this.Text = "正在使用代理：" + IEProxies[index].IP + ":" + IEProxies[index].Port;
            }
            else
            {
                IEProxy.InternetSetOption(String.Empty);
            }

            if (!String.IsNullOrWhiteSpace(URL))
            {
                this.Browser.Navigate(URL, null, null, null);
            }

            Task.Factory.StartNew(() =>
            {
                do
                {
                    Thread.Sleep(500);

                    WebBrowserReadyState state = WebBrowserReadyState.Uninitialized;
                    this.Invoke(new Action(() =>
                    {
                        state = this.Browser.ReadyState;
                    }));

                    if (state != WebBrowserReadyState.Uninitialized)
                    {
                        break;
                    }
                    if (IEProxies != null && IEProxies.Count > 0)
                    {
                        index++;
                        IEProxy.InternetSetOption(IEProxies[index].IP + ":" + IEProxies[index].Port);
                        this.Invoke(new Action(() =>
                        {
                            this.Text = "正在使用代理：" + IEProxies[index].IP + ":" + IEProxies[index].Port;
                        }));

                        if (index >= IEProxies.Count)
                        {
                            index = 0;
                        }
                    }

                    this.Invoke(new Action(() =>
                    {
                        this.Browser.Navigate(URL, null, null, null);
                    }));
                } while (true);

            });


            base.OnShown(e);
        }

        /// <summary>
        /// 窗口加载完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (IEProxies != null && IEProxies.Count > 0)
            {
                this.Text = this.Browser.DocumentTitle + "    正在使用代理：" + IEProxies[index].IP + ":" + IEProxies[index].Port;
            }
            else
            {
                this.Text = this.Browser.DocumentTitle;
            }

            DocumentCompleted?.Invoke(this.Browser.Document);
        }

        /// <summary>
        /// 网页加载完成时触发
        /// </summary>
        public Action<HtmlDocument> DocumentCompleted { get; set; }
    }
}
