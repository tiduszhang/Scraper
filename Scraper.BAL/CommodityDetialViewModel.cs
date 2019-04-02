using Gecko.Events;
using MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper.BAL
{
    public class CommodityDetialViewModel : NotifyBaseModel
    {
        public WebBrowserEx WebBrowser { get; set; }

        public string Url { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValue(o => o.Title);
            }
            set
            {
                this.SetValue(o => o.Title, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CommodityDetialViewModel()
        {
            WebBrowser = new WebBrowserEx();
            WebBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
            WebBrowser.ProgressChanged += WebBrowser_ProgressChanged;
        }

        private void WebBrowser_ProgressChanged(object sender, Gecko.GeckoProgressEventArgs e)
        {
            SetTitle();
        }
         
        private void WebBrowser_DocumentCompleted(object sender, GeckoDocumentCompletedEventArgs e)
        {
            SetTitle();

            //开始爬数据
        }

        /// <summary>
        /// 设置标题
        /// </summary>
        private void SetTitle()
        {
            //if (String.IsNullOrWhiteSpace(WebBrowser.DocumentTitle) && WebBrowser.CurrentProxy != null)
            //{
            //    this.Title = "正在尝试使用代理：" + WebBrowser.CurrentProxy.IP + ":" + WebBrowser.CurrentProxy.Port + "。";
            //}
            //else if (WebBrowser.CurrentProxy != null)
            //{
            //    this.Title = WebBrowser.DocumentTitle + "，当前代理：" + WebBrowser.CurrentProxy.IP + ":" + WebBrowser.CurrentProxy.Port + "。";
            //}
            //else if (!String.IsNullOrWhiteSpace(WebBrowser.DocumentTitle))
            //{
            //    this.Title = WebBrowser.DocumentTitle + "，当前没有使用代理。";
            //}
            //if (WebBrowser.Url != null)
            //{
            //    this.Title += WebBrowser.Url.ToString();
            //}
            this.Title = WebBrowser.DocumentTitle + WebBrowser.Url.ToString();
        }
    }
}
